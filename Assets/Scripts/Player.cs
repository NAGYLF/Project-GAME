using UnityEngine;
using UI;
using ItemHandler;
public class Player : MonoBehaviour
{
    [SerializeField] public GameObject Hand;

    private const float MaxHealt = 100f;
    private float Healt = 100f;

    private const float MaxStamina = 100f;
    private float Stamina = 100f;

    private const float MaxHunger = 100f;
    private float Hunger = 100f;

    private const float MaxThirst = 100f;
    private float Thirst = 100f;

    public void HealtUp(float count)
    {
        if (count+Healt>MaxHealt)
        {
            count = MaxHealt - Healt;
        }
        Healt += count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetHealtBar(Healt);
    }
    public void HealtDown(float count)
    {
        if (Healt-count < 0f)
        {
            count -= count - Healt;
        }
        Healt -= count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetHealtBar(Healt);
    }

    public void StaminaUp(float count)
    {
        if (count + Stamina > MaxStamina)
        {
            count = MaxStamina - Stamina;
        }
        Stamina += count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetStaminatBar(Stamina);
    }
    public void StaminaDown(float count)
    {
        if (Stamina - count < 0f)
        {
            count -= count - Stamina;
        }
        Stamina -= count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetStaminatBar(Stamina);
    }

    public void HungerUp(float count)
    {
        if (count + Hunger > MaxHunger)
        {
            count = MaxHunger - Hunger;
        }
        Hunger += count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetHungerBar(Hunger);
    }
    public void HungerDown(float count)
    {
        if (Hunger - count < 0f)
        {
            count -= count - Hunger;
        }
        Hunger -= count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetHungerBar(Hunger);
    }

    public void ThirstUp(float count)
    {
        if (count + Thirst > MaxThirst)
        {
            count = MaxThirst - Thirst;
        }
        Thirst += count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetThirstBar(Thirst);
    }
    public void ThirstDown(float count)
    {
        if (Thirst - count < 0f)
        {
            count -= count - Thirst;
        }
        Thirst -= count;
        InGameUI.InGameUI_.GetComponent<InGameUI>().SetThirstBar(Thirst);
    }
}