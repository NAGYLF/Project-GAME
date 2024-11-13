using Assets.Scripts;
using ItemHandler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ItemHandler.ItemObject;

public class EquipmentSlot : MonoBehaviour
{
    #region DataSynch
    private Item ActualData;//ezek alapján vizualizálja es szinkronizálja az itemeket
    private GameObject VirualChildObject;
    #endregion

    public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
    public string SlotName;
    
    public Item PartOfItem;//ezt adatként kaphatja meg

    public GameObject PartOfItemObject;
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
    private void Start()
    {
        DataLoad();
    }
    private void Update()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<EquipmentSlot>().ActualPartOfItemObject;
            placer.activeItemSlots = activeSlots;
            placer.newStarter = gameObject;
            PlaceableObject.GetComponent<ItemObject>().placer = placer;
        }
    }
    public void DataOut(Item Data, GameObject VirtualChildObject)
    {
        ActualData = new Item();//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
    }
    public void DataUpdate(Item Data, GameObject VirtualChildObject)
    {
        VirualChildObject = VirtualChildObject;
        ActualData = Data;//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
    }
    public void DataIn(Item Data, GameObject VirtualChildObject)
    {
        VirualChildObject = VirtualChildObject;
        ActualData = Data;//az actual data a gyökér adatokban modositja az adatokat ezert tovabbi szinkronizaciora nincs szukseg
        VirualChildObject.GetComponent<ItemObject>().SetDataRoute(ActualData, gameObject);
    }
    public void SetRootDataRoute(ref Item Data)//ezt csak is a parent hivhatja meg
    {
        ActualData = Data;
    }
    public void DataLoad()
    {
        //EquipmentSlot.cs --> ItemObject.cs
        if (ActualData != null)
        {
            Debug.LogWarning($"{ActualData.ItemName} EquipmentSlot.cs ------- ref --------> ItemObject.cs");
            GameObject itemObject = new GameObject($"{ActualData.ItemName}");
            itemObject.AddComponent<ItemObject>().SetDataRoute(ActualData, gameObject);//item adatok itemobjektumba való adatátvitele//itemobjektum létrehozása
            PartOfItemObject = itemObject;
        }
    }
}
