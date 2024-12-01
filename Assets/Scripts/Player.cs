using PlayerInventoryClass;
using MainData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UI;
public class Player : MonoBehaviour
{
    public GameObject InGameUI;

    public bool PlayerMovmentOnline = true;
    [HideInInspector] private float PlayerMovementSpeed = 50f;//default speed
    [HideInInspector] private float PlayerMovementSpeedMax = 100f;//sprint speed
    [SerializeField] private float moveSpeed;//actual speeed
    private bool isSprinting = false;

    private const float MaxHealt = 100f;
    private float Healt = 100f;

    private const float MaxStamina = 100f;
    private float Stamina = 100f;

    private const float MaxHunger = 100f;
    private float Hunger = 100f;

    private const float MaxThirst = 100f;
    private float Thirst = 100f;

    private Rigidbody2D Objectrigidbody;//az objektum ami jelenleg a player, van egy unity altal biztositott fizika komponense ezzel lehet olyat csinalni, hogy nekimegy valaminek, mozog, stb
    private void Awake()
    {
        moveSpeed = PlayerMovementSpeed;
        Objectrigidbody = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (PlayerMovmentOnline)
        {
            PlayerAnimation(Input.GetAxis("Horizontal"));
        }
    }
    private void FixedUpdate()
    {
        if (PlayerMovmentOnline)
        {
            Movement();//mozgas
        }
    }
    private void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");//a mozgasi inputok horizontalisan ertekuk 1 tol -1 ig
        float moveVertical = Input.GetAxis("Vertical");//a mozgasi inputok vertikalisan ertekuk 1 tol -1 ig

        // Mozgás vektor
        //ugyebar matekbol tanultuk a vektorokat, na itt is az van:
        //itt jelenleg az tortenik, hogy a new vektor2 az ezt a c# scriptet tartalmazó objektumból kiindulva mutat egy másik pozícióra(ő a movement) a koordináta-rendszerben.
        //szoval lenyegeben ő a tervezett cél pozitcio ahova mozogni fog
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        
        //az van, ha van egy negyzet akkor annak atloja hosszabb minha a negyzet jobb oldalan haladnank, ezert ha a karakter atlosan menne akkor lenyegeben gyorsabb lenne.
        //ezt azzal kerulom el, hogy normalizalom a vektort az az csupan erteket 1 re teszem, ezzel a vektor iranya nem valtozik, csak a hossza.
        if (movement.magnitude>1)// a magnitute az a célirany és az objektum közötti tavolsag
        {
            movement = movement.normalized;
        }
        //a mozgas lenyegeben ugy mukodik, hogy az unity altal biztositva van egy fizikai komponenes, ez enyit tesz, hogy a mozgasat amit 1 fps az az frame per second alatt tenne meg
        //ő lebontja anyi utvonal darabkara, hogy azt a jelenlegi fps alatt tegye meg ezzel finom lezs a mozgasa
        //kb ugy mint egy video ha 3 fps ed van akkor szaggat mint a kurvaelet, ha meg van 200 akkor meg nagyob finoman mozog.
        //itt igy erteheto hogy az tortenik, hogy a movement az az az utvonal iranyat és annak iranyat felszorzom a sebeseggel tovabba hozza adom a fizikai komponenshez ami azt lebontja és vegre is hajtja.
        if (movement.magnitude > 0)
        {
            Objectrigidbody.velocity = movement * moveSpeed;
        }
        else// mivel a magnitude az az a célirány és az objektum közötti tavolsag maximum 1 lehet (az elobbiekbol adodoan), de csak akkor ha van bevitt WASD.
            //és az a helyzet, hogy meg igy is eszreveheto, hogy ha elengeded a WASD valamelyiket akkor a karakte rmeg egy kicsit megy, na ez azert van mert a fizikai komponens vegrehajtja a pozitcionalast,
            //de az utan is folytatja ha meg nem felyezte be, hogy te elengedted a WASD-t ezert ekkor meg kell allitani azzal, hogy 0 lesz és kesz.
        {
            // Azonnali megállás
            Objectrigidbody.velocity = Vector2.zero;
        }


        // Ellenőrizzük, hogy a játékos sprintel-e
        if (!isSprinting && Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = PlayerMovementSpeedMax;
            isSprinting = true;
        }
        else if (isSprinting && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            moveSpeed = PlayerMovementSpeed;
            isSprinting = false;
        }
    }

    private void PlayerAnimation(float moveHorizontal)
    {
        //A SpriteRenderer a Unityben egy komponens, amely lehetővé teszi a 2D grafikai elemek (spritek) megjelenítését a játékban. Ez a komponens felelős azért, hogy a spritet a megfelelő helyen és méretben jelenítse meg a képernyőn.
        //itt csak anyi a feladata, hogy ha a fegyver (ami a CharacterHand c# scriptet tartalmazza) flip ertekeit lekerje és amerre a fegyver nez arra nezzen a karaker.
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Transform child = transform.GetChild(0);
        SpriteRenderer spriteRendererChild = child.GetComponent<SpriteRenderer>();

        if (spriteRendererChild.flipY)
        {
            spriteRenderer.flipX = true;
        }
        else if (!spriteRendererChild.flipY)
        {
            spriteRenderer.flipX = false;
        }
    }

    public void HealtUp(float count)
    {
        if (count+Healt>MaxHealt)
        {
            count = MaxHealt - Healt;
        }
        Healt += count;
        InGameUI.GetComponent<InGameUI>().SetHealtBar(Healt);
    }
    public void HealtDown(float count)
    {
        if (Healt-count < 0f)
        {
            count -= count - Healt;
        }
        Healt -= count;
        InGameUI.GetComponent<InGameUI>().SetHealtBar(Healt);
    }

    public void StaminaUp(float count)
    {
        if (count + Stamina > MaxStamina)
        {
            count = MaxStamina - Stamina;
        }
        Stamina += count;
        InGameUI.GetComponent<InGameUI>().SetStaminatBar(Stamina);
    }
    public void StaminaDown(float count)
    {
        if (Stamina - count < 0f)
        {
            count -= count - Stamina;
        }
        Stamina -= count;
        InGameUI.GetComponent<InGameUI>().SetStaminatBar(Stamina);
    }

    public void HungerUp(float count)
    {
        if (count + Hunger > MaxHunger)
        {
            count = MaxHunger - Hunger;
        }
        Hunger += count;
        InGameUI.GetComponent<InGameUI>().SetHungerBar(Hunger);
    }
    public void HungerDown(float count)
    {
        if (Hunger - count < 0f)
        {
            count -= count - Hunger;
        }
        Hunger -= count;
        InGameUI.GetComponent<InGameUI>().SetHungerBar(Hunger);
    }

    public void ThirstUp(float count)
    {
        if (count + Thirst > MaxThirst)
        {
            count = MaxThirst - Thirst;
        }
        Thirst += count;
        InGameUI.GetComponent<InGameUI>().SetThirstBar(Thirst);
    }
    public void ThirstDown(float count)
    {
        if (Thirst - count < 0f)
        {
            count -= count - Thirst;
        }
        Thirst -= count;
        InGameUI.GetComponent<InGameUI>().SetThirstBar(Thirst);
    }
}