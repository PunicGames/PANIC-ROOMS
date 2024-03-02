using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterCollectionSystem : MonoBehaviour
{
    private GameObject[] collectibles;
    private GameObject[] bateries;
    private int num_points = 0;
    private int max_points = 8;
    private float min_distance_to_collect = 2.0f;

    private EnemyStats enemy_stats;
    private CharacterMovement character_movement;
    [SerializeField] private Lantern_Bahavior lantern_behavior;
    [SerializeField] private Transform player_camera_transform;

    private SprayNumBehaviour sprayUI;
    private InGameUI game_ui;

    private void Start()
    {
        character_movement = GetComponent<CharacterMovement>();
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        bateries = GameObject.FindGameObjectsWithTag("Battery");
        max_points = collectibles.Length;
        game_ui = GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>();
        sprayUI = GameObject.FindGameObjectWithTag("UI").GetComponent<SprayNumBehaviour>();
        sprayUI.UpdateNumbersLeft(max_points);
        enemy_stats = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyStats>();
        
    }

    private void Update()
    {
        if (DetectObjectUpdateUI(collectibles, "Collectible"))
        {
            game_ui.ActivateCollectInfo("Collect collectible");
        }
        else if (DetectObjectUpdateUI(bateries, "Battery"))
        {
            game_ui.ActivateCollectInfo("Collect battery");
        }
        else {
            game_ui.DeactivateCollectInfo();
        }
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

    public void CollectObject()
    {
        // Check collectibles
        if (CollectGenericObject(collectibles, "Collectible"))
        {
            // Update num collectibles
            num_points++;
            //Update spray paint UI
            sprayUI.UpdateNumbersLeft(max_points - num_points);
            // Update enemy stats
            enemy_stats.UpdateStats(num_points);

            // Check win condition
            CheckWinCondition();
        }
        // Check bateries
        else {
            if (CollectGenericObject(bateries, "Battery")) {
                // Recargar linterna
                lantern_behavior.RechargeBattery();
            }
        }
    }

    private bool CollectGenericObject(GameObject[] objects, string tag) {

        for (int i = 0; i < objects.Length; i++)
        {

            // If it hasn't been collected already
            if (objects[i].active)
            {

                // If the object is near
                float distance_to_object = Vector3.Distance(player_camera_transform.position, objects[i].transform.position);
                if (distance_to_object < min_distance_to_collect)
                {

                    // If player is looking at the object
                    RaycastHit hit;

                    if (Physics.Raycast(player_camera_transform.position, player_camera_transform.forward, out hit, min_distance_to_collect))
                    {
                        if (hit.collider.gameObject.CompareTag(tag) && hit.collider.gameObject == objects[i])
                        {
                            objects[i].active = false;

                            // Stop checking
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private bool DetectObjectUpdateUI(GameObject[] objects, string tag) {

        for (int i = 0; i < objects.Length; i++)
        {
            // If it hasn't been collected already
            if (objects[i].active)
            {

                // If the object is near
                float distance_to_object = Vector3.Distance(player_camera_transform.position, objects[i].transform.position);
                if (distance_to_object < min_distance_to_collect)
                {

                    RaycastHit hit;

                    // If player is looking at the object
                    if (Physics.Raycast(player_camera_transform.position, player_camera_transform.forward , out hit, min_distance_to_collect))
                    {
                        if (hit.collider.gameObject.CompareTag(tag) && hit.collider.gameObject == objects[i])
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private void CheckWinCondition()
    {
        if (num_points == max_points)
        {
            character_movement.WinGame();
        }
    }
}
