using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{

    // AI related
    private NavMeshAgent enemy_nav_mesh;
    private Vector3 enemy_destination;

    // Detection
    private MeshRenderer enemy_mesh;
    private float catch_distance;
    public EnemyRayCasting enemy_ray_caster;
    private bool player_in_sight = false;

    // Movement
    public Transform enemy_transform;
    private float enemy_speed;
    private float distance_to_player;

    private bool trigger_teleport = false;
    private float teleportation_chance = 1.0f;
    private float teleport_distance;
    private float teleport_timer;
    private Coroutine teleport_coroutine;

    // Player related
    private GameObject player;
    private CharacterMovement character_movement;
    private Camera player_camera;
    private float health_increase_rate;
    private float health_decrease_rate;
    public Transform player_transform;

    // Enemy Camera
    [SerializeField] private Camera enemy_camera;
    [SerializeField] private AudioListener enemy_audio_listener;

    // Sounds
    [SerializeField] private AudioSource static_sound;
    private float static_volume = 0;
    private float sound_increase_rate = 0.2f;
    private float sound_decrease_rate = 0.4f;

    // Others
    private bool kill_player = false;
    private EnemyStats enemy_stats;
    private bool fully_spawned = false;

    private void Awake()
    {
        InitPlayerDependencies();
        enemy_stats.UpdateStats(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!fully_spawned) return;


        // Kill player if health below 0
        if (character_movement.GetHealth() <= 0) {
        if (!kill_player) // Just a trigger to run KillPlayer() once
            {
                KillPlayer();
                kill_player = true;
            }
        }

        // Detect if player inside or outside camera's frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player_camera);
        if (GeometryUtility.TestPlanesAABB(planes, enemy_mesh.bounds))
        {

            // Stop enemie's movement in case its kinda close. We dont want to stop its
            // movement being really far, even if its inside the frustum
            if (distance_to_player < 10.0f)
            {
                enemy_nav_mesh.enabled = false;
            }
            else {
                // If its inside the frustum but kinda far, we activate again persecution
                enemy_nav_mesh.enabled = true;
                enemy_nav_mesh.speed = enemy_speed;
                enemy_destination = player_transform.position;
                enemy_nav_mesh.SetDestination(enemy_destination);
            }

            // If there's direct vision between enemy and player
            if (enemy_ray_caster.DetectPlayer() == true)
            {
                player_in_sight = true;
                DecreaseSanity();

                // Enable trigger to teleport again
                trigger_teleport = true;

                // Reset coroutine in case there was a previous one running
                if (teleport_coroutine != null)
                {
                    // If the coroutine is already running, stop it and restart it
                    StopCoroutine(teleport_coroutine);
                }
            }
            else {
                player_in_sight = false;
                IncreaseSanity();
            }
        }
        else {
            player_in_sight = false;

            enemy_nav_mesh.enabled = true; // Enable enemie's movement
            enemy_nav_mesh.speed = enemy_speed;
            enemy_destination = player_transform.position;
            enemy_nav_mesh.SetDestination(enemy_destination);

            // Teleportation
            if (trigger_teleport) {
                if (teleport_coroutine != null)
                {
                    // If the coroutine is already running, stop it and restart it
                    StopCoroutine(teleport_coroutine);
                }
                teleport_coroutine = StartCoroutine(TeleportAfterDelay(teleport_timer));
                trigger_teleport = false;
            }

            // If enemy is not close enough to player
            if (distance_to_player > catch_distance)
            {
                IncreaseSanity();
            }
            // If enemy is so close to player even if it's not in sight
            else {
                DecreaseSanity();
            }
        }


        // Update LookAt to player
        this.transform.LookAt(new Vector3(player_transform.position.x, this.transform.position.y, player_transform.position.z));
        

        // Update distance to player
        distance_to_player = Vector3.Distance(this.transform.position, player_transform.position);

        if (distance_to_player <= catch_distance) {
            float current_health = character_movement.GetHealth();
            character_movement.SetHealth(current_health - health_decrease_rate * Time.deltaTime);
        }

        // Update sounds
        static_sound.volume = static_volume * 0.5f;

        // Update tension
        UpdatePlayerTension();
    }

    IEnumerator TeleportAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TeleportToNewPosition();
    }
    public void TeleportToNewPosition(int max_samples=750) {

        // Teleport at random ocassions, not only when enemy goes out of frustum.
        float random_teleport_sample = Random.Range(0.0f, 1.0f);
        if (random_teleport_sample > teleportation_chance) return;

        // Compute teleportation in the disk area
        Vector3 random_position = Vector3.zero;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player_camera);
        bool found_spot = false;
        int current_sample = 0;
        RaycastHit hit;
        
        while (!found_spot && (current_sample < max_samples))
        {
            // Generate a random direction at a specified distance in a disk centered in player
            Vector2 random_point_in_disk = Random.insideUnitCircle.normalized * teleport_distance;
            Vector3 random_direction = new Vector3(random_point_in_disk.x, 0, random_point_in_disk.y);
            random_position = player_transform.position + random_direction;


            // Cast a ray downwards from the randomPosition to find the ground
            if (Physics.Raycast(random_position + Vector3.up, Vector3.down, out hit, Mathf.Infinity))
            {

                // Check if the hit object has the "Floor" tag
                if (hit.collider.CompareTag("Floor"))
                {
                    random_position = hit.point;

                    // Check if new position is not in frustum
                    if (!GeometryUtility.TestPlanesAABB(planes, enemy_mesh.bounds)) { 
                        found_spot = true;
                    }
                }
            }

            // Accumulate num samples to keep control
            current_sample++;
        }

        Debug.Log("Did enemy found a new spot for teleportation? " + found_spot);

        // Teleport the enemy to the new position
        if (found_spot) { 
            transform.position = random_position;
        }
    }


    private void IncreaseSanity() {
        // Increase sanity
        float current_health = character_movement.GetHealth();
        character_movement.SetHealth(current_health + health_increase_rate * Time.deltaTime);
        if (character_movement.GetHealth() > 100)
        {
            character_movement.SetHealth(100);
        }

        // Update volume values
        static_volume -= sound_decrease_rate * Time.deltaTime;
        if (static_volume < 0)
        {
            static_volume = 0;
        }
    }
    private void DecreaseSanity() {

        // Decrease sanity
        float current_health = character_movement.GetHealth();
        character_movement.SetHealth(current_health - health_decrease_rate * Time.deltaTime * 0.5f);

        // Update volume values
        static_volume += sound_increase_rate * Time.deltaTime * 0.5f;
        if (static_volume > 1)
        {
            static_volume = 1;
        }
    }


    private void KillPlayer() {

        // Enable nav mesh to prevent errors
        enemy_nav_mesh.enabled = true;
        enemy_nav_mesh.SetDestination(this.transform.position);
        enemy_nav_mesh.speed = 0;

        player.active = false;
        player_camera.enabled = false;

        enemy_camera.enabled = true;
        enemy_audio_listener.enabled = true;

        StartCoroutine(PlayKillAnimation());
    }

    IEnumerator PlayKillAnimation() { 
        yield return new WaitForSeconds(2.0f);
        // Kill or whatever...
        character_movement.LoseGame();
    }

    public void InitPlayerDependencies() {
        player = GameObject.FindGameObjectWithTag("Player");
        player_camera = player.GetComponentInChildren<Camera>();
        character_movement = player.GetComponent<CharacterMovement>();
        player_transform = player.GetComponent<Transform>();

        enemy_mesh = GetComponent<MeshRenderer>();
        enemy_nav_mesh = GetComponent<NavMeshAgent>();
        enemy_ray_caster = GetComponent<EnemyRayCasting>();
        enemy_ray_caster.SetPlayerTransform(player_transform);
        enemy_transform = GetComponent<Transform>();
        enemy_camera.enabled = false;
        enemy_stats = GetComponent<EnemyStats>();

        fully_spawned = true;
    }

    private void UpdatePlayerTension() {
        float tension = 0.0f;

        // Adjust tension depending on player's health
        tension += (100.0f - character_movement.GetHealth()) * 1.0f;

        // Adjust tension based on in-sight enemy
        if (player_in_sight) tension += 20.0f;

        character_movement.SetBaseTension(tension);
    }

    public void SetHealthIncreaseRate(float new_value) 
    {
        health_increase_rate = new_value;
    }
    public void SetHealthDecreaseRate(float new_value) 
    {
        health_decrease_rate = new_value;
    }
    public void SetCatchDistance(float new_value) 
    {
        catch_distance = new_value;
    }
    public void SetSpeed(float new_value) 
    {
        enemy_speed = new_value;
    }
    public void SetTeleportationChance(float new_value)
    {
        teleportation_chance = new_value;
    }
    public void SetTeleportDistance(float new_value) 
    {
        teleport_distance = new_value;
    }

    public void SetTeleportTimer(float new_value)
    {
        teleport_timer = new_value;
    }

}
