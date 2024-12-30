using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class HealtUILineRenderel : MonoBehaviour
{
    [Header("Settings")]
    public List<GameObject> points = new List<GameObject>(); // A GameObject-ek listája
    private const float lineWidth = 0.5f; // A vonal szélessége

    private LineRenderer lineRenderer;

    void Start()
    {
        // Line Renderer inicializálása
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true; // World Space használata

        UpdateLine();
    }

    void UpdateLine()
    {
        if (points.Count < 2)
        {
            // Ha nincs elég pont, ne rajzoljon vonalat
            lineRenderer.positionCount = 0;
            return;
        }

        // Beállítja a Line Renderer pozícióit a GameObject-ek alapján
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