using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class PanelLoot : MonoBehaviour
    {
        public static GameObject TheObject;//mivel egy panelLootból egy lehet mindig ezert az eleresi utvonala a script tartalma lehet;
        public GameObject Tartget;//ahova az itemcontainerek kerulni fognak
        public GameObject Scrollbar;
        public void Awake()
        {
            TheObject = gameObject;
        }
    }
}