using Assets.Scripts.Inventory;
using ItemHandler;
using MainData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModificationWindow : MonoBehaviour, IPointerDownHandler
{
    //!!! a fejlesztes soran valtozhat igy odafigylest igenyel !!!
    private const string ModificationPartBoxPath = "GameElements/ModificationPartBox";

    public Item AdvancedItem;
    public GameObject PartsPanel;
    public GameObject ItemPanel;

    private List<GameObject> Boxes;
    private List<Part> cloneParts2;
    private List<Part> cloneParts;

    private Vector3 offset; // A különbség az objektum pozíciója és a kattintás világkoordinátája között
    private Camera mainCamera;

    void Start()
    {
        mainCamera = InGameUI.Camera.GetComponent<Camera>();
    }
    public void Openwindow(Item item)
    {
        this.AdvancedItem = item;
        AdvancedItem.ModificationWindowRef = this;
        cloneParts = new();
        cloneParts2 = new();
        Boxes = new();
        ItemPartTrasformation();
    }
    public void CloseWindow()
    {
        InGameUI.PlayerInventory.GetComponent<WindowManager>().DestroyWindow(gameObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogWarning("OnMouseDown");
        InGameUI.PlayerInventory.GetComponent<WindowManager>().SetWindowToTheTop(gameObject);
        // Konvertáljuk az egér pozícióját world koordinátává.
        Vector3 mousePoint = Input.mousePosition;
        // Állítsuk be a megfelelõ z-t, ami a kamera és az objektum távolságát jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Számoljuk ki az offset-et
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }
    public void ItemPartTrasformation()
    {
        ItemPanel.GetComponent<ItemImgFitter>().ResetFitter();
        //parts panel !!!

        // 1. unset live all part
        foreach (Part part in cloneParts)
        {
            part.UnSetLive();
        }

        // 2. clear all part
        cloneParts.Clear();

        // 3. create new parts (a parts-okat az Advanced Item referenciaja alpjan keszitjuk)
        foreach (Part part in AdvancedItem.Parts)
        {
            Part clonePart = new(part.item_s_Part)
            {
                HierarhicPlace = part.HierarhicPlace
            };
            cloneParts.Add(clonePart);
        }

        ConnectionPoint[] connectionPoints = cloneParts.SelectMany(x => x.ConnectionPoints).ToArray();
        ConnectionPoint[] connectionPointsRef = AdvancedItem.Parts.SelectMany(x => x.ConnectionPoints).ToArray();

        // 4. CP connecting a referncia alapjan
        //lenyege hogy a connectionpointok masolatat hozzacsatlakoztassuk egy masik masolat cp hez a connectionpoints referencia szerint
        for (int i = 0; i < connectionPoints.Length; i++)
        {
            if (!connectionPoints[i].Used && connectionPointsRef[i].ConnectedPoint != null)
            {
                //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart != null} --> {connectionPointsRef[i].ConnectedPoint != null}");
                ConnectionPoint ConnectablePoint = connectionPoints[Array.IndexOf(connectionPointsRef, connectionPointsRef[i].ConnectedPoint)];/* connectionPoints.FirstOrDefault(item => item.SelfPart.PartData.PartName ==.SelfPart.PartData.PartName);*/
                connectionPoints[i].Connect(ConnectablePoint);
                //Debug.LogWarning($"{connectionPoints[i].SelfPart.PartData.PartName} / {connectionPoints[i].CPData.PointName}  Connected To {connectionPoints[i].ConnectedPoint.SelfPart.PartData.PartName} / {connectionPoints[i].ConnectedPoint.CPData.PointName}");
            }
        }

        //egyebkent feltetelezheto hogy rendezve kerul el idaig de biztonsagi okokbol rendezzuk
        //cloneParts.OrderBy(part => part.HierarhicPlace);

        // 5. CP set live
        foreach (Part part in cloneParts)
        {
            part.SetLive(ItemPanel.GetComponent<ItemImgFitter>().fitter.gameObject);
            foreach (ConnectionPoint cp in part.ConnectionPoints)
            {
                cp.SetLive();
            }
        }

        // 6. a partokat pozitcionaljuk egymashoz
        foreach (Part part in cloneParts)
        {
            //Debug.LogWarning($"part CP fitting Start: {part.item_s_Part.ItemName}");
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
        ItemPanel.GetComponent<ItemImgFitter>().Fitting();




        // 8. unset all live part

        foreach (GameObject box in Boxes)
        {
            Destroy(box);
        }

        // 9. clear all part
        Boxes.Clear();
        cloneParts2.Clear();

        foreach (Part part in AdvancedItem.Parts)
        {
            Part clonePart2 = new(part.item_s_Part)
            {
                HierarhicPlace = part.HierarhicPlace
            };
            cloneParts2.Add(part);
        }

        // 8. kiszamoljuk az itembox meretet
        float itemBoxSize = CalculateBoxSize(cloneParts2.Count, PartsPanel.GetComponent<RectTransform>());
        ConnectionPoint[] connectionPoints2 = cloneParts2.SelectMany(x => x.ConnectionPoints).ToArray();

        for (int i = 0; i < connectionPoints2.Length; i++)
        {
            if (!connectionPoints2[i].Used && connectionPointsRef[i].ConnectedPoint != null)
            {
                //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart.PartData.PartName} --> {connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName}");
                ConnectionPoint ConnectablePoint2 = connectionPoints2[Array.IndexOf(connectionPointsRef, connectionPointsRef[i].ConnectedPoint)];
                connectionPoints2[i].Connect(ConnectablePoint2);
                //Debug.LogWarning($"{connectionPoints[i].SelfPart.PartData.PartName} / {connectionPoints[i].CPData.PointName}  Connected To {connectionPoints[i].ConnectedPoint.SelfPart.PartData.PartName} / {connectionPoints[i].ConnectedPoint.CPData.PointName}");
            }
        }

        PartsPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(itemBoxSize, itemBoxSize);

        //egyebkent feltetelezheto hogy rendezve kerul el idaig de biztonsagi okokbol rendezzuk
        //cloneParts2.OrderBy(part => part.HierarhicPlace);

        //vizualizaljuk a box-okat
        for (int i = 1; i < cloneParts2.Count; i++)//1 tol kezd igy a hierarhiaban levo elso itemet nem lehet eltavolitani
        {
            GameObject box = SupportScripts.CreatePrefab(ModificationPartBoxPath);
            Boxes.Add(box);
            box.name = cloneParts2[i].item_s_Part.SystemName;
            box.GetComponent<ModificationPartBox>().window = this;
            box.GetComponent<ModificationPartBox>().AdvancedItem = AdvancedItem;
            box.GetComponent<ModificationPartBox>().PartIndex = i;
            box.transform.SetParent(PartsPanel.transform, false);
            box.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize, itemBoxSize);
            cloneParts2[i].SetLive(box.GetComponent<ItemImgFitter>().fitter.gameObject);
            box.GetComponent<ItemImgFitter>().Fitting();
        }
    }
    private float CalculateBoxSize(int totalItems, RectTransform containerRect)
    {
        // A konténer méretei (feltételezzük, hogy ezek a rendelkezésre álló területet adják meg)
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Annak érdekében, hogy a grid minél közelebb nézzen ki egy négyzethez, számoljuk ki:
        int columns = Mathf.CeilToInt(Mathf.Sqrt(totalItems)); // Maximális itemek száma egy sorban
        int rows = Mathf.CeilToInt((float)totalItems / columns);  // Maximális itemek száma egy oszlopban

        // Az egy itemBox mérete attól függ, hogy a konténer szélességét és magasságát hány részre osztjuk
        float itemSizeByWidth = containerWidth / columns;
        float itemSizeByHeight = containerHeight / rows;
        // Annak érdekében, hogy az itemek négyzetek legyenek, a legkisebb értéket vesszük
        float itemBoxSize = Mathf.Min(itemSizeByWidth, itemSizeByHeight);

        return itemBoxSize;
    }
}
