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
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Collections;

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
            ItemCompoundRefresh();
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
            if (ActualData.AvaiablePlacerMetodes != null)
            {
                InventorySystem.Placer(ActualData, originalRotation);
            }
            ActualData.AvaiablePlacerMetodes = null;
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
            gameObject.name = ActualData.ItemName;

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
        if (ActualData.IsEquipment && ActualData.Container != null && ActualData.ContainerObject == null)
        {
            //--> ContainerObject.cs
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().SetDataRoute(ActualData);
        }
    }
    public void DestroyContainer()
    {
        if (ActualData.ContainerObject != null)
        {
            ActualData.Container.Items.ForEach(item => item.SelfGameobject.GetComponent<ItemObject>().DestroyContainer());
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
        if (ActualData.AmmoType != "")
        {
            AmmoPlate.text = ActualData.AmmoType;
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

        ItemCompoundRefresh();

        BoxCollider2D itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);

        InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ReFresh();
    }

    public void ItemCompoundRefresh()
    {
        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();

        #region Image Setting
        if (!ActualData.IsAdvancedItem)
        {
            Sprite sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja képét

            //!!! ez a játék fejlesztes soran valtozhat ezert odafigyelst igenyel
            GameObject ImgObject = ItemCompound.transform.GetChild(0).gameObject;
            ImgObject.GetComponent<Image>().sprite = sprite;
            ImgObject.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);

            float Scale = Mathf.Min(ItemCompound.GetComponent<RectTransform>().rect.height / ImgObject.GetComponent<RectTransform>().sizeDelta.y, ItemCompound.GetComponent<RectTransform>().rect.width / ImgObject.GetComponent<RectTransform>().sizeDelta.x);
            ImgObject.GetComponent<RectTransform>().sizeDelta = new Vector2(ImgObject.GetComponent<RectTransform>().sizeDelta.x * Scale, ImgObject.GetComponent<RectTransform>().sizeDelta.y * Scale);
        }
        else
        {
            for (int i = ItemCompound.GetComponent<ItemImgFitter>().fitter.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = ItemCompound.GetComponent<ItemImgFitter>().fitter.transform.GetChild(i);
                child.SetParent(null);
                Object.Destroy(child.gameObject);
            }

            ItemCompound.GetComponent<ItemImgFitter>().ResetFitter();

            foreach (Part part in ActualData.Parts)
            {
                part.SetLive(ActualData.SelfGameobject.GetComponent<ItemObject>().ItemCompound.GetComponent<ItemImgFitter>().fitter.gameObject);
                foreach (ConnectionPoint cp in part.ConnectionPoints)
                {
                    cp.SetLive();
                }
                part.PartObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
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

                            //5. Pozíciók kiszámítása
                            Vector3 targetMidLocal = (targetPoint1.position + targetPoint2.position) * 0.5f;
                            Vector3 toMoveMidLocal = (toMovePoint1.position + toMovePoint2.position) * 0.5f;
                            Vector3 translationLocal = targetMidLocal - toMoveMidLocal;

                            // 6. Alkalmazzuk az eltolást
                            toMoveObject.position += translationLocal;
                        }
                    }
                }
            }
            ItemCompound.GetComponent<ItemImgFitter>().Fitting();
        }
        #endregion
    }
}