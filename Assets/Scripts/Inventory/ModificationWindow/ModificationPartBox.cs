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
        GameObject temporaryAdvancedItemObject = CreatePrefab("GameElements/TemporaryAdvancedItemObject");
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
        List<Part> parts = AdvancedItem.PartCut(AdvancedItem.Parts[PartIndex]);
        AdvancedItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();

        window.ItemPartTrasformation();
        //Debug.LogWarning(part_ != null);
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
