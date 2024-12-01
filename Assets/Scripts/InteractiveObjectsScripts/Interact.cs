using PlayerInventoryClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UI;

public class Interact : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject InGameUI;
    public bool Opened = false;
    public string Title;//inpectorban kell megadni !!!
    public string Description;//inpectorban kell megadni !!!
    public string ActionMode;//inpectorban kell megadni !!!

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            collider.gameObject.GetComponent<Player>().InGameUI.GetComponent<InGameUI>().IntecativeObjectSelectorBox.GetComponent<InteractiveObjectSelector>().AddInteractObject(gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            collider.gameObject.GetComponent<Player>().InGameUI.GetComponent<InGameUI>().IntecativeObjectSelectorBox.GetComponent<InteractiveObjectSelector>().RemoveInteractObject(gameObject);
            if (!UI.InGameUI.PlayerInventory.Status)
            {
                UI.InGameUI.PlayerInventory.Action();
            }
        }
    }
}