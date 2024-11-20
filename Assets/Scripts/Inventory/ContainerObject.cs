using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using System;
using static PlayerInventoryVisualBuild.PlayerInventoryVisual;
using static MainData.SupportScripts;


public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapján vizualizálja es szinkronizálja az itemeket az az ha valami ezt az objektumot staterként megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    private GameObject VirtualParentObject;
    #endregion

    #region Personal variables
    public GameObject[] SectorManagers;//a sectormanagers tartalmazza listaba helyezve a sectorokat a sectorok pedig tartlamazzak az itemslotokat azok oszlop és sorszamat
    #endregion
    private void Start()
    {
        DataLoad();
    }
    public void DataOut(Item Data)
    {
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        ActualData.Container.Items.RemoveAt(index);
        foreach (ItemSlot[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlot itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.name))
                {
                    itemSlot.PartOfItemData = null;
                }
            }
        }
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
    }
    public void DataIn(Item Data)
    {
        Data.SetSlotUseId();
        ActualData.Container.Items.Add(Data);
        foreach (ItemSlot[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlot itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.name))
                {
                    itemSlot.PartOfItemData = Data;
                }
            }
        }
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
    }
    //a megváltozott itemobjektum szikronizálja uj adatait parentobjektumával ki nem változott meg.
    public void DataUpdate(Item Data, GameObject VirtualChildrenObject)//csak itemobjektum hivhatja meg
    {
        //az itemobejtum adatának idexét egy itemslot id-ból kapja meg 
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        foreach (ItemSlot[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlot itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.name))
                {
                    itemSlot.PartOfItemData = null;
                }
            }
        }
        Data.SetSlotUseId();
        foreach (ItemSlot[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlot itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.name))
                {
                    itemSlot.PartOfItemData = Data;
                }
            }
        }
        ActualData.Container.Items[index] = Data;
        VirtualChildrenObject.GetComponent<ItemObject>().ActualData.SetSlotUseId();
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
    }
    public void SetDataRoute(Item Data,GameObject VirtualParentObject)
    {
        ActualData = Data;
        this.VirtualParentObject = VirtualParentObject;
        Debug.LogWarning($"{Data.ItemName} ------- ref -------- At = ContainerObject.cs");
    }
    public void DataLoad()
    {
        foreach (GameObject sector in SectorManagers)
        {
            sector.GetComponent<SectorManager>().Container = gameObject;
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        SelfVisualisation();
        for (int i = 0; i < ActualData.Container.Items.Count; i++)
        {
            Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab("GameElements/ItemObject");
            itemObject.name = ActualData.Container.Items[i].ItemName;
            if (itemObject != null)
            {
                Debug.Log($"{ActualData.Container.Items[i] == null}     {gameObject == null}    {i}");
                itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], gameObject);
            }
            else
            {
                Debug.LogError($"{ActualData.Container.Items[i].ItemName} item is null");
            }
        }
    }
    private void SelfVisualisation()
    {
        GameObject slotObject = SlotObject;
        RectTransform containerRectTranform = gameObject.GetComponent<RectTransform>();
        RectTransform SlotPanelObject = slotObject.GetComponent<RectTransform>();
        containerRectTranform.sizeDelta = new Vector2(SlotPanelObject.sizeDelta.x, containerRectTranform.sizeDelta.y * (SlotPanelObject.sizeDelta.x / containerRectTranform.sizeDelta.x));
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Tartget.transform, false);
    }
}
