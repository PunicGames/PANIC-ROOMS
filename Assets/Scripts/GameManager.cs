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
    private Transform player_transform;
    public GameObject player_instance;
    public GameObject enemyPrefab;

    // Dungeon generator
    [SerializeField] private Generator3D dungeon_generator;

    //public List<GameObject> keyPrefabs;

    // Rooms
    static List<Room> _rooms;

    // Collectibles
    public List<GameObject> collectibles;
    private int MAX_COLLECTIBLES;

    // Win condition connection
    [SerializeField] PauseMenuController pauseMenuController;


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

    public void InitializePlayer()
    {

        // Check premature errors
        SpawnRoom spawn_room = dungeon_generator.GetInstantiatedPanicRooms()[0].GetComponent<SpawnRoom>();

        if (spawn_room == null)
        {
            throw new System.NullReferenceException("Can't instantiate player. In dungeon generator, the List of panic rooms" +
                                                    " is intended to have the first one with a SpawnRoom Script");
        }

        // Instantiate
        Transform spawn_location = spawn_room.GetRandomLocationPosition();
        player_transform = spawn_location;
        player_instance = Instantiate(playerPrefab, player_transform.position, player_transform.localRotation);

        // Instantiate player dependencies
        GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>().InitPlayerDependencies();

        //Setup MAX points
        MAX_COLLECTIBLES = collectibles.Count;
        Debug.Log("COLLECTIBLES: " + MAX_COLLECTIBLES);
        CharacterCollectionSystem collection = player_instance.GetComponent<CharacterCollectionSystem>();
        collection.SetMaxPoints(MAX_COLLECTIBLES);
    }

    public void SpawnEnemy()
    {
        StartCoroutine(InitializeEnemy());
    }

    public void RealocatePlayer() {
        player_instance.transform.position = player_transform.position;
        player_instance.transform.localRotation = player_transform.localRotation;
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



    private void SetupRooms()
    {
        List<GameObject> rooms = dungeon_generator.GetInstantiatedPanicRooms();

        for (int i = 1; i < rooms.Count; i++)
        {
            SpawnObjects spawner = rooms[i].GetComponent<SpawnObjects>();

            if (spawner)
            {
                if (i <= MAX_COLLECTIBLES)
                    spawner.SpawnCollectible(collectibles[i-1]);
                spawner.SpawnBattery();
                spawner.SpawnAssets();
            }

        }
    }



    public void UpdateWorldState(int num_collectibles)
    {
        switch (num_collectibles)
        {
            case 0:
                _instance.InitializePlayer();
                _instance.SetupRooms();
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
                pauseMenuController.EnableWinButton();
                break;
            case 8:
                break;
            default:
                break;

        }
    }

    private void UnlockEnd() { 
        
    }

    public void WinGame() { 
        
    }

    public void LoseGame() { 
        
    }

}
