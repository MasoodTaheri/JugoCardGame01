using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    //Declaration
    public delegate void ScoreUpdated();
    public static event ScoreUpdated ScoreUpdatedEvent;

    public static GameHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetScore(int val)
    {
        PrefManager.instance.SetScore(val);
        //RasingEvent
        ScoreUpdatedEvent?.Invoke();
    }
}
