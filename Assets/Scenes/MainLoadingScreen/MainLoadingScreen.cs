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

        // Várunk 1 másodpercet a kezdés elõtt
        yield return new WaitForSeconds(1f);

        // Progress bar animáció (2 másodperc alatt 0-ról 100%-ra)
        yield return StartCoroutine(FillProgressBar());

        // Várunk 1 másodpercet a betöltés után
        yield return new WaitForSeconds(1f);

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

    IEnumerator FillProgressBar()
    {
        float duration = 2f; // 2 másodperc alatt töltsön be
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration); // Számítsd ki a progress értéket 0 és 1 között
            if (progressBar != null)
            {
                progressBar.value = progress; // Frissítsd a progress bart
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1f; // Biztosítsd, hogy a progress bar 100%-on legyen
        }
    }
}
