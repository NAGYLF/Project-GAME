using MainData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutScenes : MonoBehaviour
{
    public static float fadeDurationStart = 1.5f;
    public static float fadeDurationEnd = 1.5f;

    public GameObject videoObj;
    private static bool end = false;
    private VideoPlayer videoPlayer;
    public GameObject fadeOutScreen;
    public string scene = "";
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = Main.targetFPS;
        videoPlayer = videoObj.GetComponent<VideoPlayer>();
        StartCoroutine(StartFadeOutScreen());
    }
    private void Update()
    {
        if ((videoPlayer.length*1000-videoPlayer.time*1000 == fadeDurationEnd*1000) || end) 
        {
           StartCoroutine(EndFadeOutScreen());
           end = false;
        }

    }
    public static void CutSceneManualEnd()
    {
        end = true;
    }
    public IEnumerator EndFadeOutScreen()
    {
        Debug.Log("cutscene Ending start");
        float startVolume = videoPlayer.GetDirectAudioVolume(0); ; // Kezdõ hangerõ
        float elapsedTime = 0f;

        UnityEngine.UI.Image image = fadeOutScreen.GetComponent<UnityEngine.UI.Image>();

        if (image != null)
        {
            fadeOutScreen.GetComponentInChildren<UnityEngine.UI.Image>().enabled = true;
            image.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Debug.LogError("Image component not found!");
        }



        while (elapsedTime < fadeDurationStart)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDurationStart);
            image.color = new Color(0, 0, 0, alpha);


            float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDurationStart);
            videoPlayer.SetDirectAudioVolume(0, newVolume);
            elapsedTime += Time.deltaTime;
            yield return null; // Várj egy frame-et
        }

        // Állítsd be a hangerõt 0-ra véglegesen
        videoPlayer.SetDirectAudioVolume(0, 0);
        image.color = new Color(0, 0, 0, 255);
        SceneManager.LoadScene(scene);

    }
    public IEnumerator StartFadeOutScreen()
    {
        Debug.Log("cutscene start");
        // Állítsd be a hangerõt 0-ra, hogy biztos legyél benne, hogy elindul
        videoPlayer.SetDirectAudioVolume(0, 0);
        float startVolume = 0; // Kezdõ hangerõ (0)
        float targetVolume = 1; // Maximális hangerõ (1)
        float elapsedTime = 0f;

        UnityEngine.UI.Image image = fadeOutScreen.GetComponent<UnityEngine.UI.Image>();

        if (image != null)
        {
            image.color = new Color(0, 0, 0, 255); // Kezdõ alpha érték (1)
        }
        else
        {
            Debug.LogError("Image component not found!");
            yield break; // Ha nincs Image, lépj ki
        }

        while (elapsedTime < fadeDurationEnd)
        {
            // Alpha 255-tõl 0-ig csökken
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDurationEnd); // Alpha 1-tõl 0-ig
            image.color = new Color(0, 0, 0, alpha);

            // Hangerõ 0-tól 1-ig emelkedik
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDurationEnd);
            videoPlayer.SetDirectAudioVolume(0, newVolume);

            elapsedTime += Time.deltaTime;
            yield return null; // Várj egy frame-et
        }

        // Állítsd be a hangerõt a maximálisra véglegesen
        videoPlayer.SetDirectAudioVolume(0, targetVolume);
        image.color = new Color(0, 0, 0, 0); // Alpha 0-ra állítása
    }
}
