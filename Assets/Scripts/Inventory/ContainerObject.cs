using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using System;
using static PlayerInventoryVisualBuild.PlayerInventoryVisual;


public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapján vizualizálja es szinkronizálja az itemeket
    private Item RefData;
    private GameObject StarterObject;
    #endregion

    #region Personal variables
    public GameObject[] SectorManagers;//a sectormanagers tartalmazza listaba helyezve a sectorokat a sectorok pedig tartlamazzak az itemslotokat azok oszlop és sorszamat
    private List<GameObject> ContentItemObjects;//az objectek torlesere kell
    #endregion
    private void Start()
    {
        foreach (GameObject sector in SectorManagers)
        {
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        ContentItemObjects = new List<GameObject>();
        SelfVisualisation();
    }
    private void Update()
    {
        DataSynch();
    }
    private void DataSynch()
    {
        if (ActualData != RefData)
        {
            Debug.Log($"{ActualData.ItemName} item in Container checking for refresh:  {Array.Exists(ActualData.SlotUse, slot => slot.Contains("EquipmentSlot"))}");
            StarterObject.GetComponent<ItemObject>().ActualData = ActualData;
            RefData = ActualData;
            SetContent();
        }
    }
    public void SetDataRoute(Item Data,GameObject Starter)
    {
        ActualData = Data;
        StarterObject = Starter;
        Debug.LogWarning($"{Data.ItemName} ------- ref -------- At = ContainerObject.cs");
    }
    private void SelfVisualisation()
    {
        GameObject slotObject = SlotObject;
        RectTransform containerRectTranform = gameObject.GetComponent<RectTransform>();
        RectTransform SlotPanelObject = slotObject.GetComponent<RectTransform>();
        containerRectTranform.sizeDelta = new Vector2(SlotPanelObject.sizeDelta.x, containerRectTranform.sizeDelta.y * (SlotPanelObject.sizeDelta.x / containerRectTranform.sizeDelta.x));
        gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Tartget.transform, false);
    }
    private void SetContent()//csak a slotokat manipulálja
    {
        for (int i = 0; i < ActualData.Container.Items.Count; i++)
        {
            Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = new GameObject($"{ActualData.Container.Items[i].ItemName}");
            if (itemObject != null)
            {
                Debug.Log($"{ActualData.Container.Items[i]==null}     {gameObject==null}    {i}");
                itemObject.AddComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], gameObject, i);
                ContentItemObjects.Add(itemObject);
            }
            else
            {
                Debug.LogError($"{ActualData.Container.Items[i].ItemName} item is null");
            }

        }
    }
}
