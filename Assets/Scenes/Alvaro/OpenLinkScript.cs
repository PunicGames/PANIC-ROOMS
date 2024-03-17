using UnityEngine;

public class OpenLinkScript : MonoBehaviour
{
    [SerializeField] private string url1;
    [SerializeField] private string url2;
    [SerializeField] private string url3;
    [SerializeField] private string url4;

    // Método llamado al pulsar el primer botón
    public void OpenInstagram()
    {
        Application.OpenURL(url1);
    }

    // Método llamado al pulsar el segundo botón
    public void OpenTwitter()
    {
        Application.OpenURL(url2);
    }

    // Método llamado al pulsar el tercer botón
    public void OpenItchio()
    {
        Application.OpenURL(url3);
    }

    // Método llamado al pulsar el cuarto botón
    public void OpenYoutube()
    {
        Application.OpenURL(url4);
    }
}
