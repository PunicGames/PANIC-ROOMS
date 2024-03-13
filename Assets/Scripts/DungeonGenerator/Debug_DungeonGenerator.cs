using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_DungeonGenerator : MonoBehaviour
{
    public GameObject _dungeonGenerator;

    GameObject _obj;


    private void Awake()
    {
        _obj = Instantiate(_dungeonGenerator);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(_obj);
            _obj = Instantiate(_dungeonGenerator);
        }
    }       
}
