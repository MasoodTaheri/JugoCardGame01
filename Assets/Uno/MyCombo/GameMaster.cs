using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    public List<string> playerNames;

    public int multiPlayerNum;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        playerNames = new List<string>();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
