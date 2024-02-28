using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{

    // *** Movement related variables ***
        // Basic movement
    private Vector2 player_translation;
    private float translation_speed = 1.0f;
    private bool is_moving = false;
    private bool is_running = false;
    private bool is_stealth = false;
    // Camera movement
    [HideInInspector] Camera camera;
    private Vector2 camera_movement;
    private float camera_speed = 5.0f;
    private float camera_pitch = 0.0f;
    // Camera swing effect
    private Vector3 initial_camera_position;
    public float swing_frequency = 4.2f;
    public float swing_horizontal_amplitude = 0.1f;
    public float swing_vertical_amplitude = 0.1f;
    private float swing_timer = 0.0f;
    // Jumping
    private float jump_force = 7.0f;
    private bool jump_trigger;

    // *** Stats ***
    private float player_health = 100;

    // *** Lantern ***
    [SerializeField] Lantern_Bahavior lantern_behavior;
    bool activate_lantern = true;

    // *** UI ***
    [SerializeField] private InGameUI game_ui;
    private bool menu_paused = false;


    // *** Other componenets ***
    [SerializeField] CharacterSounds character_sounds_manager;

    public void OnMovement(InputValue input) {
        if (menu_paused) return;
        player_translation = input.Get<Vector2>();
    }

    public void OnLook(InputValue input) {
        if (menu_paused) return;
        camera_movement = input.Get<Vector2>();
    }

    public void OnJump(InputValue input)
    {
        jump_trigger = input.isPressed;
    }

    public void OnRun(InputValue input)
    {
        is_running = input.isPressed;

        if (is_running)
        {
            is_stealth = false;

            // Values related with running movement
            translation_speed = 2.0f;
            swing_frequency = 5.2f;
            swing_horizontal_amplitude = 0.15f;
            swing_vertical_amplitude = 0.15f;

            // Apply sounds variations
            character_sounds_manager.LoadRunningSound();
        }
        else {
            // Values related with normal movement
            translation_speed = 1.0f;
            swing_frequency = 4.2f;
            swing_horizontal_amplitude = 0.1f;
            swing_vertical_amplitude = 0.1f;

            // Apply sounds variations
            character_sounds_manager.LoadWalkingSound();
        }
    }

    public void OnStealth(InputValue input) 
    { 
        is_stealth = input.isPressed;

        if (is_stealth)
        {
            is_running = false;

            // Values related with stealth movement
            translation_speed = 0.6f;
            swing_frequency = 2.2f;
            swing_horizontal_amplitude = 0.07f;
            swing_vertical_amplitude = 0.07f;

            // Apply sounds variations
            character_sounds_manager.LoadStealthSound();
            
        }
        else {
            // Values related with normal movement
            translation_speed = 1.0f;
            swing_frequency = 4.2f;
            swing_horizontal_amplitude = 0.1f;
            swing_vertical_amplitude = 0.1f;

            // Apply sounds variations
            character_sounds_manager.LoadWalkingSound();
        }
    }


    public void OnLantern(InputValue input) {
        activate_lantern = !activate_lantern;

        if (activate_lantern)
        {
            lantern_behavior.ActivateLantern();
            character_sounds_manager.PlayLanternActivateSound();
        }
        else { 
            lantern_behavior.DeactivateLantern();
            character_sounds_manager.PlayLanternDeactivateSound();
        }
    }


    public void OnPauseMenu(InputValue input) { 
        menu_paused = !menu_paused;

        if (menu_paused)
        {
            UnlockCursor();
            game_ui.PauseMenu();
            player_translation = Vector2.zero;
            camera_movement = Vector2.zero;
        }
        else if (!menu_paused) {
            LockCursor();
            game_ui.UnpauseMenu();
        }
    }


    private void Awake()
    {
        LockCursor();
    }

    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        initial_camera_position = camera.transform.localPosition;

        // Activate lantern by default
        lantern_behavior.ActivateLantern();
    }

    void Update()
    {

        // Apply basic translation (WASD)
        float translation_factor = Time.deltaTime * translation_speed;
        transform.Translate(translation_factor * player_translation.x, 0, translation_factor * player_translation.y);


        // Apply camera movement and player rotation (MOUSE)
        Vector2 delta_look = camera_movement * camera_speed * Time.deltaTime;
        transform.Rotate(Vector3.up, delta_look.x);
        // Threshold for the camera to prevent gimbal lock
        camera_pitch -= delta_look.y;
        camera_pitch = Mathf.Clamp(camera_pitch, -85.0f, 85.0f);
        camera.transform.localEulerAngles = new Vector3(camera_pitch, 0, 0);
        // Apply camera swing effect
        ApplySwingEffect();
    }


    private void ApplySwingEffect() {
        if (player_translation.x != 0 || player_translation.y != 0)
        {
            if (!is_moving)
            {
                // Start playing footstep sounds
                character_sounds_manager.PlayMovingSound();
                is_moving = true;
            }

            swing_timer += Time.deltaTime * swing_frequency;

            // Calculate the horizontal and vertical offsets
            float horizontal_offset = Mathf.Sin(swing_timer) * swing_horizontal_amplitude;
            float vertical_offset = Mathf.Cos(swing_timer * 2) * swing_vertical_amplitude;

            // Apply the offsets to the camera position
            camera.transform.localPosition = new Vector3(horizontal_offset, vertical_offset, 0) + initial_camera_position;
        }
        else
        {
            if (is_moving)
            {
                // Stop playing footstep sounds when the player stops moving
                character_sounds_manager.StopMovingSound();
                is_moving = false;
            }

            // Reset the bobTimer if the player is not moving to avoid sudden jumps in camera position
            swing_timer = Mathf.PI / 2;
            // Reset to the initial position when not moving
            camera.transform.localPosition = initial_camera_position;
        }
    }


    public void SetHealth(float new_value) {
        player_health = new_value;
        game_ui.UpdateHealth(player_health);
    }

    public float GetHealth() {
        return player_health;
    }

    public void SetPauseMenu(bool option) { 
        menu_paused = option;
    }


    private void LockCursor()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the cursor from view
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        // Unlock the cursor so it can be moved freely
        Cursor.lockState = CursorLockMode.None;
        // Make the cursor visible
        Cursor.visible = true;
    }

}
