using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            IDamageable temp = collision.GetComponent<IDamageable>();
            if (temp != null) temp.Damage(1);
        }
    }
}
