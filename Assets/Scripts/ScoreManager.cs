using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public AudioManager aM;
    public TMP_Text[] Puestos;

    void Awake()
    {
        LoadData();
    }

    // Start is called before the first frame update
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        int nivel = PlayerPrefs.GetInt("Level", 1);

        GameObject canvas = GameObject.Find("Canvas");
        ArrayList puntuaciones = new ArrayList();
        ArrayList nombres = new ArrayList();

        int n = PlayerPrefs.GetInt("numPuntuaciones", 0);

        for (int i = 0; i < n; i++)
        {
            puntuaciones.Add(PlayerPrefs.GetInt("puntuacion" + i, 0));
        }

        for (int i = 0; i < n; i++)
        {
            nombres.Add(PlayerPrefs.GetString("nombre" + i, ""));
        }

        ArrayList puntuacionesOrd = new ArrayList(puntuaciones);
        puntuacionesOrd.Sort();
        object[] a = puntuacionesOrd.ToArray();

        float lineaActualX = 336.2749f;
        float lineaActualY = 42.96978f;
        float lineaActualZ = 323.2047f;
        float sepLineaX = 0.0294f;
        float sepLineaY = -1.0756f;
        float sepLineaZ = 0.0068f;

        int puesto = 1;

        for (int i = a.Length - 1; i >= 0; i--)
        {
            object p = a[i];

            int cont = 0;

            for (int z = 0; z < puntuaciones.Capacity; z++)
            {
                if (puntuaciones[z] == p)
                {
                    break;
                }
                else
                {
                    cont++;
                }
            }

            string nombre = (string)nombres[cont];
            int puntuacion = (int)puntuaciones[cont];
            char[] letras = nombre.ToCharArray();

            if (letras.Length < 10)
            {
                Puestos[puesto - 1].text = puesto + "                 ";

                for (int j = 0; j < 10; j++)
                {
                    if (j < letras.Length)
                    {
                        Puestos[puesto - 1].text += letras[j];
                    }
                    else
                    {
                        Puestos[puesto - 1].text += "  ";
                    }
                }

                Puestos[puesto - 1].text += "               " + puntuacion;
            }
            else
            {
                Puestos[puesto - 1].text = puesto + "                 " + nombre + "               " + puntuacion;
            }

            lineaActualX += sepLineaX;
            lineaActualY += sepLineaY;
            lineaActualZ += sepLineaZ;

            puesto++;

            if (puesto > 10)
            {
                break;
            }
        }
    }

    public void LoadData()
    {
        int width = PlayerPrefs.GetInt("widthScreen", 1920);
        int height = PlayerPrefs.GetInt("heightScreen", 1080);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        float soundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);

        //Resolución

        Screen.SetResolution(width, height, true);

        //Música y Sonido

        AudioManager.Instance.musicPlayer.volume = musicVolume;
        AudioManager.Instance.soundPlayer.volume = soundVolume;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
