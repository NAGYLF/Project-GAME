using MainData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryClass;
using static MainData.SupportScripts;
using System;
using UnityEngine.UI;
using NaturalInventorys;
using ItemHandler;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        #region UI/HUD Inspector objects
        [SerializeField] public GameObject HUD;
        [SerializeField] public GameObject PlayerObject;
        [SerializeField] public Camera CameraObject;
        [SerializeField] public GameObject IntecativeObjectSelectorBox;
        [SerializeField] public GameObject InGameMenuObject;
        [SerializeField] public GameObject PlayerInventoryObject;
        [SerializeField] public GameObject MessageBar;
        [SerializeField] public GameObject HealtBar;
        [SerializeField] public GameObject StaminaBar;
        [SerializeField] public GameObject HungerBar;
        [SerializeField] public GameObject ThirstBar;
        [SerializeField] public GameObject WorldMapObject;
        [SerializeField] public GameObject QuestLogObject;
        #endregion

        #region HOT - Bar Elements
        public Item HotKey0;//Ez egy mindig üres elem
        public Item Main1;
        public Item Main2;
        public Item Secondary;
        public Item Melee;
        public Item HotKey5;
        public Item HotKey6;
        public Item HotKey7;
        public Item HotKey8;
        public Item HotKey9;
        #endregion

        #region UI Other Variables
        GameObject DevConsoleObject = null;
        [HideInInspector] public GameObject SelectedObject;
        public List<GameObject> IntecativeObjects;
        #endregion

        #region UI Following Player Variables
        [SerializeField] private Vector3 offset;
        [SerializeField] private float damping;
        private Vector3 vel = Vector3.zero;
        #endregion

        #region UI Metods
        public static OpenCloseUI DevConsol;
        public static OpenCloseUI PlayerInventory;
        public static OpenCloseUI InGameMenu;
        public static OpenCloseUI WorldMap;
        public static OpenCloseUI QuestLog;
        #endregion
        private void Awake()
        {
            IntecativeObjects = new List<GameObject>();

            OpenCloseUI.Refress();//mivel statikus a valtoto ezert ami statiku az az alkalmazás egész futása alatt létezik, ezert ha én törlöm ezt a jelenetet és ujra betoltom
                                  //atol meg a regi gameobject refernciával bíró action tipusú változó eljarasai nem törlõdnek csak ujjak addolódnak hozzá. ezert töröljük õket

            #region UI Metods Builds
            DevConsol = new OpenCloseUI(DevConsoleOpen, DevConsoleClose);
            PlayerInventory = new OpenCloseUI(PlayerInventoryOpen, PlayerInventoryClose);
            InGameMenu = new OpenCloseUI(InGameMenuOpen, InGameMenuClose);
            WorldMap = new OpenCloseUI(OpenMap,CloseMap);
            QuestLog = new OpenCloseUI(OpenQuest, CloseQuest);
            #endregion

            Application.targetFrameRate = Main.targetFPS;

            float cameraHeight = CameraObject.orthographicSize * 2f;
            float cameraWidth = cameraHeight * CameraObject.aspect;

            Main.DefaultWidth = cameraWidth;
            Main.DefaultHeight = cameraHeight;

            // Objektum méretei (a kamera méreteihez igazítva)
            gameObject.GetComponent<RectTransform>().position = CameraObject.transform.position;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(cameraWidth, cameraHeight);
        }

        private void FixedUpdate()
        {
            Vector3 targetPosition = PlayerObject.transform.position + offset;
            targetPosition.z = transform.position.z;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
        }
        #region DevConsole UI parts
        private void DevConsoleOpen()
        {
            DevConsoleObject = CreatePrefab("GameElements/DevConsole");
            DevConsoleObject.GetComponent<DevConsol>().Player = PlayerObject;
            DevConsoleObject.GetComponent<DevConsol>().inventory = PlayerInventoryObject;
            DevConsoleObject.transform.SetParent(transform);
            CloseHUD();
        }
        private void DevConsoleClose()
        {
            Destroy(DevConsoleObject);
            OpenHUD();
        }
        #endregion

        #region PlayerInventory UI parts
        private void PlayerInventoryOpen()
        {
            IntecativeObjectSelectorBox.SetActive(false);
            PlayerInventoryObject.GetComponent<PlayerInventory>().OpenInventory();
            CloseHUD();
        }
        private void PlayerInventoryClose()
        {
            PlayerInventoryObject.GetComponent<PlayerInventory>().LootableObject = null;
            SelectedObject = null;
            IntecativeObjectSelectorBox.SetActive(true);
            PlayerInventoryObject.GetComponent<PlayerInventory>().CloseInventory();
            OpenHUD();
        }
        #endregion

        #region InGameMenu parts
        private void InGameMenuOpen()
        {
            IntecativeObjectSelectorBox.SetActive(false);
            InGameMenuObject.SetActive(true);
            CloseHUD();
        }
        private void InGameMenuClose()
        {
            IntecativeObjectSelectorBox.SetActive(true);
            InGameMenuObject.SetActive(false);
            OpenHUD();
        }
        #endregion

        #region WorldMapMenu parts
        private void OpenMap()
        {
            WorldMapObject.SetActive(true);
            CloseHUD();
        }
        private void CloseMap()
        {
            WorldMapObject.SetActive(false);
            OpenHUD();
        }
        #endregion
        #region QuestLog parts
        private void OpenQuest()
        {
            QuestLogObject.SetActive(true);
            CloseHUD();
        }
        private void CloseQuest()
        {
            QuestLogObject.SetActive(false);
            OpenHUD();
        }
        #endregion
        private void OnGUI()
        {
            if (Event.current != null && Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.F1:
                        DevConsol.Action();
                        break;
                    case KeyCode.Tab:
                        PlayerInventory.Action();
                        break;
                    case KeyCode.Escape:
                        InGameMenu.Action();
                        break;
                    case KeyCode.X:
                        IntecativeObjectSelectorBox.GetComponent<InteractiveObjectSelector>().Selection(1);
                        break;
                    case KeyCode.Y:
                        IntecativeObjectSelectorBox.GetComponent<InteractiveObjectSelector>().Selection(-1);
                        break;
                    case KeyCode.F:
                        SelectedObject = IntecativeObjectSelectorBox.GetComponent<InteractiveObjectSelector>().SelectedObject;
                        if (SelectedObject != null)
                        {
                            Metodes(SelectedObject.GetComponent<Interact>().ActionMode);
                        }
                        break;
                    case KeyCode.M:
                        WorldMap.Action();
                        break;
                    case KeyCode.Q:
                        QuestLog.Action();
                        break;
                    case KeyCode.Alpha2:
                        break;
                    case KeyCode.Alpha3:
                        break;
                    case KeyCode.Alpha4:
                        break;
                    case KeyCode.Alpha5:
                        break;
                    case KeyCode.Alpha6:
                        break;
                    case KeyCode.Alpha7:
                        break;
                    case KeyCode.Alpha8:
                        break;
                    case KeyCode.Alpha9:
                        break;
                    default:
                        break;
                }
            }
        }
        #region Inform Bars
        public void SetHealtBar(float count)
        {
            HealtBar.GetComponent<Slider>().value = count;
        }
        public void SetStaminatBar(float count)
        {
            StaminaBar.GetComponent<Slider>().value = count;
        }
        public void SetHungerBar(float count)
        {
            HungerBar.GetComponent<Slider>().value = count;
        }
        public void SetThirstBar(float count)
        {
            ThirstBar.GetComponent<Slider>().value = count;
        }
        #endregion

        #region Interactions
        private void Metodes(string ActionMode)
        {
            switch (ActionMode)
            {
                case "OpenSimpleInventory":
                    OpenSimpleInventory();
                    break;
                default:
                    break;
            }
        }
        private void OpenSimpleInventory()
        {
            SelectedObject.GetComponent<Interact>().Opened = true;
            PlayerInventoryObject.GetComponent<PlayerInventory>().LootableObject = SelectedObject;
            PlayerInventory.Action();
            PlayerInventoryObject.GetComponent<PlayerInventory>().LootCreate();

        }
        #endregion

        #region HUD Metodes
        public void OpenHUD()
        {
            HUD.SetActive(true);
        }
        private void CloseHUD()
        {
            HUD.SetActive(false);
        }
        #endregion
    }


    public class OpenCloseUI
    {
        private static readonly List<OpenCloseUI> allInstances = new();

        private readonly Action open;
        private readonly Action close;
        public bool Status;
        public void Action()
        {
            foreach (var instance in allInstances)
            {
                if (instance == this)
                {
                    if (Status)
                    {
                        instance.open.Invoke();
                        Status = false;
                    }
                    else
                    {
                        instance.close.Invoke();
                        Status = true;
                    }
                }
                else
                {
                    if (!instance.Status)
                    {
                        instance.close.Invoke();
                        instance.Status = true;
                    }
                }
            }
        }
        public static void Refress()
        {
            allInstances.Clear();
        }
        public OpenCloseUI(Action open, Action close)
        {
            this.open = open;
            this.close = close;
            this.Status = true;//true = closed
            allInstances.Add(this);
        }
    }
}