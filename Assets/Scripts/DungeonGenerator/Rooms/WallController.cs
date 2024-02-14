using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    Dictionary<Vector3Int, GameObject> zWalls = new Dictionary<Vector3Int, GameObject>();
    Dictionary<Vector3Int, GameObject> xWalls = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        // Buscar todos los Transform en los hijos del GameObject actual.
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        // Iterar a través de todos los hijos para buscar los tags especificados.
        foreach (Transform child in allChildren)
        {
            // Ignorar el transform del GameObject padre.
            if (child == transform) continue;

            // Chequear si el hijo tiene el tag zWall.
            if (child.CompareTag("zWall"))
            {
                // Convertir la posición a Vector3Int para usar como clave en el diccionario.
                Vector3Int positionKey = Vector3Int.RoundToInt(child.position);
                zWalls.Add(positionKey, child.gameObject);
            }
            // Chequear si el hijo tiene el tag xWall.
            else if (child.CompareTag("xWall"))
            {
                // Convertir la posición a Vector3Int para usar como clave en el diccionario.
                Vector3Int positionKey = Vector3Int.RoundToInt(child.position);
                xWalls.Add(positionKey, child.gameObject);
            }
        }
    }


    public void MakeDoor(Vector3Int position, bool movingAlongZ) 
    {
        Dictionary<Vector3Int, GameObject> dic = movingAlongZ ? zWalls : xWalls;

        if (dic.TryGetValue(position, out var wall))
        {
            wall.SetActive(false);
        }
    }

}