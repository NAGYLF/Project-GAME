using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainData;
using InventoryClass;
using ItemHandler;
public class DevConsol : MonoBehaviour
{
    public GameObject text;

    private void Start()
    {
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, gameObject.GetComponent<RectTransform>().localPosition.z);
    }
    public void Consol()
    {
        Debug.Log($"{text}");
        string[] Command = text.GetComponent<TMP_InputField>().text.Split(' ');
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
