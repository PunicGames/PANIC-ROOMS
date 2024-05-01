using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingBehavior : MonoBehaviour
{

    [SerializeField] private List<Light> m_lights;
    [SerializeField] private GameObject m_ceiling;
    [SerializeField] private Material m_offMaterial;
    [SerializeField] private Material m_onMaterial;
    public void TurnOffLight()
    {
        foreach (var light in m_lights)
        {
            light.enabled = false;
        }
        m_ceiling.GetComponent<MeshRenderer>().material = m_offMaterial;
    }
    public void TurnOnLight()
    {
        foreach (var light in m_lights)
        {
            light.enabled = true;
        }
        m_ceiling.GetComponent<MeshRenderer>().material = m_onMaterial;
    }
}
