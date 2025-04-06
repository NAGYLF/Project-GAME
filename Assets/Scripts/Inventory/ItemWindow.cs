using ItemHandler;
using MainData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static MainData.SupportScripts;

public class ItemWindow : MonoBehaviour,IPointerExitHandler
{
    [SerializeField] public GameObject Content;

    private AdvancedItem item;
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
    public void positioning(AdvancedItem item)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // Beállítjuk a szülõt

        // Egér pozíciójának lekérése
        Vector2 mousePosition = Input.mousePosition;

        // Lokális pozíció kiszámítása
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        Vector2 tempLocalPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            mousePosition,
            Camera.main,
            out tempLocalPosition
        );

        // Lokális pozíció beállítása
        Vector3 localPosition = new Vector3(tempLocalPosition.x, tempLocalPosition.y, 0);
        gameObject.GetComponent<RectTransform>().localPosition = localPosition;

        this.item = item;

        ActionConstruction();
    }
    private void ActionConstruction()
    {
        //if (item.IsUsable)
        //{
        //    GameObject button = CreatePrefab("GameElements/ItemWindowButton");
        //    button.GetComponent<Button>().onClick.AddListener(Use);
        //    button.transform.SetParent(Content.transform,false);
        //    button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Use";
        //}
        if (item.IsOpenAble)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Open);
            button.transform.SetParent(Content.transform, false);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Open";
        }
        if (item.IsModificationAble)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Modification);
            button.transform.SetParent(Content.transform, false);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Modification";
        }
        if (item.IsUnloadAble)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Unload);
            button.transform.SetParent(Content.transform, false);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Unload";
        }
        if (item.IsDropAble)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Drop);
            button.transform.SetParent(Content.transform, false);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Drop";
        }
        if (item.IsRemoveAble)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Remove);
            button.transform.SetParent(Content.transform, false);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Remove";
        }
    }
    private void Drop()
    {
        item.Drop();
        Destroy(gameObject);
    }
    private void Remove()
    {
        item.Remove();
        Destroy(gameObject);
    }
    private void Unload()
    {
        item.Unload();
        Destroy(gameObject);
    }
    private void Modification()
    {
        item.Modification();
        Destroy(gameObject);
    }
    private void Open()
    {
        item.Open();
        Destroy(gameObject);
    }
    //private void Use()
    //{
    //    item.Use();
    //    Destroy(gameObject);
    //}
}