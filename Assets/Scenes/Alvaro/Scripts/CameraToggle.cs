using UnityEngine;

public class CameraMaterialToggle : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera; // Arrastra aqu� tu c�mara

    [SerializeField]
    private Material activeMaterial; // Asigna aqu� el material para cuando la c�mara est� activada

    [SerializeField]
    private Material inactiveMaterial; // Asigna aqu� el material para cuando la c�mara est� desactivada

    // Referencia al Renderer del GameObject para cambiar los materiales
    private Renderer objectRenderer;

    // Start se llama antes de la primera actualizaci�n del frame
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        // Aseg�rate de que el array de materiales tenga al menos 2 elementos
        if (objectRenderer.materials.Length < 2)
        {
            Debug.LogError("El objeto no tiene suficientes materiales.");
            return;
        }

        // Inicializa el material en el �ndice 1 con el material inactivo
        var materials = objectRenderer.materials;
        materials[1] = inactiveMaterial;
        objectRenderer.materials = materials; // Es importante reasignar el array modificado de nuevo al renderer
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Verificar si el jugador presion� la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Invertir el estado de activaci�n de la c�mara
            targetCamera.enabled = !targetCamera.enabled;

            // Cambiar el material del elemento 1 en el array de materiales dependiendo del estado de la c�mara
            var materials = objectRenderer.materials;
            materials[1] = targetCamera.enabled ? activeMaterial : inactiveMaterial;
            objectRenderer.materials = materials; // Reasignar el array modificado
        }
    }
}
