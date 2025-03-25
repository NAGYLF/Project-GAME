using System;
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
using static MainData.Main;
using UnityEngine.UI;
using Assets.Scripts.Inventory;
using DataHandler;
using System.Collections;
using MainData;

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

        #region Ref Variables

        public ModificationWindow ModificationWindowRef;
        public LevelManager LevelManagerRef;
        public List<ItemSlotData> ItemSlotsDataRef = new List<ItemSlotData>();
        public List<Item> ContainerItemListRef = new List<Item>();
        public HotKey hotKeyRef;
        public List<ItemSlot> ItemSlotObjectsRef = new List<ItemSlot>();
        public CharacterHand PlayerHandRef;
        public GameObject SelfGameobject { get; set; }
        public GameObject InGameSelfObject { get; set; }
        public Item ParentItem { get; set; }//az az item ami tárolja ezt az itemet
        #endregion

        #region PlacerVariables
        public List<Action> AvaiablePlacerMetodes = new List<Action>();
        public Item AvaiableParentItem { get; set; }
        #endregion

        #region System Variables
        public int Lvl { get; set; }
        public string HotKey { get; set; } = "";
        public float RotateDegree { get; set; } = 0f;
        public int SectorId { get; set; }
        private (int, int)[] coordinates;
        public (int, int)[] Coordinates
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
        public SizeChanger SizeChanger { get; set; }
        #endregion

        #region Status Flags
        public bool IsInPlayerInventory { get; set; } = false;// a player inventory tagja az item
        public bool IsEquipment { set; get; } = false;// az item egy equipment
        public bool IsLoot { set; get; } = false;// az item a loot conténerekben van
        public bool IsRoot { set; get; } = false;// az item egy root data
        public bool IsEquipmentRoot { set; get; } = false;// az item a player equipmentjeinek rootja ebbol csak egy lehet
        #endregion

        #region Action Flags
        public bool IsDropAble { get; set; } = false;
        public bool IsRemoveAble { get; set; } = true;
        public bool IsUnloadAble { get; set; } = false;
        public bool IsModificationAble { get; set; } = false;
        public bool IsOpenAble { get; set; } = false;
        public bool IsUsable { get; set; } = false;
        public bool CanReload { get; set; } = false;
        #endregion

        #region Contaimnets
        public List<Part> Parts { get; set; }//az item darabjai
        public Container Container { get; set; }
        #endregion

        #region General Variables
        public string ItemType { get; set; }
        public string SystemName { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int MaxStackSize { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        #endregion

        #region NonGeneric Weapon Variables
        public int MagasineSize { get; set; }
        public double Spread { get; set; }
        public int Fpm { get; set; }
        public double Recoil { get; set; }
        public double Accturacy { get; set; }
        public double Range { get; set; }
        public double Ergonomy { get; set; }
        public string Caliber { get; set; }
        #endregion

        #region NonGeneric UseAble Items Variables
        public int UseLeft { get; set; } = 0;
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
                // System variables
                Lvl = this.Lvl,
                HotKey = this.HotKey,
                RotateDegree = this.RotateDegree,
                SectorId = this.SectorId,
                SizeChanger = this.SizeChanger,

                // Status Flags
                IsRoot = this.IsRoot,
                IsEquipment = this.IsEquipment,
                IsLoot = this.IsLoot,
                IsEquipmentRoot = this.IsEquipmentRoot,
                IsInPlayerInventory = this.IsInPlayerInventory,

                // Action Flags
                IsDropAble = this.IsDropAble,
                IsRemoveAble = this.IsRemoveAble,
                IsUnloadAble = this.IsUnloadAble,
                IsModificationAble = this.IsModificationAble,
                IsOpenAble = this.IsOpenAble,
                IsUsable = this.IsUsable,
                CanReload = this.CanReload,

                // Alap adatok
                ItemType = this.ItemType,
                SystemName = this.SystemName,
                ItemName = this.ItemName,
                Description = this.Description,
                MaxStackSize = this.MaxStackSize,
                Quantity = this.Quantity,
                Value = this.Value,
                SizeX = this.SizeX,
                SizeY = this.SizeY,

                // NonGeneral Weapon Veriables
                MagasineSize = this.MagasineSize,
                Spread = this.Spread,
                Fpm = this.Fpm,
                Recoil = this.Recoil,
                Accturacy = this.Accturacy,
                Range = this.Range,
                Ergonomy = this.Ergonomy,
                Caliber = this.Caliber,

                // NonGeneral Usable Items Veriables
                UseLeft = this.UseLeft,
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
            return (null, null, false);
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

            var partFound = Parts.FirstOrDefault(part => !string.IsNullOrEmpty(part.PartData.MainItem.SystemName));
            if (partFound != null)
            {
                MainItem mainItem = partFound.PartData.MainItem;
                if (mainItem.NecessaryItemTypes.All(Type => Parts.Exists(part => part.item_s_Part.ItemType == Type)))
                {
                    SystemName = mainItem.SystemName;
                    ItemName = mainItem.MainItemName;
                    Description = mainItem.Desctription;
                    ItemType = mainItem.Type;
                }
                else
                {
                    SystemName = $"Incompleted {mainItem.SystemName}";
                    ItemName = $"Incompleted {mainItem.SystemName}";
                    Description = mainItem.Desctription;
                    ItemType = mainItem.Type;
                }
            }
            else
            {
                if (Parts.Count>1)
                {
                    SystemName = FirstItem.SystemName +" ...";
                    ItemName = FirstItem.ItemName + " ...";
                    Description = FirstItem.Description;
                    ItemType = FirstItem.ItemType;
                }
                else
                {
                    SystemName = FirstItem.SystemName;
                    ItemName = FirstItem.ItemName;
                    Description = FirstItem.Description;
                    ItemType = FirstItem.ItemType;
                }
            }

            Quantity = FirstItem.Quantity;
            MaxStackSize = FirstItem.MaxStackSize;
            IsModificationAble = true;

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
                if (Parts.Count>1 && item.SizeChanger.Direction != '-')
                {
                    char direction = item.SizeChanger.Direction;
                    SizeChanger sizeChanger = item.SizeChanger;
                    if (direction == 'R' || direction == 'L')
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
                if (item.Caliber != null)//csak 1 lehet
                {
                    Caliber = item.Caliber;
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
        //action (Only Live Inventory)
        public void Shoot()
        {

        }
        public Item()//ha contume itememt akarunk letrehozni mint pl: egy Root item
        {

        }
        public Item(string SystemName, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            AdvancedItemStruct advancedItemRef = AdvancedItemHandler.AdvancedItemDatas.GetAdvancedItemData(SystemName);

            Item item = new()
            {
                ItemType = advancedItemRef.Type,//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
                SystemName = advancedItemRef.SystemName,//ez alapján hozza létre egy item saját magát
                ItemName = advancedItemRef.ItemName,
                Description = advancedItemRef.Description,
                Quantity = count,
                Value = advancedItemRef.Value,
                SizeX = advancedItemRef.SizeX,
                SizeY = advancedItemRef.SizeY,
                MaxStackSize = advancedItemRef.MaxStackSize,
                //Action
                IsDropAble = advancedItemRef.IsDropAble,
                IsRemoveAble = advancedItemRef.IsRemoveAble,
                IsUnloadAble = advancedItemRef.IsUnloadAble,
                IsModificationAble = advancedItemRef.IsModificationAble,
                IsOpenAble = advancedItemRef.IsOpenAble,
                IsUsable = advancedItemRef.IsUsable,
                //tartalom

                //fegyver adatok
                MagasineSize = advancedItemRef.MagasineSize,
                Spread = advancedItemRef.Spread,
                Fpm = advancedItemRef.Fpm,
                Recoil = advancedItemRef.Recoil,
                Accturacy = advancedItemRef.Accturacy,
                Range = advancedItemRef.Range,
                Ergonomy = advancedItemRef.Ergonomy,
                Caliber = advancedItemRef.Caliber,
                //hasznalhato e?
                UseLeft = advancedItemRef.UseLeft,
                //Advanced
                SizeChanger = advancedItemRef.SizeChanger,
            };

            if (advancedItemRef.ContainerPath != "-")
            {
                item.Container = new Container(advancedItemRef.ContainerPath);
            }
            else
            {
                item.Container = null;
            }

            Parts = new List<Part>
            {
                new(item)
            };

            //fügvény ami az össze spart ertekeit az advanced valtozoba tölti és adja össze
            AdvancedItemContsruct();
        }
    }
    public class SystemPoints
    {
        public GameObject RefPoint1 = null;//LIVE
        public GameObject RefPoint2 = null;//LIVE

        public SP SPData;
        public Part SelfPart;//a part amelyikhez tartozik
        public SystemPoints(SP sPData, Part selfPart)
        {
            SPData = sPData;
            SelfPart = selfPart;
        }
        public void SetLive()
        {
            GameObject SP = CreatePrefab(AdvancedItemHandler.CPPath);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            SP.transform.SetParent(SelfPart.PartObject.transform.GetChild(0).transform);
            Texture2D texture = Resources.Load<Texture2D>(SelfPart.PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;

            //!!! miert valtozik a scale ez elott meg?
            SP.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            SP.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            SP.GetComponent<RectTransform>().localPosition = Vector2.zero;

            SP.name = SPData.PointName;
            RefPoint1 = SP.transform.GetChild(0).gameObject;
            RefPoint2 = SP.transform.GetChild(1).gameObject;

            RectTransform rt1 = RefPoint1.GetComponent<RectTransform>();
            rt1.anchoredPosition = Vector2.zero;
            rt1.anchorMin = new Vector2(SPData.AnchorMin1.X, SPData.AnchorMin1.Y);
            rt1.anchorMax = new Vector2(SPData.AnchorMax1.X, SPData.AnchorMax1.Y);

            RectTransform rt2 = RefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(SPData.AnchorMin2.X, SPData.AnchorMin2.Y);
            rt2.anchorMax = new Vector2(SPData.AnchorMin2.X, SPData.AnchorMin2.Y);
        }
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
        public SystemPoints[] SystemPoints;

        public Item item_s_Part;//az item aminek a partja
        public PartData PartData;
        public Part(Item item)
        {
            item_s_Part = item;
            PartData = AdvancedItemHandler.AdvancedItemDatas.GetPartData(item.SystemName);
            ConnectionPoints = new ConnectionPoint[PartData.CPs.Length];
            SystemPoints = new SystemPoints[PartData.SPs.Length];

            for (int i = 0; i < ConnectionPoints.Length; i++)
            {
                ConnectionPoints[i] = new ConnectionPoint(PartData.CPs[i], this);
            }
            for (int i = 0; i < SystemPoints.Length; i++)
            {
                SystemPoints[i] = new SystemPoints(PartData.SPs[i],this);
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
            //new LootItem("Glock_17_9x19_pistol_PS9",1f),
            //new LootItem("AK103", 2f),
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

                    Item newItem = new(Incoming.SystemName, larger);

                    GameObject itemObject = CreatePrefab(Item.AdvancedItemObjectParth);
                    itemObject.name = newItem.SystemName;
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
        public static void ItemCompoundRefresh(ItemImgFitter ItemCompound,Item ActualData)
        {
            for (int i = ItemCompound.fitter.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = ItemCompound.fitter.transform.GetChild(i);
                child.SetParent(null);
                GameObject.Destroy(child.gameObject);
            }

            ItemCompound.ResetFitter();

            foreach (Part part in ActualData.Parts)
            {
                part.SetLive(ItemCompound.fitter.gameObject);
                foreach (ConnectionPoint cp in part.ConnectionPoints)
                {
                    cp.SetLive();
                }
                foreach (SystemPoints sp in part.SystemPoints)
                {
                    sp.SetLive();
                }
                part.PartObject.transform.localRotation = Quaternion.Euler(0, 0, 0);//nem tudom miert fordul el, de ezert szuksges a visszaallitas
            }
            foreach (Part part in ActualData.Parts)
            {
                foreach (ConnectionPoint connectionPoint in part.ConnectionPoints)
                {
                    if (connectionPoint.ConnectedPoint != null)
                    {
                        if (connectionPoint.SelfPart.HierarhicPlace < connectionPoint.ConnectedPoint.SelfPart.HierarhicPlace)
                        {
                            // 1. Referenciapontok lekérése és kiíratása
                            RectTransform targetPoint1 = connectionPoint.RefPoint1.GetComponent<RectTransform>();
                            RectTransform targetPoint2 = connectionPoint.RefPoint2.GetComponent<RectTransform>();

                            // 2. Mozgatandó objektum és referencia pontok lekérése
                            RectTransform toMoveObject = connectionPoint.ConnectedPoint.SelfPart.PartObject.GetComponent<RectTransform>();
                            RectTransform toMovePoint1 = connectionPoint.ConnectedPoint.RefPoint1.GetComponent<RectTransform>();
                            RectTransform toMovePoint2 = connectionPoint.ConnectedPoint.RefPoint2.GetComponent<RectTransform>();

                            // 3. Skálázási faktor számítása
                            float targetLocalDistance = Vector3.Distance(targetPoint1.position, targetPoint2.position);
                            float toMoveLocalDistance = Vector3.Distance(toMovePoint1.position, toMovePoint2.position);
                            float scaleFactor = targetLocalDistance / toMoveLocalDistance;
                            //Debug.LogWarning(part.item_s_Part.ItemName+" "+scaleFactor+ " targetLocalDistante: " + targetLocalDistance+ " toMoveLocalDistance: "+ toMoveLocalDistance);
                            if (float.IsNaN(scaleFactor))
                            {
                                scaleFactor = 1;
                            }

                            // 4. Alkalmazzuk a skálázást
                            toMoveObject.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                            // 5. Pozíciók kiszámítása
                            Vector3 targetMidLocal = (targetPoint1.position + targetPoint2.position) * 0.5f;
                            Vector3 toMoveMidLocal = (toMovePoint1.position + toMovePoint2.position) * 0.5f;
                            Vector3 translationLocal = targetMidLocal - toMoveMidLocal;

                            // 6. Alkalmazzuk az eltolást
                            toMoveObject.position += translationLocal;
                        }
                    }
                }
            }
            ItemCompound.Fitting();
        }
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
            if (Stand != Incoming && Stand.MaxStackSize > 1 && Stand.SystemName == Incoming.SystemName)
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
                SystemName = "Root",
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

                List<Part> ChangerParts = IncomingParts.Where(part => part.item_s_Part.SizeChanger.Direction != '-').OrderBy(part => part.item_s_Part.SizeChanger.MaxPlus).ToList();

                foreach (Part part in ChangerParts)
                {
                    SizeChanger sizeChanger = part.item_s_Part.SizeChanger;

                    if (sizeChanger.Direction == 'R' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'L' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'U' && ChangedSize.Y < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'D' && ChangedSize.Y < sizeChanger.MaxPlus)
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
                List<Part> StandParts = AdvancedItem.Parts.Where(part=> !IncomingParts.Contains(part) && part.item_s_Part.SizeChanger.Direction != '-').OrderBy(part=>part.item_s_Part.SizeChanger.MaxPlus).ToList();

                //azon incoming partok ameyleknek van sizechanger és rendezve vannak maxplus szeirtn
                List<Part> ChangerParts = IncomingParts.Where(part => part.item_s_Part.SizeChanger.Direction != '-').OrderBy(part => part.item_s_Part.SizeChanger.MaxPlus).ToList();

                //vegig megyunk azokon a partokaon amelyek maradnak és nem tavolitodnak el tovabba rendelkeznek sizechangerrel
                //tájolás szerint biraljuk el sizechangerjuket
                //minden sizechanger csak annyit novelhet amekkorat maximum-ja enged.
                foreach (Part part in StandParts)
                {
                    SizeChanger sizeChanger = part.item_s_Part.SizeChanger;

                    if (sizeChanger.Direction == 'R' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'L' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'U' && ChangedSize.Y < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'D' && ChangedSize.Y < sizeChanger.MaxPlus)
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

                    if (sizeChanger.Direction == 'R' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'L' && ChangedSize.X < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'U' && ChangedSize.Y < sizeChanger.MaxPlus)
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
                    else if (sizeChanger.Direction == 'D' && ChangedSize.Y < sizeChanger.MaxPlus)
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
                //Debug.LogWarning($"{item.SystemName}   add player inventory");
                AddPlayerInventory(item);
            }
            else if (item.IsInPlayerInventory && !StatusParent.IsInPlayerInventory)
            {
                //Debug.LogWarning($"{item.SystemName}   remove player inventory");
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
                Debug.LogWarning($"{clonedItem.ItemName}           {clonedItem.SystemName}     {clonedItem.Parts != null }      --------      cp ");
                clonedItem.LevelManagerRef = clone;

                // Ha fejlett (advanced) itemről van szó, klónozzuk a részeit (Part-eket).
                if (!clonedItem.IsRoot)
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
                SetupConnectionPoints(original, clone, i);
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
                        Debug.LogWarning($"{usedPart.PartData.PartName}      cp");
                        Item item = original.Items.Find(item => !item.IsRoot && item.Parts.Contains(usedPart));
                        int itemIndex = original.Items.IndexOf(item);
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