using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RandomNum : MonoBehaviourPunCallbacks
{
    public PhotonView photonView;
    public int BotMoney;

    public bool botStealable = true;
    int randomMoney;
    private void Start()
    {
        //int randomMoney = Random.Range(12, 60);
        photonView.RPC("ReceiveRandomMoneyFromBot", RpcTarget.All, randomMoney);

        Debug.LogError("BotMoney = "+ BotMoney);
    }

    [PunRPC]
    private void ReceiveRandomMoneyFromBot(int moneyAmount)
    {
        moneyAmount = Random.Range(12, 60);
        BotMoney = moneyAmount;

    }
}
