using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<GameObject> oggettidaDisattivare;

    public TextMeshProUGUI collezionabileUICount;
    [SerializeField] GameObject panelPause;
    [SerializeField] GameObject panelOptions;
    bool isPause;

    [SerializeField]
    private MovimentoPlayer playerMovement;
    public PlayerScript playerScript;

    //Stanza corrente per confrontarle con le altre e disattivarle
    private GameObject currentRoom;

    //Matrice contenente gli oggetti trasformabili durante il cambio meteo
    private Dictionary<GameObject, GameObject> dictChangeObj = new Dictionary<GameObject, GameObject>();

    //Padre di ogni singolo oggetto cambiabile con il potere che ha come figli l'oggetto di giorno e quello di notte
    [SerializeField]
    private Transform[] parChangeObj;

    //Coroutine corrente
    private Coroutine currentCor = null, secondCor = null;

    public AudioMixerGroup musica;
    public AudioMixerGroup sfx;

    public Slider sliderMusica;
    public Slider sliderSfx;

    //Enum per il giorno notte
    public enum StatoGiornata
    {
        Giorno,
        Notte
    }

    //Cielo da ruotare
    [SerializeField]
    private Transform sunMoon;

    //Istanza di enum che parte da giorno
    public StatoGiornata statoGiornata;

    //Checkpoint corrente
    private Transform currentCheckpoint;

    public AudioManager audiomanager;


    //Robe per il cambio colori nella transizione giorno notte
    [SerializeField] SpriteRenderer sfondo;

    [SerializeField] Color sfondoDark, luceDark;

    [SerializeField] UnityEngine.Rendering.Universal.Light2D globalLight;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

    }


    public void SetVolumeMusica(float sliderValue)
    {
        musica.audioMixer.SetFloat("MusicheVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Musica", sliderMusica.value);
    }

    public void SetVolumeSFX(float sliderValue)
    {
        sfx.audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Sfx", sliderSfx.value);

    }

    public void RiattivaElementi() //quando il player muore riattiviamo tutti gli ogetti disattivati (vengono disattivati nello script enemies)
    {
        foreach (GameObject go in oggettidaDisattivare)
        {
            if (go != null)
            {

                go.SetActive(true);
            }
        }

        oggettidaDisattivare.Clear();

    }

    public void Pausa()
    {
        if (panelPause != null)

        {
            panelPause.SetActive(true);
            Time.timeScale = 0f;
            isPause = true;
        }

    }

    public void Resume()
    {
        if (panelPause != null)
        {
            panelPause.SetActive(false);

            panelOptions.SetActive(false);
            

            Time.timeScale = 1f;
            isPause = false;
        }

    }

    public void OpenOptions()
    {
        panelOptions.SetActive(true);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        sliderMusica.value = PlayerPrefs.GetFloat("Musica",.8f);
        sliderSfx.value =  PlayerPrefs.GetFloat("Sfx", .8f); ;

        isPause = false;
        panelPause.SetActive(false);
        panelOptions.SetActive(false);

        //Imposto la giornata a giorno
        statoGiornata = StatoGiornata.Giorno;

        //Riempio nel dizionario i GameObject che verranno modificati con il potere del tempo dal array
        for(int i=0; i<parChangeObj.Length; i++)
        {
            dictChangeObj.Add(parChangeObj[i].GetChild(0).gameObject, parChangeObj[i].GetChild(1).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isPause)
            {
                Resume();
                Debug.Log("no pausa");
            }
            else
            {
                Pausa();
                Debug.Log("pausa");
            }

        }

        //Se premo il tasto per cambiare il tempo chiamo il metodo che si occupa di questo
        if (Input.GetKeyDown(KeyCode.LeftShift)) ChangeWorld();
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
        //Fermo pure il movimento del player
        playerMovement.StopMovement();
    }

    public void RestartTime()
    {
        Time.timeScale = 1f;
        //Re imposto al valore normale pure il movimento del player
        playerMovement.RestartMovement();
    }

    //Metodi set e get per la stanza corrente
    public void SetCurrentRoom(GameObject room)
    {
        currentRoom = room;
    }

    public GameObject GetCurrentRoom()
    {
        return currentRoom;
    }

    public GameObject GetPlayer()
    {
        return playerMovement.gameObject;
    }


    //Metodi set e get del checkpoint corrente
    public void SetCheckpoint(Transform trans) { currentCheckpoint = trans; }

    public Transform GetCheckpoint() { return currentCheckpoint; }
    private void ChangeWorld()
    {
        audiomanager.PlaySound("giornonotte");

        if (currentCor == null)
        {
            //Inverto la giornata
            statoGiornata = (statoGiornata == StatoGiornata.Giorno ? StatoGiornata.Notte : StatoGiornata.Giorno);
            //e la applico attivando i GameObject dello stato della giornata dal dizionario
            foreach (KeyValuePair<GameObject, GameObject> temp in dictChangeObj)
            {
                //Se diventa giorno attivo i GameObject del giorno
                if (statoGiornata == StatoGiornata.Giorno) { temp.Key.SetActive(true); temp.Value.SetActive(false); }
                //altrimenti se diventa notte attivo i GameObject della notte
                else { temp.Key.SetActive(false); temp.Value.SetActive(true); }
            }
            //Attivo coroutine che ruota il cielo per far apparire o la luna o il sole
            currentCor = StartCoroutine(ChangeSunMoon());
            //StopCoroutine("ChangeBgColor");
            if (secondCor!=null) StopCoroutine(secondCor);
            secondCor = StartCoroutine(ChangeBgColor());
        }

    }

    private bool myApproximation(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

    private IEnumerator ChangeSunMoon()
    {
       

        //Ruoto lentamente verso la destinazione
        float targetRot = sunMoon.eulerAngles.z + 180;
        if (targetRot >= 360) targetRot = 0;
        while(sunMoon.eulerAngles.z != targetRot)
        {
            sunMoon.Rotate(Vector3.forward, 150 * Time.deltaTime);
            if (myApproximation(sunMoon.eulerAngles.z, targetRot, 6f))
            {
                sunMoon.rotation = Quaternion.Euler(new Vector3(0,0,targetRot));
            }
            yield return null;
        }
        currentCor = null;
        yield return null;
    }

    private IEnumerator ChangeBgColor()
    {
        //Cambio colore sfondo e colore luce
        float elapsedTimer = 0, totalTimer = 2;
        while (elapsedTimer < totalTimer)
        {
            Debug.Log(elapsedTimer);
            elapsedTimer += Time.deltaTime;
            if (statoGiornata == StatoGiornata.Notte)
            {
                audiomanager.SwapMusicLevel("giorno", "notte");

                sfondo.color = Color.Lerp(sfondo.color, sfondoDark, elapsedTimer / totalTimer);
                globalLight.color = Color.Lerp(globalLight.color, luceDark, elapsedTimer / totalTimer);
            }
            else
            {
                audiomanager.SwapMusicLevel("notte", "giorno");

                sfondo.color = Color.Lerp(sfondo.color, Color.white, elapsedTimer / totalTimer);
                globalLight.color = Color.Lerp(globalLight.color, Color.white, elapsedTimer / totalTimer);
            }
            yield return null;
        }
        secondCor = null;
             yield return null;
    }
}
