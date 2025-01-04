using Ammunition;
using Armors;
using Assets.Scripts;
using Backpacks;
using Boots;
using Fingers;
using Headsets;
using Helmets;
using Masks;
using Melees;
using Pants;
using Skins;
using Vests;
using Weapons;
using System;
using Cash;
using Meds;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using NaturalInventorys;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;
using UI;
using Unity.Mathematics;
using UnityEngine.WSA;


namespace ItemHandler
{
    public struct PlacerStruct
    {
        public List<GameObject> ActiveItemSlots { get; set; }
        public Item NewParentData { get; set; }
    }
    [Serializable]
    public class DataGrid
    {
        public int rowNumber;
        public int columnNumber;
        public List<RowData> col;
    }
    [Serializable]
    public class RowData
    {
        public List<GameObject> row;
    }
    public class ItemSlotData
    {
        public string SlotName;
        public string SlotType;
        public Item PartOfItemData;
        public ItemSlotData(string SlotName = "", string SlotType = "", Item PartOfItemData = null)
        {
            this.SlotName = SlotName;
            this.SlotType = SlotType;
            this.PartOfItemData = PartOfItemData;
        }
    }
    public class Item : NonGeneralItemProperties
    {
        //system variables
        public Item ParentItem;
        public GameObject SelfGameobject { get; set; }// a parent objectum
        public GameObject ContainerObject { get; set; }//conainer objectum
        public List<DataGrid> SectorDataGrid { get; set; }//ezek referanca pontokat atralamaznak amelyeken kersztul a tenyleges gameobjectumokat manipulalhatjuk
        public int Lvl { get; set; }
        public string HotKey { get; set; } = "";

        //general variables
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; } = "...";
        public int MaxStackSize { get; set; } = 1;
        public int Quantity { get; set; }
        public int Value { get; set; } = 1;
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        public float RotateDegree { get; set; } = 0f;
        public bool IsInPlayerInventory { get; set; } = false;// a player inventory tagja az item
        public bool IsEquipment { set; get; } = false;// az item egy equipment
        public bool IsLoot { set; get; } = false;// az item a loot conténerekben van
        public bool IsRoot { set; get; } = false;// az item egy root data
        public bool IsEquipmentRoot { set; get; } = false;// az item a player equipmentjeinek rootja ebbol csak egy lehet

        //SlotUse
        public List<string> SlotUse = new();
        public void SetSlotUse()
        {
            SlotUse.OrderBy(slotname => int.Parse(Regex.Match(slotname, @"\((\d+)\)").Groups[1].Value));
            LowestSlotUseNumber = Regex.Match(SlotUse.FirstOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.FirstOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
            HighestSlotUseNumber = Regex.Match(SlotUse.LastOrDefault() ?? string.Empty, @"\((\d+)\)").Success ? int.Parse(Regex.Match(SlotUse.LastOrDefault(), @"\((\d+)\)").Groups[1].Value) : 0;
        }
        public int LowestSlotUseNumber { get; private set; }
        public int HighestSlotUseNumber { get; private set; }

        //action (Műveletek)
        public bool IsDropAble { get; set; } = false;
        public bool IsRemoveAble { get; set; } = true;
        public bool IsUnloadAble { get; set; } = false;
        public bool IsModificationAble { get; set; } = false;
        public bool IsOpenAble { get; set; } = false;
        public bool IsUsable { get; set; } = false;

        //Ez egy Totális Törlés ami azt jelenti, hogy mindenhonnan törli. Ez nem jo akkor ha valahonnan torolni akarjuk de mashol meg hozzadni
        public void Remove()
        {
            if (IsRemoveAble)
            {
                InventorySystem.DataRemove(ParentItem, this);
                if (SelfGameobject)
                {
                    SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                    GameObject.Destroy(SelfGameobject);
                }
            }
        }
        public void Use()
        {
            if (IsUsable)
            {
                UseLeft--;
                if (UseLeft == 0)
                {
                    InventorySystem.DataRemove(ParentItem, this);
                    if (SelfGameobject)
                    {
                        SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                        GameObject.Destroy(SelfGameobject);
                    }
                }
            }
        }
        public void Open()
        {

        }
        public void Modification()
        {

        }
        public void Unload()
        {

        }
        public void Drop()
        {

        }
        public Item()//ha contume itememt akarunk letrehozni mint pl: egy Root item
        {

        }
        public Item(string name, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            Item completedItem = name switch
            {
                "TestBackpack" => new TestBackpack().Set(),
                "TestVest" => new TestVest().Set(),
                "TestFingers" => new TestFingers().Set(),
                "TestBoots" => new TestBoots().Set(),

                //Backpacks
                "Camelback_Tri_Zip_assault_backpack" => new Camelback_Tri_Zip_assault_backpack().Set(),
                "Flyye_MBSS_backpack" => new Flyye_MBSS_backpack().Set(),
                "Gruppa_99_T30_backpack" => new Gruppa_99_T30_backpack().Set(),
                "Sanitars_bag" => new Sanitars_bag().Set(),
                "Scav_backpack" => new Scav_backpack().Set(),
                "Tactical_sling_bag" => new Tactical_sling_bag().Set(),
                "Transformer_Bag" => new Transformer_Bag().Set(),
                "Vertx_Ready_Pack_Backpack" => new Vertx_Ready_Pack_Backpack().Set(),
                "VKBO_army_bag" => new VKBO_army_bag().Set(),
                "WARTECH_Berkut_Backpack" => new WARTECH_Berkut_Backpack().Set(),

                //Melees
                "APOK_Tactical_Wasteland_Gladius" => new APOK_Tactical_Wasteland_Gladius().Set(),
                "Camper_axe" => new Camper_axe().Set(),
                "Cultist_knife" => new Cultist_knife().Set(),
                "ER_FULCRUM_BAYONET" => new ER_FULCRUM_BAYONET().Set(),
                "Miller_Bros_Blades_M_2_Tactical_Sword" => new Miller_Bros_Blades_M_2_Tactical_Sword().Set(),
                "PR_Taran_police_baton" => new PR_Taran_police_baton().Set(),
                "SOG_Voodoo_Hawk_tactical_tomahawk" => new SOG_Voodoo_Hawk_tactical_tomahawk().Set(),
                "SP_8_Survival_Machete" => new SP_8_Survival_Machete().Set(),
                "Unitted_Cutlery_M48_Tactical_Kukri" => new Unitted_Cutlery_M48_Tactical_Kukri().Set(),
                "UVSR_Taiga_1_survival_machete" => new UVSR_Taiga_1_survival_machete().Set(),

                //WeaponsMain
                "AK103" => new AK103().Set(),
                "Colt_M4A1_5_56x45_assault_rifle_KAC_RIS" => new Colt_M4A1_5_56x45_assault_rifle_KAC_RIS().Set(),
                "Desert_Tech_MDR_5_56x45_assault_rifle_HHS_1_Tan" => new Desert_Tech_MDR_5_56x45_assault_rifle_HHS_1_Tan().Set(),
                "DS_Arms_SA_58_7_62x51_assault_rifle_SPR" => new DS_Arms_SA_58_7_62x51_assault_rifle_SPR().Set(),
                "DS_Arms_SA_58_7_62x51_assault_rifle_X_FAL" => new DS_Arms_SA_58_7_62x51_assault_rifle_X_FAL().Set(),
                "Mosin_7_62x54R_bolt_action_rifle_Sniper_ATACR_7_35x56" => new Mosin_7_62x54R_bolt_action_rifle_Sniper_ATACR_7_35x56().Set(),
                "FN_SCAR_H_7_62x51_assault_rifle_BOSS_Xe" => new FN_SCAR_H_7_62x51_assault_rifle_BOSS_Xe().Set(),
                "SIG_MCX_SPEAR_6_8x51_assault_rifle_EXPS3" => new SIG_MCX_SPEAR_6_8x51_assault_rifle_EXPS3().Set(),
                "SIG_MPX_9x19_submachine_gun_MRS" => new SIG_MPX_9x19_submachine_gun_MRS().Set(),
                "TOZ_Simonov_SKS_7_62x39_carbine_TAPCO_Intrafuse" => new TOZ_Simonov_SKS_7_62x39_carbine_TAPCO_Intrafuse().Set(),
                "U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default" => new U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default().Set(),

                //WeaponsSecondary
                "Glock_17_9x19_pistol_PS9" => new Glock_17_9x19_pistol_PS9().Set(),
                "Glock_17_9x19_pistol_Tac_2" => new Glock_17_9x19_pistol_Tac_2().Set(),
                "Magnum_Research_Desert_Eagle_L5_50_AE_pistol_Default" => new Magnum_Research_Desert_Eagle_L5_50_AE_pistol_Default().Set(),
                "Magnum_Research_Desert_Eagle_L6_50_AE_pistol_WTS_Default" => new Magnum_Research_Desert_Eagle_L6_50_AE_pistol_WTS_Default().Set(),
                "RSh_12_12_7x55_revolver_TAC30" => new RSh_12_12_7x55_revolver_TAC30().Set(),

                //Armors
                "_5_11_Tactical_TacTec_plate_carrier" => new _5_11_Tactical_TacTec_plate_carrier().Set(),
                "_6B5_16_Zh_86_Uley_armored_rig" => new _6B5_16_Zh_86_Uley_armored_rig().Set(),
                "_6B43_6A_Zabralo_Sh_body_armor" => new _6B43_6A_Zabralo_Sh_body_armor().Set(),
                "BNTI_Kirasa_N_bodyarmor" => new BNTI_Kirasa_N_bodyarmor().Set(),
                "BNTI_Module_3M_body_armor" => new BNTI_Module_3M_body_armor().Set(),
                "Crye_Precision_AVS_plate_carrier" => new Crye_Precision_AVS_plate_carrier().Set(),
                "IOTV_Gen4_body_armor_Assault_Kit" => new IOTV_Gen4_body_armor_Assault_Kit().Set(),
                "IOTV_Gen4_body_armor_Full_Protection_Kit" => new IOTV_Gen4_body_armor_Full_Protection_Kit().Set(),
                "IOTV_Gen4_body_armor_High_Mobility_Kit" => new IOTV_Gen4_body_armor_High_Mobility_Kit().Set(),
                "PACA_Soft_Armor" => new PACA_Soft_Armor().Set(),

                //Helmets
                "_6B47_Ratnik_BSh_helmet_Olive_Drab" => new _6B47_Ratnik_BSh_helmet_Olive_Drab().Set(),
                "Galvion_Caiman_Hybrid_helmet_Grey" => new Galvion_Caiman_Hybrid_helmet_Grey().Set(),
                "Kolpak_1S_riot_helmet" => new Kolpak_1S_riot_helmet().Set(),
                "LShZ_lightweight_helmet_Olive_Drab" => new LShZ_lightweight_helmet_Olive_Drab().Set(),
                "PSh_97_DJETA_riot_helmet" => new PSh_97_DJETA_riot_helmet().Set(),
                "ShPM_Firefighter_helmet" => new ShPM_Firefighter_helmet().Set(),
                "Tac_Kek_FAST_MT_helmet_Replica" => new Tac_Kek_FAST_MT_helmet_Replica().Set(),
                "SSh_68_steel_helmet_Olive_Drab" => new SSh_68_steel_helmet_Olive_Drab().Set(),
                "TSh_4M_L_soft_tank_crew_helmet" => new TSh_4M_L_soft_tank_crew_helmet().Set(),
                "UNTAR_helmet" => new UNTAR_helmet().Set(),

                //Masks
                "Atomic_Defense_CQCM_ballistic_mask_Black" => new Atomic_Defense_CQCM_ballistic_mask_Black().Set(),
                "Death_Knight_mask" => new Death_Knight_mask().Set(),
                "Death_Shadow_lightweight_armored_mask" => new Death_Shadow_lightweight_armored_mask().Set(),
                "Glorious_E_lightweight_armored_mask" => new Glorious_E_lightweight_armored_mask().Set(),
                "Shattered_lightweight_armored_mask" => new Shattered_lightweight_armored_mask().Set(),
                "Tagillas_welding_mask_Gorilla" => new Tagillas_welding_mask_Gorilla().Set(),
                "Tagillas_welding_mask_UBEY" => new Tagillas_welding_mask_UBEY().Set(),

                //Headsets
                "GSSh_01_active_headset" => new GSSh_01_active_headset().Set(),
                "MSA_Sordin_Supreme_headset" => new MSA_Sordin_Supreme_headset().Set(),
                "Ops_Core_FAST_RAC_Headset" => new Ops_Core_FAST_RAC_Headset().Set(),
                "OPSMEN_Earmor_M32_headset" => new OPSMEN_Earmor_M32_headset().Set(),
                "Peltor_ComTac_IV_Hybrid_headset" => new Peltor_ComTac_IV_Hybrid_headset().Set(),
                "Peltor_ComTac_V_headset" => new Peltor_ComTac_V_headset().Set(),
                "Peltor_ComTac_VI_headset" => new Peltor_ComTac_VI_headset().Set(),
                "Safariland_Liberator_HP_2_0_Headset" => new Safariland_Liberator_HP_2_0_Headset().Set(),
                "Walkers_Razor_Digital_headset" => new Walkers_Razor_Digital_headset().Set(),
                "Walkers_XCEL_500BT_Digital_headset" => new Walkers_XCEL_500BT_Digital_headset().Set(),

                //Vests
                "_6B5_15_Zh_86_Uley_armored_rig" => new _6B5_15_Zh_86_Uley_armored_rig().Set(),
                "ANA_Tactical_M1_plate_carrier" => new ANA_Tactical_M1_plate_carrier().Set(),
                "BlackRock_chest_rig" => new BlackRock_chest_rig().Set(),
                "Scav_Vest" => new Scav_Vest().Set(),
                "Security_vest" => new Security_vest().Set(),
                "SOE_Micro_Rig" => new SOE_Micro_Rig().Set(),
                "Stich_Profi_Plate_Carrier_V2" => new Stich_Profi_Plate_Carrier_V2().Set(),
                "Tasmanian_Tiger_Plate_Carrier_MKIII" => new Tasmanian_Tiger_Plate_Carrier_MKIII().Set(),
                "Umka_M33_SET1_hunter_vest" => new Umka_M33_SET1_hunter_vest().Set(),
                "Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest" => new Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest().Set(),

                //Ammunitions
                "7.62x39FMJ" => new Ammunition762x39FMJ().Set(),
                "_12_70_8_5mm_Magnum_buckshot" => new Ammunition_12_70_8_5mm_Magnum_buckshot().Set(),
                "_12_70_Grizzly_40_slug" => new Ammunition_12_70_Grizzly_40_slug().Set(),
                "_20_70_7_5mm_buckshot" => new Ammunition_20_70_7_5mm_buckshot().Set(),
                "_23_75mm_Shrapnel_10_buckshot" => new Ammunition_23_75mm_Shrapnel_10_buckshot().Set(),
                "_9x18mm_PM_PSO_gzh" => new Ammunition_9x18mm_PM_PSO_gzh().Set(),
                "_7_62x25mm_TT_AKBS" => new Ammunition_7_62x25mm_TT_AKBS().Set(),
                "_9x19mm_Green_Tracer" => new Ammunition_9x19mm_Green_Tracer().Set(),
                "_45_ACP_Lasermatch_FMJ" => new Ammunition_45_ACP_Lasermatch_FMJ().Set(),
                "_50_AE_Hawk_JSP" => new Ammunition_50_AE_Hawk_JSP().Set(),
                "_9x21mm_7U4" => new Ammunition_9x21mm_7U4().Set(),
                "_357_Magnum_JHP" => new Ammunition_357_Magnum_JHP().Set(),
                "_5_7x28mm_SS197SR" => new Ammunition_5_7x28mm_SS197SR().Set(),
                "_4_6x30mm_JSP_SX" => new Ammunition_4_6x30mm_JSP_SX().Set(),
                "_9x39mm_PAB_9_gs" => new Ammunition_9x39mm_PAB_9_gs().Set(),
                "_366_TKM_EKO" => new Ammunition_366_TKM_EKO().Set(),
                "_5_45x39mm_FMJ" => new Ammunition_5_45x39mm_FMJ().Set(),
                "_5_56x45mm_M856" => new Ammunition_5_56x45mm_M856().Set(),
                "_7_62x39mm_T_45M1_gzh" => new Ammunition_7_62x39mm_T_45M1_gzh().Set(),
                "_300_Blackout_V_Max" => new Ammunition_300_Blackout_V_Max().Set(),
                "_6_8x51mm_SIG_Hybrid" => new Ammunition_6_8x51mm_SIG_Hybrid().Set(),
                "_7_62x51mm_M80" => new Ammunition_7_62x51mm_M80().Set(),
                "_7_62x54mm_R_SNB_gzh" => new Ammunition_7_62x54mm_R_SNB_gzh().Set(),
                "_12_7x55mm_PS12" => new Ammunition_12_7x55mm_PS12().Set(),

                //Pants
                "USEC_Base" => new USEC_Base().Set(),
                "USEC_Defender" => new USEC_Defender().Set(),
                "USEC_Legionnaire" => new USEC_Legionnaire().Set(),
                "USEC_Outdoor_Tactical" => new USEC_Outdoor_Tactical().Set(),
                "USEC_Rangemaster" => new USEC_Rangemaster().Set(),
                "USEC_Ranger_Jeans" => new USEC_Ranger_Jeans().Set(),
                "USEC_Sage_Warrior" => new USEC_Sage_Warrior().Set(),
                "USEC_Taclife_Terrain" => new USEC_Taclife_Terrain().Set(),
                "USEC_TIER3" => new USEC_TIER3().Set(),

                //Skins
                "Adik_Tracksuit" => new Adik_Tracksuit().Set(),
                "USEC_Adaptive_Combat" => new USEC_Adaptive_Combat().Set(),
                "USEC_Aggressor_TAC" => new USEC_Aggressor_TAC().Set(),
                "USEC_Base_Upper" => new USEC_Base_Upper().Set(),
                "USEC_BOSS_Delta" => new USEC_BOSS_Delta().Set(),
                "USEC_Mission" => new USEC_Mission().Set(),
                "USEC_PCU_Ironsight" => new USEC_PCU_Ironsight().Set(),
                "USEC_Sandstone" => new USEC_Sandstone().Set(),
                "USEC_Troubleshooter" => new USEC_Troubleshooter().Set(),

                //money
                "Dollar_1" => new Dollar_1().Set(),

                //Useables
                "AI_2" => new AI_2().Set(),

                _ => throw new ArgumentException($"Invalid type {name}")
            };
            completedItem.Quantity = count;

            //altalanos adatok
            ItemType = completedItem.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            ItemName = completedItem.ItemName;//ez alapján hozza létre egy item saját magát
            Description = completedItem.Description;
            Quantity = completedItem.Quantity;
            Value = completedItem.Value;
            SizeX = completedItem.SizeX;
            SizeY = completedItem.SizeY;
            ImgPath = completedItem.ImgPath;
            MaxStackSize = completedItem.MaxStackSize;
            //Action
            IsDropAble = completedItem.IsDropAble;
            IsRemoveAble = completedItem.IsRemoveAble;
            IsUnloadAble = completedItem.IsUnloadAble;
            IsModificationAble = completedItem.IsModificationAble;
            IsOpenAble = completedItem.IsOpenAble;
            IsUsable = completedItem.IsUsable;
            //tartalom
            Container = completedItem.Container;//tartalom
            //fegyver adatok
            DefaultMagasineSize = completedItem.DefaultMagasineSize;
            Spread = completedItem.Spread;
            Rpm = completedItem.Rpm;
            Recoil = completedItem.Recoil;
            Accturacy = completedItem.Accturacy;
            Range = completedItem.Range;
            Ergonomy = completedItem.Ergonomy;
            BulletType = completedItem.BulletType;
            Accessors = completedItem.Accessors;
            AmmoType = completedItem.AmmoType;
            //hasznalhato e?
            UseLeft = completedItem.UseLeft;
            Debug.Log($"Item created {completedItem.ItemName}");
        }
    }
    public abstract class NonGeneralItemProperties// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha ő equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {
        //contain
        public Container Container { get; set; }
        //weapon
        public int? DefaultMagasineSize { get; set; }
        public double? Spread { get; set; }
        public int? Rpm { get; set; }
        public double? Recoil { get; set; }
        public double? Accturacy { get; set; }
        public double? Range { get; set; }
        public double? Ergonomy { get; set; }
        public string AmmoType { get; set; } = "";
        public BulletType BulletType { get; set; }
        public Accessors Accessors { get; set; }
        //usable
        public int UseLeft { get; set; } = 0;
        //ammo

        //med

        //armor
    }
    public class Container
    {
        //egy container az itemjéhez tartozik.
        //az item constructor selekciójánál itemet peldanyositunk: pl: TestWeapon
        //ebben az eddig null érékű container változó egy ures containerrre változik
        //az item pédányosításánál igy egy új példány készül a containerből is mely alapvetően tartalmazza a container PrefabPath-ét
        //a kostructora az igy megkapott prefabPath-ből lekerdezi a Sectorokat
        public List<Item> Items { get; set; }
        public string PrefabPath;
        public ItemSlotData[][,] Sectors { get; set; }
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            Items = new List<Item>();
            List<DataGrid> DataGrid = Resources.Load(prefabPath).GetComponent<ContainerObject>().Sectors;
            Sectors = new ItemSlotData[DataGrid.Count][,];
            for (int sector = 0; sector < Sectors.Length; sector++)
            {
                int index = 0;
                Sectors[sector] = new ItemSlotData[DataGrid[sector].columnNumber, DataGrid[sector].rowNumber];
                for (int col = 0; col < Sectors[sector].GetLength(0); col++)
                {
                    for (int row = 0; row < Sectors[sector].GetLength(1); row++)
                    {
                        Sectors[sector][col, row] = new ItemSlotData(DataGrid[sector].col[col].row[row].GetComponent<ItemSlot>().name, DataGrid[sector].col[col].row[row].GetComponent<ItemSlot>().SlotType);
                        index++;
                    }
                }
            }
        }
    }
    public class BulletType
    {

    }

    public class Accessors
    {

    }
    public static class LootRandomizer
    {
        private static readonly List<LootItem> weapons = new()
        {
            new LootItem("Glock_17_9x19_pistol_PS9",1f),
            new LootItem("AK103", 2f),
            new LootItem("APOK_Tactical_Wasteland_Gladius",2f),
            new LootItem("7.62x39FMJ",2f,0.1f,0.5f)//jelentese, hogy 10% és 50% staksize ban spawnolhat.
        };
        private static readonly List<LootItem> equipments = new()
        {
            new LootItem("USEC_Base",1f),
            new LootItem("USEC_Base_Upper", 1f),
            new LootItem("Atomic_Defense_CQCM_ballistic_mask_Black", 1f),
            new LootItem("GSSh_01_active_headset", 1f),
            new LootItem("Galvion_Caiman_Hybrid_helmet_Grey", 1f),
            new LootItem("_6B43_6A_Zabralo_Sh_body_armor", 1f),
            new LootItem("TestVest", 4f),
            new LootItem("TestBackpack",4f),
            new LootItem("TestBoots", 1f),
            new LootItem("TestFingers", 1f),
        };
        private struct LootItem
        {
            public string Name;
            public float SpawnRate;
            public float MinStack;
            public float MaxStack;

            public LootItem(string Name, float SpawnRate = 0,float MinStack = 1f,float MaxStack = 1f)
            {
                this.Name = Name;
                this.SpawnRate = SpawnRate;
                this.MinStack = MinStack;
                this.MaxStack = MaxStack;
            }
        }
        private static List<LootItem> GenerateLoot(string PaletteName)
        {
            List<LootItem> list = new();
            switch (PaletteName)
            {
                case "weapons":
                    foreach (LootItem item in weapons)
                    {
                        for (int i = 0; i < item.SpawnRate; i++)
                        {
                            list.Add(item);
                        }
                    }
                    return list;
                case "equipments":
                    foreach (LootItem item in equipments)
                    {
                        for (int i = 0; i < item.SpawnRate; i++)
                        {
                            list.Add(item);
                        }
                    }
                    return list;
                default:
                    return list;
            }
        }
        public static void FillSimpleInvenotry(SimpleInventory simpleInventory,string PaletteName,float Fullness)
        {
            float MaxSlotNumber = 0;
            foreach (ItemSlotData[,] row in simpleInventory.Root.Container.Sectors)
            {
                foreach (ItemSlotData slot in row)
                {
                    MaxSlotNumber++;
                }
            }
            float ActualSlotNumber = 0;
            Math.Round(MaxSlotNumber*=Fullness,0);
            List<LootItem> WeightedList = GenerateLoot(PaletteName);
            while (MaxSlotNumber > ActualSlotNumber)
            {
                LootItem LootItem = WeightedList[UnityEngine.Random.Range(0, WeightedList.Count)];
                Item item = new(LootItem.Name);
                if (item.MaxStackSize>1)
                {
                    item = new Item(LootItem.Name,UnityEngine.Random.Range(Mathf.RoundToInt(item.MaxStackSize*LootItem.MinStack), Mathf.RoundToInt(item.MaxStackSize * LootItem.MaxStack)));
                }
                ActualSlotNumber += item.SizeX*item.SizeY;
                simpleInventory.InventoryAdd(item);
            }
        }
    }

    public static class InventorySystem
    {
        //Uj Parent Itemet allit be erre akkor van szukseg ha meg akarod határozni, hogy melyik item tárolja őt
        public static void SetNewDataParent(Item SetTo,Item Data)
        {
            Data.ParentItem = SetTo;
        }

        //A placer egy Live változó ez azt jelenti, hogy csak akkor létezik ha az inventory meg van nyitva, illetve a kijelölt(Sárga színű) slotobjectumokat használja targetnek
        //feladata, hogy az itemObjectuma számára megmondja, hogy Parentobjectumán belül melyik slotokat foglalja el.
        public static void SetSlotUseByPlacer(PlacerStruct Placer, Item Data)// 0.
        {
            Data.SlotUse.Clear();
            for (int i = 0; i < Placer.ActiveItemSlots.Count; i++)
            {
                Data.SlotUse.Add(Placer.ActiveItemSlots[i].name);
            }
            Data.SetSlotUse();

            if ((Data.IsEquipment && !Placer.NewParentData.IsEquipmentRoot) || !Placer.NewParentData.IsInPlayerInventory)
            {
                UnSetHotKey(Data);
            }
            else if (Placer.NewParentData.IsEquipmentRoot)
            {
                UnSetHotKey(Data);
                AutoSetHotkey(Data);
            }

            if (Placer.NewParentData.IsEquipmentRoot)
            {
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
            }
        }

        //Ez egy NonLive metodus az az csak adatok alapján dolgozik az inevntory bezárásakor.
        //mukodese: abba az itembe(AddTo) amelybe helyezni akarjuk az itemunket(Data) meghatározhatjuk, hogy melyik sectorában(sectorIndex) és, hogy melyik slot(X,Y) legyen az itemunk(Data) bal felső sarka
        public static void SetSlotUseBySector(int Y,int X,int sectorIndex, Item AddTo,Item Data)
        {
            Data.SlotUse.Clear();
            if (AddTo.IsEquipmentRoot)
            {
                Data.SlotUse.Add(AddTo.Container.Sectors[sectorIndex][Y, X].SlotName);//ez alapjan azonositunk egy itemslotot
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
                for (int y = Y; y < Y + Data.SizeY; y++)
                {
                    for (int x = X; x < X + Data.SizeX; x++)
                    {
                        Data.SlotUse.Add(AddTo.Container.Sectors[sectorIndex][y, x].SlotName);//ez alapjan azonositunk egy itemslotot
                    }
                }
            }
            Data.SetSlotUse();

            if ((Data.IsEquipment && !AddTo.IsEquipmentRoot) || !AddTo.IsInPlayerInventory)
            {
                UnSetHotKey(Data);
            }
            else if (AddTo.IsEquipmentRoot)
            {
                UnSetHotKey(Data);
                AutoSetHotkey(Data);
            }

            if (AddTo.IsEquipmentRoot)
            {
                Data.IsEquipment = true;
            }
            else
            {
                Data.IsEquipment = false;
            }
        }

        //Hozzáad egy itemet egy másik itemhez
        public static void DataAdd(Item AddTo, Item Data)// 1.
        {
            foreach (ItemSlotData[,] sector in AddTo.Container.Sectors)
            {
                foreach (ItemSlotData slot in sector)
                {
                    if (Data.SlotUse.Contains(slot.SlotName))
                    {
                        slot.PartOfItemData = Data;
                    }
                }
            }
            AddTo.Container.Items.Add(Data);
            int lvl = AddTo.Lvl;
            Data.Lvl = ++lvl;
            if (Data.SelfGameobject)
            {
                foreach (DataGrid dataGrid in AddTo.SectorDataGrid)
                {
                    foreach (RowData rowData in dataGrid.col)
                    {
                        foreach (GameObject slot in rowData.row)
                        {
                            if (Data.SlotUse.Contains(slot.name))
                            {
                                slot.GetComponent<ItemSlot>().PartOfItemObject = Data.SelfGameobject;
                            }
                        }
                    }
                }
            }
            if (AddTo.IsInPlayerInventory)
            {
                Data.IsInPlayerInventory = true;
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Add(Data);
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
            }
        }

        //eltávolít egy itemet egy másik itemből
        public static void DataRemove(Item RemoveFrom, Item Data)// 1.
        {
            foreach (ItemSlotData[,] sector in RemoveFrom.Container.Sectors)
            {
                foreach (ItemSlotData slot in sector)
                {
                    if (Data.SlotUse.Contains(slot.SlotName))
                    {
                        slot.PartOfItemData = null;
                    }
                }
            }
            RemoveFrom.Container.Items.Remove(Data);
            if (Data.SelfGameobject)
            {
                foreach (DataGrid dataGrid in RemoveFrom.SectorDataGrid)
                {
                    foreach (RowData rowData in dataGrid.col)
                    {
                        foreach (GameObject slot in rowData.row)
                        {
                            if (Data.SlotUse.Contains(slot.name))
                            {
                                slot.GetComponent<ItemSlot>().PartOfItemObject = null;
                            }
                        }
                    }
                }
            }
            if (Data.IsInPlayerInventory)
            {
                Data.IsInPlayerInventory = false;
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Remove(Data);
                InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
            }
        }
        public static void UnSetHotKey(Item Data)
        {
            if (Data.HotKey != "")
            {
                switch (int.Parse(Data.HotKey))
                {
                    case 1:
                        InGameUI.SetHotKey1(null);
                        break;
                    case 2:
                        InGameUI.SetHotKey2(null);
                        break;
                    case 3:
                        InGameUI.SetHotKey3(null);
                        break;
                    case 4:
                        InGameUI.SetHotKey4(null);
                        break;
                    case 5:
                        InGameUI.SetHotKey5(null);
                        break;
                    case 6:
                        InGameUI.SetHotKey6(null);
                        break;
                    case 7:
                        InGameUI.SetHotKey7(null);
                        break;
                    case 8:
                        InGameUI.SetHotKey8(null);
                        break;
                    case 9:
                        InGameUI.SetHotKey9(null);
                        break;
                    default:
                        break;
                }
                Data.HotKey = "";
            }
        }
        public static void AutoSetHotkey(Item Data)
        {
            switch (Data.LowestSlotUseNumber)
            {
                case 10:
                    Data.HotKey = "1";
                    InGameUI.SetHotKey1(Data);
                    break;
                case 11:
                    Data.HotKey = "2";
                    InGameUI.SetHotKey2(Data);
                    break;
                case 12:
                    Data.HotKey = "3";
                    InGameUI.SetHotKey3(Data);
                    break;
                case 13:
                    Data.HotKey = "4";
                    InGameUI.SetHotKey4(Data);
                    break;
                default:
                    break;
            }
        }
    }
}