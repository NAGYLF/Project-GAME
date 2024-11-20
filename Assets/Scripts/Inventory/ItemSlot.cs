using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ItemHandler;

namespace Assets.Scripts
{ 
    public class ItemSlot : MonoBehaviour
    {
        public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
        [HideInInspector] public GameObject Sector;//a saját sectora ezzel végezteti az adatszinkronizációt és a az item elhelyezés azon problemajat,
                                 //hogyha nem egy szektoron belün, de egyszere anyi itemcontainer kerülne targetba ami eleglenne az item tarolasakor,
                                 //ekkor az item ellenorzi, hogy a tergetek egy sectorba tartoznak e.

        public Item PartOfItemData;//ezt adatként kaphatja meg
        [SerializeField] private string partofitem;//csak testeléshez kell
        public GameObject PartOfItemObject;
        public GameObject ActualPartOfItemObject;//ezt vizualizációkor kapja és továbbiakban a vizualizációban lesz fumciója az iteomobjectum azonosításban
        public bool CoundAddAvaiable = false;

        private Color color;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (PartOfItemObject == null || (PartOfItemObject.GetInstanceID() == collision.gameObject.GetInstanceID()) && (SlotType == "" || SlotType.Contains(collision.gameObject.GetComponent<ItemObject>().ActualData.ItemType)))
            {
                ActualPartOfItemObject = collision.gameObject;
                Sector.GetComponent<SectorManager>().activeSlots.Add(gameObject);
                color = gameObject.GetComponent<Image>().color;
                gameObject.GetComponent<Image>().color = Color.yellow;
            }
            else if (PartOfItemObject != null && PartOfItemObject.GetComponent<ItemObject>().ActualData.ItemName == collision.gameObject.GetComponent<ItemObject>().ActualData.ItemName && PartOfItemObject.GetComponent<ItemObject>().ActualData.Quantity != PartOfItemObject.GetComponent<ItemObject>().ActualData.MaxStackSize)
            {
                ActualPartOfItemObject = collision.gameObject;
                color = gameObject.GetComponent<Image>().color;
                gameObject.GetComponent<Image>().color = Color.yellow;
                CoundAddAvaiable = true;
                Sector.GetComponent<SectorManager>().activeSlots.Add(gameObject);
            }
            else
            {
                color = gameObject.GetComponent<Image>().color;
                gameObject.GetComponent<Image>().color = Color.red;
            }

        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            Sector.GetComponent<SectorManager>().activeSlots.Remove(gameObject);
            ActualPartOfItemObject = null;
            CoundAddAvaiable = false;
            gameObject.GetComponent<Image>().color = color;
        }
        private void Update()
        {
            if (PartOfItemData != null)
            {
                partofitem = PartOfItemData.ItemName;
            }
            else
            {
                partofitem = "ures";
            }
        }
    }
}