using MainData;
using System.Collections;
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
        // Aktiv�ljuk a bet�lt� k�perny�t
        loadingScreen.SetActive(true);

        //// V�runk 1 m�sodpercet a kezd�s el�tt
        //yield return new WaitForSeconds(1f);

        // Progress bar anim�ci� (2 m�sodperc alatt 0-r�l 100%-ra)
        yield return StartCoroutine(FillProgressBar(50));
        yield return StartCoroutine(LoadData());
        yield return StartCoroutine(FillProgressBar(100));




        //// V�runk 1 m�sodpercet a bet�lt�s ut�n
        //yield return new WaitForSeconds(1f);


        // Aszinkron jelenet bet�lt�se
        AsyncOperation operation = SceneManager.LoadSceneAsync("Main Menu");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Ha a bet�lt�s k�sz, aktiv�ljuk a jelenetet
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Kikapcsoljuk a bet�lt� k�perny�t
        loadingScreen.SetActive(false);
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
        Main.AdvancedItemHandler.AdvancedItemHanderDataLoad();
        yield return null;
    }
}
