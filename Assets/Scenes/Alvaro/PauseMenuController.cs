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

    // GameObjects de los men�s
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    private GameObject currentMenu; // Almacena el men� actualmente activo
    private GameObject previousMenu; // Almacena el men� anteriormente activo

    private string previousTitle; // Almacena el t�tulo anterior

    void Start()
    {
        // Inicializar el men� principal como el men� actual
        currentMenu = pauseMenu;
        previousMenu = null; // No hay men� anterior al inicio
        pauseMenu.SetActive(true); // Aseg�rate de que el men� principal est� activo al inicio
    }

    public void SwitchScene()
    {
        StartCoroutine(LoadSceneAsync(gameSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        // Desactiva el men� actual y muestra la pantalla de carga
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

            currentMenu.SetActive(false); // Desactivar el men� actual
            currentMenu = menuToActivate; // Actualizar el men� actual
            currentMenu.SetActive(true); // Activar el nuevo men�

            titleMenu.text = title; // Actualizar el t�tulo
        }
    }

    public void ReturnToPreviousMenu()
    {
        if (previousMenu != null)
        {
            currentMenu.SetActive(false); // Desactivar el men� actual
            currentMenu = previousMenu; // Cambiar al men� anterior
            currentMenu.SetActive(true); // Activar el men� anterior

            titleMenu.text = previousTitle; // Restablecer el t�tulo anterior
        }
    }
}
