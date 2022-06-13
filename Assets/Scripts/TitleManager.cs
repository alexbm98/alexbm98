using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public AudioSource aM;

    public void Awake()
    {
        LoadData();
    }

    public void CargarJuego()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void CargarAjustes()
    {
        SceneManager.LoadScene("Configuracion");
    }

    public void CargarMarcadores()
    {
        SceneManager.LoadScene("Marcadores");
    }

    public void Salir()
    {
        Application.Quit();
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
}
