using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Text UIScore;

    //RegisteringEvent
    private void OnEnable()
    {
        //GameHandler.ScoreUpdatedEvent += GameManager_ScoreUpdatedEvent;
    }

    private void OnDisable()
    {
        //GameHandler.ScoreUpdatedEvent -= GameManager_ScoreUpdatedEvent;
    }

    private void GameManager_ScoreUpdatedEvent()
    {
        GetScore();
    }

    void Start()
    {
        GetScore();
    }

    void GetScore()
    {
        UIScore.text = PrefManager.instance.GetScore().ToString();
    }
}
