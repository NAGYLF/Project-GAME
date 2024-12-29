using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIconRotate : MonoBehaviour
{
    public Transform player; // A játékos transformja
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
        Vector3 direction = mousePosition - player.position;

        // Szög kiszámítása radiánban, majd fokokká alakítása
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ikon forgatása
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
