using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Lantern_Bahavior : MonoBehaviour
{
    [HideInInspector] Light spotlight;
    private bool activated = true;
    private float battery_amount = 100.0f;
    private float battery_decrease_rate = 2.2f;
    private float max_battery = 100.0f;

    private InGameUI game_ui;

    private void Awake()
    {
        spotlight = GetComponentInChildren<Light>();
    }

    private void Start()
    {
        game_ui = GameObject.FindGameObjectWithTag("UI").GetComponent<InGameUI>();
    }

    private void Update()
    {
        if (activated)
        {

            // Decrease battery power if on
            battery_amount -= Time.deltaTime * battery_decrease_rate;

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
        battery_amount += 20;
        if(battery_amount > max_battery) battery_amount = max_battery;
    }
}
