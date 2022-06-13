using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseManager : MonoBehaviour
{
    public AudioManager aM;

    void Awake()
    {
        LoadData();
    }

    public void VolverAIntentarlo()
    {
        aM.PlaySound("Click");
        SceneManager.LoadScene("Level 1");
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

        //Resoluci�n

        Screen.SetResolution(width, height, true);

        //M�sica y Sonido

        AudioManager.Instance.musicPlayer.volume = musicVolume;
        AudioManager.Instance.soundPlayer.volume = soundVolume;
    }
}
