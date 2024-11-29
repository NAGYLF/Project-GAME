using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject player; // A Player GameObject, amit az Inspectorban kell beállítani
    public bool Opened = false;
    public string Title;
    public string Description;
    public string ActionMode;
    public Action Action;

    private InGameUI inGameUIScript; // Az InGameUI script referencia

    void Start()
    {
        if (player != null)
        {
            // A Player script elérése
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                // Ellenõrizd, hogy van-e már InGameUI példány a jelenetben
                GameObject inGameUIInstance = GameObject.Find("InGameUI");

                // Ha megtaláltuk az InGameUI-t
                if (inGameUIInstance != null)
                {
                    // Az InGameUI script elérése a prefab GameObjectbõl
                    inGameUIScript = inGameUIInstance.GetComponent<InGameUI>();

                    // Ha van script, elérhetjük az IntecativeObjects listát
                    if (inGameUIScript != null)
                    {
                        // Az IntecativeObjects lista elérése
                        var interactiveObjects = inGameUIScript.IntecativeObjects;
                        Debug.Log($"Interaktív objektumok száma: {interactiveObjects.Count}");

                        // További mûveletek itt, ha szeretnéd manipulálni a listát
                    }
                    else
                    {
                        Debug.LogWarning("Az InGameUI prefab nem tartalmaz InGameUI scriptet!");
                    }
                }
                else
                {
                    Debug.LogWarning("Nem találtuk az InGameUI GameObjectet a jelenetben!");
                }
            }
            else
            {
                Debug.LogWarning("A Player GameObject nem tartalmaz Player scriptet!");
            }
        }
        else
        {
            Debug.LogWarning("A Player GameObject nincs beállítva!");
        }
    }
}

