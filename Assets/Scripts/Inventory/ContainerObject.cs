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
    public Item ActualData;//ezek alapj�n vizualiz�lja es szinkroniz�lja az itemeket az az ha valami ezt az objektumot staterk�nt megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    private GameObject VirtualParentObject;
    #endregion

    #region Personal variables
    public GameObject[] SectorManagers;//a sectormanagers tartalmazza listaba helyezve a sectorokat a sectorok pedig tartlamazzak az itemslotokat azok oszlop �s sorszamat
    #endregion
    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()//az objecktum l�trehoz�s�nak els� pillanat�ban t�lt�dik be
    {
        foreach (GameObject sector in SectorManagers)//beallitjuk a m�retar�nyt
        {
            sector.GetComponent<SectorManager>().Container = gameObject;
            sector.GetComponent<RectTransform>().localScale *= Main.SectorScale;
        }
        SelfVisualisation();//vizualiz�juk
        for (int i = 0; i < ActualData.Container.Items.Count; i++)//l�trehozzuk itemObjektumait
        {
            Debug.Log($"{ActualData.Container.Items[i].ItemName} creating into {ActualData.ItemName}'s container");
            GameObject itemObject = CreatePrefab("GameElements/ItemObject");
            itemObject.name = ActualData.Container.Items[i].ItemName;
            itemObject.GetComponent<ItemObject>().SetDataRoute(ActualData.Container.Items[i], gameObject);//L�trehozzuk a szikroniz�l�si utat ezen VirtualParentObject �s a VirtualChildrenObject k�z�tt
        }
    }
    #region VCO (VirtualChilderObjet) Synch -- Into --> This VPO (VirtualParentObject)
    public void DataOut(Item Data)//(a sender objectum mindig itemObjectum) Ezen eljaras c�lja, hogy ezen VPO-b�l t�r�lje a VCO adatait
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
        VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);//ezen v�ltoz�sokat tov�bb�tja Saj�t VPO-j�nak
    }
    public void DataIn(Item Data)//(a sender objectum mindig itemObjectum) Ezen elj�r�s c�la, hogy ezen VPO-ba hozz�adja a VCO adatait
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
    //a megv�ltozott itemobjektum szikroniz�lja uj adatait parentobjektum�val ki nem v�ltozott meg.
    public void DataUpdate(Item Data, GameObject SenderObject)//(a sender objectum mindig itemObjectum) Ezen elj�r�s c�la, hogy ezen VPO-ban m�dos�tsa a VCO adatait.
    {
        //az itemobejtum adat�nak idex�t egy itemslot id-b�l kapja meg 
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
    public void SetDataRoute(Item Data, GameObject VirtualParentObject)//(ezen elj�r�s ezen objektum j�t�kbaker�l�se el�tt zajlik le)    C�lja, hogy a gy�k�rb�l tov�bb�tott �s egyben ennek az objektumnak sz�nt adatokat ezen VCP megkapja
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