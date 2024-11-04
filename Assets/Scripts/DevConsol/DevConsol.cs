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
                                PlayerInventory.playerInventoryData.InventoryAdd(item);
                                break;
                            case "DevInventory":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items00 = new Item[]
                                {
                                    new Item("TestWeapon"),
                                    new Item("TestWeapon"),
                                    new Item("TestHandgun"),
                                    new Item("TestArmor"),
                                    new Item("TestBackpack"),
                                    new Item("TestVest"),
                                };
                                foreach (Item item_ in items00)
                                {
                                    Debug.Log(item_.ItemName);
                                    Debug.Log($"{PlayerInventory.playerInventoryData==null}");
                                    PlayerInventory.playerInventoryData.InventoryAdd(item_);
                                }
                                break;
                            case "3xTestWeapon+TestBackPack":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items01 = new Item[]
                                {
                                    new Item("TestBackpack"),
                                    new Item("TestWeapon"),
                                    new Item("TestWeapon"),
                                    new Item("TestWeapon"),
                                };
                                foreach (Item item_ in items01)
                                {
                                    Debug.Log(item_.ItemName);
                                    Debug.Log($"{PlayerInventory.playerInventoryData == null}");
                                    PlayerInventory.playerInventoryData.InventoryAdd(item_);
                                }
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
