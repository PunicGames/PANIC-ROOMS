using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = System.Random;
using Graphs;
using Unity.AI.Navigation;
using Unity.VisualScripting.Dependencies.NCalc;

[System.Serializable]
public struct PanicRoom
{
    public Vector3Int size;
    public GameObject prefab;
}

public class Generator3D : MonoBehaviour {

    #region Attributes
    enum CellType {
        None,
        Room,
        Hallway,
        Stairs
    }

    
    Random random;
    public string seed;

    [SerializeField] Vector3Int size;

    [SerializeField, Range(0,1)] double additionalRoomChance = 0.5;
    [SerializeField, Range(0,1)] double additionalHallwayChance = 0.1;
    
    // Prefabs
    [SerializeField] List<PanicRoom> panicRooms, additionalRooms;
    [SerializeField] List<GameObject> instantiatedPanicRooms;
    [SerializeField] GameObject hallwayPrefab;
    [SerializeField] GameObject upStairwayPrefab, downStairwayPrefab;

    private readonly Vector3Int[] testPos = { Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back };
    private readonly int MAX_TRIES = 32;

    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    Dictionary<Vector3Int, Room> roomsDictionary;
    Dictionary<Room, List<(Vector3Int, bool)>> doorPlacement;
    List<(Vector3Int, Quaternion, bool)> stairsToPlace;

    private NavMeshSurface navMeshSurface;

    private GameManager gameManager;

    #endregion

    #region MonoBehaviour
    void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        gameManager = GameManager.Instance;
        instantiatedPanicRooms = new List<GameObject>();

        if (string.IsNullOrEmpty(seed))
        {
            string randomString = RandomSeed.GenerateRandomAlphanumericString(8);
            int seedHash = randomString.GetHashCode();
            
            
            UnityEngine.Debug.Log("SEED: " + randomString + "\n" + seedHash);
            random = new Random(seedHash);
        }
        else
        {
            random = new Random(seed.GetHashCode());
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        bool success = false;
        int tries = 0;
        while (!success && tries < MAX_TRIES)
        {
            try
            {
                grid = new Grid3D<CellType>(size, Vector3Int.zero);
                rooms = new List<Room>();
                roomsDictionary = new Dictionary<Vector3Int, Room>();
                doorPlacement = new Dictionary<Room, List<(Vector3Int, bool)>>();
                stairsToPlace = new List<(Vector3Int, Quaternion, bool)>();

                SetRooms();
                Triangulate();
                CreateHallways();
                PathfindHallways();
                BuildDungeon();

                navMeshSurface?.BuildNavMesh();
                success = true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                tries++;
            }
        }
        stopwatch.Stop();

        GameManager.RoomsCreated(rooms);

        UnityEngine.Debug.Log("Tries: " + tries);
        UnityEngine.Debug.Log("Tiempo transcurrido: "+ stopwatch.ElapsedMilliseconds + " milisegundos");
    }

    #endregion

    #region Methods
    void BuildDungeon()
    {
        PlaceAllRooms();
        PlaceAllHallways();
        PlaceAllStairs();
    }

    public void SetRooms()
    {
        foreach (PanicRoom room in panicRooms)
        {
            SetRoomAtRandomLocation(room);
        }

        foreach (PanicRoom room in additionalRooms)
        {
            if (ShouldPlaceRoom())
            {
                SetRoomAtRandomLocation(room);
            }
        }
    }

    #region ShouldPlace
    bool ShouldPlace(double value)
    {
        return random.NextDouble() < value;
    }

    bool ShouldPlaceRoom()
    {
        return ShouldPlace(additionalRoomChance);
    }

    bool ShouldPlaceHallway()
    {
        return ShouldPlace(additionalHallwayChance);
    }
    #endregion

    void SetRoomAtRandomLocation(PanicRoom room)
    {
        Vector3Int roomSize = room.size;
        while (true) 
        { 
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize, room.prefab);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2), null);

            foreach (var r in rooms)
            {
                if (Room.Intersect(r, buffer))
                {
                    add = false;
                    break;
                }
            }
        
            if (newRoom.Bounds.xMin < 0 || newRoom.Bounds.xMax >= size.x
                || newRoom.Bounds.yMin < 0 || newRoom.Bounds.yMax >= size.y
                || newRoom.Bounds.zMin < 0 || newRoom.Bounds.zMax >= size.z)
            {
                add = false;
            }
        
            if (add)
            {
                rooms.Add(newRoom);
                doorPlacement[newRoom] = new List<(Vector3Int, bool)>();

                foreach (var pos in newRoom.Bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                    roomsDictionary[pos] = newRoom;
                }
                return;
            }
        }

    }

    //  vvvv AS ORIGINAL vvvv
    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector3)room.Bounds.position + ((Vector3)room.Bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) 
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);

        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            
            if (ShouldPlaceHallway())
            {
                selectedEdges.Add(edge);
            }
        }
    }
    //  ^^^^ AS ORIGINAL ^^^^

    void PathfindHallways() 
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);
        List<Vector3Int> hallways = new List<Vector3Int>();

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.Bounds.center;
            var endPosf = endRoom.Bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            //bool pathFound = false;

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) =>
            {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0)
                {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs)
                    {
                        return pathCost;
                    }
                    else if (grid[b.Position] == CellType.Room)
                    {
                        pathCost.cost += 5;
                    }
                    else if (grid[b.Position] == CellType.None)
                    {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                }
                else
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None)
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0)
                        {
                            Quaternion orientation = Quaternion.identity;
                            bool up = delta.y > 0;

                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int vOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int hOffset = new Vector3Int(xDir, 0, zDir);

                            grid[prev + hOffset] = CellType.Stairs;
                            grid[prev + hOffset * 2] = CellType.Stairs;
                            grid[prev + vOffset + hOffset] = CellType.Stairs;
                            grid[prev + vOffset + hOffset * 2] = CellType.Stairs;

                            Vector3Int horizontalOffset = Vector3Int.zero;

                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.z))
                            {
                                orientation = delta.x > 0 ? Quaternion.Euler(0, 90, 0) : Quaternion.Euler(0, 270, 0);

                                if (delta.x > 0)
                                {
                                    orientation = Quaternion.Euler(0, 90, 0);
                                    horizontalOffset += new Vector3Int(1, 0, 1);

                                }
                                else
                                {
                                    orientation = Quaternion.Euler(0, 270, 0);
                                }
                            }
                            else
                            {
                                if (delta.z < 0)
                                {
                                    orientation = Quaternion.Euler(0, 180, 0);
                                    horizontalOffset += new Vector3Int(1, 0, 0);
                                }
                                else
                                {
                                    orientation = Quaternion.identity;
                                    horizontalOffset += new Vector3Int(0, 0, 1);
                                }
                            }

                            stairsToPlace.Add((prev + horizontalOffset, orientation, up));
                            //PlaceStairs(prev + horizontalOffset, orientation, up); // Usa la orientación calculada
                        }
                    }
                }

                List<Vector3Int> transitionHallways = new List<Vector3Int>();

                if (path.Count > 1)
                {
                    for (int i = 1; i < path.Count; i++)
                    {
                        Vector3Int currentPos = path[i];
                        Vector3Int previousPos = path[i - 1];

                        // Chequear si hubo un cambio de Room a Hallway o de Hallway a Room
                        if ((grid[currentPos] == CellType.Hallway && grid[previousPos] == CellType.Room) ||
                            (grid[currentPos] == CellType.Room && grid[previousPos] == CellType.Hallway))
                        {
                            // Si es un Hallway y la celda anterior o la siguiente es una Room, guardarlo
                            if (grid[currentPos] == CellType.Hallway)
                            {
                                transitionHallways.Add(currentPos);
                            }
                            // También queremos guardar el Hallway si la celda actual es una Room pero la anterior era un Hallway
                            else if (grid[previousPos] == CellType.Hallway)
                            {
                                transitionHallways.Add(previousPos);
                            }
                        }
                    }
                }


                foreach (var pos in transitionHallways)
                {
                    CheckRoom(pos);
                }

            }
            else 
                throw new System.Exception("No path found");
            
        }
        
        foreach (var r in rooms)
        {
            if (!r.Connected)
            {
                throw new System.Exception("Room not connected");
            }
        }
    }
    #region PlacePrefabs

    void CheckRoom(Vector3Int position)
    {
        for (int i = 0; i < testPos.Length; i++)
        {
            Vector3Int move = testPos[i];

            if (roomsDictionary.TryGetValue(position + move, out var room))
            {
                room.Connected = true;
                doorPlacement[room].Add((position + move, i > 1));

                //room.GetComponent<WallController>()?.MakeDoor(position + move, i > 1);
            }

        }
    }
    GameObject PlaceRoom(Vector3Int location, GameObject obj)
    {
        GameObject go = Instantiate(obj, location, Quaternion.identity);
        go.transform.parent = transform;
        instantiatedPanicRooms.Add(go);
        return go;
    }

    void PlaceAllRooms()
    {
        foreach (var room in rooms)
        {
            var roomObj = PlaceRoom(room.Bounds.position, room.Prefab);

            foreach (var door in doorPlacement[room])
            {
                roomObj.GetComponent<WallController>()?.MakeDoor(door.Item1, door.Item2);
            }
        }
    }

    void PlaceHallway(Vector3Int location)
    {
        GameObject go = Instantiate(hallwayPrefab, location, Quaternion.identity);
        go.transform.parent = transform;
        for (int i = 0; i < testPos.Length; i++)
        {
            Vector3Int move = testPos[i];
            if (grid[location + move] != CellType.None)
            {
                go.GetComponent<HallwayWallController>().MakeWalls(move);
            }
        }
    }

    void PlaceAllHallways()
    {
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
                for (int k = 0; k < size.z; k++)
                {
                    Vector3Int pos = new Vector3Int(i, j, k);
                    if (grid[pos] == CellType.Hallway)
                    {
                        PlaceHallway(pos);
                    }
                }
    }

    void PlaceAllStairs()
    {
        foreach (var stair in stairsToPlace)
        {
            PlaceStairs(stair.Item1, stair.Item2, stair.Item3);
        }
    }

    void PlaceStairs(Vector3Int location, Quaternion orientation, bool up)
    {
        GameObject prefab = up ? upStairwayPrefab : downStairwayPrefab;

        GameObject go = Instantiate(prefab, location, orientation);
        go.transform.parent = transform;
    }

    #endregion

    public List<GameObject> GetInstantiatedPanicRooms() {
        return instantiatedPanicRooms;
    }
    #endregion
}
