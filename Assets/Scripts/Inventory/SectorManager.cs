using Assets.Scripts;
using ItemHandler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObject;

public class SectorManager : MonoBehaviour
{
    [HideInInspector] public GameObject Container;//a container object statikus
    public GameObject[] ItemSlots;//az itemslotok statikusan az inspectorban allitodnak be
    public int row;//statikusan az inspectorban allitodik be
    public int columb;//statikusan az inspectorban allitodik be

    [HideInInspector] public List<GameObject> activeSlots;

    [HideInInspector] public GameObject PlaceableObject;

    private PlacerStruct placer; 

    private void Awake()
    {
        for (int i = 0; i < ItemSlots.Length; i++)
        {
            ItemSlots[i].GetComponent<ItemSlot>().Sector = gameObject;
        }
        activeSlots = new List<GameObject>();
        placer.activeItemSlots = new List<GameObject>();
    }
    private IEnumerator Targeting()
    {
        if (activeSlots.Count > 0)
        {
            PlaceableObject = activeSlots.First().GetComponent<ItemSlot>().ActualPartOfItemObject;
            if (activeSlots.Count == PlaceableObject.GetComponent<ItemObject>().ActualData.SizeX * PlaceableObject.GetComponent<ItemObject>().ActualData.SizeY)
            {
                placer.activeItemSlots = activeSlots;
                placer.NewVirtualParentObject = Container;
                PlaceableObject.GetComponent<ItemObject>().placer = placer;
            }
        }
        yield return null;
    }
    private int activeSlotsCount = 0;
    private void Update()
    {
        if (activeSlots.Count != activeSlotsCount)
        {
            StartCoroutine(Targeting());
        }
    }
}
