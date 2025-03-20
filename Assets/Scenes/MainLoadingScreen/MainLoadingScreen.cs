using MainData;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainLoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen; // A loading képernyõ GameObject
    public Slider progressBar; // A folyamat jelzésére szolgáló csúszka

    void Start()
    {
        StartCoroutine(LoadDataAndScene());
    }

    IEnumerator LoadDataAndScene()
    {
        loadingScreen.SetActive(true);


        // Progress bar animáció (2 másodperc alatt 0-ról 100%-ra)
        yield return StartCoroutine(FillProgressBar(50));
        yield return StartCoroutine(LoadData());
        yield return StartCoroutine(FillProgressBar(100));


        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Menu");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f); // Skálázás 0-100%-ig
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // Most aktiváljuk a jelenetet
            }
            yield return null;
        }
    }
    IEnumerator FillProgressBar(int targetPercentage)
    {
        float duration = 0f; // 2 másodperc alatt töltsön be
        float elapsed = 0f;

        float targetValue = Mathf.Clamp01(targetPercentage / 100f); // Átalakítás 0-1 közötti értékre

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(progressBar.value, targetValue, elapsed / duration); // Progress fokozatos feltöltése
            if (progressBar != null)
            {
                progressBar.value = progress; // Frissítjük a progress bárt
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = targetValue; // Biztosítjuk, hogy pontosan a célértéken álljon meg
        }
    }
    IEnumerator LoadData()
    {
        Main.AdvancedItemHandler.AdvancedItemDatas.AdvancedItemHanderDataLoad();

        Task<bool> loginTask = UIFunctions.LogIn();

        // Várjuk, amíg a login befejezõdik
        while (!loginTask.IsCompleted)
        {
            yield return null; // Egy frame-et várunk
        }

        yield return null;
    }
}
