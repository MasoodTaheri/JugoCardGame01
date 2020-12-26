using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public InputField playerName;

    public Text coins;

    public Datamanager dataManager;

    void Start()
    {
        //dataManager.Load();
        //playerName.text = dataManager.data.name;
        //coins.text = dataManager.data.coins.ToString();
    }
    public void ClickCoin()
    {
        //dataManager.data.coins += 1;
        //coins.text = dataManager.data.coins.ToString();
    }

    public void ChangeName(string text)
    {
       // dataManager.data.name = text;
    }
    public void ClickSave()
    {
        dataManager.Save();
    }
}
