using MainData;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryClass;
using System;
using UnityEngine.UI;
using ItemHandler;

namespace UI
{
    public class InGameUI : MonoBehaviour
    {
        #region UI/HUD Inspector objects
        public static GameObject InGameUI_;
        [SerializeField] private GameObject InGameUIObject;

        public static GameObject HUD;
        [SerializeField] private GameObject HUDObject;

        public static GameObject Player;
        [SerializeField] private GameObject PlayerObject;

        public static GameObject Camera;
        [SerializeField] private GameObject CameraObject;

        public static GameObject IntecativeObjectSelectorBox;
        [SerializeField] private GameObject IntecativeObjectSelectorBoxObject;

        public static GameObject InGameMenu;
        [SerializeField] private GameObject InGameMenuObject;

        public static GameObject PlayerInventory;
        [SerializeField] private GameObject PlayerInventoryObject;

        public static GameObject MessageBar;
        [SerializeField] private GameObject MessageBarObject;

        public static GameObject HealtBar;
        [SerializeField] private GameObject HealtBarObject;

        public static GameObject StaminaBar;
        [SerializeField] private GameObject StaminaBarObject;

        public static GameObject HungerBar;
        [SerializeField] public GameObject HungerBarObject;

        public static GameObject ThirstBar;
        [SerializeField] public GameObject ThirstBarObject;

        public static GameObject WorldMap;
        [SerializeField] public GameObject WorldMapObject;

        public static GameObject QuestLog;
        [SerializeField] public GameObject QuestLogObject;

        public static GameObject DevConsole;
        [SerializeField] public GameObject DevConsoleObject;
        #endregion

        #region HOT - Bar Elements
        public static Item HotKey0;//Ez egy mindig üres elem
        public static Item HotKey1;//ez a main1
        public static Item HotKey2;//ez a main2
        public static Item HotKey3;//ez a secondary
        public static Item HotKey4;//ez a melee
        public static Item HotKey5;
        public static Item HotKey6;
        public static Item HotKey7;
        public static Item HotKey8;
        public static Item HotKey9;

        public static void SetHotKey1(Item item)
        {
            if (HotKey1 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey1 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey1;
            }
            else
            {
                HotKey1 = item;
            }
        }
        public static void SetHotKey2(Item item)
        {
            if (HotKey2 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey2 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey2;
            }
            else
            {
                HotKey2 = item;
            }
        }
        public static void SetHotKey3(Item item)
        {
            if (HotKey3 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey3 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey3;
            }
            else
            {
                HotKey3 = item;
            }
        }
        public static void SetHotKey4(Item item)
        {
            if (HotKey4 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey4 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey4;
            }
            else
            {
                HotKey4 = item;
            }
        }
        public static void SetHotKey5(Item item)
        {
            if (HotKey5 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey5 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey5;
            }
            else
            {
                HotKey5 = item;
            }
        }
        public static void SetHotKey6(Item item)
        {
            if (HotKey6 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey6 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey6;
            }
            else
            {
                HotKey6 = item;
            }
        }
        public static void SetHotKey7(Item item)
        {
            if (HotKey7 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey7 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey7;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey7;
            }
            else
            {
                HotKey7 = item;
            }
        }
        public static void SetHotKey8(Item item)
        {
            if (HotKey8 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey8 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey8;
            }
            else
            {
                HotKey8 = item;
            }
        }
        public static void SetHotKey9(Item item)
        {
            if (HotKey9 == Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem)
            {
                HotKey9 = item;
                Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey9;
            }
            else
            {
                HotKey9 = item;
            }
        }
        #endregion

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

        #region UI Other Variables
        [HideInInspector] public GameObject SelectedObject;
        public List<GameObject> IntecativeObjects;
        #endregion

        #region UI Following Player Variables
        [SerializeField] private Vector3 offset;
        [SerializeField] private float damping;
        private Vector3 vel = Vector3.zero;
        #endregion

        #region UI Metods
        public static OpenCloseUI DevConsolOpenClose;
        public static OpenCloseUI PlayerInventoryOpenClose;
        public static OpenCloseUI InGameMenuOpenClose;
        public static OpenCloseUI WorldMapOpenClose;
        public static OpenCloseUI QuestLogOpenClose;
        #endregion
        private void Awake()
        {
            IntecativeObjects = new List<GameObject>();

            OpenCloseUI.Refress();//mivel statikus a valtoto ezert ami statiku az az alkalmazás egész futása alatt létezik, ezert ha én törlöm ezt a jelenetet és ujra betoltom
                                  //atol meg a regi gameobject refernciával bíró action tipusú változó eljarasai nem törlõdnek csak ujjak addolódnak hozzá. ezert töröljük õket

            #region UI Metods Builds
            DevConsolOpenClose = new OpenCloseUI(DevConsoleOpen, DevConsoleClose);
            PlayerInventoryOpenClose = new OpenCloseUI(PlayerInventoryOpen, PlayerInventoryClose);
            InGameMenuOpenClose = new OpenCloseUI(InGameMenuOpen, InGameMenuClose);
            WorldMapOpenClose = new OpenCloseUI(OpenMap,CloseMap);
            QuestLogOpenClose = new OpenCloseUI(OpenQuest, CloseQuest);
            #endregion

            #region GameObject Set
            HUD = HUDObject;
            Player = PlayerObject;
            Camera = CameraObject;
            IntecativeObjectSelectorBox = IntecativeObjectSelectorBoxObject;
            InGameMenu = InGameMenuObject;
            PlayerInventory = PlayerInventoryObject;
            MessageBar = MessageBarObject;
            HealtBar = HealtBarObject;
            StaminaBar = StaminaBarObject;
            HungerBar = HungerBarObject;
            ThirstBar = ThirstBarObject;
            WorldMap = WorldMapObject;
            QuestLog = QuestLogObject;
            DevConsole = DevConsoleObject;
            #endregion

            Application.targetFrameRate = Main.targetFPS;

            float cameraHeight = Camera.GetComponent<Camera>().orthographicSize * 2f;
            float cameraWidth = cameraHeight * Camera.GetComponent<Camera>().aspect;

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
            DevConsole.SetActive(true);
            CloseHUD();
        }
        private void DevConsoleClose()
        {
            DevConsole.SetActive(false);
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
                        DevConsolOpenClose.Action();
                        break;
                    case KeyCode.Tab:
                        PlayerInventoryOpenClose.Action();
                        break;
                    case KeyCode.Escape:
                        InGameMenuOpenClose.Action();
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
                        WorldMapOpenClose.Action();
                        break;
                    case KeyCode.Q:
                        QuestLogOpenClose.Action();
                        break;
                    case KeyCode.Alpha1:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey1;
                        }
                        break;
                    case KeyCode.Alpha2:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey2;
                        }
                        break;
                    case KeyCode.Alpha3:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey3;
                        }
                        break;
                    case KeyCode.Alpha4:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey4;
                        }
                        break;
                    case KeyCode.Alpha5:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey5;
                        }
                        break;
                    case KeyCode.Alpha6:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey6;
                        }
                        break;
                    case KeyCode.Alpha7:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey7;
                        }
                        break;
                    case KeyCode.Alpha8:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey8;
                        }
                        break;
                    case KeyCode.Alpha9:
                        if (HUD)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey9;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

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
            PlayerInventoryOpenClose.Action();
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
            allInstances.Remove(this);
            foreach (var instance in allInstances)
            {
                if (!instance.Status)
                {
                    instance.close.Invoke();
                    instance.Status = true;
                }
            }
            allInstances.Add(this);
            if (Status)
            {
                open.Invoke();
                Status = false;
            }
            else
            {
                close.Invoke();
                Status = true;
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