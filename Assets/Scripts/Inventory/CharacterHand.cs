using UnityEngine;
using ItemHandler;
using UI;
using Items;
using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;

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
                        ShootPressed = Input.GetMouseButton(0),
                        UnloadPressed = Input.GetKeyDown(KeyCode.U),
                        AimPressed = Input.GetMouseButton(1)
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
            yield return StartCoroutine(contontrol.Value.Control(input));
        }
        SetIndicators();
        LockDown = false;
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
                Debug.LogWarning(InGameUI.MagasineIndicator.GetComponent<TextMeshProUGUI>().text);
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
    private void test()
    {
        Debug.LogWarning($"TEST           {SelectedItem.ItemName}                 {SelectedItem.TryGetComponent<WeaponBody>(out var weapon)}  {weapon}");
    }
}
