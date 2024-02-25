using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private EnemyBehavior enemy_behavior;

    private float[] health_increase_rate_levels = { 40.0f, 40.0f, 38.0f, 35.0f, 33.0f, 30.0f, 27.0f, 25.0f, 20.0f };
    private float[] health_decrease_rate_levels = { 20.0f, 20.0f, 25.0f, 29.5f, 35.5f, 37.0f, 40.0f, 47.0f, 55.0f };
    private float[] catch_distance_levels = { 2.0f, 2.0f, 2.25f, 2.5f, 2.75f, 3.0f, 3.0f, 3.25f, 3.5f };
    private float[] enemy_speed_levels = { 0.5f, 0.5f, 0.7f, 0.8f, 1.0f, 1.2f, 1.3f, 1.5f, 1.6f };
    private float[] teleportation_chance_levels = { 0.9f, 0.9f, 0.8f, 0.65f, 0.6f, 0.5f, 0.45f, 0.4f, 0.35f };
    private float[] teleport_distance_levels = { 50.0f, 50.0f, 40.0f, 32.0f, 25.0f, 20.0f, 18.0f, 15.0f, 13.0f };

    private void Start()
    {
        enemy_behavior = GetComponent<EnemyBehavior>();
    }

    public void UpdateStats(int player_points) {
        enemy_behavior.SetHealthIncreaseRate(health_increase_rate_levels[player_points]);
        enemy_behavior.SetHealthDecreaseRate(health_decrease_rate_levels[player_points]);
        enemy_behavior.SetCatchDistance(catch_distance_levels[player_points]);
        enemy_behavior.SetSpeed(enemy_speed_levels[player_points]);
        enemy_behavior.SetTeleportationChance(teleportation_chance_levels[player_points]);
        enemy_behavior.SetTeleportDistance(teleport_distance_levels[player_points]);
    }
}