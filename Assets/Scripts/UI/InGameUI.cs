using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{

    [SerializeField] private GameObject pause_ui;
    [SerializeField] private GameObject ingame_ui;
    [SerializeField] private Slider health_slider;
    [SerializeField] private CharacterMovement character_movement;

    public void PauseMenu() {
        Debug.Log("Pause menu");
        pause_ui.active = true;
        ingame_ui.active = false;
    }


    public void UnpauseMenu()
    {
        Debug.Log("Unpause menu");
        ingame_ui.active = true;
        pause_ui.active = false;
        character_movement.SetPauseMenu(false);

    }

    public void LoadMainMenu()
    {
        ingame_ui.active = false;
        pause_ui.active = false;

        // Go main menu
        Debug.Log("Coming back to main menu");
    }

    public void UpdateHealth(float health_player) {
        health_slider.value = health_player;
    }

}
