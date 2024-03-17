using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f; // Controla la velocidad de aparición del texto

    private TextMeshProUGUI tmpText;
    private string fullText; // El texto completo que se mostrará

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>(); // Obtiene el componente TextMeshProUGUI
        fullText = tmpText.text; // Almacena el texto completo
    }

    void OnEnable()
    {
        tmpText.text = ""; // Limpia el texto inicialmente
        StartCoroutine(TypeText()); // Comienza la coroutine para mostrar el texto
    }

    IEnumerator TypeText()
    {
        foreach (char letter in fullText.ToCharArray()) // Itera sobre cada letra del texto completo
        {
            tmpText.text += letter; // Añade la letra actual al texto del TMP
            yield return new WaitForSeconds(typingSpeed); // Espera antes de continuar con la siguiente letra
        }
    }
}
