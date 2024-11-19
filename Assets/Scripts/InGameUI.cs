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

    public Transform target;//pl player
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;

    private bool DevConsolOpen = false;
    private GameObject DevConsole;

    private void Awake()
    {
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
        }
    }
}
