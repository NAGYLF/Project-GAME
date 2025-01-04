using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UI;

[RequireComponent(typeof(LineRenderer))]
public class HealtUILineRenderel : MonoBehaviour
{
    [Header("Settings")]
    public List<GameObject> points = new List<GameObject>();
    private const float lineWidth = 0.5f;

    private LineRenderer lineRenderer;

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true; // World Space használata

        StartCoroutine(UpdateLineWhileMoving());
    }

    IEnumerator UpdateLineWhileMoving()
    {
        while (InGameUI.Player.GetComponent<Rigidbody2D>().velocity != Vector2.zero)
        {
            yield return null;
        }
        UpdateLine();
    }

    void UpdateLine()
    {
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
