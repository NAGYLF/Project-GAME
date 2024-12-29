using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EGKAnimation : MonoBehaviour
{
    public float amplitude = 2.0f; // Hull�m magass�ga
    public float frequency = 1.0f; // Hull�m frekvenci�ja
    public int pointCount = 100; // Pontok sz�ma a hull�mban
    public float animationSpeed = 1.0f; // Anim�ci� sebess�ge

    private LineRenderer lineRenderer;
    private float timeOffset = 0.0f;
    private float waveDuration = 1.0f; // Egy hull�m id�tartama

    void Start()
    {
        // LineRenderer komponens lek�r�se
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCount; // �ll�tsd be a pontok sz�m�t
        lineRenderer.loop = false; // Ne z�rja �ssze a vonalat
        lineRenderer.useWorldSpace = true; // Vil�gkoordin�t�k haszn�lata
    }

    void Update()
    {
        timeOffset += Time.deltaTime * animationSpeed; // Az id�eltol�s anim�l�sa
        if (timeOffset >= waveDuration)
        {
            timeOffset = 0; // Az anim�ci� �jraind�t�sa
        }
        DrawWave();
    }

    void DrawWave()
    {
        // Az objektum sz�less�ge �s magass�ga
        RectTransform rectTransform = GetComponent<RectTransform>();
        float objectWidth = rectTransform.rect.width; // Az objektum sz�less�ge
        float objectHeight = rectTransform.rect.height; // Az objektum magass�ga

        float adjustedAmplitude = objectHeight / 2; // Az amplit�d�t az objektum magass�g�hoz igaz�tjuk

        Vector3 startPosition = transform.position - new Vector3(objectWidth / 2, 0, 0);

        // A pontok elhelyez�se
        for (int i = 0; i < pointCount; i++)
        {
            // X koordin�ta (ar�nyos eloszl�s az objektum sz�less�g�n bel�l)
            float x = (i / (float)(pointCount - 1)) * objectWidth;

            // Y koordin�ta (EKG jelalak az objektumhoz k�pest)
            float normalizedX = (i / (float)(pointCount - 1)) + timeOffset; // Norm�l X (0 �s 1 k�z�tt), anim�ci�val eltolva
            float y = timeOffset < waveDuration ? EKGSignal(normalizedX * frequency) * adjustedAmplitude : 0; // Csak egy hull�m l�that�

            // Vil�gkoordin�t�k kisz�m�t�sa
            Vector3 wavePosition = startPosition + new Vector3(x, y, 0);

            // Be�ll�t�s a LineRenderer sz�m�ra
            lineRenderer.SetPosition(i, wavePosition);
        }
    }

    float EKGSignal(float t)
    {
        // Egy hull�m gener�l�sa pontosan a k�p alapj�n
        t = t % 1.0f; // Norm�liz�l�s 0 �s 1 k�z�

        if (t < 0.05f) // Alapvonal (bal oldal)
            return 0;
        else if (t < 0.1f) // �les emelked�s (Q)
            return Mathf.Lerp(0, -amplitude * 0.5f, (t - 0.05f) / 0.05f);
        else if (t < 0.2f) // Nagy cs�cs (R)
            return Mathf.Lerp(-amplitude * 0.5f, amplitude, (t - 0.1f) / 0.1f);
        else if (t < 0.25f) // �les s�llyed�s (S)
            return Mathf.Lerp(amplitude, -amplitude * 0.5f, (t - 0.2f) / 0.05f);
        else if (t < 0.3f) // Stabiliz�ci�
            return Mathf.Lerp(-amplitude * 0.5f, 0, (t - 0.25f) / 0.05f);
        else if (t < 0.4f) // Enyhe emelked�s (T hull�m)
            return Mathf.Lerp(0, amplitude * 0.3f, (t - 0.3f) / 0.1f);
        else if (t < 0.5f) // T hull�m vissza
            return Mathf.Lerp(amplitude * 0.3f, 0, (t - 0.4f) / 0.1f);
        else // Alapvonal (jobb oldal)
            return 0;
    }
}
