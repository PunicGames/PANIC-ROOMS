using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern_Bahavior : MonoBehaviour
{
    [HideInInspector] Light spotlight;

    private void Awake()
    {
        spotlight = GetComponentInChildren<Light>();
    }

    public void ActivateLantern()
    {
        spotlight.enabled = true;
    }
    public void DeactivateLantern() { 
        
        spotlight.enabled = false;
    }
}
