using Assets.Scripts;
using ItemHandler;
using PlayerInventoryClass;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static MainData.SupportScripts;
using static PlayerInventoryClass.PlayerInventory;
using static ItemObject;

public class EquipmentSlot : MonoBehaviour
{
    #region DataSynch
    public EquipmnetStruct PartOfItemData;//ezek alapján vizualizálja es szinkronizálja az itemeket
    public GameObject PartOfItemObject;
    private GameObject Inventory;
    #endregion

    public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
    public string SlotName;
    
    [SerializeField] private string partofitem;
    public GameObject ActualPartOfItemObject;//ezt vizualizációkor kapja és továbbiakban a vizualizációban lesz fumciója az iteomobjectum azonosításban
    private Color color;

    [HideInInspector] public List<GameObject> activeSlots;
    [HideInInspector] public GameObject PlaceableObject;
    private PlacerStruct placer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((PartOfItemObject == null || PartOfItemObject.GetInstanceID() == collision.gameObject.GetInstanceID()) && (SlotType=="" || SlotType.Contains(collision.gameObject.GetComponent<ItemObject>().ActualData.ItemType)))
        {
            ActualPartOfItemObject = collision.gameObject;
            color = gameObject.GetComponent<Image>().color;
            gameObject.GetComponent<Image>().color = Color.yellow;
            activeSlots.Add(gameObject);
        }
        else
        {
            color = gameObject.GetComponent<Image>().color;
            gameObject.GetComponent<Image>().color = Color.red;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!(PartOfItemObject != null && PartOfItemObject.GetInstanceID() != collision.gameObject.GetInstanceID()))
        {
            ActualPartOfItemObject = null;
            activeSlots.Remove(gameObject);
        }
        gameObject.GetComponent<Image>().color = color;
    }
    private void Awake()
    {
        SlotName = gameObject.name;
        activeSlots = new List<GameObject>();
        placer.activeItemSlots = new List<GameObject>();
    }
    private void Update()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<EquipmentSlot>().ActualPartOfItemObject;
            placer.activeItemSlots = activeSlots;
            placer.NewVirtualParentObject = gameObject;
            PlaceableObject.GetComponent<ItemObject>().placer = placer;
        }
    }
    public void DataOut(Item Data, GameObject VirtualChildObject)
    {
        PartOfItemData.EquipmentItem = null;//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
        PartOfItemObject = null;
        Inventory.GetComponent<PlayerInventory>().EquipmentRefresh(PartOfItemData);
    }
    public void DataUpdate(Item Data, GameObject VirtualChildObject)
    {
        PartOfItemObject = VirtualChildObject;
        PartOfItemData.EquipmentItem = Data;//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
        Inventory.GetComponent<PlayerInventory>().EquipmentRefresh(PartOfItemData);
    }
    public void DataIn(Item Data, GameObject VirtualChildObject)
    {
        PartOfItemObject = VirtualChildObject;
        PartOfItemData.EquipmentItem = Data;//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
        PartOfItemObject.GetComponent<ItemObject>().SetDataRoute(PartOfItemData.EquipmentItem, gameObject);
        Inventory.GetComponent<PlayerInventory>().EquipmentRefresh(PartOfItemData);
    }
    public void SetRootDataRoute(EquipmnetStruct Data, GameObject inventory)//ezt csak is a parent hivhatja meg
    {
        Inventory = inventory;
        PartOfItemData = Data;
        DataLoad();
    }
    public void DataLoad()
    {
        //EquipmentSlot.cs --> ItemObject.cs
        if (PartOfItemData.EquipmentItem != null)
        {
            Debug.Log($"{PartOfItemData.EquipmentItem.ItemName} EquipmentSlot.cs ------- SetDataRoute --------> ItemObject.cs     (RootItemObject)");
            GameObject itemObject = CreatePrefab("GameElements/ItemObject");
            itemObject.name = PartOfItemData.EquipmentItem.ItemName;
            itemObject.GetComponent<ItemObject>().SetDataRoute(PartOfItemData.EquipmentItem, gameObject);//item adatok itemobjektumba való adatátvitele//itemobjektum létrehozása
            PartOfItemObject = itemObject;
        }
    }
}
