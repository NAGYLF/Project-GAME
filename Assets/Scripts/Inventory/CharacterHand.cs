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
        // Az eg�r poz�ci�ja a k�perny� koordin�t�i
        Vector3 mousePosition = Input.mousePosition;

        // �talak�tjuk a k�perny� koordin�t�it vil�g koordin�t�kk�
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Sz�m�tsuk ki a k�l�nbs�get az objektum �s az eg�r poz�ci�ja k�z�tt
        Vector3 direction = mousePosition - transform.position;

        // Sz�m�tsuk ki az �j rot�ci�t
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Be�ll�tjuk az objektum rot�ci�j�t
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    private void FlipObject()
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Ellen�rizz�k az aktu�lis rot�ci�t
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 90 && angle < 270)
        {
            spriteRenderer.flipY = true; // Flippelj�k az Y tengely ment�n
            int sortingLayer = InGameUI.Player.GetComponent<SpriteRenderer>().sortingOrder;
            spriteRenderer.sortingOrder = --sortingLayer;
        }
        else
        {
            spriteRenderer.flipY = false; // Eredeti �llapot
            int sortingLayer = InGameUI.Player.GetComponent<SpriteRenderer>().sortingOrder;
            spriteRenderer.sortingOrder = ++sortingLayer;
        }
    }

}
