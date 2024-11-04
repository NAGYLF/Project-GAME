using MainData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInventoryVisualBuild;
using static MainData.SupportScripts;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Vector2 InGameUISize = new Vector2(192,108);

    public Transform target;
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;


    private bool DevConsolOpen = false;
    private GameObject DevConsole;

    private void Awake()
    {
        Main.DefaultHeight = InGameUISize.y;
        Main.DefaultWidth = InGameUISize.x;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Main.DefaultWidth, Main.DefaultHeight);
        gameObject.GetComponent<RectTransform>().transform.localPosition = new Vector3(Main.DefaultWidth,Main.DefaultHeight,-8);
        CameraObject.orthographicSize = gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;//ha szukseges a camera manualis meretezese
    }
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position +offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition,ref vel,damping);
    }
    private void Update()
    {
        OpenCloseDevConsole();
    }
    private void OpenCloseDevConsole()
    {
        if (Input.GetKeyDown(KeyCode.F1) && DevConsolOpen)
        {
            DevConsolOpen = false;
            Destroy(DevConsole);
        }
        else if (Input.GetKeyDown(KeyCode.F1) && !DevConsolOpen)
        {
            DevConsolOpen = true;
            DevConsole = CreatePrefab("GameElements/DevConsole");
            DevConsole.transform.SetParent(transform);
            DevConsole.GetComponent<RectTransform>().transform.localPosition = new Vector3(Main.DefaultWidth, Main.DefaultHeight,0);

        }
    }
}
