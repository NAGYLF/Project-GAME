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

public class ItemObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI NamePlate;
    public TextMeshProUGUI AmmoPlate;
    public TextMeshProUGUI HotKeyPlate;
    public TextMeshProUGUI Counter;
    public GameObject ItemCompound;
    //public List<GameObject> ItemObjectParts;//opcionálisan használandó
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
            //Debug.LogWarning("window opened");
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

            #region Set Targeting Mode 
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region Set Dragable mod
            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = false;
            isDragging = true;
            DestroyContainer();
            InventorySystem.ItemCompoundRefresh(ItemCompound.GetComponent<ItemImgFitter>(), ActualData);
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

            #region unSet Targeting Mode
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            transform.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
            #endregion

            #region unSet Dragable mod
            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;
            isDragging = false;
            if (ActualData.AvaiablePlacerMetodes.Count > 0)
            {
                InventorySystem.Placer(ActualData, originalRotation);
            }
            ActualData.AvaiablePlacerMetodes.Clear();
            InventorySystem.ItemCompoundRefresh(ItemCompound.GetComponent<ItemImgFitter>(), ActualData);
            #endregion
        }
    }
    private void Start()
    {
        Inicialisation();//ez automatikusan vegrehajtodik
    }
    public void Inicialisation()//manualisan és automatikusan is vegrehajtodik, elofodulaht hogy za obejctuma meg nem letezik és az is hogy letezik
    {
        if (ActualData.ParentItem != null)// a temporary objectum fix - je
        {
            gameObject.name = ActualData.SystemName;

            ActualData.SelfGameobject = gameObject;

            InventorySystem.LiveCleaning(ActualData);
            InventorySystem.Live_Placing(ActualData, ActualData.ParentItem);

            BuildContainer();

            SelfVisualisation();//itt nem allitunk be referenciat
        }
        else// a temporary objectum fix - je
        {
            Destroy(gameObject);
        }
    }
    public void SetDataRoute(Item Data, Item Parent)
    {
        ActualData = Data;
        ActualData.ParentItem = Parent;
    }

    public void BuildContainer()
    {
        if (ActualData.IsEquipment && ActualData.Container != null && ActualData.Container.ContainerObject == null)
        {
            //--> ContainerObject.cs
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData);
        }
    }
    public void DestroyContainer()
    {
        if (ActualData.Container != null && ActualData.Container.ContainerObject != null)
        {
            ActualData.Container.Items.ForEach(item => item.SelfGameobject.GetComponent<ItemObject>().DestroyContainer());
            Destroy(ActualData.Container.ContainerObject);
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
            //Debug.LogWarning(ActualData.RotateDegree);
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
    public void SelfVisualisation()//az adatok alapjan vizualizalja az itemet
    {
        #region Titles Writing
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
        if (ActualData.CompatibleCaliber != "")
        {
            AmmoPlate.text = ActualData.CompatibleCaliber;
        }
        else
        {
            AmmoPlate.text = "";
        }
        #endregion

        #region Positioning and Scaling
        gameObject.transform.SetParent(ActualData.ItemSlotObjectsRef.First().transform.parent, false);

        Vector3 minPosition = Vector3.positiveInfinity;
        Vector3 maxPosition = Vector3.negativeInfinity;

        foreach (ItemSlot rect in ActualData.ItemSlotObjectsRef)
        {
            Vector3 rectMin = rect.GetComponent<RectTransform>().localPosition - new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);
            Vector3 rectMax = rect.GetComponent<RectTransform>().localPosition + new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);

            minPosition = Vector3.Min(minPosition, rectMin);
            maxPosition = Vector3.Max(maxPosition, rectMax);
        }

        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();

        if (ActualData.IsEquipment)
        {
            itemObjectRectTransform.sizeDelta = maxPosition - minPosition;
        }
        else
        {  
            itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX * Main.DefaultItemSlotSize, ActualData.SizeY * Main.DefaultItemSlotSize);
        }

        itemObjectRectTransform.localPosition = (minPosition + maxPosition) / 2;
        #endregion

        InventorySystem.ItemCompoundRefresh(ItemCompound.GetComponent<ItemImgFitter>(),ActualData);

        BoxCollider2D itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);

        InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ReFresh();
    }
}