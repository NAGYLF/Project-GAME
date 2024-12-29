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
        // Egér pozíciójának lekérése világkoordinátában
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 2D-s játék miatt z-t lenullázzuk

        // Irányvektor kiszámítása
        Vector3 direction = mousePosition - InGameUI.Player.transform.position;

        // Szög kiszámítása radiánban, majd fokokká alakítása
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ikon forgatása
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
