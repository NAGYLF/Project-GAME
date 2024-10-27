using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTo : MonoBehaviour
{
    public GameObject[] scalebleObjects;



    public float ThisWidth;
    public float ThisHeight;
    public float ThisScale;
    private float ActualWidth;
    private float ActualHeight;
    private void Awake()
    {
        // Mentj�k el az eredeti m�retet


        ActualHeight = gameObject.GetComponent<RectTransform>().rect.width;
        ActualWidth = gameObject.GetComponent<RectTransform>().rect.height;
    }

    private void Update()
    {
        // Aktu�lis m�ret


        // Ar�nyos�t�s
        float scaleFactorX = ActualHeight / ThisHeight;
        float scaleFactorY = ActualWidth / ThisWidth;
        Debug.Log($" ActualHeight {ActualHeight}    ThisHeight{ThisHeight}   scaleFactorX {scaleFactorX}");
        ThisScale = scaleFactorX;

        // Friss�tj�k a sk�l�t a list�ban l�v� objektumokn�l
        foreach (GameObject item in scalebleObjects)
        {
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * scaleFactorX, rectTransform.sizeDelta.y * scaleFactorY);
            }
        }
    }

}
