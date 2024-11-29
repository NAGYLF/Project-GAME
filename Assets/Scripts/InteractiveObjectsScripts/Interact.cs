using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject player; // A Player GameObject, amit az Inspectorban kell be�ll�tani
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
            // A Player script el�r�se
            Player playerScript = player.GetComponent<Player>();
            if (playerScript != null)
            {
                // Ellen�rizd, hogy van-e m�r InGameUI p�ld�ny a jelenetben
                GameObject inGameUIInstance = GameObject.Find("InGameUI");

                // Ha megtal�ltuk az InGameUI-t
                if (inGameUIInstance != null)
                {
                    // Az InGameUI script el�r�se a prefab GameObjectb�l
                    inGameUIScript = inGameUIInstance.GetComponent<InGameUI>();

                    // Ha van script, el�rhetj�k az IntecativeObjects list�t
                    if (inGameUIScript != null)
                    {
                        // Az IntecativeObjects lista el�r�se
                        var interactiveObjects = inGameUIScript.IntecativeObjects;
                        Debug.Log($"Interakt�v objektumok sz�ma: {interactiveObjects.Count}");

                        // Tov�bbi m�veletek itt, ha szeretn�d manipul�lni a list�t
                    }
                    else
                    {
                        Debug.LogWarning("Az InGameUI prefab nem tartalmaz InGameUI scriptet!");
                    }
                }
                else
                {
                    Debug.LogWarning("Nem tal�ltuk az InGameUI GameObjectet a jelenetben!");
                }
            }
            else
            {
                Debug.LogWarning("A Player GameObject nem tartalmaz Player scriptet!");
            }
        }
        else
        {
            Debug.LogWarning("A Player GameObject nincs be�ll�tva!");
        }
    }
}

