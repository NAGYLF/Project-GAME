using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;

public class InGameItemObject : MonoBehaviour
{
    public GameObject ItemCompound;
    public Item ActualData { get; private set; }

    public void Inicialisation()//manualisan és automatikusan is vegrehajtodik, elofodulaht hogy za obejctuma meg nem letezik és az is hogy letezik
    {
        gameObject.name = ActualData.ItemName;

        ActualData.InGameSelfObject = gameObject;

        SelfVisualisation();//itt nem allitunk be referenciat
    }
    public void SetDataRoute(Item Data)
    {
        ActualData = Data;
    }
    public void SelfVisualisation()//az adatok alapjan vizualizalja az itemet
    {
        #region Positioning and Scaling


        RectTransform itemObjectRectTransform = gameObject.GetComponent<RectTransform>();


        itemObjectRectTransform.sizeDelta = new Vector2(ActualData.SizeX * Main.DefaultItemSlotSize, ActualData.SizeY * Main.DefaultItemSlotSize);


        #endregion
        ItemCompoundRefresh();
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