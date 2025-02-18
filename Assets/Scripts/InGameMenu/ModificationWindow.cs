using Assets.Scripts.Inventory;
using ItemHandler;
using MainData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModificationWindow : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    //!!! a fejlesztes soran valtozhat igy odafigylest igenyel !!!
    private const string ModificationPartBoxPath = "GameElements/ModificationPartBox";

    public Item item;
    public GameObject PartsPanel;
    public GameObject ItemPanel;

    private Vector3 offset; // A k�l�nbs�g az objektum poz�ci�ja �s a kattint�s vil�gkoordin�t�ja k�z�tt
    private Camera mainCamera;

    void Start()
    {
        mainCamera = InGameUI.Camera.GetComponent<Camera>();
    }
    public void Openwindow(Item item)
    {
        this.item = item;
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
        // Konvert�ljuk az eg�r poz�ci�j�t world koordin�t�v�.
        Vector3 mousePoint = Input.mousePosition;
        // �ll�tsuk be a megfelel� z-t, ami a kamera �s az objektum t�vols�g�t jelzi
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Sz�moljuk ki az offset-et
        offset = transform.position - mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.LogWarning("DragMouse");
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        // Friss�tj�k az objektum poz�ci�j�t �gy, hogy a megfogott pont (offset-tel egy�tt) k�vessen
        transform.position = mainCamera.ScreenToWorldPoint(mousePoint) + offset;
    }
    public void ItemPartTrasformation()
    {
        if (item.IsAdvancedItem)//modifik�lhat� item
        {
            //1. l�p�s az Advanced item vizualiz�ci�ja
            ItemPanel.GetComponent<ItemImgFitter>().ResetFitter();
            GameObject fitter = ItemPanel.GetComponent<ItemImgFitter>().fitter.gameObject;
            List<Part> virtualParts = new();

            foreach (Part part in item.Parts)
            {
                Part virtualPart = new(part.item_s_Part)
                {
                    HierarhicPlace = part.HierarhicPlace
                };
                virtualParts.Add(virtualPart);
            }

            ConnectionPoint[] connectionPoints = virtualParts.SelectMany(x => x.ConnectionPoints).ToArray();
            ConnectionPoint[] connectionPointsRef = item.Parts.SelectMany(x => x.ConnectionPoints).ToArray();

            for (int i = 0; i < connectionPoints.Length; i++)
            {
                if (!connectionPoints[i].Used)
                {
                    //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart.PartData.PartName} --> {connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName}");
                    ConnectionPoint ConnectablePoint = connectionPoints.FirstOrDefault(item => item.SelfPart.PartData.PartName == connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName);
                    if (ConnectablePoint != null)
                    {
                        connectionPoints[i].Connect(ConnectablePoint);
                        //Debug.LogWarning($"{connectionPoints[i].SelfPart.PartData.PartName} / {connectionPoints[i].CPData.PointName}  Connected To {connectionPoints[i].ConnectedPoint.SelfPart.PartData.PartName} / {connectionPoints[i].ConnectedPoint.CPData.PointName}");
                    }
                }
            }

            //egyebkent feltetelezheto hogy rendezve kerul el idaig de biztonsagi okokbol rendezzuk
            virtualParts.OrderBy(part => part.HierarhicPlace);

            foreach (Part part in virtualParts)
            {
                part.SetLive(ItemPanel.GetComponent<ItemImgFitter>().fitter.gameObject);
                foreach (ConnectionPoint cp in part.ConnectionPoints)
                {
                    cp.SetLive();
                }
            }
            foreach (Part part in virtualParts)
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
                            // 1. Referenciapontok lek�r�se �s ki�rat�sa
                            RectTransform targetPoint1 = connectionPoint.RefPoint1.GetComponent<RectTransform>();
                            RectTransform targetPoint2 = connectionPoint.RefPoint2.GetComponent<RectTransform>();

                            // 2. Mozgatand� objektum �s referencia pontok lek�r�se
                            RectTransform toMoveObject = connectionPoint.ConnectedPoint.SelfPart.PartObject.GetComponent<RectTransform>();
                            RectTransform toMovePoint1 = connectionPoint.ConnectedPoint.RefPoint1.GetComponent<RectTransform>();
                            RectTransform toMovePoint2 = connectionPoint.ConnectedPoint.RefPoint2.GetComponent<RectTransform>();

                            // 3. Sk�l�z�si faktor sz�m�t�sa
                            float targetLocalDistance = Vector3.Distance(targetPoint1.position, targetPoint2.position);
                            float toMoveLocalDistance = Vector3.Distance(toMovePoint1.position, toMovePoint2.position);
                            float scaleFactor = targetLocalDistance / toMoveLocalDistance;
                            //Debug.LogWarning(part.item_s_Part.ItemName+" "+scaleFactor+ " targetLocalDistante: " + targetLocalDistance+ " toMoveLocalDistance: "+ toMoveLocalDistance);
                            if (float.IsNaN(scaleFactor))
                            {
                                scaleFactor = 1;
                            }

                            // 4. Alkalmazzuk a sk�l�z�st
                            toMoveObject.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                            //5. Poz�ci�k kisz�m�t�sa
                            Vector3 targetMidLocal = (targetPoint1.position + targetPoint2.position) * 0.5f;
                            Vector3 toMoveMidLocal = (toMovePoint1.position + toMovePoint2.position) * 0.5f;
                            Vector3 translationLocal = targetMidLocal - toMoveMidLocal;

                            // 6. Alkalmazzuk az eltol�st
                            toMoveObject.position += translationLocal;
                        }
                    }
                }
            }
            ItemPanel.GetComponent<ItemImgFitter>().Fitting();




            //2. l�p�s az item partBox-ok vizualiz�ci�ja
            List<Part> virtualParts2 = new();

            foreach (Part part in item.Parts)
            {
                Part virtualPart2 = new(part.item_s_Part)
                {
                    HierarhicPlace = part.HierarhicPlace
                };
                virtualParts2.Add(virtualPart2);
            }

            float itemBoxSize = CalculateBoxSize(virtualParts2.Count, PartsPanel.GetComponent<RectTransform>());
            ConnectionPoint[] connectionPoints2 = virtualParts.SelectMany(x => x.ConnectionPoints).ToArray();


            for (int i = 0; i < connectionPoints2.Length; i++)
            {
                if (!connectionPoints2[i].Used)
                {
                    //Debug.LogWarning($"Try connection: {connectionPoints[i].SelfPart.PartData.PartName} --> {connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName}");
                    ConnectionPoint ConnectablePoint2 = connectionPoints2.FirstOrDefault(item => item.SelfPart.PartData.PartName == connectionPointsRef[i].ConnectedPoint.SelfPart.PartData.PartName);
                    if (ConnectablePoint2 != null)
                    {
                        connectionPoints2[i].Connect(ConnectablePoint2);
                        //Debug.LogWarning($"{connectionPoints[i].SelfPart.PartData.PartName} / {connectionPoints[i].CPData.PointName}  Connected To {connectionPoints[i].ConnectedPoint.SelfPart.PartData.PartName} / {connectionPoints[i].ConnectedPoint.CPData.PointName}");
                    }
                }
            }

            PartsPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(itemBoxSize,itemBoxSize);

            //egyebkent feltetelezheto hogy rendezve kerul el idaig de biztonsagi okokbol rendezzuk
            virtualParts2.OrderBy(part => part.HierarhicPlace);
        
            foreach (Part part in virtualParts2)
            {
                GameObject box = SupportScripts.CreatePrefab(ModificationPartBoxPath);
                box.name = part.item_s_Part.ItemName;
                box.transform.SetParent(PartsPanel.transform);
                box.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBoxSize, itemBoxSize);
                part.SetLive(box.GetComponent<ItemImgFitter>().fitter.gameObject);
                box.GetComponent<ItemImgFitter>().Fitting();
            }
        }
        else//nem modifik�lhaz� item   !!!!!!Ez a r�sz m�g nem m�k�dik!!!!!!
        {
            RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();
            float Scale = Mathf.Min(itemObjectRectTransform.rect.height / ItemPanel.GetComponent<RectTransform>().sizeDelta.y, itemObjectRectTransform.rect.width / ItemPanel.GetComponent<RectTransform>().sizeDelta.x);
            ItemPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemPanel.GetComponent<RectTransform>().sizeDelta.x * Scale, ItemPanel.GetComponent<RectTransform>().sizeDelta.y * Scale);
        }
    }
    private  float CalculateBoxSize(int totalItems, RectTransform containerRect)
    {
        // A kont�ner m�retei (felt�telezz�k, hogy ezek a rendelkez�sre �ll� ter�letet adj�k meg)
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // Annak �rdek�ben, hogy a grid min�l k�zelebb n�zzen ki egy n�gyzethez, sz�moljuk ki:
        int columns = Mathf.CeilToInt(Mathf.Sqrt(totalItems)); // Maxim�lis itemek sz�ma egy sorban
        int rows = Mathf.CeilToInt((float)totalItems / columns);  // Maxim�lis itemek sz�ma egy oszlopban

        // Az egy itemBox m�rete att�l f�gg, hogy a kont�ner sz�less�g�t �s magass�g�t h�ny r�szre osztjuk
        float itemSizeByWidth = containerWidth / columns;
        float itemSizeByHeight = containerHeight / rows;
        // Annak �rdek�ben, hogy az itemek n�gyzetek legyenek, a legkisebb �rt�ket vessz�k
        float itemBoxSize = Mathf.Min(itemSizeByWidth, itemSizeByHeight);

        return itemBoxSize;
    }
}
