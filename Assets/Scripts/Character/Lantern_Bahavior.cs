using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lantern_Bahavior : MonoBehaviour
{
    [HideInInspector] Light spotlight;
    private bool activated = true;
    private float battery_amount = 100.0f;
    private float battery_decrease_rate = 1.0f;
    private float max_battery = 100.0f;
    private float battery_recharge_amount = 30.0f;
    private float light_intensity;
    private float flicker_intensity = 0.2f;
    private bool trigger_flicker = true;

    // Sounds
    [SerializeField] private AudioSource flicker_sound;
    [SerializeField] private AudioClip[] flicker_sound_clips;

    //  UI
    private InGameUI game_ui;

    private void Awake()
    {
        spotlight = GetComponentInChildren<Light>();
    }

    private void Start()
    {
        game_ui = GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>();
        light_intensity = spotlight.intensity;
        if (flicker_sound_clips.Length > 0) {
            flicker_sound.clip = flicker_sound_clips[0];
        }
    }

    private void Update()
    {
        if (activated)
        {

            // Decrease battery power if on
            battery_amount -= Time.deltaTime * battery_decrease_rate;

            // Decrease light intensity
            if (spotlight != null) { 
                spotlight.intensity = battery_amount  * 0.01f * light_intensity;

                // Random flicker
                if (battery_amount <= flicker_intensity * max_battery) {
                    if (trigger_flicker) {
                        trigger_flicker = false;
                        StartCoroutine(Flicker());
                    }
                }
            }

            // Deactivate lantern if it runs out of battery
            if (battery_amount <= 0) {
                battery_amount = 0;
                DeactivateLantern();
            }

            game_ui.UpdateBattery(battery_amount);
        }
    }

    public void ActivateLantern()
    {
        spotlight.enabled = true;
        activated = true;
    }
    public void DeactivateLantern() { 
        
        spotlight.enabled = false;
        activated = false;
    }

    public void RechargeBattery() {
        battery_amount += battery_recharge_amount;
        if(battery_amount > max_battery) battery_amount = max_battery;
    }


    IEnumerator Flicker() {
        spotlight.enabled = false;

        // Play random flicker sound if not playing a sound already
        if(flicker_sound.isPlaying == false){
            flicker_sound.clip = flicker_sound_clips[Random.Range(0, flicker_sound_clips.Length)];
            flicker_sound.Play();
        }

        // Flicker time ranges
        yield return new WaitForSeconds(Random.Range(0.0f, 0.2f));
        spotlight.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.2f, 1.5f));
        trigger_flicker = true;
    }
}
