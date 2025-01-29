using TMPro;
using UnityEngine;
using MainData;
using ItemHandler;
using PlayerInventoryClass;
using System;
using System.Linq;
using UI;
public class DevConsol : MonoBehaviour
{
    public GameObject text;
    [HideInInspector] private static Item RootData; 
    public GameObject inventory;
    //add [playerName] item [itemName] [Count]
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
                                if (Command.Length>=5)
                                {
                                    item.Quantity = int.Parse(Command[4]);
                                }
                                inventory.GetComponent<PlayerInventory>().InventoryAdd(item);
                                break;
                            case "DevInventory":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items00 = new Item[]
                                {
                                    new Item("AK103"),
                                    new Item("AK103"),
                                    new Item("Glock_17_9x19_pistol_PS9"),
                                    new Item("_6B43_6A_Zabralo_Sh_body_armor"),
                                    new Item("TestBackpack"),
                                    new Item("TestVest"),
                                    new Item("TestBoots"),
                                    new Item("TestFingers"),
                                    new Item("GSSh_01_active_headset"),
                                    new Item("Galvion_Caiman_Hybrid_helmet_Grey"),
                                    new Item("Atomic_Defense_CQCM_ballistic_mask_Black"),
                                    new Item("APOK_Tactical_Wasteland_Gladius"),
                                    new Item("USEC_Base"),
                                    new Item("USEC_Base_Upper"),
                                };
                                foreach (Item item_ in items00)
                                {
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "DevInventory_FullGear":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                Item[] items02 = new Item[]
                                           {
                                    new Item("AK103"),
                                    new Item("U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default"),
                                    new Item("Glock_17_9x19_pistol_PS9"),
                                    new Item("_6B43_6A_Zabralo_Sh_body_armor"),
                                    new Item("TestBackpack"),
                                    new Item("TestVest"),
                                    new Item("TestBoots"),
                                    new Item("TestFingers"),
                                    new Item("GSSh_01_active_headset"),
                                    new Item("Galvion_Caiman_Hybrid_helmet_Grey"),
                                    new Item("Atomic_Defense_CQCM_ballistic_mask_Black"),
                                    new Item("APOK_Tactical_Wasteland_Gladius"),
                                    new Item("USEC_Base"),
                                    new Item("USEC_Base_Upper"),
                                    new Item("_7_62x39mm_T_45M1_gzh",180),
                                    new Item("_5_56x45mm_M856",180),
                                    new Item("_9x19mm_Green_Tracer",60),
                                           };
                                foreach (Item item_ in items02)
                                {
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
                RootData = inventory.GetComponent<PlayerInventory>().levelManager.Items.FirstOrDefault(item=>item.Lvl==-1);
                break;
            case "Clear":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                /*
                Item None = new Item();
                None.Container = new Container(inventory.GetComponent<ContainerObject>().ActualData.Container.PrefabPath);
                inventory.GetComponent<ContainerObject>().ActualData = None;*/
                break;
            case "Load":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                inventory.GetComponent<PlayerInventory>().levelManager.Items.Add(RootData);
                inventory.GetComponent<ContainerObject>().ActualData = RootData;
                break;
            case var _ when Command[0] == Main.name:
                switch (Command[1])
                {
                    case "Healt":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value = float.Parse(Command[2]);
                        if (value > 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().HealtUp(value);
                        }
                        else if (value < 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().HealtDown(Math.Abs(value));
                        }
                        break;
                    case "Stamina":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value1 = float.Parse(Command[2]);
                        if (value1 > 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().StaminaUp(value1);
                        }
                        else if (value1 < 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().StaminaDown(Math.Abs(value1));
                        }
                        break;
                    case "Hunger":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value2 = float.Parse(Command[2]);
                        if (value2 > 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().HungerUp(value2);
                        }
                        else if (value2 < 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().HungerDown(Math.Abs(value2));
                        }
                        break;
                    case "Thirst":
                        Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                        float value3 = float.Parse(Command[2]);
                        if (value3 > 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().ThirstUp(value3);
                        }
                        else if (value3 < 0)
                        {
                            InGameUI.InGameUI_.GetComponent<InGameUI>().ThirstDown(Math.Abs(value3));
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
