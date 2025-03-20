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
    public GameObject loadingScreen; // A loading k�perny� GameObject
    public Slider progressBar; // A folyamat jelz�s�re szolg�l� cs�szka

    void Start()
    {
        StartCoroutine(LoadDataAndScene());
    }

    IEnumerator LoadDataAndScene()
    {
        loadingScreen.SetActive(true);


        // Progress bar anim�ci� (2 m�sodperc alatt 0-r�l 100%-ra)
        yield return StartCoroutine(FillProgressBar(50));
        yield return StartCoroutine(LoadData());
        yield return StartCoroutine(FillProgressBar(100));


        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Menu");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f); // Sk�l�z�s 0-100%-ig
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // Most aktiv�ljuk a jelenetet
            }
            yield return null;
        }
    }
    IEnumerator FillProgressBar(int targetPercentage)
    {
        float duration = 0f; // 2 m�sodperc alatt t�lts�n be
        float elapsed = 0f;

        float targetValue = Mathf.Clamp01(targetPercentage / 100f); // �talak�t�s 0-1 k�z�tti �rt�kre

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Lerp(progressBar.value, targetValue, elapsed / duration); // Progress fokozatos felt�lt�se
            if (progressBar != null)
            {
                progressBar.value = progress; // Friss�tj�k a progress b�rt
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = targetValue; // Biztos�tjuk, hogy pontosan a c�l�rt�ken �lljon meg
        }
    }
    IEnumerator LoadData()
    {
        Main.AdvancedItemHandler.AdvancedItemDatas.AdvancedItemHanderDataLoad();

        Task<bool> loginTask = UIFunctions.LogIn();

        // V�rjuk, am�g a login befejez�dik
        while (!loginTask.IsCompleted)
        {
            yield return null; // Egy frame-et v�runk
        }

        yield return null;
    }
}
