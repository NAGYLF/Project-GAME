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

    private Vector3 offset; // A k�l�nbs�g az objektum poz�ci�ja �s a kattint�s vil�gkoordin�t�ja k�z�tt
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
        // Konvert�ljuk az eg�r poz�ci�j�t world koordin�t�v�.
        Vector3 mousePoint = Input.mousePosition;
        // �ll�tsuk be a megfelel� z-t, ami a kamera �s az objektum t�vols�g�t jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Sz�moljuk ki az offset-et
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogWarning("DragMouse");
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Friss�tj�k az objektum poz�ci�j�t �gy, hogy a megfogott pont (offset-tel egy�tt) k�vessen
        transform.position = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
    }
}
