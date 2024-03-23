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
        // Inicializa con el material inactivo
        objectRenderer.material = inactiveMaterial;
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Verificar si el jugador presion� la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Invertir el estado de activaci�n de la c�mara
            targetCamera.enabled = !targetCamera.enabled;

            // Cambiar el material del objeto dependiendo del estado de la c�mara
            objectRenderer.material = targetCamera.enabled ? activeMaterial : inactiveMaterial;
        }
    }
}
