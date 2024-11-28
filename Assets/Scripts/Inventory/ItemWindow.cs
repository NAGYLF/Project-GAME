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
    [HideInInspector] public GameObject itemObject;
    [HideInInspector] public GameObject parentObject;
    private Item item;
    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
    public void positioning()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // Beállítjuk a szülõt
        gameObject.transform.SetParent(parentObject.transform, false);

        // Egér pozíciójának lekérése
        Vector2 mousePosition = Input.mousePosition;

        // Lokális pozíció kiszámítása
        RectTransform parentRect = parentObject.GetComponent<RectTransform>();
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

        item = itemObject.GetComponent<ItemObject>().ActualData;

        ActionConstruction();
    }
    private void ActionConstruction()
    {
        if (item.Open)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Open);
            button.transform.SetParent(Content.transform);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Open";
        }
        if (item.Modification)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Modification);
            button.transform.SetParent(Content.transform);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Modification";
        }
        if (item.Unload)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Unload);
            button.transform.SetParent(Content.transform);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Unload";
        }
        if (item.Drop)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Drop);
            button.transform.SetParent(Content.transform);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Drop";
        }
        if (item.Remove)
        {
            GameObject button = CreatePrefab("GameElements/ItemWindowButton");
            button.GetComponent<Button>().onClick.AddListener(Remove);
            button.transform.SetParent(Content.transform);
            button.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Remove";
        }
    }
    private void Drop()
    {
        Destroy(gameObject);
    }
    private void Remove()
    {
        itemObject.GetComponent<ItemObject>().SelfDestruction();
        Destroy(gameObject);
    }
    private void Unload()
    {
        Destroy(gameObject);
    }
    private void Modification()
    {
        Destroy(gameObject);
    }
    private void Open()
    {
        Destroy(gameObject);
    }
}