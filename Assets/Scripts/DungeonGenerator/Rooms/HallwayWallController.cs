using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayWallController : MonoBehaviour
{
    [SerializeField] List<GameObject> walls;

    private Dictionary<Vector3Int, GameObject> dWalls;

    private readonly Vector3Int[] wallPos = { Vector3Int.right, Vector3Int.left, Vector3Int.forward, Vector3Int.back };

    private void Awake()
    {
        dWalls = new Dictionary<Vector3Int, GameObject>();
        for (int i = 0; i < walls.Count; i++)
        {
            dWalls[wallPos[i]] = walls[i];
        }
    }

    public void MakeWalls(Vector3Int wallPos)
    {
        dWalls[wallPos].SetActive(false);
    }
}
