using UnityEngine;
using ItemHandler;
using UI;
using Items;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using System.Linq;

public class CharacterHand : MonoBehaviour
{
    public AdvancedItem SelectedItem;
    public InGameItemObject SelectedItemObject;

    private bool Flipped = false;
    private bool LockDown = false;
    void Update()
    {
        if (InGameUI.CharacterHandControl)
        {
            RotateObject();
            FlipObject();
            if (InGameUI.CharacterHandControl && SelectedItem != null)
            {
                bool inputDetected = Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.U) || Input.GetMouseButton(0) || Input.GetMouseButton(1);
                if (inputDetected && !LockDown)
                {
                    LockDown = true;
                    InputFrameData input = new InputFrameData
                    {
                        ReloadPressed = Input.GetKeyDown(KeyCode.R),
                        AutoShoot = Input.GetMouseButton(0),
                        UnloadPressed = Input.GetKeyDown(KeyCode.U),
                        AimPressed = Input.GetMouseButton(1),

                        SingleShoot = Input.GetMouseButtonDown(0),
                    };

                    StartCoroutine(RunItemControlsSequentially(input));
                }
            }
        }
    }
    IEnumerator RunItemControlsSequentially(InputFrameData input)
    {
        foreach (var contontrol in SelectedItem.Components)
        {
            Debug.Log(contontrol.Key.Name);
            if (contontrol.Value is IItemControl)
            {
                yield return StartCoroutine((contontrol.Value as IItemControl).Control(input));
            }
        }
        SetIndicators();
        LockDown = false;
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
    public void SetIndicators()
    {
        if (SelectedItem != null)
        {
            SelectedItem.Components.TryGetValue(typeof(Magasine), out var magasine);
            if (magasine != null)
            {
                InGameUI.MagasineIndicator.GetComponent<TextMeshProUGUI>().text = $"In Magasine: {(magasine as Magasine).ContainedAmmo.Count}";
            }
            else
            {
                InGameUI.MagasineIndicator.GetComponent<TextMeshProUGUI>().text = $"In Magasine: - ";
            }
            SelectedItem.Components.TryGetValue(typeof(WeaponBody), out var weaponBody);
            if (weaponBody != null)
            {
                InGameUI.ChamberIndiator.GetComponent<TextMeshProUGUI>().text = $"In Chamber: {(weaponBody as WeaponBody).Chamber.Count}";
            }
            else
            {
                InGameUI.ChamberIndiator.GetComponent<TextMeshProUGUI>().text = $"In Chamber: - ";
            }
        }
        else
        {
            InGameUI.MagasineIndicator.GetComponent<TextMeshProUGUI>().text = $"In Magasine: - ";
            InGameUI.ChamberIndiator.GetComponent<TextMeshProUGUI>().text = $"In Chamber: - ";
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

            foreach (var component in SelectedItem.Components)
            {
                if (component.Value is IItemControl)
                {
                    (component.Value as IItemControl).Inicialisation(item);
                }
            }

            //ez az oebcjtum flipje
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;

            SetIndicators();
        }
        else
        {
            SelectedItem = null;
            InGameUI.MagasineIndicator.GetComponent<TextMeshProUGUI>().text = $"In Magasine: - ";
            InGameUI.ChamberIndiator.GetComponent<TextMeshProUGUI>().text = $"In Chamber: - ";
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
