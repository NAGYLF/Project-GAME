using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemImgFitter : MonoBehaviour
{
    // A 'fitter' a gyermekelemek kont�nere, amelyet el�sz�r m�retre szabunk,
    // majd a sz�l�h�z sk�l�zunk.
    public RectTransform fitter;

    public void ResetFitter()
    {
        fitter.localScale = Vector3.one;
        fitter.anchorMin = new Vector2(0.5f, 0.5f);
        fitter.anchorMax = new Vector2(0.5f, 0.5f);
        fitter.pivot = new Vector2(0.5f, 0.5f);
        fitter.anchoredPosition = Vector2.zero;
        fitter.sizeDelta = new Vector2(100, 100); // Ideiglenes alapm�ret
    }

    public void Fitting()
    {
        RectTransform parentRect = GetComponent<RectTransform>();

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        for (int i = 0; i < fitter.childCount; i++)
        {
            RectTransform childRect = fitter.GetChild(i).GetComponent<RectTransform>();
            if (childRect == null || !childRect.gameObject.activeInHierarchy)
                continue;

            Vector3[] corners = new Vector3[4];
            childRect.GetLocalCorners(corners);

            Vector2 childPos = childRect.anchoredPosition;
            Vector3 childScale = childRect.localScale; // Figyelembe vessz�k a gyermek saj�t scale-j�t

            for (int c = 0; c < 4; c++)
            {
                // Sk�l�zott sarkok sz�m�t�sa
                Vector3 scaledCorner = new Vector3(
                    corners[c].x * childScale.x,
                    corners[c].y * childScale.y,
                    corners[c].z * childScale.z
                );

                Vector3 cornerInFitterSpace = scaledCorner + (Vector3)childPos;
                if (cornerInFitterSpace.x < minX) minX = cornerInFitterSpace.x;
                if (cornerInFitterSpace.y < minY) minY = cornerInFitterSpace.y;
                if (cornerInFitterSpace.x > maxX) maxX = cornerInFitterSpace.x;
                if (cornerInFitterSpace.y > maxY) maxY = cornerInFitterSpace.y;
            }
        }

        if (minX > maxX || minY > maxY)
        {
            Debug.LogWarning("Nincs akt�v gyerek, vagy �rv�nytelen bounding box.");
            return;
        }

        float contentWidth = maxX - minX;
        float contentHeight = maxY - minY;

        Vector2 contentCenter = new Vector2(minX + contentWidth * 0.5f, minY + contentHeight * 0.5f);

        for (int i = 0; i < fitter.childCount; i++)
        {
            RectTransform childRect = fitter.GetChild(i).GetComponent<RectTransform>();
            if (childRect == null || !childRect.gameObject.activeInHierarchy)
                continue;

            childRect.anchoredPosition -= contentCenter;
        }

        fitter.sizeDelta = new Vector2(contentWidth, contentHeight);

        Vector2 parentSize = parentRect.rect.size;
        float scaleX = parentSize.x / contentWidth;
        float scaleY = parentSize.y / contentHeight;
        float finalScale = Mathf.Min(scaleX, scaleY);

        fitter.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
