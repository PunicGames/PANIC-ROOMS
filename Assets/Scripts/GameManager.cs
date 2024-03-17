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


    public List<GameObject> keyPrefabs;

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
            // Instantiate player
            InitializePlayer();
            // Testing
            StartCoroutine(SpawnEnemy());
        }
    }

    public static void RoomsCreated(List<Room> rooms)
    {
        _rooms = rooms;

        Debug.Log("Rooms created" + rooms.Count);

        // Spawn the player
        // Spawn the enemies
        // Spawn the key-objects
    }

    private void InitializePlayer() {
        // Instantiate player and dependencies
        Instantiate(playerPrefab, player_init_pos.position, Quaternion.identity);
        GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>().InitPlayerDependencies();
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        Instantiate(enemyPrefab, enemy_init_pos.position, Quaternion.identity);

        // Init player dependencies to enemy instance
        playerPrefab.GetComponent<CharacterCollectionSystem>().InitEnemyDependencies();
    }

}
