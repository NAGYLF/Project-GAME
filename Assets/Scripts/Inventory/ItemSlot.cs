using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ItemHandler;
using PlayerInventoryClass;
using TMPro;

namespace Assets.Scripts
{ 
    public class ItemSlot : MonoBehaviour
    {
        #region Equipment variables
        public bool IsEquipment = false;//csak az inspectorban allithato be
        #endregion

        [HideInInspector] public GameObject ParentObject;//a saját sectora ezzel végezteti az adatszinkronizációt és a az item elhelyezés azon problemajat,
                                                   //hogyha nem egy szektoron belün, de egyszere anyi itemcontainer kerülne targetba ami eleglenne az item tarolasakor,
                                                   //ekkor az item ellenorzi, hogy a tergetek egy sectorba tartoznak e.
        public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.

        #region  Runtime Instantiated Objects Datas
        public GameObject PartOfItemObject;//ezen értéket egy itemslot egy item vizualizációjakor kellene hogy kapjon
        public GameObject ActualPartOfItemObject;//ezt vizualizációkor kapja és továbbiakban a vizualizációban lesz fumciója az iteomobjectum azonosításban
        public bool CountAddAvaiable = false;
        private Color color;
        public GameObject Title;
        public Image Background;
        #endregion
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((PartOfItemObject == null || (PartOfItemObject.GetInstanceID() == collision.gameObject.GetInstanceID())) && (SlotType == "" || SlotType.Contains(collision.gameObject.GetComponent<ItemObject>().ActualData.ItemType)))
            {
                ActualPartOfItemObject = collision.gameObject;
                ParentObject.GetComponent<ContainerObject>().activeSlots.Add(gameObject);
                color = Background.color;
                Background.color = Color.yellow;
            }
            else if (PartOfItemObject != null && PartOfItemObject.GetComponent<ItemObject>().ActualData.ItemName == collision.gameObject.GetComponent<ItemObject>().ActualData.ItemName && PartOfItemObject.GetComponent<ItemObject>().ActualData.Quantity != PartOfItemObject.GetComponent<ItemObject>().ActualData.MaxStackSize)
            {
                ActualPartOfItemObject = collision.gameObject;
                color = Background.color;
                Background.color = Color.yellow;
                CountAddAvaiable = true;
                ParentObject.GetComponent<ContainerObject>().activeSlots.Add(gameObject);
            }
            else
            {
                color = Background.color;
                Background.color = Color.red;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            ParentObject.GetComponent<ContainerObject>().activeSlots.Remove(gameObject);
            ActualPartOfItemObject = null;
            CountAddAvaiable = false;
            Background.color = color;
        }
        private void Awake()
        {
            if (!IsEquipment)
            {
                Title.GetComponent<TextMeshPro>().text = SlotType;
            }
        }
    }
}