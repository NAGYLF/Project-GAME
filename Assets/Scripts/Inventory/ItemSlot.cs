using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using ItemHandler;

namespace Assets.Scripts
{ 
    public class ItemSlot : MonoBehaviour
    {
        public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
        public Item PartOfItem;//ezt adatként kaphatja meg
        public GameObject PartOfItemObject;//ezt vizualizációkor kapja és továbbiakban a vizualizációban lesz fumciója az iteomobjectum azonosításban
        public GameObject Sector;//a saját sectora ezzel végezteti az adatszinkronizációt és a az item elhelyezés azon problemajat,
                                 //hogyha nem egy szektoron belün, de egyszere anyi itemcontainer kerülne targetba ami eleglenne az item tarolasakor,
                                 //ekkor az item ellenorzi, hogy a tergetek egy sectorba tartoznak e.

        private Color color;
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Az objektum elérte a célpontot!");
            color = gameObject.GetComponent<Image>().color;
            gameObject.GetComponent<Image>().color = Color.yellow;

            
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            gameObject.GetComponent<Image>().color = color;
        }
    }
}
