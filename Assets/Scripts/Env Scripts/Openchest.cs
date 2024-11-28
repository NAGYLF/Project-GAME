using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openchest : MonoBehaviour
{
    public GameObject StoragePanel;   // A láda storage UI-ja
    public Transform player;       // A játékos Transform referenciája
    public float openDistance = 2f; // Távolság, amin belül a láda megnyitható

    private bool isOpen = false;    // A láda nyitottsági állapota

    void Update()
    {
        // Ellenõrizzük a játékos távolságát
        float distance = Vector2.Distance(player.position, transform.position);

        // Ha közel van és megnyomja az F gombot, a láda nyílik vagy záródik
        if (distance <= openDistance && Input.GetKeyDown(KeyCode.F))
        {
            if (!isOpen)
            {
                OpenChest();
            }
            else
            {
                CloseChest();
            }
        }
    }

    void OpenChest()
    {
        isOpen = true;
        StoragePanel.SetActive(true); // Láda UI megjelenítése
        Time.timeScale = 0;        // A játék megállítása (opcionális)
    }

    void CloseChest()
    {
        isOpen = false;
        StoragePanel.SetActive(false); // Láda UI elrejtése
        Time.timeScale = 1;         // A játék újraindítása
    }
}
