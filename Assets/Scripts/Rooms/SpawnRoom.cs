using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    [SerializeField] private Transform[] spawn_positions;

    public Transform GetRandomLocationPosition()
    {
        Transform spawnTransform = spawn_positions[Random.Range(0, spawn_positions.Length)];
        return spawnTransform;
    }
}
