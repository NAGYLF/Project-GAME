using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using System;
using static PlayerInventoryVisualBuild.PlayerInventoryVisual;
using System.Reflection;


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
    public void DataOut(Item Data, GameObject VirtualChildrenObject)
    {
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        ActualData.Container.Items.RemoveAt(index);
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData,gameObject);
    }
    public void DataUpdate(Item Data, GameObject VirtualChildrenObject)
    {
        int index = ActualData.Container.Items.FindIndex(elem => elem.GetSlotUseId() == Data.GetSlotUseId());
        ActualData.Container.Items[index] = Data;
        Data.SetSlotUseId();
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData,gameObject);
    }
    public void DataIn(Item Data, GameObject VirtualChildrenObject)
    {
        Data.SetSlotUseId();
        ActualData.Container.Items.Add(Data);
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData,gameObject);
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
            GameObject itemObject = new GameObject(ActualData.Container.Items[i].ItemName);
            if (itemObject != null)
            {
                Debug.Log($"{ActualData.Container.Items[i] == null}     {gameObject == null}    {i}");
                itemObject.AddComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], gameObject);
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
