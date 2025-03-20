using Ammunition;
using Armors;
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
using WeaponBodys;
using Dustcovers;
using Grips;
using Handguards;
using Magasines;
using Modgrips;
using Muzzles;
using Sights;
using Stoks;
using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using NaturalInventorys;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;
using UI;
using Newtonsoft.Json.Linq;
using static MainData.SupportScripts;
using static TestModulartItems.TestModularItems;
using static MainData.Main;
using UnityEngine.UI;
using Assets.Scripts.Inventory;
using ExelFileReader;
using System.Collections;

namespace ItemHandler
{
    public class ItemSlotData
    {
        public int SectorID;
        public (int Height, int Widht) Coordinate;
        public string SlotType;
        public Item PartOfItemData;

        public ItemSlotData(int SectorID,int Height,int Width,string SlotType = "", Item PartOfItemData = null)
        {
            this.SectorID = SectorID;
            this.Coordinate = (Height,Width);
            this.SlotType = SlotType;
            this.PartOfItemData = PartOfItemData;
        }
    }
    public class Item
    {
        public const string SimpleItemObjectParth = "GameElements/SimpleItemObject";
        public const string AdvancedItemObjectParth = "GameElements/AdvancedItemObject";
        public const string TemporaryItemObjectPath = "GameElements/TemporaryAdvancedItemObject";
        //system variables

        #region SelfRefVariables
        public ModificationWindow ModificationWindowRef;
        public LevelManager LevelManagerRef;
        public List<ItemSlotData> ItemSlotsDataRef = new List<ItemSlotData>();
        public List<Item> ContainerItemListRef = new List<Item>();
        public HotKey hotKeyRef;
        public List<ItemSlot> ItemSlotObjectsRef = new List<ItemSlot>();
        #endregion

        #region PlacerVariables
        public List<Action> AvaiablePlacerMetodes = new List<Action>();
        public Item AvaiableParentItem { get; set; }
        #endregion

        public List<Part> Parts { get; set; }//az item darabjai
        public string ImgPath { get; set; }
        public string ObjectPath { get; private set; }//az, hogy milyen obejctum tipust hasznal
        public Item ParentItem { get; set; }//az az item ami tárolja ezt az itemet
        public GameObject SelfGameobject { get; set; }// a parent objectum
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
        public float RotateDegree { get; set; } = 0f;
        public bool IsInPlayerInventory { get; set; } = false;// a player inventory tagja az item
        public bool IsEquipment { set; get; } = false;// az item egy equipment
        public bool IsLoot { set; get; } = false;// az item a loot conténerekben van
        public bool IsRoot { set; get; } = false;// az item egy root data
        public bool IsEquipmentRoot { set; get; } = false;// az item a player equipmentjeinek rootja ebbol csak egy lehet
        public bool IsAdvancedItem { set; get; } = false;// az item egy advanced item

        //SlotUse
        public int SectorId { get; set; }
        private (int, int)[] coordinates;
        public (int,int)[] Coordinates 
        {
            get
            {
                return coordinates;
            }
            set
            {
                coordinates = value;
                Array.Sort(coordinates);
            }
        }
        //action (Műveletek)
        public bool IsDropAble { get; set; } = false;
        public bool IsRemoveAble { get; set; } = true;
        public bool IsUnloadAble { get; set; } = false;
        public bool IsModificationAble { get; set; } = false;
        public bool IsOpenAble { get; set; } = false;
        public bool IsUsable { get; set; } = false;
        public bool CanReload { get; set; } = false;

        #region NonGeneric Variables
        //Size Changer
        public SizeChanger SizeChanger {get; set; }
        //contain
        public Container Container { get; set; }
        //weapon
        public int? MagasineSize { get; set; }
        public double? Spread { get; set; }
        public int? Fpm { get; set; }
        public double? Recoil { get; set; }
        public double? Accturacy { get; set; }
        public double? Range { get; set; }
        public double? Ergonomy { get; set; }
        public string AmmoType { get; set; } = "";
        public BulletType BulletType { get; set; }
        //usable
        public int UseLeft { get; set; } = 0;
        //ammo

        //med

        //armor
        #endregion

        //Ez egy Totális Törlés ami azt jelenti, hogy mindenhonnan törli. Ez nem jo akkor ha valahonnan torolni akarjuk de mashol meg hozzadni
        public void Remove()
        {
            if (IsRemoveAble)
            {
                InventorySystem.Delete(this);
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
                    InventorySystem.Delete(this);
                    if (SelfGameobject)
                    {
                        SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                        GameObject.Destroy(SelfGameobject);
                    }
                }
            }
        }
        //action (Live/NonLive Inventory)
        public void Open()
        {

        }
        public void Modification()
        {
            InGameUI.PlayerInventory.GetComponent<WindowManager>().CreateModificationPanel(this);
        }
        public void Reload()
        {

        }
        public void Unload()
        {

        }
        public void Drop()
        {

        }
        public Item ShallowClone()
        {
            Item cloned = new()
            {
                // Alap adatok
                ImgPath = this.ImgPath,
                ObjectPath = this.ObjectPath,
                ItemType = this.ItemType,
                ItemName = this.ItemName,
                Description = this.Description,
                Quantity = this.Quantity,
                Value = this.Value,
                SizeX = this.SizeX,
                SizeY = this.SizeY,
                MaxStackSize = this.MaxStackSize,

                // Akciók
                IsDropAble = this.IsDropAble,
                IsRemoveAble = this.IsRemoveAble,
                IsUnloadAble = this.IsUnloadAble,
                IsModificationAble = this.IsModificationAble,
                IsOpenAble = this.IsOpenAble,
                IsUsable = this.IsUsable,
                IsAdvancedItem = this.IsAdvancedItem,

                IsRoot = this.IsRoot,
                IsEquipment = this.IsEquipment,
                IsLoot = this.IsLoot,
                IsEquipmentRoot = this.IsEquipmentRoot,
                IsInPlayerInventory = this.IsInPlayerInventory,

                // Fegyver adatok
                MagasineSize = this.MagasineSize,
                Spread = this.Spread,
                Fpm = this.Fpm,
                Recoil = this.Recoil,
                Accturacy = this.Accturacy,
                Range = this.Range,
                Ergonomy = this.Ergonomy,
                AmmoType = this.AmmoType,

                // Használhatóság
                UseLeft = this.UseLeft,

                // Advanced
                SizeChanger = this.SizeChanger,

                Lvl = this.Lvl,
                SectorId = this.SectorId,

                RotateDegree = this.RotateDegree,



                //BulletType = this.BulletType,
            };

            if (Container != null)
            {
                cloned.Container = new Container(this.Container.PrefabPath);
            }

            if (Coordinates != null)
            {
                cloned.Coordinates = this.Coordinates.ToArray();
            }

            return cloned;
        }
        public (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) PartPut_IsPossible(Item Incoming_AdvancedItem)
        {
            if (IsAdvancedItem && Incoming_AdvancedItem.IsAdvancedItem)
            {
                //amit rá helyezunk
                ConnectionPoint[] IncomingCPs = Incoming_AdvancedItem.Parts.SelectMany(x => x.ConnectionPoints).ToArray();//az összes connection point amitje az itemnek van
                                                                                                                          //amire helyezunk
                ConnectionPoint[] SelfCPs = Parts.SelectMany(x => x.ConnectionPoints).ToArray();//az össze sconnection point amihez hozzadhatja
                /*
                 * ellenorizzuk, hogy a CP-k egyike sincs e hasznalva
                 * ellenorizzuk, hogy a self cp kompatibilis e az incoming cp-vel
                 */
                foreach (ConnectionPoint SCP in SelfCPs)
                {
                    foreach (ConnectionPoint ICP in IncomingCPs)
                    {
                        if (!SCP.Used && !ICP.Used && SCP.CPData.CompatibleItemNames.Contains(ICP.SelfPart.PartData.PartName))
                        {
                            return (SCP, ICP, true);
                        }
                    }
                }
            }
            return (null,null,false);
        }
        public void PartPut(Item AdvancedItem, ConnectionPoint SCP, ConnectionPoint ICP)//ha egy item partjait belerakjuk akkor az item az inventoryban megmaradhat ezert azt torolni kellesz vagy vmi
        {
            SCP.Connect(ICP);

            int baseHierarhicPlace = SCP.SelfPart.HierarhicPlace;
            int IncomingCPPlace = ICP.SelfPart.HierarhicPlace;
            int hierarhicPlaceChanger = 0;

            if (baseHierarhicPlace < IncomingCPPlace)
            {
                hierarhicPlaceChanger = (IncomingCPPlace - (++baseHierarhicPlace)) * -1;
            }
            else if (baseHierarhicPlace > IncomingCPPlace)
            {
                hierarhicPlaceChanger = baseHierarhicPlace - IncomingCPPlace + 1;
            }
            else
            {
                hierarhicPlaceChanger = 1;
            }

            foreach (Part part in AdvancedItem.Parts)
            {
                part.HierarhicPlace += hierarhicPlaceChanger;
            }

            Parts.AddRange(AdvancedItem.Parts);

            Parts = Parts.OrderBy(part => part.HierarhicPlace).ToList();

            InventorySystem.Delete(AdvancedItem);//törli az advanced itemet amely a partokat tartalmazta

            AdvancedItemContsruct();
        }
        public List<Part> PartCut(Part part)
        {
            //Debug.LogWarning(Parts.SelectMany(x => x.ConnectionPoints).ToArray().Last().ConnectedPoint.SelfPart != null);
            ConnectionPoint CPStand = Parts.SelectMany(x => x.ConnectionPoints).FirstOrDefault(y => y.ConnectedPoint?.SelfPart == part);
            ConnectionPoint CPOff = Parts.SelectMany(x => x.ConnectionPoints).FirstOrDefault(y => y.SelfPart == part);
            CPStand.Disconnect();
            List<Part> parts = new()
            {
                part
            };
            part.GetConnectedPartsTree(parts);
            //Debug.LogWarning("-----------------------------PartCut-------------------------------");
            foreach (Part part_ in parts)
            {
                Parts.Remove(part_);
                //Debug.LogWarning(part_.PartData.PartName);
            }
            //Debug.LogWarning("------------------------------------------------------------");
            Parts = Parts.OrderBy(part => part.HierarhicPlace).ToList();
            parts = parts.OrderBy(part => part.HierarhicPlace).ToList();

            AdvancedItemContsruct();

            return parts;
        }
        public void AdvancedItemContsruct()
        {
            Item FirstItem = Parts.First().item_s_Part;
            
            if (Parts.Count > 1)
            {
                MainItem mainItem = Parts.Select(part => AdvancedItemHandler.AdvancedItemDatas.GetMainItemData(part.item_s_Part.ItemName)).FirstOrDefault(mi => !string.IsNullOrEmpty(mi.MainItemName));
                if (mainItem.MainItemName != null && mainItem.NecessaryItemTypes.All(Type => Parts.Exists(part=>part.item_s_Part.ItemType == Type)))
                {
                    Debug.LogWarning("MainItem");
                    ItemName = mainItem.MainItemName;
                    Description = mainItem.Desctription;
                    ItemType = mainItem.Type;
                }
                else if(mainItem.MainItemName != null)
                {
                    Debug.LogWarning("incompleted MainItem");
                    ItemName = $"Incompleted {mainItem.MainItemName}";
                    Description = mainItem.Desctription;
                    ItemType = mainItem.Type;
                }

                SizeX = FirstItem.SizeX;
                SizeY = FirstItem.SizeY;
                Container = FirstItem.Container;
                Value = 0;
                Spread = 0;
                Fpm = 0;
                Recoil = 0;
                Accturacy = 0;
                Range = 0;
                Ergonomy = 0;

                foreach (Part part in Parts)
                {
                    Item item = part.item_s_Part;
                    Value += item.Value;
                    if (item.SizeChanger != null)
                    {
                        string direction = item.SizeChanger.Direction;
                        SizeChanger sizeChanger = item.SizeChanger;
                        if (direction == "R" || direction == "L")
                        {
                            SizeX += sizeChanger.Plus;
                            if (SizeX > sizeChanger.MaxPlus)
                            {
                                SizeX = sizeChanger.MaxPlus;
                            }
                        }
                        else
                        {
                            SizeY += sizeChanger.Plus;
                            if (SizeY > sizeChanger.MaxPlus)
                            {
                                SizeY = sizeChanger.MaxPlus;
                            }
                        }
                        //Debug.Log($"{SizeX} x {SizeY}");
                    }
                    if (item.IsDropAble)
                    {
                        IsDropAble = item.IsDropAble;
                    }
                    if (item.IsUnloadAble)
                    {
                        IsUnloadAble = item.IsUnloadAble;
                    }
                    if (item.IsRemoveAble)
                    {
                        IsRemoveAble = item.IsRemoveAble;
                    }
                    if (item.IsOpenAble)
                    {
                        IsOpenAble = item.IsOpenAble;
                    }
                    if (item.IsUsable)
                    {
                        IsUsable = item.IsUsable;
                    }
                    if (item.BulletType != null)//csak 1 lehet
                    {
                        BulletType = item.BulletType;
                    }
                    if (item.AmmoType != null)//csak 1 lehet
                    {
                        AmmoType = item.AmmoType;
                    }
                    if (item.Spread != 0)
                    {
                        Spread += item.Spread;
                    }
                    if (item.Fpm != 0)
                    {
                        Fpm += item.Fpm;
                    }
                    if (item.Recoil != 0)
                    {
                        Recoil += item.Recoil;
                    }
                    if (item.Accturacy != 0)
                    {
                        Accturacy += item.Accturacy;
                    }
                    if (item.Range != 0)
                    {
                        Range += item.Range;
                    }
                    if (item.Ergonomy != 0)
                    {
                        Ergonomy += item.Ergonomy;
                    }
                    if (item.MagasineSize != 0)
                    {
                        MagasineSize += item.MagasineSize;
                    }
                    //hasznalhato e?
                    if (UseLeft != 0)
                    {
                        UseLeft = item.UseLeft;
                    }
                }
            }
            else
            {
                SizeX = FirstItem.SizeX;
                SizeY = FirstItem.SizeY;
                ItemName = FirstItem.ItemName;
                Description = FirstItem.Description;
                Value = FirstItem.Value;
                //Action
                IsDropAble = FirstItem.IsDropAble;
                IsUnloadAble = FirstItem.IsUnloadAble;
                IsRemoveAble = FirstItem.IsRemoveAble;
                IsOpenAble = FirstItem.IsOpenAble;
                IsUsable = FirstItem.IsUsable;
                //fegyver adatok
                MagasineSize = FirstItem.MagasineSize;
                Spread = FirstItem.Spread;
                Fpm = FirstItem.Fpm;
                Recoil = FirstItem.Recoil;
                Accturacy = FirstItem.Accturacy;
                Range = FirstItem.Range;
                Ergonomy = FirstItem.Ergonomy;
                BulletType = FirstItem.BulletType;
                AmmoType = FirstItem.AmmoType;
                //hasznalhato e?
                UseLeft = FirstItem.UseLeft;

                Description = FirstItem.Description;

                ItemType = FirstItem.ItemType;
            }
            Quantity = 1;
            MaxStackSize = 1;
            IsModificationAble = true;
            IsAdvancedItem = true;
            ObjectPath = FirstItem.ObjectPath;

            if (SelfGameobject != null)
            {

            }
            //if (originalSizeX != SizeX || originalSizeY != SizeY)
            //{
            //    foreach (SizeChanger changer in OriginalsizeChangers)
            //    {

            //    }
            //}
        }
        //action (Only Live Inventory)
        public void Shoot()
        {

        }
        public Item()//ha contume itememt akarunk letrehozni mint pl: egy Root item
        {

        }
        public Item(string name, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            Item completedItem = name switch
            {
                //Test
                "TestBackpack" => new TestBackpack().Set(),
                "TestVest" => new TestVest().Set(),
                "TestFingers" => new TestFingers().Set(),
                "TestBoots" => new TestBoots().Set(),
                "AK103" => new AK103().Set(),

                "TestCenter" => new TestCenter().Set(),
                "TestBox" => new TestBox().Set(),
                //"TestUpper" => new AK103().Set(),
                //"TestButtom" => new AK103().Set(),
                //"TestHead" => new AK103().Set(),
                //"TestFoot" => new AK103().Set(),
                //"TestFront" => new AK103().Set(),
                //"TestBack" => new AK103().Set(),

                //Weapon Bodys
                "AKS-74U_Body" => new AKS74UBody().Set(),

                //Dustcovers
                "AKS-74U_dust_cover" => new AKS_74U_dust_cover().Set(),
                "AKS-74U_Legal_Arsenal_Pilgrim_railed_dust_cover" => new AKS_74U_Legal_Arsenal_Pilgrim_railed_dust_cover().Set(),

                //Grips
                "AK_Zenit_RK-3_pistol_grip" => new AK_Zenit_RK_3_pistol_grip().Set(),
                "AKS-74U_bakelite_pistol_grip" => new AKS_74U_bakelite_pistol_grip().Set(),

                //Handguars
                "AKS-74U_Wooden_handguard" => new AKS_74U_Wooden_handguard().Set(),
                "AKS-74U_Zenit_B-11_handguard" => new AKS_74U_Zenit_B_11_handguard().Set(),

                //Magasines
                "AK-74_5.45x39_6L20_30-round_magasine" => new AK_74_545x39_6L20_30_round_magasine().Set(),
                "AK-74_5.45x39_6L31_60-round_magasine" => new AK_74_545x39_6L31_60_round_magasine().Set(),

                //Modgrips
                "KAC_vertical_foregrip" => new KAC_vertical_foregrip().Set(),

                //Muzzles
                "AK-105_5.45x39_muzzle_brake-compensator" => new AK_105_545x39_muzzle_brake_compensator().Set(),

                //Sights
                "Walther_MRS_reflex_sight" => new Walther_MRS_reflex_sight().Set(),

                //Stoks
                "AKS-74U_Skeletonized_Stock" => new AKS_74U_Skeletonized_Stock().Set(),

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
            if (completedItem.IsAdvancedItem)
            {
                Item item = new()
                {
                    ImgPath = completedItem.ImgPath,
                    ObjectPath = AdvancedItemObjectParth,
                    ItemType = completedItem.ItemType,//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
                    ItemName = completedItem.ItemName,//ez alapján hozza létre egy item saját magát
                    Description = completedItem.Description,
                    Quantity = completedItem.Quantity,
                    Value = completedItem.Value,
                    SizeX = completedItem.SizeX,
                    SizeY = completedItem.SizeY,
                    MaxStackSize = completedItem.MaxStackSize,
                    //Action
                    IsDropAble = completedItem.IsDropAble,
                    IsRemoveAble = completedItem.IsRemoveAble,
                    IsUnloadAble = completedItem.IsUnloadAble,
                    IsModificationAble = completedItem.IsModificationAble,
                    IsOpenAble = completedItem.IsOpenAble,
                    IsUsable = completedItem.IsUsable,
                    IsAdvancedItem = completedItem.IsAdvancedItem,
                    //tartalom
                    Container = completedItem.Container,
                    //fegyver adatok
                    MagasineSize = completedItem.MagasineSize,
                    Spread = completedItem.Spread,
                    Fpm = completedItem.Fpm,
                    Recoil = completedItem.Recoil,
                    Accturacy = completedItem.Accturacy,
                    Range = completedItem.Range,
                    Ergonomy = completedItem.Ergonomy,
                    BulletType = completedItem.BulletType,
                    AmmoType = completedItem.AmmoType,
                    //hasznalhato e?
                    UseLeft = completedItem.UseLeft,
                    //Advanced
                    SizeChanger = completedItem.SizeChanger,
                };
                Parts = new List<Part>
                {
                    new(item)
                };
                //fügvény ami az össze spart ertekeit az advanced valtozoba tölti és adja össze
                AdvancedItemContsruct();

                //Debug.LogWarning($"Item(A) created {completedItem.ItemName}");
            }
            else
            {
                ObjectPath = SimpleItemObjectParth;

                //altalanos adatok
                ImgPath = completedItem.ImgPath;
                ItemType = completedItem.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
                ItemName = completedItem.ItemName;//ez alapján hozza létre egy item saját magát
                Description = completedItem.Description;
                Quantity = completedItem.Quantity;
                Value = completedItem.Value;
                SizeX = completedItem.SizeX;
                SizeY = completedItem.SizeY;
                MaxStackSize = completedItem.MaxStackSize;
                //Action
                IsDropAble = completedItem.IsDropAble;
                IsRemoveAble = completedItem.IsRemoveAble;
                IsUnloadAble = completedItem.IsUnloadAble;
                IsModificationAble = completedItem.IsModificationAble;
                IsOpenAble = completedItem.IsOpenAble;
                IsUsable = completedItem.IsUsable;
                IsAdvancedItem = completedItem.IsAdvancedItem;
                //tartalom
                Container = completedItem.Container;
                //fegyver adatok
                MagasineSize = completedItem.MagasineSize;
                Spread = completedItem.Spread;
                Fpm = completedItem.Fpm;
                Recoil = completedItem.Recoil;
                Accturacy = completedItem.Accturacy;
                Range = completedItem.Range;
                Ergonomy = completedItem.Ergonomy;
                BulletType = completedItem.BulletType;
                AmmoType = completedItem.AmmoType;
                //hasznalhato e?
                UseLeft = completedItem.UseLeft;

                //Debug.Log($"Item(S) created {ItemName} - {Quantity}db");
            }
        }
    }
    public class SizeChanger
    {
        public SizeChanger(int plus, int maxPlus, string direction)
        {
            Plus = plus;
            MaxPlus = maxPlus;
            Direction = direction;
        }
        public int Plus { get; private set; }
        public int MaxPlus { get; private set; }
        public string Direction { get; private set; }
    }
    //a connection point inpectorban létező dolog ami lenyegeben statikusan jelen van nem kell generalni
    [System.Serializable]
    public class ConnectionPoint
    {
        //Live adatok meylek addig vannak amig a connectionPoint létezik
        public GameObject RefPoint1 = null;//LIVE
        public GameObject RefPoint2 = null;//LIVE

        //active adatok melyek valtozhatnak
        public ConnectionPoint ConnectedPoint = null;//amelyik ponttal össze van kötve
        public bool Used = false;//alapotjezo

        //statikus adatok melyek nem valtoznak
        public CP CPData;
        public Part SelfPart;//a part amelyikhez tartozik
        public ConnectionPoint(CP cPData,Part selfPart)
        {
            CPData = cPData;
            SelfPart = selfPart;
        }
        public void Connect(ConnectionPoint cp)
        {
            ConnectedPoint = cp;
            Used = true;
            cp.ConnectedPoint = this;
            cp.Used = true;
        }
        public void Disconnect()
        {
            ConnectedPoint.Used = false;
            ConnectedPoint.ConnectedPoint = null;
            Used = false;
            ConnectedPoint = null;
        }
        public void SetLive()
        {
            GameObject CP = CreatePrefab(AdvancedItemHandler.CPPath);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            CP.transform.SetParent(SelfPart.PartObject.transform.GetChild(0).transform);
            Texture2D texture = Resources.Load<Texture2D>(SelfPart.PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;

            //!!! miert valtozik a scale ez elott meg?
            CP.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            CP.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            CP.GetComponent<RectTransform>().localPosition = Vector2.zero;

            CP.name = CPData.PointName;
            RefPoint1 = CP.transform.GetChild(0).gameObject;
            RefPoint2 = CP.transform.GetChild(1).gameObject;

            RectTransform rt1 = RefPoint1.GetComponent<RectTransform>();
            rt1.anchoredPosition = Vector2.zero;
            rt1.anchorMin = new Vector2(CPData.AnchorMin1.X, CPData.AnchorMin1.Y);
            rt1.anchorMax = new Vector2(CPData.AnchorMax1.X, CPData.AnchorMax1.Y);


            RectTransform rt2 = RefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(CPData.AnchorMin2.X, CPData.AnchorMin2.Y);
            rt2.anchorMax = new Vector2(CPData.AnchorMin2.X, CPData.AnchorMin2.Y);
        }
    }
    public class Part
    {
        //Live adatok meylek addig vannak amig a connectionPoint létezik
        public GameObject PartObject;//csak live ban van

        //active adatok melyek valtozhatnak
        public int HierarhicPlace = 0;

        //statikus adatok melyek nem valtoznak
        public ConnectionPoint[] ConnectionPoints;//a tartalmazott pontok
        public Item item_s_Part;//az item aminek a partja
        public ItemPartData PartData;
        public Part(Item item)
        {
            item_s_Part = item;
            PartData = AdvancedItemHandler.AdvancedItemDatas.GetPartData(item.ItemName);
            ConnectionPoints = new ConnectionPoint[PartData.CPs.Length];

            for (int i = 0; i < ConnectionPoints.Length; i++)
            {
                ConnectionPoints[i] = new ConnectionPoint(PartData.CPs[i], this);
            }
        }
        public void SetLive(GameObject ParentObject)
        {
            //Debug.LogWarning($"Set {PartData.PartName}");
            GameObject Part = CreatePrefab(AdvancedItemHandler.PartPath);
            //Debug.LogWarning($"creted obejct {Part.GetInstanceID()}");
            PartObject = Part;
            //Debug.LogWarning($"referalt obejct {PartObject.GetInstanceID()}");
            Part.name = PartData.PartName;
            Part.GetComponent<PartObject>().SelfData = this;

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            Part.transform.SetParent(ParentObject.transform);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            Sprite sprite = Resources.Load<Sprite>(PartData.ImagePath);
            Part.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            Part.GetComponent<RectTransform>().localPosition = Vector2.zero;
            Part.GetComponent<RectTransform>().localScale = Vector3.one;
            Texture2D texture = Resources.Load<Texture2D>(PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;
            Part.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            Part.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
        }
        public void UnSetLive()//out of order
        {
            //Debug.LogWarning($"UnsetPart {PartData.PartName}");
            if (PartObject != null)
            {
                //Debug.LogWarning($"deleted obejct {PartObject.GetInstanceID()}");
                PartObject.transform.SetParent(null);
                UnityEngine.Object.Destroy(PartObject);
            }
        }
        public void GetConnectedPartsTree(List<Part> FillableList)
        {
            foreach (ConnectionPoint cp in ConnectionPoints)
            {
                if (cp.Used && cp.ConnectedPoint.SelfPart.HierarhicPlace > HierarhicPlace)
                {
                    FillableList.Add(cp.ConnectedPoint.SelfPart);
                    cp.ConnectedPoint.SelfPart.GetConnectedPartsTree(FillableList);
                }
            }
        }
    }
    public class BulletType
    {

    }
    public class Container
    {
        //egy container az itemjéhez tartozik.
        //az item constructor selekciójánál itemet peldanyositunk: pl: TestWeapon
        //ebben az eddig null érékű container változó egy ures containerrre változik
        //az item pédányosításánál igy egy új példány készül a containerből is mely alapvetően tartalmazza a container PrefabPath-ét
        //a kostructora az igy megkapott prefabPath-ből lekerdezi a Sectorokat
        public string PrefabPath;
        public List<Item> Items { get; set; }
        public ItemSlotData[][,] NonLive_Sectors { get; set; }
        public ItemSlot[][,] Live_Sector { get; set; }//ezek referanca pontokat atralamaznak amelyeken kersztul a tenyleges gameobjectumokat manipulalhatjuk
        public GameObject ContainerObject { get; set; }//conainer objectum
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            ContainerObject containerObject = Resources.Load(prefabPath).GetComponent<ContainerObject>();
            ContainerObject.SectorData[] staticSectorDatas = containerObject.StaticSectorDatas;

            Items = new List<Item>();
            NonLive_Sectors = new ItemSlotData[staticSectorDatas.Length][,];
            Live_Sector = new ItemSlot[staticSectorDatas.Length][,];

            for (int sector = 0; sector < NonLive_Sectors.Length; sector++)
            {
                NonLive_Sectors[sector] = new ItemSlotData[staticSectorDatas[sector].Heigth, staticSectorDatas[sector].Widht];
                Live_Sector[sector] = new ItemSlot[staticSectorDatas[sector].Heigth, staticSectorDatas[sector].Widht];

                for (int height = 0; height < NonLive_Sectors[sector].GetLength(0); height++)
                {
                    for (int width = 0; width < NonLive_Sectors[sector].GetLength(1); width++)
                    {
                        ItemSlot RefSlot = staticSectorDatas[sector].SectorObject.transform.GetChild(height* NonLive_Sectors[sector].GetLength(1)+width).GetComponent<ItemSlot>();
                        NonLive_Sectors[sector][height, width] = new ItemSlotData(sector,height,width,RefSlot.SlotType);
                    }
                }
            }
        }
    }
    public static class LootRandomizer
    {
        private static readonly List<LootItem> weapons = new()
        {
            new LootItem("Glock_17_9x19_pistol_PS9",1f),
            new LootItem("AK103", 2f),
            new LootItem("APOK_Tactical_Wasteland_Gladius",2f),
            new LootItem("7.62x39FMJ",2f,0.1f,0.5f)//jelentese, hogy 10% és 50% staksize között spawnolhat.
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

            public LootItem(string Name, float SpawnRate = 0, float MinStack = 1f, float MaxStack = 1f)
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
        public static void FillSimpleInvenotry(SimpleInventory simpleInventory, string PaletteName, float Fullness)
        {
            float MaxSlotNumber = 0;
            foreach (ItemSlotData[,] row in simpleInventory.Root.Container.NonLive_Sectors)
            {
                foreach (ItemSlotData slot in row)
                {
                    MaxSlotNumber++;
                }
            }
            float ActualSlotNumber = 0;
            Math.Round(MaxSlotNumber *= Fullness, 0);
            List<LootItem> WeightedList = GenerateLoot(PaletteName);
            while (MaxSlotNumber > ActualSlotNumber)
            {
                LootItem LootItem = WeightedList[UnityEngine.Random.Range(0, WeightedList.Count)];
                Item item = new(LootItem.Name);
                if (item.MaxStackSize > 1)
                {
                    item = new Item(LootItem.Name, UnityEngine.Random.Range(Mathf.RoundToInt(item.MaxStackSize * LootItem.MinStack), Mathf.RoundToInt(item.MaxStackSize * LootItem.MaxStack)));
                }
                ActualSlotNumber += item.SizeX * item.SizeY;
                simpleInventory.InventoryAdd(item);
            }
        }
    }
    public static class InventorySystem
    {
        #region Placer Metodes
        public class Merge
        {
            public Item Stand { get; private set; }
            public Item Incoming { get; private set; }
            public Merge(Item stand, Item incoming)
            {
                Stand = stand;
                Incoming = incoming;
            }
            public void Execute_Merge()
            {
                int count = Stand.Quantity;
                Stand.Quantity += Incoming.Quantity;
                if (Stand.Quantity > Stand.MaxStackSize)
                {
                    Incoming.Quantity = Stand.Quantity - Stand.MaxStackSize;
                    Stand.Quantity = Stand.MaxStackSize;
                    Stand.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    Incoming.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                }
                else
                {
                    Stand.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    Delete(Incoming);
                }
            }
        }
        public class Split
        {
            public Item Incoming { get; private set; }
            public ItemSlot[] ActiveSlots { get; private set; }
            public Item Stand { get; private set; } = null;
            public Split(Item incoming, ItemSlot[] activeSlots)
            {
                Incoming = incoming;
                ActiveSlots = activeSlots.ToArray();
                if (ActiveSlots.First().PartOfItemObject != null)
                {
                    Stand = ActiveSlots.First().PartOfItemObject.GetComponent<ItemObject>().ActualData;
                }
            }
            public void Execute_Split()
            {
                (int smaller, int larger) = SplitInteger(Incoming.Quantity);

                if (Stand != null)//split and Merge
                {
                    Stand.Quantity += larger;
                    Incoming.Quantity = smaller;
                    if (Stand.Quantity > Stand.MaxStackSize)//ha a split több mint a maximalis stacksize
                    {
                        Incoming.Quantity += (Stand.Quantity - Stand.MaxStackSize);
                        Stand.Quantity = Stand.MaxStackSize;
                        Stand.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                        Incoming.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    }
                    else//ha nem több a split mint a maximális stacksize
                    {
                        Incoming.Quantity = smaller;
                        Stand.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                        if (Incoming.Quantity < 1)
                        {
                            Delete(Incoming);
                        }
                        else
                        {
                            Incoming.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                        }
                    }
                }
                else
                {
                    Item Parent = ActiveSlots.First().SlotParentItem;

                    Item newItem = new(Incoming.ItemName, larger);

                    GameObject itemObject = CreatePrefab(Incoming.ObjectPath);
                    itemObject.name = newItem.ItemName;
                    newItem.SelfGameobject = itemObject;
                    newItem.ParentItem = Parent;
                    itemObject.GetComponent<ItemObject>().SetDataRoute(newItem, newItem.ParentItem);

                    Add(newItem, Parent);
                    Live_Positioning(newItem, ActiveSlots);

                    NonLive_Placing(newItem, Parent);
                    Live_Placing(newItem, Parent);

                    HotKey_SetStatus_SupplementaryTransformation(newItem, Parent);

                    Incoming.Quantity = smaller;
                    if (Incoming.Quantity < 1)
                    {
                        Delete(Incoming);
                    }
                    else
                    {
                        Incoming.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    }
                }
            }
        }
        public class MergeParts
        {
            public Item IncomingItem { get; private set; }
            public Item InteractiveItem { get; private set; }
            public bool IsPossible { get; private set; }

            private ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect;
            private (HashSet<(int Height, int Widht)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition;
            private (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data;
            public MergeParts(Item interactiveItem,Item incomingItem)
            {
                IncomingItem = incomingItem;
                InteractiveItem = interactiveItem;
                Data = InteractiveItem.PartPut_IsPossible(IncomingItem);

                IsPossible = Data.IsPossible;

                if (Data.IsPossible)
                {
                    List<Part> parts_ = new List<Part>()
                    {
                        IncomingItem.Parts.First()
                    };

                    IncomingItem.Parts.First().GetConnectedPartsTree(parts_);
                    Effect = AdvancedItem_SizeChanger_EffectDetermination(InteractiveItem, parts_, true);
                    NewPosition = Try_PartPositioning(InteractiveItem, Effect.ChangedSize, Effect.Directions);

                    IsPossible = NewPosition.IsPositionAble;
                }
            }
            public void Execute_MergeParts()
            {
                InteractiveItem.PartPut(IncomingItem, Data.SCP, Data.ICP);

                NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Height, NewPosition.NonLiveCoordinates.First().Widht, NewPosition.SectorIndex, InteractiveItem, InteractiveItem.ParentItem);

                NonLive_UnPlacing(InteractiveItem);
                NonLive_Placing(InteractiveItem, InteractiveItem.ParentItem);

                Live_UnPlacing(InteractiveItem);
                Live_Placing(InteractiveItem, InteractiveItem.ParentItem);

                InteractiveItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();

                if (InteractiveItem.ModificationWindowRef != null)
                {
                    InteractiveItem.ModificationWindowRef.ItemPartTrasformation();
                }
            }
        }
        public class RePlace
        {
            public RePlace(Item item, Item Parent,ItemSlot[] activeSlots)
            {
                this.item = item;
                this.PossibleNewParent = Parent;
                this.activeSlots = activeSlots.ToArray();
            }

            public Item PossibleNewParent{ get; private set; }
            public Item item { get; private set; }
            public ItemSlot[] activeSlots { get; private set; }

            public void Execute_RePlace()
            {
                Remove(item, item.ParentItem);
                Add(item, PossibleNewParent);

                NonLive_UnPlacing(item);
                Live_UnPlacing(item);

                Live_Positioning(item,activeSlots);

                NonLive_Placing(item, PossibleNewParent);
                Live_Placing(item, PossibleNewParent);

                HotKey_SetStatus_SupplementaryTransformation(item, PossibleNewParent);

                if (item.SelfGameobject != null)//a temporary objectum fix-je 
                {
                    item.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();

                    item.SelfGameobject.GetComponent<ItemObject>().BuildContainer();
                }
            }
        }
        #endregion

        #region Special
        public static void Placer(Item item, float originalRotation)
        {
            Action[] actions = item.AvaiablePlacerMetodes.ToArray();

            if (Input.GetKey(KeyCode.LeftControl) && actions.FirstOrDefault(action => action.Method.Name == nameof(Split.Execute_Split)) != null)//split
            {
                Debug.LogWarning("split");
                actions.First(action => action.Method.Name == nameof(Split.Execute_Split)).Invoke();
            }
            else if (actions.FirstOrDefault(action => action.Method.Name == nameof(Merge.Execute_Merge)) != null)//csak containerekben mukodik
            {
                Debug.LogWarning("Merge");
                actions.First(action => action.Method.Name == nameof(Merge.Execute_Merge)).Invoke();
            }
            else if (actions.FirstOrDefault(action => action.Method.Name == nameof(MergeParts.Execute_MergeParts)) != null)
            {
                Debug.LogWarning("MergeParts");
                actions.First(action => action.Method.Name == nameof(MergeParts.Execute_MergeParts)).Invoke();
            }
            else if (actions.FirstOrDefault(action => action.Method.Name == nameof(RePlace.Execute_RePlace)) != null)
            {
                Debug.LogWarning("RePlace");
                actions.First(action => action.Method.Name == nameof(RePlace.Execute_RePlace)).Invoke();
            }
            else
            {
                Debug.LogWarning("Return place");
                item.RotateDegree = originalRotation;

                item.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();

                item.SelfGameobject.GetComponent<ItemObject>().BuildContainer();
            }

            item.AvaiablePlacerMetodes.Clear();
            item.AvaiableParentItem = null;
        }
        public static void Delete(Item item)
        {
            //Debug.LogWarning($"Delete {item.ItemName}");

            UnsetHotKey(item);

            NonLive_UnPlacing(item);

            if (item.ParentItem != null)
            {
                Remove(item, item.ParentItem);
            }

            if (item.IsInPlayerInventory)
            {
                RemovePlayerInventory(item);
            }

            if (item.ItemSlotObjectsRef.FirstOrDefault() != null)
            {
                Live_UnPlacing(item);
                if (item.SelfGameobject != null)
                {
                    item.SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
                    GameObject.Destroy(item.SelfGameobject);
                }
            }
        }
        public static bool CanSplitable(Item Stand, Item Incoming)
        {
            if (Incoming.Quantity > 1 && Stand == null)
            {
                return true;
            }
            else if (Stand != Incoming && Incoming.Quantity > 1 && CanMergable(Stand, Incoming))
            {
                return true;
            }
            return false;
        }
        public static bool CanMergable(Item Stand, Item Incoming)
        {
            if (Stand != Incoming && Stand.MaxStackSize > 1 && Stand.ItemName == Incoming.ItemName)
            {
                return true;
            }
            return false;
        }
        public static void LiveCleaning(Item item)//ha az inventory-t bezarjuk akkor megsemisulnek a refernciak es egy nullokkal teli lista lesz, ez ezt hivatott orvosolni
        {
            item.ItemSlotObjectsRef.Clear();
        }
        #endregion

        #region Data Manipulation
        public static void Add(Item item, Item Parent)
        {
            item.ParentItem = Parent;
            Parent.Container.Items.Add(item);
            item.ContainerItemListRef = Parent.Container.Items;//set ref
        }
        public static void Remove(Item item, Item Parent)
        {
            item.ParentItem = null;
            Parent.Container.Items.Remove(item);
            item.ContainerItemListRef = null;//unset ref
        }

        public static void AddPlayerInventory(Item item)
        {
            item.IsInPlayerInventory = true;
            item.LevelManagerRef = InventoryObjectRef.GetComponent<PlayerInventory>().levelManager;//set ref
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Add(item);
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
        }
        public static void RemovePlayerInventory(Item item)
        {
            item.IsInPlayerInventory = false;
            item.LevelManagerRef.Items.Remove(item);
            item.LevelManagerRef.SetMaxLVL_And_Sort();
            item.LevelManagerRef = null;//unset ref
        }
        public static void PlayerInventoryLoad(ref LevelManager LoadFrom, ref LevelManager levelManager)
        {
            if (levelManager.Items.Count > 0)
            {
                List<Item> TemporaryItemList_ = new List<Item>(levelManager.Items);
                foreach (Item item in TemporaryItemList_)
                {
                    Delete(levelManager.Items.Find(i => i == item));
                }

                TemporaryItemList_.Clear();
                levelManager.Items.Clear();
            }

            levelManager = PlayerInventoryClone(LoadFrom);

            foreach (Item item in levelManager.Items)
            {
                if (item.hotKeyRef != null)
                {
                    item.hotKeyRef.SetHotKey(item);
                }
            }
        }
        public static void PlayerInventoryDefault(ref LevelManager levelManager)
        {
            List<Item> TemporaryItemList = new List<Item>(levelManager.Items);
            foreach (Item item in TemporaryItemList)
            {
                Delete(levelManager.Items.Find(i => i == item));
            }

            TemporaryItemList.Clear();
            levelManager.Items.Clear();

            Item RootData = new()
            {
                ItemName = "Root",
                Lvl = -1,
                SectorId = 0,
                Coordinates = new (int, int)[] { (0, 0) },
                IsRoot = true,
                IsEquipmentRoot = true,
                IsInPlayerInventory = true,
                Container = new Container("GameElements/PlayerInventory"),
                LevelManagerRef = levelManager,
            };

            levelManager.Items.Add(RootData);
            levelManager.SetMaxLVL_And_Sort();
        }
        public static void PlayerInventorySave(ref LevelManager SaveTo, ref LevelManager levelManager)
        {
            SaveTo = PlayerInventoryClone(levelManager);
        }
        #endregion

        #region Positioning
        public static void NonLive_Positioning(int Y, int X, int sectorIndex, Item item, Item Parent)
        {
            item.SectorId = sectorIndex;
            if (Parent.IsEquipmentRoot)
            {
                item.Coordinates = new[] { Parent.Container.NonLive_Sectors[sectorIndex][Y, X].Coordinate };//ez alapjan azonositunk egy itemslotot
            }
            else
            {
                List<(int, int)> coordiantes = new List<(int, int)>();
                if (item.RotateDegree == 90 || item.RotateDegree == 270)
                {
                    for (int y = Y; y < Y + item.SizeX; y++)//megforditjuk a koordinatakat mivel elforgazva van
                    {
                        for (int x = X; x < X + item.SizeY; x++)//megforditjuk a koordinatakat mivel elforgazva van
                        {
                            coordiantes.Add(Parent.Container.NonLive_Sectors[sectorIndex][y, x].Coordinate);//ez alapjan azonositunk egy itemslotot
                        }
                    }
                }
                else
                {
                    for (int y = Y; y < Y + item.SizeY; y++)
                    {
                        for (int x = X; x < X + item.SizeX; x++)
                        {
                            coordiantes.Add(Parent.Container.NonLive_Sectors[sectorIndex][y, x].Coordinate);//ez alapjan azonositunk egy itemslotot
                        }
                    }
                }
                item.Coordinates = coordiantes.ToArray();
            }

            item.ParentItem.Container.Items.Sort((a, b) => a.Coordinates.First().CompareTo(b.Coordinates.First()));

            SetHierarhicLVL(item, item.ParentItem);
        }
        public static void Live_Positioning(Item item, ItemSlot[] activeSlots)
        {
            List<(int, int)> coordiantes = new List<(int, int)>();
            for (int i = 0; i < activeSlots.Length; i++)
            {
                coordiantes.Add(activeSlots[i].GetComponent<ItemSlot>().Coordinate);
            }
            item.SectorId = activeSlots.First().sectorId;
            item.Coordinates = coordiantes.ToArray();

            item.ParentItem.Container.Items.Sort((a, b) => a.Coordinates.First().CompareTo(b.Coordinates.First()));

            SetHierarhicLVL(item, item.ParentItem);
        }

        public static ((int X,int Y) ChangedSize, Dictionary<char,int> Directions) AdvancedItem_SizeChanger_EffectDetermination(Item AdvancedItem,List<Part> IncomingParts,bool Add)
        {
            /*
             * meghaatrozza, hogy a megadott sizechanger az advanced itememet merre és menyivel mozgatja
             * 
             * visszadja az uj meretet és a valtozott directiont
             * 
             * ha a sizechangert tartalmazza akkor torli ha nem akkor hozzadja
             */
            if (Add)//add
            {
                //Debug.LogWarning($"DEtermination Add");
                Dictionary<char, int> Directions = new();

                (int X, int Y) ChangedSize = new(AdvancedItem.SizeX, AdvancedItem.SizeY);

                List<Part> ChangerParts = IncomingParts.Where(part => part.item_s_Part.SizeChanger != null).OrderBy(part => part.item_s_Part.SizeChanger.MaxPlus).ToList();

                foreach (Part part in ChangerParts)
                {
                    SizeChanger sizeChanger = part.item_s_Part.SizeChanger;

                    if (sizeChanger.Direction == "R" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('R'))
                            {
                                Directions['R'] += sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            else
                            {
                                Directions['R'] = sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                            if (Directions.ContainsKey('R'))
                            {
                                Directions['R'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['R'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "L" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('L'))
                            {
                                Directions['L'] += sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            else
                            {
                                Directions['L'] = sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                            if (Directions.ContainsKey('L'))
                            {
                                Directions['L'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['L'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "U" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('U'))
                            {
                                Directions['U'] += sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            else
                            {
                                Directions['U'] = sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            ChangedSize.Y = sizeChanger.MaxPlus;

                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                            if (Directions.ContainsKey('U'))
                            {
                                Directions['U'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['U'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "D" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('D'))
                            {
                                Directions['D'] += sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            else
                            {
                                Directions['D'] = sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            ChangedSize.Y = sizeChanger.MaxPlus;

                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                            if (Directions.ContainsKey('D'))
                            {
                                Directions['D'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['D'] = sizeChanger.Plus;
                            }
                        }
                    }
                }

                return (ChangedSize,Directions);
            }
            else//delete
            {
                //Debug.LogWarning($"DEtermination Remove");
                Dictionary<char, int> Directions = new();

                //az elso item szelessege és magassaga.
                //a partoka hierarhiai pontjukn szerint novekvo sorrendben vannak
                (int X, int Y) ChangedSize = new(AdvancedItem.Parts.First().item_s_Part.SizeX, AdvancedItem.Parts.First().item_s_Part.SizeY);

                //azok a partok amelyek nincsneek az incoming partokba és rendelkeznek sizechangerrel rendezve Maxplus szerint
                List<Part> StandParts = AdvancedItem.Parts.Where(part=> !IncomingParts.Contains(part) && part.item_s_Part.SizeChanger != null).OrderBy(part=>part.item_s_Part.SizeChanger.MaxPlus).ToList();

                //azon incoming partok ameyleknek van sizechanger és rendezve vannak maxplus szeirtn
                List<Part> ChangerParts = IncomingParts.Where(part => part.item_s_Part.SizeChanger != null).OrderBy(part => part.item_s_Part.SizeChanger.MaxPlus).ToList();

                //vegig megyunk azokon a partokaon amelyek maradnak és nem tavolitodnak el tovabba rendelkeznek sizechangerrel
                //tájolás szerint biraljuk el sizechangerjuket
                //minden sizechanger csak annyit novelhet amekkorat maximum-ja enged.
                foreach (Part part in StandParts)
                {
                    SizeChanger sizeChanger = part.item_s_Part.SizeChanger;

                    if (sizeChanger.Direction == "R" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                        }
                    }
                    else if (sizeChanger.Direction == "L" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                        }
                    }
                    else if (sizeChanger.Direction == "U" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            ChangedSize.Y = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                        }
                    }
                    else if (sizeChanger.Direction == "D" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            ChangedSize.Y = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                        }
                    }
                }

                //vegig megyunk az osszes a changer parton
                //tájolás szeritn biráljuk el őket
                //megnezzuk menyit novelne miden egyes cizechanger
                //ezeket listazzuk tájolás szeirnt
                foreach (Part part in ChangerParts)
                {
                    SizeChanger sizeChanger = part.item_s_Part.SizeChanger;

                    if (sizeChanger.Direction == "R" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('R'))
                            {
                                Directions['R'] += sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            else
                            {
                                Directions['R'] = sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                            if (Directions.ContainsKey('R'))
                            {
                                Directions['R'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['R'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "L" && ChangedSize.X < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.X) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('L'))
                            {
                                Directions['L'] += sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            else
                            {
                                Directions['L'] = sizeChanger.MaxPlus - ChangedSize.X;
                            }
                            ChangedSize.X = sizeChanger.MaxPlus;
                        }
                        else
                        {
                            ChangedSize.X += sizeChanger.Plus;
                            if (Directions.ContainsKey('L'))
                            {
                                Directions['L'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['L'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "U" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('U'))
                            {
                                Directions['U'] += sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            else
                            {
                                Directions['U'] = sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            ChangedSize.Y = sizeChanger.MaxPlus;

                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                            if (Directions.ContainsKey('U'))
                            {
                                Directions['U'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['U'] = sizeChanger.Plus;
                            }
                        }
                    }
                    else if (sizeChanger.Direction == "D" && ChangedSize.Y < sizeChanger.MaxPlus)
                    {
                        if ((sizeChanger.Plus + ChangedSize.Y) > sizeChanger.MaxPlus)
                        {
                            if (Directions.ContainsKey('D'))
                            {
                                Directions['D'] += sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            else
                            {
                                Directions['D'] = sizeChanger.MaxPlus - ChangedSize.Y;
                            }
                            ChangedSize.Y = sizeChanger.MaxPlus;

                        }
                        else
                        {
                            ChangedSize.Y += sizeChanger.Plus;
                            if (Directions.ContainsKey('D'))
                            {
                                Directions['D'] += sizeChanger.Plus;
                            }
                            else
                            {
                                Directions['D'] = sizeChanger.Plus;
                            }
                        }
                    }
                }

                //fix helye
                //vegigmeygunk az osszes parton ami marad
                //elofordulhat hogy egy partnak nincs sizechangerja, de a part nagyobb mint a sizechangerek altal meghatarozott meret
                //ilynekor vissza kell korrigálni.
                //amelyik directionba változas tortent ott vissza korrigáhato a size
                foreach (Part part in StandParts)
                {
                    if (part.item_s_Part.SizeX > ChangedSize.X)
                    {
                        int Correction = part.item_s_Part.SizeX - ChangedSize.X;
                        if (Directions.ContainsKey('R') && Directions['R'] > 0)
                        {
                            Directions['R'] -= Correction;
                            if (Directions['R'] <= 0)
                            {
                                Correction = Directions['R'] * (-1);
                                ChangedSize.X += Correction;
                                Directions.Remove('R');
                            }
                            else
                            {
                                ChangedSize.X += Correction;
                            }
                        }
                    }

                    if (part.item_s_Part.SizeX > ChangedSize.X)
                    {
                        int Correction = part.item_s_Part.SizeX - ChangedSize.X;
                        if (Directions.ContainsKey('L') && Directions['L'] > 0)
                        {
                            Directions['L'] -= Correction;
                            if (Directions['L'] <= 0)
                            {
                                Correction = Directions['L'] * (-1);
                                ChangedSize.X += Correction;
                                Directions.Remove('L');
                            }
                            else
                            {
                                ChangedSize.X += Correction;
                            }
                        }
                    }

                    if (part.item_s_Part.SizeY > ChangedSize.Y)
                    {
                        int Correction = part.item_s_Part.SizeY - ChangedSize.Y;
                        if (Directions.ContainsKey('U') && Directions['U'] > 0)
                        {
                            Directions['U'] -= Correction;
                            if (Directions['U'] <= 0)
                            {
                                Correction = Directions['U'] * (-1);
                                ChangedSize.Y += Correction;
                                Directions.Remove('U');
                            }
                        }
                        else
                        {
                            ChangedSize.Y += Correction;
                        }
                    }

                    if (part.item_s_Part.SizeY > ChangedSize.Y)
                    {
                        int Correction = part.item_s_Part.SizeY - ChangedSize.Y;
                        if (Directions.ContainsKey('D') && Directions['D'] > 0)
                        {
                            Directions['D'] -= Correction;
                            if (Directions['D'] <= 0)
                            {
                                Correction = Directions['D'] * (-1);
                                ChangedSize.Y += Correction;
                                Directions.Remove('D');
                            }
                        }
                        else
                        {
                            ChangedSize.Y += Correction;
                        }
                    }

                }

                foreach (var key in Directions.Keys.ToList()) // Másolat készítés, hogy ne módosítsunk közvetlen iteráció közben
                {
                    Directions[key] *= -1;
                }

                //Debug.LogWarning($" Determination: changedSize:{ChangedSize} ------Directions:------");
                //foreach (KeyValuePair<char,int> item in Directions)
                //{
                //    Debug.LogWarning($"key: {item.Key}    value: {item.Value}");
                //}
                //Debug.LogWarning("---------------------------");

                return (ChangedSize, Directions);
            }
        }
        public static (HashSet<(int Height, int Widht)> NonLiveCoordinates,int SectorIndex, bool IsPositionAble) Try_PartPositioning(Item AdvancedItem, (int X, int Y) ChangedSize, Dictionary<char, int> Directions)
        {
            /*
             * egy megvaltoztataott referncia meretbol meghatarozza hogy az advanced itemet kicsinyiteni kell e vagy nagyobbitani
             * a directionbol meghatarozza hogy melyik iranyba novekszik vagy csokken
             * 
             * visszatereskor a vegezetul lefogalat coordianatak adja vissza és azt hogy a koordinatakra valo athelyezes lehetseges e
             */
            if (Directions != null)
            {
                bool IsPositionAble = true;
                int sectorindex = 0;
                HashSet<(int Height, int Width)> ExtendCoordinates = new();

                //sectorIndex keresese
                int index = 0;
                foreach (ItemSlotData[,] sector in AdvancedItem.ParentItem.Container.NonLive_Sectors)
                {
                    foreach (ItemSlotData slot in sector)
                    {
                        if (slot.PartOfItemData == AdvancedItem)
                        {
                            sectorindex = index;
                        }
                    }
                    index++;
                }

                ItemSlotData[,] NonLiveGrid = AdvancedItem.ParentItem.Container.NonLive_Sectors[sectorindex];
                //itemcoordinata grid létrehozasa forgatas szerint
                (int X, int Y)[,] ItemCoordinates;

                if (AdvancedItem.RotateDegree == 90 || AdvancedItem.RotateDegree == 270)
                {
                    ItemCoordinates = new (int X, int Y)[AdvancedItem.SizeX, AdvancedItem.SizeY];
                }
                else
                {
                    ItemCoordinates = new (int X, int Y)[AdvancedItem.SizeY, AdvancedItem.SizeX];
                }

                //Debug.LogWarning($" X: {ItemCoordinates.GetLength(0)}                ----- Itemo coordinate grid ------             Y: {ItemCoordinates.GetLength(1)}");
                //item coordinátáinak megkeresese a NonLive Gridben
                //Debug.LogWarning($"------------------");
                for (int Height = 0; Height < NonLiveGrid.GetLength(0); Height++)
                {
                    for (int Width = 0; Width < NonLiveGrid.GetLength(1); Width++)
                    {
                        if (NonLiveGrid[Height, Width].PartOfItemData == AdvancedItem)
                        {
                            ExtendCoordinates.Add((Height, Width));
                            //Debug.LogWarning($"height: {Height}  witdh: {Width}");
                        }
                    }
                }
                //Debug.LogWarning($"------------------");
                //item megkeresett koordinatainak beépítése annak gridjébe
                for (int height = 0, index_ = 0; height < ItemCoordinates.GetLength(0); height++)
                {
                    for (int width = 0; width < ItemCoordinates.GetLength(1); width++)
                    {
                        ItemCoordinates[height, width] = ExtendCoordinates.ElementAt(index_++);
                    }
                }

                foreach (KeyValuePair<char, int> direction in Directions)
                {
                    if (direction.Key == 'U')
                    {
                        //Debug.LogWarning($"{direction.Key} Up Way  {direction.Value}");
                        IsPositionAble = Try_UpWayScaling(AdvancedItem, NonLiveGrid, direction.Value, ExtendCoordinates);
                    }
                    else if (direction.Key == 'D')
                    {
                        //Debug.LogWarning($"{direction.Key} Down Way  {direction.Value}");
                        IsPositionAble = Try_DownWayScaling(AdvancedItem, NonLiveGrid, direction.Value, ExtendCoordinates);
                    }
                    else if (direction.Key == 'L')
                    {
                        //Debug.LogWarning($"{direction.Key} Left Way  {direction.Value}");
                        IsPositionAble = Try_LeftWayScaling(AdvancedItem, NonLiveGrid, direction.Value, ExtendCoordinates);
                    }
                    else if (direction.Key == 'R')
                    {
                        //Debug.LogWarning($"{direction.Key} Right Way  {direction.Value}");
                        IsPositionAble = Try_RightWayScaling(AdvancedItem, NonLiveGrid, direction.Value, ExtendCoordinates);
                    }
                }

                ExtendCoordinates = new HashSet<(int Height, int Width)>(ExtendCoordinates.OrderBy(coord => coord.Height).ThenBy(coord => coord.Width).ToList());

                //Debug.LogWarning($"Placememnt coodinate X: {ExtendCoordinates.First().Height}    first y: {ExtendCoordinates.First().Width}");

                return (ExtendCoordinates, sectorindex, IsPositionAble);
            }
            else
            {
                return (null, 0, false);
            }
           
        }
        #endregion

        #region Placing
        public static void Live_Placing(Item item, Item PlacingInto)
        {
            foreach (ItemSlot[,] sector in PlacingInto.Container.Live_Sector)
            {
                if (item.SectorId == sector[0,0].sectorId)
                {
                    foreach (ItemSlot slot in sector)
                    {
                        if (item.Coordinates.Contains(slot.Coordinate))
                        {
                            slot.PartOfItemObject = item.SelfGameobject;
                            item.ItemSlotObjectsRef.Add(slot);//set ref
                        }
                    }
                }
            }
        }
        public static void Live_UnPlacing(Item item)
        {
            foreach (ItemSlot slotObject in item.ItemSlotObjectsRef)
            {
                slotObject.PartOfItemObject = null;
            }
            item.ItemSlotObjectsRef.Clear();//unset ref
        }

        public static void NonLive_Placing(Item item, Item AddTo)
        {
            foreach (ItemSlotData[,] sector in AddTo.Container.NonLive_Sectors)
            {
                if (item.SectorId == sector[0, 0].SectorID)
                {
                    foreach (ItemSlotData slot in sector)
                    {
                        if (item.Coordinates.Contains(slot.Coordinate))
                        {
                            slot.PartOfItemData = item;
                            item.ItemSlotsDataRef.Add(slot);//set ref
                        }
                    }
                }
            }
        }
        public static void NonLive_UnPlacing(Item item)
        {
            foreach (ItemSlotData slotData in item.ItemSlotsDataRef)
            {
                slotData.PartOfItemData = null;//remove
            }
            item.ItemSlotsDataRef.Clear();//unset ref
        }
        #endregion

        #region Status
        public static void HotKey_SetStatus_SupplementaryTransformation(Item item, Item StatusParent)
        {
            #region Hotkey
            if (item.hotKeyRef != null)
            {
                if ((item.IsEquipment && !StatusParent.IsEquipmentRoot) ||//ha equipmentből inventoryba kerul
                    (!StatusParent.IsInPlayerInventory) ||//ha inventoryn kivulre kerul
                    (!item.IsEquipment && StatusParent.IsEquipmentRoot) ||//ha iventorybol equipmentbe kerul
                    (item.IsEquipment && StatusParent.IsEquipmentRoot)//ha equipmentbol equipmentbe
                    )
                {
                    item.hotKeyRef.UnSetHotKey();
                }
            }
            if (StatusParent.IsEquipmentRoot)
            {
                AutoSetHotKey(item);
            }
            #endregion

            #region Status
            if (!item.IsEquipment && StatusParent.IsEquipmentRoot)
            {
                item.IsEquipment = true;
            }
            else if (item.IsEquipment && !StatusParent.IsEquipmentRoot)
            {
                item.IsEquipment = false;
            }

            if (!item.IsInPlayerInventory && StatusParent.IsInPlayerInventory)
            {
                Debug.LogWarning($"{item.ItemName}   add player inventory");
                AddPlayerInventory(item);
            }
            else if (item.IsInPlayerInventory && !StatusParent.IsInPlayerInventory)
            {
                Debug.LogWarning($"{item.ItemName}   remove player inventory");
                RemovePlayerInventory(item);
            }

            StatusIsInPlayerInventory(item);
            #endregion

            #region Supplementary Transformations
            if (item.IsEquipment)
            {
                item.RotateDegree = 0;
            }
            #endregion
        }
        public static void UnsetHotKey(Item item)
        {
            if (item.hotKeyRef != null)
            {
                item.hotKeyRef.UnSetHotKey();
            }
        }
        #endregion

        #region Inventory-System Support Scripts
        private static (int X, int Y)[] Get_ItemCoodinateLine_AtDataGrid(Item AdvancedItem, HashSet<(int X, int Y)> ExtendCoordinates,char Orientation)
        {
            if (Orientation == 'U')
            {
                if (AdvancedItem.RotateDegree == 90)
                {
                    int MinY = ExtendCoordinates.Min(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MinY).OrderBy(Item => Item.X).ToArray();
                }
                else if(AdvancedItem.RotateDegree == 0)
                {
                    int MinX = ExtendCoordinates.Min(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MinX).OrderBy(Item => Item.Y).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 270)
                {
                    int MaxY = ExtendCoordinates.Max(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MaxY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 180)
                {
                    int MaxX = ExtendCoordinates.Max(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MaxX).OrderBy(Item => Item.Y).ToArray();
                }
                else
                {
                    return null;
                }
            }
            else if (Orientation == 'D')
            {
                if (AdvancedItem.RotateDegree == 0)
                {
                    int MaxX = ExtendCoordinates.Max(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MaxX).OrderBy(Item => Item.Y).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 90)
                {
                    int MaxY = ExtendCoordinates.Max(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MaxY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 180)
                {
                    int MinX = ExtendCoordinates.Min(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MinX).OrderBy(Item => Item.Y).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 270)
                {
                    int MinY = ExtendCoordinates.Min(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MinY).OrderBy(Item => Item.X).ToArray();
                }
                else
                {
                    return null;
                }
            }
            else if (Orientation == 'R')
            {
                if (AdvancedItem.RotateDegree == 0)
                {
                    int MaxY = ExtendCoordinates.Max(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MaxY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 90)
                {
                    int MinX = ExtendCoordinates.Min(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MinX).OrderBy(Item => Item.Y).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 180)
                {
                    int MinY = ExtendCoordinates.Min(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MinY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 270)
                {
                    int MaxX = ExtendCoordinates.Max(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MaxX).OrderBy(Item => Item.Y).ToArray();
                }
                else
                {
                    return null;
                }
            }
            else if (Orientation == 'L')
            {
                if (AdvancedItem.RotateDegree == 0)
                {
                    int MinY = ExtendCoordinates.Min(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MinY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 90)
                {
                    int MaxX = ExtendCoordinates.Max(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MaxX).OrderBy(Item => Item.Y).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 180)
                {
                    int MaxY = ExtendCoordinates.Max(coordiante => coordiante.Y);

                    return ExtendCoordinates.Where(item => item.Y == MaxY).OrderBy(Item => Item.X).ToArray();
                }
                else if (AdvancedItem.RotateDegree == 270)
                {
                    int MinX = ExtendCoordinates.Max(coordiante => coordiante.X);

                    return ExtendCoordinates.Where(item => item.X == MinX).OrderBy(Item => Item.Y).ToArray();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private static bool Try_UpWayScaling(Item AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
        {
            (int Height, int Width)[] ActualDownLine = null;
            int Value = 0;//a kezdo ertek
            bool AllCoordianateIsEmpty = true;

            //------------------------------
            int GridStop = 0;//az az ertek ameddig az iterációt nem kell leállitani
            int ChangerValue = 0;//az ertek amit a value hez hozzadunk minden iterácionál
            char Orientation = 'U';
            bool HorizontalWay = false;
            //------------------------------

            //meg kell hatarozni hogy melyik iranyba és hogyan kell indexelni és meddig.    
            //
            //az actual line az item által elfogfalat azon koordinatak melyek az itemhez képest vannal tajolva
            //
            //mivel nekunk a NonLive DataGrid ben lévő tájolás kell ezert néhol máshogy bíráljuk el
            if (AdvancedItem.RotateDegree == 0)
            {
                GridStop = -1;//_/  azert -1 mert ha nulla lenne akkor a legfelso slot ot minden kihagyna, de ez nem veszelyeztet a getLeight-nel mert az alapbol 1 el nagyobb mint az index
                ChangerValue = -1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 90)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 180)
            {
                GridStop = NonLiveGrid.GetLength(0);//_/
                ChangerValue = 1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 270)
            {
                GridStop = NonLiveGrid.GetLength(1);//_/
                ChangerValue = 1;//_/
                HorizontalWay = true;//_/
            }

            if (ChangedSize < 0)
            {
                ChangerValue *= -1;
            }

            ActualDownLine = Get_ItemCoodinateLine_AtDataGrid(AdvancedItem, ExtendCoordinates, Orientation);

            //itt zajik a koordinatak hozzadasa vagy eltavolitasa
            if (ChangedSize > 0)
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int Coord = 0; Coord < ActualDownLine.Length; Coord++)
                        {
                            if (ActualDownLine[Coord].Width + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            if (ActualDownLine[j].Height + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[j].Height + Value, ActualDownLine[j].Width ));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[j].Height + Value, ActualDownLine[j].Width].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height }  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height, ActualDownLine[j].Width + Value));
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                        }
                    }
                }
            }

            return AllCoordianateIsEmpty;
        }
        public static bool Try_DownWayScaling(Item AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
        {
            (int Height, int Width)[] ActualDownLine = null;
            int Value = 0;//a kezdo ertek
            bool AllCoordianateIsEmpty = true;

            //------------------------------
            int GridStop = 0;//az az ertek ameddig az iterációt nem kell leállitani
            int ChangerValue = 0;//az ertek amit a value hez hozzadunk minden iterácionál
            char Orientation = 'D';
            bool HorizontalWay = false;
            //------------------------------

            //meg kell hatarozni hogy melyik iranyba és hogyan kell indexelni és meddig.    
            //
            //az actual line az item által elfogfalat azon koordinatak melyek az itemhez képest vannal tajolva
            //
            //mivel nekunk a NonLive DataGrid ben lévő tájolás kell ezert néhol máshogy bíráljuk el
            if (AdvancedItem.RotateDegree == 0)
            {
                GridStop = NonLiveGrid.GetLength(0);//_/
                ChangerValue = 1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 90)
            {
                GridStop = NonLiveGrid.GetLength(1);//_/
                ChangerValue = 1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 180)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 270)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = true;//_/
            }

            if (ChangedSize < 0)
            {
                ChangerValue *= -1;
            }

            ActualDownLine = Get_ItemCoodinateLine_AtDataGrid(AdvancedItem, ExtendCoordinates, Orientation);

            //itt zajik a koordinatak hozzadasa vagy eltavolitasa
            if (ChangedSize > 0)
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int Coord = 0; Coord < ActualDownLine.Length; Coord++)
                        {
                            if (ActualDownLine[Coord].Width + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            if (ActualDownLine[j].Height + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[j].Height + Value, ActualDownLine[j].Width].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height, ActualDownLine[j].Width + Value));
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                        }
                    }
                }
            }

            return AllCoordianateIsEmpty;
        }
        private static bool Try_RightWayScaling(Item AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
        {
            (int Height, int Width)[] ActualDownLine = null;
            int Value = 0;//a kezdo ertek
            bool AllCoordianateIsEmpty = true;

            //------------------------------
            int GridStop = 0;//az az ertek ameddig az iterációt nem kell leállitani
            int ChangerValue = 0;//az ertek amit a value hez hozzadunk minden iterácionál
            char Orientation = 'R';
            bool HorizontalWay = false;
            //------------------------------

            //meg kell hatarozni hogy melyik iranyba és hogyan kell indexelni és meddig.    
            //
            //az actual line az item által elfogfalat azon koordinatak melyek az itemhez képest vannal tajolva
            //
            //mivel nekunk a NonLive DataGrid ben lévő tájolás kell ezert néhol máshogy bíráljuk el
            if (AdvancedItem.RotateDegree == 0)
            {
                GridStop = NonLiveGrid.GetLength(1);//_/
                ChangerValue = 1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 90)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 180)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 270)
            {
                GridStop = NonLiveGrid.GetLength(0);//_/
                ChangerValue = 1;//_/
                HorizontalWay = false;//_/
            }

            if (ChangedSize < 0)
            {
                ChangerValue *= -1;
            }

            ActualDownLine = Get_ItemCoodinateLine_AtDataGrid(AdvancedItem, ExtendCoordinates, Orientation);

            //Debug.LogWarning($" Line hossz: {ActualDownLine.Length}     ChangerValue: {ChangerValue}     Item.Rptationdegree: {AdvancedItem.RotateDegree}");

            //itt zajik a koordinatak hozzadasa vagy eltavolitasa
            if (ChangedSize > 0)
            {
                if (HorizontalWay)
                {
                    //Debug.LogWarning($"Vertical, iteralhato");
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int Coord = 0; Coord < ActualDownLine.Length; Coord++)
                        {
                            if (ActualDownLine[Coord].Width + Value != GridStop)
                            {
                                //Debug.LogWarning($"H:  {ActualDownLine[Coord].Height}   W: {ActualDownLine[Coord].Width + Value}");
                                ExtendCoordinates.Add((ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                    //Debug.LogWarning($"------------------------");
                }
                else
                {
                    //Debug.LogWarning($"Vertical, iteralhato");
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {

                        for (int Coord = 0; Coord < ActualDownLine.Length; Coord++)
                        {
                            if (ActualDownLine[Coord].Height + Value != GridStop)
                            {
                                //Debug.LogWarning($"H:  {ActualDownLine[Coord].Height + Value}   W: {ActualDownLine[Coord].Width}");
                                ExtendCoordinates.Add((ActualDownLine[Coord].Height + Value, ActualDownLine[Coord].Width));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[Coord].Height + Value, ActualDownLine[Coord].Width].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                    //Debug.LogWarning($"------------------------");
                }
            }
            else
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height, ActualDownLine[j].Width + Value));
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                        }
                    }
                }
            }

            return AllCoordianateIsEmpty;
        }
        private static bool Try_LeftWayScaling(Item AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
        {
            (int Height, int Width)[] ActualDownLine = null;
            int Value = 0;//a kezdo ertek
            bool AllCoordianateIsEmpty = true;

            //------------------------------
            int GridStop = 0;//az az ertek ameddig az iterációt nem kell leállitani
            int ChangerValue = 0;//az ertek amit a value hez hozzadunk minden iterácionál
            char Orientation = 'L';
            bool HorizontalWay = false;
            //------------------------------

            //meg kell hatarozni hogy melyik iranyba és hogyan kell indexelni és meddig.    
            //
            //az actual line az item által elfogfalat azon koordinatak melyek az itemhez képest vannal tajolva
            //
            //mivel nekunk a NonLive DataGrid ben lévő tájolás kell ezert néhol máshogy bíráljuk el
            if (AdvancedItem.RotateDegree == 0)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 90)
            {
                GridStop = NonLiveGrid.GetLength(0);//_/
                ChangerValue = 1;//_/
                HorizontalWay = false;//_/
            }
            else if (AdvancedItem.RotateDegree == 180)
            {
                GridStop = NonLiveGrid.GetLength(1);//_/
                ChangerValue = 1;//_/
                HorizontalWay = true;//_/
            }
            else if (AdvancedItem.RotateDegree == 270)
            {
                GridStop = -1;//_/
                ChangerValue = -1;//_/
                HorizontalWay = false;//_/
            }

            if (ChangedSize < 0)
            {
                ChangerValue *= -1;
            }

            ActualDownLine = Get_ItemCoodinateLine_AtDataGrid(AdvancedItem, ExtendCoordinates, Orientation);

            //itt zajik a koordinatak hozzadasa vagy eltavolitasa
            if (ChangedSize > 0)
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int Coord = 0; Coord < ActualDownLine.Length; Coord++)
                        {
                            if (ActualDownLine[Coord].Width + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[Coord].Height, ActualDownLine[Coord].Width + Value].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) <= Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            if (ActualDownLine[j].Height + Value != GridStop)
                            {
                                ExtendCoordinates.Add((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                            }
                            if (Value > 0 && NonLiveGrid[ActualDownLine[j].Height + Value, ActualDownLine[j].Width].PartOfItemData != null)
                            {
                                AllCoordianateIsEmpty = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (HorizontalWay)
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height, ActualDownLine[j].Width + Value));
                        }
                    }
                }
                else
                {
                    for (; Math.Abs(Value) < Math.Abs(ChangedSize); Value += ChangerValue)
                    {
                        for (int j = 0; j < ActualDownLine.Length; j++)
                        {
                            //Debug.LogWarning($"Deleted Horzontal coodinate: Y: {ActualDownLine[j].Height}  X: {ActualDownLine[j].Width + Value}");
                            ExtendCoordinates.Remove((ActualDownLine[j].Height + Value, ActualDownLine[j].Width));
                        }
                    }
                }
            }

            return AllCoordianateIsEmpty;
        }

        private static void StatusIsInPlayerInventory(Item Data)
        {
            if (Data.Container != null)
            {
                foreach (Item item in Data.Container.Items)
                {
                    if (!item.IsInPlayerInventory && Data.IsInPlayerInventory)
                    {
                        AddPlayerInventory(item);
                    }
                    else if (item.IsInPlayerInventory && !Data.IsInPlayerInventory)
                    {
                        RemovePlayerInventory(item);
                    }
                    if (item.SelfGameobject != null)
                    {
                        item.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                        item.SelfGameobject.GetComponent<ItemObject>().BuildContainer();
                    }
                    SetHierarhicLVL(item, Data);
                    StatusIsInPlayerInventory(item);
                }
            }
        }
        private static void SetHierarhicLVL(Item item, Item Parent)
        {
            int lvl = Parent.Lvl;
            item.Lvl = ++lvl;
        }
        private static void AutoSetHotKey(Item SetIn)
        {
            switch (SetIn.Coordinates.First())
            {
                case (0,10):
                    InGameUI.HotKey1.SetHotKey(SetIn);
                    break;
                case (0, 11):
                    InGameUI.HotKey2.SetHotKey(SetIn);
                    break;
                case (0, 12):
                    InGameUI.HotKey3.SetHotKey(SetIn);
                    break;
                case (0, 13):
                    InGameUI.HotKey4.SetHotKey(SetIn);
                    break;
                default:
                    break;
            }
        }
        private static (int smaller, int larger) SplitInteger(int number)
        {
            int half = number / 2;
            if (number % 2 == 0)
            {
                return (half, half);
            }
            else
            {
                return (half, half + 1);
            }
        }

        private static LevelManager PlayerInventoryClone(LevelManager original)
        {
            LevelManager clone = new()
            {
                Items = new List<Item>()
            };

            // 1. Klónozzuk az összes itemet, és beállítjuk a LevelManager referenciát.
            foreach (Item item in original.Items)
            {
                Item clonedItem = item.ShallowClone();
                clonedItem.LevelManagerRef = clone;

                // Ha fejlett (advanced) itemről van szó, klónozzuk a részeit (Part-eket).
                if (clonedItem.IsAdvancedItem)
                {
                    CloneParts(item, clonedItem);
                }

                clone.Items.Add(clonedItem);
            }

            // 2. Beállítjuk a kapcsolódó referenciákat az itemek között.
            // Feltételezzük, hogy az első item (index 0) a root item, ezért az i=1-től indulunk.
            for (int i = 1; i < clone.Items.Count; i++)
            {
                SetupParentReference(original, clone, i);
                SetupContainerReferences(original, clone, i);
                SetupHotKeyReference(original, clone, i);

                if (original.Items[i].IsAdvancedItem)
                {
                    SetupConnectionPoints(original, clone, i);
                }
            }

            return clone;
        }
        private static void CloneParts(Item originalItem, Item clonedItem)
        {
            clonedItem.Parts = new List<Part>();
            foreach (var partRef in originalItem.Parts)
            {
                // Klónozzuk a part-hoz tartozó itemet, majd létrehozunk egy új Part példányt.
                Item clonedPartItem = partRef.item_s_Part.ShallowClone();
                clonedItem.Parts.Add(new Part(clonedPartItem));
                clonedItem.Parts.Last().HierarhicPlace = partRef.HierarhicPlace;
            }
        }
        private static void SetupParentReference(LevelManager original, LevelManager clone, int index)
        {
            // Megkeressük az eredeti item parentjának indexét, majd a klónban beállítjuk a referenciát.
            Item originalItem = original.Items[index];
            int parentIndex = original.Items.IndexOf(originalItem.ParentItem);
            clone.Items[index].ParentItem = clone.Items[parentIndex];
        }
        private static void SetupContainerReferences(LevelManager original, LevelManager clone, int index)
        {
            // Beállítjuk a container listát és a grid referenciákat.
            Item clonedItem = clone.Items[index];
            Item parent = clonedItem.ParentItem;

            // Hozzáadjuk a klón itemet a parent container listájához, majd tároljuk a referenciát.
            parent.Container.Items.Add(clonedItem);
            clonedItem.ContainerItemListRef = parent.Container.Items;

            // A koordináták alapján frissítjük a container grid-et.
            foreach ((int h, int w) coord in clonedItem.Coordinates)
            {
                // A parent container NonLive_Sectors tömbében beállítjuk, hogy az adott cella tartalmazza a klón itemet.
                parent.Container.NonLive_Sectors[clonedItem.SectorId][coord.h, coord.w].PartOfItemData = clonedItem;
                clonedItem.ItemSlotsDataRef.Add(parent.Container.NonLive_Sectors[clonedItem.SectorId][coord.h, coord.w]);
            }
        }
        private static void SetupHotKeyReference(LevelManager original, LevelManager clone, int index)
        {
            // Ha az eredeti itemnek volt hotKey referenciája, azt átmásoljuk.
            if (original.Items[index].hotKeyRef != null)
            {
                clone.Items[index].hotKeyRef = original.Items[index].hotKeyRef;
            }
        }
        private static void SetupConnectionPoints(LevelManager original, LevelManager clone, int index)
        {
            // Fejlett itemek esetén végigiterálunk a partok connection pointjain,
            // és újracsatlakoztatjuk őket a megfelelő kapcsolatokat létrehozva az eredeti kapcsolatok alapján.
            Item clonedItem = clone.Items[index];
            Item originalItem = original.Items[index];

            for (int j = 0; j < originalItem.Parts.Count; j++)
            {
                var originalPart = originalItem.Parts[j];
                var clonedPart = clonedItem.Parts[j];

                for (int k = 0; k < originalPart.ConnectionPoints.Length; k++)
                {
                    var cp = originalPart.ConnectionPoints[k];
                    if (cp.Used)
                    {
                        // Megkeressük az eredeti kapcsolódó partot, majd a connection point indexet.
                        Part usedPart = cp.ConnectedPoint.SelfPart;
                        int itemIndex = original.Items.IndexOf(original.Items.Find(item => item.IsAdvancedItem && item.Parts.Contains(usedPart)));
                        int partIndex = original.Items[itemIndex].Parts.IndexOf(usedPart);
                        int cpIndex = Array.IndexOf(original.Items[itemIndex].Parts[partIndex].ConnectionPoints, cp.ConnectedPoint);

                        // Újracsatlakoztatjuk a klónban a connection pointokat.
                        clonedPart.ConnectionPoints[k].Connect(clone.Items[itemIndex].Parts[partIndex].ConnectionPoints[cpIndex]);
                    }
                }
            }
        }
        #endregion
    }
}