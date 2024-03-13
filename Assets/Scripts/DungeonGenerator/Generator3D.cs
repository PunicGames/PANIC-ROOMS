using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = System.Random;
using Graphs;
using Unity.AI.Navigation;

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

    class Room {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size) 
        {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b) 
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }
    Random random;
    public string seed;

    [SerializeField] Vector3Int size;
    Dictionary<Vector3Int, GameObject> dRooms;
    Dictionary<GameObject, bool> connectedRooms;
    [SerializeField] List<PanicRoom> panicRooms, additionalRooms;
    [SerializeField] GameObject hallwayPrefab;
    [SerializeField] GameObject upStairwayPrefab, downStairwayPrefab;

    private readonly Vector3Int[] testPos = { Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back };

    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    private NavMeshSurface navMeshSurface;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();

        if (string.IsNullOrEmpty(seed))
        {
            string randomString = RandomSeed.GenerateRandomAlphanumericString(8);
            UnityEngine.Debug.Log("SEED: \t"+ randomString);
            random = new Random(randomString.GetHashCode());
        }
        else
        {
            random = new Random(seed.GetHashCode());
        }

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        bool success = false;
        int maxTries = 10;
        int tries = 0;
        while (!success && tries < maxTries)
        {
            try
            {
                grid = new Grid3D<CellType>(size, Vector3Int.zero);
                rooms = new List<Room>();
                dRooms = new Dictionary<Vector3Int, GameObject>();
                connectedRooms = new Dictionary<GameObject, bool>();

                PlaceRooms();
                Triangulate();
                CreateHallways();
                PathfindHallways();

                navMeshSurface?.BuildNavMesh();
                success = true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                tries++;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log("Tries: " + tries);
        UnityEngine.Debug.Log("Tiempo transcurrido: "+ stopwatch.ElapsedMilliseconds + " milisegundos");
    }

    #endregion

    #region Methods
    public void PlaceRooms()
    {
        // Primero, colocar las habitaciones de 'firstRoomSizes'

        int roomCount = 0;
        foreach (PanicRoom room in panicRooms)
        {
            PlaceRoomAtRandomLocation(room);
            roomCount++;
        }

        foreach (PanicRoom room in additionalRooms)
        {
            if (ShouldPlaceRoom())
            {
                PlaceRoomAtRandomLocation(room);
            }
        }
    }

    bool ShouldPlaceRoom()
    {
        return random.NextDouble() < 0.5;
    }


    bool ShouldPlaceHallway()
    {
        return random.NextDouble() < 0.10;
    }

    void PlaceRoomAtRandomLocation(PanicRoom room)
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
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var r in rooms)
            {
                if (Room.Intersect(r, buffer))
                {
                    add = false;
                    break;
                }
            }
        
            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z)
            {
                add = false;
            }
        
            if (add)
            {
                rooms.Add(newRoom);
                GameObject placedRoom = PlaceRoom(newRoom.bounds.position, room);
                connectedRooms.Add(placedRoom, false);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                    dRooms[pos] = placedRoom;
                }
                return;
            }
        }

    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) 
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }


        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);


        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);

        foreach (var edge in selectedEdges)
        {
            UnityEngine.Debug.DrawLine(edge.U.Position, edge.V.Position, Color.red, 1000);
        }

        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            
            if (ShouldPlaceHallway())
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() 
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);
        List<Vector3Int> hallways = new List<Vector3Int>();

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            bool pathFound = false;

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
                        /*
                        if (delta.y != 0) {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);
                            
                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                            PlaceStairs(prev + horizontalOffset);
                            PlaceStairs(prev + horizontalOffset * 2);
                            PlaceStairs(prev + verticalOffset + horizontalOffset);
                            PlaceStairs(prev + verticalOffset + horizontalOffset * 2);
                        }*/

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
                                // Movimiento principal en el eje x
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
                                // Movimiento principal en el eje z
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

                            // Ajusta la posición si es necesario antes de colocar la escalera
                            PlaceStairs(prev + horizontalOffset, orientation, up); // Usa la orientación calculada
                        }
                    }
                }

                List<Vector3Int> transitionHallways = new List<Vector3Int>(); // Lista para guardar las posiciones de los pasillos de transición

                // Asegúrate de tener al menos 2 celdas en el camino para comparar
                if (path.Count > 1)
                {
                    // Comenzar desde 1 ya que vamos a comparar cada posición con la anterior
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

                pathFound = true;
            }

            if (!pathFound)
            {
                throw new System.Exception("No path found");
            }

        }
        
        foreach (var connectedRoom in connectedRooms)
        {
            if (!connectedRoom.Value)
            {
                throw new System.Exception("Room not connected");
            }
        }

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

    #endregion

    #region PlacePrefabs

    void CheckRoom(Vector3Int position)
    {
        for( int i = 0; i < testPos.Length; i++ )
        {
            Vector3Int move = testPos[i];

            if (dRooms.TryGetValue(position + move, out var room))
            {
                connectedRooms[room] = true;
                room.GetComponent<WallController>()?.MakeDoor(position + move, i > 1);
            }
            
        }
    }

    GameObject PlaceRoom(Vector3Int location, PanicRoom room)
    {
        GameObject go = Instantiate(room.prefab, location, Quaternion.identity);
        go.transform.parent = transform;
        return go;
    }

    void PlaceHallway(Vector3Int location) {
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

    void PlaceStairs(Vector3Int location, Quaternion orientation, bool up) {
        GameObject prefab = up ? upStairwayPrefab : downStairwayPrefab;
        
        GameObject go = Instantiate(prefab, location, orientation);
        go.transform.parent = transform;
        go.GetComponent<DCubeTint>().material = greenMaterial;
    }

    #endregion
}
