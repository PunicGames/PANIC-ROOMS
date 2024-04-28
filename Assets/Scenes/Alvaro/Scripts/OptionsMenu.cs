using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider rotationSpeedSlider;

    void OnEnable()
    {
        LoadSettings();
    }

    void OnDisable()
    {
        SaveSettings();
    }

    private void Update()
    {
        // Aplicar las configuraciones en el juego
        ApplySettings();
    }

    public void SaveSettings()
    {
        // Guardar las configuraciones actuales
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        PlayerPrefs.SetFloat("RotationSpeed", rotationSpeedSlider.value);

        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // Cargar las configuraciones guardadas y aplicarlas a los elementos de UI
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
        rotationSpeedSlider.value = PlayerPrefs.GetFloat("RotationSpeed", 1.0f);

        // Aplicar las configuraciones en el juego
        ApplySettings();
    }

    public void ApplySettings()
    {
        AudioListener.volume = volumeSlider.value;
    }
}

