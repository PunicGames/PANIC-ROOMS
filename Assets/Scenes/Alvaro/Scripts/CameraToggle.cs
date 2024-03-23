using UnityEngine;

public class CameraMaterialToggle : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera; // Arrastra aquí tu cámara

    [SerializeField]
    private Material activeMaterial; // Asigna aquí el material para cuando la cámara está activada

    [SerializeField]
    private Material inactiveMaterial; // Asigna aquí el material para cuando la cámara está desactivada

    // Referencia al Renderer del GameObject para cambiar los materiales
    private Renderer objectRenderer;

    // Start se llama antes de la primera actualización del frame
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        // Asegúrate de que el array de materiales tenga al menos 2 elementos
        if (objectRenderer.materials.Length < 2)
        {
            Debug.LogError("El objeto no tiene suficientes materiales.");
            return;
        }

        // Inicializa el material en el índice 1 con el material inactivo
        var materials = objectRenderer.materials;
        materials[1] = inactiveMaterial;
        objectRenderer.materials = materials; // Es importante reasignar el array modificado de nuevo al renderer
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Verificar si el jugador presionó la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Invertir el estado de activación de la cámara
            targetCamera.enabled = !targetCamera.enabled;

            // Cambiar el material del elemento 1 en el array de materiales dependiendo del estado de la cámara
            var materials = objectRenderer.materials;
            materials[1] = targetCamera.enabled ? activeMaterial : inactiveMaterial;
            objectRenderer.materials = materials; // Reasignar el array modificado
        }
    }
}
