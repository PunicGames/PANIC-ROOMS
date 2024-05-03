using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName;

    // GameObjects de la pantalla de carga
    public GameObject loadingScreen;
    public Slider loadingSlider;

    public TextMeshProUGUI titleMenu;
    public TMP_InputField seed;

    // GameObjects de los menús
    public GameObject mainMenu;
    public GameObject customGameMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    // GameObjects para los menús de créditos
    public GameObject PGCreditsMenu;
    public GameObject JRCreditsMenu;
    public GameObject JSCreditsMenu;
    public GameObject AOCreditsMenu;
    public GameObject AECreditsMenu;

    private GameObject currentMenu; // Almacena el menú actualmente activo
    private GameObject previousMenu; // Almacena el menú anteriormente activo

    private string previousTitle; // Almacena el título anterior

    void Start()
    {
        // Inicializar el menú principal como el menú actual
        currentMenu = mainMenu;
        previousMenu = null; // No hay menú anterior al inicio
        mainMenu.SetActive(true); // Asegúrate de que el menú principal esté activo al inicio
        PlayerPrefs.SetFloat("RotationSpeed", 1.0f);
    }

    public void PlayGame()
    {
        PlayerPrefs.SetString("Seed", "");
        StartCoroutine(LoadSceneAsync(gameSceneName));
    }

    public void PlayCustomGame()
    {
        PlayerPrefs.SetString("Seed", seed.text);
        StartCoroutine(LoadSceneAsync(gameSceneName));
        print("Seed: " + seed.text);
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

    public void MainMenuButtonClicked()
    {
        SwitchMenu(mainMenu, "MAIN MENU");
    }

    // Métodos para los botones de créditos específicos
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
