using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
