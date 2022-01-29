using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    //T per cambiare il tempo
    //M per morire

    private Transform player;
    private PlayerScript playerHealth;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerScript>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) playerHealth.Damage(1);
    }
}
