using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadebehind : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // A fa Sprite Renderer komponense
    public Transform player;              // A j�t�kos Transform referenci�ja
    public float transparentAlpha = 0.5f; // �tl�tsz�s�gi szint (0 = teljesen �tl�tsz�, 1 = teljesen l�that�)
    public float proximityDistance = 2f;  // T�vols�g, amelyen bel�l a fa �ttetsz�v� v�lik
    private Color originalColor;          // Az eredeti sz�n, hogy vissza tudjuk �ll�tani


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Mentj�k az eredeti sz�nt
    }

    void Update()
    {
        // Kisz�m�tjuk a t�vols�got a j�t�kos �s a fa k�z�tt
        float distance = Vector2.Distance(player.position, transform.position);

        // Csak akkor v�gezz�k el a sz�nez�st, ha a j�t�kos a k�zelben van
        if (distance <= proximityDistance)
        {
            // Ellen�rizz�k, hogy a j�t�kos a fa m�g�tt van-e
            if (player.position.y > transform.position.y)
            {
                spriteRenderer.sortingOrder = 11;
                // A j�t�kos m�g�tt van, tegy�k �tl�tsz�v� a f�t
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, transparentAlpha);
            }
            else
            {
                // A j�t�kos el�tt van, �ll�tsuk vissza az eredeti sz�nt
                spriteRenderer.color = originalColor;
            }
        }
        else
        {
            // Ha a j�t�kos nincs a k�zelben, �ll�tsuk vissza az eredeti sz�nt
            spriteRenderer.color = originalColor;
            spriteRenderer.sortingOrder = 0;
        }
    }
}
