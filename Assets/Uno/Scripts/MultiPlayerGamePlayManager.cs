using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    public GameObject loadingView, rayCastBlocker;
    public Animator cardEffectAnimator;
    public ParticleSystem wildCardParticle;
    public GameObject menuButton;

    [Header("Player Setting")]
    public List<PunPlayer> players;
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

    private List<Card> cards;
    private List<Card> wasteCards;

    public bool isOdd, isEven;

    public int turnCount = 0, roundsCount = 0;








    public List<PlayerDatas> PlayerSharedDatas;









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
        //if (GameManager.currentGameMode == GameMode.Computer)
        //{
        //    SetTotalPlayer(4);
        //    SetupGame();
        //}
        //else
        //{
        //StartCoroutine(CheckNetwork());
        //playerChoose.ShowPopup();
        //}
        //playerChoose.HidePopup(false);
        int i = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("PlayerCount=" + i);
        PlayerSharedDatas = new List<PlayerDatas>();
        for (int j = 0; j < i; j++)
        {
            PlayerDatas pd = new PlayerDatas();
            PlayerSharedDatas.Add(pd);

        }
        StartCoroutine(StartMultiPlayerGameMode(i));
    }

    //public void OnPlayerSelect(ToggleGroup group)
    //{
    //    playerChoose.HidePopup(false);
    //    int i = 4;
    //    foreach (var t in group.ActiveToggles())
    //    {
    //        i = int.Parse(t.name);
    //    }
    //    StartCoroutine(StartMultiPlayerGameMode(i));
    //    GameManager.PlayButton();
    //}

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
        if (totalPlayer == 2)
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
        }
    }

    void SetupGame()
    {
        menuButton.SetActive(true);
        currentPlayerIndex = Random.Range(0, players.Count);
        players[0].SetAvatarProfile(GameManager.PlayerAvatarProfile);

        //if (GameManager.currentGameMode == GameMode.MultiPlayer)
        //{
        string[] nameList = multiplayerNames.text.Split('\n');
        List<int> indexes = new List<int>();

        for (int i = 1; i < players.Count; i++)
        {
            while (true)
            {
                int index = Random.Range(0, nameList.Length);
                var name = nameList[index].Trim();
                if (name.Length == 0) continue;

                if (!indexes.Contains(index))
                {
                    indexes.Add(index);
                    if (Random.value < LowercaseNameProbability / 100f) name = name.ToLower();
                    players[i].SetAvatarProfile(new AvatarProfile { avatarIndex = index % GameManager.TOTAL_AVATAR, avatarName = name });
                    break;
                }
            }
        }
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
        GetallplayerAroundTable
        GameObject Go = PhotonNetwork.Instantiate("PvController", Vector3.zero, Quaternion.identity);
        //Debug.Log(PhotonNetwork.)
            //Go.GetComponent<PunPlayerRefs>().
        if (!PhotonNetwork.IsMasterClient)
            return;


     
        cards.Shuffle();
        StartCoroutine(DealCards(NumOfCardsToPlay));
    }

    void CreateDeck()
    {
        cards = new List<Card>();
        wasteCards = new List<Card>();
        for (int j = 1; j <= 4; j++)
        {
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Wild));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.DrawTwo));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Ego));
            if (j <= 2)
            {
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Odd));
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Even));
            }
        }
        for (int i = 0; i <= 9; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
                // cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
            }
        }
    }

    Card CreateCardOnDeck(CardType t, CardValue v)
    {
        Card temp = Instantiate(_cardPrefab, cardDeckTransform.position, Quaternion.identity, cardDeckTransform);
        temp.Type = t;
        temp.Value = v;
        temp.IsOpen = false;
        temp.CalcPoint();
        temp.GetCardValue();
        temp.name = t.ToString() + "_" + v.ToString();
        return temp;
    }

    IEnumerator DealCards(int total)
    {
        yield return new WaitForSeconds(1f);
        for (int t = 0; t < total; t++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PickCardFromDeck(players[i]);
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

        for (int i = 0; i < players.Count; i++)
        {
            players[i].cardsPanel.UpdatePos();
        }

        setup = true;
        //CurrentPlayer.OnTurn();
        Debug.Log("CurrentPlayer.OnTurn();");
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
        //PlayerSharedDatas
        p.AddCard(cards[0]);
        cards[0].IsOpen = p.isUserPlayer;
        if (updatePos)
            p.cardsPanel.UpdatePos();
        else
            cards[0].SetTargetPosAndRot(Vector3.zero, 0f);
        cards.RemoveAt(0);
        GameManager.PlaySound(throw_card_clip);
        p.GetTotalPoints(); // add points to text .... .. . 
                            //p.GetListOfValues();
                            //p.GetDoubleCard();
                            // CurrentPlayer.UpdateCardColor();
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
        for (int i = 1; i < players.Count; i++)
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
            currentPlayerIndex = Mod(currentPlayerIndex + step, players.Count);
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
        Invoke("SetupGameOver", 2f);
        GameManager.PlaySound(uno_btn_clip);
    }

    public void ApplyUnoCharge(PunPlayer p)
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Jugo Charges");
        StartCoroutine(DealCardsToPlayer(p, 2, .3f));
    }

    public void SetupGameOver()
    {
        gameOver = true;
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (!players[i].isInRoom)
            {
                players.RemoveAt(i);
            }
        }

        if (players.Count == 2)
        {
            playerObject[0].SetActive(true);
            playerObject[2].SetActive(true);
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "2nd Place";
            playerObject.RemoveAt(3);
            playerObject.RemoveAt(1);

        }
        else if (players.Count == 3)
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
            for (int i = 0; i < 4; i++)
            {
                playerObject[i].SetActive(true);
            }
        }

        players.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
        var winner = players[0];

        starParticle.gameObject.SetActive(winner.isUserPlayer);
        playerObject[0].GetComponentsInChildren<Image>()[1].sprite = winner.avatarImage.sprite;

        for (int i = 0; i < playerObject.Count; i++)
        {
            var playerNameText = playerObject[i].GetComponentInChildren<Text>();
            playerNameText.text = players[i].playerName;
            playerNameText.GetComponent<EllipsisText>().UpdateText();
            playerObject[i].GetComponentsInChildren<Image>()[1].sprite = players[i].avatarImage.sprite;
        }

        for (int i = 0; i < playerPoints.Count; i++)
        {
            playerPoints[i].text = players[i].GetTotalPoints().ToString();
        }

        GameManager.PlaySound(winner.isUserPlayer ? music_win_clip : music_loss_clip);
        gameOverPopup.SetActive(true);

        for (int i = 1; i < players.Count; i++)
        {
            if (players[i].isUserPlayer)
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
}


/*      5th Dec BackUp
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MultiPlayerGamePlayManager : MonoBehaviourPun, IPunTurnManagerCallbacks
{
    public PhotonView photonView;

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
    public GameObject loadingView, rayCastBlocker;
    public Animator cardEffectAnimator;
    public ParticleSystem wildCardParticle;
    public GameObject menuButton;

    [Header("Player Setting")]
    public GameObject playerPos;
    public GameObject playerPfb;
    public List<PunPlayer> players;
    public List<GameObject> playersPos;
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

    private List<Card> cards;
    private List<Card> wasteCards;

    public bool isOdd, isEven;

    public int turnCount = 0, roundsCount = 0;

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

    private PunTurnManager turnManager;
    public float turnDuration = 15;
    // keep track of when we show the results to handle game logic.
    private bool IsShowingResults;

    public int playerCounts = 0;
    private int[] numbers;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += onPhotonEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= onPhotonEvent;
    }

    //private void OnEvent(EventData photonEvent)
    //{
    //    byte eventCode = photonEvent.Code;
    //    if (eventCode == MoveUnitsToTargetPositionEvent)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;
    //        Vector3 targetPosition = (Vector3)data[0];
    //        for (int index = 1; index < data.Length; ++index)
    //        {
    //            int unitId = (int)data[index];
    //            UnitList[unitId].TargetPosition = targetPosition;
    //        }
    //    }
    //}

    //void OnEnable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived += onPhotonEvent;
    //}

    //void OnDisable()
    //{
    //    PhotonNetwork.NetworkingClient.EventReceived -= onPhotonEvent;
    //}
    //private void onPhotonEvent(byte eventCode, object content, int senderId)
    //{
    //    object[] datas = content as object[];
    //    List<Card> cards2 = new List<Card>();
    //    if (datas.Length == 56)
    //    {
    //        for (int i = 0; i < datas.Length; i++)
    //        {
    //            //Debug.LogError("cards[(int)datas[i]] = " + cards[(int)datas[i]]);
    //            cards2.Add(cards[(int)datas[i]]);
    //        }
    //        cards = cards2;
    //    }
    //}

    private void onPhotonEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 0)
        {
            object[] datas = photonEvent.CustomData as object[];
            List<Card> cards2 = new List<Card>();
            if (datas.Length == 56)
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    //Debug.LogError("cards[(int)datas[i]] = " + cards[(int)datas[i]]);
                    cards2.Add(cards[(int)datas[i]]);
                }
                cards = cards2;
            }
        }
    }

    void Start()
    {
        instance = this;

        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
        this.turnManager.TurnManagerListener = this;
        this.turnManager.TurnDuration = turnDuration;

        Input.multiTouchEnabled = false;
        if (GameManager.currentGameMode == GameMode.Computer)
        {
            SetTotalPlayer(4);
            SetupGame();
        }
        else
        {
            //StartCoroutine(CheckNetwork());
            //playerChoose.ShowPopup();
            StartCoroutine(StartMultiPlayerGameMode(2));
        }

        numbers = new int[56];
        for (int i = 0; i < numbers.Length; i++) numbers[i] = i;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RandomNums();
            }
        }

        if (!PhotonNetwork.InRoom)
        {
            return;
        }
    }

    void RandomNums()
    {
        object[] datas = new object[numbers.Length];
        for (int i = 0; i < numbers.Length; i++)
        {
            int index = Random.Range(0, numbers.Length);
            int temp = numbers[i];
            numbers[i] = numbers[index];
            numbers[index] = temp;
        }

        for (int i = 0; i < numbers.Length; i++)
        {
            datas[i] = numbers[i];
        }

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent(0, datas, options, SendOptions.SendReliable);
    }

    public void OnPlayerSelect(ToggleGroup group)
    {
        playerChoose.HidePopup(false);
        int i = 4;
        foreach (var t in group.ActiveToggles())
        {
            i = int.Parse(t.name);
        }
        StartCoroutine(StartMultiPlayerGameMode(i));
        GameManager.PlayButton();
    }

    IEnumerator StartMultiPlayerGameMode(int i)
    {
        loadingView.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 4f));
        loadingView.SetActive(false);
        SetTotalPlayer(i);
        SetupGame();
        AddAllActivePlayers();

        bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
        if (leftRoom)
        {
            float inTime = Random.Range(7, 5 * 60);
            StartCoroutine(RemovePlayerFromRoom(inTime));
        }

        multiplayerLoaded = true;
    }

    public void AddAllActivePlayers()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            //Debug.LogError("player.NickName = "+ player.NickName);
            GameMaster.instance.playerNames.Add(player.NickName);
        }
    }

    public void EnableCardDeck()
    {
        cardDeckBtn.GetComponent<Button>().interactable = true;
    }
    public void DisableCardDeck()
    {
        cardDeckBtn.GetComponent<Button>().interactable = false;
    }
    public GameObject gO;
    public int spawnPos;
    void SetTotalPlayer(int totalPlayer = 4)
    {
        cardDeckBtn.SetActive(true);
        DisableCardDeck();
        cardWastePile.gameObject.SetActive(true);
        unoBtn.SetActive(true);
        if (totalPlayer == 2)
        {
            if (photonView.IsMine)
            {
                //Debug.LogError("playerCounts = "+ playerCounts);
                GameObject playerGO = PhotonNetwork.Instantiate(playerPfb.name, playersPos[0].transform.position, playersPos[playerCounts].transform.rotation, 0);
                //playerGO.transform.SetParent(playersPos[0].transform, false);
                //playerGO.transform.localScale = Vector3.one;
                //playerGO.transform.localPosition = Vector3.zero;                
            }
            else
            {
                //Debug.LogError("playerCounts = " + playerCounts);
                GameObject playerGO = PhotonNetwork.Instantiate(playerPfb.name, playersPos[1].transform.position, playersPos[playerCounts].transform.rotation, 0);
                //playerGO.transform.SetParent(playersPos[1].transform, false);
                //playerGO.transform.localScale = Vector3.one;
                //playerGO.transform.localPosition = Vector3.zero;
            }

            photonView.RPC("IncCount", RpcTarget.AllBuffered);

            //PhotonNetwork.Instantiate(playerPos.name, playerPos.transform.position, playerPos.transform.rotation, 0);            

            //players[0].gameObject.SetActive(true);
            //players[0].CardPanelBG.SetActive(true);

            //players[2].gameObject.SetActive(true);
            //players[2].CardPanelBG.SetActive(true);

            //players.RemoveAt(3);
            //players.RemoveAt(1);
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
        }
    }

    [PunRPC]
    void IncCount()
    {
        playerCounts++;
    }

    void SetupGame()
    {
        menuButton.SetActive(true);
        //if(PhotonNetwork.IsMasterClient)
        //{
            currentPlayerIndex = 0;
        //}
        //else
        //{
        //    currentPlayerIndex = 1;
        //}
        //currentPlayerIndex = Random.Range(0, players.Count);
        //players[0].SetAvatarProfile(GameManager.PlayerAvatarProfile);

        if (GameManager.currentGameMode == GameMode.MultiPlayer)
        {
            string[] nameList = multiplayerNames.text.Split('\n');
            List<int> indexes = new List<int>();

            for (int i = 1; i < players.Count; i++)
            {
                //players[i].playerName = GameMaster.instance.playerNames[i];
                //Debug.LogError("players[i].playerName = "+ players[i].playerName);

                //while (true)
                //{
                //    int index = Random.Range(0, nameList.Length);
                //    var name = nameList[index].Trim();
                //    if (name.Length == 0) continue;

                //    if (!indexes.Contains(index))
                //    {
                //        indexes.Add(index);
                //        //if (Random.value < LowercaseNameProbability / 100f) name = name.ToLower();
                //        players[i].SetAvatarProfile(new AvatarProfile { avatarIndex = index % GameManager.TOTAL_AVATAR, avatarName = "" });// players[i].playerName });
                //        break;
                //    }
                //}
            }
        }
        else
        {
            var profiles = JsonUtility.FromJson<AvatarProfiles>(computerProfiles.text).profiles;
            for (int i = 0; i < profiles.Count; i++)
            {
                players[i + 1].SetAvatarProfile(profiles[i]);
            }
        }

        CreateDeck();
        if (photonView.IsMine)
        {
            //Shuffle();
            RandomNums();
        }

        StartCoroutine(DealCards(NumOfCardsToPlay));
    }

    public void Shuffle()
    {
        object[] datas = new object[cards.Count];
        Debug.LogError("datas.Length = " + datas.Length);
        for (int i = 0; i < cards.Count; i++)
        {
            int index = Random.Range(0, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[index];
            cards[index] = temp;
        }

        //for (int i = 0; i < cards.Count; i++)
        //{
        //    datas[i] = cards[i];
        //    Debug.LogError("datas[i] = "+ datas[i].ToString());
        //}
        //datas = cards.ToArray();

        RaiseEventOptions options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };
        Debug.LogError("datas.Length = " + datas.Length);

        //PhotonNetwork.RaiseEvent(0, datas, options, SendOptions.SendReliable);
        //PhotonNetwork.RaiseEvent(0, new object[] { cards[0], cards[1], cards[2], cards[3], cards[4], cards[5], cards[6], cards[7], cards[8], cards[9], cards[10], cards[11]
        //, cards[12], cards[13], cards[14], cards[15], cards[16], cards[17], cards[18], cards[19], cards[20], cards[21], cards[22], cards[23]
        //, cards[24], cards[25], cards[26], cards[27], cards[28], cards[29], cards[31], cards[32], cards[33], cards[34], cards[35], cards[36]
        //, cards[37], cards[38], cards[39], cards[40], cards[41], cards[41], cards[42], cards[43], cards[44], cards[45], cards[46], cards[47]
        //, cards[48], cards[49], cards[50], cards[51], cards[52], cards[53], cards[54], cards[55]}, false, options);
    }

    void CreateDeck()
    {
        cards = new List<Card>();
        wasteCards = new List<Card>();
        for (int j = 1; j <= 4; j++)
        {
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Wild));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.DrawTwo));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Ego));
            if (j <= 2)
            {
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Odd));
                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Even));
            }
        }
        for (int i = 0; i <= 9; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
                // cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
            }
        }
    }

    Card CreateCardOnDeck(CardType t, CardValue v)
    {
        Card temp = Instantiate(_cardPrefab, cardDeckTransform.position, Quaternion.identity, cardDeckTransform);

        //GameObject cardGO = PhotonNetwork.Instantiate(_cardPrefab.name, transform.position, Quaternion.identity, 0);
        //cardGO.transform.SetParent(cardDeckTransform, false);
        //cardGO.transform.localScale = Vector3.one;
        //cardGO.transform.localPosition = Vector3.zero;

        //Card temp = cardGO.GetComponent<Card>();
        temp.Type = t;
        temp.Value = v;
        temp.IsOpen = false;
        temp.CalcPoint();
        temp.GetCardValue();
        temp.name = t.ToString() + "_" + v.ToString();
        return temp;
    }

    IEnumerator DealCards(int total)
    {
        yield return new WaitForSeconds(3f);
        for (int t = 0; t < total; t++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PickCardFromDeck(players[i]);
                yield return new WaitForSeconds(cardDealTime);
            }
        }

        yield return new WaitForSeconds(1.5f);
        int a = 0;
        while (cards[a].Type == CardType.Other)
        {
            a++;
        }

        for (int i = 0; i < players.Count; i++)
        {
            players[i].cardsPanel.UpdatePos();
        }

        setup = true;
        this.StartTurn();
        //CurrentPlayer.OnTurn();
    }

    IEnumerator DealCardsToPlayer(PunPlayer p, int NoOfCard = 1, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        for (int t = 0; t < NoOfCard; t++)
        {
            PickCardFromDeck(p, true);
            yield return new WaitForSeconds(cardDealTime);
        }
        this.StartTurn();
        //CurrentPlayer.OnTurn();
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
        p.AddCard(cards[0]);
        cards[0].IsOpen = p.isUserPlayer;
        if (updatePos)
            p.cardsPanel.UpdatePos();
        else
            cards[0].SetTargetPosAndRot(Vector3.zero, 0f);
        cards.RemoveAt(0);
        GameManager.PlaySound(throw_card_clip);
        p.GetTotalPoints(); // add points to text .... .. . 
        return temp;
    }

    public void PutCardToWastePile(Card c, PunPlayer p = null)
    {
        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
        {
            this.MakeTurn();
            //NextPlayerTurn();
            return;
        }
        if (c == null)
        {
            this.MakeTurn();
            //NextPlayerTurn();
            return;
        }

        if (p != null)
        {
            p.RemoveCard(c);
            GameManager.PlaySound(draw_card_clip);
        }

        CurrentType = c.Type;
        CurrentValue = c.Value;
        wasteCards.Add(c);
        c.IsOpen = true;
        c.transform.SetParent(cardWastePile.transform, true);
        c.SetTargetPosAndRot(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 1), c.transform.localRotation.eulerAngles.z + Random.Range(-15f, 15f));

        if (p != null)
        {
            //Debug.LogError("ccccccccc      CardType = " + c.Type);
            if (c.Type == CardType.Other || (c.Value == CardValue.Wild && CurrentPlayer.isSequentialCards))
            {
                if (c.Value == CardValue.DrawTwo)
                {
                    //this.MakeTurn();
                    //Invoke("NextPlayerTurn", 2f);
                    Invoke("WaitMakeTurn", 2f);
                    Invoke("DrawTwoCard", 3f);
                }
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
                        //this.MakeTurn();
                        //Invoke("NextPlayerTurn", 2f);
                        Invoke("WaitMakeTurn", 2f);
                    }
                }
            }
            else
            {
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
                else
                {
                    //this.MakeTurn();
                    //Debug.LogError("CurrentPlayer.isDoubleCards = "+ CurrentPlayer.isDoubleCards + "  &  = CurrentPlayer.isSequentialCards" + CurrentPlayer.isSequentialCards);
                    //Invoke("NextPlayerTurn", 2f);
                    Invoke("WaitMakeTurn", 2f);
                }
            }
        }
    }

    void WaitMakeTurn()
    {
        this.MakeTurn();
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
        for (int i = 1; i < players.Count; i++)
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
            currentPlayerIndex = Mod(currentPlayerIndex + step, players.Count);
        } while (!players[currentPlayerIndex].isInRoom);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void NextPlayerTurn()
    {
        Debug.LogError("NextPlayerTurn() ... playerName = " + gameObject.name);
        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
        {
            CurrentPlayer.isUserClicked = true;
        }
        else
        {
            OnDeckClick();
        }
        NextPlayerIndex();
        this.StartTurn();
        //CurrentPlayer.OnTurn();
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
        this.MakeTurn();
        //NextPlayerTurn();
    }

    public void EnableDeckClick()
    {
        arrowObject.SetActive(true);
    }
    public int oddEvenCount = 0;
    public void OnDeckClick()
    {
        if (!setup) return;

        if (arrowObject.activeInHierarchy)
        {
            arrowObject.SetActive(false);
            CurrentPlayer.pickFromDeck = true;
            PickCardFromDeck(CurrentPlayer, true);
            if (CurrentPlayer.cardsPanel.AllowedCard.Count == 0 || (!CurrentPlayer.Timer && CurrentPlayer.isUserPlayer))
            {
                this.OnEndTurn();
                //CurrentPlayer.OnTurnEnd();
                this.MakeTurn();
                //NextPlayerTurn();
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
        }
    }

    public void ResetPlayerCards()
    {
        if (CurrentPlayer.isUserPlayer)
        {
            oddEvenCount++;
            this.StartTurn();
            //CurrentPlayer.OnTurn();
            arrowObject.SetActive(false);
            DisableCardDeck();
        }
    }

    public void EnableUnoBtn()
    {
        unoBtn.GetComponent<Button>().interactable = true;
        unoBtn.GetComponent<Image>().color = Color.white;
    }

    public void DisableUnoBtn()
    {
        unoBtn.GetComponent<Button>().interactable = false;
        unoBtn.GetComponent<Image>().color = Color.gray;
    }

    public void OnUnoClick()
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Jugo!", true);
        CurrentPlayer.unoClicked = true;
        Invoke("SetupGameOver", 2f);
        GameManager.PlaySound(uno_btn_clip);
    }

    public void SetupGameOver()
    {
        gameOver = true;
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (!players[i].isInRoom)
            {
                players.RemoveAt(i);
            }
        }

        if (players.Count == 2)
        {
            playerObject[0].SetActive(true);
            playerObject[2].SetActive(true);
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "2nd Place";
            playerObject.RemoveAt(3);
            playerObject.RemoveAt(1);

        }
        else if (players.Count == 3)
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
            for (int i = 0; i < 4; i++)
            {
                playerObject[i].SetActive(true);
            }
        }

        players.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
        var winner = players[0];

        starParticle.gameObject.SetActive(winner.isUserPlayer);
        playerObject[0].GetComponentsInChildren<Image>()[1].sprite = winner.avatarImage.sprite;

        for (int i = 0; i < playerObject.Count; i++)
        {
            var playerNameText = playerObject[i].GetComponentInChildren<Text>();
            playerNameText.text = players[i].playerName;
            playerNameText.GetComponent<EllipsisText>().UpdateText();
            playerObject[i].GetComponentsInChildren<Image>()[1].sprite = players[i].avatarImage.sprite;
        }

        for (int i = 0; i < playerPoints.Count; i++)
        {
            playerPoints[i].text = players[i].GetTotalPoints().ToString();
        }

        GameManager.PlaySound(winner.isUserPlayer ? music_win_clip : music_loss_clip);
        gameOverPopup.SetActive(true);

        for (int i = 1; i < players.Count; i++)
        {
            if (players[i].isUserPlayer)
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

    public void StartTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            this.turnManager.BeginTurn();
            Debug.LogError("StartTurn()  this.turnManager.BeginTurn() ... .. . ");
        }
        else
        {
            Debug.LogError("StartTurn() Other   ... .. . ");
        }
    }

    public void MakeTurn()
    {
        //this.turnManager.SendMove(null, true);
        this.NextPlayerTurn();
        //this.CurrentPlayer.OnTurnEnd();
        Debug.LogError("MakeTurn() ... .. . ");
    }

    public void OnEndTurn()
    {
        Debug.LogError("OnEndTurn() ... .. . ");
        this.CurrentPlayer.OnTurnEnd();
        photonView.RPC("ClientTurnEnd", RpcTarget.Others);
        //this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
    }

    [PunRPC]
    void ClientTurnEnd()
    {
        Debug.LogError("ClientTurnEnd() = ");
        if(!PhotonNetwork.IsMasterClient)// || (PhotonNetwork.IsMasterClient && this.CurrentPlayer.isComputerTurn))
        {
            Debug.LogError("PhotonNetwork.IsMasterClient = " + PhotonNetwork.IsMasterClient);
            Debug.LogError("this.CurrentPlayer.isComputerTurn = " + this.CurrentPlayer.isComputerTurn);
            this.CurrentPlayer.OnTurnEnd();
            this.NextPlayerTurn();
        }
        
    }

    public void OnTurnBegins(int turn)
    {
        Debug.LogError("OnTurnBegins() turn = " + turn);
        this.CurrentPlayer.OnTurn();
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.LogError("OnTurnCompleted() turn = " + turn);
        this.CurrentPlayer.OnTurnEnd();
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        Debug.LogError("OnPlayerMove() player = " + player.NickName + "  & turn = " + turn);
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        Debug.LogError("OnPlayerFinished: " + player.NickName + " turn: " + turn + " action: " + move);

    }

    public void OnTurnTimeEnds(int turn)
    {
        Debug.LogError("OnTurnTimeEnds: Calling OnTurnCompleted turn = " + turn);
        //OnTurnCompleted(0);
    }
}     
*/


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;
//using ExitGames.Client.Photon;
//using Photon.Realtime;
//using Photon.Pun.UtilityScripts;

//public class MultiPlayerGamePlayManager : MonoBehaviourPun, IPunTurnManagerCallbacks
//{
//    public PhotonView photonView;

//    [Header("Sound")]
//    public AudioClip music_win_clip;
//    public AudioClip music_loss_clip;
//    public AudioClip draw_card_clip;
//    public AudioClip throw_card_clip;
//    public AudioClip uno_btn_clip;
//    public AudioClip choose_color_clip;

//    [Header("Gameplay")]
//    [Range(0, 100)]
//    public int LeftRoomProbability = 10;
//    [Range(0, 100)]
//    public int UnoProbability = 85;
//    [Range(0, 100)]
//    public int LowercaseNameProbability = 30;

//    public float cardDealTime = 0.05f;
//    public Card _cardPrefab;
//    public Transform cardDeckTransform;
//    public Image cardWastePile;
//    public GameObject arrowObject, unoBtn, cardDeckBtn;
//    public Popup colorChoose, playerChoose, noNetwork;
//    public GameObject loadingView, rayCastBlocker;
//    public Animator cardEffectAnimator;
//    public ParticleSystem wildCardParticle;
//    public GameObject menuButton;

//    [Header("Player Setting")]
//    public GameObject playerPos;
//    public GameObject playerPfb;
//    public List<PunPlayer> players;
//    public List<GameObject> playersPos;
//    public TextAsset multiplayerNames;
//    public TextAsset computerProfiles;
//    public bool clockwiseTurn = true;
//    public int currentPlayerIndex = 0;
//    public int NumOfCardsToPlay;
//    public PunPlayer CurrentPlayer { get { return players[currentPlayerIndex]; } }

//    [Header("Game Over")]
//    public GameObject gameOverPopup;
//    public ParticleSystem starParticle;
//    public List<GameObject> playerObject;
//    public List<Text> playerPoints;
//    public GameObject loseTimerAnimation;

//    private List<Card> cards;
//    private List<Card> wasteCards;

//    public bool isOdd, isEven;

//    public int turnCount = 0, roundsCount = 0;

//    public CardType CurrentType
//    {
//        get { return _currentType; }
//        set { _currentType = value; cardWastePile.color = value.GetColor(); }
//    }

//    public CardValue CurrentValue
//    {
//        get { return _currentValue; }
//        set { _currentValue = value; }
//    }

//    [SerializeField] CardType _currentType;
//    [SerializeField] CardValue _currentValue;

//    public bool IsDeckArrow
//    {
//        get { return arrowObject.activeSelf; }
//    }
//    public static MultiPlayerGamePlayManager instance;

//    System.DateTime pauseTime;
//    int fastForwardTime = 0;
//    bool setup = false, multiplayerLoaded = false, gameOver = false;

//    private PunTurnManager turnManager;
//    public float turnDuration = 15;
//    // keep track of when we show the results to handle game logic.
//    private bool IsShowingResults;

//    public int playerCounts = 0;
//    private int[] numbers;

//    private void OnEnable()
//    {
//        PhotonNetwork.NetworkingClient.EventReceived += onPhotonEvent;
//    }

//    private void OnDisable()
//    {
//        PhotonNetwork.NetworkingClient.EventReceived -= onPhotonEvent;
//    }

//    //private void OnEvent(EventData photonEvent)
//    //{
//    //    byte eventCode = photonEvent.Code;
//    //    if (eventCode == MoveUnitsToTargetPositionEvent)
//    //    {
//    //        object[] data = (object[])photonEvent.CustomData;
//    //        Vector3 targetPosition = (Vector3)data[0];
//    //        for (int index = 1; index < data.Length; ++index)
//    //        {
//    //            int unitId = (int)data[index];
//    //            UnitList[unitId].TargetPosition = targetPosition;
//    //        }
//    //    }
//    //}

//    //void OnEnable()
//    //{
//    //    PhotonNetwork.NetworkingClient.EventReceived += onPhotonEvent;
//    //}

//    //void OnDisable()
//    //{
//    //    PhotonNetwork.NetworkingClient.EventReceived -= onPhotonEvent;
//    //}
//    //private void onPhotonEvent(byte eventCode, object content, int senderId)
//    //{
//    //    object[] datas = content as object[];
//    //    List<Card> cards2 = new List<Card>();
//    //    if (datas.Length == 56)
//    //    {
//    //        for (int i = 0; i < datas.Length; i++)
//    //        {
//    //            //Debug.LogError("cards[(int)datas[i]] = " + cards[(int)datas[i]]);
//    //            cards2.Add(cards[(int)datas[i]]);
//    //        }
//    //        cards = cards2;
//    //    }
//    //}

//    private void onPhotonEvent(EventData photonEvent)
//    {
//        byte eventCode = photonEvent.Code;
//        if (eventCode == 0)
//        {
//            object[] datas = photonEvent.CustomData as object[];
//            List<Card> cards2 = new List<Card>();
//            if (datas.Length == 56)
//            {
//                for (int i = 0; i < datas.Length; i++)
//                {
//                    //Debug.LogError("cards[(int)datas[i]] = " + cards[(int)datas[i]]);
//                    cards2.Add(cards[(int)datas[i]]);
//                }
//                cards = cards2;
//            }
//        }
//    }

//    void Start()
//    {
//        instance = this;

//        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();
//        this.turnManager.TurnManagerListener = this;
//        this.turnManager.TurnDuration = turnDuration;

//        Input.multiTouchEnabled = false;
//        if (GameManager.currentGameMode == GameMode.Computer)
//        {
//            SetTotalPlayer(4);
//            SetupGame();
//        }
//        else
//        {
//            //StartCoroutine(CheckNetwork());
//            //playerChoose.ShowPopup();
//            StartCoroutine(StartMultiPlayerGameMode(2));
//        }

//        numbers = new int[56];
//        for (int i = 0; i < numbers.Length; i++) numbers[i] = i;
//    }

//    void Update()
//    {
//        if (photonView.IsMine)
//        {
//            if (Input.GetKeyDown(KeyCode.Space))
//            {
//                RandomNums();
//            }
//        }

//        if (!PhotonNetwork.InRoom)
//        {
//            return;
//        }
//    }

//    void RandomNums()
//    {
//        object[] datas = new object[numbers.Length];
//        for (int i = 0; i < numbers.Length; i++)
//        {
//            int index = Random.Range(0, numbers.Length);
//            int temp = numbers[i];
//            numbers[i] = numbers[index];
//            numbers[index] = temp;
//        }

//        for (int i = 0; i < numbers.Length; i++)
//        {
//            datas[i] = numbers[i];
//        }

//        RaiseEventOptions options = new RaiseEventOptions()
//        {
//            CachingOption = EventCaching.DoNotCache,
//            Receivers = ReceiverGroup.All
//        };

//        PhotonNetwork.RaiseEvent(0, datas, options, SendOptions.SendReliable);
//    }

//    public void OnPlayerSelect(ToggleGroup group)
//    {
//        playerChoose.HidePopup(false);
//        int i = 4;
//        foreach (var t in group.ActiveToggles())
//        {
//            i = int.Parse(t.name);
//        }
//        StartCoroutine(StartMultiPlayerGameMode(i));
//        GameManager.PlayButton();
//    }

//    IEnumerator StartMultiPlayerGameMode(int i)
//    {
//        loadingView.SetActive(true);
//        yield return new WaitForSeconds(Random.Range(3f, 4f));
//        loadingView.SetActive(false);
//        SetTotalPlayer(i);
//        SetupGame();
//        AddAllActivePlayers();

//        bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
//        if (leftRoom)
//        {
//            float inTime = Random.Range(7, 5 * 60);
//            StartCoroutine(RemovePlayerFromRoom(inTime));
//        }

//        multiplayerLoaded = true;
//    }

//    public void AddAllActivePlayers()
//    {
//        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
//        {
//            //Debug.LogError("player.NickName = "+ player.NickName);
//            GameMaster.instance.playerNames.Add(player.NickName);
//        }
//    }

//    public void EnableCardDeck()
//    {
//        cardDeckBtn.GetComponent<Button>().interactable = true;
//    }
//    public void DisableCardDeck()
//    {
//        cardDeckBtn.GetComponent<Button>().interactable = false;
//    }
//    public GameObject gO;
//    public int spawnPos;
//    void SetTotalPlayer(int totalPlayer = 4)
//    {
//        cardDeckBtn.SetActive(true);
//        DisableCardDeck();
//        cardWastePile.gameObject.SetActive(true);
//        unoBtn.SetActive(true);
//        if (totalPlayer == 2)
//        {
//            if (photonView.IsMine)
//            {
//                //Debug.LogError("playerCounts = "+ playerCounts);
//                GameObject playerGO = PhotonNetwork.Instantiate(playerPfb.name, playersPos[playerCounts].transform.position, playersPos[playerCounts].transform.rotation, 0);
//                //playerGO.transform.SetParent(playersPos[0].transform, false);
//                //playerGO.transform.localScale = Vector3.one;
//                //playerGO.transform.localPosition = Vector3.zero;                
//            }
//            else
//            {
//                //Debug.LogError("playerCounts = " + playerCounts);
//                GameObject playerGO = PhotonNetwork.Instantiate(playerPfb.name, playersPos[playerCounts].transform.position, playersPos[playerCounts].transform.rotation, 0);
//                //playerGO.transform.SetParent(playersPos[1].transform, false);
//                //playerGO.transform.localScale = Vector3.one;
//                //playerGO.transform.localPosition = Vector3.zero;
//            }

//            photonView.RPC("IncCount", RpcTarget.AllBuffered);

//            //PhotonNetwork.Instantiate(playerPos.name, playerPos.transform.position, playerPos.transform.rotation, 0);            

//            //players[0].gameObject.SetActive(true);
//            //players[0].CardPanelBG.SetActive(true);

//            //players[2].gameObject.SetActive(true);
//            //players[2].CardPanelBG.SetActive(true);

//            //players.RemoveAt(3);
//            //players.RemoveAt(1);
//        }
//        else if (totalPlayer == 3)
//        {
//            for (int i = 0; i < 3; i++)
//            {
//                players[i].gameObject.SetActive(true);
//                players[i].CardPanelBG.SetActive(true);
//            }
//            players[3].CardPanelBG.SetActive(true);
//            players.RemoveAt(3);

//        }
//        else
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                players[i].gameObject.SetActive(true);
//                players[i].CardPanelBG.SetActive(true);
//            }
//        }
//    }

//    [PunRPC]
//    void IncCount()
//    {
//        playerCounts++;
//    }

//    void SetupGame()
//    {
//        menuButton.SetActive(true);
//        currentPlayerIndex = 0; //Random.Range(0, players.Count);
//        //players[0].SetAvatarProfile(GameManager.PlayerAvatarProfile);

//        if (GameManager.currentGameMode == GameMode.MultiPlayer)
//        {
//            string[] nameList = multiplayerNames.text.Split('\n');
//            List<int> indexes = new List<int>();

//            for (int i = 1; i < players.Count; i++)
//            {
//                //players[i].playerName = GameMaster.instance.playerNames[i];
//                //Debug.LogError("players[i].playerName = "+ players[i].playerName);

//                //while (true)
//                //{
//                //    int index = Random.Range(0, nameList.Length);
//                //    var name = nameList[index].Trim();
//                //    if (name.Length == 0) continue;

//                //    if (!indexes.Contains(index))
//                //    {
//                //        indexes.Add(index);
//                //        //if (Random.value < LowercaseNameProbability / 100f) name = name.ToLower();
//                //        players[i].SetAvatarProfile(new AvatarProfile { avatarIndex = index % GameManager.TOTAL_AVATAR, avatarName = "" });// players[i].playerName });
//                //        break;
//                //    }
//                //}
//            }
//        }
//        else
//        {
//            var profiles = JsonUtility.FromJson<AvatarProfiles>(computerProfiles.text).profiles;
//            for (int i = 0; i < profiles.Count; i++)
//            {
//                players[i + 1].SetAvatarProfile(profiles[i]);
//            }
//        }

//        CreateDeck();
//        if (photonView.IsMine)
//        {
//            //Shuffle();
//            RandomNums();
//        }

//        StartCoroutine(DealCards(NumOfCardsToPlay));
//    }

//    public void Shuffle()
//    {
//        object[] datas = new object[cards.Count];
//        Debug.LogError("datas.Length = " + datas.Length);
//        for (int i = 0; i < cards.Count; i++)
//        {
//            int index = Random.Range(0, cards.Count);
//            Card temp = cards[i];
//            cards[i] = cards[index];
//            cards[index] = temp;
//        }

//        //for (int i = 0; i < cards.Count; i++)
//        //{
//        //    datas[i] = cards[i];
//        //    Debug.LogError("datas[i] = "+ datas[i].ToString());
//        //}
//        //datas = cards.ToArray();

//        RaiseEventOptions options = new RaiseEventOptions()
//        {
//            CachingOption = EventCaching.DoNotCache,
//            Receivers = ReceiverGroup.All
//        };
//        Debug.LogError("datas.Length = " + datas.Length);

//        //PhotonNetwork.RaiseEvent(0, datas, options, SendOptions.SendReliable);
//        //PhotonNetwork.RaiseEvent(0, new object[] { cards[0], cards[1], cards[2], cards[3], cards[4], cards[5], cards[6], cards[7], cards[8], cards[9], cards[10], cards[11]
//        //, cards[12], cards[13], cards[14], cards[15], cards[16], cards[17], cards[18], cards[19], cards[20], cards[21], cards[22], cards[23]
//        //, cards[24], cards[25], cards[26], cards[27], cards[28], cards[29], cards[31], cards[32], cards[33], cards[34], cards[35], cards[36]
//        //, cards[37], cards[38], cards[39], cards[40], cards[41], cards[41], cards[42], cards[43], cards[44], cards[45], cards[46], cards[47]
//        //, cards[48], cards[49], cards[50], cards[51], cards[52], cards[53], cards[54], cards[55]}, false, options);
//    }

//    void CreateDeck()
//    {
//        cards = new List<Card>();
//        wasteCards = new List<Card>();
//        for (int j = 1; j <= 4; j++)
//        {
//            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Wild));
//            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.DrawTwo));
//            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Ego));
//            if (j <= 2)
//            {
//                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Odd));
//                cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Even));
//            }
//        }
//        for (int i = 0; i <= 9; i++)
//        {
//            for (int j = 1; j <= 4; j++)
//            {
//                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
//                // cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
//            }
//        }
//    }

//    Card CreateCardOnDeck(CardType t, CardValue v)
//    {
//        Card temp = Instantiate(_cardPrefab, cardDeckTransform.position, Quaternion.identity, cardDeckTransform);

//        //GameObject cardGO = PhotonNetwork.Instantiate(_cardPrefab.name, transform.position, Quaternion.identity, 0);
//        //cardGO.transform.SetParent(cardDeckTransform, false);
//        //cardGO.transform.localScale = Vector3.one;
//        //cardGO.transform.localPosition = Vector3.zero;

//        //Card temp = cardGO.GetComponent<Card>();
//        temp.Type = t;
//        temp.Value = v;
//        temp.IsOpen = false;
//        temp.CalcPoint();
//        temp.GetCardValue();
//        temp.name = t.ToString() + "_" + v.ToString();
//        return temp;
//    }

//    IEnumerator DealCards(int total)
//    {
//        yield return new WaitForSeconds(3f);
//        for (int t = 0; t < total; t++)
//        {
//            for (int i = 0; i < players.Count; i++)
//            {
//                PickCardFromDeck(players[i]);
//                yield return new WaitForSeconds(cardDealTime);
//            }
//        }

//        yield return new WaitForSeconds(1.5f);
//        int a = 0;
//        while (cards[a].Type == CardType.Other)
//        {
//            a++;
//        }

//        for (int i = 0; i < players.Count; i++)
//        {
//            players[i].cardsPanel.UpdatePos();
//        }

//        setup = true;
//        if(PhotonNetwork.IsMasterClient)
//        {
//            Debug.LogError("playerName = "+ photonView.name);
//            //
//            photonView.RPC("OnTurn_RPC", RpcTarget.AllBuffered);
//        }
//        else
//        {
//            Debug.LogError("PhotonNetwork.IsMasterClient = " + PhotonNetwork.IsMasterClient);

//        }
//    }

//    [PunRPC]
//    public void OnTurn_RPC()
//    {
//        Debug.LogError("OnTurn_RPC... .. .");
//        CurrentPlayer.OnTurn();
//    }

//    IEnumerator DealCardsToPlayer(PunPlayer p, int NoOfCard = 1, float delay = 0f)
//    {
//        yield return new WaitForSeconds(delay);
//        for (int t = 0; t < NoOfCard; t++)
//        {
//            PickCardFromDeck(p, true);
//            yield return new WaitForSeconds(cardDealTime);
//        }
//        CurrentPlayer.OnTurn();
//    }

//    public Card PickCardFromDeck(PunPlayer p, bool updatePos = false)
//    {
//        if (cards.Count == 0)
//        {
//            print("Card Over");
//            while (wasteCards.Count > 5)
//            {
//                cards.Add(wasteCards[0]);
//                wasteCards[0].transform.SetParent(cardDeckTransform);
//                wasteCards[0].transform.localPosition = Vector3.zero;
//                wasteCards[0].transform.localRotation = Quaternion.Euler(Vector3.zero);
//                wasteCards[0].IsOpen = false;
//                wasteCards.RemoveAt(0);
//            }
//        }
//        Card temp = cards[0];
//        p.AddCard(cards[0]);
//        cards[0].IsOpen = p.isUserPlayer;
//        if (updatePos)
//            p.cardsPanel.UpdatePos();
//        else
//            cards[0].SetTargetPosAndRot(Vector3.zero, 0f);
//        cards.RemoveAt(0);
//        GameManager.PlaySound(throw_card_clip);
//        p.GetTotalPoints(); // add points to text .... .. . 
//        return temp;
//    }

//    public void PutCardToWastePile(Card c, PunPlayer p = null)
//    {
//        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
//        {
//            NextPlayerTurn();
//            return;
//        }
//        if (c == null)
//        {
//            NextPlayerTurn();
//            return;
//        }

//        if (p != null)
//        {
//            p.RemoveCard(c);
//            GameManager.PlaySound(draw_card_clip);
//        }

//        CurrentType = c.Type;
//        CurrentValue = c.Value;
//        wasteCards.Add(c);
//        c.IsOpen = true;
//        c.transform.SetParent(cardWastePile.transform, true);
//        c.SetTargetPosAndRot(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 1), c.transform.localRotation.eulerAngles.z + Random.Range(-15f, 15f));

//        if (p != null)
//        {
//            //Debug.LogError("ccccccccc      CardType = " + c.Type);
//            if (c.Type == CardType.Other || (c.Value == CardValue.Wild && CurrentPlayer.isSequentialCards))
//            {
//                if (c.Value == CardValue.DrawTwo)
//                {
//                    Invoke("NextPlayerTurn", 2f);
//                    Invoke("DrawTwoCard", 3f);
//                }
//                else
//                {
//                    if (c.Value == CardValue.Odd)
//                    {
//                        isOdd = true;
//                    }
//                    else if (c.Value == CardValue.Even)
//                    {
//                        isEven = true;
//                    }

//                    if (!CurrentPlayer.isSequentialCards)
//                    {
//                        Invoke("NextPlayerTurn", 2f);
//                    }
//                }
//            }
//            else
//            {
//                //Debug.LogError("CurrentPlayer.isDoubleCards = " + CurrentPlayer.isDoubleCards + "  &  = CurrentPlayer.isSequentialCards = " + CurrentPlayer.isSequentialCards);
//                //Debug.LogError("CurrentPlayer.isOddEvenDoubles = " + CurrentPlayer.isOddEvenDoubles);
//                if (CurrentPlayer.isDoubleCards)
//                {
//                    return;
//                }
//                else if (CurrentPlayer.isSequentialCards)
//                {
//                    return;
//                }
//                else if (CurrentPlayer.isOddEvenDoubles)
//                {
//                    return;
//                }
//                else
//                {
//                    //Debug.LogError("CurrentPlayer.isDoubleCards = "+ CurrentPlayer.isDoubleCards + "  &  = CurrentPlayer.isSequentialCards" + CurrentPlayer.isSequentialCards);
//                    Invoke("NextPlayerTurn", 2f);
//                }
//            }
//        }
//    }

//    public void DrawTwoCard()
//    {
//        CurrentPlayer.ShowMessage("+2");
//        wildCardParticle.Emit(30);
//        StartCoroutine(DealCardsToPlayer(CurrentPlayer, 2, .5f));
//    }

//    private IEnumerator RemovePlayerFromRoom(float time)
//    {
//        yield return new WaitForSeconds(time);

//        if (gameOver) yield break;

//        List<int> indexes = new List<int>();
//        for (int i = 1; i < players.Count; i++)
//        {
//            indexes.Add(i);
//        }
//        indexes.Shuffle();

//        int index = -1;
//        foreach (var i in indexes)
//        {
//            if (!players[i].Timer)
//            {
//                index = i;
//                break;
//            }
//        }

//        var player = players[index];
//        player.isInRoom = false;

//        Toast.instance.ShowMessage(player.playerName + " left the room", 2.5f);

//        yield return new WaitForSeconds(2f);

//        player.gameObject.SetActive(false);
//        foreach (var item in player.cardsPanel.cards)
//        {
//            item.gameObject.SetActive(false);
//        }
//    }

//    void ChooseColorforAI()
//    {
//        CurrentPlayer.ChooseBestColor();
//    }

//    public void NextPlayerIndex()
//    {
//        int step = clockwiseTurn ? 1 : -1;
//        do
//        {
//            currentPlayerIndex = Mod(currentPlayerIndex + step, players.Count);
//        } while (!players[currentPlayerIndex].isInRoom);
//    }

//    private int Mod(int x, int m)
//    {
//        return (x % m + m) % m;
//    }       

//    public void NextPlayerTurn()
//    {
//        //Debug.LogError("NextPlayerTurn() ... playerName = "+ gameObject.name);
//        if (CurrentPlayer.isUserPlayer && !CurrentPlayer.isUserClicked)
//        {
//            CurrentPlayer.isUserClicked = true;
//        }
//        else
//        {
//            OnDeckClick();
//        }
//        NextPlayerIndex();
//        CurrentPlayer.OnTurn();
//    }

//    public void OnColorSelect(int i)
//    {
//        if (!colorChoose.isOpen) return;
//        colorChoose.HidePopup();

//        SelectColor(i);
//    }

//    public void SelectColor(int i)
//    {
//        CurrentPlayer.Timer = false;
//        CurrentPlayer.choosingColor = false;
//        NextPlayerTurn();
//    }

//    public void EnableDeckClick()
//    {
//        arrowObject.SetActive(true);
//    }
//    public int oddEvenCount = 0;
//    public void OnDeckClick()
//    {
//        if (!setup) return;

//        if (arrowObject.activeInHierarchy)
//        {
//            arrowObject.SetActive(false);
//            CurrentPlayer.pickFromDeck = true;
//            PickCardFromDeck(CurrentPlayer, true);
//            if (CurrentPlayer.cardsPanel.AllowedCard.Count == 0 || (!CurrentPlayer.Timer && CurrentPlayer.isUserPlayer))
//            {
//                CurrentPlayer.OnTurnEnd();
//                NextPlayerTurn();
//            }
//            else
//            {
//                CurrentPlayer.UpdateCardColor();
//            }
//        }
//        else if (!CurrentPlayer.pickFromDeck)// && CurrentPlayer.isUserPlayer)  //commented to pickup card from deck for every player...... 
//        {
//            PickCardFromDeck(CurrentPlayer, true);
//            CurrentPlayer.pickFromDeck = true;
//        }
//    }

//    public void ResetPlayerCards()
//    {
//        if (CurrentPlayer.isUserPlayer)
//        {
//            oddEvenCount++;
//            CurrentPlayer.OnTurn();
//            arrowObject.SetActive(false);
//            DisableCardDeck();
//        }
//    }

//    public void EnableUnoBtn()
//    {
//        unoBtn.GetComponent<Button>().interactable = true;
//        unoBtn.GetComponent<Image>().color = Color.white;
//    }

//    public void DisableUnoBtn()
//    {
//        unoBtn.GetComponent<Button>().interactable = false;
//        unoBtn.GetComponent<Image>().color = Color.gray;
//    }

//    public void OnUnoClick()
//    {
//        DisableUnoBtn();
//        CurrentPlayer.ShowMessage("Jugo!", true);
//        CurrentPlayer.unoClicked = true;
//        Invoke("SetupGameOver", 2f);
//        GameManager.PlaySound(uno_btn_clip);
//    }

//    public void SetupGameOver()
//    {
//        gameOver = true;
//        for (int i = players.Count - 1; i >= 0; i--)
//        {
//            if (!players[i].isInRoom)
//            {
//                players.RemoveAt(i);
//            }
//        }

//        if (players.Count == 2)
//        {
//            playerObject[0].SetActive(true);
//            playerObject[2].SetActive(true);
//            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "2nd Place";
//            playerObject.RemoveAt(3);
//            playerObject.RemoveAt(1);

//        }
//        else if (players.Count == 3)
//        {
//            playerObject.RemoveAt(2);
//            for (int i = 0; i < 3; i++)
//            {
//                playerObject[i].SetActive(true);
//            }
//            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "3rd Place";

//        }
//        else
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                playerObject[i].SetActive(true);
//            }
//        }

//        players.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
//        var winner = players[0];

//        starParticle.gameObject.SetActive(winner.isUserPlayer);
//        playerObject[0].GetComponentsInChildren<Image>()[1].sprite = winner.avatarImage.sprite;

//        for (int i = 0; i < playerObject.Count; i++)
//        {
//            var playerNameText = playerObject[i].GetComponentInChildren<Text>();
//            playerNameText.text = players[i].playerName;
//            playerNameText.GetComponent<EllipsisText>().UpdateText();
//            playerObject[i].GetComponentsInChildren<Image>()[1].sprite = players[i].avatarImage.sprite;
//        }

//        for (int i = 0; i < playerPoints.Count; i++)
//        {
//            playerPoints[i].text = players[i].GetTotalPoints().ToString();
//        }

//        GameManager.PlaySound(winner.isUserPlayer ? music_win_clip : music_loss_clip);
//        gameOverPopup.SetActive(true);

//        for (int i = 1; i < players.Count; i++)
//        {
//            if (players[i].isUserPlayer)
//            {
//                loseTimerAnimation.SetActive(true);
//                loseTimerAnimation.transform.position = playerObject[i].transform.position;
//                break;
//            }
//        }

//        gameOverPopup.GetComponent<Animator>().enabled = winner.isUserPlayer;
//        gameOverPopup.GetComponentInChildren<Text>().text = winner.isUserPlayer ? "You win Game." : "You Lost Game ...   Try Again.";
//        fastForwardTime = 0;
//        Time.timeScale = 0;
//    }

//    IEnumerator CheckNetwork()
//    {
//        while (true)
//        {
//            WWW www = new WWW("https://www.google.com");
//            yield return www;
//            if (string.IsNullOrEmpty(www.error))
//            {
//                if (noNetwork.isOpen)
//                {
//                    noNetwork.HidePopup();

//                    Time.timeScale = 1;
//                    OnApplicationPause(false);
//                }
//            }
//            else
//            {
//                if (Time.timeScale == 1)
//                {
//                    noNetwork.ShowPopup();

//                    Time.timeScale = 0;
//                    pauseTime = System.DateTime.Now;
//                }
//            }

//            yield return new WaitForSecondsRealtime(1f);
//        }
//    }

//    void OnApplicationPause(bool pauseStatus)
//    {
//        if (pauseStatus)
//        {
//            pauseTime = System.DateTime.Now;
//        }
//        else
//        {
//            if (GameManager.currentGameMode == GameMode.MultiPlayer && multiplayerLoaded && !gameOver)
//            {
//                fastForwardTime += Mathf.Clamp((int)(System.DateTime.Now - pauseTime).TotalSeconds, 0, 3600);
//                if (Time.timeScale == 1f)
//                {
//                    StartCoroutine(DoFastForward());
//                }
//            }
//        }
//    }

//    IEnumerator DoFastForward()
//    {
//        Time.timeScale = 10f;
//        rayCastBlocker.SetActive(true);
//        while (fastForwardTime > 0)
//        {
//            yield return new WaitForSeconds(1f);
//            fastForwardTime--;
//        }
//        Time.timeScale = 1f;
//        rayCastBlocker.SetActive(false);

//    }

//    public void StartTurn()
//    {
//        if (PhotonNetwork.IsMasterClient)
//        {
//            this.turnManager.BeginTurn();
//            Debug.LogError("StartTurn()  this.turnManager.BeginTurn() ... .. . ");
//        }
//        else
//        {
//            Debug.LogError("StartTurn() Other   ... .. . ");
//        }
//    }

//    public void MakeTurn()
//    {
//        this.turnManager.SendMove(null, true);
//        Debug.LogError("MakeTurn() ... .. . ");
//    }

//    public void OnEndTurn()
//    {
//        Debug.LogError("OnEndTurn() ... .. . ");
//        //this.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
//    }


//    public void OnTurnBegins(int turn)
//    {
//        Debug.LogError("OnTurnBegins() turn = " + turn);
//    }

//    public void OnTurnCompleted(int turn)
//    {
//        Debug.LogError("OnTurnCompleted() turn = " + turn);
//    }

//    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
//    {
//        Debug.LogError("OnPlayerMove() player = " + player.NickName + "  & turn = " + turn);
//    }

//    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
//    {
//        Debug.LogError("OnPlayerFinished: " + player.NickName + " turn: " + turn + " action: " + move);

//    }

//    public void OnTurnTimeEnds(int turn)
//    {
//        Debug.LogError("OnTurnTimeEnds: Calling OnTurnCompleted turn = " + turn);
//    }
//}