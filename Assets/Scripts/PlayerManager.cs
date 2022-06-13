using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Camera camera;
    public Image health;
    public Image stamina;
    public Rigidbody player;
    public Animator animatorP;
    private Vector3 offset;
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    private int recargaStamina = 0;
    private float velRecargaStamina = 0;
    private float velGastoStamina = 0;
    public int enemigosDerrotadosTurtle = 0;
    public int enemigosDerrotadosDemon = 0;
    public TMP_Text TEnemigosDerrotadosTurtle;
    public TMP_Text TEnemigosDerrotadosDemon;

    private void Awake()
    {
        Instance = this;
        LoadData();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadParameters();
        tag = "Player";
        offset = new Vector3(0f, 1.3f, -2.38f);
        SpawnManager.Instance.StartSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject != null)
        {
            HandleCameraMovement();
            HandlePlayerMovement();
            HandleHealth();
            HandleStamina();
            HandleDie();
            HandleScore();
        }
    }

    public void HandleScore()
    {
        TEnemigosDerrotadosTurtle.text = enemigosDerrotadosTurtle.ToString();
        TEnemigosDerrotadosDemon.text = enemigosDerrotadosDemon.ToString();
    }

    public void HandleHealth()
    {
        health.fillAmount = currentHealth / maxHealth;
    }

    public void HandleStamina()
    {
        if (recargaStamina == 0 && currentStamina < maxStamina)
        {
            currentStamina += velRecargaStamina;
        }
        
        if (recargaStamina == 1 && currentStamina > 0)
        {
            currentStamina -= velGastoStamina;
        }

        stamina.fillAmount = currentStamina / maxStamina;
    }

    public void HandleCameraMovement()
    {
        if (Input.GetKey("a"))
        {
            camera.transform.LookAt(player.transform.position);
            camera.transform.Rotate(-24.5f, 0f, 0f);
            player.transform.Rotate(-1.1f * Vector3.up);
        }

        if (Input.GetKey("d"))
        {
            camera.transform.LookAt(player.transform.position);
            camera.transform.Rotate(-24.5f, 0f, 0f);
            player.transform.Rotate(1.1f * Vector3.up);
        }
    }

    public void HandlePlayerMovement()
    {
        if (!animatorP.GetBool("muere"))
        {
            if (Input.GetKey("w") && !animatorP.GetBool("defender") && !animatorP.GetBool("atacar01"))
            {
                if (Input.GetKey("left shift"))
                {
                    if (currentStamina > 0)
                    {
                        animatorP.SetBool("corre", true);
                        animatorP.SetBool("seMueveAdelante", true);
                        player.transform.Translate(0, 0, 0.075f);
                        recargaStamina = 1;
                    }
                    else
                    {
                        animatorP.SetBool("corre", false);
                        animatorP.SetBool("seMueveAdelante", true);
                        player.transform.Translate(0, 0, 0.045f);
                    }
                }
                else
                {
                    animatorP.SetBool("seMueveAdelante", true);
                    player.transform.Translate(0, 0, 0.045f);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                animatorP.SetBool("atacar01", true);
                recargaStamina = 2;
            }

            if (Input.GetMouseButtonUp(0))
            {
                animatorP.SetBool("atacar01", false);
                recargaStamina = 0;
            }

            if (Input.GetMouseButtonDown(1))
            {
                animatorP.SetBool("defender", true);
                recargaStamina = 2;
            }

            if (Input.GetMouseButtonUp(1))
            {
                animatorP.SetBool("defender", false);
                recargaStamina = 0;
            }

            if (Input.GetKeyUp("left shift"))
            {
                animatorP.SetBool("corre", false);
                recargaStamina = 0;
            }

            if (Input.GetKeyUp("w"))
            {
                animatorP.SetBool("seMueveAdelante", false);
                recargaStamina = 0;
            }

            if (Input.GetKey("s") && !animatorP.GetBool("defender") && !animatorP.GetBool("atacar01"))
            {
                animatorP.SetBool("seMueveAtras", true);
                player.transform.Translate(0, 0, -0.045f);
                recargaStamina = 0;
            }

            if (Input.GetKeyUp("s"))
            {
                animatorP.SetBool("seMueveAtras", false);
                recargaStamina = 0;
            }
        }
    }

    public void HandleDie()
    {
        if (currentHealth <= 0)
        {
            animatorP.SetBool("muere", true);
        }
    }

    public void LoadData()
    {
        int width = PlayerPrefs.GetInt("widthScreen", 1920);
        int height = PlayerPrefs.GetInt("heightScreen", 1080);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);

        //Resolución

        Screen.SetResolution(width, height, true);

        //Música y Sonido

        AudioManager.Instance.musicPlayer.volume = musicVolume;
    }

    private void LoadParameters()
    {
        if (PlayerPrefs.GetString("Difficulty") == "Facil")
        {
            velRecargaStamina = 0.5f;
            velGastoStamina = 0.5f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Normal")
        {
            velRecargaStamina = 0.25f;
            velGastoStamina = 0.5f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Dificil")
        {
            velRecargaStamina = 0.25f;
            velGastoStamina = 0.75f;
        }
    }
}