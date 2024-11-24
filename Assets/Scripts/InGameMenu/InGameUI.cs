using MainData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryVisualBuild;
using static MainData.SupportScripts;
using PlayerInventoryClass;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    List<GameObject> InteractivObjects;

    #region UI objects
    public GameObject InGameMenuObject;
    public GameObject HealtBar;
    public GameObject StaminaBar;
    public GameObject HungerBar;
    public GameObject ThirstBar;
    GameObject DevConsoleObject = null;
    #endregion

    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Transform target;//pl player
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;

    #region UI Metods
    public static OpenCloseUI DevConsol;
    public static OpenCloseUI PlayerInventory;
    public static OpenCloseUI InGameMenu;
    #endregion
    private void Awake()
    {
        InteractivObjects = new List<GameObject>();

        OpenCloseUI.Refress();//mivel statikus a valtoto ezert ami statiku az az alkalmazás egész futása alatt létezik, ezert ha én törlöm ezt a jelenetet és ujra betoltom
                              //atol meg a regi gameobject refernciával bíró action tipusú változó eljarasai nem törlõdnek csak ujjak addolódnak hozzá. ezert töröljük õket

        #region UI Metods Builds
        DevConsol = new OpenCloseUI(DevConsoleOpen,DevConsoleClose);
        PlayerInventory = new OpenCloseUI(PlayerInventoryOpen,PlayerInventoryClose);
        InGameMenu = new OpenCloseUI(InGameMenuOpen,InGameMenuClose);
        #endregion

        gameObject.AddComponent<PlayerInventory>();

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
        Vector3 targetPosition = target.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition,ref vel,damping);
    }
    #region DevConsole UI parts
    private void DevConsoleOpen()
    {
        DevConsoleObject = CreatePrefab("GameElements/DevConsole");
        DevConsoleObject.GetComponent<DevConsol>().Player = target.gameObject;
        DevConsoleObject.GetComponent<DevConsol>().inventory = gameObject;
        DevConsoleObject.transform.SetParent(transform);
    }
    private void DevConsoleClose()
    {
        Destroy(DevConsoleObject);
    }
    #endregion

    #region PlayerInventory UI parts
    private void PlayerInventoryOpen()
    {
        gameObject.GetComponent<PlayerInventoryVisual>().OpenInventory();
    }
    private void PlayerInventoryClose()
    {
        gameObject.GetComponent<PlayerInventoryVisual>().CloseInventory();
    }
    #endregion

    #region InGameMenu parts
    private void InGameMenuOpen()
    {
        InGameMenuObject.SetActive(true);
    }
    private void InGameMenuClose()
    {
        InGameMenuObject.SetActive(false);
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
                default:
                    break;
            }
        }
    }

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
    public void RefreshInteractiveObjectList()
    {

    }
}

public class OpenCloseUI
{
    private static List<OpenCloseUI> allInstances = new List<OpenCloseUI>();

    private Action open;
    private Action close;
    private bool Status;
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
                instance.close.Invoke();
                instance.Status = true;
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
        this.Status = true;
        allInstances.Add(this);
    }
}
