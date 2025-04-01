using UnityEngine;
using ItemHandler;
using UI;
using Items;
using System.Collections.Generic;
using System;
using System.Collections;

public class CharacterHand : MonoBehaviour
{
    public class InputFrameData
    {
        public bool ReloadPressed;
        public bool ShootPressed;
        public bool UnloadPressed;
        public bool AimPressed;
    }

    public AdvancedItem SelectedItem;
    public InGameItemObject SelectedItemObject;

    private bool Flipped = false;
    void Update()
    {
        RotateObject();
        FlipObject();
        if (InGameUI.CharacterHandControl && SelectedItem != null)
        {
            InputFrameData input = new InputFrameData
            {
                ReloadPressed = Input.GetKeyDown(KeyCode.R),
                ShootPressed = Input.GetMouseButton(0),
                UnloadPressed = Input.GetKeyDown(KeyCode.U),
                AimPressed = Input.GetMouseButton(1)
            };

            StartCoroutine(RunItemControlsSequentially(input));
        }
    }
    IEnumerator RunItemControlsSequentially(InputFrameData input)
    {
        foreach (var contontrol in SelectedItem.Components)
        {
            Debug.Log(contontrol.Key.Name);
            yield return StartCoroutine(contontrol.Value.Control(input));
        }
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
                InGameUI.Player.GetComponent<SpriteRenderer>().flipX = true;
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
                InGameUI.Player.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }
    public void SetItem(AdvancedItem item)
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

            foreach (Part part in SelectedItem.Parts)
            {
                if (part.item_s_Part.Component != null)
                {
                    part.item_s_Part.Component.Inicialisation(item);
                }
            }
            test();

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
    private void test()
    {
        Debug.LogWarning($"TEST           {SelectedItem.ItemName}                 {SelectedItem.TryGetComponent<WeaponBody>(out var weapon)}  {weapon}");
    }
}
