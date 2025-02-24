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
    public Item ActualData;//ezek alapján vizualizálja es szinkronizálja az itemeket az az ha valami ezt az objektumot staterként megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    #endregion

    #region Personal variables
    public List<DataGrid> Sectors;//ez egy mátrox lista amely tartalmazza az összes itemSlot Objectumot
    public GameObject[] SectorObjects;//ez egy gamObject lista amely tatalmmazza az összes sectort méretezési és itemObject elérésének szándékából
    #endregion

    #region Active Slot Handler variables
    //Ezen változók szükségesek ahoz, hogy egy itemet helyezni tudjunk slotokból slotokba
    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public GameObject PlaceableObject;
    #endregion

    #region Active Slot Handler
    //Ezen eljárások szükségesek ahoz, hogy egy itemet helyezni tudjunk slotokból slotokba
    private IEnumerator Targeting()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject;
            if (PlaceableObject.GetComponent<ItemObject>() != null)
            {
                if ((activeSlots.First().GetComponent<ItemSlot>().IsEquipment && activeSlots.Count == 1) || (activeSlots.Count == PlaceableObject.GetComponent<ItemObject>().ActualData.SizeX * PlaceableObject.GetComponent<ItemObject>().ActualData.SizeY))
                {
                    PlacerStruct placer = new PlacerStruct(activeSlots, ActualData);
                    ActualData.GivePlacer = placer;
                    PlaceableObject.GetComponent<ItemObject>().AvaiableNewParentObject = gameObject;
                }
            }
            else if(PlaceableObject.GetComponent<TemporaryItemObject>() != null)
            {
                if ((activeSlots.First().GetComponent<ItemSlot>().IsEquipment && activeSlots.Count == 1) || (activeSlots.Count == PlaceableObject.GetComponent<TemporaryItemObject>().ActualData.SizeX * PlaceableObject.GetComponent<TemporaryItemObject>().ActualData.SizeY))
                {
                    PlacerStruct placer = new PlacerStruct(activeSlots, ActualData);
                    ActualData.GivePlacer = placer;
                    PlaceableObject.GetComponent<TemporaryItemObject>().AvaiableNewParentObject = gameObject;
                }
            }
        }
        yield return null;
    }
    private void Update()
    {
        StartCoroutine(Targeting());
    }
    public void SetDataRoute(Item Data)//(ezen eljárás ezen objektum játékbakerülése elõtt zajlik le)    Célja, hogy a gyökérbõl továbbított és egyben ennek az objektumnak szánt adatokat ezen VCP megkapja
    {
        ActualData = Data;
    }
    #endregion
    public void Start()
    {
        Inicialisation();
        LoadItemObjects();
    }
    public void Inicialisation()//az objecktum létrehozásának elsõ pillanatában töltõdik be
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

        ActualData.ContainerObject = gameObject;
        ActualData.Container.Live_Sector = gameObject.GetComponent<ContainerObject>().Sectors;

        if (ActualData.IsEquipment)
        {
            VisualisationToSlotPanel();
        }
        else if (ActualData.IsLoot)
        {
            VisualisationToLootPanel();
        }
    }
    private void LoadItemObjects()
    {
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//létrehozzuk itemObjektumait
        {
            //Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab(ActualData.Container.Items[i].ObjectPath);
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], ActualData);//Létrehozzuk a szikronizálási utat ezen VirtualParentObject és a VirtualChildrenObject között
        }
    }
    private void VisualisationToLootPanel()
    {
        GameObject lootObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().LootPanelObject;
        gameObject.transform.SetParent(lootObject.GetComponent<PanelLoot>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a méretarányt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        //ActualData.ContainerObject = gameObject;
        //ActualData.Container.Live_Sector = gameObject.GetComponent<ContainerObject>().Sectors;
    }
    private void VisualisationToSlotPanel()
    {
        GameObject slotObject = InventoryObjectRef.gameObject.GetComponent<PlayerInventory>().SlotPanelObject;
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Content.transform, false);
        foreach (GameObject sector in SectorObjects)//beallitjuk a méretarányt
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }

        slotObject.GetComponent<PanelSlots>().ReFresh();
    }
}