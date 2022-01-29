using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NemicoLupo : Enemies, IDamageable
{
    //Direzione iniziale che è uguale ad 1
    int dir = 1;
    //LayerMask del pavimento/muro
    [SerializeField]
    private LayerMask obstacleMask, playerMask;
    //Velocità iniziale che viene memorizzata per essere poi resettata
    private float startSpeed;
    //Collider per infliggere danni
    [SerializeField]
    private Transform dmgCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Proiettile"))
        {
            KillEnemy();
        }
    }

    private void Start()
    {
        //Memorizzo la velocità iniziale
        startSpeed = speed;
    }
    private void Update()
    {

        //Mi muovo a partire da destra
        rb.velocity = new Vector2(speed * dir,0);
        //Controllo se c'è il player di fronte
        if (Physics2D.Raycast(transform.position, Vector3.right * dir, 2, playerMask))
        {
            //Fermo il movimento è attacco
            if (speed != 0) speed = 0;
            //Imposto la direzione del DamageCollider
            //dmgCollider.transform.position = new Vector3(dmgCollider.transform.position.x * dir, dmgCollider.transform.position.y);
            //Attacco in loop se l'animazione di attacco è a false
            if (enemyAn.GetBool("Attacking") == false) enemyAn.SetBool("Attacking", true);
        }
        else
        {
            //Non sto attaccando quindi metto animazione di attacco a false, se è a true
            if (enemyAn.GetBool("Attacking")) enemyAn.SetBool("Attacking", false);
            //Se l'animazione di attacco è finita posso muovermi
            if (enemyAn.GetCurrentAnimatorStateInfo(0).IsName("LupoWalking")){
                //Resetto la velocità se è stata modificata dall'attacco
                if (speed != startSpeed) speed = startSpeed;

                //Se c'è il vuoto di fronte
                if (!Physics2D.Raycast(transform.position + Vector3.right * dir, Vector3.down, 1, obstacleMask) ||
                //O un muro cambio direzione
                Physics2D.Raycast(transform.position, Vector3.right * dir, 2, obstacleMask)) { 
                    dir *= -1;
                    transform.eulerAngles = dir == 1? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);
                }
            }
        }

        //Debug dei raycast
        Debug.DrawRay(transform.position + Vector3.right * dir, Vector3.down * 1, Color.red);
        Debug.DrawRay(transform.position, (Vector3.right * dir) * 2, Color.blue);
    }

    public override void KillEnemy()
    {
        enemyAn.SetTrigger("Death");
        speed = 0;
        rb.velocity = Vector3.zero;
        rb.angularDrag = 0;
        float timer = enemyAn.GetCurrentAnimatorClipInfo(0).Length;
        Invoke("DisableObject", timer);
    }

    private void DisableObject()
    {
        transform.gameObject.SetActive(false);
    }
}
