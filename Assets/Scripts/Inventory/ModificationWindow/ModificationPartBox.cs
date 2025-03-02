using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static MainData.SupportScripts;
using ItemHandler;
using System.Linq;

public class ModificationPartBox : MonoBehaviour, IPointerDownHandler
{
    public Item AdvancedItem;
    public int PartIndex;
    public ModificationWindow window;
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogWarning("DragMouse at box");


        List<Part> parts_ = new List<Part>()
        {
            AdvancedItem.Parts[PartIndex]
        };
        AdvancedItem.Parts[PartIndex].GetConnectedPartsTree(parts_);

        ((int X, int Y) ChangedSize, Dictionary<char, int> Directions) Effect = InventorySystem.AdvancedItem_SizeChanger_EffectDetermination(AdvancedItem, parts_, false);
        (HashSet<(int X, int Y)> NonLiveCoordinates, int SectorIndex, bool IsPositionAble) NewPosition = InventorySystem.Try_PartPositioning(AdvancedItem, Effect.ChangedSize, Effect.Directions);

        List<Part> parts = AdvancedItem.PartCut(AdvancedItem.Parts[PartIndex]);

        if (NewPosition.IsPositionAble)
        {
            //fix ezt forditva mukodik elv
            InventorySystem.NonLive_Positioning(NewPosition.NonLiveCoordinates.First().Y, NewPosition.NonLiveCoordinates.First().X, NewPosition.SectorIndex, AdvancedItem, AdvancedItem.ParentItem);

            InventorySystem.NonLive_UnPlacing(AdvancedItem);
            InventorySystem.NonLive_Placing(AdvancedItem, AdvancedItem.ParentItem);

            InventorySystem.Live_UnPlacing(AdvancedItem);
            InventorySystem.Live_Placing(AdvancedItem, AdvancedItem.ParentItem);
        }

        AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();

        window.ItemPartTrasformation();

        GameObject temporaryAdvancedItemObject = CreatePrefab(Item.TemporaryItemObjectPath);
        Item newAdvancedItem = new()
        {
            Parts = new List<Part>()
        };

        //Debug.LogWarning("-----------------Advenced item parts---------------------");
        //foreach (Part item in AdvancedItem.Parts)
        //{
        //    Debug.LogWarning($"{item.PartData.PartName}");
        //}
        //Debug.LogWarning($" Advanced Item PartCut delete from part:  {AdvancedItem.Parts.First(part => part.PartData.PartName == PartName).PartData.PartName}");
        //Debug.LogWarning("-----------------partt --> parts---------------------");
        //foreach (ConnectionPoint cp in part_.ConnectionPoints)
        //{
        //    if (cp.Used)
        //    {
        //        Debug.LogWarning($"{cp.SelfPart.PartData.PartName}");
        //    }
        //}
        //part_.GetConnectedPartsTree(parts);
        //parts = AdvancedItem.Parts.First(part=>part.PartData.PartName == PartName).GetConnectedPartsTree();
        //Debug.LogWarning("-----------------parts---------------------");
        //foreach (Part item in parts)
        //{
        //    Debug.LogWarning($"{item.PartData.PartName}");
        //}
        foreach (Part part in parts)
        {
            newAdvancedItem.Parts.Add(part);
        }
        newAdvancedItem.AdvancedItemContsruct();
        temporaryAdvancedItemObject.GetComponent<TemporaryItemObject>().SetDataRoute(newAdvancedItem);
        temporaryAdvancedItemObject.GetComponent<TemporaryItemObject>().AdvancedItem = AdvancedItem;
        temporaryAdvancedItemObject.GetComponent<TemporaryItemObject>().window = window;
    }
}