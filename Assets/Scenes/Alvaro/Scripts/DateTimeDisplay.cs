using UnityEngine;
using TMPro; // Asegúrate de incluir el espacio de nombres de TextMeshPro

public class DateTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI seedText;

    public GameManager gameManager;

    private void Start()
    {
        if (gameManager != null)
        {
            print("gameManager detected");
            seedText.text = "SEED: " + gameManager.actualSeed;
        }
    }
    void Update()
    {
        // Actualiza el texto con la hora actual del sistema
        timeText.text = System.DateTime.Now.ToString("HH:mm:ss");

        // Actualiza el texto con la fecha actual del sistema
        dateText.text = System.DateTime.Now.ToString("MMM dd yyyy").ToUpper();
    }
}
