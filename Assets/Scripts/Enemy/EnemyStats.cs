using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField]private EnemyBehavior enemy_behavior;

    private float[] health_increase_rate_levels = { 0.0f, 40.00f, 37.00f, 33.00f, 30.00f, 27.00f, 23.00f, 20.00f, 20.00f };
    private float[] health_decrease_rate_levels = { 0.0f,  6.00f,  7.00f,  8.50f,  9.50f, 10.00f, 12.00f, 13.00f, 13.00f };
    private float[] catch_distance_levels =       { 0.0f,  0.70f,  0.80f,  0.90f,  1.00f,  1.10f,  1.20f,  1.30f,  1.30f };
    private float[] enemy_speed_levels =          { 0.0f,  0.40f,  0.45f,  0.50f,  0.55f,  0.60f,  0.70f,  0.75f,  0.75f };
    private float[] teleportation_chance_levels = { 0.0f,  1.00f,  0.90f,  0.80f,  0.70f,  0.60f,  0.50f,  0.40f,  0.40f };
    private float[] teleport_distance_levels =    { 0.0f, 35.00f, 31.00f, 26.00f, 22.00f, 18.00f, 13.00f,  8.00f,  8.00f };
    private float[] teleport_timer_levels =       { 0.0f,  4.00f,  3.60f,  3.20f,  2.75f,  2.20f,  1.80f,  1.50f,  1.50f };


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