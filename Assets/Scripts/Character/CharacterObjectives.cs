using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectives : MonoBehaviour
{

    [SerializeField] private GameObject[] collectibles;
    [SerializeField] private EnemyStats enemy_stats;
    [SerializeField] private CharacterMovement character_movement;
    private int num_collectibles = 0;
    private int num_max_collectibles;
    private float min_distance_to_collect = 2.0f;
    public float angle_threshold = 5.0f;


    private void Start()
    {
        num_max_collectibles = collectibles.Length;
    }

    public void CollectCollectible(Transform camera_transform) {
        for (int i = 0; i < collectibles.Length; i++) {

            // If it hasn't been collected already
            if (collectibles[i].active) {

                // If the object is near
                float distance_to_object = Vector3.Distance(camera_transform.position, collectibles[i].transform.position);
                if (distance_to_object < min_distance_to_collect) {

                    // If player is looking at the object
                    Vector3 direction_to_collectible = collectibles[i].transform.position - camera_transform.position;
                    direction_to_collectible.Normalize();
                    float angle = Vector3.Angle(camera_transform.forward, direction_to_collectible);

                    if (angle <= angle_threshold) { 
                        collectibles[i].active = false;

                        // Update num collectibles
                        num_collectibles++;
                        // Update enemy stats
                        enemy_stats.UpdateStats(num_collectibles);

                        // Check win condition
                        CheckWinCondition();

                        // Stop checking collectibles. One is enough!!!!!!!
                        break;
                    }

                }
            }
        }
    }

    private void CheckWinCondition() {
        if (num_collectibles == num_max_collectibles) {
            character_movement.WinGame();
        }
    }

}
