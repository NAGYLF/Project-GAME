using Assets.Scripts;
using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;
using static PlayerInventoryClass.PlayerInventory;
using TMPro;
using PlayerInventoryClass;
using Items;

public class TemporaryItemObject : MonoBehaviour
{
    public TextMeshProUGUI NamePlate;
    public TextMeshProUGUI AmmoPlate;
    public TextMeshProUGUI HotKeyPlate;
    public TextMeshProUGUI Counter;
    public GameObject ItemCompound;
    private bool IsHavePurpose = true;
    public AdvancedItem ActualData { get; private set; }
    public AdvancedItem AdvancedItem { get; set; }
    public ModificationWindow window;

    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()
    {

        #region Set Moveable position
        transform.SetParent(InventoryObjectRef.transform, false);

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
    public void SetDataRoute(AdvancedItem Data)
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
            //Mozgatás leállítása, amikor elengedjük az egeret
            //#region unSet Moveable position
            //transform.SetParent(originalParent, false);
            //#endregion

            InventoryObjectRef.GetComponent<PlayerInventory>().SlotPanelObject.GetComponent<PanelSlots>().ScrollPanel.GetComponent<ScrollRect>().enabled = true;

            if (ActualData.AvaiablePlacerMetodes.Count > 0)//ha van lehetseges placer metodus
            {
                GameObject ItemObject = SupportScripts.CreatePrefab(AdvancedItem.AdvancedItemObjectParth);
                ItemObject.transform.SetParent(InventoryObjectRef.transform, false);
                ItemObject.GetComponent<ItemObject>().SetDataRoute(ActualData, ActualData.AvaiableParentItem);

                ItemObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                ItemObject.GetComponent<BoxCollider2D>().size = transform.GetComponent<RectTransform>().rect.size;

                InventorySystem.Placer(ActualData, ActualData.RotateDegree);

                ItemObject.GetComponent<ItemObject>().BuildContainer();
            }
            else
            {
                ActualData.AvaiablePlacerMetodes.Clear();//mivel a targeting aktivan probal rajonni a megfelelo plcaerre és toltogeti ebbe a lehetosegeke, de nekunk vegul ebben az esetben csak erre van szuksegunk
                InventorySystem.MergeParts mergeParts = new InventorySystem.MergeParts(AdvancedItem, ActualData);
                ActualData.AvaiablePlacerMetodes.Add(mergeParts.Execute_MergeParts);
                InventorySystem.Placer(ActualData, ActualData.RotateDegree);
            }
            IsHavePurpose = false;
            Destroy(gameObject);

        }
    }
    public void ItemPartTrasformation()
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
    public void SelfVisualisation()//ha az item equipment slotban van
    {
        NamePlate.text = ActualData.SystemName;
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
        if (ActualData.TryGetComponent<WeaponBody>(out var weaponBody))
        {
            AmmoPlate.text = weaponBody.Caliber_Weapon + "x" + weaponBody.CartridgeSize_Weapon;
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
                InventorySystem.NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Height, NewPosition.NonLiveCoordinates.First().Widht, NewPosition.SectorIndex, AdvancedItem, (AdvancedItem)AdvancedItem.ParentItem);

                InventorySystem.NonLive_UnPlacing(AdvancedItem);
                InventorySystem.NonLive_Placing(AdvancedItem, (AdvancedItem)AdvancedItem.ParentItem);

                InventorySystem.Live_UnPlacing(AdvancedItem);
                InventorySystem.Live_Placing(AdvancedItem, (AdvancedItem)AdvancedItem.ParentItem);
            }

            AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
            //AdvancedItem.PartPut(ActualData);
            //AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
            window.ItemPartTrasformation();
            Destroy(gameObject);
        }
    }
}