using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{

    // Ai related
    private NavMeshAgent enemy_nav_mesh;
    private Vector3 enemy_destination;

    // Detection
    private MeshRenderer enemy_mesh;
    private float catch_distance = 2.5f;
    public GameObject enemy_cam;
    public EnemyRayCasting enemy_ray_caster;

    // Movement
    public Transform enemy_transform;
    private float enemy_speed = 0.8f;
    private int teleportation_chance;
    private float distance_to_player;
    private bool trigger_teleport = false;

    // Player related
    private GameObject player;
    [SerializeField] private Camera player_camera;
    private float health_increase_rate = 40.0f;
    private float health_decrease_rate = 20.0f;
    private float player_health = 100;
    public Transform player_transform;

    // UI related
    public Slider health_slider;

    // Enemy Camera
    [SerializeField] private Camera enemy_camera;
    [SerializeField] private AudioListener enemy_audio_listener;

    // Sounds
    [SerializeField] private AudioSource static_sound;
    float static_volume = 0;
    private float sound_increase_rate = 0.2f;
    private float sound_decrease_rate = 0.4f;

    // Others
    private bool kill_player = false;


    // Start is called before the first frame update
    void Start()
    {
        enemy_mesh = GetComponent<MeshRenderer>();
        enemy_nav_mesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        player_transform = player.GetComponent<Transform>();
        enemy_ray_caster = GetComponent<EnemyRayCasting>();
        enemy_ray_caster.SetPlayerTransform(player_transform);
        enemy_transform = GetComponent<Transform>();
        enemy_camera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Compute enemy behavior if it hasn't ever triggered killing animation
        
        // Kill player if health below 0
        if (player_health <= 0) {
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
            //**Debug.Log("Dentro de frustrum");
            enemy_nav_mesh.enabled = false; // Stop enemie's movement

            // If there's direct vision between enemy and player
            if (enemy_ray_caster.DetectPlayer() == true) {

                DecreaseSanity();

                // Enable trigger to teleport again
                trigger_teleport = true;
            }

        }
        else {
            //**Debug.Log("Fuera de frustrum");
            enemy_nav_mesh.enabled = true; // Enable enemie's movement
            enemy_nav_mesh.speed = enemy_speed;
            enemy_destination = player_transform.position;
            enemy_nav_mesh.SetDestination(enemy_destination);

            // Teleportation
            if (trigger_teleport) {
                TeleportToNewPosition();
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
            player_health -= health_decrease_rate * Time.deltaTime;
        }


        // Update UI
        UpdateUI();

        // Update sounds
        static_sound.volume = static_volume;
    }

    private void TeleportToNewPosition() {

        // TODO: Meter random de mantener posicion o calcular una nueva

        // METHOD 2
        Vector3 random_position = Vector3.zero;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(player_camera);
        float teleport_distance = 20.0f;
        bool found_spot = false;
        int current_sample = 0;
        int max_samples = 50;
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
                    this.transform.position = random_position;
                    if (!GeometryUtility.TestPlanesAABB(planes, enemy_mesh.bounds)) { 
                        found_spot = true;
                    }
                }
            }

            // Accumulate num samples to keep control
            current_sample++;
        }

        // Teleport the enemy to the new position
        if (found_spot) { 
            transform.position = random_position;
        }

        Debug.Log(current_sample);
    }


    private void IncreaseSanity() {
        // Increase sanity
        player_health += health_increase_rate * Time.deltaTime;
        if (player_health > 100)
        {
            player_health = 100;
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
        player_health -= health_decrease_rate * Time.deltaTime * 0.5f;

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
        yield return new WaitForSeconds(1.0f);
        // Kill or whatever...
        
    }

    private void UpdateUI()
    {
        health_slider.value = player_health;
    }
}