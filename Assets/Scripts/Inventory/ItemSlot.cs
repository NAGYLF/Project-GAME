using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEngine.UI;

namespace Assets.Scripts
{ 
    internal class ItemSlot : MonoBehaviour
    {
        public string SlotType;//azon tipusok melyeket befogadhat, ha nincs megadva akkor mindent.
        public string SlotName;
        public GameObject[] SectorCompanions;
        private void Awake()
        {
            SlotName = gameObject.name;
        }

    }
}
