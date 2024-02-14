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
    // Camera movement
    [HideInInspector] Camera camera;
    private Vector2 camera_movement;
    private float camera_speed = 5.0f;
    private float camera_pitch = 0.0f;
    // Camera swing effect
    private Vector3 initial_camera_position;
    public float swing_frequency = 5f;
    public float swing_horizontal_amplitude = 0.1f;
    public float swing_vertical_amplitude = 0.1f;
    private float swing_timer = 0.0f;
    // Jumping
    private float jump_force = 7.0f;
    private bool jump_trigger;


    // *** Lantern ***
    [SerializeField] Lantern_Bahavior lantern_behavior;
    bool activate_lantern = true;

    public void OnMovement(InputValue input) {
        player_translation = input.Get<Vector2>();
    }

    public void OnLook(InputValue input) {
        camera_movement = input.Get<Vector2>();
    }

    public void OnJump(InputValue input)
    {
        jump_trigger = input.isPressed;
    }

    public void OnLantern(InputValue input) {
        activate_lantern = !activate_lantern;

        if (activate_lantern)
        {
            lantern_behavior.ActivateLantern();
        }
        else { 
            lantern_behavior.DeactivateLantern();
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
            swing_timer += Time.deltaTime * swing_frequency;

            // Calculate the horizontal and vertical offsets
            float horizontal_offset = Mathf.Sin(swing_timer) * swing_horizontal_amplitude;
            float vertical_offset = Mathf.Cos(swing_timer * 2) * swing_vertical_amplitude;

            // Apply the offsets to the camera position
            camera.transform.localPosition = new Vector3(horizontal_offset, vertical_offset, 0) + initial_camera_position;
        }
        else
        {
            // Reset the bobTimer if the player is not moving to avoid sudden jumps in camera position
            swing_timer = Mathf.PI / 2;
            // Reset to the initial position when not moving
            camera.transform.localPosition = initial_camera_position;
        }
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
