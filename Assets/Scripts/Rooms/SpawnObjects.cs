using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private Transform[] m_collectibleSpawnPoints;
    [SerializeField] private GameObject[] m_collectibles;

    [SerializeField] private Transform[] m_batterySpawnPoints;
    [SerializeField] private GameObject[] m_batteries;

    [SerializeField] private Transform[] m_assetSpawnPoints;
    [SerializeField] private GameObject[] m_assets;

    const float MAX_FORCE = 3.0f;
   
    public void SpawnCollectible()
    {
        int idx = Random.Range(0, m_collectibleSpawnPoints.Length);
        int idy = Random.Range(0, m_collectibles.Length);
        GameObject collectible = Instantiate(m_collectibles[idy], m_collectibleSpawnPoints[idx].position, m_collectibleSpawnPoints[idx].localRotation);

        //Apply force in random direction
        collectible.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-MAX_FORCE, MAX_FORCE), MAX_FORCE,  Random.Range(-MAX_FORCE, MAX_FORCE)), ForceMode.Impulse);
    }
    public void SpawnBattery()
    {
        int idx = Random.Range(0, m_batterySpawnPoints.Length);
        int idy = Random.Range(0, m_batteries.Length);
        GameObject battery = Instantiate(m_batteries[idy], m_batterySpawnPoints[idx].position, m_batterySpawnPoints[idx].localRotation);

        //Apply force in random direction
        battery.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-MAX_FORCE, MAX_FORCE), MAX_FORCE, Random.Range(-MAX_FORCE, MAX_FORCE)), ForceMode.Impulse);
    }
    public void ArrangeAssets()
    {
        foreach (var p in m_assetSpawnPoints)
        {
            int idx = Random.Range(0, m_assets.Length);
            m_assets[idx].transform.SetPositionAndRotation(p.position, p.rotation);
        }

    }
}
