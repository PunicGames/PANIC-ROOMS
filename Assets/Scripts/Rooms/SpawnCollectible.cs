using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollectible : MonoBehaviour
{
    [SerializeField] private Transform[] spawn_positions;
    [SerializeField] private GameObject collectible;

    public void SpawnCollectibles()
    {
        int idx = Random.RandomRange(0, spawn_positions.Length);
        Instantiate(collectible, spawn_positions[idx].position, spawn_positions[idx].localRotation);
    }
}
