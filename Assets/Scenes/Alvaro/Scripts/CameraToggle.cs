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

    private bool able_to_switch = false;

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







    public void SetCameraTarget(Camera _cam) { 
        targetCamera = _cam;
    }

    public void SwitchTV(bool distance = true)
    {
        able_to_switch = !able_to_switch;
        var materials = objectRenderer.materials;

        // Shut down by distance 
        if (!distance && able_to_switch) able_to_switch = false;

        if (able_to_switch)
        {
            targetCamera.enabled = true;
            materials[1] = activeMaterial;
        }
        else {
            targetCamera.enabled = false;
            materials[1] = inactiveMaterial;
        }
        objectRenderer.materials = materials;
    }
}
