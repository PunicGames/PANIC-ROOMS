using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLightControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<CeilingBehavior>().TurnOffLight();

    }
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<CeilingBehavior>().TurnOnLight();

    }

}
