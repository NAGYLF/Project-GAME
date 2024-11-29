using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fadebehind : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // A fa Sprite Renderer komponense
    public Transform player;              // A játékos Transform referenciája
    public float transparentAlpha = 0.5f; // Átlátszósági szint (0 = teljesen átlátszó, 1 = teljesen látható)
    public float proximityDistance = 2f;  // Távolság, amelyen belül a fa áttetszõvé válik
    private Color originalColor;          // Az eredeti szín, hogy vissza tudjuk állítani


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Mentjük az eredeti színt
    }

    void Update()
    {
        // Kiszámítjuk a távolságot a játékos és a fa között
        float distance = Vector2.Distance(player.position, transform.position);

        // Csak akkor végezzük el a színezést, ha a játékos a közelben van
        if (distance <= proximityDistance)
        {
            // Ellenõrizzük, hogy a játékos a fa mögött van-e
            if (player.position.y > transform.position.y)
            {
                spriteRenderer.sortingOrder = 11;
                // A játékos mögött van, tegyük átlátszóvá a fát
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, transparentAlpha);
            }
            else
            {
                // A játékos elõtt van, állítsuk vissza az eredeti színt
                spriteRenderer.color = originalColor;
            }
        }
        else
        {
            // Ha a játékos nincs a közelben, állítsuk vissza az eredeti színt
            spriteRenderer.color = originalColor;
            spriteRenderer.sortingOrder = 0;
        }
    }
}
