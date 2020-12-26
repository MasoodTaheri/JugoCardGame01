using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using System;

[System.Serializable]
public class PlayerDatas
{
    public string PhotonName;
    public string RealName;
    public PhotonView pv;
    public string CardsString;
}

public class MultiPlayerGamePlayManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks
{
    public GameScene gameScene;

    [Header("Sound")]
    public AudioClip music_win_clip;
    public AudioClip music_loss_clip;
    public AudioClip draw_card_clip;
    public AudioClip throw_card_clip;
    public AudioClip uno_btn_clip;
    public AudioClip choose_color_clip;

    [Header("Gameplay")]
    [Range(0, 100)]
    public int LeftRoomProbability = 10;
    [Range(0, 100)]
    public int UnoProbability = 85;
    [Range(0, 100)]
    public int LowercaseNameProbability = 30;

    public float cardDealTime = 0.05f;
    public Card _cardPrefab;
    public Transform cardDeckTransform;
    public Image cardWastePile;
    public GameObject arrowObject, unoBtn, cardDeckBtn;
    public Popup colorChoose, playerChoose, noNetwork;
    public GameObject loadingView, playerLeftView, rayCastBlocker;
    public Text leftPlayerName;
    public Animator cardEffectAnimator;
    public ParticleSystem wildCardParticle;
    public GameObject menuButton;

    [Header("Player Setting")]
    public PunPlayer[] players;
    public TextAsset multiplayerNames;
    public TextAsset computerProfiles;
    public bool clockwiseTurn = true;
    public int currentPlayerIndex = 0;
    public int NumOfCardsToPlay;
    public PunPlayer CurrentPlayer { get { return players[currentPlayerIndex]; } }

    [Header("Game Over")]
    public GameObject gameOverPopup;
    public ParticleSystem starParticle;
    public List<GameObject> playerObject;
    public List<Text> playerPoints;
    public GameObject loseTimerAnimation;

    public List<Card> cards;
    public List<Card> wasteCards;

    public bool isOdd, isEven;
    public int turnCount = 0, roundsCount = 0;

    public List<PlayerDatas> PlayerSharedDatas;
    public List<GameObject> PlayerBg;
    public List<GameObject> Avatars;

    public CardType CurrentType
    {
        get { return _currentType; }
        set { _currentType = value; cardWastePile.color = value.GetColor(); }
    }

    public CardValue CurrentValue
    {
        get { return _currentValue; }
        set { _currentValue = value; }
    }

    [SerializeField] CardType _currentType;
    [SerializeField] CardValue _currentValue;

    public bool IsDeckArrow
    {
        get { return arrowObject.activeSelf; }
    }
    public static MultiPlayerGamePlayManager instance;

    System.DateTime pauseTime;
    int fastForwardTime = 0;
    bool setup = false, multiplayerLoaded = false, gameOver = false;

    void Start()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        players = new PunPlayer[PhotonNetwork.CurrentRoom.PlayerCount];
        GameObject go = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

        int i = PhotonNetwork.CurrentRoom.PlayerCount;
        PlayerSharedDatas = new List<PlayerDatas>();
        for (int j = 0; j < i; j++)
        {
            PlayerDatas pd = new PlayerDatas();
            PlayerSharedDatas.Add(pd);
        }

        StartCoroutine(StartMultiPlayerGameMode(i));
    }

    int leftPlayerCount = 0;
    public void PlayerLeft(string playerName)
    {
        if(!gameOver && leftPlayerCount <= 0)
        {
            leftPlayerCount++;
            playerLeftView.SetActive(true);
            leftPlayerName.text = "Player Left:  " + playerName;
            Invoke("LoadHomeScene", 2.5f);
        }        
    }

    void LoadHomeScene()
    {
        playerLeftView.SetActive(false);
        gameScene.CloseGame();
    }

    IEnumerator StartMultiPlayerGameMode(int i)
    {
        loadingView.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 10f));
        loadingView.SetActive(false);
        SetTotalPlayer(i);
        SetupGame();

        ////bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
        ////if (leftRoom)
        ////{
        ////    float inTime = Random.Range(7, 5 * 60);
        ////    StartCoroutine(RemovePlayerFromRoom(inTime));
        ////}

        multiplayerLoaded = true;
    }

    public void EnableCardDeck()
    {
        cardDeckBtn.GetComponent<Button>().interactable = true;
    }
    public void DisableCardDeck()
    {
        cardDeckBtn.GetComponent<Button>().interactable = false;
    }

    void SetTotalPlayer(int totalPlayer = 4)
    {
        cardDeckBtn.SetActive(true);
        DisableCardDeck();
        cardWastePile.gameObject.SetActive(true);
        unoBtn.SetActive(true);
        /*  if (totalPlayer == 2)
          {
              players[0].gameObject.SetActive(true);
              players[0].CardPanelBG.SetActive(true);
              players[2].gameObject.SetActive(true);
              players[2].CardPanelBG.SetActive(true);
              players.RemoveAt(3);
              players.RemoveAt(1);
          }
          else if (totalPlayer == 3)
          {
              for (int i = 0; i < 3; i++)
              {
                  players[i].gameObject.SetActive(true);
                  players[i].CardPanelBG.SetActive(true);
              }
              players[3].CardPanelBG.SetActive(true);
              players.RemoveAt(3);
          }
          else
          {
              for (int i = 0; i < 4; i++)
              {
                  players[i].gameObject.SetActive(true);
                  players[i].CardPanelBG.SetActive(true);
              }
          }*/
    }

    void SetupGame()
    {
        menuButton.SetActive(true);
        currentPlayerIndex = 0;// Random.Range(0, players.Length);
        //players[0].SetAvatarProfile(GameManager.PlayerAvatarProfile);

        //if (GameManager.currentGameMode == GameMode.MultiPlayer)
        //{
        //string[] nameList = multiplayerNames.text.Split('\n');
        //List<int> indexes = new List<int>();

        //for (int i = 1; i < players.Length; i++)
        //{
        //    while (true)
        //    {
        //        int index = Random.Range(0, nameList.Length);
        //        var name = nameList[index].Trim();
        //        if (name.Length == 0) continue;

        //        if (!indexes.Contains(index))
        //        {
        //            indexes.Add(index);
        //            if (Random.value < LowercaseNameProbability / 100f) name = name.ToLower();
        //            players[i].SetAvatarProfile(new AvatarProfile { avatarIndex = index % GameManager.TOTAL_AVATAR, avatarName = name });
        //            break;
        //        }
        //    }
        //}
        //}
        //else
        //{
        //    var profiles = JsonUtility.FromJson<AvatarProfiles>(computerProfiles.text).profiles;
        //    for (int i = 0; i < profiles.Count; i++)
        //    {
        //        players[i + 1].SetAvatarProfile(profiles[i]);
        //    }
        //}
        CreateDeck();
        StartCoroutine(Setup2());

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    cards.Shuffle();
        //    StartCoroutine(DealCards(NumOfCardsToPlay));
        //}
    }

    IEnumerator Setup2()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            generateAndShuffleCards();
            Launcher.instance.SetRoomCustomProperty(
             "cardsid", Launcher.instance.ListToString(cardsId));
        }
        yield return new WaitForSeconds(1.5f);
        if (PhotonNetwork.IsMasterClient)
            Launcher.instance.RaseEvent(Launcher.PhotonEvent_SyncDeck);// call SyncCards on all clients

        yield return new WaitForSeconds(1.5f);
        CurrentPlayer.OnTurn();
    }

    public List<string> cardsId;
    public void generateAndShuffleCards()
    {
        //= new List<string>();
        for (int i = 0; i < 55; i++)
            cardsId.Add(i.ToString());
        cardsId.Shuffle();
    }

    public void SyncCards(string cardsList)//called in event PhotonEvent_SyncDeck
    {
        Debug.Log("SyncCards " + cardsList);
        StartCoroutine(SyncCardsIE(cardsList));
    }

    IEnumerator SyncCardsIE(string cardsList)
    {
        while (cards.Count < 50)
            yield return new WaitForSeconds(0.5f);

        List<Card> oldcards = new List<Card>();
        foreach (var item in cards)
            oldcards.Add(item);

        string[] cardshaveShuffled = cardsList.Split(',');
        Card temp1;
        for (int i = 0; i < cardshaveShuffled.Length; i++)
        {
            int newid = Convert.ToInt32(cardshaveShuffled[i]);
            //Debug.Log(newid + " copied to" + i);
            if ((newid < 0) || (newid > oldcards.Count))
                Debug.LogError("newid=" + newid + "  cardshaveShuffled[i]=" + cardshaveShuffled[i] + "  oldcards.Count=" + oldcards.Count);

            temp1 = oldcards[newid];
            cards[i] = temp1;

        }
        StartCoroutine(DealCards(NumOfCardsToPlay));
    }

    void CreateDeck()
    {
        cards = new List<Card>();
        wasteCards = new List<Card>();
        int k = 1;
        for (int j = 1; j <= 4; j++)
        {
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Wild, k++));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.DrawTwo, k++));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Ego, k++));
            if (j <= 2)
            {
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Odd, k++));
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Even, k++));
            }
        }
        for (int i = 0; i <= 9; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i, k++));
                // cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
            }
        }
    }

    Card CreateCardOnDeck(CardType t, CardValue v, int id)
    {
        Card temp = Instantiate(_cardPrefab, cardDeckTransform.position, Quaternion.identity, cardDeckTransform);
        temp.Type = t;
        temp.Value = v;
        temp.IsOpen = false;
        temp.CalcPoint();
        temp.GetCardValue();
        temp.name = id.ToString() + "_" + t.ToString() + "_" + v.ToString();
        return temp;
    }

    IEnumerator DealCards(int total)
    {
        yield return new WaitForSeconds(1f);
        for (int t = 0; t < total; t++)
        {
            for (int i = 0; i < players.Length; i++)
            {
                //Debug.Log("DealCards to player" + i);
                //PickCardFromDeck(players[i]);
                PickCardFromDeckInit(players[i]);
                yield return new WaitForSeconds(cardDealTime);
            }
        }

        yield return new WaitForSeconds(1.5f);
        int a = 0;
        while (cards[a].Type == CardType.Other)
        {
            a++;
        }
        /* Commented for not to put card from Deck first ....  */
        //PutCardToWastePile(cards[a]);
        //cards.RemoveAt(a);

        //for (int i = 0; i < players.Length; i++)
        //{
        //    players[i].cardsPanel.UpdatePos();
        //    yield return new WaitForSeconds(1.5f);
        //}

        setup = true;
        Debug.Log("Dealing cards is completed");

        for (int i = 0; i < PlayerSharedDatas.Count; i++)
        {
            Launcher.instance.SetRoomCustomProperty(
                Launcher.instance.PlayersInRoom[i],
                PlayerSharedDatas[i].CardsString);
        }
        Launcher.instance.RaseEvent(Launcher.PhotonEvent_DealCardCompleted);

        StartCoroutine(animatecards(total));
        //CurrentPlayer.OnTurn();
    }

    public Card PickCardFromDeckInit(PunPlayer p)
    {
        Card temp = cards[0];
        //Debug.Log(p.id + " " + temp.name);
        PlayerSharedDatas[p.Actornumber - 1].CardsString += temp.name + ",";
        p.Cards.Add(cards[0]);

        p.AddCard(cards[0]);
        cards[0].IsOpen = p.isUserPlayer;
        cards.RemoveAt(0);
        return temp;
    }
    IEnumerator animatecards(int total)
    {
        for (int i = 0; i < players.Length; i++)
        {
            PunPlayer p = players[i];
            for (int j = 0; j < total; j++)
            {
                p.Cards[j].SetTargetPosAndRot(Vector3.zero, 0f);
                yield return new WaitForSeconds(cardDealTime);
            }

            GameManager.PlaySound(throw_card_clip);
            p.GetTotalPoints();

        }
        yield return new WaitForSeconds(1);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].cardsPanel.UpdatePos();
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator DealCardsToPlayer(PunPlayer p, int NoOfCard = 1, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        for (int t = 0; t < NoOfCard; t++)
        {
            PickCardFromDeck(p, true);
            yield return new WaitForSeconds(cardDealTime);
        }
        //p.GetListOfValues();
        CurrentPlayer.OnTurn();
        //if(CurrentPlayer.isUserPlayer)
        //{
        //    p.UpdateCardColor();
        //}
        //p.GetDoubleCard();
        //p.UpdateCardColor();
    }

    public Card PickCardFromDeck(PunPlayer p, bool updatePos = false)
    {
        if (cards.Count == 0)
        {
            print("Card Over");
            while (wasteCards.Count > 5)
            {
                cards.Add(wasteCards[0]);
                wasteCards[0].transform.SetParent(cardDeckTransform);
                wasteCards[0].transform.localPosition = Vector3.zero;
                wasteCards[0].transform.localRotation = Quaternion.Euler(Vector3.zero);
                wasteCards[0].IsOpen = false;
                wasteCards.RemoveAt(0);
            }
        }
        Card temp = cards[0];

        Debug.Log(p.playerName + "   " + temp.name);
        PlayerSharedDatas[p.Actornumber - 1].CardsString += temp.name + ",";
        p.Cards.Add(cards[0]);

        p.AddCard(cards[0]);
        cards[0].IsOpen = p.isUserPlayer;
        if (updatePos)
            p.cardsPanel.UpdatePos();
        else
            cards[0].SetTargetPosAndRot(Vector3.zero, 0f);
        cards.RemoveAt(0);
        GameManager.PlaySound(throw_card_clip);
        p.GetTotalPoints(); 
        return temp;
    }
    
    public void PutCardToWastePile(Card c, PunPlayer p = null)
    {
        Debug.Log("PutCardToWastePile");
        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
        {
            //CurrentPlayer.isUserClicked = true;
            NextPlayerTurn();
            return;
        }
        if (c == null)
        {
            NextPlayerTurn();
            return;
        }

        if (p != null)
        {
            p.RemoveCard(c);
            //if (p.cardsPanel.cards.Count == 1 && !p.unoClicked)
            //{
            //    ApplyUnoCharge(CurrentPlayer);
            //}
            GameManager.PlaySound(draw_card_clip);
        }

        CurrentType = c.Type;
        CurrentValue = c.Value;
        wasteCards.Add(c);
        c.IsOpen = true;
        c.transform.SetParent(cardWastePile.transform, true);
        c.SetTargetPosAndRot(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 1), c.transform.localRotation.eulerAngles.z + Random.Range(-15f, 15f));

        //isOdd = false;
        //isEven = false;
        if (p != null)
        {
            //if (p.GetTotalPoints() <= 10 && !CurrentPlayer.isUserPlayer)   //(p.cardsPanel.cards.Count == 0)
            //{
            //    Debug.LogError("GetTotalPoints() = "+ p.GetTotalPoints());
            //    Invoke("SetupGameOver", 2f);
            //    return;
            //}
            //Debug.LogError("ccccccccc      CardType = " + c.Type);
            if (c.Type == CardType.Other || (c.Value == CardValue.Wild && CurrentPlayer.isSequentialCards))
            {
                //CurrentPlayer.Timer = true;
                //CurrentPlayer.choosingColor = true;
                //if (CurrentPlayer.isUserPlayer)
                //{
                //    //colorChoose.ShowPopup();
                //    ChooseColorforAI();
                //    Debug.LogError("CurrentPlayer.isUserPlayer = " + CurrentPlayer.isUserPlayer);
                //}
                //else
                //{
                //    Invoke("ChooseColorforAI", Random.Range(3f, 9f));
                //    Debug.LogError("ChooseColorforAI . .. . ");
                //}

                if (c.Value == CardValue.DrawTwo)
                {
                    //NextPlayerIndex();
                    //CurrentPlayer.ShowMessage("+2");
                    //wildCardParticle.Emit(30);
                    //StartCoroutine(DealCardsToPlayer(CurrentPlayer, 2, .5f));
                    //Invoke("NextPlayerTurn", 1.5f);

                    Invoke("NextPlayerTurn", 2f);
                    Invoke("DrawTwoCard", 2.5f);
                }
                //else if (c.Value == CardValue.Odd)
                //{
                //    isOdd = true;
                //    Invoke("NextPlayerTurn", 2f);
                //}
                //else if (c.Value == CardValue.Even)
                //{
                //    isEven = true;
                //    Invoke("NextPlayerTurn", 2f);
                //}
                else
                {
                    if (c.Value == CardValue.Odd)
                    {
                        isOdd = true;
                    }
                    else if (c.Value == CardValue.Even)
                    {
                        isEven = true;
                    }

                    if (!CurrentPlayer.isSequentialCards)
                    {
                        Invoke("NextPlayerTurn", 2f);
                    }
                }
            }
            else
            {
                //if (c.Value == CardValue.Reverse)
                //{
                //    clockwiseTurn = !clockwiseTurn;
                //    cardEffectAnimator.Play(clockwiseTurn ? "ClockWiseAnim" : "AntiClockWiseAnim");
                //    Invoke("NextPlayerTurn", 1.5f);
                //}
                //else if (c.Value == CardValue.Skip)
                //{
                //    NextPlayerIndex();
                //    CurrentPlayer.ShowMessage("Turn Skipped!");
                //    Invoke("NextPlayerTurn", 1.5f);
                //}
                //if (c.Value == CardValue.DrawTwo)
                //{
                //    NextPlayerIndex();
                //    CurrentPlayer.ShowMessage("+2");
                //    wildCardParticle.Emit(30);
                //    StartCoroutine(DealCardsToPlayer(CurrentPlayer, 2, .5f));
                //    Invoke("NextPlayerTurn", 1.5f);
                //}
                //else
                //{
                //NextPlayerTurn();

                //if(CurrentPlayer.isUserPlayer)
                //{
                //    Invoke("NextPlayerTurn", 2f);
                //}
                //else
                //{
                //Debug.LogError("CurrentPlayer.isDoubleCards = " + CurrentPlayer.isDoubleCards + "  &  = CurrentPlayer.isSequentialCards = " + CurrentPlayer.isSequentialCards);
                //Debug.LogError("CurrentPlayer.isOddEvenDoubles = " + CurrentPlayer.isOddEvenDoubles);
                if (CurrentPlayer.isDoubleCards)
                {
                    return;
                }
                else if (CurrentPlayer.isSequentialCards)
                {
                    return;
                }
                else if (CurrentPlayer.isOddEvenDoubles)
                {
                    return;
                }
                else    //if (!CurrentPlayer.isDoubleCards && !CurrentPlayer.isSequentialCards)
                {
                    //Debug.LogError("CurrentPlayer.isDoubleCards = "+ CurrentPlayer.isDoubleCards + "  &  = CurrentPlayer.isSequentialCards" + CurrentPlayer.isSequentialCards);
                    Invoke("NextPlayerTurn", 2f);
                }
                //}

                //}
            }
        }
    }

    public void DrawTwoCard()
    {
        CurrentPlayer.ShowMessage("+2");
        wildCardParticle.Emit(30);
        StartCoroutine(DealCardsToPlayer(CurrentPlayer, 2, .5f));
    }

    private IEnumerator RemovePlayerFromRoom(float time)
    {
        yield return new WaitForSeconds(time);

        if (gameOver) yield break;

        List<int> indexes = new List<int>();
        for (int i = 1; i < players.Length; i++)
        {
            indexes.Add(i);
        }
        indexes.Shuffle();

        int index = -1;
        foreach (var i in indexes)
        {
            if (!players[i].Timer)
            {
                index = i;
                break;
            }
        }

        var player = players[index];
        player.isInRoom = false;

        Toast.instance.ShowMessage(player.playerName + " left the room", 2.5f);

        yield return new WaitForSeconds(2f);

        player.gameObject.SetActive(false);
        foreach (var item in player.cardsPanel.cards)
        {
            item.gameObject.SetActive(false);
        }
    }

    void ChooseColorforAI()
    {
        CurrentPlayer.ChooseBestColor();
    }

    public void NextPlayerIndex()
    {
        int step = clockwiseTurn ? 1 : -1;
        do
        {
            currentPlayerIndex = Mod(currentPlayerIndex + step, players.Length);
        } while (!players[currentPlayerIndex].isInRoom);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void NextPlayerTurn()
    {
        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
        {
            CurrentPlayer.isUserClicked = true;
        }
        else
        {
            OnDeckClick();
        }
        NextPlayerIndex();
        CurrentPlayer.OnTurn();
    }

    public void OnColorSelect(int i)
    {
        if (!colorChoose.isOpen) return;
        colorChoose.HidePopup();

        SelectColor(i);
    }

    public void SelectColor(int i)
    {
        CurrentPlayer.Timer = false;
        CurrentPlayer.choosingColor = false;
        NextPlayerTurn();
        //CurrentType = (CardType)i;
        //cardEffectAnimator.Play("DrawFourAnim");
        //if (CurrentValue == CardValue.Wild)
        //{
        //    wildCardParticle.gameObject.SetActive(true);
        //    wildCardParticle.Emit(30);
        //    Invoke("NextPlayerTurn", 1.5f);
        //    GameManager.PlaySound(choose_color_clip);
        //}
        //else
        //{
        //    NextPlayerIndex();
        //    CurrentPlayer.ShowMessage("+4");
        //    StartCoroutine(DealCardsToPlayer(CurrentPlayer, 4, .5f));
        //    Invoke("NextPlayerTurn", 2f);
        //    GameManager.PlaySound(choose_color_clip);
        //}
    }

    public void EnableDeckClick()
    {
        arrowObject.SetActive(true);
    }
    public int oddEvenCount = 0;

    public void OnDeckClick()
    {
        photonView.RPC("RPC_OnDeckClick", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_OnDeckClick()
    {
        if (!setup) return;

        if (arrowObject.activeInHierarchy)
        {
            arrowObject.SetActive(false);
            CurrentPlayer.pickFromDeck = true;
            PickCardFromDeck(CurrentPlayer, true);
            if (CurrentPlayer.cardsPanel.AllowedCard.Count == 0 || (!CurrentPlayer.Timer && CurrentPlayer.isUserPlayer))
            {
                CurrentPlayer.OnTurnEnd();
                NextPlayerTurn();
            }
            else
            {
                CurrentPlayer.UpdateCardColor();
            }
        }
        else if (!CurrentPlayer.pickFromDeck)// && CurrentPlayer.isUserPlayer)  //commented to pickup card from deck for every player...... 
        {
            PickCardFromDeck(CurrentPlayer, true);
            CurrentPlayer.pickFromDeck = true;
            // CurrentPlayer.UpdateCardColor();
        }
    }

    public void ResetPlayerCards()
    {
        if (CurrentPlayer.isUserPlayer)
        {
            oddEvenCount++;
            turnCount--;
            CurrentPlayer.OnTurn();
            arrowObject.SetActive(false);
            DisableCardDeck();
        }
    }

    public void EnableUnoBtn()
    {
        //Debug.LogError("EnableUnoBtn() .... .");
        unoBtn.GetComponent<Button>().interactable = true;
        unoBtn.GetComponent<Image>().color = Color.white;
    }

    public void DisableUnoBtn()
    {
        //Debug.LogError("DisableUnoBtn() .... .");
        unoBtn.GetComponent<Button>().interactable = false;
        unoBtn.GetComponent<Image>().color = Color.gray;
    }

    public void OnUnoClick()
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Jugo!", true);
        CurrentPlayer.unoClicked = true;
        //Invoke("SetupGameOver", 2f);
        photonView.RPC("GameOver", RpcTarget.All);
        GameManager.PlaySound(uno_btn_clip);
    }

    [PunRPC]
    void GameOver()
    {
        Invoke("SetupGameOver", 2f);
    }

    public void ApplyUnoCharge(PunPlayer p)
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Jugo Charges");
        StartCoroutine(DealCardsToPlayer(p, 2, .3f));
    }

    public List<PunPlayer> playerScores;
    public void SetupGameOver()
    {
        gameOver = true;
        for (int i = 0; i < players.Length; i++)
        {
            playerScores.Add(players[i]);
        }

        //for (int i = players.Length - 1; i >= 0; i--)
        //{
        //    if (!players[i].isInRoom)
        //    {
        //        players.RemoveAt(i);
        //    }
        //}

        if (players.Length == 2)
        {
            playerObject[0].SetActive(true);
            playerObject[2].SetActive(true);
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "2nd Place";
            playerObject.RemoveAt(3);
            playerObject.RemoveAt(1);

        }
        else if (players.Length == 3)
        {
            playerObject.RemoveAt(2);
            for (int i = 0; i < 3; i++)
            {
                playerObject[i].SetActive(true);
            }
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "3rd Place";

        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                playerObject[i].SetActive(true);
            }
        }

        //players.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
        playerScores.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
        var winner = playerScores[0];

        starParticle.gameObject.SetActive(winner.isUserPlayer);
        playerObject[0].GetComponentsInChildren<Image>()[1].sprite = winner.avatarImage.sprite;

        for (int i = 0; i < playerObject.Count; i++)
        {
            var playerNameText = playerObject[i].GetComponentInChildren<Text>();
            playerNameText.text = playerScores[i].playerName;
            playerNameText.GetComponent<EllipsisText>().UpdateText();
            playerObject[i].GetComponentsInChildren<Image>()[1].sprite = playerScores[i].avatarImage.sprite;
        }

        for (int i = 0; i < players.Length; i++)
        {
            playerPoints[i].text = playerScores[i].GetTotalPoints().ToString();
        }

        GameManager.PlaySound(winner.isUserPlayer ? music_win_clip : music_loss_clip);
        gameOverPopup.SetActive(true);

        for (int i = 1; i < playerScores.Count; i++)
        {
            if (playerScores[i].isUserPlayer)
            {
                loseTimerAnimation.SetActive(true);
                loseTimerAnimation.transform.position = playerObject[i].transform.position;
                break;
            }
        }

        gameOverPopup.GetComponent<Animator>().enabled = winner.isUserPlayer;
        gameOverPopup.GetComponentInChildren<Text>().text = winner.isUserPlayer ? "You win Game." : "You Lost Game ...   Try Again.";
        fastForwardTime = 0;
        Time.timeScale = 0;
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
                    OnApplicationPause(false);
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    noNetwork.ShowPopup();

                    Time.timeScale = 0;
                    pauseTime = System.DateTime.Now;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            pauseTime = System.DateTime.Now;
        }
        else
        {
            if (GameManager.currentGameMode == GameMode.MultiPlayer && multiplayerLoaded && !gameOver)
            {
                fastForwardTime += Mathf.Clamp((int)(System.DateTime.Now - pauseTime).TotalSeconds, 0, 3600);
                if (Time.timeScale == 1f)
                {
                    StartCoroutine(DoFastForward());
                }
            }
        }
    }

    IEnumerator DoFastForward()
    {
        Time.timeScale = 10f;
        rayCastBlocker.SetActive(true);
        while (fastForwardTime > 0)
        {
            yield return new WaitForSeconds(1f);
            fastForwardTime--;
        }
        Time.timeScale = 1f;
        rayCastBlocker.SetActive(false);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //Debug.Log(Launcher.instance.GetRoomCustomProperty("cardsid"));
        //for (int i = 0; i < PlayerSharedDatas.Count; i++)
        //{
        //    string str1 = Launcher.instance.GetRoomCustomProperty(PlayerSharedDatas[i].PhotonName);

        //    if ((str1 != null) && (PlayerSharedDatas[i].PhotonName != ""))
        //    {
        //        Debug.Log(PlayerSharedDatas[i].PhotonName + "=" + str1);
        //        //PlayerSharedDatas[i].CardsString = str1;
        //    }
        //}
        //propertiesThatChanged.ContainsKey(propertiesKey)
    }
}
