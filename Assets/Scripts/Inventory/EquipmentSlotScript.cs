using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlotScript : MonoBehaviour
{
    public GameObject Contains;//a tartalmazott item objektum
    public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
    public string SlotName;
    // Start is called before the first frame update
    private void Awake()
    {
        SlotName = gameObject.name;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
