using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRayCasting : MonoBehaviour
{
    private Transform player_transform;
    private float detection_range = Mathf.Infinity;
    [SerializeField] private Transform[] raycast_positions;

    public bool DetectPlayer()
    {

        // Direction from the enemy to the player
        Vector3 directionToPlayer = player_transform.position - transform.position;

        // Raycast from the enemy towards the player
        RaycastHit hit;
        for (int i = 0; i < raycast_positions.Length; i++) { 
            Vector3 starting_pos = raycast_positions[i].position;
            if (Physics.Raycast(starting_pos, directionToPlayer, out hit, detection_range))
            {
                // Check if the first object hit by the ray is the player
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Player in sight!");
                    //Debug.DrawLine(starting_pos, hit.point, Color.green);
                    return true;
                }
                // If something else is hit...
                else
                {
                    //Debug.Log("Player not in direct sight");
                    //Debug.DrawLine(starting_pos, hit.point, Color.red);
                    //return false;
                }
            }
        }

        return false;
    }

    public void SetPlayerTransform(Transform pt) {
        player_transform = pt;
    }
}
