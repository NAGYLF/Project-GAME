using Assets.Scripts;
using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;
using static MainData.SupportScripts;
using static PlayerInventoryVisualBuild.PlayerInventoryVisual;
using TMPro;
using System;
using Unity.VisualScripting;

public class ItemObject : MonoBehaviour
{
    public GameObject Counter;
    public Item ActualData { get; private set; }
    private GameObject VirtualParentObject;//azon objektum melynek a friss adatszinkronizációért felel

    private Transform originalParent;
    private Vector3 originalPosition;
    private Vector2 originalSize;
    private Vector2 originalPivot;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private float originalRotation;//ezt kivetelesen nem az onMouseDown eljarasban hasznaljuk hanem a placing eljaras azon else agaban amely a CanBePlacing false agan helyezkedik el.
    List<GameObject> itemSlots { get; set; }//az itemlsotok pillanatnyi eltarolasara van szükség

    private bool isDragging = false;
    public PlacerStruct placer { private get; set; }
    public struct PlacerStruct
    {
        public List<GameObject> activeItemSlots { get; set; }
        public GameObject NewVirtualParentObject { get; set; }
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
        originalRotation = ActualData.RotateDegree;
        #endregion

        #region Set Moveable position
        transform.SetParent(InventoryObject.transform, false);
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
        Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize;
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
        Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize / Main.SectorScale;
        isDragging = false;
        #endregion



        Placing(CanBePlace());
    }
    private (int smaller, int larger) SplitInteger(int number)
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
    private void Placing(bool placementCanStart)
    {
        if (Input.GetKey(KeyCode.LeftControl) && placer.activeItemSlots.FindAll(slot=> ActualData.GetSlotUseId().Contains(slot.name)).Count != ActualData.SlotUse.Length)//split
        {
            (int smaller, int larger) SplitedCount = SplitInteger(ActualData.Quantity);

            if (placer.activeItemSlots.Exists(slot => slot.GetComponent<ItemSlot>() != null && slot.GetComponent<ItemSlot>().CoundAddAvaiable))//split and megre
            {
                GameObject MergeObject = placer.activeItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CoundAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
                MergeObject.GetComponent<ItemObject>().ActualData.Quantity += SplitedCount.larger;
                ActualData.Quantity = SplitedCount.smaller;
                if (MergeObject.GetComponent<ItemObject>().ActualData.Quantity > MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize)//ha a split több mint a maximalis stacksize
                {
                    ActualData.Quantity += (MergeObject.GetComponent<ItemObject>().ActualData.Quantity - MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize);
                    MergeObject.GetComponent<ItemObject>().ActualData.Quantity = MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize;
                    MergeObject.GetComponent<ItemObject>().SelfVisualisation();
                    SelfVisualisation();
                    placementCanStart = false;
                }
                else//ha nem több a split mint a maximális stacksize
                {
                    ActualData.Quantity = SplitedCount.smaller;
                    MergeObject.GetComponent<ItemObject>().SelfVisualisation();
                    placementCanStart = false;
                    if (ActualData.Quantity<1)
                    {
                        SelfDestruction();
                    }
                    else
                    {
                        SelfVisualisation();
                    }
                }
            }
            else if (placer.activeItemSlots.Count == ActualData.SizeY * ActualData.SizeX)//egy specialis objektumlétrehozási folyamat
            {
                Item item = new Item(ActualData.ItemName, SplitedCount.larger);
                item.SlotUse = new string[placer.activeItemSlots.Count];
                for (int i = 0; i < item.SlotUse.Length; i++)
                {
                    item.SlotUse[i] = placer.activeItemSlots[i].name;
                }
                GameObject itemObject = CreatePrefab("GameElements/ItemObject");
                itemObject.name = ActualData.ItemName;
                itemObject.GetComponent<ItemObject>().SelfBuild(item , placer.NewVirtualParentObject);
                ActualData.Quantity = SplitedCount.smaller;
                placementCanStart = false;
                if (ActualData.Quantity < 1)
                {
                    SelfDestruction();
                }
                else
                {
                    SelfVisualisation();
                }
            }
        }
        #region Item Merge
        else if (placer.activeItemSlots.Exists(slot=>slot.GetComponent<ItemSlot>() != null && slot.GetComponent<ItemSlot>().CoundAddAvaiable))//csak containerekben mukodik
        {
            GameObject MergeObject = placer.activeItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CoundAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
            int count = MergeObject.GetComponent<ItemObject>().ActualData.Quantity;
            MergeObject.GetComponent<ItemObject>().ActualData.Quantity += ActualData.Quantity;
            if (MergeObject.GetComponent<ItemObject>().ActualData.Quantity > MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize)
            {
                MergeObject.GetComponent<ItemObject>().ActualData.Quantity = MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize;
                ActualData.Quantity -= (ActualData.Quantity - count);
                MergeObject.GetComponent<ItemObject>().SelfVisualisation();
                SelfVisualisation();
                placementCanStart = false;
            }
            else
            {
                MergeObject.GetComponent<ItemObject>().SelfVisualisation();
                placementCanStart = false;
                SelfDestruction();
            }
        }
        #endregion
        if (placementCanStart)
        {
            Debug.Log(placer.activeItemSlots.Count);
            if (VirtualParentObject.GetInstanceID() != placer.NewVirtualParentObject.GetInstanceID())//új VPO (VirtualParentObject)
            {
                if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
                {
                    VirtualParentObject.GetComponent<EquipmentSlot>().DataOut(ActualData);
                }
                else
                {
                    VirtualParentObject.GetComponent<ContainerObject>().DataOut(ActualData);
                    ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                    for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                    {
                        for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                        {
                            if (ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                            {
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = null;
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemData = null;
                            }
                        }
                    }
                }
                VirtualParentObject = placer.NewVirtualParentObject;
                if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
                {
                    ActualData.SlotUse = new string[] { VirtualParentObject.GetComponent<EquipmentSlot>().SlotName };
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
                            if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]))
                            {
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                            }
                        }
                    }
                    ActualData.SlotUse = new string[itemSlots.Count];
                    Debug.Log($" ActualData itemslotuse {ActualData.SlotUse.Length}             itemslot {itemSlots.Count}");
                    for (int i = 0; i < ActualData.SlotUse.Length; i++)
                    {
                        ActualData.SlotUse[i] = itemSlots[i].name;
                    }
                    VirtualParentObject.GetComponent<ContainerObject>().DataIn(ActualData);
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
                            //ha az aktiv slotok küzül az egyik az iterált slot      és ezt a slotot tartalmazza még ez az item actualdataja is.    az azt jelenti, hogy ezen slot ugyan ugy az itemobjektum része maradt
                            if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                            {
                                //ezert hozzadjuk az itemSlots listához az itemslot objektumot
                                itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                            }
                            //ha az aktiv slotok küzül az egyik az iterált slot      de ezt nem tartalmazza az item actualdataja        az azt jelenti, hogy ezen itemslot egy uj része lett az itemobjektumnak
                            else if (placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && !ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                            {
                                //ezen uj slot része lesz az itemobjektumnak és annak adatának      illetve hozzáadjuk az itemslot listához
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemData = ActualData;
                                itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                            }
                            //ha ezen iterált slot nem része az aktív slotoknak       de része az item actualdata-jának       az azt jelenti, hogy ez egy régi slot az itemobjektumnak
                            else if (!placer.activeItemSlots.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]) && ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))
                            {
                                //ezt a slotot megfosztjuk az itemobjektumtól és annak adatától
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = null;
                                containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemData = null;
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
        else
        {
            ActualData.RotateDegree = originalRotation;
            SelfVisualisation();
        }
    }
    private void Start()
    {
        DataLoad();
    }
    private void Update()
    {
        ObjectMovement();
        Rotation();
    }
    private void Rotation()
    {
        if (Input.GetKeyUp(KeyCode.R) && isDragging)
        {
            ActualData.RotateDegree = transform.rotation.eulerAngles.z + 90;
            if (ActualData.RotateDegree == 360f)
            {
                ActualData.RotateDegree = 0f;
            }
            transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);
        }
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
    #region Data Synch
    public void SelfDestruction()//egy gombal hivhatod meg.
    {
        if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
        {
            VirtualParentObject.GetComponent<EquipmentSlot>().DataOut(ActualData);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataOut(ActualData);
        }
        Destroy(gameObject);
    }
    public void SelfBuild(Item Data, GameObject VirtualParentObject)
    {
        ActualData = Data;
        this.VirtualParentObject = VirtualParentObject;
        if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
        {
            VirtualParentObject.GetComponent<EquipmentSlot>().DataIn(ActualData,gameObject);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataIn(ActualData);
        }
    }
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
            VirtualParentObject.GetComponent<EquipmentSlot>().DataUpdate(ActualData, gameObject);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData, gameObject);
        }
    }
    public void DataUpdate(Item Data)
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
        if (ActualData.Container != null)//11. ha az item adatai tartalmaznak containert akkor az létrejön
        {
            //--> ContainerObject.cs
            Debug.LogWarning($"{ActualData.ItemName} ItemObject ------- ref --------> ContainerObject.cs");
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData, gameObject);
        }
        DataUpdate(ActualData);
    }
    #endregion
    private void SelfVisualisation()//ha az item equipment slotban van
    {
        if (ActualData.Quantity == 1)
        {
            Counter.GetComponent<TextMeshPro>().text = "";
        }
        else
        {
            Counter.GetComponent<TextMeshPro>().text = ActualData.Quantity.ToString();
        }

        Rigidbody2D itemObjectRigibody2D = gameObject.GetComponent<Rigidbody2D>();

        itemObjectRigibody2D.mass = 0;
        itemObjectRigibody2D.drag = 0;
        itemObjectRigibody2D.angularDrag = 0;
        itemObjectRigibody2D.gravityScale = 0;
        itemObjectRigibody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        itemObjectRigibody2D.bodyType = RigidbodyType2D.Static;

        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();

        SpriteRenderer itemObjectSpriteRedner = gameObject.GetComponent<SpriteRenderer>();
        itemObjectSpriteRedner.sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja képét
        itemObjectSpriteRedner.drawMode = SpriteDrawMode.Sliced;

        if (VirtualParentObject.GetComponent<EquipmentSlot>() != null)
        {
            Debug.Log($"{ActualData.ItemName}: Equipment visualisation");
            RectTransform EquipmentSlot = VirtualParentObject.GetComponent<RectTransform>();

            gameObject.transform.SetParent(EquipmentSlot.transform.parent, false);//itemObj parent set

            itemObjectRectTransform.localPosition = new Vector3(EquipmentSlot.localPosition.x, EquipmentSlot.localPosition.y, 0);
            itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX * Main.DefaultItemSlotSize, ActualData.SizeY * Main.DefaultItemSlotSize);
            itemObjectRectTransform.anchorMin = EquipmentSlot.anchorMin;
            itemObjectRectTransform.anchorMax = EquipmentSlot.anchorMax;
            itemObjectRectTransform.pivot = EquipmentSlot.pivot;
            itemObjectRectTransform.offsetMin = Vector2.zero;
            itemObjectRectTransform.offsetMax = Vector2.zero;

            ActualData.RotateDegree = 0;

            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
            itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);

            ActualData.SlotUse = new string[] { VirtualParentObject.name };
            VirtualParentObject.GetComponent<EquipmentSlot>().PartOfItemObject = gameObject;

            Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize;
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
                        containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                    }
                }
            }
            ActualData.SlotUse = new string[itemSlots.Count];
            for (int i = 0; i < ActualData.SlotUse.Length; i++)
            {
                ActualData.SlotUse[i] = itemSlots[i].name;
            }
            // Alapértelmezett kezdõértékek (értelmesen kiszámítva)
            Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

            // Végigmegy az összes itemSlot-on és kiszámítja a minimális és maximális pozíciókat
            foreach (GameObject itemSlot in itemSlots)
            {
                // Az itemSlot helyi pozíciója a gameObject szülõhöz képest
                Vector3 slotLocalPos = itemSlot.GetComponent<RectTransform>().localPosition;
                Vector2 slotMin = slotLocalPos - (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;
                Vector2 slotMax = slotLocalPos + (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;

                // Beállítja a minimális és maximális pontokat az összes itemSlot lefedésére
                minPos = Vector2.Min(minPos, slotMin);
                maxPos = Vector2.Max(maxPos, slotMax);
            }

            // Méret kiszámítása és a szülõ objektum méretének beállítása
            Vector2 newSize = maxPos - minPos;

            gameObject.transform.SetParent(itemSlots.First().transform.parent, false);
            // A szülõ objektum pivotját középre állítja a pontos igazításhoz
            itemObjectRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.pivot = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.offsetMin = Vector2.zero;
            itemObjectRectTransform.offsetMax = Vector2.zero;
            itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX * Main.DefaultItemSlotSize, ActualData.SizeY * Main.DefaultItemSlotSize);
            itemObjectRectTransform.localPosition = (Vector3)((maxPos + minPos) / 2f);

            // Új pozíció beállítása úgy, hogy a szülõ lefedje az itemSlots összes elemét

            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
            itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
            //Debug.Log($"{ActualData.ItemName}:  position     {gameObject.GetComponent<RectTransform>().localPosition}");
            Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize/Main.SectorScale;
        }
        else
        {
            Debug.LogError($"{ActualData.ItemName}: non visualisation");
        }
        BoxCollider2D itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        itemObjectBoxCollider2D.autoTiling = true;
        itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;

        transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);
    }
}