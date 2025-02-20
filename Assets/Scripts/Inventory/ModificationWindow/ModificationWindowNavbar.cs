using Assets.Scripts.Inventory;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModificationWindowNavbar : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public GameObject Window;
    private Camera mainCamera;
    private Vector3 offset;
    void Start()
    {
        mainCamera = InGameUI.Camera.GetComponent<Camera>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.LogWarning("DragMouse");
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(Window.transform.position).z;
        // Frissítjük az objektum pozícióját úgy, hogy a megfogott pont (offset-tel együtt) kövessen
        Window.transform.position = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogWarning("OnMouseDown");
        InGameUI.PlayerInventory.GetComponent<WindowManager>().SetWindowToTheTop(Window);
        // Konvertáljuk az egér pozícióját world koordinátává.
        Vector3 mousePoint = Input.mousePosition;
        // Állítsuk be a megfelelõ z-t, ami a kamera és az objektum távolságát jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(Window.transform.position).z;
        // Számoljuk ki az offset-et
        offset = Window.transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
