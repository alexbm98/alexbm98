using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DemonController : MonoBehaviour
{
    public static DemonController Instance { get; private set; }

    public int vidas;
    public float daño;
    public float velocidad;
    private Transform player;
    public Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadParameters();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        tag = "Enemy";
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleDie();
    }

    void OnDestroy()
    {
        PlayerManager.Instance.enemigosDerrotadosDemon++;
    }

    public static explicit operator DemonController(GameObject v)
    {
        throw new NotImplementedException();
    }

    public void HandleMovement()
    {
        if (!animator.GetBool("muere"))
        {
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, player.position, velocidad * Time.deltaTime);
            GetComponent<Transform>().LookAt(player.position);
            animator.SetBool("Walk Forward", true);
        }
    }

    public void HandleDie()
    {
        if (vidas <= 0)
        {
            animator.SetBool("muere", true);
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Destroy(this.gameObject, 4);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (this.gameObject != null)
        {
            if (collision.CompareTag("Weapon") && PlayerManager.Instance.animatorP.GetBool("atacar01"))
            {
                vidas--;
                animator.SetBool("esGolpeado", true);
                GetComponent<Rigidbody>().AddForce(Vector3.back * 15f, ForceMode.Impulse);
            }
            else
            {
                if (collision.CompareTag("Player") && !animator.GetBool("muere"))
                {
                    animator.SetBool("atacar", true);
                    PlayerManager.Instance.animatorP.SetBool("esGolpeado", true);

                    if (!PlayerManager.Instance.animatorP.GetBool("defender"))
                    {
                        PlayerManager.Instance.currentHealth -= daño * Time.deltaTime;
                    }
                }
            }
        }
    }

    public void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player") && !animator.GetBool("muere"))
        {
            animator.SetBool("atacar", true);
            PlayerManager.Instance.animatorP.SetBool("esGolpeado", true);

            if (!PlayerManager.Instance.animatorP.GetBool("defender"))
            {
                PlayerManager.Instance.currentHealth -= daño * Time.deltaTime;
            }
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (this.gameObject != null)
        {
            if (collision.CompareTag("Weapon"))
            {
                animator.SetBool("esGolpeado", false);
            }
            else
            {
                if (collision.CompareTag("Player"))
                {
                    animator.SetBool("atacar", false);
                    PlayerManager.Instance.animatorP.SetBool("esGolpeado", false);
                }
            }
        }
    }

    private void LoadParameters()
    {
        if (PlayerPrefs.GetString("Difficulty") == "Facil")
        {
            vidas = 2;
            daño = 10f;
            velocidad = 1.75f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Normal")
        {
            vidas = 3;
            daño = 20f;
            velocidad = 2f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Dificil")
        {
            vidas = 4;
            daño = 30f;
            velocidad = 2f;
        }
    }
}
