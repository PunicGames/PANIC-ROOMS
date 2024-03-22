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
        // Inicializa con el material inactivo
        objectRenderer.material = inactiveMaterial;
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Verificar si el jugador presionó la tecla espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Invertir el estado de activación de la cámara
            targetCamera.enabled = !targetCamera.enabled;

            // Cambiar el material del objeto dependiendo del estado de la cámara
            objectRenderer.material = targetCamera.enabled ? activeMaterial : inactiveMaterial;
        }
    }
}
