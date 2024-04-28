using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider gammaSlider;

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
        PlayerPrefs.SetFloat("Gamma", gammaSlider.value);

        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        // Cargar las configuraciones guardadas y aplicarlas a los elementos de UI
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
        gammaSlider.value = PlayerPrefs.GetFloat("Gamma", 1.0f);

        // Aplicar las configuraciones en el juego
        ApplySettings();
    }

    public void ApplySettings()
    {
        // Aqu� deber�as agregar la l�gica para aplicar efectivamente estas configuraciones en tu juego
        // Por ejemplo:
        AudioListener.volume = volumeSlider.value;
        // Para el gamma, necesitar�s una implementaci�n espec�fica basada en c�mo tu juego maneja el brillo o la exposici�n

        // Nota: Aseg�rate de llamar a ApplySettings() tambi�n despu�s de que el usuario haga cambios, no solo al cargar
    }
}

