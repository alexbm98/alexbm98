using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    public AudioManager aM;
    public TMP_InputField nombre;

    void Awake()
    {
        LoadData();
    }

    public void Registrar()
    {
        int i = PlayerPrefs.GetInt("numPuntuaciones", 0);

        //Guardado de datos
        PlayerPrefs.SetInt("puntuacion" + i, PlayerPrefs.GetInt("UltimaMejorPuntuacion"));
        PlayerPrefs.SetString("nombre" + i, nombre.text);
        PlayerPrefs.SetInt("numPuntuaciones", i + 1);
    }

    public void VolverAMenuPrincipal()
    {
        aM.PlaySound("Click");
        SceneManager.LoadScene("Title");
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
