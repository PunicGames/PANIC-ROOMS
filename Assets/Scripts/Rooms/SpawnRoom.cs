using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    [SerializeField] private Transform[] spawn_positions;
    [SerializeField] private Vector3 room_offset;

    private void Awake()
    {
        room_offset = transform.position;
        Debug.Log("1" + room_offset);
    }
    public Vector3 GetRandomLocationPosition()
    {
        Transform spawnTransform = spawn_positions[Random.Range(0, spawn_positions.Length)];
        Debug.Log("2" + room_offset);
        Debug.Log("3" + spawnTransform.position);
        return spawnTransform.position;
    }
}
