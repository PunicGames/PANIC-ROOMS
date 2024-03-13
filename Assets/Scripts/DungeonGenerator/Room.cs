using UnityEngine;

public class Room
{
    public BoundsInt Bounds { get; private set; }
    public bool Connected { get; set; }
    public Vector3Int Location { get; private set; }
    public GameObject Prefab { get; private set; }

    public Room(Vector3Int location, Vector3Int size, GameObject prefab)
    {
        Location = location;
        Prefab = prefab;
        Bounds = new BoundsInt(location, size);
        Connected = false;
    }

    public static bool Intersect(Room a, Room b)
    {
        return !((a.Bounds.position.x >= (b.Bounds.position.x + b.Bounds.size.x)) || ((a.Bounds.position.x + a.Bounds.size.x) <= b.Bounds.position.x)
            || (a.Bounds.position.y >= (b.Bounds.position.y + b.Bounds.size.y)) || ((a.Bounds.position.y + a.Bounds.size.y) <= b.Bounds.position.y)
            || (a.Bounds.position.z >= (b.Bounds.position.z + b.Bounds.size.z)) || ((a.Bounds.position.z + a.Bounds.size.z) <= b.Bounds.position.z));
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Room other = (Room)obj;
        return Bounds.Equals(other.Bounds) && Location.Equals(other.Location);
        // Considera si prefieres incluir Prefab en la comparación
    }

    public override int GetHashCode()
    {
        unchecked // Para evitar desbordamiento en caso de que el hash sea muy grande
        {
            int hash = 17;
            hash = hash * 23 + Bounds.GetHashCode();
            hash = hash * 23 + Location.GetHashCode();
            // Considera si prefieres incluir Prefab en el cálculo del hash
            return hash;
        }
    }
}
