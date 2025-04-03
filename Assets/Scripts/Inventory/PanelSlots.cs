using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts;
using PlayerInventoryClass;

public class PanelSlots : MonoBehaviour
{
    public GameObject Content;//ahova az itemcontainerek kerulni fognak
    public GameObject ScrollPanel;
    public GameObject PlayerInventoryObject;
    public void ReFresh()
    {
        var children = new List<Transform>();
        foreach (Transform child in Content.transform)
        {
            children.Add(child);
        }

        for (int i = 0; i < children.Count - 1; i++)
        {
            for (int j = 0; j < children.Count - i - 1; j++)
            {
                var childSlotTypeA = children[j].GetComponent<ContainerObject>().ActualData.ItemType;
                var childSlotTypeB = children[j + 1].GetComponent<ContainerObject>().ActualData.ItemType;

                int indexA = Array.FindIndex(PlayerInventoryObject.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().EquipmentsSlots, slot => slot.GetComponent<ItemSlot>().SlotType == childSlotTypeA);
                int indexB = Array.FindIndex(PlayerInventoryObject.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().EquipmentsSlots, slot => slot.GetComponent<ItemSlot>().SlotType == childSlotTypeB);

                if (indexA > indexB)
                {
                    var temp = children[j];
                    children[j] = children[j + 1];
                    children[j + 1] = temp;
                }
            }
        }
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }
}
