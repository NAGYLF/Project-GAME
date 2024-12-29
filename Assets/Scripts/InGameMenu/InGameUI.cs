using MainData;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryClass;
using System;
using UnityEngine.UI;
using ItemHandler;
using TMPro;
using Newtonsoft.Json.Linq;

namespace UI
{
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
        private void Awake()
        {
            #region Player Part Inicialisation
            moveSpeed = PlayerMovementSpeed;
            #endregion

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
            if (HUD.activeSelf)
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
                        if (!HUD.activeSelf)
                        {

                        }
                        break;
                    case KeyCode.S:
                        if (!HUD.activeSelf)
                        {

                        }
                        break;
                    case KeyCode.A:
                        if (!HUD.activeSelf)
                        {

                        }
                        break;
                    case KeyCode.D:
                        if (!HUD.activeSelf)
                        {

                        }
                        break;
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
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey1;
                        }
                        break;
                    case KeyCode.Alpha2:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey2;
                        }
                        break;
                    case KeyCode.Alpha3:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey3;
                        }
                        break;
                    case KeyCode.Alpha4:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey4;
                        }
                        break;
                    case KeyCode.Alpha5:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey5;
                        }
                        if (SetHotKeyWithMouse)
                        {
                            SetHotKey(5);
                        }
                        break;
                    case KeyCode.Alpha6:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey6;
                        }
                        if (SetHotKeyWithMouse)
                        {
                            SetHotKey(6);
                        }
                        break;
                    case KeyCode.Alpha7:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey7;
                        }
                        if (SetHotKeyWithMouse)
                        {
                            SetHotKey(7);
                        }
                        break;
                    case KeyCode.Alpha8:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey8;
                        }
                        if (SetHotKeyWithMouse)
                        {
                            SetHotKey(8);
                        }
                        break;
                    case KeyCode.Alpha9:
                        if (HUD.activeSelf)
                        {
                            Player.GetComponent<Player>().Hand.GetComponent<CharacterHand>().SelectedItem = HotKey9;
                        }
                        if (SetHotKeyWithMouse)
                        {
                            SetHotKey(9);
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

        #region HotKeySet
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
        public static void SetHotKey(int SetToHotKeyNumber)
        {
            Item ItemData = SetGameObjectToHotKey.GetComponent<ItemObject>().ActualData;
            if (!ItemData.IsEquipment && ItemData.IsInPlayerInventory)
            {
                if (ItemData.HotKey == SetToHotKeyNumber.ToString())
                {
                    InventorySystem.UnSetHotKey(ItemData);
                    SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                }
                else
                {
                    InventorySystem.UnSetHotKey(ItemData);
                    switch (SetToHotKeyNumber)
                    {
                        case 5:
                            if (HotKey5 != null)
                            {
                                HotKey5.HotKey = "";
                                HotKey5.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                            SetHotKey5(ItemData);
                            ItemData.HotKey = SetToHotKeyNumber.ToString();
                            SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                            break;
                        case 6:
                            if (HotKey6 != null)
                            {
                                HotKey6.HotKey = "";
                                HotKey6.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                            SetHotKey6(ItemData);
                            ItemData.HotKey = SetToHotKeyNumber.ToString();
                            SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                            break;
                        case 7:
                            if (HotKey7 != null)
                            {
                                HotKey7.HotKey = "";
                                HotKey7.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                            SetHotKey7(ItemData);
                            ItemData.HotKey = SetToHotKeyNumber.ToString();
                            SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                            break;
                        case 8:
                            if (HotKey8 != null)
                            {
                                HotKey8.HotKey = "";
                                HotKey8.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                            SetHotKey8(ItemData);
                            ItemData.HotKey = SetToHotKeyNumber.ToString();
                            SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                            break;
                        case 9:
                            if (HotKey9 != null)
                            {
                                HotKey9.HotKey = "";
                                HotKey9.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                            SetHotKey9(ItemData);
                            ItemData.HotKey = SetToHotKeyNumber.ToString();
                            SetGameObjectToHotKey.GetComponent<ItemObject>().SelfVisualisation();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region PlayerMovement And Animations
        public void SetWeightBar(float value)
        {
            WeightIndicador.GetComponent<TextMeshPro>().text = $"{MaxWeight} / {value}";
        }
        public void SetHealtBar(float value)
        {
            HealtBar.GetComponent<Slider>().value = value;
            HealtIndicador.GetComponent<TextMeshPro>().text = $"{value}";
        }
        public void SetStaminatBar(float value)
        {
            StaminaBar.GetComponent<Slider>().value = value;
            StaminaIndicador.GetComponent<TextMeshPro>().text = $"{value}";
        }
        public void SetHungerBar(float value)
        {
            HungerBar.GetComponent<Slider>().value = value;
            HungerIndicador.GetComponent<TextMeshPro>().text = $"{value}";
        }
        public void SetThirstBar(float value)
        {
            ThirstBar.GetComponent<Slider>().value = value;
            ThirstyIndicador.GetComponent<TextMeshPro>().text = $"{value}";
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
        private void PlayerAnimation()
        {
            //A SpriteRenderer a Unityben egy komponens, amely lehetővé teszi a 2D grafikai elemek (spritek) megjelenítését a játékban. Ez a komponens felelős azért, hogy a spritet a megfelelő helyen és méretben jelenítse meg a képernyőn.
            //itt csak anyi a feladata, hogy ha a fegyver (ami a CharacterHand c# scriptet tartalmazza) flip ertekeit lekerje és amerre a fegyver nez arra nezzen a karaker.
            SpriteRenderer spriteRenderer = Player.GetComponent<SpriteRenderer>();

            Transform child = Player.transform.GetChild(0);
            SpriteRenderer spriteRendererChild = child.GetComponent<SpriteRenderer>();

            if (spriteRendererChild.flipY)
            {
                spriteRenderer.flipX = true;
            }
            else if (!spriteRendererChild.flipY)
            {
                spriteRenderer.flipX = false;
            }
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