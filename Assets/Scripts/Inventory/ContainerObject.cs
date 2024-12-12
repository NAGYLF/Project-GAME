using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using MainData;
using Assets.Scripts;
using System.Linq;
using System;
using static PlayerInventoryClass.PlayerInventory;
using static MainData.SupportScripts;
using NaturalInventorys;
using Assets.Scripts.Inventory;
using static ItemObject;


public class ContainerObject : MonoBehaviour
{
    #region DataSynch
    public Item ActualData;//ezek alapj�n vizualiz�lja es szinkroniz�lja az itemeket az az ha valami ezt az objektumot staterk�nt megkapja akkor ezt kell megvaltoztatni a szinkronizalashoz
    private GameObject VirtualParentObject;
    #endregion

    #region Personal variables
    private int activeSlotsCount = 0;
    public List<DataGrid> Sectors;//ez egy m�trox lista amely tartalmazza az �sszes itemSlot Objectumot
    public GameObject[] SectorObjects;//ez egy gamObject lista amely tatalmmazza az �sszes sectort m�retez�si �s itemObject el�r�s�nek sz�nd�k�b�l
    #endregion

    #region Active Slot Handler variables
    //Ezen v�ltoz�k sz�ks�gesek ahoz, hogy egy itemet helyezni tudjunk slotokb�l slotokba
    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public GameObject PlaceableObject;
    private PlacerStruct placer;
    #endregion

    [Serializable]
    public class DataGrid
    {
        public int rowNumber;       // M�trix sorainak sz�ma
        public int columnNumber;    // M�trix oszlopainak sz�ma
        public List<RowData> col; // M�trix adatai
    }

    [Serializable]
    public class RowData
    {
        public List<GameObject> row; // Egyetlen sor adatai
    }
    private IEnumerator Targeting()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject;
            if (activeSlots.Count == PlaceableObject.GetComponent<ItemObject>().ActualData.SizeX * PlaceableObject.GetComponent<ItemObject>().ActualData.SizeY)
            {
                placer.activeItemSlots = activeSlots;
                placer.NewVirtualParentObject = gameObject;
                PlaceableObject.GetComponent<ItemObject>().placer = placer;
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
        placer.activeItemSlots = new List<GameObject>();
    }
    private void Start()
    {
        DataLoad();
    }
    public void DataLoad()//az objecktum l�trehoz�s�nak els� pillanat�ban t�lt�dik be
    {
        foreach (GameObject sector in SectorObjects)//beallitjuk a m�retar�nyt
        {
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
        if (VirtualParentObject.GetComponent<ItemObject>() != null)
        {
            VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
        }
        else if (VirtualParentObject.GetComponent<SimpleInventory>())
        {
            VirtualParentObject.GetComponent<SimpleInventory>().DataUpdate(ActualData);
        }
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
        if (VirtualParentObject.GetComponent<ItemObject>() != null)
        {
            VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
        }
        else if (VirtualParentObject.GetComponent<SimpleInventory>())
        {
            VirtualParentObject.GetComponent<SimpleInventory>().DataUpdate(ActualData);
        }
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

        if (VirtualParentObject.GetComponent<ItemObject>() != null)
        {
            VirtualParentObject.GetComponent<ItemObject>().DataUpdate(ActualData);
        }
        else if (VirtualParentObject.GetComponent<SimpleInventory>())
        {
            VirtualParentObject.GetComponent<SimpleInventory>().DataUpdate(ActualData);
        }
    }
    #endregion
    #region VPO (VirtualParentObjet) Synch -- Into --> This VCO (VirtualChildrenObject)
    public void SetDataRoute(Item Data, GameObject VirtualParentObject)//(ezen elj�r�s ezen objektum j�t�kbaker�l�se el�tt zajlik le)    C�lja, hogy a gy�k�rb�l tov�bb�tott �s egyben ennek az objektumnak sz�nt adatokat ezen VCP megkapja
    {
        ActualData = Data;
        this.VirtualParentObject = VirtualParentObject;
        //Debug.LogWarning($"{Data.ItemName} ------- ref -------- At = ContainerObject.cs");
    }
    #endregion
    private void SelfVisualisation()
    {
        if (VirtualParentObject.GetComponent<ItemObject>() && VirtualParentObject.GetComponent<ItemObject>().VirtualParentObject.GetComponent<EquipmentSlot>())
        {
            GameObject slotObject = PlayerInventoryClass.PlayerInventory.SlotPanelObject;
            RectTransform containerRectTranform = gameObject.GetComponent<RectTransform>();
            RectTransform SlotPanelObject = slotObject.GetComponent<RectTransform>();
            containerRectTranform.sizeDelta = new Vector2(SlotPanelObject.sizeDelta.x, containerRectTranform.sizeDelta.y * (SlotPanelObject.sizeDelta.x / containerRectTranform.sizeDelta.x));
            gameObject.transform.SetParent(slotObject.GetComponent<PanelSlots>().Content.transform, false);
        }
        else if (VirtualParentObject.GetComponent<SimpleInventory>())
        {
            GameObject lootObject = LootPanelObject;
            RectTransform containerRectTranform = gameObject.GetComponent<RectTransform>();
            RectTransform lootPanelObject = lootObject.GetComponent<RectTransform>();
            containerRectTranform.sizeDelta = new Vector2(lootPanelObject.sizeDelta.x, containerRectTranform.sizeDelta.y * (lootPanelObject.sizeDelta.x / containerRectTranform.sizeDelta.x));
            gameObject.transform.SetParent(lootObject.GetComponent<PanelLoot>().Content.transform, false);
        }
    }
}