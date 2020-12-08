using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections.Generic;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public int numOfPlayer = 0;
    public Popup selectRoom, playerChoose, noNetwork;
    public GameObject loadingView;

    public InputField createRoomTF, joinRoomTF;
    public bool isConnecting;

    public bool ismaster;
    public List<string> PlayersInRoom;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void MulitplayerBtn()
    {
        isConnecting = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            PhotonNetwork.NickName = GameManager.PlayerAvatarName;
        }
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
        StartCoroutine(CheckNetwork());
    }

    public override void OnJoinedLobby()
    {
        //playerChoose.ShowPopup();
        selectRoom.ShowPopup();
    }

    public void OnPlayerSelect(ToggleGroup group)
    {
        playerChoose.HidePopup(false);
        int i = 4;
        foreach (var t in group.ActiveToggles())
        {
            i = int.Parse(t.name);
            GameMaster.instance.multiPlayerNum = i;
        }
        //StartCoroutine(StartMultiPlayerGameMode(i));
        selectRoom.ShowPopup();
        GameManager.PlayButton();
    }

    //public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    //{
    //    Debug.LogError("OmMasterClientSwithed .... ");
    //    ismaster = PhotonNetwork.IsMasterClient;
    //    string nextTurnName = PhotonNetwork.CurrentRoom.CustomProperties["nextTurn"].ToString();
    //    if (!PlayersInRoom.Contains(nextTurnName))
    //    {
    //        SetNextPlayer(0);
    //    }
    //}

    public void updatePlayerInRoom()
    {
        PlayersInRoom.Clear();
        for (int i = 0; i < 200; i++)
        {
            Photon.Realtime.Player TempPlayer;
            if (PhotonNetwork.CurrentRoom.Players.TryGetValue(i, out TempPlayer))
            {
                //Debug.LogError("TempPlayer.NickName = " + TempPlayer.NickName);
                PlayersInRoom.Add(TempPlayer.NickName);
            }
            if (PlayersInRoom.Count == PhotonNetwork.CurrentRoom.PlayerCount)
                break;
        }
    }


    public void OnClick_JoinRoom()
    {
        if (joinRoomTF.text.Length >= 2)
        {
            PhotonNetwork.JoinRoom(joinRoomTF.text, null);
            selectRoom.HidePopup();
            loadingView.SetActive(true);
        }
        
    }
    public void OnClick_CreateRoom()
    {
        if (createRoomTF.text.Length >= 2)
        {
            PhotonNetwork.CreateRoom(createRoomTF.text, new RoomOptions { MaxPlayers = 2/*(byte)GameMaster.instance.multiPlayerNum*/ }, null);
            selectRoom.HidePopup();
            loadingView.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(CheckNetwork());
        ismaster = PhotonNetwork.IsMasterClient;
        updatePlayerInRoom();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
            loadingView.SetActive(false);

            GameManager.currentGameMode = GameMode.MultiPlayer;            
            PhotonNetwork.LoadLevel(2);
        }
        //else
        //{
        //    Debug.Log("Waiting for another player");
        //}
    }
    
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        StartCoroutine(CheckNetwork());
        updatePlayerInRoom();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
            loadingView.SetActive(false);

            GameManager.currentGameMode = GameMode.MultiPlayer;            
            PhotonNetwork.LoadLevel(2);
        }
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("RoomFailed " + returnCode + "    Message " + message);
        StartCoroutine(CheckNetwork());

    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        ismaster = PhotonNetwork.IsMasterClient;
    }

    IEnumerator StartMultiPlayerGameMode(int i)
    {
        loadingView.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 10f));
        loadingView.SetActive(false);
        //SetTotalPlayer(i);
        //SetupGame();

        //bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
        //if (leftRoom)
        //{
        //    float inTime = Random.Range(7, 5 * 60);
        //    StartCoroutine(RemovePlayerFromRoom(inTime));
        //}

        //multiplayerLoaded = true;
    }

    public void CloseGame()
    {
        StartCoroutine(DoSwitchScene());
    }

    IEnumerator DoSwitchScene()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene(0);
    }

    IEnumerator CheckNetwork()
    {
        while (true)
        {
            WWW www = new WWW("https://www.google.com");
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (noNetwork.isOpen)
                {
                    noNetwork.HidePopup();

                    Time.timeScale = 1;
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    noNetwork.ShowPopup();

                    Time.timeScale = 0;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }
}
