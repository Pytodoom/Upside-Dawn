using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Camera principale
    [SerializeField]
    private GameObject vCam;
    [SerializeField]
    private Transform checkPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Se il player entra nell'area attivo la telecamera corrispondente
        if (other.CompareTag("Player"))
        {
            vCam.SetActive(true);
            //Assegnoil check point corrente
            GameManager.instance.SetCheckpoint(checkPoint);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Se il player esce nell'area disattivo la telecamera corrispondente
        if (other.CompareTag("Player"))
        {
            vCam.SetActive(false);            
        }
    }

}