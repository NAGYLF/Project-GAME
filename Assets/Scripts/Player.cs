using PlayerInventoryVisualBuild;
using PlayerInventoryClass;
using MainData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Player : MonoBehaviour
{
    public static bool PlayerMovmentOnline = true;
    [HideInInspector] private float PlayerMovementSpeed = 50f;//default speed
    [HideInInspector] private float PlayerMovementSpeedMax = 100f;//sprint speed
    [SerializeField] private float moveSpeed;//actual speeed
    private bool isSprinting = false;

    private Rigidbody2D Objectrigidbody;//az objektum ami jelenleg a player, van egy unity altal biztositott fizika komponense ezzel lehet olyat csinalni, hogy nekimegy valaminek, mozog, stb
    private void Awake()
    {
        moveSpeed = PlayerMovementSpeed;
        Objectrigidbody = GetComponent<Rigidbody2D>();
        gameObject.AddComponent<PlayerInventory>();
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

        // Mozg�s vektor
        //ugyebar matekbol tanultuk a vektorokat, na itt is az van:
        //itt jelenleg az tortenik, hogy a new vektor2 az ezt a c# scriptet tartalmaz� objektumb�l kiindulva mutat egy m�sik poz�ci�ra(� a movement) a koordin�ta-rendszerben.
        //szoval lenyegeben � a tervezett c�l pozitcio ahova mozogni fog
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        
        //az van, ha van egy negyzet akkor annak atloja hosszabb minha a negyzet jobb oldalan haladnank, ezert ha a karakter atlosan menne akkor lenyegeben gyorsabb lenne.
        //ezt azzal kerulom el, hogy normalizalom a vektort az az csupan erteket 1 re teszem, ezzel a vektor iranya nem valtozik, csak a hossza.
        if (movement.magnitude>1)// a magnitute az a c�lirany �s az objektum k�z�tti tavolsag
        {
            movement = movement.normalized;
        }
        //a mozgas lenyegeben ugy mukodik, hogy az unity altal biztositva van egy fizikai komponenes, ez enyit tesz, hogy a mozgasat amit 1 fps az az frame per second alatt tenne meg
        //� lebontja anyi utvonal darabkara, hogy azt a jelenlegi fps alatt tegye meg ezzel finom lezs a mozgasa
        //kb ugy mint egy video ha 3 fps ed van akkor szaggat mint a kurvaelet, ha meg van 200 akkor meg nagyob finoman mozog.
        //itt igy erteheto hogy az tortenik, hogy a movement az az az utvonal iranyat �s annak iranyat felszorzom a sebeseggel tovabba hozza adom a fizikai komponenshez ami azt lebontja �s vegre is hajtja.
        if (movement.magnitude > 0)
        {
            Objectrigidbody.velocity = movement * moveSpeed;
        }
        else// mivel a magnitude az az a c�lir�ny �s az objektum k�z�tti tavolsag maximum 1 lehet (az elobbiekbol adodoan), de csak akkor ha van bevitt WASD.
            //�s az a helyzet, hogy meg igy is eszreveheto, hogy ha elengeded a WASD valamelyiket akkor a karakte rmeg egy kicsit megy, na ez azert van mert a fizikai komponens vegrehajtja a pozitcionalast,
            //de az utan is folytatja ha meg nem felyezte be, hogy te elengedted a WASD-t ezert ekkor meg kell allitani azzal, hogy 0 lesz �s kesz.
        {
            // Azonnali meg�ll�s
            Objectrigidbody.velocity = Vector2.zero;
        }


        // Ellen�rizz�k, hogy a j�t�kos sprintel-e
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
        //A SpriteRenderer a Unityben egy komponens, amely lehet�v� teszi a 2D grafikai elemek (spritek) megjelen�t�s�t a j�t�kban. Ez a komponens felel�s az�rt, hogy a spritet a megfelel� helyen �s m�retben jelen�tse meg a k�perny�n.
        //itt csak anyi a feladata, hogy ha a fegyver (ami a CharacterHand c# scriptet tartalmazza) flip ertekeit lekerje �s amerre a fegyver nez arra nezzen a karaker.
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
}