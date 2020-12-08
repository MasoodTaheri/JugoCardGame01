using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PunPlayerInfo : MonoBehaviourPun
{
    public PhotonView photonView;

    void Start()
    {
        if(photonView.IsMine)
        {
            //GameMaster.instance.playerNames.Add(PhotonNetwork.playerName);
            Debug.LogError("PhotonNetwork.playerName = "+ PhotonNetwork.NickName);
        }
        else
        {
            //GameMaster.instance.playerNames.Add(photonView.owner.NickName);
            Debug.LogError("photonView.owner.NickName = " + photonView.Owner.NickName);
        }
    }

}
