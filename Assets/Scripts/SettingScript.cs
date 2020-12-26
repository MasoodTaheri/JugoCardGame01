using System.Collections;
using System.Collections.Generic;
//using AssemblyCSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{

    public GameObject Sounds;
    public GameObject Vibrations;
    public GameObject Notifications;
    void Start()
    {
        setting();
    }

    private void OnEnable()
    {
        setting();
    }


    private void setting()
    {

        //Debug.LogError("sett");
        if (PlayerPrefs.GetInt(StaticStrings.SoundsKey, 0) == 1)
        {
            Sounds.GetComponent<Toggle>().isOn = false;
        }
        else
        {
            Sounds.GetComponent<Toggle>().isOn = true;
        }

        if (PlayerPrefs.GetInt(StaticStrings.NotificationsKey, 0) == 1)
        {
            Notifications.GetComponent<Toggle>().isOn = false;
        }

        //if (PlayerPrefs.GetInt(StaticStrings.VibrationsKey, 0) == 1)
        //{
        //    Vibrations.GetComponent<Toggle>().isOn = false;
        //}

        Sounds.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        Notifications.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
       // Vibrations.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
       
        Sounds.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            //Debug.LogError("sett2");
            PlayerPrefs.SetInt(StaticStrings.SoundsKey, value ? 0 : 1);
            if (value)
            {
                AudioListener.volume = 1;
            }
            else
            {
                AudioListener.volume = 0;
            }
        }
        );

        Notifications.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(StaticStrings.NotificationsKey, value ? 0 : 1);
        }
        );

        Vibrations.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt(StaticStrings.VibrationsKey, value ? 0 : 1);
        }
        );

        
    }
}
