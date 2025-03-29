using MainData;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryClass;
using System;
using UnityEngine.UI;
using ItemHandler;
using TMPro;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;

namespace UI
{
    public class HotKey
    {
        public AdvancedItem Item;

        public int Key;

        public bool IsInPlayerHand = false;

        public HotKey(int key)
        {
            Key = key;
        }
        public void SetWithUI(AdvancedItem item)
        {
            if (Item == item)
            {
                UnSetHotKey();
            }
            else
            {
                if (item.hotKeyRef != null)
                {
                    item.hotKeyRef.UnSetHotKey();
                }
                UnSetHotKey();
                SetHotKey(item);
            }
        }
        public void UnSetHotKey()
        {
            if (Item != null)
            {
                Item.HotKey = "";
                Item.hotKeyRef = null;
                if (IsInPlayerHand)
                {
                    InGameUI.Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().UnsetItem();
                }
                if (Item.SelfGameobject)
                {
                    Item.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                }
                Item = null;
            }
        }
        public void SetHotKey(AdvancedItem SetIn)
        {
            if (Item == null)
            {
                SetIn.HotKey = Key.ToString();
                SetIn.hotKeyRef = this;
                if (IsInPlayerHand)
                {
                    InGameUI.Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SetItem(SetIn);
                }
                if (SetIn.SelfGameobject)
                {
                    SetIn.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                }
                Item = SetIn;
            }
            else
            {
                Debug.LogError($"HotKey is occupied {Key}");
            }
        }
    }
    public class InGameUI : MonoBehaviour
    {
        #region UI/HUD Inspector objects
        public static GameObject InGameUI_;

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

        //public static GameObject MessageBar;
        //[SerializeField] private GameObject MessageBarObject;

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

        public static GameObject HealtIndicador;
        [SerializeField] public GameObject HealtIndicadorObject;

        public static GameObject StaminaIndicador;
        [SerializeField] public GameObject StaminaIndicadorObject;

        public static GameObject HungerIndicador;
        [SerializeField] public GameObject HungerIndicadorObject;

        public static GameObject ThirstyIndicador;
        [SerializeField] public GameObject ThirstyIndicadorObject;

        public static GameObject WeightIndicador;//Out of order
        [SerializeField] public GameObject WeightIndicadorObject;
        #endregion

        #region UI Other Variables
        public static bool SetHotKeyWithMouse = false;
        public static GameObject SetGameObjectToHotKey = null;

        public Texture2D cursorTexture;
        private bool isCustomCursorActive = false;

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

        #region Player Variables
        [HideInInspector] private float PlayerMovementSpeed = 50f;//default speed
        [HideInInspector] private float PlayerMovementSpeedMax = 100f;//sprint speed
        [HideInInspector] private float moveSpeed;//actual speeed
        [HideInInspector] private bool isSprinting = false;
        private float moveHorizontal = 0;
        private float moveVertical = 0;

        private const float MaxHealt = 100f;
        private float Healt = 100f;

        private const float MaxStamina = 100f;
        private float Stamina = 100f;

        private const float MaxHunger = 100f;
        private float Hunger = 100f;

        private const float MaxThirst = 100f;
        private float Thirst = 100f;

        private const float MaxWeight = 40f;
        //private float Weight = 0;
        #endregion

        #region HotKeySet
        public static HotKey HotKey0;//Ez egy mindig üres elem
        public static HotKey HotKey1;//ez a main1
        public static HotKey HotKey2;//ez a main2
        public static HotKey HotKey3;//ez a secondary
        public static HotKey HotKey4;//ez a melee
        public static HotKey HotKey5;
        public static HotKey HotKey6;
        public static HotKey HotKey7;
        public static HotKey HotKey8;
        public static HotKey HotKey9;
        #endregion

        private void Awake()
        {
            #region Player Part Inicialisation
            moveSpeed = PlayerMovementSpeed;
            #endregion

            IntecativeObjects = new List<GameObject>();

            OpenCloseUI.Refress();//mivel statikus a valtoto ezert ami statiku az az alkalmazás egész futása alatt létezik, ezert ha én törlöm ezt a jelenetet és ujra betoltom
                                  //atol meg a regi gameobject refernciával bíró action tipusú változó eljarasai nem törlõdnek csak ujjak addolódnak hozzá. ezert töröljük õket

            #region Set HotKey
            HotKey0 = new HotKey(0);
            HotKey1 = new HotKey(1);
            HotKey2 = new HotKey(2);
            HotKey3 = new HotKey(3);
            HotKey4 = new HotKey(4);
            HotKey5 = new HotKey(5);
            HotKey6 = new HotKey(6);
            HotKey7 = new HotKey(7);
            HotKey8 = new HotKey(8);
            HotKey9 = new HotKey(9);
            #endregion

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
            //MessageBar = MessageBarObject;
            WorldMap = WorldMapObject;
            QuestLog = QuestLogObject;
            DevConsole = DevConsoleObject;
            InGameUI_ = gameObject;

            HealtBar = HealtBarObject;
            StaminaBar = StaminaBarObject;
            HungerBar = HungerBarObject;
            ThirstBar = ThirstBarObject;

            HealtIndicador = HealtIndicadorObject;
            StaminaIndicador = StaminaIndicadorObject;
            HungerIndicador = HungerIndicadorObject;
            ThirstyIndicador = ThirstyIndicadorObject;
            WeightIndicador = WeightIndicadorObject;
            #endregion

            #region Set Indicators
            SetHealtBar(100);
            SetStaminatBar(100);
            SetHungerBar(100);
            SetThirstBar(100);
            SetWeightBar(0);
            #endregion


            Application.targetFrameRate = Main.targetFPS;

            float cameraHeight = Camera.GetComponent<Camera>().orthographicSize * 2f;
            float cameraWidth = cameraHeight * Camera.GetComponent<Camera>().aspect;

            Main.DefaultWidth = cameraWidth;
            Main.DefaultHeight = cameraHeight;

            // Objektum méretei (a kamera méreteihez igazítva)
            gameObject.GetComponent<RectTransform>().position = new Vector3(CameraObject.transform.position.x, CameraObject.transform.position.y, gameObject.GetComponent<RectTransform>().position.z) ;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(cameraWidth, cameraHeight);
        }
        void Update()
        {
            if (HUD.activeInHierarchy)
            {
                if (!isCustomCursorActive)
                {
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    isCustomCursorActive = true;
                }

                moveHorizontal = Input.GetAxis("Horizontal");
                moveVertical = Input.GetAxis("Vertical");
            }
            else
            {
                moveHorizontal = 0;
                moveVertical = 0;
                // Csak akkor állítsd vissza az alapértelmezett kurzort, ha az egyedi aktív
                if (isCustomCursorActive)
                {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    isCustomCursorActive = false;
                }
            }

            PlayerMovement(moveHorizontal, moveVertical);
        }
        private void FixedUpdate()//nem mindegyik frameben fut le
        {
            PlayerAnimation();

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
                    case KeyCode.W:
                        if (!HUD.activeInHierarchy)
                        {

                        }
                        break;
                    case KeyCode.S:
                        if (!HUD.activeInHierarchy)
                        {

                        }
                        break;
                    case KeyCode.A:
                        if (!HUD.activeInHierarchy)
                        {

                        }
                        break;
                    case KeyCode.D:
                        if (!HUD.activeInHierarchy)
                        {

                        }
                        break;
                    case KeyCode.F1:
                        if (Main.playerData.Admin != null && Main.playerData.Admin.DevConsole)
                        {
                            DevConsolOpenClose.Action();
                        }
                        break;
                    case KeyCode.Tab:
                        if (PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().EqipmentsPanel.activeInHierarchy)
                        {
                            PlayerInventoryOpenClose.Action();
                        }
                        else if (PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().HealthPanel.activeInHierarchy)
                        {
                            PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().OpenGearsPanel();
                        }
                        else
                        {
                            PlayerInventoryOpenClose.Action();
                            PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().OpenGearsPanel();
                        }
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
                    case KeyCode.H:
                        if (PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().EqipmentsPanel.activeInHierarchy)
                        {
                            PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().OpenHealtPanel();
                        }
                        else if (PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().HealthPanel.activeInHierarchy)
                        {
                            PlayerInventoryOpenClose.Action();
                        }
                        else
                        {
                            PlayerInventoryOpenClose.Action();
                            PlayerInventory.GetComponent<PlayerInventory>().EquipmentsPanelObject.GetComponent<PanelMain>().OpenHealtPanel();
                        }
                        break;
                    case KeyCode.Alpha1:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey1.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey1.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey1.Item);
                            }
                        }
                        break;
                    case KeyCode.Alpha2:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey2.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey2.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey2.Item);
                            }
                        }
                        break;
                    case KeyCode.Alpha3:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey3.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey3.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey3.Item);
                            }
                        }
                        break;
                    case KeyCode.Alpha4:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey4.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey4.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey4.Item);
                            }
                        }
                        break;
                    case KeyCode.Alpha5:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey5.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey5.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey5.Item);
                            }
                        }
                        if (SetHotKeyWithMouse && SetGameObjectToHotKey != null)//ha az objectumot ativaltuk de aztan megsemmistettuk akkor ezt nem allitjuk vissza false-ra
                        {
                            HotKey5.SetWithUI(SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData);
                        }
                        break;
                    case KeyCode.Alpha6:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey6.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey6.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey6.Item);
                            }
                        }
                        if (SetHotKeyWithMouse && SetGameObjectToHotKey != null)
                        {
                            HotKey6.SetWithUI(SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData);
                        }
                        break;
                    case KeyCode.Alpha7:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey7.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey7.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey7.Item);
                            }
                        }
                        if (SetHotKeyWithMouse && SetGameObjectToHotKey != null)
                        {
                            HotKey7.SetWithUI(SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData);
                        }
                        break;
                    case KeyCode.Alpha8:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey8.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey8.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey8.Item);
                            }
                        }
                        if (SetHotKeyWithMouse && SetGameObjectToHotKey != null)
                        {
                            HotKey8.SetWithUI(SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData);
                        }
                        break;
                    case KeyCode.Alpha9:
                        if (HUD.activeInHierarchy)
                        {
                            CharacterHand characterHand = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>();

                            if (characterHand.SelectedItem == HotKey9.Item)
                            {
                                characterHand.UnsetItem();
                            }
                            else if (HotKey9.Item != null)
                            {
                                characterHand.UnsetItem();
                                characterHand.SetItem(HotKey9.Item);
                            }
                        }
                        if (SetHotKeyWithMouse && SetGameObjectToHotKey != null)
                        {
                            HotKey9.SetWithUI(SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData);
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

        #region PlayerMovement And Animations
        public void SetWeightBar(float value)
        {
            WeightIndicador.GetComponent<TextMeshProUGUI>().text = $"{MaxWeight} / {value}";
        }
        public void SetHealtBar(float value)
        {
            HealtBar.GetComponent<Slider>().value = value;
            HealtIndicador.GetComponent<TextMeshProUGUI>().text = $"{value}";
        }
        public void SetStaminatBar(float value)
        {
            StaminaBar.GetComponent<Slider>().value = value;
            StaminaIndicador.GetComponent<TextMeshProUGUI>().text = $"{value}";
        }
        public void SetHungerBar(float value)
        {
            HungerBar.GetComponent<Slider>().value = value;
            HungerIndicador.GetComponent<TextMeshProUGUI>().text = $"{value}";
        }
        public void SetThirstBar(float value)
        {
            ThirstBar.GetComponent<Slider>().value = value;
            ThirstyIndicador.GetComponent<TextMeshProUGUI>().text = $"{value}";
        }
        public void HealtUp(float count)
        {
            if (count + Healt > MaxHealt)
            {
                count = MaxHealt - Healt;
            }
            Healt += count;
            SetHealtBar(count);
        }
        public void HealtDown(float count)
        {
            if (Healt - count < 0f)
            {
                count -= count - Healt;
            }
            Healt -= count;
            SetHealtBar(count);
        }

        public void StaminaUp(float count)
        {
            if (count + Stamina > MaxStamina)
            {
                count = MaxStamina - Stamina;
            }
            Stamina += count;
            SetStaminatBar(count);
        }
        public void StaminaDown(float count)
        {
            if (Stamina - count < 0f)
            {
                count -= count - Stamina;
            }
            Stamina -= count;
            SetStaminatBar(count);
        }

        public void HungerUp(float count)
        {
            if (count + Hunger > MaxHunger)
            {
                count = MaxHunger - Hunger;
            }
            Hunger += count;
            SetHungerBar(count);
        }
        public void HungerDown(float count)
        {
            if (Hunger - count < 0f)
            {
                count -= count - Hunger;
            }
            Hunger -= count;
            SetHungerBar(count);
        }

        public void ThirstUp(float count)
        {
            if (count + Thirst > MaxThirst)
            {
                count = MaxThirst - Thirst;
            }
            Thirst += count;
            SetThirstBar(count);
        }
        public void ThirstDown(float count)
        {
            if (Thirst - count < 0f)
            {
                count -= count - Thirst;
            }
            Thirst -= count;
            SetThirstBar(count);
        }
        private static void SetPlayerHand(AdvancedItem item)//ez inicializálja az uj selected itemet
        {
            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = item;
            HandHUDRefresh();
        }
        private static void HandHUDRefresh()//ez a selected item alapján frissiti a HUD-ot
        {
            AdvancedItem selectedItem = Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem;
            Player player = Player.GetComponent<Player>();
            GameObject playerObject = Player;
        }
        private void PlayerAnimation()
        {
            //A SpriteRenderer a Unityben egy komponens, amely lehetővé teszi a 2D grafikai elemek (spritek) megjelenítését a játékban. Ez a komponens felelős azért, hogy a spritet a megfelelő helyen és méretben jelenítse meg a képernyőn.
            //itt csak anyi a feladata, hogy ha a fegyver (ami a CharacterHand c# scriptet tartalmazza) flip ertekeit lekerje és amerre a fegyver nez arra nezzen a karaker.
            SpriteRenderer spriteRenderer = Player.GetComponent<SpriteRenderer>();

            //Transform child = Player.transform.GetChild(0);
            //SpriteRenderer spriteRendererChild = child.GetComponent<SpriteRenderer>();

            //if (spriteRendererChild.flipY)
            //{
            //    spriteRenderer.flipX = true;
            //}
            //else if (!spriteRendererChild.flipY)
            //{
            //    spriteRenderer.flipX = false;
            //}
        }
        private void PlayerMovement(float moveHorizontal, float moveVertical)
        {
            Rigidbody2D playerRB2D = Player.GetComponent<Rigidbody2D>();

            // Mozgás vektor
            //ugyebar matekbol tanultuk a vektorokat, na itt is az van:
            //itt jelenleg az tortenik, hogy a new vektor2 az ezt a c# scriptet tartalmazó objektumból kiindulva mutat egy másik pozícióra(ő a movement) a koordináta-rendszerben.
            //szoval lenyegeben ő a tervezett cél pozitcio ahova mozogni fog
            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            //az van, ha van egy negyzet akkor annak atloja hosszabb minha a negyzet jobb oldalan haladnank, ezert ha a karakter atlosan menne akkor lenyegeben gyorsabb lenne.
            //ezt azzal kerulom el, hogy normalizalom a vektort az az csupan erteket 1 re teszem, ezzel a vektor iranya nem valtozik, csak a hossza.
            if (movement.magnitude > 1)// a magnitute az a célirany és az objektum közötti tavolsag
            {
                movement = movement.normalized;
            }

            //a mozgas lenyegeben ugy mukodik, hogy az unity altal biztositva van egy fizikai komponenes, ez enyit tesz, hogy a mozgasat amit 1 fps az az frame per second alatt tenne meg
            //ő lebontja anyi utvonal darabkara, hogy azt a jelenlegi fps alatt tegye meg ezzel finom lezs a mozgasa
            //kb ugy mint egy video ha 3 fps ed van akkor szaggat mint a kurvaelet, ha meg van 200 akkor meg nagyob finoman mozog.
            //itt igy erteheto hogy az tortenik, hogy a movement az az az utvonal iranyat és annak iranyat felszorzom a sebeseggel tovabba hozza adom a fizikai komponenshez ami azt lebontja és vegre is hajtja.
            if (movement.magnitude > 0)
            {
                playerRB2D.velocity = movement * moveSpeed;
            }
            else// mivel a magnitude az az a célirány és az objektum közötti tavolsag maximum 1 lehet (az elobbiekbol adodoan), de csak akkor ha van bevitt WASD.
                //és az a helyzet, hogy meg igy is eszreveheto, hogy ha elengeded a WASD valamelyiket akkor a karakte rmeg egy kicsit megy, na ez azert van mert a fizikai komponens vegrehajtja a pozitcionalast,
                //de az utan is folytatja ha meg nem felyezte be, hogy te elengedted a WASD-t ezert ekkor meg kell allitani azzal, hogy 0 lesz és kesz.
            {
                // Azonnali megállás
                playerRB2D.velocity = Vector2.zero;
            }

            // Ellenőrizzük, hogy a játékos sprintel-e
            if (!isSprinting && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                moveSpeed = PlayerMovementSpeedMax;
                isSprinting = true;
            }
            else if (isSprinting && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                moveSpeed = PlayerMovementSpeed;
                isSprinting = false;
            }
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