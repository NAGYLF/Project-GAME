using UnityEngine;
using ItemHandler;
using UI;
using Unity.VisualScripting;

public class CharacterHand : MonoBehaviour
{
    public Item SelectedItem;
    public InGameItemObject SelectedItemObject;

    private bool Flipped = false;
    // Update is called once per frame
    void Update()
    {
        RotateObject();
        FlipObject();
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
        float angle = transform.rotation.eulerAngles.z;

        if (angle > 90 && angle < 270)
        {
            if (Flipped)
            {
                Vector3 scale = transform.localScale;
                scale.y = -Mathf.Abs(scale.y);
                transform.localScale = scale;
                Flipped = false;
            }
        }
        else
        {
            if (!Flipped)
            {
                Vector3 scale = transform.localScale;
                scale.y = Mathf.Abs(scale.y);
                transform.localScale = scale;
                Flipped = true;
            }
        }
    }
    public void SetItem(Item item)
    {
        if (item != null)
        {
            item.PlayerHandRef = this;
            SelectedItem = item;
            if (SelectedItem.hotKeyRef != null)
            {
                SelectedItem.hotKeyRef.IsInPlayerHand = true;
            }
            SelectedItemObject.SetDataRoute(item);
            SelectedItemObject.Inicialisation();

            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
            //Debug.LogWarning($"{item.ItemName} setted to player hand");
        }
        else
        {
            SelectedItem = null;
        }
    }
    public void UnsetItem()
    {
        if (SelectedItem != null)
        {
            SelectedItem.PlayerHandRef = null;
            if (SelectedItem.hotKeyRef != null)
            {
                SelectedItem.hotKeyRef.IsInPlayerHand = false;
            }
            SelectedItem = null;
            SelectedItemObject.SetDataRoute(null);
            SelectedItemObject.Inicialisation();
        }
    }
}
