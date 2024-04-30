using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingBehavior : MonoBehaviour
{

    [SerializeField] private Light m_light;
    [SerializeField] private GameObject m_ceiling;
    [SerializeField] private Material m_offMaterial;
    [SerializeField] private Material m_onMaterial;
    public void TurnOffLight()
    {
        m_light.enabled = false;
        m_ceiling.GetComponent<MeshRenderer>().material = m_offMaterial;
    }
    public void TurnOnLight()
    {
        m_light.enabled = true;
        m_ceiling.GetComponent<MeshRenderer>().material = m_onMaterial;
    }
}
