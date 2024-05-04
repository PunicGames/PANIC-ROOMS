using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskBehavior : MonoBehaviour
{


    private Transform initial_pos;

    private void Update()
    {
        if (this.transform.position.y <= -1.0f)
        {
            this.transform.position = initial_pos.position;
        }
    }

    public void SetInititalPos(Transform _pos) { 
        initial_pos = _pos;
    }


}
