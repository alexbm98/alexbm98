using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurtleShellController : MonoBehaviour
{
    public static TurtleShellController Instance { get; private set; }

    public int vidas;
    public float da�o;
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
        PlayerManager.Instance.enemigosDerrotadosTurtle++;
    }

    public static explicit operator TurtleShellController(GameObject v)
    {
        throw new NotImplementedException();
    }

    public void HandleMovement()
    {
        if (!animator.GetBool("muere"))
        {
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, player.position, velocidad * Time.deltaTime);
            GetComponent<Transform>().LookAt(player.position);
            animator.SetBool("seMueve", true);
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
                        PlayerManager.Instance.currentHealth -= da�o;
                    }
                }
            }
        }
    }

    public void OnTriggerStay(Collider collision)
    {
        if (this.gameObject != null)
        {
            if (collision.CompareTag("Player") && !animator.GetBool("muere"))
            {
                animator.SetBool("atacar", true);
                PlayerManager.Instance.animatorP.SetBool("esGolpeado", true);

                if (!PlayerManager.Instance.animatorP.GetBool("defender"))
                {
                    PlayerManager.Instance.currentHealth -= da�o * Time.deltaTime;
                }
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
            vidas = 1;
            da�o = 10f;
            velocidad = 1.75f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Normal")
        {
            vidas = 2;
            da�o = 20f;
            velocidad = 2f;
        }

        if (PlayerPrefs.GetString("Difficulty") == "Dificil")
        {
            vidas = 3;
            da�o = 30f;
            velocidad = 2f;
        }
    }
}
