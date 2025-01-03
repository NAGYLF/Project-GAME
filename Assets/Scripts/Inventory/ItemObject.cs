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
using UnityEngine.EventSystems;
using PlayerInventoryClass;
using UI;

public class ItemObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler , IPointerEnterHandler ,IPointerExitHandler
{
    public TextMeshProUGUI NamePlate;
    public TextMeshProUGUI AmmoPlate;
    public TextMeshProUGUI HotKeyPlate;
    public TextMeshProUGUI Counter;
    public Image image;
    private GameObject Window;
    public Item ActualData { get; private set; }

    private Transform originalParent;
    private Vector3 originalPosition;
    private Vector2 originalSize;
    private Vector2 originalPivot;
    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private float originalRotation;//ezt kivetelesen nem az onMouseDown eljarasban hasznaljuk hanem a placing eljaras azon else agaban amely a CanBePlacing false agan helyezkedik el.

    private bool isDragging = false;
    public PlacerStruct placer { private get; set; }
    private bool CanBePlace()
    {
        //az itemslotok szama egynelo az item meretevel és mindegyik slot ugyan abban a sectorban van     vagy a placer aktiv slotjaiban egy elem van ami egy equipmentslot
        //Debug.Log(placer.activeItemSlots.First().name);
        if (placer.ActiveItemSlots != null && placer.ActiveItemSlots.Count == 1 && placer.ActiveItemSlots.First().GetComponent<ItemSlot>().IsEquipment)
        {
            if (placer.ActiveItemSlots.First().GetComponent<ItemSlot>().PartOfItemObject != null && placer.ActiveItemSlots.First().GetComponent<ItemSlot>().PartOfItemObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                return false;
            }
            return true;
        }
        else if (placer.ActiveItemSlots != null && placer.ActiveItemSlots.Count == ActualData.SizeX * ActualData.SizeY && placer.ActiveItemSlots.Count == placer.ActiveItemSlots.FindAll(elem => elem.GetComponent<ItemSlot>().ParentObject == placer.ActiveItemSlots.First().GetComponent<ItemSlot>().ParentObject).Count)
        {
            for (int i = 0; i < placer.ActiveItemSlots.Count; i++)
            {
                //az itemslototk itemobject tartalma vagy null vagy az itemobjectum maga
                if (placer.ActiveItemSlots[i].GetComponent<ItemSlot>().PartOfItemObject != null && placer.ActiveItemSlots[i].GetComponent<ItemSlot>().PartOfItemObject.GetInstanceID() != gameObject.GetInstanceID())
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        InGameUI.SetHotKeyWithMouse = true;
        InGameUI.SetGameObjectToHotKey = gameObject;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InGameUI.SetHotKeyWithMouse = false;
        InGameUI.SetGameObjectToHotKey = null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.LogWarning("window opened");
            GameObject itemWindow = CreatePrefab("GameElements/ItemWindow");
            itemWindow.transform.SetParent(InventoryObjectRef.transform);
            itemWindow.GetComponent<ItemWindow>().itemObject = gameObject;
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
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / image.GetComponent<RectTransform>().sizeDelta.y, itemObjectRectTransform.rect.width / image.GetComponent<RectTransform>().sizeDelta.x);
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(image.GetComponent<RectTransform>().sizeDelta.x * Scale, image.GetComponent<RectTransform>().sizeDelta.y * Scale);
            #endregion

            #region Set Targeting Mode 
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region Set Dragable mod
            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = false;
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
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / image.GetComponent<RectTransform>().sizeDelta.y, itemObjectRectTransform.rect.width / image.GetComponent<RectTransform>().sizeDelta.x);
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(image.GetComponent<RectTransform>().sizeDelta.x * Scale, image.GetComponent<RectTransform>().sizeDelta.y * Scale);
            #endregion

            #region unSet Targeting Mode
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region unSet Dragable mod
            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;
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
        //placer.activeItemSlots.FindAll(slot=> ActualData.SlotUse.Contains(slot.name)).Count != ActualData.SlotUse.Count
        if (Input.GetKey(KeyCode.LeftControl) && !placer.ActiveItemSlots.Exists(slot=>slot.GetComponent<ItemSlot>().PartOfItemObject != null && slot.GetComponent<ItemSlot>().PartOfItemObject.GetInstanceID() == gameObject.GetInstanceID()))//split  (azt ellenorizzuk hogy meg lett e nyomva a ctrl és nincs olyan slot amelyet az item tartlamaz ezzel saját magéba nem slpitelhet ezzel megsporoljuk a felesleges szamitasokat)
        {
            (int smaller, int larger) SplitedCount = SplitInteger(ActualData.Quantity);

            if (placer.ActiveItemSlots.Exists(slot => slot.GetComponent<ItemSlot>().CountAddAvaiable))//split and megre
            {
                GameObject MergeObject = placer.ActiveItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CountAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
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
                        ActualData.Remove();
                    }
                    else
                    {
                        SelfVisualisation();
                    }
                }
            }
            else if ((placer.ActiveItemSlots.Count == ActualData.SizeY * ActualData.SizeX) || (placer.ActiveItemSlots.Count == 1 && placer.ActiveItemSlots.First().GetComponent<ItemSlot>().IsEquipment))//egy specialis objektumlétrehozási folyamat ez akkor lép érvénybe ha üres slotba kerül az item
            {
                Item item = new Item(ActualData.ItemName, SplitedCount.larger);

                GameObject itemObject = CreatePrefab("GameElements/ItemObject");
                itemObject.name = item.ItemName;
                item.SelfGameobject = itemObject;
                item.ParentItem = placer.NewParentData;
                itemObject.GetComponent<ItemObject>().SetDataRoute(item, item.ParentItem);

                InventorySystem.SetSlotUseByPlacer(placer,item);

                InventorySystem.DataAdd(item.ParentItem,item);

                ActualData.Quantity = SplitedCount.smaller;
                placementCanStart = false;
                if (ActualData.Quantity < 1)
                {
                    ActualData.Remove();
                }
                else
                {
                    SelfVisualisation();
                }
            }
        }
        #region Item Merge
        //nem bizots hogy equipment tipusu itemslotokkal mukodik
        else if (placer.ActiveItemSlots !=null && placer.ActiveItemSlots.Exists(slot=>slot.GetComponent<ItemSlot>() != null && slot.GetComponent<ItemSlot>().CountAddAvaiable))//csak containerekben mukodik
        {
            GameObject MergeObject = placer.ActiveItemSlots.Find(slot => slot.GetComponent<ItemSlot>().CountAddAvaiable).GetComponent<ItemSlot>().PartOfItemObject;
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
                ActualData.Remove();//Totális Törlés mindenhonnan
                placementCanStart = false;
            }
        }
        #endregion
        if (placementCanStart)
        {
            InventorySystem.DataRemove(ActualData.ParentItem,ActualData);

            InventorySystem.SetNewDataParent(placer.NewParentData,ActualData);

            InventorySystem.SetSlotUseByPlacer(placer,ActualData);
            InventorySystem.DataAdd(ActualData.ParentItem,ActualData);

            SelfVisualisation();
        }
        else
        {
            //ha nem sikerul elhelyzni akkor eredeti allpotaba kerul
            ActualData.RotateDegree = originalRotation;
            SelfVisualisation();
        }
        InventoryObjectRef.GetComponent<PlayerInventory>().levelManager.SetMaxLVL_And_Sort();
    }
    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()
    {
        ActualData.SelfGameobject = gameObject;

        Sprite sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja képét
        image.sprite = sprite;
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);

        SelfVisualisation();
    }
    public void SetDataRoute(Item Data,Item Parent)
    {
        ActualData = Data;
        ActualData.ParentItem = Parent;
    }

    public void BuildContainer()
    {
        if (ActualData.IsEquipment && ActualData.Container != null && ActualData.ContainerObject == null)
        {
            //--> ContainerObject.cs
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData);
            ActualData.ContainerObject = containerObject;
            ActualData.SectorDataGrid = ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors;
        }
    }
    public void DestroyContainer()
    {
        if (ActualData.ContainerObject != null)
        {
            for (int sector = 0; sector < ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors.Count; sector++)
            {
                for (int col = 0; col < ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors[sector].columnNumber; col++)
                {
                    for (int row = 0; row < ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors[sector].rowNumber; row++)
                    {
                        if (ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors[sector].col[col].row[row].GetComponent<ItemObject>() != null)
                        {
                            ActualData.ContainerObject.GetComponent<ContainerObject>().Sectors[sector].col[col].row[row].GetComponent<ItemObject>().DestroyContainer();
                        }
                    }
                }
            }
            Destroy(ActualData.ContainerObject);
        }
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
    public void SelfVisualisation()//ha az item equipment slotban van
    {
        NamePlate.text = ActualData.ItemName;
        if (ActualData.Quantity == 1)
        {
            Counter.text = "";
        }
        else
        {
            Counter.text = ActualData.Quantity.ToString();
        }
        if (ActualData.HotKey != "")
        {
            HotKeyPlate.text = ActualData.HotKey.ToString();
        }
        else
        {
            HotKeyPlate.text = "";
        }
        if (ActualData.AmmoType != "")
        {
            AmmoPlate.text = ActualData.AmmoType;
        }
        else
        {
            AmmoPlate.text = "";
        }

        Rigidbody2D itemObjectRigibody2D = gameObject.GetComponent<Rigidbody2D>();

        itemObjectRigibody2D.mass = 0;
        itemObjectRigibody2D.drag = 0;
        itemObjectRigibody2D.angularDrag = 0;
        itemObjectRigibody2D.gravityScale = 0;
        itemObjectRigibody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        itemObjectRigibody2D.bodyType = RigidbodyType2D.Static;

        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();

        // Alapértelmezett kezdõértékek (értelmesen kiszámítva)
        // Meghatározzuk a legkisebb és legnagyobb pozíciót
        Vector3 minPosition = Vector3.positiveInfinity;
        Vector3 maxPosition = Vector3.negativeInfinity;

        List<GameObject> itemSlots = new List<GameObject>();
        foreach (DataGrid dataGrid in ActualData.ParentItem.SectorDataGrid)
        {
            foreach (RowData rowData in dataGrid.col)
            {
                foreach (GameObject slot in rowData.row)
                {
                    if (ActualData.SlotUse.Contains(slot.name))
                    {
                        itemSlots.Add(slot);
                        slot.GetComponent<ItemSlot>().PartOfItemObject = gameObject;
                    }
                }
            }
        }

        // Végigiterálunk az összes objektumon
        foreach (GameObject rect in itemSlots)
        {
            // Ha van RectTransform, frissítjük a minimum és maximum pozíciókat
            Vector3 rectMin = rect.GetComponent<RectTransform>().localPosition - new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);
            Vector3 rectMax = rect.GetComponent<RectTransform>().localPosition + new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);

            minPosition = Vector3.Min(minPosition, rectMin);
            maxPosition = Vector3.Max(maxPosition, rectMax);
        }

        gameObject.transform.SetParent(itemSlots.First().transform.parent, false);

        if (ActualData.IsEquipment)
        {
            itemObjectRectTransform.sizeDelta = maxPosition - minPosition;
            ActualData.RotateDegree = 0;
        }
        else
        {
            itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX*Main.DefaultItemSlotSize,ActualData.SizeY*Main.DefaultItemSlotSize);
        }

        itemObjectRectTransform.localPosition = (minPosition + maxPosition) / 2;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);

        float Scale = Mathf.Min(itemObjectRectTransform.rect.height / image.GetComponent<RectTransform>().sizeDelta.y, itemObjectRectTransform.rect.width / image.GetComponent<RectTransform>().sizeDelta.x);
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(image.GetComponent<RectTransform>().sizeDelta.x * Scale, image.GetComponent<RectTransform>().sizeDelta.y * Scale);

        BoxCollider2D itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;

        InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ReFresh();

        BuildContainer();
    }
}