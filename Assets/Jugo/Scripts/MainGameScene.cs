using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainGameScene : MonoBehaviour
{
    public static MainGameScene instance;
    public Text scoreText;

    public Popup WaittingPanel;
    public GameObject loadingView, noRoomPanel;
    public Text failedRoomName;
    public GameObject vsFriendsPanel, createRPanel, joinRPanel;
    public InputField createRoomTF, joinRoomTF;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public GameObject comingSoonPanel;
    public Image playerImg;
    public Text playerName;
    public InputField playerName1;

    public int score;
    public int currentLevl;
    public int nextLevl;
    public TextAsset userLevels;

    public Slider lvlSlider;
    public Text currentLvl, nextLvl;

    void Start()
    {
        ProfileClick();
        playerImg.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        UpdateScore();
        //var levels = JsonUtility.FromJson<UserLevels>(userLevels.text).levelXP;
        //for (int i = 0; i < levels.Count; i++)
        //{
        //    if (score >= levels[i].totalScore)
        //    {
        //        Debug.LogError("levels[i].totalScore = " + levels[i].totalScore);

        //        currentLevl = levels[i].currentLvl;
        //        nextLevl = levels[i].nextLvl;

        //        currentLvl.text = currentLevl.ToString();
        //        nextLvl.text = nextLevl.ToString();

        //        UpdateXP(score, levels[i].currentLvl, levels[i].totalScore, levels[i + 1].totalScore);
        //    }
        //}
    }

    public void UpdateScore()
    {
        scoreText.text = PrefManager.instance.GetScore().ToString();
    }

    public void UpdateXP(int xp, int currentlvl, int diffXP, int nextXP)
    {
        int curLvl = (int)(1.5f * Mathf.Sqrt(xp));
        if (curLvl != currentlvl)
        {
            currentlvl = curLvl;
        }
        int differentXP = nextXP - xp;
        int totalDifferenceXP = nextXP - diffXP;

        lvlSlider.value = ((float)differentXP / (float)totalDifferenceXP);
    }
    public void ProfileClick()
    {
        playerImg.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        playerName.text = GameManager.PlayerAvatarName;
        playerName.GetComponent<EllipsisText>().UpdateText();
        playerName1.text = GameManager.PlayerAvatarName;
        //playerName1.GetComponent<EllipsisText>().UpdateText();
    }

    public void UpdatePlayerName()
    {
        GameManager.PlayerAvatarName = playerName1.text;
        playerName.text = GameManager.PlayerAvatarName;
        playerName.GetComponent<EllipsisText>().UpdateText();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void League_TrainingBtn()
    {
        StartCoroutine(DisableComingSoon());
    }

    IEnumerator DisableComingSoon()
    {
        comingSoonPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        comingSoonPanel.SetActive(false);
    }
}

[System.Serializable]
public class UserLevel
{
    public int totalScore;
    public int currentLvl;
    public int nextLvl;
}
public class UserLevels
{
    public List<UserLevel> levelXP;
}