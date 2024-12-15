using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MainData;
using ItemHandler;
using PlayerInventoryClass;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System;
public class DevConsol : MonoBehaviour
{
    public GameObject text;
    [HideInInspector] private static List<ItemSlotData> equipments;
    [HideInInspector] public GameObject inventory;
    [HideInInspector] public GameObject Player;
    //add [playerName] item [itemName] [Count]
    private void Start()
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.sizeDelta = new Vector2(Main.DefaultWidth,Main.DefaultHeight);
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
                                Item item = new Item(Command[3]);
                                if (Command.Length==5)
                                {
                                    item.Quantity = int.Parse(Command[4]);
                                }
                                inventory.GetComponent<PlayerInventory>().InventoryAdd(item);
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
                                    new Item("TestBoots"),
                                    new Item("TestFingers"),
                                    new Item("TestHeadset"),
                                    new Item("TestHelmet"),
                                    new Item("TestMask"),
                                    new Item("TestMelee"),
                                    new Item("TestPant"),
                                    new Item("TestSkin"),
                                };
                                foreach (Item item_ in items00)
                                {
                                    //Debug.Log(item_.ItemName);
                                    //Debug.Log($"{PlayerInventory.playerInventoryData==null}");
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
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
                                    Debug.Log($"{inventory.GetComponent<PlayerInventory>() == null}");
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "DevInventory_2xHandgun":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items02 = new Item[]
                                           {
                                    new Item("TestWeapon"),
                                    new Item("TestWeapon"),
                                    new Item("TestHandgun"),
                                    new Item("TestArmor"),
                                    new Item("TestBackpack"),
                                    new Item("TestVest"),
                                    new Item("TestBoots"),
                                    new Item("TestFingers"),
                                    new Item("TestHeadset"),
                                    new Item("TestHelmet"),
                                    new Item("TestMask"),
                                    new Item("TestMelee"),
                                    new Item("TestPant"),
                                    new Item("TestSkin"),
                                    new Item("TestHandgun"),
                                    new Item("TestHandgun"),
                                           };
                                foreach (Item item_ in items02)
                                {
                                    Debug.Log(item_.ItemName);
                                    Debug.Log($"{inventory.GetComponent<PlayerInventory>() == null}");
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "_7.62x39FMJx600":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items03 = new Item[]
                                           {
                                    new Item("7.62x39FMJ",600),
                                           };
                                foreach (Item item_ in items03)
                                {
                                    Debug.Log(item_.ItemName);
                                    Debug.Log($"{inventory.GetComponent<PlayerInventory>() == null}");
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
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
            case "remove":
                switch (Command[1])
                {
                    case var _ when Command[1] == Main.name:
                        switch (Command[2])
                        {
                            case "item":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item item = new Item(Command[3]);
                                if (Command.Length > 4)
                                {
                                    item.Quantity = int.Parse(Command[4]);
                                }
                                inventory.GetComponent<PlayerInventory>().InventoryRemove(item);
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "Save":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                equipments = inventory.GetComponent<PlayerInventory>().equipments;
                break;
            case "Clear":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                inventory.GetComponent<PlayerInventory>().equipments = new List<ItemSlotData>();
                break;
            case "Load":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                inventory.GetComponent<PlayerInventory>().equipments = equipments;
                break;
            case var _ when Command[0] == Main.name:
                switch (Command[1])
                {
                    case "Healt":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value = float.Parse(Command[2]);
                        if (value > 0)
                        {
                            Player.GetComponent<Player>().HealtUp(value);
                        }
                        else if (value < 0)
                        {
                            Player.GetComponent<Player>().HealtDown(Math.Abs(value));
                        }
                        break;
                    case "Stamina":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value1 = float.Parse(Command[2]);
                        if (value1 > 0)
                        {
                            Player.GetComponent<Player>().StaminaUp(value1);
                        }
                        else if (value1 < 0)
                        {
                            Player.GetComponent<Player>().StaminaDown(Math.Abs(value1));
                        }
                        break;
                    case "Hunger":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value2 = float.Parse(Command[2]);
                        if (value2 > 0)
                        {
                            Player.GetComponent<Player>().HungerUp(value2);
                        }
                        else if (value2 < 0)
                        {
                            Player.GetComponent<Player>().HungerDown(Math.Abs(value2));
                        }
                        break;
                    case "Thirst":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value3 = float.Parse(Command[2]);
                        if (value3 > 0)
                        {
                            Player.GetComponent<Player>().ThirstUp(value3);
                        }
                        else if (value3 < 0)
                        {
                            Player.GetComponent<Player>().ThirstDown(Math.Abs(value3));
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        text.GetComponent<TMP_InputField>().text = "";
    }
}
