using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField]
    private bool damagePlayer = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Se non deve fare danno al player fa danno a qualunque cosa
        if (!damagePlayer)
        {
            IDamageable temp = collision.GetComponent<IDamageable>();
            if (temp != null) temp.Damage(1);
        }
        //Altrimenti
        else
        {
            //Fa danno solo al player
            if (collision.CompareTag("Player"))
            {
                IDamageable temp = collision.GetComponent<IDamageable>();
                if (temp != null) temp.Damage(1);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Se non deve fare danno al player fa danno a qualunque cosa
        if (!damagePlayer)
        {
            IDamageable temp = collision.transform.GetComponent<IDamageable>();
            if (temp != null) temp.Damage(1);
        }
        //Altrimenti
        else
        {
            //Fa danno solo al player
            if (collision.transform.CompareTag("Player"))
            {
                IDamageable temp = collision.transform.GetComponent<IDamageable>();
                if (temp != null) temp.Damage(1);
            }
        }
    }
}
