using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class murderScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int condition  = Random.Range(0,3);
        gameObject.SetActive(condition == 0);
    }

   
}
