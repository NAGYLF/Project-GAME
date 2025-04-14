using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;
using UnityEngine.UIElements;

public class InGameItemObject : MonoBehaviour
{
    public ItemImgFitter ItemCompound;
    public AdvancedItem ActualData { get; private set; }

    public void Inicialisation()//manualisan és automatikusan is vegrehajtodik, elofodulaht hogy za obejctuma meg nem letezik és az is hogy letezik
    {
        if (ActualData != null)
        {
            gameObject.name = ActualData.SystemName;

            ActualData.InGameSelfObject = gameObject;



            SelfVisualisation();//itt nem allitunk be referenciat

            SetInGameItemSize();


        }
        else
        {
            gameObject.name = "Hands";

            for (int i = ItemCompound.fitter.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(ItemCompound.fitter.transform.GetChild(i).gameObject);
            }

            ReSetInGameItemSize();
        }
    }
    public void SetInGameItemSize()
    {
        SystemPoints sp = ActualData?.Parts.SelectMany(part => part.SystemPoints).FirstOrDefault(sp => sp.SPData.PointName == "FirstHand");
        if (sp != null)
        {
            Debug.LogWarning("size");
            float distance = Vector2.Distance(sp.InGameRefPoint1.transform.position, sp.InGameRefPoint2.transform.position);
            Debug.LogWarning(distance);
            float scale = MainData.Main.CharacterHandSize / distance;
            //SelectedItemObject.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            gameObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
        }
    }
    public void ReSetInGameItemSize()
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    public void SetDataRoute(AdvancedItem Data)
    {
        ActualData = Data;
    }
    public void SelfVisualisation()//az adatok alapjan vizualizalja az itemet
    {
        InventorySystem.ItemCompoundRefresh_InGame(ItemCompound,ActualData);
    }
}