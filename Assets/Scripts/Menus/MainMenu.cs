using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEngine.UI;
using MainData;//Main

public class MainMenu : MonoBehaviour
{
    public static float fadeDuration = 1.5f;
    public static GameObject secondMenu;
    void Start()
    {
        //Aplication Start
        Debug.Log("Aplication Start");
        //beallitjuk a target fps-t
        Application.targetFrameRate = Main.targetFPS;
        //ez az az obecjtum amely a new game startra kattintva a fekete elhomalyosodasert felel
        GameObject.Find("FadeOutScreen").GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;
        //ez a bejelentkezesi kis felugro menu ami alapbol inactiv
        secondMenu = GameObject.Find("SecondMenu");
        secondMenu.SetActive(false);
        //a player adatait a MainLoadScreenben betoltottuk. Itt a profil objectumon beallitjuk a profil kezdobetujet ami a Adminnel: T
        if (Main.playerData != null)
        {
            UIFunctions.ProfileBTStyle();
        }
    }
    //az elhomályosodás eljarasa
    private IEnumerator StartFadeOutScreen(VideoPlayer videoPlayer,GameObject fadeOutScreen)
    {
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

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            image.color = new Color(0, 0, 0, alpha);


            float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            videoPlayer.SetDirectAudioVolume(0,newVolume);
            elapsedTime += Time.deltaTime;
            yield return null; // Várj egy frame-et
        }

        // Állítsd be a hangerõt 0-ra véglegesen
        videoPlayer.SetDirectAudioVolume(0,0);
        image.color = new Color(0, 0, 0, 255);
        SceneManager.LoadScene("NewGameCutScene");
    }
    //uj jatek kezdete
    public void NewGameBt()
    {
        if (Main.playerData != null)
        {
            VideoPlayer videoPlayer = GameObject.FindWithTag("VideoPlayerMainMenu").GetComponent<VideoPlayer>();
            GameObject fadeOutScreen = GameObject.Find("FadeOutScreen");

            if (videoPlayer != null)
            {
                Debug.Log("VideoPlayer found!");
                StartCoroutine(StartFadeOutScreen(videoPlayer, fadeOutScreen));

            }
            else
            {
                Debug.Log("No VideoPlayer component found on this object.");
            }
        }
        else
        {
            //Debug.LogError("The New game wasn't started because the user hadn't logined");
            secondMenu.SetActive(true);
        }
    }
    public void OptionsBt()
    {

    }
    public void LoadBt()
    {

    }
    public void ExitBt()
    {
        Debug.Log("Quit was happened");
        Application.Quit();
    }
    public void AboutBT()
    {

    }
    public void ProfileBT()
    {
        secondMenu.SetActive(true);
    }
}
