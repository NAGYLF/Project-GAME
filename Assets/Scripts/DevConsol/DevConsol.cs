using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainData;
using PlayerInventoryVisualBuild;
using ItemHandler;
using PlayerInventoryClass;
public class DevConsol : MonoBehaviour
{
    public GameObject text;
    //add [playerName] Item [itemName]
    private void Start()
    {
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, gameObject.GetComponent<RectTransform>().localPosition.z);
    }
    public void Consol()
    {
        string[] Command = text.GetComponent<TMP_InputField>().text.Split(' ');
        switch (Command[0])
        {
            case "add":
                switch (Command[1])
                {
                    case var _ when Command[1] == Main.name:
                        switch (Command[2])
                        {
                            case "item":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Debug.Log($"{Command[3]}");
                                Item item = new Item(Command[3]);
                                PlayerInventory.InventoryAdd(item);
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
