using MainData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;


    public Transform target;
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;


    private bool DevConsolOpen = false;
    private GameObject DevConsole;

    private void Awake()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Main.DefaultWidth, Main.DefaultHeight);
        CameraObject.orthographicSize = gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;
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
            DevConsole.GetComponent<RectTransform>().sizeDelta = new Vector2(Main.DefaultWidth,Main.DefaultHeight);

        }
    }

    private GameObject CreatePrefab(string path)
    {
        GameObject prefab = Instantiate(Resources.Load<GameObject>(path));
        if (prefab != null)
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"{path} prefab nem található!");
            return null;
        }
    }

}
