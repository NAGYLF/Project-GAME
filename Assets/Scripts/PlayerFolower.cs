using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFolower : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;


    public Transform target;
    public Camera CameraObject;

    private Vector3 vel = Vector3.zero;

    private void Awake()
    {
        CameraObject.orthographicSize = gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;
    }
    private void FixedUpdate()
    {
        Vector3 targetPosition = target.position +offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position,targetPosition,ref vel,damping);
    }
}
