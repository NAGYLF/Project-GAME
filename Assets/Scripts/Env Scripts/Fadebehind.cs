using UnityEngine;

public class Fadebehind : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 11;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }
}