using TMPro;
using UnityEngine;
using MainData;
using ItemHandler;
using PlayerInventoryClass;
using System;
using System.Linq;
using UI;
using System.Collections.Generic;
using static PlayerInventoryClass.PlayerInventory;
public class DevConsol : MonoBehaviour
{
    public GameObject text;
    [HideInInspector] private static LevelManager Save; 
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
                    case var _ when Command[1] == Main.playerData.Name:
                        switch (Command[2])
                        {
                            case "item":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem item = new AdvancedItem(Command[3]);
                                if (Command.Length>=5)
                                {
                                    item.Quantity = int.Parse(Command[4]);
                                }
                                inventory.GetComponent<PlayerInventory>().InventoryAdd(item);
                                break;
                            case "DevInventory":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem[] items00 = new AdvancedItem[]
                                {
                                    new AdvancedItem("AK103"),
                                    new AdvancedItem("AK103"),
                                    new AdvancedItem("Glock_17_9x19_pistol_PS9"),
                                    new AdvancedItem("_6B43_6A_Zabralo_Sh_body_armor"),
                                    new AdvancedItem("TestBackpack"),
                                    new AdvancedItem("TestVest"),
                                    new AdvancedItem("TestBoots"),
                                    new AdvancedItem("TestFingers"),
                                    new AdvancedItem("GSSh_01_active_headset"),
                                    new AdvancedItem("Galvion_Caiman_Hybrid_helmet_Grey"),
                                    new AdvancedItem("Atomic_Defense_CQCM_ballistic_mask_Black"),
                                    new AdvancedItem("APOK_Tactical_Wasteland_Gladius"),
                                    new AdvancedItem("USEC_Base"),
                                    new AdvancedItem("USEC_Base_Upper"),
                                };
                                foreach (AdvancedItem item_ in items00)
                                {
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "DevInventory_FullGear":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem[] items02 = new AdvancedItem[]
                                           {
                                    new AdvancedItem("AK103"),
                                    new AdvancedItem("U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default"),
                                    new AdvancedItem("Glock_17_9x19_pistol_PS9"),
                                    new AdvancedItem("_6B43_6A_Zabralo_Sh_body_armor"),
                                    new AdvancedItem("TestBackpack"),
                                    new AdvancedItem("TestVest"),
                                    new AdvancedItem("TestBoots"),
                                    new AdvancedItem("TestFingers"),
                                    new AdvancedItem("GSSh_01_active_headset"),
                                    new AdvancedItem("Galvion_Caiman_Hybrid_helmet_Grey"),
                                    new AdvancedItem("Atomic_Defense_CQCM_ballistic_mask_Black"),
                                    new AdvancedItem("APOK_Tactical_Wasteland_Gladius"),
                                    new AdvancedItem("USEC_Base"),
                                    new AdvancedItem("USEC_Base_Upper"),
                                    new AdvancedItem("_7_62x39mm_T_45M1_gzh",180),
                                    new AdvancedItem("_5_56x45mm_M856",180),
                                    new AdvancedItem("_9x19mm_Green_Tracer",60),
                                           };
                                foreach (AdvancedItem item_ in items02)
                                {
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "DevInvenotry_AKS74UAdvnacedItem":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem[] items_AK74UTest = new AdvancedItem[]
                                {
                                    new AdvancedItem("_6B43_6A_Zabralo_Sh_body_armor"),
                                    new AdvancedItem("TestBackpack"),
                                    new AdvancedItem("TestVest"),
                                    new AdvancedItem("TestBoots"),
                                    new AdvancedItem("TestFingers"),
                                    new AdvancedItem("GSSh_01_active_headset"),
                                    new AdvancedItem("Galvion_Caiman_Hybrid_helmet_Grey"),
                                    new AdvancedItem("Atomic_Defense_CQCM_ballistic_mask_Black"),
                                    new AdvancedItem("APOK_Tactical_Wasteland_Gladius"),
                                    new AdvancedItem("USEC_Base"),
                                    new AdvancedItem("USEC_Base_Upper"),
                                    new AdvancedItem("_5_45x39mm_FMJ",60),
                                    new AdvancedItem("_5_45x39mm_FMJ",60),
                                    new AdvancedItem("_5_45x39mm_FMJ",60),
                                    new AdvancedItem("AKS-74U_Body"),
                                    new AdvancedItem("Glock_19X_9x19_pistol_body"),

                                };
                                AdvancedItem[] parts_Glock19X = new AdvancedItem[]
                                {
                                    new AdvancedItem("Glock_9x19_19-round_magasine_(Coyote)"),
                                    new AdvancedItem("Glock_19X_9x19_barrel"),
                                    new AdvancedItem("Glock_19X_pistol_slide"),
                                    new AdvancedItem("Glock_19X_front_sight"),
                                    new AdvancedItem("Glock_19X_rear_sight"),
                                    new AdvancedItem("Olight_Baldr_Pro_tactical_flashlight_with_laser_(tan)"),
                                };
                                AdvancedItem[] parts_AKS74U = new AdvancedItem[]
                                {
                                    new AdvancedItem("AKS-74U_Legal_Arsenal_Pilgrim_railed_dust_cover"),
                                    new AdvancedItem("AKS-74U_Zenit_B-11_handguard"),
                                    new AdvancedItem("AKS-74U_bakelite_pistol_grip"),
                                    new AdvancedItem("AK-74_5.45x39_6L20_30-round_magasine"),
                                    new AdvancedItem("KAC_vertical_foregrip"),
                                    new AdvancedItem("AK-105_5.45x39_muzzle_brake-compensator"),
                                    new AdvancedItem("Walther_MRS_reflex_sight"),
                                    new AdvancedItem("AKS-74U_Skeletonized_Stock"),
                                };
                                foreach (AdvancedItem item_ in parts_AKS74U)
                                {
                                    (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data = items_AK74UTest[items_AK74UTest.Length-2].PartPut_IsPossible(item_);
                                    items_AK74UTest[items_AK74UTest.Length - 2].PartPut(item_,Data.SCP,Data.ICP);
                                }
                                foreach (AdvancedItem item_ in parts_Glock19X)
                                {
                                    (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data = items_AK74UTest[items_AK74UTest.Length - 1].PartPut_IsPossible(item_);
                                    items_AK74UTest[items_AK74UTest.Length - 1].PartPut(item_, Data.SCP, Data.ICP);
                                }
                                foreach (AdvancedItem item_ in items_AK74UTest)
                                {
                                    inventory.GetComponent<PlayerInventory>().InventoryAdd(item_);
                                }
                                break;
                            case "_7.62x39FMJx600":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem[] items03 = new AdvancedItem[]
                                           {
                                    new AdvancedItem("7.62x39FMJ",600),
                                           };
                                foreach (AdvancedItem item_ in items03)
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
                    case var _ when Command[1] == Main.playerData.Name:
                        switch (Command[2])
                        {
                            case "item":
                                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                                AdvancedItem item = new AdvancedItem(Command[3]);
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
                InventorySystem.PlayerInventorySave(ref Save,ref inventory.GetComponent<PlayerInventory>().levelManager);
                break;
            case "Clear":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                InventorySystem.PlayerInventoryDefault(ref inventory.GetComponent<PlayerInventory>().levelManager);
                break;
            case "Load":
                Debug.Log($"{text.GetComponent<TMP_InputField>().text}");
                InventorySystem.PlayerInventoryLoad(ref Save, ref inventory.GetComponent<PlayerInventory>().levelManager);
                break;
            case var _ when Command[0] == Main.playerData.Name:
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
