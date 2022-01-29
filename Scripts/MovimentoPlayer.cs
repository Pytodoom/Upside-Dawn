using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoPlayer : MonoBehaviour
{
    //Riferimento rigidbody per muovere il personaggio
    private Rigidbody2D rb;
    //Velocità di movimento
    [SerializeField]
    private float speed;
    private float startSpeed;
    //Potenza di salto
    [SerializeField]
    private float JumpForce;
    //Input della direzione
    private float moveInput;

    //Booleana che controlla se tocco il pavimento
    [SerializeField]
    private bool isGrounded;
    //Posizione dei piedi
    [SerializeField]
    private Transform feetPos;
    //Radius per il check del pavimento
    [SerializeField]
    private float checkRadius;
    //Layer del ground
    [SerializeField]
    private LayerMask whatIsGound;

    [SerializeField]
    private float jumpTimeCounter;
    [SerializeField]
    private float jumpTime;
    private bool isJumping;

    //Riferimento all'animator
    [SerializeField]
    private Animator playerAn;


    //////
    public Transform wallChecker;
    public Vector2 wallCheckerSize;
    public bool isTouchingWall;

    public float wallJumpForce = 18f;
    public float airMoveSpeed;
    public float wallJumpDirection;
    public Vector2 wallJumpAngle;

    bool canMove=true;

    bool hasPressedSpace = false;

    int lastDir = 0;

    bool stopFriction = true;

    void ToccaMuro()
    {
        isTouchingWall = Physics2D.OverlapBox(wallChecker.position, wallCheckerSize, 0, whatIsGound);
    }

    [SerializeField] float jumpWallMultiplierX, jumpWallMultiplierY;

    Coroutine cor;

    void WallJump()
    {
        if(isTouchingWall && !isGrounded)
        {
            //if (stopFriction)
            //{
            //    rb.velocity = Vector2.zero;
            //    stopFriction = false;
            //}
            //if (lastDir != wallJumpDirection)
            //{
            //    rb.velocity = new Vector2(rb.velocity.x, -1);
            //}
            //Attivo animazione arrampicata
            playerAn.SetBool("Arrampicata", true);
            //rb.velocity = Vector2.zero;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (lastDir != wallJumpDirection)
                {
                    stopFriction = true;
                    lastDir = (int)wallJumpDirection;
                    //canMove = false;

                    //if (cor != null)
                    //{
                    //    StopCoroutine(cor);
                    //}

                    //cor = StartCoroutine(timer());

                    //moveInput = wallJumpDirection;

                    rb.velocity = new Vector2(moveInput * speed, jumpWallMultiplierY);

                    //rb.AddForce(new Vector2(wallJumpForce * wallJumpAngle.x * wallJumpDirection, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);

                    //rb.velocity += new Vector2(speed * -50, rb.velocity.y);

                    //Debug.Log("FORZA X " + wallJumpForce * wallJumpAngle.x * wallJumpDirection);

                }
            }
        } else
        {
            //Disattivo animazione arrampicata e così passa automaticamente a quella del salto
            if (playerAn.GetBool("Arrampicata")) playerAn.SetBool("Arrampicata", false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallChecker.position, wallCheckerSize);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //assegno il rigidbody a rb
        //Salvo la velocità iniziale
        startSpeed = speed;

        wallJumpAngle.Normalize();

    }

    IEnumerator timer()
    {
        GameManager.instance.audiomanager.PlaySound("walljump");
        //Debug.Log("coroutine");
        yield return new WaitForSeconds(0.5f);
        canMove = true;

        cor = null;

        yield return null;

    }

    void FixedUpdate()
    {
        ToccaMuro();

        //Se il player può moversi, memorizza il valore horizontal nel move input(-1 o 1)
        //if (GameManager.inst.canPlayerMove)
        //{
        if(canMove)
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

         /*
        if (isGrounded )
        {
             rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            
        }
        else if(!isGrounded && !isTouchingWall )
        {
            rb.AddForce(new Vector2(airMoveSpeed*moveInput,0));

            if (Mathf.Abs(rb.velocity.x) > speed)
            {
                   rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
            }
        }
         */
        //}
        //Altrimenti lo ferma
        //else StopMovement();
    }

    public void StopMovement()
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        speed = 0;
    }

    public void RestartMovement()
    {
        rb.isKinematic = false;
        speed = startSpeed;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal"); //se premo WASD mi muovo.

        WallJump();

        //Valori orizzontali e verticali di input
        float verticalMoveInput = Input.GetAxisRaw("Vertical");
        float horizontalMoveInput = Input.GetAxisRaw("Horizontal");
   
        //Check ground più ampio per passare all'animazione atterraggio dopo il salto
        //bool isGrounded; //emanuele tolta

        //RaycastHit2D rayGround;
        if (Physics2D.Raycast(transform.position, transform.TransformDirection(-Vector3.up), 2f, whatIsGound))
        {
            isGrounded = true;
            //Remposto il controllo per la direzione del salto tra le pareti
            lastDir = 0;
            if (hasPressedSpace)
            {
                if (playerAn.GetCurrentAnimatorStateInfo(0).IsName("JumpingRecoil")) { hasPressedSpace = false; }
            }
            //Debug.Log("isGrounded " + isGrounded);
        }
        else
        {
            isGrounded = false;

            //Debug.Log("isGrounded " + isGrounded);

        }
            //Controllo se sono in aria e in caso passo alla animazione di atterraggio se tocco il pavimento altrimenti rimango con l'animazione per aria
            //if (playerAn.GetCurrentAnimatorClipInfo(1)[0].clip.name == "Air")
            //    if (isGrounded) playerAn.SetBool("Recovery", true);
            //    else playerAn.SetBool("Recovery", false);
            //Booleana per il controllo di entrambi i tasti di salto
            bool jumpButton = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);

            //Se mi sto muovendo
            if (moveInput != 0)
            {
            //Se tocco il pavimento vado in corsa
                if (isGrounded) Run();
                //Metodo che si occupa dell'animazione di movimento
                if (moveInput > 0) //faccio flippare lo sprite.. se mi muovo verso destra gira verso destra e viceversa.
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    wallJumpDirection = -1;
                }
                else if (moveInput < 0)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    wallJumpDirection = 1;
                }
            }
            //Se invece sono fermo metto animazione di idle
            else if (isGrounded) Idle();

            //Salto prima parte
            if (isGrounded == true && jumpButton) //se premo J il player salta
            {
                GameManager.instance.audiomanager.PlaySound("jumpplayer");
                hasPressedSpace = true;
                playerAn.SetTrigger("Jump");
                isJumping = true; //se sono Ground e premo il salto. Allora attivo il salto e dò il valore di JumpTime a JumpTimeCounter
                jumpTimeCounter = jumpTime;
                rb.velocity = Vector2.up * JumpForce; //assegno a rb.velocity la direzione verso l'alto + la forza.

                
                //GameManager.inst.PlaySound("salto");
            }

        //Controllo se sono in aria e in caso passo alla animazione di atterraggio se tocco il pavimento altrimenti rimango con l'animazione per aria
        if (playerAn.GetCurrentAnimatorStateInfo(0).IsName("JumpingAir"))
        {
            if (isGrounded) playerAn.SetBool("Recoil", true);
            else playerAn.SetBool("Recoil", false);
        }

        //
        if (!isGrounded && !hasPressedSpace) 
        {
            playerAn.SetBool("Air", true);
        }
        else playerAn.SetBool("Air", false);
        //Booleana per il controllo di entrambi i tasti di salto continui (il tasto premuto)
        bool jumpButtonContinuos = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);

            if (jumpButtonContinuos && isJumping == true) //salta all'infinito
            {
                if (jumpTimeCounter > 0)
                {
                    rb.velocity = Vector2.up * JumpForce * 1.5f; //assegno a rb.velocity la direzione verso l'alto + la forza.
                    jumpTimeCounter -= Time.deltaTime; //diminuisco JumpTimeCounter
                }
                else
                {
                    isJumping = false; //se JumpTimeCounter è minore di 0 allora disattivo il salto
                }
            }

            //Booleana per il controllo di entrambi i tasti di salto, se qualcuno viene rilasciato diventa true
            bool jumpButtonUp = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space);

            if (jumpButtonUp)
            {
                isJumping = false; //disattivo il salto premendo J
            }
    
    }

    private void Idle()
    {
        playerAn.SetBool("Run", false);
    }

    private void Run()
    {
        playerAn.SetBool("Run", true);
    }

    //Metodo set per dire se il player può muoversi o no
    public void SetCanMove(bool temp)
    {
        canMove = temp;
    }
}
