using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainData;
using InventoryClass;
using ItemHandler;
public class DevConsol : MonoBehaviour
{
    public TextMeshPro text;
    
    public void Consol()
    {
        Debug.Log($"{text}");
        string[] Command = text.text.Split(' ');
        switch (Command[0])
        {
            case "add":
                switch (Command[1])
                {
                    case var _ when Command[1] == Main.name:
                        switch (Command[2])
                        {
                            case "Item":
                                Item item = new();
                                item.name = Command[3];
                                PlayerInventory.Playerinventory.InventoryAdd(item);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
}
