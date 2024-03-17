using UnityEngine;

public class OpenLinkScript : MonoBehaviour
{
    [SerializeField] private string url1;
    [SerializeField] private string url2;
    [SerializeField] private string url3;
    [SerializeField] private string url4;

    // M�todo llamado al pulsar el primer bot�n
    public void OpenInstagram()
    {
        Application.OpenURL(url1);
    }

    // M�todo llamado al pulsar el segundo bot�n
    public void OpenTwitter()
    {
        Application.OpenURL(url2);
    }

    // M�todo llamado al pulsar el tercer bot�n
    public void OpenItchio()
    {
        Application.OpenURL(url3);
    }

    // M�todo llamado al pulsar el cuarto bot�n
    public void OpenYoutube()
    {
        Application.OpenURL(url4);
    }
}
