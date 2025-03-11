using Assets.Scripts;
using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;
using static PlayerInventoryClass.PlayerInventory;
using TMPro;
using UnityEngine.EventSystems;
using PlayerInventoryClass;
using UnityEditor.PackageManager.UI;
using Unity.VisualScripting;

public class TemporaryItemObject : MonoBehaviour/*, IPointerUpHandler*/
{
    public TextMeshProUGUI NamePlate;
    public TextMeshProUGUI AmmoPlate;
    public TextMeshProUGUI HotKeyPlate;
    public TextMeshProUGUI Counter;
    public GameObject ItemCompound;
    private bool IsHavePurpose = true;
    public Item ActualData { get; private set; }
    public Item AdvancedItem { get; set; }
    public ModificationWindow window;

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    // Mozgatás leállítása, amikor elengedjük az egeret
    //    //Debug.LogWarning("temporary item mouse up");
    //    #region unSet Dragable mod
    //    //if (AvaiableNewParentObject != null)
    //    //{
    //    //    if (ActualData.SelfGameobject != null)
    //    //    {
    //    //        ActualData.SelfGameobject.GetComponent<ItemObject>().Placer = AvaiableNewParentObject.GetComponent<ContainerObject>().ActualData.GivePlacer;
    //    //        ActualData.SelfGameobject.GetComponent<ItemObject>().Placing(InventorySystem.CanBePlace(ActualData, Placer), Placer);
    //    //        ActualData.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
    //    //        ActualData.SelfGameobject.GetComponent<ItemObject>().BuildContainer();
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    ActualData.SelfGameobject.GetComponent<ItemObject>().Placer = AvaiableNewParentObject.GetComponent<ContainerObject>().ActualData.GivePlacer;
    //    //    ActualData.SelfGameobject.GetComponent<ItemObject>().Placing(InventorySystem.CanBePlace(ActualData, Placer), Placer);
    //    //}
    //    InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;
    //    Destroy(gameObject);
    //    #endregion
    //}
    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()
    {
        //ActualData.SelfGameobject = gameObject;
        if (!ActualData.IsAdvancedItem)
        {
            Sprite sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja képét
            ItemCompound.GetComponent<Image>().sprite = sprite;
            ItemCompound.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        }
        #region Set Moveable position
        transform.SetParent(InventoryObjectRef.transform, false);
        //transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        //transform.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        //transform.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(ActualData.SizeX * Main.SectorScale * Main.DefaultItemSlotSize, ActualData.SizeY * Main.SectorScale * Main.DefaultItemSlotSize);
        #endregion

        #region Set Targeting Mode 
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
        #endregion

        #region Set Dragable mod
        InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = false;
        //DestroyContainer();

        //InGameUI.PlayerInventory.GetComponent<WindowManager>().ClearWindowManager();
        #endregion

        // Az egér pozíciójának lekérése a világkoordináta rendszerben
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Az objektum új pozíciójának beállítása (csak az X és Y, hogy a Z tengely ne változzon)
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);

        SelfVisualisation();
    }
    public void SetDataRoute(Item Data)
    {
        ActualData = Data;
    }

    private void Update()
    {
        ObjectMovement();
        Rotation();
    }
    private void Rotation()
    {
        if (Input.GetKeyUp(KeyCode.R))
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
        // Az egér pozíciójának lekérése a világkoordináta rendszerben
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Az objektum új pozíciójának beállítása (csak az X és Y, hogy a Z tengely ne változzon)
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        if (Input.GetMouseButtonUp(0) && IsHavePurpose)
        {
            //OnPointerUp(null); // vagy külön metódusban kezeled a felengedést

            // Mozgatás leállítása, amikor elengedjük az egeret
            //#region unSet Moveable position
            //transform.SetParent(originalParent, false);
            //#endregion

            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;

            if (ActualData.AvaiablePlacerMetodes != null)
            {
                if (ActualData.AvaiablePlacerMetodes.Count > 0)
                {
                    GameObject ItemObject = SupportScripts.CreatePrefab(ActualData.ObjectPath);
                    ItemObject.transform.SetParent(InventoryObjectRef.transform, false);
                    ItemObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    ItemObject.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;
                    ItemObject.GetComponent<ItemObject>().SetDataRoute(ActualData,ActualData.AvaiableParentItem);
                    ItemObject.GetComponent<ItemObject>().ActualData.SelfGameobject = ItemObject;
                    InventorySystem.Placer(ActualData, ActualData.RotateDegree);
                    ItemObject.GetComponent<ItemObject>().BuildContainer();
                }
                else
                {
                    List<Part> parts_ = new List<Part>()
                    {
                        ActualData.Parts.First()
                    };
                    ActualData.Parts.First().GetConnectedPartsTree(parts_);

                    ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect = InventorySystem.AdvancedItem_SizeChanger_EffectDetermination(AdvancedItem, parts_, true);
                    (HashSet<(int Height, int Widht)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition = InventorySystem.Try_PartPositioning(AdvancedItem, Effect.ChangedSize, Effect.Directions);

                    (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data = AdvancedItem.PartPut_IsPossible(ActualData);
                    AdvancedItem.PartPut(ActualData, Data.SCP, Data.ICP);

                    if (NewPosition.IsPositionAble)
                    {
                        InventorySystem.NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Height, NewPosition.NonLiveCoordinates.First().Widht, NewPosition.SectorIndex, AdvancedItem, AdvancedItem.ParentItem);

                        InventorySystem.NonLive_UnPlacing(AdvancedItem);
                        InventorySystem.NonLive_Placing(AdvancedItem, AdvancedItem.ParentItem);

                        InventorySystem.Live_UnPlacing(AdvancedItem);
                        InventorySystem.Live_Placing(AdvancedItem, AdvancedItem.ParentItem);
                    }

                    AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    //AdvancedItem.PartPut(ActualData);
                    //AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                    window.ItemPartTrasformation();
                }
            }
            else
            {
                List<Part> parts_ = new()
                {
                    ActualData.Parts.First()
                };

                ActualData.Parts.First().GetConnectedPartsTree(parts_);

                ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect = InventorySystem.AdvancedItem_SizeChanger_EffectDetermination(AdvancedItem, parts_, true);
                (HashSet<(int Height, int Widht)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition = InventorySystem.Try_PartPositioning(AdvancedItem, Effect.ChangedSize, Effect.Directions);

                (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data = AdvancedItem.PartPut_IsPossible(ActualData);
                AdvancedItem.PartPut(ActualData, Data.SCP, Data.ICP);

                if (NewPosition.IsPositionAble)
                {
                    InventorySystem.NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Height, NewPosition.NonLiveCoordinates.First().Widht, NewPosition.SectorIndex, AdvancedItem, AdvancedItem.ParentItem);

                    InventorySystem.NonLive_UnPlacing(AdvancedItem);
                    InventorySystem.NonLive_Placing(AdvancedItem, AdvancedItem.ParentItem);

                    InventorySystem.Live_UnPlacing(AdvancedItem);
                    InventorySystem.Live_Placing(AdvancedItem, AdvancedItem.ParentItem);
                }

                AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                //AdvancedItem.PartPut(ActualData);
                //AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                window.ItemPartTrasformation();

            }
            IsHavePurpose = false;
            Destroy(gameObject);

        }
    }
    public void ItemPartTrasformation()
    {
        if (ActualData.IsAdvancedItem)//modifikálható item
        {
            ItemCompound.GetComponent<ItemImgFitter>().ResetFitter();

            //foreach (Part part in ActualData.Parts)
            //{
            //    part.UnSetLive();
            //}

            List<Part> clonedParts = new();

            foreach (Part part in ActualData.Parts)
            {
                Part clonedPart = new(part.item_s_Part)
                {
                    HierarhicPlace = part.HierarhicPlace
                };
                clonedParts.Add(clonedPart);
            }

            ConnectionPoint[] connectionPoints = clonedParts.SelectMany(x => x.ConnectionPoints).ToArray();
            ConnectionPoint[] connectionPointsRef = ActualData.Parts.SelectMany(x => x.ConnectionPoints).ToArray();

            //lenyege hogy a connectionpointok masolatat hozzacsatlakoztassuk egy masik masolat cp hez a connectionpoints referencia szerint
            for (int i = 0; i < connectionPoints.Length; i++)
            {
                //esetek:
                //ha a cp nincs hasznalva és a referenciaban van pontja
                //ha a cp nincs hasznalva de a referenciaban nincs pontja
                if (!connectionPoints[i].Used && connectionPointsRef[i].ConnectedPoint != null)
                {
                    //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart.PartData.PartName} --> {connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName}");
                    //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart != null} --> {connectionPointsRef[i].ConnectedPoint != null}");
                    ConnectionPoint ConnectablePoint = connectionPoints.FirstOrDefault(item => item.SelfPart.PartData.PartName == connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName);
                    if (ConnectablePoint != null)
                    {
                        connectionPoints[i].Connect(ConnectablePoint);
                        //Debug.LogWarning($"{connectionPoints[i].SelfPart.PartData.PartName} / {connectionPoints[i].CPData.PointName}  Connected To {connectionPoints[i].ConnectedPoint.SelfPart.PartData.PartName} / {connectionPoints[i].ConnectedPoint.CPData.PointName}");
                    }
                }
            }

            //egyebkent feltetelezheto hogy rendezve kerul el idaig de biztonsagi okokbol rendezzuk
            //clonedParts.OrderBy(part => part.HierarhicPlace);

            foreach (Part part in clonedParts)
            {
                part.SetLive(ItemCompound.GetComponent<ItemImgFitter>().fitter.gameObject);
                foreach (ConnectionPoint cp in part.ConnectionPoints)
                {
                    cp.SetLive();
                }
            }
            foreach (Part part in clonedParts)
            {
                //Debug.LogWarning($"CP part ###########  {part.item_s_Part.ItemName}");
                foreach (ConnectionPoint connectionPoint in part.ConnectionPoints)
                {
                    //Debug.LogWarning($"CP connected   {connectionPoint.ConnectedPoint != null}");
                    if (connectionPoint.ConnectedPoint != null)
                    {
                        if (connectionPoint.SelfPart.HierarhicPlace < connectionPoint.ConnectedPoint.SelfPart.HierarhicPlace)
                        {

                            //Debug.LogWarning($"{connectionPoint.SelfPart.item_s_Part.ItemName} CP action   -------------------+++++++++++++++++++++++");
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
        else//nem modifikálhazó item
        {
            RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / ItemCompound.GetComponent<RectTransform>().sizeDelta.y, itemObjectRectTransform.rect.width / ItemCompound.GetComponent<RectTransform>().sizeDelta.x);
            ItemCompound.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemCompound.GetComponent<RectTransform>().sizeDelta.x * Scale, ItemCompound.GetComponent<RectTransform>().sizeDelta.y * Scale);
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
        //itemObjectRigibody2D.bodyType = RigidbodyType2D.Static;

        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();

        //Vector3 minPosition = Vector3.positiveInfinity;
        //Vector3 maxPosition = Vector3.negativeInfinity;

        //ActualData.ItemSlotObjectsRef.Clear();//remove
        //foreach (DataGrid dataGrid in ActualData.ParentItem.SectorDataGrid)
        //{
        //    foreach (RowData rowData in dataGrid.col)
        //    {
        //        foreach (GameObject slot in rowData.row)
        //        {
        //            if (ActualData.SlotUse.Contains(slot.name))
        //            {
        //                slot.GetComponent<ItemSlot>().PartOfItemObject = gameObject;
        //                ActualData.ItemSlotObjectsRef.Add(slot);//ref
        //            }
        //        }
        //    }
        //}

        //foreach (GameObject rect in ActualData.ItemSlotObjectsRef)
        //{
        //    Vector3 rectMin = rect.GetComponent<RectTransform>().localPosition - new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);
        //    Vector3 rectMax = rect.GetComponent<RectTransform>().localPosition + new Vector3(rect.GetComponent<RectTransform>().rect.width / 2, rect.GetComponent<RectTransform>().rect.height / 2, 0);

        //    minPosition = Vector3.Min(minPosition, rectMin);
        //    maxPosition = Vector3.Max(maxPosition, rectMax);
        //}

        //gameObject.transform.SetParent(ActualData.ItemSlotObjectsRef.First().transform.parent, false);

        //if (ActualData.IsEquipment)
        //{
        //    itemObjectRectTransform.sizeDelta = maxPosition - minPosition;
        //}
        //else
        //{
        //    itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX * Main.DefaultItemSlotSize, ActualData.SizeY * Main.DefaultItemSlotSize);
        //}

        //itemObjectRectTransform.localPosition = (minPosition + maxPosition) / 2;

        ItemPartTrasformation();

        gameObject.transform.rotation = Quaternion.Euler(0, 0, ActualData.RotateDegree);

        BoxCollider2D itemObjectBoxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        itemObjectBoxCollider2D.size = itemObjectRectTransform.rect.size;

        //InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ReFresh();

        //BuildContainer();
    }
    void OnDisable()
    {
        if (IsHavePurpose)
        {
            List<Part> parts_ = new List<Part>()
                    {
                        ActualData.Parts.First()
                    };
            ActualData.Parts.First().GetConnectedPartsTree(parts_);

            ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect = InventorySystem.AdvancedItem_SizeChanger_EffectDetermination(AdvancedItem, parts_, true);
            (HashSet<(int Height, int Widht)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition = InventorySystem.Try_PartPositioning(AdvancedItem, Effect.ChangedSize, Effect.Directions);

            (ConnectionPoint SCP, ConnectionPoint ICP, bool IsPossible) Data = AdvancedItem.PartPut_IsPossible(ActualData);
            AdvancedItem.PartPut(ActualData,Data.SCP,Data.ICP);

            if (NewPosition.IsPositionAble)
            {
                InventorySystem.NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Height, NewPosition.NonLiveCoordinates.First().Widht, NewPosition.SectorIndex, AdvancedItem, AdvancedItem.ParentItem);

                InventorySystem.NonLive_UnPlacing(AdvancedItem);
                InventorySystem.NonLive_Placing(AdvancedItem, AdvancedItem.ParentItem);

                InventorySystem.Live_UnPlacing(AdvancedItem);
                InventorySystem.Live_Placing(AdvancedItem, AdvancedItem.ParentItem);
            }

            AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
            //AdvancedItem.PartPut(ActualData);
            //AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
            window.ItemPartTrasformation();
            Destroy(gameObject);
        }
    }
}