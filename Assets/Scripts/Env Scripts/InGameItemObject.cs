using MainData;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ItemHandler;
using System.Linq;

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
        }
        else
        {
            gameObject.name = "Hands";

            for (int i = ItemCompound.fitter.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(ItemCompound.fitter.transform.GetChild(i).gameObject);
            }
        }
    }
    public void SetDataRoute(AdvancedItem Data)
    {
        ActualData = Data;
    }
    public void SelfVisualisation()//az adatok alapjan vizualizalja az itemet
    {
        InventorySystem.ItemCompoundRefresh_Live(ItemCompound,ActualData);
    }
}