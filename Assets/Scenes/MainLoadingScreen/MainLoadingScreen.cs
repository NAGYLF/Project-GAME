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

        // V�runk 1 m�sodpercet a kezd�s el�tt
        yield return new WaitForSeconds(1f);

        // Progress bar anim�ci� (2 m�sodperc alatt 0-r�l 100%-ra)
        yield return StartCoroutine(FillProgressBar());

        // V�runk 1 m�sodpercet a bet�lt�s ut�n
        yield return new WaitForSeconds(1f);

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

    IEnumerator FillProgressBar()
    {
        float duration = 2f; // 2 m�sodperc alatt t�lts�n be
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration); // Sz�m�tsd ki a progress �rt�ket 0 �s 1 k�z�tt
            if (progressBar != null)
            {
                progressBar.value = progress; // Friss�tsd a progress bart
            }
            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1f; // Biztos�tsd, hogy a progress bar 100%-on legyen
        }
    }
}
