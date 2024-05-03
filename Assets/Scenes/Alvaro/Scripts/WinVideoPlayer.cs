using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class WinVideoPlayer : MonoBehaviour
{
    public GameObject mainMenuController;
    public VideoPlayer finalVideo;
    public GameObject videoplayer;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("Win") == 1)
        {
            print("viene de victoria");
            mainMenuController.SetActive(false);
            finalVideo.Play();
            StartCoroutine(WaitForVideoEnd());
        }
        else
        {
            print("no viene de victoria");
        }
    }


    private IEnumerator WaitForVideoEnd()
    {
        // Espera a que el video esté listo para reproducirse
        while (!finalVideo.isPrepared)
        {
            Debug.Log("Preparando video...");
            yield return null;
        }

        Debug.Log("Duración del video: " + finalVideo.length);

        // Monitorea el tiempo actual del video hasta que esté cerca del final
        while (finalVideo.time < finalVideo.length * 0.95)  // Verifica hasta el 95% de la duración para evitar pequeños errores de cálculo
        {
            Debug.Log("Tiempo del video: " + finalVideo.time);
            yield return null;
        }

        print("video terminado, que see activer el UI");
        PlayerPrefs.SetInt("Win", 0);
        
        videoplayer.SetActive(false);
        mainMenuController.SetActive(true);
    }
}
