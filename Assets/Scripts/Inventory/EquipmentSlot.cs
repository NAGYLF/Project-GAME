using ItemHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainData.SupportScripts;
using static PlayerInventoryClass.PlayerInventory;

public class EquipmentSlot : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapj�n vizualiz�lja es szinkroniz�lja az itemeket
    private Item RefData;
    #endregion

    public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
    public string SlotName;
    // Start is called before the first frame update
    private void Awake()
    {
        SlotName = gameObject.name;
    }
    public void Update()
    {
        DataSynch();
    }
    private void DataSynch()
    {
        if (ActualData != RefData && ActualData != null)//resetelni kell a slotot
        {
            ItemVisualisation();
            RefData = ActualData;
        }
        else if (ActualData == null)
        {
            RefData = null;
        }
    }
    public void SetRootDataRoute(ref Item Data)
    {
        ActualData = Data;
    }
    private void ItemVisualisation()//10. az item vizu�lisan l�trej�n az equipmentslotban, objektuma tov�bb �r�kli az item adat�nak referenci�j�t
    {
        //--> ItemObject.cs (Item)
        Debug.LogWarning($"{ActualData.ItemName} EquipmentSlot.cs ------- ref --------> ItemObject.cs");
        GameObject itemObject = new GameObject($"{ActualData.ItemName}");
        itemObject.AddComponent<ItemObject>().SetDataRoute(ActualData, gameObject);//item adatok itemobjektumba val� adat�tvitele//itemobjektum l�trehoz�sa                                                                                          
    }
}
