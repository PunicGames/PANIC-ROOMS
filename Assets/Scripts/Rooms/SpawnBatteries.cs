using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBatteries : MonoBehaviour
{
    [SerializeField] private Transform[] spawn_positions;
    [SerializeField] private GameObject battery;

    public void _SpawnBatteries()
    {
        int idx = Random.RandomRange(0, spawn_positions.Length);
        Instantiate(battery, spawn_positions[idx].position, spawn_positions[idx].localRotation);
    }
}
