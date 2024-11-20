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
    public void DataLoad()//az objecktum létrehozásának elsõ pillanatában töltõdik be
    {
        foreach (GameObject sector in SectorManagers)//beallitjuk a méretarányt
        {
            sector.GetComponent<SectorManager>().Container = gameObject;
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        SelfVisualisation();//vizualizájuk
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//létrehozzuk itemObjektumait
        {
            Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab("GameElements/ItemObject");
            itemObject.name = ActualData.Container.Items[i].ItemName;
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], gameObject);//Létrehozzuk a szikronizálási utat ezen VirtualParentObject és a VirtualChildrenObject között
        }
    }
    #region VCO (VirtualChilderObjet) Synch -- Into --> This VPO (VirtualParentObject)
    public void DataOut(Item Data)//(a sender objectum mindig itemObjectum) Ezen eljaras célja, hogy ezen VPO-ból törölje a VCO adatait
    {
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        ActualData.Container.Items.RemoveAt(index);
        foreach (ItemSlotData[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlotData itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.SlotName))
                {
                    itemSlot.PartOfItemData = null;
                }
            }
        }
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);//ezen változásokat továbbítja Saját VPO-jának
    }
    public void DataIn(Item Data)//(a sender objectum mindig itemObjectum) Ezen eljárás céla, hogy ezen VPO-ba hozzáadja a VCO adatait
    {
        Data.SetSlotUseId();
        ActualData.Container.Items.Add(Data);
        foreach (ItemSlotData[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlotData itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.SlotName))
                {
                    itemSlot.PartOfItemData = Data;
                }
            }
        }
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
    }
    //a megváltozott itemobjektum szikronizálja uj adatait parentobjektumával ki nem változott meg.
    public void DataUpdate(Item Data, GameObject SenderObject)//(a sender objectum mindig itemObjectum) Ezen eljárás céla, hogy ezen VPO-ban módosítsa a VCO adatait.
    {
        //az itemobejtum adatának idexét egy itemslot id-ból kapja meg 
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        foreach (ItemSlotData[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlotData itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.SlotName))
                {
                    itemSlot.PartOfItemData = null;
                }
            }
        }
        Data.SetSlotUseId();
        foreach (ItemSlotData[,] sector in ActualData.Container.Sectors)
        {
            foreach (ItemSlotData itemSlot in sector)
            {
                if (Data.GetSlotUseId().Contains(itemSlot.SlotName))
                {
                    itemSlot.PartOfItemData = Data;
                }
            }
        }
        ActualData.Container.Items[index] = Data;
        SenderObject.GetComponent<ItemObject>().ActualData.SetSlotUseId();
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
    }
    #endregion
    #region VPO (VirtualParentObjet) Synch -- Into --> This VCO (VirtualChildrenObject)
    public void SetDataRoute(Item Data, GameObject VirtualParentObject)//(ezen eljárás ezen objektum játékbakerülése elõtt zajlik le)    Célja, hogy a gyökérbõl továbbított és egyben ennek az objektumnak szánt adatokat ezen VCP megkapja
    {
        ActualData = Data;
        this.VirtualParentObject = VirtualParentObject;
        Debug.LogWarning($"{Data.ItemName} ------- ref -------- At = ContainerObject.cs");
    }
    #endregion
    private void SelfVisualisation()
    {
        GameObject slotObject = SlotObject;
        RectTransform containerRectTranform = gameObject.GetComponent<RectTransform>();
        RectTransform SlotPanelObject = slotObject.GetComponent<RectTransform>();
        containerRectTranform.sizeDelta = new Vector2(SlotPanelObject.sizeDelta.x, containerRectTranform.sizeDelta.y * (SlotPanelObject.sizeDelta.x / containerRectTranform.sizeDelta.x));
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Tartget.transform, false);
    }
}