using Assets.Scripts.Inventory;
using ItemHandler;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModificationWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Item item;
    public GameObject PartsPanel;
    public GameObject ItemPanel;

    private Vector3 offset; // A különbség az objektum pozíciója és a kattintás világkoordinátája között
    private Camera mainCamera;

    void Start()
    {
        mainCamera = InGameUI.Camera.GetComponent<Camera>();
    }
    public void Openwindow(Item item)
    {
        this.item = item;
    }
    public void CloseWindow()
    {
        InGameUI.PlayerInventory.GetComponent<WindowManager>().DestroyWindow(gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogWarning("OnMouseDown");
        InGameUI.PlayerInventory.GetComponent<WindowManager>().SetWindowToTheTop(gameObject);
        // Konvertáljuk az egér pozícióját world koordinátává.
        Vector3 mousePoint = Input.mousePosition;
        // Állítsuk be a megfelelõ z-t, ami a kamera és az objektum távolságát jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Számoljuk ki az offset-et
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogWarning("DragMouse");
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Frissítjük az objektum pozícióját úgy, hogy a megfogott pont (offset-tel együtt) kövessen
        transform.position = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
    }
}
