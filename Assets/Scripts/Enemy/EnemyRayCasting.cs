using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRayCasting : MonoBehaviour
{
    private Transform player_transform;
    private float detection_range = Mathf.Infinity;

    public bool DetectPlayer()
    {
        // Direction from the enemy to the player
        Vector3 directionToPlayer = player_transform.position - transform.position;

        // Raycast from the enemy towards the player
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detection_range))
        {
            // Check if the first object hit by the ray is the player
            if (hit.collider.CompareTag("Player"))
            {
                //Debug.Log("Player in sight!");
                Debug.DrawLine(transform.position, hit.point, Color.green);
                return true;
            }
            // If something else is hit...
            else
            {
                //Debug.Log("Player not in direct sight");
                Debug.DrawLine(transform.position, hit.point, Color.red);
                return false;
            }
        }
        return false;
    }

    public void SetPlayerTransform(Transform pt) {
        player_transform = pt;
    }
}
