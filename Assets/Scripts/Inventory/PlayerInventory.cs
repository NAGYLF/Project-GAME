using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Assets.Scripts;
using System;
using Weapons;
using Backpacks;
using Vests;
using Armors;
using System.Reflection;
using System.Linq;
using System.Drawing;
using MainData;
using static MainData.SupportScripts;
using PlayerInventoryVisualBuild;
using PlayerInventoryClass;
using static PlayerInventoryVisualBuild.PlayerInventoryVisual;
using UnityEngine.UI;



namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory playerInventoryData;//a player mindenhol elerheto inventoryja ez a gyökér mely referencia ként össze van kötve a playerInventoryVisual.cs - vel
        //a playerinventoryVisual equipmentenként osztja szét a root adatokat, így ezen adat az equipmetekig tart, utána az itemek egymás shyncronizálásáért felelnek 

        public Equipmnets equipments;//ez lényegében az inventory class adatai mivel a playerInventory többi része ezen változó kiszolgálásáért, mogosításáért felel vagy adatelérési antennaként (playerInventoryData) fumcionál

        public class Equipmnets
        {
            public List<EquipmnetStruct> equipmnetsData;
            public Equipmnets()//az equipmnets változó adatait itt példányosítjuk. az összes equipmentet alapbeallitottsággal példányosítjuk
            {
                //1.az equpmentek adatlistáját pédányosítjuk
                equipmnetsData = new List<EquipmnetStruct>();
                Transform transform = Resources.Load<GameObject>("GameElements/Equipment-Inventory").transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    //2. az equipment adatlistát feltöltjük az erőre megalkotot prefab adataival, továbbá létrehoz egy ures item példányt
                    equipmnetsData.Add(new EquipmnetStruct(transform.GetChild(i).GetComponent<EquipmentSlot>().SlotName, transform.GetChild(i).GetComponent<EquipmentSlot>().SlotType, new Item()));
                }
            }
            public class EquipmnetStruct
            {
                public string EquipmentSlotName;
                public string EquipmnetSlotType;
                public Item EquipmnetItem;
                public EquipmnetStruct(string name, string type, Item item)
                {
                    this.EquipmentSlotName = name;
                    this.EquipmnetSlotType = type;
                    this.EquipmnetItem = item;
                }
            }
        }

        private void Awake()//----------------------------------------------------------- ELSO LEPES AZ INVENTORY MEGALKOTASAKOR ---------------------------------------------------------------------------
        {
            //0. egy uj pedlanyt hozunk letre az equipmnetkenek
            equipments = new Equipmnets();
            //3. ha van inventory mentés akkor betoltjük azt
            InventoryLoad();
            //5. a player megkapja a vizualizációt az inventoryhozés megtörténik az adatpárosítás
            gameObject.AddComponent<PlayerInventoryVisual>().DataSynch(ref playerInventoryData);
        }
        public void InventorySave()
        {

        }
        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                equipments = JsonConvert.DeserializeObject<Equipmnets>(jsonString);
            }
            //4. ha nincs elmentett inventory akkor az eddig letrehozott ures inventory adatát a fő publikus playerInventoryData változó megkapja
            InventoryUpdate();
        }

        private void InventoryUpdate()
        {
            playerInventoryData = this;
        }




        public void InventoryAdd(Item item)
        {
            item.SetItem(item.ItemName);
            Debug.Log($"Add item: {item.ItemName}   in progress");
            bool ItemAdded = false;
            for (int i = 0; i < equipments.equipmnetsData.Count; i++)//equipment
            {
                Debug.Log($"Adding into equipmnets...    {i}   equipments.equipmnetsData[i].EquipmnetType:{equipments.equipmnetsData[i].EquipmnetSlotType} =?= item.ItemType:{item.ItemType}   equipments.equipmnetsData[i].EquipmnetItem.ItemName:{equipments.equipmnetsData[i].EquipmnetItem.ItemName == null}");
                if (equipments.equipmnetsData[i].EquipmnetSlotType.Contains(item.ItemType) && equipments.equipmnetsData[i].EquipmnetItem.ItemName == null)
                {
                    item.SlotUse = new string[] { equipments.equipmnetsData[i].EquipmentSlotName };
                    equipments.equipmnetsData[i].EquipmnetItem = item;
                    ItemAdded = true;
                    InventoryUpdate();
                    Debug.Log($"item: {item.ItemName} added in equipment: {equipments.equipmnetsData[i].EquipmentSlotName}");
                    break;
                }
            }
            if (!ItemAdded)//container
            {
                for (int equpmentIndex = 0; equpmentIndex < equipments.equipmnetsData.Count; equpmentIndex++)//vegig iterálunk az osszes equipmenten
                {
                    if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem == null) { Debug.LogError($"EquipmnetItem == null"); };
                    if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container != null)//ha az equipmnetnek nincs containerje akkor kihadjuk
                    {
                        if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors == null) { Debug.LogError($"EquipmnetItem.Container.Sectrors == null"); };
                        for (int sectorIndex = 0; sectorIndex < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                        {
                            if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(1) >= item.SizeX && equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(0) >= item.SizeY)//egy gyors ellenörzést végzünk, hogy az itemunk a feltetelezett teljesen ures sectorba belefér e, ha nem kihadjuk
                            {
                                Debug.Log($"AZ item elfér X: {item.SizeX}   Y: {item.SizeY}   SectorIndex: {sectorIndex}");
                                for (int Y = 0; Y < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; Y++)//vegig iterálunk a sorokon
                                {
                                    for (int X = 0; X < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; X++)//a sorokon belul az oszlopokon
                                    {
                                        if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][Y, X].PartOfItem == null && CanBePlace(equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex], Y, X, item))//ha a slot nem tagja egy itemnek sem akkor target
                                        {
                                            int index = 0;
                                            item.SlotUse = new string[item.SizeX * item.SizeY];
                                            for (int y = Y; y < Y + item.SizeY; y++)
                                            {
                                                for (int x = X; x < X + item.SizeX; x++)
                                                {
                                                    equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][y, x].PartOfItem = item;
                                                    item.SlotUse[index] = equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][y, x].name;//ez alapjan azonositunk egy itemslotot
                                                    index++;
                                                }
                                            }
                                            item.SetSlotUseId();
                                            equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Items.Add(item);
                                            InventoryUpdate();
                                            ItemAdded = true;
                                            Debug.Log($"Item Added in container");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!ItemAdded)
                {
                    Debug.LogWarning($"item: {item.ItemName} cannot added, probably no space for that");
                }
            }
        }
        public void InventoryDelete(Item item)
        {

        }
        public void InventoryModify(Item item)
        {

        }
        private bool CanBePlace(ItemSlot[,] slots, int Y, int X, Item item)
        {
            if (X + item.SizeX <= slots.GetLength(1) && Y + item.SizeY <= slots.GetLength(0))
            {
                for (int y = Y; y < Y + item.SizeY; y++)
                {
                    for (int x = X; x < X + item.SizeY; x++)
                    {
                        if (slots[y, x].PartOfItem != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
    };
}



namespace ItemHandler
{
    public class ItemObject : MonoBehaviour
    {
        public Item ActualData;

        private GameObject VirtualParentObject;//azon objektum melynek a friss adatszinkronizációért felel

        private bool isDragging = false;

        private Vector3 originalPosition;
        private Vector2 originalSize;

        private Transform originalParent;

        private Vector2 originalPivot;
        private Vector2 originalAnchorMin;
        private Vector2 originalAnchorMax;
        public PlacerStruct placer { private get; set; }
        List<GameObject> itemSlots { get; set; }
        public struct PlacerStruct
        {

            public List<GameObject> activeItemSlots { get; set; }
            public GameObject newStarter { get; set; }
        }
        private bool CanBePlace()
        {
            //az itemslotok szama egynelo az item meretevel és mindegyik slot ugyan abban a sectorban van     vagy a placer aktiv slotjaiban egy elem van ami egy equipmentslot
            //Debug.Log(placer.activeItemSlots.First().name);
            if (placer.activeItemSlots != null && placer.activeItemSlots.Count == 1 && placer.activeItemSlots.First().name.Contains("EquipmentSlot"))
            {
                if (placer.activeItemSlots.First().GetComponent<EquipmentSlot>().PartOfItemObject != null && placer.activeItemSlots.First().GetComponent<EquipmentSlot>().PartOfItemObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    return false;
                }
                return true;
            }
            else if (placer.activeItemSlots != null && placer.activeItemSlots.Count == ActualData.SizeX * ActualData.SizeY && placer.activeItemSlots.Count == placer.activeItemSlots.FindAll(elem => elem.GetComponent<ItemSlot>().Sector == placer.activeItemSlots.First().GetComponent<ItemSlot>().Sector).Count)
            {
                for (int i = 0; i < placer.activeItemSlots.Count; i++)
                {
                    //az itemslototk itemobject tartalma vagy null vagy az itemobjectum maga
                    if (placer.activeItemSlots[i].GetComponent<ItemSlot>().PartOfItemObject != null && placer.activeItemSlots[i].GetComponent<ItemSlot>().PartOfItemObject.GetInstanceID() != gameObject.GetInstanceID())
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /*
        public void Rotate(int deg)
        {
            switch (deg)
            {
                case 0:

                default:
                    break;
            }
        }
        */
        public void Delete()
        {

        }
        private void ObjectMovement()
        {
            if (isDragging)
            {
                // Az egér pozíciójának lekérése a világkoordináta rendszerben
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                // Az objektum új pozíciójának beállítása (csak az X és Y, hogy a Z tengely ne változzon)
                transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
            }
        }
        private void OnMouseDown()
        {
            // Ekkor indul a mozgatás, ha rákattintunk az objektumra
            #region Save Original Object Datas
            originalPosition = transform.position;
            originalSize = transform.GetComponent<RectTransform>().sizeDelta;
            originalParent = transform.parent;
            originalPivot = transform.GetComponent<RectTransform>().pivot;
            originalAnchorMin = transform.GetComponent<RectTransform>().anchorMin;
            originalAnchorMax = transform.GetComponent<RectTransform>().anchorMax;
            #endregion

            #region Set Moveable position
            transform.SetParent(InventoryObject.transform,false);
            transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(ActualData.SizeX * Main.SectorScale * Main.DefaultItemSlotSize, ActualData.SizeY * Main.SectorScale * Main.DefaultItemSlotSize);
            #endregion

            #region Set Sprite
            RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();
            SpriteRenderer itemObjectSpriteRedner = gameObject.GetComponent<SpriteRenderer>();
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
            itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
            #endregion

            #region Set Targeting Mode 
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region Set Dragable mod
            SlotObject.GetComponent<PanelSlots>().Scrollbar.GetComponent<ScrollRect>().enabled = false;
            isDragging = true;
            #endregion
        }
        private void OnMouseUp()
        {
            // Mozgatás leállítása, amikor elengedjük az egeret
            #region unSet Moveable position
            transform.SetParent(originalParent, false);
            #endregion

            #region Load Original Object Datas
            transform.GetComponent<RectTransform>().pivot = originalPivot;
            transform.GetComponent<RectTransform>().anchorMin = originalAnchorMin;
            transform.GetComponent<RectTransform>().anchorMax = originalAnchorMax;
            transform.position = originalPosition;
            transform.GetComponent<RectTransform>().sizeDelta = originalSize;
            #endregion

            #region Set Sprite
            RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();
            SpriteRenderer itemObjectSpriteRedner = gameObject.GetComponent<SpriteRenderer>();
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
            itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
            #endregion

            #region unSet Targeting Mode 
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region unSet Dragable mod
            SlotObject.GetComponent<PanelSlots>().Scrollbar.GetComponent<ScrollRect>().enabled = true;
            isDragging = false;
            #endregion

            Placing(CanBePlace());
        }
        private void Placing(bool placementCanStart)
        {
            if (placementCanStart)
            {
                Debug.Log(placer.activeItemSlots.Count);
                if (VirtualParentObject.GetInstanceID() != placer.newStarter.GetInstanceID())//új VPO (VirtualParentObject)
                {
                    if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
                    {
                        VirtualParentObject.GetComponent<EquipmentSlot>().DataOut(ActualData, gameObject);
                        EquipmentSlot equipmentSlot = VirtualParentObject.GetComponent<EquipmentSlot>();
                        equipmentSlot.PartOfItemObject = null;
                    }
                    else
                    {
                        VirtualParentObject.GetComponent<ContainerObject>().DataOut(ActualData, gameObject);
                        ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                        for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                        {
                            for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                            {
                                if (!placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = null;
                                }
                            }
                        }
                    }
                    VirtualParentObject = placer.newStarter;
                    if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
                    {
                        ActualData.SlotUse = new string[] {VirtualParentObject.GetComponent<EquipmentSlot>().SlotName};
                        VirtualParentObject.GetComponent<EquipmentSlot>().DataIn(ActualData, gameObject);
                        SelfVisualisation();
                    }
                    else
                    {
                        ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                        itemSlots.Clear();
                        for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                        {
                            for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                            {
                                if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && !ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                    itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                                }
                                else if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                    itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                                }
                            }
                        }
                        ActualData.SlotUse = new string[itemSlots.Count];
                        Debug.Log($" ActualData itemslotuse { ActualData.SlotUse.Length}             itemslot {itemSlots.Count}");
                        for (int i = 0; i < ActualData.SlotUse.Length; i++)
                        {
                            ActualData.SlotUse[i] = itemSlots[i].name;
                        }
                        VirtualParentObject.GetComponent<ContainerObject>().DataIn(ActualData, gameObject);
                        SelfVisualisation();
                    }
                }
                else
                {
                    if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
                    {
                        VirtualParentObject.GetComponent<EquipmentSlot>().DataUpdate(ActualData, gameObject);
                        SelfVisualisation();
                    }
                    else
                    {
                        ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                        itemSlots.Clear();
                        for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                        {
                            for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                            {
                                if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                                }
                                else if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && !ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                    itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                                }
                                else if (!placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = null;
                                }
                            }
                        }
                        ActualData.SlotUse = new string[itemSlots.Count];
                        for (int i = 0; i < ActualData.SlotUse.Length; i++)
                        {
                            ActualData.SlotUse[i] = itemSlots[i].name;
                        }
                        VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData, gameObject);
                        SelfVisualisation();
                    }
                }
            }
        }
        private void Start()
        {
            DataLoad();
        }
        private void Update()
        {
            ObjectMovement();
        }
        #region Data Synch
        public void DataIn(Item Data, GameObject VirtualChildObject)
        {
            ActualData = Data;
            SelfVisualisation();
            if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
            {
                VirtualParentObject.GetComponent<EquipmentSlot>().DataUpdate(ActualData, gameObject);
            }
            else
            {
                VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData, gameObject);
            }
        }
        public void DataOut(Item Data, GameObject VirtualChildObject)
        {
            ActualData = Data;
            SelfVisualisation();
            if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
            {
                VirtualParentObject.GetComponent<EquipmentSlot>().DataUpdate(ActualData,gameObject);
            }
            else
            {
                VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData,gameObject);
            }
        }
        public void DataUpdate(Item Data, GameObject VirtualChildObject)
        {
            ActualData = Data;
            SelfVisualisation();
            if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
            {
                VirtualParentObject.GetComponent<EquipmentSlot>().DataUpdate(ActualData, gameObject);
            }
            else
            {
                VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData, gameObject);
            }
        }
        public void SetDataRoute(Item Data, GameObject VirtualParentObject)
        {
            ActualData = Data;
            this.VirtualParentObject = VirtualParentObject;
        }
        public void DataLoad()
        {
            itemSlots = new List<GameObject>();
            SelfVisualisation();
            if (ActualData.Container != null /*&& !Array.Exists(ActualData.SlotUse,elem=>elem.Contains("EquipmentSlot"))*/)//11. ha az item adatai tartalmaznak containert akkor az létrejön
            {
                //--> ContainerObject.cs
                Debug.LogWarning($"{ActualData.ItemName} ItemObject ------- ref --------> ContainerObject.cs");
                GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
                containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData, gameObject);
            }
        }
        #endregion
        private void SelfVisualisation()//ha az item equipment slotban van
        {
            Rigidbody2D itemObjectRigibody2D;
            if (gameObject.GetComponent<Rigidbody2D>() == null) 
            {
                itemObjectRigibody2D = gameObject.AddComponent<Rigidbody2D>();
            }
            else
            {
                itemObjectRigibody2D = gameObject.GetComponent<Rigidbody2D>();
            }
            itemObjectRigibody2D.mass = 0;
            itemObjectRigibody2D.drag = 0;
            itemObjectRigibody2D.angularDrag = 0;
            itemObjectRigibody2D.gravityScale = 0;
            itemObjectRigibody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            itemObjectRigibody2D.bodyType = RigidbodyType2D.Static;

            RectTransform itemObjectRectTransform;
            if (gameObject.GetComponent<RectTransform>() == null)
            {
                itemObjectRectTransform = gameObject.AddComponent<RectTransform>();
            }
            else
            {
                itemObjectRectTransform = gameObject.GetComponent<RectTransform>();
            }

            SpriteRenderer itemObjectSpriteRedner;
            if (gameObject.GetComponent<SpriteRenderer>() == null)
            {
                itemObjectSpriteRedner = gameObject.AddComponent<SpriteRenderer>();
            }
            else
            {
                itemObjectSpriteRedner = gameObject.GetComponent<SpriteRenderer>();
            }
            itemObjectSpriteRedner.sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja képét
            itemObjectSpriteRedner.drawMode = SpriteDrawMode.Sliced;

            if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
            {
                Debug.Log($"{ActualData.ItemName}: Equipment visualisation");
                RectTransform EquipmentSlot = VirtualParentObject.GetComponent<RectTransform>();

                gameObject.transform.SetParent(EquipmentSlot.transform.parent, false);//itemObj parent set

                itemObjectRectTransform.localPosition = new Vector3(EquipmentSlot.localPosition.x, EquipmentSlot.localPosition.y, 0);
                itemObjectRectTransform.anchorMin = EquipmentSlot.anchorMin;
                itemObjectRectTransform.anchorMax = EquipmentSlot.anchorMax;
                itemObjectRectTransform.pivot = EquipmentSlot.pivot;
                itemObjectRectTransform.offsetMin = Vector2.zero;
                itemObjectRectTransform.offsetMax = Vector2.zero;

                float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
                itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
            }
            else if (VirtualParentObject.GetComponent<ContainerObject>() != null)//ha az item containerben van
            {
                Debug.Log($"{ActualData.ItemName}: Slot visualisation     {gameObject.GetComponent<RectTransform>().localPosition}");
                ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                itemSlots.Clear();
                for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                {
                    for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                    {
                        if (ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                        {
                            itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                        }
                    }
                }
                for (int i = 0; i < ActualData.SlotUse.Length; i++)
                {
                    ActualData.SlotUse[i] = itemSlots[i].name;
                }
                // Alapértelmezett kezdőértékek (értelmesen kiszámítva)
                Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

                // Végigmegy az összes itemSlot-on és kiszámítja a minimális és maximális pozíciókat
                foreach (GameObject itemSlot in itemSlots)
                {
                    // Az itemSlot helyi pozíciója a gameObject szülőhöz képest
                    Vector3 slotLocalPos = itemSlot.GetComponent<RectTransform>().localPosition;
                    Vector2 slotMin = slotLocalPos - (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;
                    Vector2 slotMax = slotLocalPos + (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;

                    // Beállítja a minimális és maximális pontokat az összes itemSlot lefedésére
                    minPos = Vector2.Min(minPos, slotMin);
                    maxPos = Vector2.Max(maxPos, slotMax);
                }

                // Méret kiszámítása és a szülő objektum méretének beállítása
                Vector2 newSize = maxPos - minPos;

                gameObject.transform.SetParent(itemSlots[0].transform.parent, false);
                // A szülő objektum pivotját középre állítja a pontos igazításhoz
                itemObjectRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                itemObjectRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                itemObjectRectTransform.pivot = new Vector2(0.5f, 0.5f);
                itemObjectRectTransform.offsetMin = Vector2.zero;
                itemObjectRectTransform.offsetMax = Vector2.zero;
                itemObjectRectTransform.sizeDelta = newSize;
                itemObjectRectTransform.localPosition = (Vector3)((maxPos + minPos) / 2f);

                // Új pozíció beállítása úgy, hogy a szülő lefedje az itemSlots összes elemét

                float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
                itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
                Debug.Log($"{ActualData.ItemName}:  position     {gameObject.GetComponent<RectTransform>().localPosition}");
            }
            else
            {
                Debug.LogError($"{ActualData.ItemName}: non visualisation");
            }

            BoxCollider2D itemObjectBoxCollider2D;
            if (gameObject.GetComponent<BoxCollider2D>() == null)
            {
                itemObjectBoxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            }
            else
            {
                itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
            }
            itemObjectBoxCollider2D.autoTiling = true;
            itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;
        }
    }

    public class Item : NonGeneralItemProperties
    {
        //general
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string[] SlotUse { get; set; }
        private string SlotUseId;
        public string GetSlotUseId()
        {
            return SlotUseId;
        }
        public void SetSlotUseId()
        {
            string id = "";
            for (int i = 0; i < SlotUse.Length; i++)
            {
                id += SlotUse[i];
            }
            SlotUseId = id;
        }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        //public int Rotated { get; set; }
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;//ez alapján kerülhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusváltozójában benen kell, hogy legyen.
            ItemName = source.ItemName;//ez alapján hozza létre egy item saját magát
            Description = source.Description;
            Quantity = source.Quantity;
            SlotUse = source.SlotUse;// ez a jelenleg elfoglalt helye, ezt a betolteskor hasznaljuk, hogy tudjuk mit hova raktunk el.
            ImgPath = source.ImgPath;
            SizeX = source.SizeX;
            SizeY = source.SizeY;
            //tartalom
            Container = source.Container;//tartalom
            //fegyver adatok
            DefaultMagasineSize = source.DefaultMagasineSize;
            Spread = source.Spread;
            Rpm = source.Rpm;
            Recoil = source.Recoil;
            Accturacy = source.Accturacy;
            Range = source.Range;
            Ergonomy = source.Ergonomy;
            BulletType = source.BulletType;
            Accessors = source.Accessors;
            //felhasznalhato e?
            usable = source.usable;
        }
        public void SetItem(string name)
        {
            Item completedItem = name switch
            {
                "TestWeapon" => new TestWeapon().Set(),
                "TestBackpack" => new TestBackpack().Set(),
                "TestVest" => new TestVest().Set(),
                "TestArmor" => new TestArmor().Set(),
                "TestHandgun" => new TestHandgun().Set(),
                /*
                "TestHelmet" => new TestVest().Set(),
                "TestFingers" => new TestVest().Set(),
                "TestBoots" => new TestVest().Set(),
                "TestMask" => new TestVest().Set(),
                "TestHeadset" => new TestVest().Set(),
                "TestSkin" => new TestVest().Set(),
                "TestPant" => new TestVest().Set(),
                "TestMelee" => new TestVest().Set(),
                */
                _ => throw new ArgumentException("Invalid type")
            };
            CopyProperties(completedItem);
            Debug.Log($"Item created {this}");
        }
        public Item() { }
        public Item(string name)// egy itemet mindeg név alapjan peldanyositunk
        {
            SetItem(name);
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
        public BulletType BulletType { get; set; }
        public Accessors Accessors { get; set; }
        //usable
        public bool usable { get; set; } = false;
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
        public List<Item> Items {  get; set; }
        public string PrefabPath;
        public ItemSlot[][,] Sectors { get; set; }
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            Items = new List<Item>();
            GameObject[] sectors = Resources.Load(prefabPath).GetComponent<ContainerObject>().SectorManagers;
            Sectors = new ItemSlot[sectors.Length][,];
            for (int i = 0; i < Sectors.Length; i++)
            {
                int index = 0;
                Sectors[i] = new ItemSlot[sectors[i].GetComponent<SectorManager>().row, sectors[i].GetComponent<SectorManager>().columb];
                for (int row = 0; row < Sectors[i].GetLength(0); row++)
                {
                    for (int col = 0; col < Sectors[i].GetLength(1); col++)
                    {
                        Debug.Log($"index: {index} {sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>().name}     R{row}   C{col}");
                        Sectors[i][row, col] = sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>();
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
}










namespace PlayerInventoryVisualBuild
{
    public class PlayerInventoryVisual : MonoBehaviour
    {
        private bool InventoryOpen = false;

        [HideInInspector] public static GameObject InventoryObject;//az invenory fő objektumának tárolásáért fele

        private PlayerInventory playerInventory;

        [HideInInspector] public static GameObject EquipmentsObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject SlotObject;//az inventory 3 alsóbrendűbb objektumának egyike
        [HideInInspector] public static GameObject LootObject;//az inventory 3 alsóbrendűbb objektumának egyike
        private void Update()
        {
            //6. ebben az update metódusban az inventory be és ki kapcsolasat figyeli
            OpenCloseInventory();
        }
        public void DataSynch(ref PlayerInventory playerInventory)
        {
            this.playerInventory = playerInventory;
        }
        public void OpenCloseInventory()//ez az inventoryt epiti fel
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (InventoryOpen)
                {
                    InventoryOpen = false;
                    Destroy(InventoryObject);
                }
                else
                {
                    InventoryOpen = true;
                    EquipmentInitialisation();
                }
            }
        }
        private void EquipmentInitialisation()//7. ha az inventory megnyitódik akkor az inventory adatai felépítik az inventoryt
        {
            GameObject UI = GameObject.FindGameObjectWithTag("InGameUI");
            InventoryObject = new GameObject("Inventory");

            if (UI != null)
            {
                InventoryObject.transform.SetParent(UI.transform, false);

                InventoryObject.AddComponent<RectTransform>().localPosition = new Vector3(0, 0, UI.transform.position.z);
            }
            else
            {
                Debug.LogError("UI nem található!");
            }
            if (playerInventory == null || playerInventory.equipments.equipmnetsData == null)
            {
                Debug.LogError("Equipmnets vagy EquipmnetsData null!");
            }

            float[] aranyok = Aranyszamitas(new float[] { 6, 5, 6 }, Main.DefaultWidth);

            EquipmentsObject = CreatePrefab("GameElements/Equipment-Inventory");
            EquipmentsObject.transform.SetParent(InventoryObject.transform);
            EquipmentsObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
            EquipmentsObject.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1] / 2) * -1, 0, 0);
            PanelEquipments panelEquipments = EquipmentsObject.GetComponent<PanelEquipments>();

            SlotObject = CreatePrefab("GameElements/Slots-Inventory");
            SlotObject.transform.SetParent(InventoryObject.transform);
            SlotObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
            SlotObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] * -1 / 2, 0, 0);
            PanelSlots panelSlots = SlotObject.GetComponent<PanelSlots>();

            LootObject = CreatePrefab("GameElements/Loot-Inventory");
            LootObject.transform.SetParent(InventoryObject.transform);
            LootObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
            LootObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);

            for (int i = 0; i < panelEquipments.EquipmentsSlots.Length; i++)
            {
                for (int j = 0; j < playerInventory.equipments.equipmnetsData.Count; j++)
                {
                    if (playerInventory.equipments.equipmnetsData[j].EquipmnetItem.SlotUse != null && playerInventory.equipments.equipmnetsData[j].EquipmnetItem.SlotUse.Contains(panelEquipments.EquipmentsSlots[i].name))
                    {
                        //--> EquipmnetSlot.cs
                        //8. az inventory equipmnetjei egyesével innin indulnak ki, ez a vizualizáció és az adatáramlás láncreakciószerű kiindulásipontja.
                        Debug.LogWarning($"{playerInventory.equipments.equipmnetsData[j].EquipmnetItem.ItemName} PlayerInventory.cs ------- ref --------> EquipmentSlot.cs");
                        //9. az equipmnetSlot objektum komponense referál egy equipmnet változó item adatára
                        panelEquipments.EquipmentsSlots[i].GetComponent<EquipmentSlot>().SetRootDataRoute(ref playerInventory.equipments.equipmnetsData[j].EquipmnetItem);
                    }
                }
            }
        }
    };
}