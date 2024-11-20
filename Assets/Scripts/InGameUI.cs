using MainData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryVisualBuild;
using static MainData.SupportScripts;
using PlayerInventoryClass;
using System;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Transform target;//pl player
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;

    #region UI Metods
    OpenCloseUI DevConsol;
    OpenCloseUI PlayerInventory;
    #endregion
    private void Awake()
    {
        #region UI Metods Builds
        DevConsol = new OpenCloseUI(DevConsoleOpen,DevConsoleClose);
        PlayerInventory = new OpenCloseUI(PlayerInventoryOpen,PlayerInventoryClose);
        #endregion

        gameObject.AddComponent<PlayerInventory>();

        Application.targetFrameRate = Main.targetFPS;

        float cameraHeight = CameraObject.orthographicSize * 2f;
        float cameraWidth = cameraHeight * CameraObject.aspect;

        Main.DefaultWidth = cameraWidth;
        Main.DefaultHeight = cameraHeight;

        // Objektum méretei (a kamera méreteihez igazítva)
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(cameraWidth, cameraHeight);
        rectTransform.localPosition = Vector3.zero;
    }
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position +offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition,ref vel,damping);
    }
    #region DevConsole UI parts
    GameObject DevConsoleObject = null;
    private void DevConsoleOpen()
    {
        DevConsoleObject = CreatePrefab("GameElements/DevConsole");
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
                default:
                    break;
            }
        }
    }
}

class OpenCloseUI
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
    public OpenCloseUI(Action open, Action close)
    {
        this.open = open;
        this.close = close;
        this.Status = true;
        allInstances.Add(this);
    }
}
