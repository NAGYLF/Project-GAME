using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EGKAnimation : MonoBehaviour
{
    public float amplitude = 2.0f; // Hullám magassága
    public float frequency = 1.0f; // Hullám frekvenciája
    public int pointCount = 100; // Pontok száma a hullámban
    public float animationSpeed = 1.0f; // Animáció sebessége

    private LineRenderer lineRenderer;
    private float timeOffset = 0.0f;
    private float waveDuration = 1.0f; // Egy hullám idõtartama

    void Start()
    {
        // LineRenderer komponens lekérése
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCount; // Állítsd be a pontok számát
        lineRenderer.loop = false; // Ne zárja össze a vonalat
        lineRenderer.useWorldSpace = true; // Világkoordináták használata
    }

    void Update()
    {
        timeOffset += Time.deltaTime * animationSpeed; // Az idõeltolás animálása
        if (timeOffset >= waveDuration)
        {
            timeOffset = 0; // Az animáció újraindítása
        }
        DrawWave();
    }

    void DrawWave()
    {
        // Az objektum szélessége és magassága
        RectTransform rectTransform = GetComponent<RectTransform>();
        float objectWidth = rectTransform.rect.width; // Az objektum szélessége
        float objectHeight = rectTransform.rect.height; // Az objektum magassága

        float adjustedAmplitude = objectHeight / 2; // Az amplitúdót az objektum magasságához igazítjuk

        Vector3 startPosition = transform.position - new Vector3(objectWidth / 2, 0, 0);

        // A pontok elhelyezése
        for (int i = 0; i < pointCount; i++)
        {
            // X koordináta (arányos eloszlás az objektum szélességén belül)
            float x = (i / (float)(pointCount - 1)) * objectWidth;

            // Y koordináta (EKG jelalak az objektumhoz képest)
            float normalizedX = (i / (float)(pointCount - 1)) + timeOffset; // Normál X (0 és 1 között), animációval eltolva
            float y = timeOffset < waveDuration ? EKGSignal(normalizedX * frequency) * adjustedAmplitude : 0; // Csak egy hullám látható

            // Világkoordináták kiszámítása
            Vector3 wavePosition = startPosition + new Vector3(x, y, 0);

            // Beállítás a LineRenderer számára
            lineRenderer.SetPosition(i, wavePosition);
        }
    }

    float EKGSignal(float t)
    {
        // Egy hullám generálása pontosan a kép alapján
        t = t % 1.0f; // Normálizálás 0 és 1 közé

        if (t < 0.05f) // Alapvonal (bal oldal)
            return 0;
        else if (t < 0.1f) // Éles emelkedés (Q)
            return Mathf.Lerp(0, -amplitude * 0.5f, (t - 0.05f) / 0.05f);
        else if (t < 0.2f) // Nagy csúcs (R)
            return Mathf.Lerp(-amplitude * 0.5f, amplitude, (t - 0.1f) / 0.1f);
        else if (t < 0.25f) // Éles süllyedés (S)
            return Mathf.Lerp(amplitude, -amplitude * 0.5f, (t - 0.2f) / 0.05f);
        else if (t < 0.3f) // Stabilizáció
            return Mathf.Lerp(-amplitude * 0.5f, 0, (t - 0.25f) / 0.05f);
        else if (t < 0.4f) // Enyhe emelkedés (T hullám)
            return Mathf.Lerp(0, amplitude * 0.3f, (t - 0.3f) / 0.1f);
        else if (t < 0.5f) // T hullám vissza
            return Mathf.Lerp(amplitude * 0.3f, 0, (t - 0.4f) / 0.1f);
        else // Alapvonal (jobb oldal)
            return 0;
    }
}
