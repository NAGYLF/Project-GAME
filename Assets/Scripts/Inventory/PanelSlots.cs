using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using System.Linq;
using System;
using static MainData.SupportScripts;
using UnityEngine.UI;

public class PanelSlots : MonoBehaviour
{
    public static GameObject TheObject;//mivel egy panelslotbol egy lehet mindig ezert az eleresi utvonala a script tartalma lehet;
    public GameObject Tartget;//ahova az itemcontainerek kerulni fognak
    public GameObject Scrollbar;
    public void Awake()
    {
        TheObject = gameObject;
    }
}
