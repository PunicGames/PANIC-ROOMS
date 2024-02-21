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
    private float catch_distance = 1.5f;
    public GameObject enemy_cam;
    public EnemyRayCasting enemy_ray_caster;

    // Movement
    public Transform enemy_transform;
    private float enemy_speed = 1.0f;
    private int teleportation_chance;
    private float distance_to_player;
    public List<Transform> teleportation_spots;

    // Player related
    private GameObject player;
    [SerializeField] private Camera player_camera;
    private float health_increase_rate, health_decrease_rate;
    private float player_health = 100;
    public Transform player_transform;

    // UI related
    public Slider health_slider;

    // Enemy Camera
    [SerializeField] private Camera enemy_camera;

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
            //enemy_nav_mesh.speed = 0;
            //enemy_nav_mesh.SetDestination(transform.position); // Set destination to itself
            enemy_nav_mesh.enabled = false; // Stop enemie's movement

            // If there's direct vision between enemy and player
            if (enemy_ray_caster.DetectPlayer() == true) { 
            
            }

        }
        else {
            //**Debug.Log("Fuera de frustrum");
            enemy_nav_mesh.enabled = true; // Enable enemie's movement
            enemy_nav_mesh.speed = enemy_speed;
            enemy_destination = player_transform.position;
            enemy_nav_mesh.SetDestination(enemy_destination);
        }


        // Update UI
        UpdateUI();

        // Update LookAt to player
        this.transform.LookAt(new Vector3(player_transform.position.x, this.transform.position.y, player_transform.position.z));

        // Update distance to player
        distance_to_player = Vector3.Distance(this.transform.position, player_transform.position);

        // Catch player
        if (distance_to_player <= catch_distance) {
            CatchPlayer();
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

        StartCoroutine(PlayKillAnimation());
    }

    IEnumerator PlayKillAnimation() { 
        yield return new WaitForSeconds(1.0f);
        // Kill or whatever...
        
    }

    private void CatchPlayer() {
        player_health = 0;
    }

    private void UpdateUI()
    {
        health_slider.value = player_health;
    }
}
