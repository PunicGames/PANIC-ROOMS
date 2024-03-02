using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    private GameObject[] collectibles;
    private int num_points = 0;
    private int max_points = 8;
    private float min_distance_to_collect = 2.0f;
    private float angle_threshold = 20.0f;

    private EnemyStats enemy_stats;
    private CharacterMovement character_movement;

    private SprayNumBehaviour sprayUI;

    private void Start()
    {
        character_movement = GetComponent<CharacterMovement>();
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        max_points = collectibles.Length;
        sprayUI = GameObject.FindGameObjectWithTag("UI").GetComponent<SprayNumBehaviour>();
        sprayUI.UpdateNumbersLeft(max_points);
        enemy_stats = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyStats>();
        
    }
    public void IncrementPoint() { 
        num_points++;
        if (num_points > max_points) { 
            num_points = max_points;
        }

        enemy_stats.UpdateStats(num_points);
    }

    public void SetPoint(int new_point) { 
        num_points = new_point;
    }

    public int GetPoints() {
        return num_points;
    }

    public int GetMaxPoints()
    {
        return max_points;
    }

    public void CollectCollectible(Transform camera_transform)
    {
        for (int i = 0; i < collectibles.Length; i++)
        {

            // If it hasn't been collected already
            if (collectibles[i].active)
            {

                // If the object is near
                float distance_to_object = Vector3.Distance(camera_transform.position, collectibles[i].transform.position);
                if (distance_to_object < min_distance_to_collect)
                {

                    // If player is looking at the object
                    Vector3 direction_to_collectible = collectibles[i].transform.position - camera_transform.position;
                    direction_to_collectible.Normalize();
                    float angle = Vector3.Angle(camera_transform.forward, direction_to_collectible);

                    if (angle <= angle_threshold)
                    {
                        collectibles[i].active = false;

                        // Update num collectibles
                        num_points++;
                        //Update spray paint UI
                        sprayUI.UpdateNumbersLeft(max_points-num_points);
                        // Update enemy stats
                        enemy_stats.UpdateStats(num_points);

                        // Check win condition
                        CheckWinCondition();

                        // Stop checking collectibles. One is enough!!!!!!!
                        break;
                    }

                }
            }
        }
    }

    private void CheckWinCondition()
    {
        if (num_points == max_points)
        {
            character_movement.WinGame();
        }
    }
}
