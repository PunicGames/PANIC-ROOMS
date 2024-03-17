using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

// Singleton class to manage the game
// Tasks High Level:
// 1. Keep track of the game state (number of key-objects collected)
// 2. Provides scene management (all the operations to change between scenes)
// 3. Provides information between scenes (Seed)

// 4. Is enchanced to spawn the player, enemies, and other objects in the scene

public class GameManager : MonoBehaviour
{
    // Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // Prefabs
    public GameObject playerPrefab;
    [SerializeField] private Transform player_init_pos;
    public GameObject enemyPrefab;
    [SerializeField] private Transform enemy_init_pos;

    // Dungeon generator
    [SerializeField] private Generator3D dungeon_generator;

    public List<GameObject> keyPrefabs;

    // Rooms
    static List<Room> _rooms;

    #region MonoBehaviour
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private void Start()
    {
        // TODO: Move to RoomsCreated() in the future
        {
            // Initialize player
            //InitializePlayer();
            // Testing
            //SpawnEnemy();
        }
    }

    public static void RoomsCreated(List<Room> rooms)
    {
        _rooms = rooms;

        Debug.Log("Rooms created" + rooms.Count);

        // Spawn the player
        _instance.InitializePlayer();
        // Spawn the enemies
        // TODO: spawn enemy not here but when first object is collected
        _instance.SpawnEnemy();
        // Spawn the key-objects

    }

    public void InitializePlayer() {

        // Check premature errors
        SpawnRoom spawn_room = dungeon_generator.GetAditionalRooms()[0].prefab.GetComponent<SpawnRoom>();
        if(spawn_room == null)
        {
            throw new System.NullReferenceException("Can't instantiate player. In dungeon generator, the List of additional rooms" +
                                                    " is intended to have the first one with a SpawnRoom Script");
        }

        // Instantiate
        Vector3 spawn_location = spawn_room.GetRandomLocationPosition();
        Instantiate(playerPrefab, spawn_location, Quaternion.identity);

        // Instantiate player dependencies
        GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>().InitPlayerDependencies();
    }

    public void SpawnEnemy() {
        StartCoroutine(InitializeEnemy());
    }

    IEnumerator InitializeEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        Instantiate(enemyPrefab, enemy_init_pos.position, Quaternion.identity);

        // Init player dependencies to enemy instance
        playerPrefab.GetComponent<CharacterCollectionSystem>().InitEnemyDependencies();
    }

}
