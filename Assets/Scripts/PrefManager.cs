using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefManager : MonoBehaviour
{
    public static PrefManager instance;
    public const string Score = "Score";

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public int GetScore()
    {
        return PlayerPrefs.GetInt(Score, 0);
    }

    public void SetScore(int val)
    {
        PlayerPrefs.SetInt(Score, GetScore() + val);
    }
}
