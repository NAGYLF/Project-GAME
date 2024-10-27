using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScaleTo : MonoBehaviour
{
    public GameObject[] scalebleObjects;



    public float ThisWidth;
    public float ThisHeight;
    private float ActualWidth;
    private float ActualHeight;
    private void Start()
    {
        ActualHeight = gameObject.GetComponent<RectTransform>().rect.width;
        ActualWidth = gameObject.GetComponent<RectTransform>().rect.height;
        float scaleFactorX = ActualHeight / ThisHeight;
        float scaleFactorY = ActualWidth / ThisWidth;

        foreach (GameObject item in scalebleObjects)
        {
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * scaleFactorX, rectTransform.sizeDelta.y * scaleFactorY);
            }
            if (item.TryGetComponent<TMP_InputField>(out TMP_InputField inputField))
            {
                inputField.textComponent.fontSize *= scaleFactorY; 
            }
        }
    }


}
