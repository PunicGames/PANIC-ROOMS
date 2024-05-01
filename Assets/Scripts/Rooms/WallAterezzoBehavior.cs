using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WallAterezzoBehavior : MonoBehaviour
{
    [SerializeField] private List<Material> m_attrezo;
    [SerializeField] private DecalProjector m_projector;
    [SerializeField] private int m_atrezzoProbability;


    private void Start()
    {
        int prob = Random.Range(0, 100);
        int atrezzoId =Random.Range(0, m_attrezo.Count);
        //Russian roulette
        if (prob < m_atrezzoProbability)
        {
            m_projector.enabled = true;
            m_projector.material = m_attrezo[atrezzoId];
        }

    }

}


