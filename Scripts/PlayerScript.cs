using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamageable
{
   //public bool playerIsDead;

   //il punto di respawn
   //public Vector3 playerNewPos; 
    
   //public int maxHp;
   //[SerializeField] int currentHp;

    private Coroutine cor;
    //Attributi per il player
    [SerializeField] private Animator playerAn;
    private Vector3 playerStartPos;
    private MovimentoPlayer movPlayer;
    private Rigidbody2D rb;

    public int collezionabileCount;

    //public int Health
    //{
    //    get { return currentHp; }

    //    set
    //    {
    //        currentHp = value;

    //        if (currentHp <= 0)
    //        {
    //            PlayerDeath();
    //        }
    //        else if (currentHp > maxHp)
    //        {
    //            currentHp = maxHp;
    //        }
    //    }

    //}

    public void Damage(int value)
    {
        if (cor == null) cor = StartCoroutine(PlayerDeath());
    }

    //public void PlayerDeath()
    //{

    //}

    //public void RespawnThings()
    //{
    //    GameManager.instance.RiattivaElementi();
        //transform.position = playerNewPos;
    //}

    // Start is called before the first frame update
    void Start()
    {
        //playerIsDead = false;
        //transform.position = playerStartPos;
        playerStartPos = transform.position;
        movPlayer = GetComponent<MovimentoPlayer>();
        rb = GetComponent<Rigidbody2D>();

        collezionabileCount = 0;

    }

    private IEnumerator PlayerDeath()
    {
        GameManager.instance.audiomanager.PlaySound("morteplayer");
        Time.timeScale = 0;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        //timer di animazione
        float timer;
        //Disattivo movimento del player
        movPlayer.SetCanMove(false);
        //Attivo effetto morte
        playerAn.SetTrigger("Death");
        //Aspetto che finisce
        timer = playerAn.GetCurrentAnimatorClipInfo(0).Length;
        //Debug.Log(timer);
        yield return new WaitForSecondsRealtime(timer/2);
        //Controllo se c'è un checkpoint e se si respawno in quello corrente
        Transform checkPoint = GameManager.instance.GetCheckpoint();
        if (checkPoint) transform.position = checkPoint.position;
        //altrimenti respawno nel punto iniziale
        else transform.position = playerStartPos;
        //Riattivo movimento del player
        movPlayer.SetCanMove(true);
        //Respawno gli oggetti
        GameManager.instance.RiattivaElementi();
        //Attivo effetto riapparizione
        playerAn.SetTrigger("Respawn");
        //Aspetto che finisce
        yield return new WaitForSecondsRealtime(timer / 2);
        rb.isKinematic = false;
        Time.timeScale = 1;
        cor = null;
        yield return null;
    }

}
