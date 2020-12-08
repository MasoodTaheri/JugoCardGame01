using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Mypun : MonoBehaviourPunCallbacks
{
    public GameObject connectPanel, createRoomPanel;
    public InputField createRoomIF, joinRoomIF;

    public void ConnectBtn()
    {
        Debug.LogError("ConnectBtn ...... . ");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //StartCoroutine(CheckNetwork());
    }

    public override void OnJoinedLobby()
    {
        Debug.LogError("OnJoinedLobby ...... . ");
        connectPanel.SetActive(false);
        createRoomPanel.SetActive(true);
    }

    public void OnClick_JoinRoom()
    {
        if (joinRoomIF.text.Length >= 2)
        {
            PhotonNetwork.JoinRoom(joinRoomIF.text, null);
        }
    }

    public void OnClick_CreateRoom()
    {
        if (createRoomIF.text.Length >= 2)
        {
            PhotonNetwork.CreateRoom(createRoomIF.text, new RoomOptions { MaxPlayers = 2/*(byte)GameMaster.instance.multiPlayerNum*/ }, null);
        }
    }

    public override void OnJoinedRoom()
    {
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        //{
        //    // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
            
            PhotonNetwork.NickName = GameManager.PlayerAvatarName;
            PhotonNetwork.LoadLevel("PUNTurnBase_TestScene1");
        //}
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        //{
            // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                        
            PhotonNetwork.NickName = GameManager.PlayerAvatarName;
            PhotonNetwork.LoadLevel("PUNTurnBase_TestScene1");
        //}
    }
}
