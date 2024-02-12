using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCubeTint : MonoBehaviour
{
    [HideInInspector] public Material material = null;
    private List<MeshRenderer> m_MeshRenderer;


    private void Awake()
    {
        m_MeshRenderer = new List<MeshRenderer>(gameObject.GetComponentsInChildren<MeshRenderer>());
    }

    private void Start()
    {
        if (material)
        {
            foreach (MeshRenderer renderer in m_MeshRenderer) { renderer.material = material; }
        }
    }
}
