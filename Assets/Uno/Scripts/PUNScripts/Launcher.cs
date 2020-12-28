using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class Launcher : MonoBehaviourPunCallbacks, IMatchmakingCallbacks, IOnEventCallback
{

    public static Launcher instance;
    public string myname;
    public int MinimumPlayerInRoom = 2;
    public GameObject Root;

    public bool ismaster;
    //public List<PhotonView> PlayersInRoom;
    public List<string> PlayersInRoom;
    public List<string> RealPlayerInRoom;
    //public int CurrntTurnId;
    public Popup selectRoom, playerChoose, noNetwork, WaittingPanel;
    public GameObject loadingView;
    public InputField createRoomTF, joinRoomTF;
    public bool isConnecting;
    public int WaitBeforeStartGame;



    public const byte PhotonEvent_StartGame = 1;
    public const byte PhotonEvent_EndGame = 2;
    public const byte PhotonEvent_LeaveMatch = 3;
    public const byte PhotonEvent_DealCardCompleted = 4;
    public const byte PhotonEvent_SyncDeck = 5;


    void Awake()
    {
        instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);

    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;
        Debug.Log("Connecting ....");
        isConnecting = true;
        //Uimanager.instance.OnConnecting();
        PhotonNetwork.GameVersion = "1.0";
        PhotonNetwork.ConnectUsingSettings();
    }
    public void MulitplayerBtn()
    {
        WaittingPanel.ShowPopup();
        WaittingPanel.GetComponent<Waitforallplayers>().CurrnetPlayerTxt.gameObject.SetActive(false);
        Connect();
    }


    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        //Uimanager.instance.DisConnectedTomasterServer();

    }

    public void JoinRandomRoom()
    {
        // PhotonNetwork.JoinRandomRoom();//OnJoinedRoom   or OnJoinRandomFailed

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void JoinPreviousRoom()
    {
        string PreviousRoomName = PlayerPrefs.GetString("RoomName");
        Debug.Log("PreviousRoomName=" + PreviousRoomName);
        bool b = false;
        if (PreviousRoomName != "")
            b = PhotonNetwork.JoinRoom(PreviousRoomName);

        Debug.Log("JoinPreviousRoom b=" + b);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");

        if (isConnecting)
        {
            string PhotonNickName = PlayerPrefs.GetString("PhotonNickName", "player" + Random.Range(0, 10000).ToString());

            if (!PlayerPrefs.HasKey("PhotonNickName"))
                PlayerPrefs.SetString("PhotonNickName", PhotonNickName);
            PhotonNetwork.NickName = PhotonNickName;
            Debug.Log("I am " + PhotonNickName);
            myname = PhotonNetwork.NickName;
            PhotonNetwork.JoinRandomRoom();//OnJoinedRoom   or OnJoinRandomFailed
        }
        //Uimanager.instance.ConnectedTomasterServer();
    }

    [ContextMenu("ClearPlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("OnJoinRandomFailed " + returnCode + "  " + message);
        byte maxPlayersPerRoom = 5;
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }



    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        //Debug.Log("OnJoinedRoom " + PhotonNetwork.CurrentRoom.Name);
        ismaster = PhotonNetwork.IsMasterClient;
        //CurrentPlayer.transform.SetParent(Root.transform);
        Debug.Log("OnJoinedRoom " + PhotonNetwork.NickName + "  id=" + PhotonNetwork.LocalPlayer.ActorNumber);
        PlayerPrefs.SetString("RoomName", PhotonNetwork.CurrentRoom.Name);
        //updatePlayerInRoom();
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        //{
        //    Debug.Log("We load the 'Room for 1' ");
        //    //PhotonNetwork.LoadLevel("PunBasics-Room for 1");
        //}
        UpdateCardsText();
        GetallplayerAroundTable();

        if (GetRoomCustomProperty("GameState") == "Started")
        {
            FillPlayersInRoom();
            //Uimanager.instance.GeneratePlayer();
        }
        //else
        //Uimanager.instance.WaitForStart();
        WaittingPanel.GetComponent<Waitforallplayers>().CurrnetPlayerTxt.gameObject.SetActive(true);
        WaittingPanel.GetComponent<Waitforallplayers>().CurrnetPlayerTxt
            .text = "Current Player In Room =" + PhotonNetwork.CurrentRoom.PlayerCount;
    }







    public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting
        WaittingPanel.GetComponent<Waitforallplayers>().CurrnetPlayerTxt
    .text = "Current Player In Room =" + PhotonNetwork.CurrentRoom.PlayerCount;
        //updatePlayerInRoom();
        UpdateCardsText();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            // if player with turn left the room we select next player to play the game
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            string nextturnName = PhotonNetwork.CurrentRoom.CustomProperties["nextturn"].ToString();
            if (other.NickName == nextturnName)
            {
                Debug.Log("player with turn left the room");
                for (int i = 0; i < PlayersInRoom.Count; i++)
                    if (PlayersInRoom[i] == other.NickName)
                    {
                        //CurrntTurnId++;
                        //if (CurrntTurnId >= PlayersInRoom2.Count)
                        //    CurrntTurnId = 0;
                        //Debug.Log("nextturn=" + CurrntTurnId + "  " + PlayersInRoom2[CurrntTurnId]);
                        SetNextPlayer();
                    }
                //Hashtable props = new Hashtable { { "nextturn", PlayersInRoom2[CurrntTurnId] } };
                //PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            //LoadArena();
        }
        //updatePlayerInRoom();
    }

    public void SetNextPlayer(int playerid = -1)
    {
        string str = (playerid == -1) ?
            NextInList(PlayersInRoom, PhotonNetwork.CurrentRoom.CustomProperties["nextturn"].ToString())
            : PlayersInRoom[playerid];

        SetRoomCustomProperty("nextturn", str);
        //Hashtable props = new Hashtable { { "nextturn", str } };
        //PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        //Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
    }
    public string NextInList(List<string> list, string Current)
    {
        int currentid = 0;
        for (int i = 0; i < list.Count; i++)
            if (list[i] == Current) currentid = i;


        currentid++;
        if (currentid >= list.Count)
            currentid = 0;

        Debug.Log("IsUserExistInRoomNow? " + list[currentid] + IsUserExistInRoomNow(list[currentid]));
        if (IsUserExistInRoomNow(list[currentid]))
            return list[currentid];
        else
            return NextInList(PlayersInRoom, list[currentid]);


        ////check if next player is exist in room:
        //List<string> RealPlayerInRoom = new List<string>();
        //for (int i = 0; i < 200; i++)
        //{
        //    Photon.Realtime.Player TempPlayer;
        //    if (PhotonNetwork.CurrentRoom.Players.TryGetValue(i, out TempPlayer))
        //        RealPlayerInRoom.Add(TempPlayer.NickName);
        //    if (PlayersInRoom.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        //        break;
        //}
        //if (RealPlayerInRoom.Contains(list[currentid])) return list[currentid];
        //else return
        //    NextInList(PlayersInRoom, PhotonNetwork.CurrentRoom.CustomProperties["nextturn"].ToString())

        //return list[currentid];

    }
    //public int getUserPos(List<string> list, string Current)
    //{
    //    return list.IndexOf()
    //    for (int i=0;i<list.Count;i++)
    //    {
    //        if (list[i] == Current)
    //            return i;
    //    }
    //    return -1;

    //}

    public bool IsUserExistInRoomNow(string nickname)
    {
        Debug.Log("IsUserExistInRoomNow? " + nickname);
        //List<string> 
        RealPlayerInRoom = new List<string>();
        for (int i = 0; i < 25; i++)
        {
            Photon.Realtime.Player TempPlayer;
            if (PhotonNetwork.CurrentRoom.Players.TryGetValue(i, out TempPlayer))
                RealPlayerInRoom.Add(TempPlayer.NickName);
            //if (PlayersInRoom.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            //    break;
        }
        return RealPlayerInRoom.Contains(nickname);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
        ismaster = PhotonNetwork.IsMasterClient;
        string nextturnName = PhotonNetwork.CurrentRoom.CustomProperties["nextturn"].ToString();
        if (!PlayersInRoom.Contains(nextturnName))
        {

            //Hashtable props = new Hashtable
            //        {
            //    //{"nextturn",PhotonWrapper.instance.PlayersInRoom[0].Owner.NickName}
            //    {"nextturn",PhotonWrapper.instance.PlayersInRoom2[0]}
            //};
            //PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            SetNextPlayer(0);
        }
    }

    public override void OnLeftRoom()
    {
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }


    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            //Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }

        //Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        //PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }




    public void GetallplayerAroundTable()
    {
        PlayersInRoom.Clear();
        //for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        for (int i = 0; i < 200; i++)
        {
            Photon.Realtime.Player TempPlayer;
            if (PhotonNetwork.CurrentRoom.Players.TryGetValue(i, out TempPlayer))
            {
                PlayersInRoom.Add(TempPlayer.NickName);
            }
            if (PlayersInRoom.Count == PhotonNetwork.CurrentRoom.PlayerCount)
                break;
        }
        //Debug.Log(PlayersInRoom.Count);
        //Debug.Log(PlayersInRoom[0]);

    }

    public void AddcardToGame(string str)
    {
        string cards = GetRoomCustomProperty("Cards") + ",";
        if (cards == ",") cards = "Cards: ";
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Cards"))
        //    cards = PhotonNetwork.CurrentRoom.CustomProperties["Cards"].ToString() + ",";
        //else cards = "Cards: ";

        cards += str;
        SetRoomCustomProperty("Cards", cards);
        //Hashtable props = new Hashtable { { "Cards", cards } };
        //PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        //Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        UpdateCardsText();
    }

    public void UpdateCardsText()
    {
        string cards = "Cards: ";
        cards = GetRoomCustomProperty("Cards");
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Cards"))
        //    cards = PhotonNetwork.CurrentRoom.CustomProperties["Cards"].ToString();
        //Uimanager.instance.CardsInGame.text = cards;
    }

    public string GetRoomCustomProperty(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";
        //Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        //Debug.Log("GetRoomCustomProperty key=" + key);
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
        {
            string value = PhotonNetwork.CurrentRoom.CustomProperties[key].ToString();
            Debug.Log("GetRoomCustomProperty key=" + key + "  value=" + value);
            return value;
        }

        else
            return "";
    }

    public void SetAllPlayerAroundTable()
    {
        string allplayers = "";
        foreach (var item in PlayersInRoom)
        {
            allplayers += item + ",";
        }
        //Debug.Log(allplayers);
        allplayers = allplayers.Remove(allplayers.Length - 1);
        //Debug.Log(allplayers);
        SetRoomCustomProperty("PlayersInRoom", allplayers);
    }

    public void FillPlayersInRoom()
    {
        string str = GetRoomCustomProperty("PlayersInRoom");
        //Debug.Log("FillPlayersInRoom " + str);
        string[] str2 = str.Split(',');
        PlayersInRoom.Clear();
        foreach (var item in str2)
            PlayersInRoom.Add(item);
    }

    public void SetRoomCustomProperty(string key, string Value)
    {
        if ((string.IsNullOrEmpty(key)) || (string.IsNullOrEmpty(Value)))
            return;
        Hashtable props = new Hashtable { { key, Value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Debug.Log("SetRoomCustomProperty key=" + key
            + "  value=" + Value + " \n" + PhotonNetwork.CurrentRoom.ToStringFull());
        //Debug.Log();
    }


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void RaseEvent(byte Code)
    {
        //object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        //PhotonNetwork.RaiseEvent(Code, content, raiseEventOptions, SendOptions.SendReliable);
        PhotonNetwork.RaiseEvent(Code, null, raiseEventOptions, SendOptions.SendReliable);
        Debug.Log("RaseEvent " + Code.ToString());
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        //if (eventCode == MoveUnitsToTargetPositionEvent)
        //{
        //    object[] data = (object[])photonEvent.CustomData;
        //    Vector3 targetPosition = (Vector3)data[0];
        //    for (int index = 1; index < data.Length; ++index)
        //    {
        //        int unitId = (int)data[index];
        //        UnitList[unitId].TargetPosition = targetPosition;
        //    }
        //}

        if (eventCode == PhotonEvent_StartGame)
        {
            //updatePlayerInRoom();
            Debug.Log("OnEvent  PhotonEvent_StartGame");
            GetallplayerAroundTable();
            FillPlayersInRoom();
            //Uimanager.instance.GeneratePlayer();
            SetRoomCustomProperty("GameState", "Started");
            GameManager.currentGameMode = GameMode.MultiPlayer;
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(2);
        }
        if (eventCode == PhotonEvent_EndGame)
        {
            Debug.Log("OnEvent  PhotonEvent_EndGame");
        }
        if (eventCode == PhotonEvent_LeaveMatch)
        {
            Debug.Log("OnEvent  PhotonEvent_LeaveMatch");
            PhotonNetwork.LeaveRoom();
        }
        if (eventCode == PhotonEvent_DealCardCompleted)
        {
            Debug.Log("OnEvent  PhotonEvent_DealCardCompleted");
            ////if (players[0].PV == null)
            //foreach (var item in GameObject.FindObjectsOfType<PunPlayerRefs>())
            //{
            //    Debug.Log("OnRoomPropertiesUpdate Findplayer", item);
            //    item.Findplayer();

            //}
            ////if (PlayerSharedDatas[0].PhotonName == "")
            //{
            //    foreach (var item in GameObject.FindObjectsOfType<PunPlayerRefs>())
            //    {
            //        MultiPlayerGamePlayManager.instance.
            //                            PlayerSharedDatas[item.PhotonViewOf.id].PhotonName = item.pv.Owner.NickName;
            //        MultiPlayerGamePlayManager.instance.
            //        PlayerSharedDatas[item.PhotonViewOf.id].pv = item.pv;
            //    }
            //}
        }
        if (eventCode == PhotonEvent_SyncDeck)
        {
            Debug.Log("PhotonEvent_SyncDeck");
            MultiPlayerGamePlayManager.instance.SyncCards(
                Launcher.instance.GetRoomCustomProperty("cardsid"));
        }
    }

    public string ListToString(List<string> t)
    {
        string str = "";
        foreach (var item in t)
        {
            str += item + ",";
        }
        str = str.Remove(str.Length - 1);
        //Debug.Log("ListToString=" + str);
        return str;
    }
    public void StrToList(string str, out List<string> t)
    {
        t = new List<string>();
        string[] s1 = str.Split(',');
        foreach (var item in s1)
        {
            t.Add(item);
        }
    }

    //public static Launcher instance;
    //public int numOfPlayer = 0;
    //public Popup selectRoom, playerChoose, noNetwork;
    //public GameObject loadingView;

    //public InputField createRoomTF, joinRoomTF;
    //public bool isConnecting;

    //public bool ismaster;
    //public List<string> PlayersInRoom;

    //void Awake()
    //{
    //    instance = this;
    //    DontDestroyOnLoad(this.gameObject);
    //}

    //public void MulitplayerBtn()
    //{
    //    isConnecting = true;
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    //public override void OnConnectedToMaster()
    //{
    //    if(isConnecting)
    //    {
    //        PhotonNetwork.NickName = GameManager.PlayerAvatarName;
    //    }
    //    //PhotonNetwork.JoinLobby(TypedLobby.Default);
    //}

    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    isConnecting = false;
    //    StartCoroutine(CheckNetwork());
    //}

    //public override void OnJoinedLobby()
    //{
    //    //playerChoose.ShowPopup();
    //    selectRoom.ShowPopup();
    //}

    //public void OnPlayerSelect(ToggleGroup group)
    //{
    //    playerChoose.HidePopup(false);
    //    int i = 4;
    //    foreach (var t in group.ActiveToggles())
    //    {
    //        i = int.Parse(t.name);
    //        GameMaster.instance.multiPlayerNum = i;
    //    }
    //    //StartCoroutine(StartMultiPlayerGameMode(i));
    //    selectRoom.ShowPopup();
    //    GameManager.PlayButton();
    //}

    ////public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    ////{
    ////    Debug.LogError("OmMasterClientSwithed .... ");
    ////    ismaster = PhotonNetwork.IsMasterClient;
    ////    string nextTurnName = PhotonNetwork.CurrentRoom.CustomProperties["nextTurn"].ToString();
    ////    if (!PlayersInRoom.Contains(nextTurnName))
    ////    {
    ////        SetNextPlayer(0);
    ////    }
    ////}

    //public void updatePlayerInRoom()
    //{
    //    PlayersInRoom.Clear();
    //    for (int i = 0; i < 200; i++)
    //    {
    //        Photon.Realtime.Player TempPlayer;
    //        if (PhotonNetwork.CurrentRoom.Players.TryGetValue(i, out TempPlayer))
    //        {
    //            //Debug.LogError("TempPlayer.NickName = " + TempPlayer.NickName);
    //            PlayersInRoom.Add(TempPlayer.NickName);
    //        }
    //        if (PlayersInRoom.Count == PhotonNetwork.CurrentRoom.PlayerCount)
    //            break;
    //    }
    //}


    //public void OnClick_JoinRoom()
    //{
    //    if (joinRoomTF.text.Length >= 2)
    //    {
    //        PhotonNetwork.JoinRoom(joinRoomTF.text, null);
    //        selectRoom.HidePopup();
    //        loadingView.SetActive(true);
    //    }

    //}
    //public void OnClick_CreateRoom()
    //{
    //    if (createRoomTF.text.Length >= 2)
    //    {
    //        PhotonNetwork.CreateRoom(createRoomTF.text, new RoomOptions { MaxPlayers = 2/*(byte)GameMaster.instance.multiPlayerNum*/ }, null);
    //        selectRoom.HidePopup();
    //        loadingView.SetActive(true);
    //    }
    //}

    //public override void OnJoinedRoom()
    //{
    //    StartCoroutine(CheckNetwork());
    //    ismaster = PhotonNetwork.IsMasterClient;
    //    updatePlayerInRoom();
    //    if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
    //    {
    //        // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
    //        loadingView.SetActive(false);

    //        GameManager.currentGameMode = GameMode.MultiPlayer;            
    //        PhotonNetwork.LoadLevel(2);
    //    }
    //    //else
    //    //{
    //    //    Debug.Log("Waiting for another player");
    //    //}
    //}

    //public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    //{
    //    StartCoroutine(CheckNetwork());
    //    updatePlayerInRoom();
    //    if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
    //    {
    //        // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
    //        loadingView.SetActive(false);

    //        GameManager.currentGameMode = GameMode.MultiPlayer;            
    //        PhotonNetwork.LoadLevel(2);
    //    }
    //}

    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    print("RoomFailed " + returnCode + "    Message " + message);
    //    StartCoroutine(CheckNetwork());

    //}

    //public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    //{
    //    ismaster = PhotonNetwork.IsMasterClient;
    //}

    //IEnumerator StartMultiPlayerGameMode(int i)
    //{
    //    loadingView.SetActive(true);
    //    yield return new WaitForSeconds(Random.Range(3f, 10f));
    //    loadingView.SetActive(false);
    //    //SetTotalPlayer(i);
    //    //SetupGame();

    //    //bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
    //    //if (leftRoom)
    //    //{
    //    //    float inTime = Random.Range(7, 5 * 60);
    //    //    StartCoroutine(RemovePlayerFromRoom(inTime));
    //    //}

    //    //multiplayerLoaded = true;
    //}

    //public void CloseGame()
    //{
    //    StartCoroutine(DoSwitchScene());
    //}

    //IEnumerator DoSwitchScene()
    //{
    //    PhotonNetwork.Disconnect();
    //    while (PhotonNetwork.IsConnected)
    //        yield return null;
    //    SceneManager.LoadScene(0);
    //}

    //IEnumerator CheckNetwork()
    //{
    //    while (true)
    //    {
    //        WWW www = new WWW("https://www.google.com");
    //        yield return www;
    //        if (string.IsNullOrEmpty(www.error))
    //        {
    //            if (noNetwork.isOpen)
    //            {
    //                noNetwork.HidePopup();

    //                Time.timeScale = 1;
    //            }
    //        }
    //        else
    //        {
    //            if (Time.timeScale == 1)
    //            {
    //                noNetwork.ShowPopup();

    //                Time.timeScale = 0;
    //            }
    //        }

    //        yield return new WaitForSecondsRealtime(1f);
    //    }
    //}
}
