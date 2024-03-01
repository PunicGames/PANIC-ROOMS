using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName;

    // GameObjects de los men�s que quieras activar/desactivar
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
        // Desactiva el men� principal y activa el men� pasado como par�metro
        mainMenu.SetActive(false);
        menuToActivate.SetActive(true);
    }

    // Puedes llamar a este m�todo desde otros scripts para volver al men� principal
    public void ReturnToMainMenu()
    {
        customGameMenu.SetActive(false);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}
