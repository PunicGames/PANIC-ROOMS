using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName;

    // GameObjects de la pantalla de carga
    public GameObject loadingScreen; // Referencia al GameObject de la pantalla de carga
    public Slider loadingSlider; // Referencia al Slider que muestra el progreso

    public TextMeshProUGUI titleMenu;

    // GameObjects de los men�s
    public GameObject mainMenu;
    public GameObject customGameMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    // GameObjects para los men�s de cr�ditos
    public GameObject PGCreditsMenu;
    public GameObject JRCreditsMenu;
    public GameObject JSCreditsMenu;
    public GameObject AOCreditsMenu;
    public GameObject AECreditsMenu;

    private GameObject currentMenu; // Almacena el men� actualmente activo
    private GameObject previousMenu; // Almacena el men� anteriormente activo

    private string previousTitle; // Almacena el t�tulo anterior

    void Start()
    {
        // Inicializar el men� principal como el men� actual
        currentMenu = mainMenu;
        previousMenu = null; // No hay men� anterior al inicio
        mainMenu.SetActive(true); // Aseg�rate de que el men� principal est� activo al inicio
    }

    public void PlayGame()
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
            // Actualiza el valor del slider con el progreso de la carga
            // El progreso de asyncLoad va de 0 a 0.9, por eso se divide entre 0.9 para normalizarlo a 1
            loadingSlider.value = asyncLoad.progress / 0.9f;
            yield return null; // Espera un frame antes de continuar
        }
    }

    public void CustomGameButtonClicked()
    {
        SwitchMenu(customGameMenu, "CUSTOM GAME");
    }

    public void OptionsButtonClicked()
    {
        SwitchMenu(optionsMenu, "OPTIONS");
    }

    public void CreditsButtonClicked()
    {
        SwitchMenu(creditsMenu, "CREDITS");
    }

    // M�todos para los botones de cr�ditos espec�ficos
    public void AOCreditsButtonClicked()
    {
        SwitchMenu(AOCreditsMenu, "ALVARO OLAVARRIA");
    }

    public void AECreditsButtonClicked()
    {
        SwitchMenu(AECreditsMenu, "ANTONIO ESPINOSA");
    }

    public void JRCreditsButtonClicked()
    {
        SwitchMenu(JRCreditsMenu, "JAVIER RAJA");
    }

    public void JSCreditsButtonClicked()
    {
        SwitchMenu(JSCreditsMenu, "JAVIER SERRANO");
    }

    public void PGCreditsButtonClicked()
    {
        SwitchMenu(PGCreditsMenu, "PUNIC GAMES");
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
