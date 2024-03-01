using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName;

    // GameObjects de los menús que quieras activar/desactivar
    public GameObject mainMenu;
    public GameObject customGameMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    public TextMeshProUGUI titleMenu;

    public void PlayGame()
    {
        // Carga la escena del juego
        SceneManager.LoadScene(gameSceneName);
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

    private void SwitchMenu(GameObject menuToActivate, string title)
    {
        // Desactiva el menú principal y activa el menú pasado como parámetro
        mainMenu.SetActive(false);
        menuToActivate.SetActive(true);

        // Actualiza el texto del título
        titleMenu.text = title;
    }

    // Puedes llamar a este método desde otros scripts para volver al menú principal
    public void ReturnToMainMenu()
    {
        customGameMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
        titleMenu.text = "MAIN MENU";
    }
}
