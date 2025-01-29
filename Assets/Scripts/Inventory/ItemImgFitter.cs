using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemImgFitter : MonoBehaviour
{
    // A 'fitter' a gyermekelemek kont�nere, amelyet el�sz�r m�retre szabunk,
    // majd a sz�l�h�z sk�l�zunk.
    public RectTransform fitter;

    public void Fitting()
    {
        // 1) Sz�l� RectTransform (ugyanazon a GameObjecten vagyunk), aminek a m�ret�hez igazodunk
        RectTransform parentRect = GetComponent<RectTransform>();

        // 2) A 'fitter' alaphelyzetbe hoz�sa
        fitter.localScale = Vector3.one;          // ne legyen nagy�tva/kicsiny�tve
        fitter.anchorMin = new Vector2(0.5f, 0.5f);
        fitter.anchorMax = new Vector2(0.5f, 0.5f);
        fitter.pivot = new Vector2(0.5f, 0.5f); // forg�spont a k�z�p
        fitter.anchoredPosition = Vector2.zero;           // helyezze k�z�pre
        fitter.sizeDelta = Vector2.zero;           // induljunk null�r�l

        // 3) A gyerekek �ltal lefedett ter�let kisz�m�t�sa lok�lis t�rben
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        for (int i = 0; i < fitter.childCount; i++)
        {
            RectTransform childRect = fitter.GetChild(i).GetComponent<RectTransform>();
            if (childRect == null || !childRect.gameObject.activeInHierarchy)
                continue;

            // A gyerek 4 sark�t lek�rdezz�k a *gyerek saj�t* lok�lis t�rben.
            Vector3[] corners = new Vector3[4];
            childRect.GetLocalCorners(corners);

            // A felt�telez�s szerint NINCS extra lok�lis forgat�s vagy sk�l�z�s a gyereken,
            // �gy egyszer�en a gyerek anchoredPosition-j�t hozz�adjuk,
            // hogy a 'fitter' lok�lis ter�be helyezz�k ezeket a sarkokat.
            Vector2 childPos = childRect.anchoredPosition;

            for (int c = 0; c < 4; c++)
            {
                Vector3 cornerInFitterSpace = corners[c] + (Vector3)childPos;

                if (cornerInFitterSpace.x < minX) minX = cornerInFitterSpace.x;
                if (cornerInFitterSpace.y < minY) minY = cornerInFitterSpace.y;
                if (cornerInFitterSpace.x > maxX) maxX = cornerInFitterSpace.x;
                if (cornerInFitterSpace.y > maxY) maxY = cornerInFitterSpace.y;
            }
            childRect.localScale = Vector3.one;
            childRect.localRotation = Quaternion.identity;
        }
        // Ha nincs �rv�nyes bounding box (nincsenek akt�v gyermekek), kil�p�nk
        if (minX > maxX || minY > maxY)
        {
            Debug.LogWarning("Nincs akt�v gyerek, vagy �rv�nytelen bounding box.");
            return;
        }

        // 4) A gyerekek �ltal lefedett teljes m�ret
        float contentWidth = maxX - minX;
        float contentHeight = maxY - minY;

        // -----------------------------
        // L�P�S 1: A FITTER M�RETEZ�SE
        // -----------------------------
        // Itt a 'fitter' val�di sz�less�g�t/magass�g�t h�zzuk pont akkor�ra,
        // amekkor�t a gyerekek t�nylegesen ig�nyelnek.
        fitter.sizeDelta = new Vector2(contentWidth, contentHeight);

        // --------------------------
        // L�P�S 2: FITTER SK�L�Z�SA
        // --------------------------
        // A sz�l� m�ret�hez igaz�tjuk a 'fitter' m�retet (ar�nytart�an),
        // hogy biztosan belef�rjen a parentRectbe.
        Vector2 parentSize = parentRect.rect.size;

        float scaleX = parentSize.x / contentWidth;
        float scaleY = parentSize.y / contentHeight;
        float finalScale = Mathf.Min(scaleX, scaleY); // ar�nytart� illeszt�s

        fitter.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
