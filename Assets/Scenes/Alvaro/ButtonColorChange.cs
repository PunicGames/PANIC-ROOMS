using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Color normalColor;
    public Color hoverColor;
    public FontStyles normalFontStyle;
    public FontStyles hoverFontStyle;
    public string normalText;
    public string hoverText;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null)
        {
            buttonText.color = hoverColor;
            buttonText.fontStyle = hoverFontStyle;
            buttonText.text = hoverText;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button != null)
        {
            buttonText.color = normalColor;
            buttonText.fontStyle = normalFontStyle;
            buttonText.text = normalText;
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            buttonText.color = normalColor;
            buttonText.fontStyle = normalFontStyle;
            buttonText.text = normalText;
        }
    }
}
