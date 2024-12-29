using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class MiniMapIconRotate : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Eg�r poz�ci�j�nak lek�r�se vil�gkoordin�t�ban
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 2D-s j�t�k miatt z-t lenull�zzuk

        // Ir�nyvektor kisz�m�t�sa
        Vector3 direction = mousePosition - InGameUI.Player.transform.position;

        // Sz�g kisz�m�t�sa radi�nban, majd fokokk� alak�t�sa
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ikon forgat�sa
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
