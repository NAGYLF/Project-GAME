using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform Underline;
    public float targetWidth = 200f;
    public float duration = 2f;

    private float startWidth;
    private float timer;
    private bool expanding = false;

    void Start()
    {
        if (Underline != null)
        {
            startWidth = Underline.sizeDelta.x;
            Underline.sizeDelta = new Vector2(0, Underline.sizeDelta.y);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        expanding = true;
        timer = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        expanding = false;
        timer = 0;
    }

    void Update()
    {
        if (Underline == null) return;

        timer += Time.deltaTime / duration;
        float width = expanding ? Mathf.Lerp(startWidth, targetWidth, timer) : Mathf.Lerp(targetWidth, startWidth, timer);
        Underline.sizeDelta = new Vector2(width, Underline.sizeDelta.y);
    }
}
