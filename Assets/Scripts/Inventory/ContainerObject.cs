using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using static PlayerInventoryClass.PlayerInventory;
using static MainData.SupportScripts;
using Assets.Scripts.Inventory;
using PlayerInventoryClass;


public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapj�n vizualiz�lja es szinkroniz�lja az itemeket az az ha valami ezt az objektumot staterk�nt megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    #endregion

    #region Personal variables
    public List<DataGrid> Sectors;//ez egy m�trox lista amely tartalmazza az �sszes itemSlot Objectumot
    public GameObject[] SectorObjects;//ez egy gamObject lista amely tatalmmazza az �sszes sectort m�retez�si �s itemObject el�r�s�nek sz�nd�k�b�l
    #endregion

    #region Active Slot Handler variables
    //Ezen v�ltoz�k sz�ks�gesek ahoz, hogy egy itemet helyezni tudjunk slotokb�l slotokba
    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public GameObject PlaceableObject;
    private int activeSlotsCount = 0;
    #endregion

    #region Active Slot Handler
    //Ezen elj�r�sok sz�ks�gesek ahoz, hogy egy itemet helyezni tudjunk slotokb�l slotokba
    private IEnumerator Targeting()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject;
            if ((activeSlots.First().GetComponent<ItemSlot>().IsEquipment && activeSlots.Count == 1) || (activeSlots.Count == PlaceableObject.GetComponent<ItemObject>().ActualData.SizeX * PlaceableObject.GetComponent<ItemObject>().ActualData.SizeY))
            {
                PlacerStruct placer = new PlacerStruct(activeSlots,ActualData);
                ActualData.GivePlacer = placer;
                PlaceableObject.GetComponent<ItemObject>().AvaiableNewParentObject = gameObject;
            }
        }
        yield return null;
    }
    private void Update()
    {
        if (activeSlots.Count != activeSlotsCount)
        {
            StartCoroutine(Targeting());
        }
    }
    private void Awake()
    {
        for (int sector = 0; sector < Sectors.Count; sector++)
        {
            for (int col = 0; col < Sectors[sector].columnNumber; col++)
            {
                for (int row = 0; row < Sectors[sector].rowNumber; row++)
                {
                    Sectors[sector].col[col].row[row].GetComponent<ItemSlot>().ParentObject = gameObject;
                }
            }
        }
        activeSlots = new List<GameObject>();
    }
    #endregion
    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()//az objecktum l�trehoz�s�nak els� pillanat�ban t�lt�dik be
    {
        if (ActualData.IsEquipment)
        {
            VisualisationToSlotPanel();
        }
        else if (ActualData.IsLoot)
        {
            VisualisationToLootPanel();
        }
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//l�trehozzuk itemObjektumait
        {
            Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject;
            itemObject = CreatePrefab(ActualData.Container.Items[i].ObjectPath);
            itemObject.name = ActualData.Container.Items[i].ItemName;
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i],ActualData);//L�trehozzuk a szikroniz�l�si utat ezen VirtualParentObject �s a VirtualChildrenObject k�z�tt
        }
    }
    public void SetDataRoute(Item Data)//(ezen elj�r�s ezen objektum j�t�kbaker�l�se el�tt zajlik le)    C�lja, hogy a gy�k�rb�l tov�bb�tott �s egyben ennek az objektumnak sz�nt adatokat ezen VCP megkapja
    {
        ActualData = Data;
    }
    private void VisualisationToLootPanel()
    {
        GameObject lootObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().LootPanelObject;
        gameObject.transform.SetParent(lootObject.GetComponent<PanelLoot>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a m�retar�nyt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        ActualData.ContainerObject = gameObject;
        ActualData.SectorDataGrid = gameObject.GetComponent<ContainerObject>().Sectors;
    }
    private void VisualisationToSlotPanel()
    {
        GameObject slotObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().SlotPanelObject;
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a m�retar�nyt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }

        slotObject.GetComponent<PanelSlots>().ReFresh();
    }
}