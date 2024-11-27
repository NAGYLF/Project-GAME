using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openchest : MonoBehaviour
{
    public GameObject StoragePanel;   // A l�da storage UI-ja
    public Transform player;       // A j�t�kos Transform referenci�ja
    public float openDistance = 2f; // T�vols�g, amin bel�l a l�da megnyithat�

    private bool isOpen = false;    // A l�da nyitotts�gi �llapota

    void Update()
    {
        // Ellen�rizz�k a j�t�kos t�vols�g�t
        float distance = Vector2.Distance(player.position, transform.position);

        // Ha k�zel van �s megnyomja az F gombot, a l�da ny�lik vagy z�r�dik
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
        StoragePanel.SetActive(true); // L�da UI megjelen�t�se
        Time.timeScale = 0;        // A j�t�k meg�ll�t�sa (opcion�lis)
    }

    void CloseChest()
    {
        isOpen = false;
        StoragePanel.SetActive(false); // L�da UI elrejt�se
        Time.timeScale = 1;         // A j�t�k �jraind�t�sa
    }
}
