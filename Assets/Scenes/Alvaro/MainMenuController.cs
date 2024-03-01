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

    public void PlayGame()
    {
        // Carga la escena del juego
        SceneManager.LoadScene(gameSceneName);
    }

    public void SwitchMenu(GameObject menuToActivate)
    {
        // Desactiva el menú principal y activa el menú pasado como parámetro
        mainMenu.SetActive(false);
        menuToActivate.SetActive(true);
    }

    // Puedes llamar a este método desde otros scripts para volver al menú principal
    public void ReturnToMainMenu()
    {
        customGameMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
