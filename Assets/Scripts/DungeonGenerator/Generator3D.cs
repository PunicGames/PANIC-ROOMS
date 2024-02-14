using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

[System.Serializable]
public struct PanicRoom
{
    public Vector3Int size;
    public GameObject prefab;
}

public class Generator3D : MonoBehaviour {
    enum CellType {
        None,
        Room,
        Hallway,
        Stairs
    }

    class Room {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size) {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    public string seed;

    [SerializeField] Vector3Int size;
    [SerializeField] int roomCount;
    [SerializeField] Vector3Int roomMaxSize;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] List<PanicRoom> panicRooms, additionalRooms;
    [SerializeField] GameObject hallwayPrefab;
    [SerializeField] GameObject upStairwayPrefab, downStairwayPrefab;

    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material greenMaterial;


    Random random;
    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start() {
        random = new Random(seed.GetHashCode());
        grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void PlaceRooms()
    {
        // Primero, colocar las habitaciones de 'firstRoomSizes'
        foreach (PanicRoom room in panicRooms)
        {
            PlaceRoomAtRandomLocation(room);
        }

        List<PanicRoom> shuffledRoomSizes = new List<PanicRoom>(additionalRooms);
        Shuffle(shuffledRoomSizes); // Mezclar la lista

        foreach (PanicRoom room in shuffledRoomSizes)
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
                PlaceRoom(newRoom.bounds.position, room);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
                return;
            }
        }

    }



    /*
    void PlaceRooms() {
        int i = 0;

        //for (int i = 0; i < roomCount; i++)
        while ( i < roomCount)
        {
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(3, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(3, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);
                Debug.Log(newRoom.bounds.size);
                i++;

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }
    */
    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            
            if (random.NextDouble() < 0.125)
            {
                selectedEdges.Add(edge);
            }
            //selectedEdges.Add(edge);
        }
    }

    void PathfindHallways() {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0) {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs) {
                        return pathCost;
                    } else if (grid[b.Position] == CellType.Room) {
                        pathCost.cost += 5;
                    } else if (grid[b.Position] == CellType.None) {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                } else {
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
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset)) {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None) {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
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

                                if (delta.x > 0 )
                                {
                                    orientation = Quaternion.Euler(0, 90, 0);
                                    horizontalOffset += new Vector3Int(1, 0, 1);
                                    
                                } else
                                {
                                    orientation = Quaternion.Euler(0,270,0);
                                    
                                }
                            }
                            else
                            {
                                // Movimiento principal en el eje z
                                if (delta.z < 0)
                                {
                                    orientation = Quaternion.Euler(0, 180, 0);
                                    horizontalOffset += new Vector3Int(1,0,0);
                                    
                                } else
                                {
                                    orientation = Quaternion.identity;
                                    horizontalOffset += new Vector3Int(0, 0, 1);
                                }
                            }

                            // Ajusta la posición si es necesario antes de colocar la escalera
                            PlaceStairs(prev + horizontalOffset, orientation, up); // Usa la orientación calculada
                        }



                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }

                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway) {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    //TO-DO : Checkear que en primer y ultimo pasillo si hay pared
    // Acceder al prefab del room y quitarle la pared (llamando a una funcion de quitar paredes del room)
    void CheckRoom()
    {
    }

    void PlaceCube(Vector3Int location, Vector3Int size, Material material) {
        GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
        go.name = size.ToString();
        go.GetComponent<Transform>().localScale = size;
        go.GetComponent<DCubeTint>().material = material;
    } 


    void PlaceRoom(Vector3Int location, PanicRoom room)
    {
        GameObject go = Instantiate(room.prefab, location, Quaternion.identity);
        go.GetComponent<DCubeTint>().material = redMaterial;
    }
    void PlaceRoom(Vector3Int location, Vector3Int size) {
        PlaceCube(location, size, redMaterial);
    }

    void PlaceHallway(Vector3Int location) {
        GameObject go = Instantiate(hallwayPrefab, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3Int(1, 1, 1);
        go.GetComponent<DCubeTint>().material = blueMaterial;
    }

    void PlaceStairs(Vector3Int location, Quaternion orientation, bool up) {
        GameObject prefab = up ? upStairwayPrefab : downStairwayPrefab;
        
        GameObject go = Instantiate(prefab, location, orientation);
        go.GetComponent<DCubeTint>().material = greenMaterial;
    }
}
