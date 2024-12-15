using Assets.Scripts;
using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;
using static MainData.SupportScripts;
using static PlayerInventoryClass.PlayerInventory;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using PlayerInventoryClass;

public class ItemObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject Counter;
    private GameObject Window;
    private GameObject Container = null;
    public Item ActualData { get; private set; }
    public GameObject VirtualParentObject;//azon objektum melynek a friss adatszinkronizációért felel

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
        if (placer.activeItemSlots != null && placer.activeItemSlots.Count == 1 && placer.activeItemSlots.First().GetComponent<ItemSlot>().IsEquipment)
        {
            if (placer.activeItemSlots.First().GetComponent<ItemSlot>().PartOfItemObject != null && placer.activeItemSlots.First().GetComponent<ItemSlot>().PartOfItemObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                return false;
            }
            return true;
        }
        else if (placer.activeItemSlots != null && placer.activeItemSlots.Count == ActualData.SizeX * ActualData.SizeY && placer.activeItemSlots.Count == placer.activeItemSlots.FindAll(elem => elem.GetComponent<ItemSlot>().ParentObject == placer.activeItemSlots.First().GetComponent<ItemSlot>().ParentObject).Count)
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
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject itemWindow = CreatePrefab("GameElements/ItemWindow");
            itemWindow.GetComponent<ItemWindow>().itemObject = gameObject;
            itemWindow.GetComponent<ItemWindow>().parentObject = InventoryObjectRef;
            itemWindow.GetComponent<ItemWindow>().positioning();
            Window = itemWindow;
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Window != null) Destroy(Window);
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
            transform.SetParent(InventoryObjectRef.transform, false);
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
            SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = false;
            Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize;
            isDragging = true;
            DestroyContainer();
            #endregion
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
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
            SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;
            Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize / Main.SectorScale;
            isDragging = false;
            Placing(CanBePlace());
            BuildContainer();
            #endregion
        } 
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

            if (placer.activeItemSlots.Exists(slot => slot.GetComponent<ItemSlot>() != null && slot.GetComponent<ItemSlot>().CountAddAvaiable))//split and megre
            {
                GameObject MergeObject = placer.activeItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CountAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
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
            else if (placer.activeItemSlots.Count == ActualData.SizeY * ActualData.SizeX)//egy specialis objektumlétrehozási folyamat ez akkor lép érvénybe ha üres slotba kerül az item
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
        //nem bizots hogy equipment tipusu itemslotokkal mukodik
        else if (placer.activeItemSlots !=null && placer.activeItemSlots.Exists(slot=>slot.GetComponent<ItemSlot>() != null && slot.GetComponent<ItemSlot>().CountAddAvaiable))//csak containerekben mukodik
        {
            GameObject MergeObject = placer.activeItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CountAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
            int count = MergeObject.GetComponent<ItemObject>().ActualData.Quantity;
            MergeObject.GetComponent<ItemObject>().ActualData.Quantity += ActualData.Quantity;
            if (MergeObject.GetComponent<ItemObject>().ActualData.Quantity > MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize)
            {
                ActualData.Quantity = MergeObject.GetComponent<ItemObject>().ActualData.Quantity - MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize;
                MergeObject.GetComponent<ItemObject>().ActualData.Quantity = MergeObject.GetComponent<ItemObject>().ActualData.MaxStackSize;
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
                if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
                {
                    VirtualParentObject.GetComponent<PlayerInventory>().DataOut(ActualData);
                    PlayerInventory playerInventory = VirtualParentObject.GetComponent<PlayerInventory>();
                    for (int sector = 0; sector < playerInventory.EquipmentSlots.Count; sector++)
                    {
                        if (ActualData.SlotUse.Contains(playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().name))
                        {
                            playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().PartOfItemObject = null;
                        }
                    }
                }
                else
                {
                    VirtualParentObject.GetComponent<ContainerObject>().DataOut(ActualData);
                    ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                    for (int sector = 0; sector < containerObject.Sectors.Count; sector++)
                    {
                        for (int col = 0; col < containerObject.Sectors[sector].columnNumber; col++)
                        {
                            for (int row = 0; row < containerObject.Sectors[sector].rowNumber; row++)
                            {
                                if (ActualData.SlotUse.Contains(containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().name))
                                {
                                    containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().PartOfItemObject = null;
                                }
                            }
                        }
                    }
                }
                VirtualParentObject = placer.NewVirtualParentObject;
                if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
                {
                    PlayerInventory playerInventory = VirtualParentObject.GetComponent<PlayerInventory>();
                    itemSlots.Clear();
                    for (int sector = 0; sector < playerInventory.EquipmentSlots.Count; sector++)
                    {
                        if (placer.activeItemSlots.Contains(playerInventory.EquipmentSlots[sector]))
                        {
                            playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                            itemSlots.Add(playerInventory.EquipmentSlots[sector]);
                        }
                    }
                    ActualData.SlotUse = new string[itemSlots.Count];
                    for (int i = 0; i < ActualData.SlotUse.Length; i++)
                    {
                        ActualData.SlotUse[i] = itemSlots[i].name;
                    }
                    VirtualParentObject.GetComponent<PlayerInventory>().DataIn(ActualData,gameObject);
                    SelfVisualisation();
                }
                else
                {
                    ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                    itemSlots.Clear();
                    for (int sector = 0; sector < containerObject.Sectors.Count; sector++)
                    {
                        for (int col = 0; col < containerObject.Sectors[sector].columnNumber; col++)
                        {
                            for (int row = 0; row < containerObject.Sectors[sector].rowNumber; row++)
                            {
                                if (placer.activeItemSlots.Contains(containerObject.Sectors[sector].col[col].row[row]))
                                {
                                    containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                    itemSlots.Add(containerObject.Sectors[sector].col[col].row[row]);
                                }
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
                if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
                {
                    PlayerInventory playerInventory = VirtualParentObject.GetComponent<PlayerInventory>();
                    itemSlots.Clear();
                    for (int sector = 0; sector < playerInventory.EquipmentSlots.Count; sector++)
                    {
                        if (placer.activeItemSlots.Contains(playerInventory.EquipmentSlots[sector]))
                        {
                            playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                            itemSlots.Add(playerInventory.EquipmentSlots[sector]);
                        }
                        else if (!placer.activeItemSlots.Contains(playerInventory.EquipmentSlots[sector]) && ActualData.SlotUse.Contains(playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().name))
                        {
                            //ezt a slotot megfosztjuk az itemobjektumtól és annak adatától
                            playerInventory.EquipmentSlots[sector].GetComponent<ItemSlot>().PartOfItemObject = null;
                        }
                    }
                    ActualData.SlotUse = new string[itemSlots.Count];
                    for (int i = 0; i < ActualData.SlotUse.Length; i++)
                    {
                        ActualData.SlotUse[i] = itemSlots[i].name;
                    }
                    VirtualParentObject.GetComponent<PlayerInventory>().DataIn(ActualData, gameObject);
                    SelfVisualisation();
                }
                else
                {
                    ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
                    itemSlots.Clear();
                    for (int sector = 0; sector < containerObject.Sectors.Count; sector++)
                    {
                        for (int col = 0; col < containerObject.Sectors[sector].columnNumber; col++)
                        {
                            for (int row = 0; row < containerObject.Sectors[sector].rowNumber; row++)
                            {
                                //ha az aktiv slotok küzül az egyik az iterált slot      és ezt a slotot tartalmazza még ez az item actualdataja is.    az azt jelenti, hogy ezen slot ugyan ugy az itemobjektum része maradt
                                if (placer.activeItemSlots.Contains(containerObject.Sectors[sector].col[col].row[row]) && ActualData.SlotUse.Contains(containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().name))
                                {
                                    //ezert hozzadjuk az itemSlots listához az itemslot objektumot
                                    itemSlots.Add(containerObject.Sectors[sector].col[col].row[row]);
                                }
                                //ha az aktiv slotok küzül az egyik az iterált slot      de ezt nem tartalmazza az item actualdataja        az azt jelenti, hogy ezen itemslot egy uj része lett az itemobjektumnak
                                else if (placer.activeItemSlots.Contains(containerObject.Sectors[sector].col[col].row[row]) && !ActualData.SlotUse.Contains(containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().name))
                                {
                                    //ezen uj slot része lesz az itemobjektumnak és annak adatának      illetve hozzáadjuk az itemslot listához
                                    containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                                    itemSlots.Add(containerObject.Sectors[sector].col[col].row[row]);
                                }
                                //ha ezen iterált slot nem része az aktív slotoknak       de része az item actualdata-jának       az azt jelenti, hogy ez egy régi slot az itemobjektumnak
                                else if (!placer.activeItemSlots.Contains(containerObject.Sectors[sector].col[col].row[row]) && ActualData.SlotUse.Contains(containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().name))
                                {
                                    //ezt a slotot megfosztjuk az itemobjektumtól és annak adatától
                                    containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().PartOfItemObject = null;
                                }
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
        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            VirtualParentObject.GetComponent<PlayerInventory>().DataOut(ActualData);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataOut(ActualData);
        }
        DestroyContainer();
        Destroy(gameObject);
    }
    public void SelfBuild(Item Data, GameObject VirtualParentObject)
    {
        ActualData = Data;
        this.VirtualParentObject = VirtualParentObject;
        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            VirtualParentObject.GetComponent<PlayerInventory>().DataIn(ActualData,gameObject);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataIn(ActualData);
        }
    }
    public void DataIn(Item Data)
    {
        ActualData = Data;
        SelfVisualisation();
        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            VirtualParentObject.GetComponent<PlayerInventory>().DataUpdate(ActualData, gameObject);
        }
        else
        {
            VirtualParentObject.GetComponent<ContainerObject>().DataUpdate(ActualData, gameObject);
        }
    }
    public void DataOut(Item Data)
    {
        ActualData = Data;
        SelfVisualisation();
        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            VirtualParentObject.GetComponent<PlayerInventory>().DataUpdate(ActualData, gameObject);
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
        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            VirtualParentObject.GetComponent<PlayerInventory>().DataUpdate(ActualData, gameObject);
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
        BuildContainer();
        DataUpdate(ActualData);
    }
    public void BuildContainer()
    {
        if (VirtualParentObject.GetComponent<PlayerInventory>()!=null && ActualData.Container != null && Container == null)//11. ha az item adatai tartalmaznak containert akkor az létrejön
        {
            //--> ContainerObject.cs
            Debug.LogWarning($"{ActualData.ItemName} ItemObject ------- ref --------> ContainerObject.cs");
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData, gameObject);
            Container = containerObject;
        }
    }
    public void DestroyContainer()
    {
        if (Container != null)//11. ha az item adatai tartalmaznak containert akkor az létrejön
        {
            for (int sector = 0; sector < Container.GetComponent<ContainerObject>().Sectors.Count; sector++)
            {
                for (int col = 0; col < Container.GetComponent<ContainerObject>().Sectors[sector].columnNumber; col++)
                {
                    for (int row = 0; row < Container.GetComponent<ContainerObject>().Sectors[sector].rowNumber; row++)
                    {
                        if (Container.GetComponent<ContainerObject>().Sectors[sector].col[col].row[row].GetComponent<ItemObject>() != null)
                        {
                            Container.GetComponent<ContainerObject>().Sectors[sector].col[col].row[row].GetComponent<ItemObject>().DestroyContainer();
                        }
                    }
                }
            }
            Destroy(Container);
            Container = null;
        }
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

        if (VirtualParentObject.GetComponent<PlayerInventory>() != null)
        {
            Debug.Log($"{ActualData.ItemName}: Equipment visualisation");
            PlayerInventory playerInventory = VirtualParentObject.GetComponent<PlayerInventory>();
            itemSlots.Clear();
            for (int slot = 0; slot < playerInventory.EquipmentSlots.Count; slot++)
            {
                if (ActualData.SlotUse.Contains(playerInventory.EquipmentSlots[slot].GetComponent<ItemSlot>().name))
                {
                    itemSlots.Add(playerInventory.EquipmentSlots[slot]);
                    playerInventory.EquipmentSlots[slot].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
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

            ActualData.RotateDegree = 0;

            itemObjectRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.pivot = new Vector2(0.5f, 0.5f);
            itemObjectRectTransform.offsetMin = Vector2.zero;
            itemObjectRectTransform.offsetMax = Vector2.zero;
            itemObjectRectTransform.sizeDelta = itemSlots.First().GetComponent<RectTransform>().rect.size;
            itemObjectRectTransform.localPosition = (Vector3)((maxPos + minPos) / 2f);

            // Új pozíció beállítása úgy, hogy a szülõ lefedje az itemSlots összes elemét

            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.size.x);
            itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);

            Counter.GetComponent<TextMeshPro>().fontSize = Main.ItemCounterFontSize;
        }
        else if (VirtualParentObject.GetComponent<ContainerObject>() != null)//ha az item containerben van
        {
            Debug.Log($"{ActualData.ItemName}: Slot visualisation     {gameObject.GetComponent<RectTransform>().localPosition}");
            ContainerObject containerObject = VirtualParentObject.GetComponent<ContainerObject>();
            itemSlots.Clear();
            for (int sector = 0; sector < containerObject.Sectors.Count; sector++)
            {
                for (int col = 0; col < containerObject.Sectors[sector].columnNumber; col++)
                {
                    for (int row = 0; row < containerObject.Sectors[sector].rowNumber; row++)
                    {
                        if (ActualData.SlotUse.Contains(containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().name))
                        {
                            itemSlots.Add(containerObject.Sectors[sector].col[col].row[row]);
                            containerObject.Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                        }
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

        SlotPanelObject.GetComponent<PanelSlots>().ReFresh();
    }
}