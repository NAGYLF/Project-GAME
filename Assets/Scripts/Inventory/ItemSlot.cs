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
        public Item PartOfItem;

        private Color color;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Draggable"))
            {
                Debug.Log("Az objektum elérte a célpontot!");
                color = gameObject.GetComponent<Image>().color;
                gameObject.GetComponent<Image>().color = Color.yellow;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Draggable"))
            {
                gameObject.GetComponent<Image>().color = color;
            }
        }
    }
}
