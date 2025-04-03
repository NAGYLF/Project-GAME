using ItemHandler;
using PlayerInventoryClass;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

public class PanelMain : MonoBehaviour
{
    public GameObject[] EquipmentsSlots;
    public GameObject Equipments;//ezek az equipments-ek

    public GameObject EqipmentsPanel;
    public GameObject HealthPanel;

    public void OpenGearsPanel()
    {
        HealthPanel.SetActive(false);
        EqipmentsPanel.SetActive(true);

        //fix (ha heat panellel nyitjuk meg az inventory akkor az equipmenteket nem tudja fittelni.)
        InGameUI.PlayerInventory.GetComponent<PlayerInventory>().levelManager.Items.Where(item => item.IsEquipment && item.SelfGameobject != null).ToList().ForEach(item => InventorySystem.ItemCompoundRefresh(item.SelfGameobject.GetComponent<ItemObject>().ItemCompound.GetComponent<ItemImgFitter>(),item));
        //fix end
    }
    public void OpenHealtPanel()
    {
        HealthPanel.SetActive(true);
        EqipmentsPanel.SetActive(false);
    }
}
