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
        // Friss�tj�k az objektum poz�ci�j�t �gy, hogy a megfogott pont (offset-tel egy�tt) k�vessen
        Window.transform.position = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogWarning("OnMouseDown");
        InGameUI.PlayerInventory.GetComponent<WindowManager>().SetWindowToTheTop(Window);
        // Konvert�ljuk az eg�r poz�ci�j�t world koordin�t�v�.
        Vector3 mousePoint = Input.mousePosition;
        // �ll�tsuk be a megfelel� z-t, ami a kamera �s az objektum t�vols�g�t jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(Window.transform.position).z;
        // Sz�moljuk ki az offset-et
        offset = Window.transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
