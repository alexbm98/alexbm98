using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text salud;
    public TMP_Text Tminutos;
    public TMP_Text Tsegundos;
    private float tiempoRestanteDT;
    public int minutos;
    public int segundos;

    private void Start()
    {
        tiempoRestanteDT = PlayerPrefs.GetFloat("time", 180);
    }

    // Update is called once per frame
    void Update()
    {
        HandleRemainingTime();
        HandleWin();
    }

    public void HandleWin()
    {
        if (tiempoRestanteDT < 0)
        {
            PlayerPrefs.SetInt("UltimaMejorPuntuacion", PlayerManager.Instance.enemigosDerrotadosTurtle + PlayerManager.Instance.enemigosDerrotadosDemon);
            SceneManager.LoadScene("Victoria");
        }
    }

    public void HandleRemainingTime()
    {
        if (tiempoRestanteDT >= 0)
        {
            int tiempoRestante = (int)tiempoRestanteDT;
            minutos = (int)(tiempoRestante / 60);
            float s = tiempoRestante % 60;

            if (s != 0)
            {
                segundos = (int)(tiempoRestante - (minutos * 60));
            }

            if (minutos >= 10)
            {
                Tminutos.text = minutos.ToString();
            }
            else
            {
                Tminutos.text = "0" + minutos.ToString();
            }

            if (segundos >= 10)
            {
                Tsegundos.text = segundos.ToString();
            }
            else
            {
                Tsegundos.text = "0" + segundos.ToString();
            }

            tiempoRestanteDT = tiempoRestanteDT - Time.deltaTime;
        }
        else
        {
            Tminutos.text = "00";
            Tsegundos.text = "00";
        }
    }
}
