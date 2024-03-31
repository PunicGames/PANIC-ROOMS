using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameUI : MonoBehaviour
{

    [SerializeField] private GameObject pause_ui;
    [SerializeField] private GameObject ingame_ui;
    [SerializeField] private TextMeshProUGUI collect_ui;
    private CharacterMovement character_movement;

    public void PauseMenu() {
        //Debug.Log("Pause menu");
        pause_ui.active = true;
        ingame_ui.active = false;
    }


    public void UnpauseMenu()
    {
        //Debug.Log("Unpause menu");
        ingame_ui.active = true;
        pause_ui.active = false;

        if (character_movement != null) { 
            character_movement.SetPauseMenu(false);
        }

    }

    // If main menu button pressed
    public void LoadMainMenu()
    {
        ingame_ui.active = false;
        pause_ui.active = false;

        // Go main menu
        SceneManager.LoadScene("UI-testing");
        //Debug.Log("Coming back to main menu");
    }

    public void ActivateCollectInfo(string message) {
        collect_ui.text = message;
        collect_ui.enabled = true;
    }
    public void DeactivateCollectInfo()
    {
        collect_ui.enabled = false;
    }

    public void InitPlayerDependencies() {
        character_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        Debug.Log("UI Dependencies Initialized");
    }
}
