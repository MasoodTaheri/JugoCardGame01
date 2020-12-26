using NiobiumStudios;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyAdsRewards : MonoBehaviour
{
    public int dailyreward;
    public int lastReward;
    public DailyAdsRewards dailyAdsRewards;
    private const string LAST_REWARD = "LastReward";

    public GameObject Ads;
    public GameObject panelReward;              // Rewards panel
    public Text textReward;                     // Reward Text to show an explanatory message to the player
    public Button buttonCloseReward;
    public Image imageReward;
    public int rewardAmt = 200;
    public string rewardIconName="Coins";

    public GameObject RewardBtn;
    private string GetLastRewardKey()
    {
        return LAST_REWARD;
    }
    private DailyRewards dailyRewards;          // DailyReward Instance      

    void Awake()
    {
        dailyRewards = GetComponent<DailyRewards>();
    }
    private void Start()
    {
        buttonCloseReward.onClick.AddListener(() =>
        {
            panelReward.SetActive(false);
        });
    }
   

    void OnDisable()
    {
        if (dailyRewards != null)
        {
            dailyRewards.onClaimPrize -= OnClaimPrize;
           
        }
    }

    private void OnClaimPrize(int day)
    {
        CheckForAdsEligiblity();
    }

    private void OnEnable()
    {
        dailyRewards.onClaimPrize += OnClaimPrize;
       
        CheckForAdsEligiblity();
    }
    void CheckForAdsEligiblity()
    {
        lastReward = PlayerPrefs.GetInt(GetLastRewardKey());
        if (lastReward >= 6)
        {
            RewardBtn.SetActive(true);
        }
        else
        {
            RewardBtn.SetActive(false);
        }
    }
    public void OnDistributeReward()
    {
        Ads.SetActive(true);
        panelReward.SetActive(true);       
        imageReward.sprite = Datamanager.Instance.FetchSprite(rewardIconName);

        GameHandler.Instance.SetScore(rewardAmt);
        if (rewardAmt > 0)
        {
            textReward.text = string.Format("You got {0} {1}!", rewardAmt, rewardIconName);
        }
        else
        {
            textReward.text = string.Format("You got {0}!", rewardIconName);
        }
    }
}
