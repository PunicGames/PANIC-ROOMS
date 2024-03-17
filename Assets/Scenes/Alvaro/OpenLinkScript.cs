using UnityEngine;

public class OpenLinkScript : MonoBehaviour
{
    [SerializeField] private string url1;
    [SerializeField] private string url2;
    [SerializeField] private string url3;
    [SerializeField] private string url4;

    public void OpenUrl1()
    {
        Application.OpenURL(url1);
    }

    public void OpenUrl2()
    {
        Application.OpenURL(url2);
    }

    public void OpenUrl3()
    {
        Application.OpenURL(url3);
    }

    public void OpenUrl4()
    {
        Application.OpenURL(url4);
    }
}
