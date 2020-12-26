using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LuckySpinManager : MonoBehaviour
{
    public FortuneWheelManager fortuneWheelManager;
    public int spinCnt;
    public bool CheckRemainingTime;
    DateTime fetchedDate;
    public const string nextSpinChance = "nextSpinChance";
    public const string availableAdsKey = "availableAdsKey";
    public const string boostedKey = "boostedKey";
    public enum SpinWheelState { FreeChance, Ad, Purchase };
    public SpinWheelState state;
    public GameObject FreeSpin;
    public GameObject AdBtnSpin;
    public GameObject PurchaseSpin;
    public int RefAvailableAds;
    public static int Chances = 0;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
    }

    private void OnEnable()
    {


        CheckForState();
    }
    public void CheckForState()
    {
        state = SpinWheelState.Purchase;
        FreeSpin.gameObject.SetActive(false);
        AdBtnSpin.gameObject.SetActive(false);
        PurchaseSpin.gameObject.SetActive(false);

        if (DateTime.Now > GetNextSpinDate())
        {
            state = SpinWheelState.FreeChance;
            Chances = 1;
            AvailableAds = 2;
            BoostedValue = false;
        }
        else
        {

            if (AvailableAds > 0)
            {
                if (Chances == 0)
                {
                    AdBtnSpin.gameObject.SetActive(true);
                }
                state = SpinWheelState.Ad;
            }
            else
            {
                //PurchaseSpin.gameObject.SetActive(true);
                state = SpinWheelState.Purchase;
                BoostedValue = false;
            }

        }
        if (Chances > 0)
        {
            FreeSpin.gameObject.SetActive(true);
        }
        fortuneWheelManager.CheckBoostState(BoostedValue);
        RefAvailableAds = AvailableAds;
    }
    public void PlayAd()
    {
        OnClosedAD();
    }

    public void OnClosedAD()
    {
        BoostedValue = true;
        Chances = 1;
        CheckForState();
    }
    DateTime GetNextSpinDate()
    {
        return DateTime.Parse(PlayerPrefs.GetString(nextSpinChance, "12/7/2020 10:13:40 PM")); ;
    }
    void SetNextSpinDate()
    {
        PlayerPrefs.SetString(nextSpinChance,
           DateTime.Now.AddDays(1).ToString());
    }

    int AvailableAds
    {
        get
        {
            return PlayerPrefs.GetInt(availableAdsKey, 2);
        }

        set
        {
            PlayerPrefs.SetInt(availableAdsKey, value);
        }
    }

    public bool BoostedValue
    {
        get
        {
            return PlayerPrefs.GetInt(boostedKey, 0) == 1 ? true : false;
        }

        set
        {
            PlayerPrefs.SetInt(boostedKey, value == true ? 1 : 0);
        }
    }
    public void OnWheelSpun()
    {
        switch (state)
        {
            case SpinWheelState.FreeChance:
                SetNextSpinDate();
                break;
            case SpinWheelState.Ad:
                AvailableAds = AvailableAds - 1;
                break;
            case SpinWheelState.Purchase:
                break;
            default:
                break;
        }
        Chances = 0;
        CheckForState();
    }

    public List<SavedItems> AllPuchasedData;
}

[System.Serializable]
public class SavedItems
{
    public string iconNane;
    public string value;

}
