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
    public GameObject enemyPrefab;

    // Dungeon generator
    [SerializeField] private Generator3D dungeon_generator;

    public List<GameObject> keyPrefabs;

    // Rooms
    static List<Room> _rooms;

    // Other variables
    private int num_collected_colletibles = 0;

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
        _instance.UpdateWorldState(0);

    }

    public void InitializePlayer() {

        // Check premature errors
        SpawnRoom spawn_room = dungeon_generator.GetInstantiatedPanicRooms()[0].GetComponent<SpawnRoom>();

        if(spawn_room == null)
        {
            throw new System.NullReferenceException("Can't instantiate player. In dungeon generator, the List of panic rooms" +
                                                    " is intended to have the first one with a SpawnRoom Script");
        }

        // Instantiate
        Transform spawn_location = spawn_room.GetRandomLocationPosition();
        Instantiate(playerPrefab, spawn_location.position, spawn_location.localRotation);

        // Instantiate player dependencies
        GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>().InitPlayerDependencies();
    }

    public void SpawnEnemy() {
        StartCoroutine(InitializeEnemy());
    }

    IEnumerator InitializeEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        SpawnRoom spawn_room = dungeon_generator.GetInstantiatedPanicRooms()[0].GetComponent<SpawnRoom>();
        Transform spawn_location = spawn_room.GetRandomLocationPosition();
        Instantiate(enemyPrefab, spawn_location.position, Quaternion.identity);

        // Solve dependencies
        playerPrefab.GetComponent<CharacterCollectionSystem>().InitEnemyDependencies();
    }

    private void SpawnCollectibles() {

        List<GameObject> rooms = dungeon_generator.GetInstantiatedPanicRooms();

        for (int i = 1; i < rooms.Count; i++) { 
            SpawnCollectible spawn_collectible = rooms[i].GetComponent<SpawnCollectible>();
            
            if (spawn_collectible != null)
            {
                spawn_collectible.SpawnCollectibles();
            }
        
        }


    }

    private void SpawnBatteries()
    {

        List<GameObject> rooms = dungeon_generator.GetInstantiatedPanicRooms();

        for (int i = 1; i < rooms.Count; i++)
        {
            SpawnBatteries spawn_batteries = rooms[i].GetComponent<SpawnBatteries>();

            if (spawn_batteries != null)
            {
                spawn_batteries._SpawnBatteries();
            }

        }


    }

    public void UpdateWorldState(int num_collectibles) {
        switch (num_collectibles) {
            case 0:
                _instance.InitializePlayer();
                _instance.SpawnCollectibles();
                _instance.SpawnBatteries();
                break;
            case 1:
                _instance.SpawnEnemy();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            default:
                break;

        }
    }

}
