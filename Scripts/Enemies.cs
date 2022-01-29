using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour, IDamageable
{
    public int maxHp;
    //[HideInInspector]
    public int currentHp;

    //public bool isDead;
    [HideInInspector]
    public Vector3 startPosition;

    public ParticleSystem psCambioSprite; //se cambiamo nel passaggio giorno notte
    public ParticleSystem psMorte;

    public float speed;

    public Rigidbody2D rb;

    public Animator enemyAn;

    // Start is called before the first frame update
    void Awake()
    {
        //isDead = false;
        startPosition = transform.position;
        //transform.position = startPosition;
    }

    public virtual void OnEnable()
    {
        //isDead = false;

        //transform.position = startPosition;
    }

    public virtual int Health 
    {
        get { return currentHp; }

        set
        {
            currentHp = value; 

            if (currentHp <= 0) 
            {
                //isDead = true;
                KillEnemy();
            }
            else if (currentHp > maxHp)
            {
                currentHp = maxHp;
            }
        }

    }

    public virtual void Damage(int amount)  
    {
        Health -= amount;
        Debug.Log("Prova");
    }

    public virtual void KillEnemy()
    {
        //GameManager.instance.oggettidaDisattivare.Add(this.gameObject);
        // Destroy(gameObject);
       

        if (psMorte != null)
        {
            ParticleSystem ps = Instantiate(psMorte, transform.position, Quaternion.identity);
            ps.transform.SetParent(null);
            ps.Play();
            Destroy(ps, 1.5f);

        }

        gameObject.SetActive(false);

    }


}
