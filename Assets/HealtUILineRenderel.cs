using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class HealtUILineRenderel : MonoBehaviour
{
    [Header("Settings")]
    public List<GameObject> points = new List<GameObject>(); // A GameObject-ek list�ja
    private const float lineWidth = 0.5f; // A vonal sz�less�ge

    private LineRenderer lineRenderer;

    void Start()
    {
        // Line Renderer inicializ�l�sa
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true; // World Space haszn�lata

        UpdateLine();
    }

    void UpdateLine()
    {
        if (points.Count < 2)
        {
            // Ha nincs el�g pont, ne rajzoljon vonalat
            lineRenderer.positionCount = 0;
            return;
        }

        // Be�ll�tja a Line Renderer poz�ci�it a GameObject-ek alapj�n
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] != null)
            {
                lineRenderer.SetPosition(i, points[i].transform.position);
            }
        }
    }
}