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
        // Mentjük el az eredeti méretet


        ActualHeight = gameObject.GetComponent<RectTransform>().rect.width;
        ActualWidth = gameObject.GetComponent<RectTransform>().rect.height;
    }

    private void Update()
    {
        // Aktuális méret


        // Arányosítás
        float scaleFactorX = ActualHeight / ThisHeight;
        float scaleFactorY = ActualWidth / ThisWidth;
        Debug.Log($" ActualHeight {ActualHeight}    ThisHeight{ThisHeight}   scaleFactorX {scaleFactorX}");
        ThisScale = scaleFactorX;

        // Frissítjük a skálát a listában lévõ objektumoknál
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
