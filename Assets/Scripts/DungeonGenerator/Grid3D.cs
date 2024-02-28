using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D<T> : IEnumerable<T> 
{
    
    T[] data;

    public Vector3Int Size { get; private set; }
    public Vector3Int Offset { get; set; }

    public Grid3D(Vector3Int size, Vector3Int offset) 
    {
        Size = size;
        Offset = offset;

        data = new T[size.x * size.y * size.z];
    }

    public int GetIndex(Vector3Int pos)
    {
        return pos.x + (Size.x * pos.y) + (Size.x * Size.y * pos.z);
    }

    public bool InBounds(Vector3Int pos)
    {
        return new BoundsInt(Vector3Int.zero, Size).Contains(pos + Offset);
    }

    public T this[int x, int y, int z]
    {
        get {
            return this[new Vector3Int(x, y, z)];
        }
        set {
            this[new Vector3Int(x, y, z)] = value;
        }
    }

    public bool TryGet(Vector3Int pos, out T value)
    {
        int idx = GetIndex(pos + Offset);

        if (idx < 0 || idx >= data.Length)
        {
            value = default;
            return false;
        } else
        {
            value = data[idx];
            return true;
        }
    }

    public T this[Vector3Int pos]
    {
        get {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in data)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
