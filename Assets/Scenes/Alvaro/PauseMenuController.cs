using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public string gameSceneName;

    // GameObjects de la pantalla de carga
    public GameObject loadingScreen;
    public Slider loadingSlider;

    public TextMeshProUGUI titleMenu;

    // GameObjects de los menús
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    private GameObject currentMenu; // Almacena el menú actualmente activo
    private GameObject previousMenu; // Almacena el menú anteriormente activo

    private string previousTitle; // Almacena el título anterior

    void Start()
    {
        // Inicializar el menú principal como el menú actual
        currentMenu = pauseMenu;
        previousMenu = null; // No hay menú anterior al inicio
        pauseMenu.SetActive(true); // Asegúrate de que el menú principal esté activo al inicio
    }

    public void SwitchScene()
    {
        StartCoroutine(LoadSceneAsync(gameSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        // Desactiva el menú actual y muestra la pantalla de carga
        currentMenu.SetActive(false);
        loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Mientras la escena no haya terminado de cargar
        while (!asyncLoad.isDone)
        {
            loadingSlider.value = asyncLoad.progress / 0.9f;
            print("Loading value: " + loadingSlider.value);
            yield return null; // Espera un frame antes de continuar
        }
    }

    public void OptionsButtonClicked()
    {
        SwitchMenu(optionsMenu, "OPTIONS");
    }

    private void SwitchMenu(GameObject menuToActivate, string title)
    {
        if (currentMenu != menuToActivate)
        {
            previousMenu = currentMenu;
            previousTitle = titleMenu.text;

            currentMenu.SetActive(false); // Desactivar el menú actual
            currentMenu = menuToActivate; // Actualizar el menú actual
            currentMenu.SetActive(true); // Activar el nuevo menú

            titleMenu.text = title; // Actualizar el título
        }
    }

    public void ReturnToPreviousMenu()
    {
        if (previousMenu != null)
        {
            currentMenu.SetActive(false); // Desactivar el menú actual
            currentMenu = previousMenu; // Cambiar al menú anterior
            currentMenu.SetActive(true); // Activar el menú anterior

            titleMenu.text = previousTitle; // Restablecer el título anterior
        }
    }
}
