using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class CharacterMovement : MonoBehaviour
{

    // *** Movement related variables ***
        // Basic movement
    private Vector2 player_translation;
    private float translation_speed = 0.5f;
    private bool is_running = false;
    private bool is_stealth = false;
    private bool is_crouch = false;
    // Camera movement
    private Vector2 camera_movement;
    private float camera_vertical_sensitivity = 0.04f;
    private float camera_horizontal_sensitivity = 5.0f;
    // Camera
    [SerializeField] private CinemachineVirtualCamera virtual_camera;
    private CinemachinePOV pov;
    private CinemachineBasicMultiChannelPerlin noise;
    private float target_camera_noise_amplitude_gain;
    private float target_camera_noise_frecuency_gain;
    private float camera_noise_transition_speed = 1.0f;
    [SerializeField] private Transform raised_camera_transform;
    [SerializeField] private Transform crouch_camera_transform;
    private float camera_transition_speed = 3.5f;

    // Jumping
    private float jump_force = 7.0f;
    private bool jump_trigger;

    // *** Stats ***
    private float player_health = 100;
    private bool finished_round = true;
    private float tension = 0.0f;
    private float base_tension = 0.0f;
    private float movement_tension = 0.0f;
    private float max_tension = 100.0f;
    private float min_tension = 0.0f;

    // *** Lantern ***
    [SerializeField] Lantern_Bahavior lantern_behavior;
    bool activate_lantern = false;

    // *** UI ***
    private InGameUI game_ui;
    private bool menu_paused = false;


    // *** Other componenets ***
    [SerializeField] Animator animator;
    [SerializeField] CharacterSounds character_sounds_manager;
    public bool walk_sound_trigger = false;
    [SerializeField] private CharacterCollectionSystem character_collection_system;

    public void OnMovement(InputValue input) {
        if (!finished_round) return;
        if (menu_paused) return;
        player_translation = input.Get<Vector2>();
    }

    public void OnLook(InputValue input) {
        if (!finished_round) return;
        if (menu_paused) return;

        camera_movement = input.Get<Vector2>();
        pov.m_VerticalAxis.Value -= camera_movement.y * camera_vertical_sensitivity;
    }

    public void OnJump(InputValue input)
    {
        //jump_trigger = input.isPressed;
        character_collection_system.CheckTV();
    }

    public void OnRun(InputValue input)
    {
        is_running = input.isPressed;

        if (is_running)
        {
            is_stealth = false;
            is_crouch = false;

            // Values related with running movement
            translation_speed = 1.20f;

            // Apply sounds variations
            character_sounds_manager.LoadRunningSound();
        }
        else {
            // Values related with normal movement
            translation_speed = 0.5f;

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
            is_crouch = false;

            // Values related with stealth movement
            translation_speed = 0.3f;

            // Apply sounds variations
            character_sounds_manager.LoadStealthSound();
            
        }
        else {
            // Values related with normal movement
            translation_speed = 0.5f;

            // Apply sounds variations
            character_sounds_manager.LoadWalkingSound();
        }
    }

    public void OnCrouch(InputValue input) {
        is_crouch = !is_crouch;

        if (is_crouch)
        {
            is_running = false;
            is_stealth = false;

            // Values related with stealth movement
            translation_speed = 0.25f;

            // Apply sounds variations. Stealth is fine for crouching
            character_sounds_manager.LoadStealthSound();
        }
        else {
            // Values related with normal movement
            translation_speed = 0.5f;

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
        if(!finished_round) { return; }

        menu_paused = !menu_paused;

        if (menu_paused)
        {
            UnlockCursor();
            game_ui.PauseMenu();
            player_translation = Vector2.zero;
            camera_movement = Vector2.zero;
            pov.m_VerticalAxis.Value = camera_movement.y;
            pov.m_HorizontalAxis.Value = camera_movement.x;
        }
        else if (!menu_paused) {
            LockCursor();
            game_ui.UnpauseMenu();
        }
    }

    public void OnCollect(InputValue input) {
        character_collection_system.CollectObject();
    }

    private void Awake()
    {
        LockCursor();
    }

    void Start()
    {
        game_ui = GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>();
        pov = virtual_camera.GetCinemachineComponent<CinemachinePOV>();
        noise = virtual_camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Camera.main.transform.position = raised_camera_transform.position;

        // Activate lantern by default
        lantern_behavior.ActivateLantern();
    }

    void Update()
    {

        //Set animator
        if(player_translation != Vector2.zero)
            animator.SetBool("moving", true);
        else
            animator.SetBool("moving", false);

        if (is_crouch)
            animator.SetBool("crouched", true);
        else
            animator.SetBool("crouched", false);

        // Apply basic translation (WASD)
        float translation_factor = Time.deltaTime * translation_speed;
        transform.Translate(translation_factor * player_translation.x, 0, translation_factor * player_translation.y);


        // Apply player rotation (MOUSE)
        Vector2 delta_look = camera_movement * camera_horizontal_sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, delta_look.x);

        // Apply camera swing effect
        ApplySwingEffect();


        // Camera position (crouch-normal) transition
        Vector3 camera_target = is_crouch ? crouch_camera_transform.position : raised_camera_transform.position;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, camera_target, camera_transition_speed * Time.deltaTime);

        // Camera noise transition
        if (noise != null)
        {
            noise.m_AmplitudeGain = Mathf.Lerp(noise.m_AmplitudeGain, target_camera_noise_amplitude_gain, Time.deltaTime * camera_noise_transition_speed);
            noise.m_FrequencyGain = Mathf.Lerp(noise.m_FrequencyGain, target_camera_noise_frecuency_gain, Time.deltaTime * camera_noise_transition_speed);
        }


        // Check if out of bounds (z-death)
        if (transform.position.y <= -1.0f) {
            GameManager.Instance.RealocatePlayer();
        }

        // Manage tension
        if (is_running && !IsMoving()) IncreaseTension(3.0f * Time.deltaTime);
        else IncreaseTension((-1) * 3.0f * Time.deltaTime);
    }


    private void ApplySwingEffect()
    {
        if (player_translation.x != 0 || player_translation.y != 0)
        {
            if (is_running)
            {
                target_camera_noise_amplitude_gain = 1.3f;
                target_camera_noise_frecuency_gain = 1.7f;
            }
            else if (is_stealth)
            {
                target_camera_noise_amplitude_gain = 0.8f;
                target_camera_noise_frecuency_gain = 0.8f;
            } else if (is_crouch) 
            {
                target_camera_noise_amplitude_gain = 0.6f;
                target_camera_noise_frecuency_gain = 0.6f;
            }
            else
            {
                target_camera_noise_amplitude_gain = 1.1f;
                target_camera_noise_frecuency_gain = 1.1f;
            }
            

            if (!walk_sound_trigger)
            {
                // Triggers one step sound
                character_sounds_manager.PlayMovingSound();
                walk_sound_trigger = true;
            }
        }

        // Not moving
        if (IsMoving())
        {
            target_camera_noise_amplitude_gain = 0.5f;
            target_camera_noise_frecuency_gain = 0.5f;
        }

        // Reset walk sound trigger if stopped
        if (IsMoving() && walk_sound_trigger)
        {
            walk_sound_trigger = false;
        }
    }

    private bool IsMoving() {
        return player_translation.x == 0 && player_translation.y == 0;
    }
    public void SetHealth(float new_value) {
        player_health = new_value;
    }

    public float GetHealth() {
        return player_health;
    }

    public void SetPauseMenu(bool option) { 
        menu_paused = option;
    }

    public void WinGame() {
        finished_round = false;
        player_translation = Vector2.zero;
        camera_movement = Vector2.zero;
        pov.m_VerticalAxis.Value = camera_movement.y;
        pov.m_HorizontalAxis.Value = camera_movement.x;
        menu_paused = true;
        UnlockCursor();
        game_ui.PauseMenu();
    }

    public void LoseGame() {
        finished_round = false;
        player_translation = Vector2.zero;
        camera_movement = Vector2.zero;
        pov.m_VerticalAxis.Value = camera_movement.y;
        pov.m_HorizontalAxis.Value = camera_movement.x;
        menu_paused = true;
        UnlockCursor();
        game_ui.PauseMenu();
    }

    private void IncreaseTension(float _tension) {
        movement_tension += _tension;
        if (movement_tension > GetMaxTension()) movement_tension = GetMaxTension();
        if(movement_tension < GetMinTension()) movement_tension = GetMinTension();

        tension = movement_tension + base_tension;

        if (tension > GetMaxTension()) tension = GetMaxTension();
        if (tension < GetMinTension()) tension = GetMinTension();

        //Debug.Log("Tension: " + tension);
    }
    public float GetTension() {
        return tension;
    }
    public float GetMaxTension()
    {
        return max_tension;
    }
    public float GetMinTension()
    {
        return min_tension;
    }
    public void SetBaseTension(float val)
    {
        base_tension = val;
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
