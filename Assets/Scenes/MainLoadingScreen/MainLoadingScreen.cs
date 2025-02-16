using MainData;
using System.Collections;
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
        // Aktiváljuk a betöltõ képernyõt
        loadingScreen.SetActive(true);

        //// Várunk 1 másodpercet a kezdés elõtt
        //yield return new WaitForSeconds(1f);

        // Progress bar animáció (2 másodperc alatt 0-ról 100%-ra)
        yield return StartCoroutine(FillProgressBar(50));
        yield return StartCoroutine(LoadData());
        yield return StartCoroutine(FillProgressBar(100));




        //// Várunk 1 másodpercet a betöltés után
        //yield return new WaitForSeconds(1f);


        // Aszinkron jelenet betöltése
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Menu");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Ha a betöltés kész, aktiváljuk a jelenetet
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Kikapcsoljuk a betöltõ képernyõt
        loadingScreen.SetActive(false);
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
        Main.AdvancedItemHandler.AdvancedItemHanderDataLoad();
        yield return null;
    }
}
