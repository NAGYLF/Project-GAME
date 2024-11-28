using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layersort : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Transform player;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Sorting Order számítás az Y-pozíció alapján
        spriteRenderer.sortingOrder = (int)(-transform.position.y * 100);
    }
}
