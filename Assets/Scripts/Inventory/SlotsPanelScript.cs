using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using System.Linq;

public class SlotsPanelScript : MonoBehaviour
{
    public List<GameObject> Containers;
    private void Awake()
    {
        Containers = new List<GameObject>();
    }
    public void Start()
    {
        foreach (GameObject container in Containers)
        {
            List<Item> Items = container.GetComponent<Container>().Items;
            for (int i = 0; i < Items.Count; i++)
            {
                GameObject ItemObject = new GameObject($"{Items[i].Name}");
                foreach (Transform childerObject in gameObject.transform)
                {
                    if (Items[i].SlotUse.Contains(childerObject.name))
                    {
                        childerObject.transform.SetParent(ItemObject.transform);
                    }
                }
                ItemObject.AddComponent<Item>().SetItem(Items[i]);
            }
        }
    }
    public void UpdateSlotPanel()
    {
        
    }
}
