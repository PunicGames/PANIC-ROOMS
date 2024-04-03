using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField]private EnemyBehavior enemy_behavior;

    private float[] health_increase_rate_levels = { 0.0f, 40.0f, 38.0f, 35.00f, 33.0f, 30.0f, 27.00f, 25.0f, 20.00f };
    private float[] health_decrease_rate_levels = { 0.0f, 20.0f, 22.0f, 25.50f, 29.5f, 32.0f, 35.00f, 37.0f, 40.00f };
    private float[] catch_distance_levels =       { 0.0f,  0.7f,  0.8f, 0.9f,  1.0f,  1.05f,  1.15f,  1.3f,  1.40f };
    private float[] enemy_speed_levels =          { 0.0f,  0.4f,  0.45f,  0.50f,  0.55f,  0.65f,  0.70f,  0.75f,  0.80f };
    private float[] teleportation_chance_levels = { 0.0f,  1.0f,  0.9f,  0.8f,  0.7f,  0.65f,  0.6f,  0.5f,  0.5f };
    private float[] teleport_distance_levels =    { 0.0f, 35.0f, 20.0f, 16.00f, 12.5f, 10.0f,  9.00f,  8.0f,  7.50f };
    private float[] teleport_timer_levels =       { 5.0f,  5.0f,  4.5f,  4.00f,  3.0f,  2.5f,  2.00f,  1.5f,  0.75f };


    public void UpdateStats(int player_points) {

        int upgrade_level = (player_points >= health_increase_rate_levels.Length) ? (health_increase_rate_levels.Length - 1) : player_points;

        enemy_behavior.SetHealthIncreaseRate(health_increase_rate_levels[upgrade_level]);
        enemy_behavior.SetHealthDecreaseRate(health_decrease_rate_levels[upgrade_level]);
        enemy_behavior.SetCatchDistance(catch_distance_levels[upgrade_level]);
        enemy_behavior.SetSpeed(enemy_speed_levels[upgrade_level]);
        enemy_behavior.SetTeleportationChance(teleportation_chance_levels[upgrade_level]);
        enemy_behavior.SetTeleportDistance(teleport_distance_levels[upgrade_level]);
        enemy_behavior.SetTeleportTimer(teleport_timer_levels[upgrade_level]);
    }
}