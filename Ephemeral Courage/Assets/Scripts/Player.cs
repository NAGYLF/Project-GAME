using MainData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] private float PlayerMovementSpeed = 5f;//default speed
    [SerializeField] private float PlayerMovementSpeedMax = 8f;//sprint speed
    [SerializeField] private float moveSpeed;//actual speeed
    private bool isSprinting = false;

    private Rigidbody2D rigidbody;//az objektum ami jelenleg a player, van egy unity altal biztositott fizika komponense ezzel lehet olyat csinalni, hogy nekimegy valaminek, mozog, stb
    private void Awake()
    {
        moveSpeed = PlayerMovementSpeed;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerAnimation(Input.GetAxis("Horizontal"));
    }
    private void FixedUpdate()
    {
        Movement();//mozgas
        InventoryOpen();//Not working
    }
    private void InventoryOpen()//Not working
    {
        if (Input.GetKey(KeyCode.Tab))
        {

        }
    }
    private void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");//a mozgasi inputok horizontalisan ertekuk 1 tol -1 ig
        float moveVertical = Input.GetAxis("Vertical");//a mozgasi inputok vertikalisan ertekuk 1 tol -1 ig

        // Mozgás vektor
        //ugyebar matekbol tanultuk a vektorokat, na itt is az van:
        //itt jelenleg az tortenik, hogy a new vektor2 az ezt a c# scriptet tartalmazó objektumból kiindulva mutat egy másik pozícióra(õ a movement) a koordináta-rendszerben.
        //szoval lenyegeben õ a tervezett cél pozitcio ahova mozogni fog
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        
        //az van, ha van egy negyzet akkor annak atloja hosszabb minha a negyzet jobb oldalan haladnank, ezert ha a karakter atlosan menne akkor lenyegeben gyorsabb lenne.
        //ezt azzal kerulom el, hogy normalizalom a vektort az az csupan erteket 1 re teszem, ezzel a vektor iranya nem valtozik, csak a hossza.
        if (movement.magnitude>1)// a magnitute az a célirany és az objektum közötti tavolsag
        {
            movement = movement.normalized;
        }
        //a mozgas lenyegeben ugy mukodik, hogy az unity altal biztositva van egy fizikai komponenes, ez enyit tesz, hogy a mozgasat amit 1 fps az az frame per second alatt tenne meg
        //õ lebontja anyi utvonal darabkara, hogy azt a jelenlegi fps alatt tegye meg ezzel finom lezs a mozgasa
        //kb ugy mint egy video ha 3 fps ed van akkor szaggat mint a kurvaelet, ha meg van 200 akkor meg nagyob finoman mozog.
        //itt igy erteheto hogy az tortenik, hogy a movement az az az utvonal iranyat és annak iranyat felszorzom a sebeseggel tovabba hozza adom a fizikai komponenshez ami azt lebontja és vegre is hajtja.
        if (movement.magnitude > 0)
        {
            rigidbody.velocity = movement * moveSpeed;
        }
        else// mivel a magnitude az az a célirány és az objektum közötti tavolsag maximum 1 lehet (az elobbiekbol adodoan), de csak akkor ha van bevitt WASD.
            //és az a helyzet, hogy meg igy is eszreveheto, hogy ha elengeded a WASD valamelyiket akkor a karakte rmeg egy kicsit megy, na ez azert van mert a fizikai komponens vegrehajtja a pozitcionalast,
            //de az utan is folytatja ha meg nem felyezte be, hogy te elengedted a WASD-t ezert ekkor meg kell allitani azzal, hogy 0 lesz és kesz.
        {
            // Azonnali megállás
            rigidbody.velocity = Vector2.zero;
        }


        // Ellenõrizzük, hogy a játékos sprintel-e
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
        //A SpriteRenderer a Unityben egy komponens, amely lehetõvé teszi a 2D grafikai elemek (spritek) megjelenítését a játékban. Ez a komponens felelõs azért, hogy a spritet a megfelelõ helyen és méretben jelenítse meg a képernyõn.
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
}