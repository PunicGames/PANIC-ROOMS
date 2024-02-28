using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    private int num_points = 0;
    private int max_points = 8;

    [SerializeField] private EnemyStats enemy_stats;

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
}
