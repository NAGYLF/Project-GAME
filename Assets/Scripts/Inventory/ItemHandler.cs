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
using System.Collections;
using MainData;
using Newtonsoft.Json;
using System.IO;
using Items;
using static CharacterHand;

namespace ItemHandler
{
    public interface IItemComponent
    {
        IItemComponent CloneComponent();
        void Inicialisation(AdvancedItem advancedItem);
        IEnumerator Control(InputFrameData input);
    }
    public class ItemSlotData
    {
        public int SectorID;
        public (int Height, int Widht) Coordinate;
        public string SlotType;
        public AdvancedItem PartOfItemData;

        public ItemSlotData(int SectorID,int Height,int Width,string SlotType = "", AdvancedItem PartOfItemData = null)
        {
            this.SectorID = SectorID;
            this.Coordinate = (Height,Width);
            this.SlotType = SlotType;
            this.PartOfItemData = PartOfItemData;
        }
    }
    public class AdvancedItem
    {
        public const string SimpleItemObjectParth = "GameElements/SimpleItemObject";
        public const string AdvancedItemObjectParth = "GameElements/AdvancedItemObject";
        public const string TemporaryItemObjectPath = "GameElements/TemporaryAdvancedItemObject";

        #region Ref Variables
        public ModificationWindow ModificationWindowRef;
        public LevelManager LevelManagerRef;
        public List<ItemSlotData> ItemSlotsDataRef = new List<ItemSlotData>();
        public List<AdvancedItem> ContainerItemListRef = new List<AdvancedItem>();
        public HotKey hotKeyRef;
        public List<ItemSlot> ItemSlotObjectsRef = new List<ItemSlot>();
        public CharacterHand PlayerHandRef;
        public GameObject SelfGameobject { get; set; }
        public GameObject InGameSelfObject { get; set; }
        public AdvancedItem ParentItem { get; set; }//az az item ami tárolja ezt az itemet
        #endregion

        #region PlacerVariables
        public List<Action> AvaiablePlacerMetodes = new List<Action>();
        public AdvancedItem AvaiableParentItem { get; set; }
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

        //nem szukseges clonozni
        #region Live Action Flags
        public bool IsShooting { get; set; } = false;
        public bool IsReloading { get; set; } = false;
        public bool IsUnloading { get; set; } = false;
        public bool IsAiming{ get; set; } = false;
        public bool IsUsing { get; set; } = false;
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
        public List<Part> Parts { get; set; } = new List<Part>();
        public Container Container { get; set; }
        #endregion

        #region Componens Manager
        public Dictionary<Type, IItemComponent> Components { private set; get; } = new();

        public void AddComponent(IItemComponent component)
        {
            if (component == null) return;

            var type = component.GetType();
            if (type == typeof(IItemComponent)) return; // Ne tároljon általános interfészt

            if (Components.TryGetValue(type, out var existing))
            {
                MergeWith(existing,component);//out of order
                return;
            }

            Components[type] = component;
        }

        public bool RemoveComponent<T>() where T : IItemComponent
        {
            var type = typeof(T);
            if (type == typeof(IItemComponent)) return false;
            return Components.Remove(type);
        }

        public bool TryGetComponent<T>(out T component) where T : class, IItemComponent
        {
            var type = typeof(T);
            if (type == typeof(IItemComponent))
            {
                component = null;
                return false;
            }

            if (Components.TryGetValue(type, out var found))
            {
                component = found as T;
                return component != null;
            }

            component = null;
            return false;
        }

        private void MergeWith(IItemComponent first,IItemComponent second)
        {
            //assault rifle data merge
            if (first is WeaponBody && second is WeaponBody)
            {

            }
        }
        #endregion
        public AdvancedItem()
        {

        }
        public AdvancedItem(string SystemName, int count = 1)// egy itemet mindeg név alapjan peldanyositunk
        {
            AdvancedItemStruct advancedItemRef = DataHandler.GetAdvancedItemData(SystemName);

            SimpleItem item = new(advancedItemRef);
            item.Quantity = count;

            Parts.Add(new(item));

            //fügvény ami az össze spart ertekeit az advanced valtozoba tölti és adja össze
            AdvancedItemContsruct();
        }
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
        //public void Use()
        //{
        //    if (IsUsable)
        //    {
        //        UseLeft--;
        //        if (UseLeft == 0)
        //        {
        //            InventorySystem.Delete(this);
        //            if (SelfGameobject)
        //            {
        //                SelfGameobject.GetComponent<ItemObject>().DestroyContainer();
        //                GameObject.Destroy(SelfGameobject);
        //            }
        //        }
        //    }
        //}
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
        //action (Only Live Inventory)
        public void Shoot()
        {

        }
        public AdvancedItem ShallowClone()
        {
            AdvancedItem cloned = new()
            {
                Lvl = this.Lvl,
                HotKey = this.HotKey,
                RotateDegree = this.RotateDegree,
                SectorId = this.SectorId,

                IsInPlayerInventory = this.IsInPlayerInventory,
                IsEquipment = this.IsEquipment,
                IsLoot = this.IsLoot,
                IsRoot = this.IsRoot,
                IsEquipmentRoot = this.IsEquipmentRoot,

                IsDropAble = this.IsDropAble,
                IsRemoveAble = this.IsRemoveAble,
                IsUnloadAble = this.IsUnloadAble,
                IsModificationAble = this.IsModificationAble,
                IsOpenAble = this.IsOpenAble,
                IsUsable = this.IsUsable,
                CanReload = this.CanReload,

                ItemType = this.ItemType,
                SystemName = this.SystemName,
                ItemName = this.ItemName,
                Description = this.Description,
                MaxStackSize = this.MaxStackSize,
                Quantity = this.Quantity,
                Value = this.Value,
                SizeX = this.SizeX,
                SizeY = this.SizeY,
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
        public (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) PartPut_IsPossible(AdvancedItem Incoming_AdvancedItem)
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
        public void PartPut(AdvancedItem AdvancedItem, ConnectionPoint SCP, ConnectionPoint ICP)//ha egy item partjait belerakjuk akkor az item az inventoryban megmaradhat ezert azt torolni kellesz vagy vmi
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
            SimpleItem FirstItem = Parts.First().item_s_Part;

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
                    ItemType = $"Incompleted {mainItem.Type}";
                }
            }
            else
            {
                if (Parts.Count > 1)
                {
                    SystemName = FirstItem.SystemName + " ...";
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

            Components.Clear();
            Quantity = FirstItem.Quantity;
            MaxStackSize = FirstItem.MaxStackSize;
            IsModificationAble = true;

            SizeX = FirstItem.SizeX;
            SizeY = FirstItem.SizeY;
            Container = FirstItem.Container;

            foreach (Part part in Parts)
            {
                SimpleItem item = part.item_s_Part;
                Value += item.Value;
                if (Parts.Count > 1 && item.SizeChanger.Direction != '-')
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

                if (item.Component != null)
                {
                    AddComponent(item.Component);
                }
            }
        }
    }
    public class SimpleItem
    {
        #region System Variables
        public SizeChanger SizeChanger { get; set; }
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

        #region General Variables
        public string SystemName { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public int MaxStackSize { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public Container Container { get; set; }
        #endregion

        public IItemComponent Component { get; set; }

        private SimpleItem()
        {

        }
        public SimpleItem(AdvancedItemStruct advancedItemStruct)
        {
            SizeChanger = advancedItemStruct.SizeChanger;

            IsDropAble = advancedItemStruct.IsDropAble;
            IsRemoveAble = advancedItemStruct.IsRemoveAble;
            IsUnloadAble = advancedItemStruct.IsUnloadAble;
            IsModificationAble = advancedItemStruct.IsModificationAble;
            IsOpenAble = advancedItemStruct.IsOpenAble;
            IsUsable = advancedItemStruct.IsUsable;
            CanReload = advancedItemStruct.CanReload;

            SystemName = advancedItemStruct.SystemName;
            ItemName = advancedItemStruct.ItemName;
            ItemType = advancedItemStruct.Type;
            Description = advancedItemStruct.Description;
            MaxStackSize = advancedItemStruct.MaxStackSize;
            Quantity = advancedItemStruct.Quantity;
            Value = advancedItemStruct.Value;
            SizeX = advancedItemStruct.SizeX;
            SizeY = advancedItemStruct.SizeY;

            if (advancedItemStruct.ContainerPath != "-")
            {
                Container = new Container(advancedItemStruct.ContainerPath);
            }

            switch (ItemType)
            {
                case nameof(WeaponBody):
                    Component = new WeaponBody(advancedItemStruct);
                    break;
                case nameof(Ammunition):
                    Component = new Ammunition(advancedItemStruct);
                    break;
                case nameof(Magasine):
                    Component = new Magasine(advancedItemStruct);
                    break;
                default:
                    break;
            }
        }

        public SimpleItem ShallowClone()
        {
            SimpleItem clone = new SimpleItem()
            {
                SizeChanger = this.SizeChanger,

                IsDropAble = this.IsDropAble,
                IsRemoveAble = this.IsRemoveAble,
                IsUnloadAble = this.IsUnloadAble,
                IsModificationAble = this.IsModificationAble,
                IsOpenAble = this.IsOpenAble,
                IsUsable = this.IsUsable,
                CanReload = this.CanReload,

                SystemName = this.SystemName,
                ItemName = this.ItemName,
                ItemType = this.ItemType,
                Description = this.Description,
                MaxStackSize = this.MaxStackSize,
                Quantity = this.Quantity,
                Value = this.Value,
                SizeX = this.SizeX,
                SizeY = this.SizeY,
            };

            if (this.Container != null)
            {
                clone.Container = new Container(Container.PrefabPath);
            }

            if (Component != null)
            {
                clone.Component = this.Component.CloneComponent();
            }

            return clone;
        }
    }
    public class SystemPoints
    {
        public GameObject RefPoint1 = null;//LIVE
        public GameObject RefPoint2 = null;//LIVE

        public GameObject InGameRefPoint1 = null;//LIVE
        public GameObject InGameRefPoint2 = null;//LIVE

        public SP SPData;
        public Part SelfPart;//a part amelyikhez tartozik
        public SystemPoints(SP sPData, Part selfPart)
        {
            SPData = sPData;
            SelfPart = selfPart;
        }
        public void SetLive()
        {
            GameObject SP = CreatePrefab(Main.DataHandler.CPPath);

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
            rt1.anchorMin = new Vector2(SPData.AnchorMin1.x, SPData.AnchorMin1.y);
            rt1.anchorMax = new Vector2(SPData.AnchorMax1.x , SPData.AnchorMax1.y);

            RectTransform rt2 = RefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(SPData.AnchorMin2.x, SPData.AnchorMin2.y);
            rt2.anchorMax = new Vector2(SPData.AnchorMin2.x , SPData.AnchorMin2.y);
        }
        public void SetLiveInGame()
        {
            GameObject SP = CreatePrefab(Main.DataHandler.CPPath);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            SP.transform.SetParent(SelfPart.InGamePartObject.transform.GetChild(0).transform);
            Texture2D texture = Resources.Load<Texture2D>(SelfPart.PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;

            //!!! miert valtozik a scale ez elott meg?
            SP.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            SP.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            SP.GetComponent<RectTransform>().localPosition = Vector2.zero;

            SP.name = SPData.PointName;
            InGameRefPoint1 = SP.transform.GetChild(0).gameObject;
            InGameRefPoint2 = SP.transform.GetChild(1).gameObject;

            RectTransform rt1 = InGameRefPoint1.GetComponent<RectTransform>();
            rt1.anchoredPosition = Vector2.zero;
            rt1.anchorMin = new Vector2(SPData.AnchorMin1.x, SPData.AnchorMin1.y);
            rt1.anchorMax = new Vector2(SPData.AnchorMax1.x, SPData.AnchorMax1.y);


            RectTransform rt2 = InGameRefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(SPData.AnchorMin2.x, SPData.AnchorMin2.y);
            rt2.anchorMax = new Vector2(SPData.AnchorMin2.x, SPData.AnchorMin2.y);
        }
    }
    //a connection point inpectorban létező dolog ami lenyegeben statikusan jelen van nem kell generalni
    [System.Serializable]
    public class ConnectionPoint
    {
        //Live adatok meylek addig vannak amig a connectionPoint létezik
        public GameObject RefPoint1 = null;//LIVE
        public GameObject RefPoint2 = null;//LIVE

        public GameObject InGameRefPoint1 = null;//LIVE
        public GameObject InGameRefPoint2 = null;//LIVE

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
            GameObject CP = CreatePrefab(Main.DataHandler.CPPath);

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
            rt1.anchorMin = new Vector2(CPData.AnchorMin1.x, CPData.AnchorMin1.y);
            rt1.anchorMax = new Vector2(CPData.AnchorMax1.x, CPData.AnchorMax1.y);


            RectTransform rt2 = RefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(CPData.AnchorMin2.x, CPData.AnchorMin2.y);
            rt2.anchorMax = new Vector2(CPData.AnchorMin2.x, CPData.AnchorMin2.y);
        }
        public void SetLiveInGame()
        {
            GameObject CP = CreatePrefab(Main.DataHandler.CPPath);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            CP.transform.SetParent(SelfPart.InGamePartObject.transform.GetChild(0).transform);
            Texture2D texture = Resources.Load<Texture2D>(SelfPart.PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;

            //!!! miert valtozik a scale ez elott meg?
            CP.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            CP.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            CP.GetComponent<RectTransform>().localPosition = Vector2.zero;

            CP.name = CPData.PointName;
            InGameRefPoint1 = CP.transform.GetChild(0).gameObject;
            InGameRefPoint2 = CP.transform.GetChild(1).gameObject;

            RectTransform rt1 = InGameRefPoint1.GetComponent<RectTransform>();
            rt1.anchoredPosition = Vector2.zero;
            rt1.anchorMin = new Vector2(CPData.AnchorMin1.x, CPData.AnchorMin1.y);
            rt1.anchorMax = new Vector2(CPData.AnchorMax1.x, CPData.AnchorMax1.y);


            RectTransform rt2 = InGameRefPoint2.GetComponent<RectTransform>();
            rt2.anchoredPosition = Vector2.zero;
            rt2.anchorMin = new Vector2(CPData.AnchorMin2.x, CPData.AnchorMin2.y);
            rt2.anchorMax = new Vector2(CPData.AnchorMin2.x, CPData.AnchorMin2.y);
        }
    }
    public class Part
    {
        //Live adatok meylek addig vannak amig a connectionPoint létezik
        public GameObject PartObject;//csak live ban van
        public GameObject InGamePartObject;

        //active adatok melyek valtozhatnak
        public int HierarhicPlace = 0;

        //statikus adatok melyek nem valtoznak
        public ConnectionPoint[] ConnectionPoints;//a tartalmazott pontok
        public SystemPoints[] SystemPoints;

        public SimpleItem item_s_Part;//az item aminek a partja
        public PartData PartData;
        public Part(SimpleItem item)
        {
            item_s_Part = item;
            PartData = DataHandler.GetPartData(item.SystemName);
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
            GameObject Part = CreatePrefab(DataHandler.PartPath);
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
        public void SetLiveInGame(GameObject ParentObject)
        {
            //Debug.LogWarning($"Set {PartData.PartName}");
            GameObject InGamePart = CreatePrefab(DataHandler.PartPath);
            //Debug.LogWarning($"creted obejct {Part.GetInstanceID()}");
            InGamePartObject = InGamePart;
            //Debug.LogWarning($"referalt obejct {PartObject.GetInstanceID()}");
            InGamePart.name = PartData.PartName;
            InGamePart.GetComponent<PartObject>().SelfData = this;

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            InGamePart.transform.SetParent(ParentObject.transform);

            //!!! Ez változhat a fejlesztes soran szoval oda kell ra figyelni !!!
            Sprite sprite = Resources.Load<Sprite>(PartData.ImagePath);
            InGamePart.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            InGamePart.GetComponent<RectTransform>().localPosition = Vector2.zero;
            InGamePart.GetComponent<RectTransform>().localScale = Vector3.one;
            Texture2D texture = Resources.Load<Texture2D>(PartData.ImagePath);
            float imgWidth = texture.width;
            float imgHeight = texture.height;
            InGamePart.GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
            InGamePart.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(imgWidth, imgHeight);
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
            if (InGamePartObject != null)//??? nem bitios hoyg mukodik
            {
                InGamePartObject.transform.SetParent(null);
                UnityEngine.Object.Destroy(InGamePartObject);
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
        public List<AdvancedItem> Items { get; set; }
        public ItemSlotData[][,] NonLive_Sectors { get; set; }
        public ItemSlot[][,] Live_Sector { get; set; }//ezek referanca pontokat atralamaznak amelyeken kersztul a tenyleges gameobjectumokat manipulalhatjuk
        public GameObject ContainerObject { get; set; }//conainer objectum
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            ContainerObject containerObject = Resources.Load(prefabPath).GetComponent<ContainerObject>();
            ContainerObject.SectorData[] staticSectorDatas = containerObject.StaticSectorDatas;

            Items = new List<AdvancedItem>();
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
                AdvancedItem item = new(LootItem.Name);
                if (item.MaxStackSize > 1)
                {
                    item = new AdvancedItem(LootItem.Name, UnityEngine.Random.Range(Mathf.RoundToInt(item.MaxStackSize * LootItem.MinStack), Mathf.RoundToInt(item.MaxStackSize * LootItem.MaxStack)));
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
            public AdvancedItem Stand { get; private set; }
            public AdvancedItem Incoming { get; private set; }
            public Merge(AdvancedItem stand, AdvancedItem incoming)
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
            public AdvancedItem Incoming { get; private set; }
            public ItemSlot[] ActiveSlots { get; private set; }
            public AdvancedItem Stand { get; private set; } = null;
            public Split(AdvancedItem incoming, ItemSlot[] activeSlots)
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
                    AdvancedItem Parent = ActiveSlots.First().SlotParentItem;

                    AdvancedItem newItem = new(Incoming.SystemName, larger);

                    GameObject itemObject = CreatePrefab(AdvancedItem.AdvancedItemObjectParth);
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
            public AdvancedItem IncomingItem { get; private set; }
            public AdvancedItem InteractiveItem { get; private set; }
            public bool IsPossible { get; private set; }

            private ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect;
            private (HashSet<(int Height, int Widht)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition;
            private (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data;
            public MergeParts(AdvancedItem interactiveItem,AdvancedItem incomingItem)
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
            public RePlace(AdvancedItem item, AdvancedItem Parent,ItemSlot[] activeSlots)
            {
                this.item = item;
                this.PossibleNewParent = Parent;
                this.activeSlots = activeSlots.ToArray();
            }

            public AdvancedItem PossibleNewParent{ get; private set; }
            public AdvancedItem item { get; private set; }
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
        public static void ItemCompoundRefresh_Live(ItemImgFitter ItemCompound, AdvancedItem ActualData)
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
                part.SetLiveInGame(ItemCompound.fitter.gameObject);
                foreach (ConnectionPoint cp in part.ConnectionPoints)
                {
                    cp.SetLiveInGame();
                }
                foreach (SystemPoints sp in part.SystemPoints)
                {
                    sp.SetLiveInGame();
                }
                part.InGamePartObject.transform.localRotation = Quaternion.Euler(0, 0, 0);//nem tudom miert fordul el, de ezert szuksges a visszaallitas
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
                            RectTransform targetPoint1 = connectionPoint.InGameRefPoint1.GetComponent<RectTransform>();
                            RectTransform targetPoint2 = connectionPoint.InGameRefPoint2.GetComponent<RectTransform>();

                            // 2. Mozgatandó objektum és referencia pontok lekérése
                            RectTransform toMoveObject = connectionPoint.ConnectedPoint.SelfPart.InGamePartObject.GetComponent<RectTransform>();
                            RectTransform toMovePoint1 = connectionPoint.ConnectedPoint.InGameRefPoint1.GetComponent<RectTransform>();
                            RectTransform toMovePoint2 = connectionPoint.ConnectedPoint.InGameRefPoint2.GetComponent<RectTransform>();

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
        public static void ItemCompoundRefresh(ItemImgFitter ItemCompound,AdvancedItem ActualData)
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
        public static void Placer(AdvancedItem item, float originalRotation)
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
        public static void Delete(AdvancedItem item)
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
        public static bool CanSplitable(AdvancedItem Stand, AdvancedItem Incoming)
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
        public static bool CanMergable(AdvancedItem Stand, AdvancedItem Incoming)
        {
            if (Stand != Incoming && Stand.MaxStackSize > 1 && Stand.SystemName == Incoming.SystemName)
            {
                return true;
            }
            return false;
        }
        public static void LiveCleaning(AdvancedItem item)//ha az inventory-t bezarjuk akkor megsemisulnek a refernciak es egy nullokkal teli lista lesz, ez ezt hivatott orvosolni
        {
            item.ItemSlotObjectsRef.Clear();
        }
        #endregion

        #region Data Manipulation
        public static void Add(AdvancedItem item, AdvancedItem Parent)
        {
            item.ParentItem = Parent;
            Parent.Container.Items.Add(item);
            item.ContainerItemListRef = Parent.Container.Items;//set ref
        }
        public static void Remove(AdvancedItem item, AdvancedItem Parent)
        {
            item.ParentItem = null;
            Parent.Container.Items.Remove(item);
            item.ContainerItemListRef = null;//unset ref
        }

        public static (List<AdvancedItem> items,int remaing) InventoryTakeOut(ref LevelManager levelManager, string SystemName, int count = 1)
        {
            List<AdvancedItem> advancedItems = new List<AdvancedItem>();
            List<AdvancedItem> removableItems = new List<AdvancedItem>();
            foreach (AdvancedItem item in levelManager.Items)
            {
                if (count > 0 && item.SystemName == SystemName)
                {
                    if (item.Quantity-count < 1)
                    {
                        count -= item.Quantity;
                        removableItems.Add(item);
                        advancedItems.Add(item);
                    }
                    else
                    {
                        item.Quantity-= count;
                        advancedItems.Add(item);
                        count = 0;
                    }
                }
            }
            foreach (AdvancedItem item in removableItems)
            {
                Delete(item);
            }
            return (advancedItems,count);
        }
        public static void InventorySave(ref LevelManager levelManager, string FilePath, string FileName)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(levelManager, settings);

            Debug.Log(json);

            File.WriteAllText(FilePath, json);
        }
        public static void InventoryLoad(ref LevelManager levelManager, string FilePath, string FileName)
        {
            string jsonFromFile = File.ReadAllText(FilePath);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            levelManager = JsonConvert.DeserializeObject<LevelManager>(jsonFromFile, settings);
        }
        public static void InventoryDefault(ref LevelManager levelManager)
        {
            List<AdvancedItem> TemporaryItemList = new List<AdvancedItem>(levelManager.Items);
            foreach (AdvancedItem item in TemporaryItemList)
            {
                Delete(levelManager.Items.Find(i => i == item));
            }
            levelManager.Items.Clear();

            AdvancedItem RootRef = TemporaryItemList.First();

            AdvancedItem RootData = new()
            {
                SystemName = RootRef.SystemName,
                ItemName = RootRef.ItemName,
                Lvl = RootRef.Lvl,
                SectorId = RootRef.SectorId,
                Coordinates = RootRef.Coordinates.ToArray(),
                IsRoot = RootRef.IsRoot,
                IsEquipmentRoot = RootRef.IsEquipmentRoot,
                IsInPlayerInventory = RootRef.IsInPlayerInventory,
                Container = new Container(TemporaryItemList.First().Container.PrefabPath),
                LevelManagerRef = levelManager,
            };

            levelManager.Items.Add(RootData);

            levelManager.SetMaxLVL_And_Sort();

            TemporaryItemList.Clear();
        }
        public static bool InventoryAdd(ref LevelManager levelManager, AdvancedItem item)
        {
            bool ItemAdded = false;
            int quantity = item.Quantity;
            if (item.MaxStackSize > 1)//gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int lvl = 0; lvl <= levelManager.MaxLVL && !ItemAdded; lvl++)//equipment
                {
                    //Debug.LogWarning($"{item.ItemName}     maxlvl{levelManager.MaxLVL} / {lvl}");
                    ItemAdded = AddingByCount(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)//uj item hozzaadasa
            {
                for (int lvl = -1; lvl <= levelManager.MaxLVL && !ItemAdded; lvl++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItem(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)//uj item hozzaadasa rotate-vel
            {
                for (int lvl = -1; lvl <= levelManager.MaxLVL && !ItemAdded; lvl++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItemByRotate(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)
            {
                Debug.LogWarning($"item: {item.SystemName} cannot added, probably no space for that");
                return false;
            }
            return true;
        }
        public static bool InventoryAdd(ref LevelManager levelManager, AdvancedItem item, int StartLVL = 1, int StopLVL = 1)
        {
            bool ItemAdded = false;
            int quantity = item.Quantity;
            if (item.MaxStackSize > 1)//gyorsitott item hozzadas mely nem ad uj elemet hanem csak quanity-t novel
            {
                for (int lvl = StartLVL; (lvl <= levelManager.MaxLVL || lvl <= StopLVL) && !ItemAdded; lvl++)//equipment
                {
                    //Debug.LogWarning($"{item.ItemName}     maxlvl{levelManager.MaxLVL} / {lvl}");
                    ItemAdded = AddingByCount(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)//uj item hozzaadasa
            {
                for (int lvl = StartLVL; (lvl <= levelManager.MaxLVL || lvl <= StopLVL) && !ItemAdded; lvl++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItem(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)//uj item hozzaadasa rotate-vel
            {
                for (int lvl = StartLVL; (lvl <= levelManager.MaxLVL || lvl <= StopLVL) && !ItemAdded; lvl++)//vegig iterálunk az osszes equipmenten
                {
                    ItemAdded = AddingByNewItemByRotate(levelManager, lvl, item);
                }
            }
            if (!ItemAdded)
            {
                Debug.LogWarning($"item: {item.SystemName} cannot added, probably no space for that");
                return false;
            }
            return true;
        }
        public static (bool IsCompleted, int Remaining) InventoryRemove(ref LevelManager levelManager, AdvancedItem item, int count = 1)
        {
            if (levelManager.Items.Contains(item))
            {
                if ((item.Quantity - count) > 0)
                {
                    item.Quantity -= count;
                    count -= item.Quantity;
                }
                else
                {
                    Delete(item);
                    return (true, 0);
                }
            }
            return (false, count);
        }
        public static (bool IsCompleted, int Remaining) InventoryRemove(ref LevelManager levelManager, string SystemName, int StartLVL = 1, int StopLVL = 1, int count = 1)
        {
            int Remaining = count;
            for (int i = StartLVL; i <= StopLVL; i++)
            {
                foreach (var item in levelManager.Items.FindAll(item => item.Lvl == i))
                {
                    if (item.SystemName == SystemName)
                    {
                        if ((Remaining - item.Quantity) > 0)
                        {
                            item.Quantity -= Remaining;
                            Remaining -= item.Quantity;
                        }
                        else
                        {
                            Delete(item);
                            return (true, 0);
                        }
                    }
                }
            }
            return (false, Remaining);
        }

        public static void AddPlayerInventory(AdvancedItem item)
        {
            item.IsInPlayerInventory = true;
            item.LevelManagerRef = InventoryObjectRef.GetComponent<PlayerInventory>().levelManager;//set ref
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.Items.Add(item);
            InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
        }
        public static void RemovePlayerInventory(AdvancedItem item)
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
                List<AdvancedItem> TemporaryItemList_ = new List<AdvancedItem>(levelManager.Items);
                foreach (AdvancedItem item in TemporaryItemList_)
                {
                    Delete(levelManager.Items.Find(i => i == item));
                }

                TemporaryItemList_.Clear();
                levelManager.Items.Clear();
            }

            levelManager = PlayerInventoryClone(LoadFrom);

            foreach (AdvancedItem item in levelManager.Items)
            {
                if (item.hotKeyRef != null)
                {
                    item.hotKeyRef.SetHotKey(item);
                }
            }
        }
        public static void PlayerInventoryDefault(ref LevelManager levelManager)
        {
            List<AdvancedItem> TemporaryItemList = new List<AdvancedItem>(levelManager.Items);
            foreach (AdvancedItem item in TemporaryItemList)
            {
                Delete(levelManager.Items.Find(i => i == item));
            }

            TemporaryItemList.Clear();
            levelManager.Items.Clear();

            AdvancedItem RootData = new()
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
        public static void NonLive_Positioning(int Y, int X, int sectorIndex, AdvancedItem item, AdvancedItem Parent)
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
        public static void Live_Positioning(AdvancedItem item, ItemSlot[] activeSlots)
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

        public static ((int X,int Y) ChangedSize, Dictionary<char,int> Directions) AdvancedItem_SizeChanger_EffectDetermination(AdvancedItem AdvancedItem,List<Part> IncomingParts,bool Add)
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
        public static (HashSet<(int Height, int Widht)> NonLiveCoordinates,int SectorIndex, bool IsPositionAble) Try_PartPositioning(AdvancedItem AdvancedItem, (int X, int Y) ChangedSize, Dictionary<char, int> Directions)
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
        public static void Live_Placing(AdvancedItem item, AdvancedItem PlacingInto)
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
        public static void Live_UnPlacing(AdvancedItem item)
        {
            foreach (ItemSlot slotObject in item.ItemSlotObjectsRef)
            {
                slotObject.PartOfItemObject = null;
            }
            item.ItemSlotObjectsRef.Clear();//unset ref
        }

        public static void NonLive_Placing(AdvancedItem item, AdvancedItem AddTo)
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
        public static void NonLive_UnPlacing(AdvancedItem item)
        {
            foreach (ItemSlotData slotData in item.ItemSlotsDataRef)
            {
                slotData.PartOfItemData = null;//remove
            }
            item.ItemSlotsDataRef.Clear();//unset ref
        }
        #endregion

        #region Status
        public static void HotKey_SetStatus_SupplementaryTransformation(AdvancedItem item, AdvancedItem StatusParent)
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
        public static void UnsetHotKey(AdvancedItem item)
        {
            if (item.hotKeyRef != null)
            {
                item.hotKeyRef.UnSetHotKey();
            }
        }
        #endregion

        #region Inventory-System Support Scripts
        private static (int X, int Y)[] Get_ItemCoodinateLine_AtDataGrid(AdvancedItem AdvancedItem, HashSet<(int X, int Y)> ExtendCoordinates,char Orientation)
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
        private static bool Try_UpWayScaling(AdvancedItem AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
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
        public static bool Try_DownWayScaling(AdvancedItem AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
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
        private static bool Try_RightWayScaling(AdvancedItem AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
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
        private static bool Try_LeftWayScaling(AdvancedItem AdvancedItem, ItemSlotData[,] NonLiveGrid, int ChangedSize, HashSet<(int X, int Y)> ExtendCoordinates)
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

        private static void StatusIsInPlayerInventory(AdvancedItem Data)
        {
            if (Data.Container != null)
            {
                foreach (AdvancedItem item in Data.Container.Items)
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
        private static void SetHierarhicLVL(AdvancedItem item, AdvancedItem Parent)
        {
            int lvl = Parent.Lvl;
            item.Lvl = ++lvl;
        }
        private static void AutoSetHotKey(AdvancedItem SetIn)
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
                Items = new List<AdvancedItem>()
            };

            // 1. Klónozzuk az összes itemet, és beállítjuk a LevelManager referenciát.
            foreach (AdvancedItem item in original.Items)
            {
                AdvancedItem clonedItem = item.ShallowClone();
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
        private static void CloneParts(AdvancedItem originalItem, AdvancedItem clonedItem)
        {
            clonedItem.Parts = new List<Part>();
            foreach (var partRef in originalItem.Parts)
            {
                // Klónozzuk a part-hoz tartozó itemet, majd létrehozunk egy új Part példányt.
                SimpleItem clonedPartItem = partRef.item_s_Part.ShallowClone();
                clonedItem.Parts.Add(new Part(clonedPartItem));
                clonedItem.Parts.Last().HierarhicPlace = partRef.HierarhicPlace;
            }
        }
        private static void SetupParentReference(LevelManager original, LevelManager clone, int index)
        {
            // Megkeressük az eredeti item parentjának indexét, majd a klónban beállítjuk a referenciát.
            AdvancedItem originalItem = original.Items[index];
            int parentIndex = original.Items.IndexOf(originalItem.ParentItem);
            clone.Items[index].ParentItem = clone.Items[parentIndex];
        }
        private static void SetupContainerReferences(LevelManager original, LevelManager clone, int index)
        {
            // Beállítjuk a container listát és a grid referenciákat.
            AdvancedItem clonedItem = clone.Items[index];
            AdvancedItem parent = clonedItem.ParentItem;

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
            AdvancedItem clonedItem = clone.Items[index];
            AdvancedItem originalItem = original.Items[index];

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
                        AdvancedItem item = original.Items.Find(item => !item.IsRoot && item.Parts.Contains(usedPart));
                        int itemIndex = original.Items.IndexOf(item);
                        int partIndex = original.Items[itemIndex].Parts.IndexOf(usedPart);
                        int cpIndex = Array.IndexOf(original.Items[itemIndex].Parts[partIndex].ConnectionPoints, cp.ConnectedPoint);

                        // Újracsatlakoztatjuk a klónban a connection pointokat.
                        clonedPart.ConnectionPoints[k].Connect(clone.Items[itemIndex].Parts[partIndex].ConnectionPoints[cpIndex]);
                    }
                }
            }
        }

        private static bool AddingByNewItemByRotate(LevelManager levelManager, int lvl, AdvancedItem Data)
        {
            Data.RotateDegree = 90;
            List<AdvancedItem> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.NonLive_Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(1) >= Data.SizeY && itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(0) >= Data.SizeX))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(0); Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(1); X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    Add(Data, itemsOfLvl[itemIndex]);
                                    NonLive_Positioning(Y, X, sectorIndex, Data, itemsOfLvl[itemIndex]);
                                    NonLive_Placing(Data, itemsOfLvl[itemIndex]);
                                    HotKey_SetStatus_SupplementaryTransformation(Data, itemsOfLvl[itemIndex]);
                                    //Debug.Log($"Item Added in container");
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            Data.RotateDegree = 0;
            return false;
        }
        private static bool AddingByNewItem(LevelManager levelManager, int lvl, AdvancedItem Data)
        {
            List<AdvancedItem> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl && Item.Container != null).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                for (int sectorIndex = 0; sectorIndex < itemsOfLvl[itemIndex].Container.NonLive_Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                {
                    if (itemsOfLvl[itemIndex].IsRoot || (itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(1) >= Data.SizeX && itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(0) >= Data.SizeY))
                    {
                        for (int Y = 0; Y < itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(0); Y++)//vegig iterálunk a sorokon
                        {
                            for (int X = 0; X < itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex].GetLength(1); X++)//a sorokon belul az oszlopokon
                            {
                                if ((itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].SlotType.Contains(Data.ItemType) || itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].SlotType == "") && itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex][Y, X].PartOfItemData == null && (CanBePlace(itemsOfLvl[itemIndex].Container.NonLive_Sectors[sectorIndex], Y, X, Data) || itemsOfLvl[itemIndex].IsRoot))//ha a slot nem tagja egy itemnek sem akkor target
                                {
                                    //Debug.Log($"Item Added in container");
                                    Add(Data, itemsOfLvl[itemIndex]);
                                    NonLive_Positioning(Y, X, sectorIndex, Data, itemsOfLvl[itemIndex]);
                                    NonLive_Placing(Data, itemsOfLvl[itemIndex]);
                                    HotKey_SetStatus_SupplementaryTransformation(Data, itemsOfLvl[itemIndex]);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        private static bool AddingByCount(LevelManager levelManager, int lvl, AdvancedItem Data)
        {
            bool ItemAdded = false;
            List<AdvancedItem> itemsOfLvl = levelManager.Items.Where(Item => Item.Lvl == lvl).ToList();
            for (int itemIndex = 0; itemIndex < itemsOfLvl.Count; itemIndex++)
            {
                if (!ItemAdded && itemsOfLvl[itemIndex].SystemName == Data.SystemName && itemsOfLvl[itemIndex].Quantity != itemsOfLvl[itemIndex].MaxStackSize)
                {
                    int originalCount = itemsOfLvl[itemIndex].Quantity;
                    itemsOfLvl[itemIndex].Quantity += Data.Quantity;
                    if (itemsOfLvl[itemIndex].Quantity > itemsOfLvl[itemIndex].MaxStackSize)
                    {
                        Data.Quantity -= (itemsOfLvl[itemIndex].MaxStackSize - originalCount);
                        itemsOfLvl[itemIndex].Quantity = itemsOfLvl[itemIndex].MaxStackSize;
                    }
                    else
                    {
                        ItemAdded = true;
                    }
                }
            }
            return ItemAdded;
        }
        private static bool CanBePlace(ItemSlotData[,] slots, int Y, int X, AdvancedItem item)
        {
            if (item.RotateDegree == 0 || item.RotateDegree == 180)
            {
                if (X + item.SizeX <= slots.GetLength(1) && Y + item.SizeY <= slots.GetLength(0))
                {
                    for (int y = Y; y < Y + item.SizeY; y++)
                    {
                        for (int x = X; x < X + item.SizeX; x++)
                        {
                            if (slots[y, x].PartOfItemData != null)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            else
            {
                if (X + item.SizeY <= slots.GetLength(1) && Y + item.SizeX <= slots.GetLength(0))
                {
                    for (int y = Y; y < Y + item.SizeX; y++)
                    {
                        for (int x = X; x < X + item.SizeY; x++)
                        {
                            if (slots[y, x].PartOfItemData != null)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}