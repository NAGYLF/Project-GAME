using UnityEngine;
using ItemHandler;
using UI;

public class CharacterHand : MonoBehaviour
{
    [HideInInspector] public Item SelectedItem;
    // Update is called once per frame
    void Update()
    {
        RotateObject();
        FlipObject();
        HandHUDRefresh();
    }
    private void HandHUDRefresh()
    {
        //a slected item adatait jeleniti meg
    }
    private void RotateObject()
    {
        // Az egér pozíciója a képernyõ koordinátái
        Vector3 mousePosition = Input.mousePosition;

        // Átalakítjuk a képernyõ koordinátáit világ koordinátákká
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Számítsuk ki a különbséget az objektum és az egér pozíciója között
        Vector3 direction = mousePosition - transform.position;

        // Számítsuk ki az új rotációt
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Beállítjuk az objektum rotációját
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    private void FlipObject()
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Ellenõrizzük az aktuális rotációt
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 90 && angle < 270)
        {
            spriteRenderer.flipY = true; // Flippeljük az Y tengely mentén
            int sortingLayer = InGameUI.Player.GetComponent<SpriteRenderer>().sortingOrder;
            spriteRenderer.sortingOrder = --sortingLayer;
        }
        else
        {
            spriteRenderer.flipY = false; // Eredeti állapot
            int sortingLayer = InGameUI.Player.GetComponent<SpriteRenderer>().sortingOrder;
            spriteRenderer.sortingOrder = ++sortingLayer;
        }
    }

}
