using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEquipments : MonoBehaviour
{
    public GameObject[] EquipmentsSlots;
    public GameObject Equipments;//ezek az equipments-ek

    public GameObject EqipmentsPanel;
    public GameObject HealthPanel;

    public void OpenGearsPanel()
    {
        HealthPanel.SetActive(false);
        EqipmentsPanel.SetActive(true);
    }
    public void OpenHealtPanel()
    {
        HealthPanel.SetActive(true);
        EqipmentsPanel.SetActive(false);
    }
}
