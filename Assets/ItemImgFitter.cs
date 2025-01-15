using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemImgFitter : MonoBehaviour
{
    // A 'fitter' a gyermekelemek konténere, amelyet elõször méretre szabunk,
    // majd a szülõhöz skálázunk.
    public RectTransform fitter;

    public void Fitting()
    {
        // 1) Szülõ RectTransform (ugyanazon a GameObjecten vagyunk), aminek a méretéhez igazodunk
        RectTransform parentRect = GetComponent<RectTransform>();

        // 2) A 'fitter' alaphelyzetbe hozása
        fitter.localScale = Vector3.one;          // ne legyen nagyítva/kicsinyítve
        fitter.anchorMin = new Vector2(0.5f, 0.5f);
        fitter.anchorMax = new Vector2(0.5f, 0.5f);
        fitter.pivot = new Vector2(0.5f, 0.5f); // forgáspont a közép
        fitter.anchoredPosition = Vector2.zero;           // helyezze középre
        fitter.sizeDelta = Vector2.zero;           // induljunk nulláról

        // 3) A gyerekek által lefedett terület kiszámítása lokális térben
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        for (int i = 0; i < fitter.childCount; i++)
        {
            RectTransform childRect = fitter.GetChild(i).GetComponent<RectTransform>();
            if (childRect == null || !childRect.gameObject.activeInHierarchy)
                continue;

            // A gyerek 4 sarkát lekérdezzük a *gyerek saját* lokális térben.
            Vector3[] corners = new Vector3[4];
            childRect.GetLocalCorners(corners);

            // A feltételezés szerint NINCS extra lokális forgatás vagy skálázás a gyereken,
            // így egyszerûen a gyerek anchoredPosition-jét hozzáadjuk,
            // hogy a 'fitter' lokális terébe helyezzük ezeket a sarkokat.
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
        // Ha nincs érvényes bounding box (nincsenek aktív gyermekek), kilépünk
        if (minX > maxX || minY > maxY)
        {
            Debug.LogWarning("Nincs aktív gyerek, vagy érvénytelen bounding box.");
            return;
        }

        // 4) A gyerekek által lefedett teljes méret
        float contentWidth = maxX - minX;
        float contentHeight = maxY - minY;

        // -----------------------------
        // LÉPÉS 1: A FITTER MÉRETEZÉSE
        // -----------------------------
        // Itt a 'fitter' valódi szélességét/magasságát húzzuk pont akkorára,
        // amekkorát a gyerekek ténylegesen igényelnek.
        fitter.sizeDelta = new Vector2(contentWidth, contentHeight);

        // --------------------------
        // LÉPÉS 2: FITTER SKÁLÁZÁSA
        // --------------------------
        // A szülõ méretéhez igazítjuk a 'fitter' méretet (aránytartóan),
        // hogy biztosan beleférjen a parentRectbe.
        Vector2 parentSize = parentRect.rect.size;

        float scaleX = parentSize.x / contentWidth;
        float scaleY = parentSize.y / contentHeight;
        float finalScale = Mathf.Min(scaleX, scaleY); // aránytartó illesztés

        fitter.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
