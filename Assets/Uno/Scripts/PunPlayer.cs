using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunPlayer : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    public PhotonView PV;
    public int Actornumber;
    //public int posinscene;
    public GameObject CardPanelBG;
    //public GameObject PlayerBG;
    public PlayerCards cardsPanel;
    public GameObject Avatar;
    public string playerName;
    public bool isUserPlayer, isUserClicked;
    public Image avatarImage;
    public Text avatarName;
    public Text messageLbl;
    public ParticleSystem starParticleSystem;
    public Image timerImage;
    public GameObject timerOjbect;
    public Text pointTxt;
    public List<Card> Cards;
    public List<int> cardVal;
    public int PlayerAvatarIndex = -1;

    private float totalTimer = 45f;//15f;

    [HideInInspector]
    public bool pickFromDeck, unoClicked, choosingColor;
    [HideInInspector]
    public bool isInRoom = true;

    public bool isDoubleCards, isSequentialCards, isOddEvenDoubles;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        isUserPlayer = PV.IsMine;
    }

    void Start()
    {
        transform.SetParent(
    MultiPlayerGamePlayManager.instance.gameObject.transform, false);

        Timer = false;
        //Debug.Log(PV.Owner.ActorNumber);
        Actornumber = PV.Owner.ActorNumber;
        string myname = PhotonNetwork.NickName;
        int d = -1;
        foreach (var item in PhotonNetwork.CurrentRoom.Players)
            if (item.Value.NickName == myname)
                d = item.Key;

        int pos = ((Actornumber - d) % PhotonNetwork.CurrentRoom.PlayerCount);
        if (pos < 0)
            pos = PhotonNetwork.CurrentRoom.PlayerCount + pos;
        Debug.Log("Pos=" + pos + " Actornumber=" + Actornumber + "    d=" + d
            + "   PlayerCount=" + PhotonNetwork.CurrentRoom.PlayerCount);
        //Debug.Log("this system is for number" + d);
        //Debug.Log("actorid=" + Actornumber + "  sytemid=" + d + " pos=" + pos);
        CardPanelBG.transform.SetParent(MultiPlayerGamePlayManager.instance.PlayerBg[pos].transform);
        RectTransform parentRect = MultiPlayerGamePlayManager.instance.PlayerBg[pos].GetComponent<RectTransform>();
        CardPanelBG.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        CardPanelBG.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        CardPanelBG.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

        CardPanelBG.GetComponent<RectTransform>().localPosition = Vector3.zero;
        CardPanelBG.GetComponent<RectTransform>().sizeDelta = parentRect.sizeDelta;

        CardPanelBG.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;


        Avatar.transform.SetParent(MultiPlayerGamePlayManager.instance.Avatars[pos].transform);
        Avatar.GetComponent<RectTransform>().localPosition = Vector3.zero;
        cardsPanel.maxSpace = (PV.IsMine) ? 70 : 40;
        MultiPlayerGamePlayManager.instance.players[Actornumber - 1] = this;
        MultiPlayerGamePlayManager.instance.PlayerSharedDatas[Actornumber - 1].PhotonName = PV.Owner.NickName;

        if (PV.IsMine)
        {
            playerName = GameManager.PlayerAvatarName;
            PlayerAvatarIndex = GameManager.PlayerAvatarIndex;
            if (avatarName != null)
            {
                avatarName.text = GameManager.PlayerAvatarName;
                avatarName.GetComponent<EllipsisText>().UpdateText();
            }
            if (avatarImage != null)
                avatarImage.sprite = Resources.Load<Sprite>("Avatar/" + GameManager.PlayerAvatarIndex);
        }

    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Q))
    //        CardPanelBG.GetComponent<RectTransform>().eulerAngles = Vector3.zero;

    //    if (Input.GetKeyUp(KeyCode.W))
    //        CardPanelBG.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);

    //    if (Input.GetKeyUp(KeyCode.E))
    //        CardPanelBG.transform.eulerAngles = Vector3.zero;

    //    if (Input.GetKeyUp(KeyCode.R))
    //        CardPanelBG.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

    //}

    //public void SetAvatarProfile(AvatarProfile p)
    //{
    //    playerName = p.avatarName;
    //    if (avatarName != null)
    //    {
    //        avatarName.text = p.avatarName;
    //        avatarName.GetComponent<EllipsisText>().UpdateText();
    //    }
    //    if (avatarImage != null)
    //        avatarImage.sprite = Resources.Load<Sprite>("Avatar/" + p.avatarIndex);
    //}

    public bool Timer
    {
        get
        {
            return timerOjbect.activeInHierarchy;
        }
        set
        {
            CancelInvoke("UpdateTimer");
            timerOjbect.SetActive(value);
            if (value)
            {
                timerImage.fillAmount = 1f;
                InvokeRepeating("UpdateTimer", 0f, .1f);
            }
            else
            {
                timerImage.fillAmount = 0f;
            }
        }
    }

    void UpdateTimer()
    {
        return;
        Debug.Log("Timer for" + Actornumber + "  " + PV.Owner.NickName);
        timerImage.fillAmount -= 0.1f / totalTimer;
        if (timerImage.fillAmount <= 0)
        {
            //if (choosingColor)
            //{
            //    if (isUserPlayer)
            //    {
            //        MultiPlayerGamePlayManager.instance.colorChoose.HidePopup();
            //    }
            //    ChooseBestColor();
            //}
            if (MultiPlayerGamePlayManager.instance.IsDeckArrow)
            {
                MultiPlayerGamePlayManager.instance.OnDeckClick();
            }
            else if (cardsPanel.AllowedCard.Count > 0)
            {
                isUserClicked = false;
                OnCardClick(FindBestPutCard());
            }
            //else if (cardsPanel.AllowOddCards.Count > 0)
            //{
            //    isUserClicked = false;
            //    OnCardClick(GetBestOddCard());
            //}
            //else if (cardsPanel.AllowEvenCards.Count > 0)
            //{
            //    isUserClicked = false;
            //    OnCardClick(GetBestEvenCard());
            //}
            else
            {
                //OnTurnEnd();
            }
        }
    }

    int oddEvenCount = 0;
    public void OnTurn()
    {
        Debug.Log("OnTurn() ... playerName = " + Actornumber);
        unoClicked = false;
        pickFromDeck = false;
        Timer = true;
        GetListOfValues();
        MultiPlayerGamePlayManager.instance.DisableUnoBtn();
        if (MultiPlayerGamePlayManager.instance.isEven || MultiPlayerGamePlayManager.instance.isOdd)
        {
            MultiPlayerGamePlayManager.instance.turnCount++;
            if (MultiPlayerGamePlayManager.instance.turnCount > 3)
            {
                MultiPlayerGamePlayManager.instance.isEven = false;
                MultiPlayerGamePlayManager.instance.isOdd = false;
                MultiPlayerGamePlayManager.instance.turnCount = 0;
            }
            //Debug.LogError("turnCount = "+ MultiPlayerGamePlayManager.instance.turnCount);
            //Debug.LogError("MultiPlayerGamePlayManager.instance.isEven = " + MultiPlayerGamePlayManager.instance.isEven + "  & MultiPlayerGamePlayManager.instance.isOdd = " + MultiPlayerGamePlayManager.instance.isOdd);
        }
        //if (!MultiPlayerGamePlayManager.instance.isEven && !MultiPlayerGamePlayManager.instance.isOdd)
        //{
        //    GetDoubleCard();
        //}

        if (isUserPlayer)
        {
            MultiPlayerGamePlayManager.instance.roundsCount++;
            //Debug.Log("isUserPlayer = " + isUserPlayer);
            if ((MultiPlayerGamePlayManager.instance.isEven && cardsPanel.AllowEvenCards.Count < 1) || (MultiPlayerGamePlayManager.instance.isOdd && cardsPanel.AllowOddCards.Count < 1))
            {
                if (MultiPlayerGamePlayManager.instance.oddEvenCount < 1)
                {
                    MultiPlayerGamePlayManager.instance.EnableDeckClick();
                    MultiPlayerGamePlayManager.instance.EnableCardDeck();
                }
            }
            UpdateCardColor();
            if (MultiPlayerGamePlayManager.instance.roundsCount >= 2)   //(cardsPanel.AllowedCard.Count == 0)
            {
                MultiPlayerGamePlayManager.instance.EnableUnoBtn();
            }
        }
        //else
        //{
        //    StartCoroutine(DoComputerTurn());
        //}
    }

    public void UpdateCardColor()
    {
        if (isUserPlayer)
        {
            foreach (var item in cardsPanel.cards)
            {
                //item.SetCardsPos(true);
                StartCoroutine(item.SetCardsPos(true, 0f));
            }
            if (MultiPlayerGamePlayManager.instance.isEven)
            {
                Debug.LogError("Even Num = " + MultiPlayerGamePlayManager.instance.isEven);
                foreach (var item in cardsPanel.AllowEvenCards)
                {
                    item.SetGaryColor(false);
                    item.IsClickable = true;
                    //item.SetCardsPos(false);
                    StartCoroutine(item.SetCardsPos(false, 0.5f));
                }
                foreach (var item in cardsPanel.DisallowedEvenCard)
                {
                    item.SetGaryColor(true);
                    item.IsClickable = false;
                    //item.SetCardsPos(true);
                    StartCoroutine(item.SetCardsPos(true, 0f));
                }
            }
            else if (MultiPlayerGamePlayManager.instance.isOdd)
            {
                //Debug.LogError("Odd Num = " + MultiPlayerGamePlayManager.instance.isOdd);
                foreach (var item in cardsPanel.AllowOddCards)
                {
                    item.SetGaryColor(false);
                    item.IsClickable = true;
                    //item.SetCardsPos(false);
                    StartCoroutine(item.SetCardsPos(false, 0.5f));
                }
                foreach (var item in cardsPanel.DisallowedOddCard)
                {
                    item.SetGaryColor(true);
                    item.IsClickable = false;
                    //item.SetCardsPos(true);
                    StartCoroutine(item.SetCardsPos(true, 0f));
                }
            }
            else if (isDoubleCards || isSequentialCards)
            {
                //Debug.LogError("UpdateCardColor()    isDoubleCards = " + isDoubleCards + "  & isSequentialCards = " + isSequentialCards);
                foreach (var item in cardsPanel.AllowDoubleCards)
                {
                    //Debug.LogError("item = "+ item.name);
                    //item.SetGaryColor(false);
                    item.IsClickable = true;
                    //item.SetCardsPos(false);
                    StartCoroutine(item.SetCardsPos(false, 0.5f));
                }
                foreach (var item in cardsPanel.DisallowDoubleCards)
                {
                    //item.SetGaryColor(true);
                    item.IsClickable = true;
                    //item.SetCardsPos(true);
                    StartCoroutine(item.SetCardsPos(true, 0f));
                }

                foreach (var item in cardsPanel.AllowSequentialCards)
                {
                    //item.SetGaryColor(false);
                    item.IsClickable = true;
                    //item.SetCardsPos(false);
                    StartCoroutine(item.SetCardsPos(false, 0.5f));
                }
                foreach (var item in cardsPanel.DisallowSequentialCards)
                {
                    //item.SetGaryColor(true);
                    item.IsClickable = true;
                    //item.SetCardsPos(true);
                    StartCoroutine(item.SetCardsPos(true, 0f));
                }
            }
            else
            {
                foreach (var item in cardsPanel.AllowedCard)
                {
                    item.SetGaryColor(false);
                    item.IsClickable = true;
                }
                foreach (var item in cardsPanel.DisallowedCard)
                {
                    item.SetGaryColor(true);
                    item.IsClickable = false;
                }
            }

            if (cardsPanel.AllowedCard.Count > 0 && GetTotalPoints() <= 10) // cardsPanel.cards.Count == 2)
            {
                MultiPlayerGamePlayManager.instance.EnableUnoBtn();
            }
            else
            {
                MultiPlayerGamePlayManager.instance.DisableUnoBtn();
            }
        }
    }

    public void AddCard(Card c)
    {
        //Debug.Log("AddCard " + c.name+ "  "+this.gameObject.name);
        cardsPanel.cards.Add(c);
        c.transform.SetParent(cardsPanel.transform);
        if (isUserPlayer)
        {
            isUserClicked = true;
            c.onClick = OnCardClick;
            c.IsClickable = false;
        }
    }



    public void RemoveCard(Card c)
    {
        cardsPanel.cards.Remove(c);
        c.onClick = null;
        c.IsClickable = false;
    }

    int count = 0;
    public void OnCardClick(Card c)
    {
        Debug.Log("OnCardClick call rpc " + c.name);
        PV.RPC("RPC_OnCardClick", RpcTarget.All, c.name);
    }

    [PunRPC]
    public void RPC_OnCardClick(string cardName)
    {
        //Card c = GameObject.Find(cardName).GetComponent<Card>();
        GameObject obj = GameObject.Find(cardName);
        if (obj == null)
            Debug.LogError("Gameobject with the name " + cardName + " not found");

        if (obj.GetComponent<Card>() == null)
            Debug.LogError("Gameobject with the name " + cardName + " Dose not have a component Card");

        Card c = GameObject.Find(cardName).GetComponent<Card>();
        Debug.Log("PunRPC OnCardClick " + gameObject.name + "  " + c.name);
        if (c == null)
        {
            isUserClicked = false;
        }
        if (Timer)
        {
            //Debug.LogError("isDoubleCards = " + isDoubleCards + "  &  = isSequentialCards = " + isSequentialCards);
            if (isUserPlayer && (isDoubleCards || isSequentialCards))
            {
                //Debug.LogError("c.IsDouble = " + c.IsDouble);
                Debug.Log("OnCardClick if1");
                if (!c.IsDouble && !c.isSequential)
                {
                    isDoubleCards = false;
                    isSequentialCards = false;
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                    OnTurnEnd();
                }
                else if (c.isSequential)
                {
                    isDoubleCards = false;
                    Debug.LogError("isSequentialCards = " + isSequentialCards);
                    foreach (var items in cardsPanel.cards)
                    {
                        if (items.isSequential)
                        {
                            //if (wild2 && c.Type != CardType.Other)
                            //{
                            //    if(items.Type != CardType.Other)
                            //    {
                            //        items.IsClickable = false;
                            //        items.IsDouble = false;
                            //    }
                            //    else
                            //    {
                            //        items.IsClickable = true;
                            //    }
                            //}
                            //else
                            //{
                            items.IsClickable = true;
                            //}                            
                        }
                        else
                        {
                            items.IsDouble = false;
                            items.IsClickable = false;
                        }
                    }
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                }
                else
                {
                    Debug.LogError("isDoubleCards = " + isDoubleCards);
                    isSequentialCards = false;
                    foreach (var item in cardsPanel.cards)
                    {
                        if (item.IsDouble && (RemoveSpecialCases(c.name) == RemoveSpecialCases(item.name)))
                        {
                            Debug.LogError("c.name = " + c.name + "  & RemoveSpecialCases(item.name) = " + RemoveSpecialCases(item.name));
                            item.IsClickable = true;
                        }
                        else
                        {
                            item.isSequential = false;
                            item.IsClickable = false;
                        }
                    }
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                }

                if (isSequentialCards)
                {
                    count++;
                    Debug.LogError("count = " + count);
                    if (count > 2)
                    {
                        isSequentialCards = false;
                        count = 0;
                    }
                }
                else
                {
                    count++;
                    if (count >= 1)
                    {
                        isDoubleCards = false;
                        count = 0;
                    }
                }
            }
            else if (isUserPlayer && (MultiPlayerGamePlayManager.instance.isEven || MultiPlayerGamePlayManager.instance.isOdd))
            {
                //Debug.LogError("c.name = " + c.name);
                Debug.Log("OnCardClick if2");
                if (MultiPlayerGamePlayManager.instance.isEven)
                {
                    //Debug.LogError(" c.name = " + c.name + " & MultiPlayerGamePlayManager.instance.isEven = " + MultiPlayerGamePlayManager.instance.isEven);
                    Debug.Log("OnCardClick i2-1");
                    if (GetBestEvenCard_WithDouble(c.name))
                    {
                        isOddEvenDoubles = true;
                        count++;
                        if (count > 1)
                        {
                            isOddEvenDoubles = false;
                            count = 0;
                        }
                    }
                    else
                    {
                        isOddEvenDoubles = false;
                    }
                    Debug.LogError("isOddEvenDoubles = " + isOddEvenDoubles);
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                    if (!isOddEvenDoubles)
                        OnTurnEnd();

                    return;
                }
                else if (MultiPlayerGamePlayManager.instance.isOdd)
                {
                    //Debug.LogError(" c.name = " + c.name + "   & MultiPlayerGamePlayManager.instance.isOdd = " + MultiPlayerGamePlayManager.instance.isOdd);
                    Debug.Log("OnCardClick if2-2");
                    if (GetBestOddCard_WithDouble(c.name))
                    {
                        isOddEvenDoubles = true;
                        count++;
                        if (count > 1)
                        {
                            isOddEvenDoubles = false;
                            count = 0;
                        }
                    }
                    else
                    {
                        isOddEvenDoubles = false;
                    }
                    //Debug.LogError("isOddEvenDoubles = " + isOddEvenDoubles);
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                    if (!isOddEvenDoubles)
                        OnTurnEnd();

                    return;
                }
                else
                {
                    Debug.Log("OnCardClick if2-3");
                    isOddEvenDoubles = false;
                    MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                    OnTurnEnd();
                }
            }
            else
            {
                Debug.Log("OnCardClick if3");
                //Debug.LogError("xDDDDDDDDD  isDoubleCards = " + isDoubleCards + "  &  = isSequentialCards = " + isSequentialCards);
                MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
                if (isDoubleCards || isSequentialCards)
                {
                    return;
                }
                else if (isOddEvenDoubles)
                {
                    return;
                }
                else
                {
                    OnTurnEnd();
                }
                //if (!isDoubleCards || !isSequentialCards)
                //    OnTurnEnd();
            }
        }
    }

    public void OnCardAdd(Card c)
    {
        Debug.Log("OnCardAdd " + c.name);
        if (Timer)
        {
            MultiPlayerGamePlayManager.instance.PutCardToWastePile(c, this);
            OnTurnEnd();
        }
    }

    public void OnTurnEnd()
    {
        Debug.Log("OnTurnEnd");
        if (!choosingColor) Timer = false;
        cardsPanel.UpdatePos();
        foreach (var item in cardsPanel.cards)
        {
            //item.SetCardsPos(false);
            if (isUserPlayer)
            {
                StartCoroutine(item.SetCardsPos(true, 0f));
            }
            item.SetGaryColor(false);
        }
        GetTotalPoints();
        //GetListOfValues();
        //GetDoubleCard();

        MultiPlayerGamePlayManager.instance.oddEvenCount = 0;
    }

    public void ShowMessage(string message, bool playStarParticle = false)
    {
        messageLbl.text = message;
        messageLbl.GetComponent<Animator>().SetTrigger("show");
        if (playStarParticle)
        {
            starParticleSystem.gameObject.SetActive(true);
            starParticleSystem.Emit(30);
        }
    }

    public IEnumerator DoComputerTurn()
    {
        if (cardsPanel.AllowedCard.Count > 0)
        {
            StartCoroutine(ComputerTurnHasCard(0.25f));
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(1f, totalTimer * .3f));
            MultiPlayerGamePlayManager.instance.EnableDeckClick();
            MultiPlayerGamePlayManager.instance.OnDeckClick();

            if (cardsPanel.AllowedCard.Count > 0)
            {
                StartCoroutine(ComputerTurnHasCard(0.2f));
            }
        }
    }

    private IEnumerator ComputerTurnHasCard(float unoCoef)
    {
        bool unoClick = false;
        float unoPossibality = MultiPlayerGamePlayManager.instance.UnoProbability / 100f;
        //Debug.LogError("unoPossibality = " + unoPossibality);

        if (Random.value < unoPossibality && GetTotalPoints() <= 10)  //cardsPanel.cards.Count == 2)
        {
            yield return new WaitForSeconds(Random.Range(1f, totalTimer * unoCoef));
            MultiPlayerGamePlayManager.instance.OnUnoClick();
            Debug.LogError("GetTotalPoints() = " + GetTotalPoints());
            unoClick = true;
            yield return null;
        }

        //if (p.GetTotalPoints() <= 10 && !CurrentPlayer.isUserPlayer)   //(p.cardsPanel.cards.Count == 0)
        //{
        //    Debug.LogError("GetTotalPoints() = " + p.GetTotalPoints());
        //    Invoke("SetupGameOver", 2f);
        //    return;
        //}

        yield return new WaitForSeconds(Random.Range(1f, totalTimer * (unoClick ? unoCoef : unoCoef * 2)));
        //Debug.LogError("MultiPlayerGamePlayManager.instance.isEven = "+ MultiPlayerGamePlayManager.instance.isEven + "  & MultiPlayerGamePlayManager.instance.isOdd = " + MultiPlayerGamePlayManager.instance.isOdd);
        //Debug.LogError("isSequentialCards = " + isSequentialCards + " &  isDoubleCards = " + isDoubleCards);
        if (MultiPlayerGamePlayManager.instance.isEven)
        {
            OnCardClick(GetBestEvenCard());
        }
        else if (MultiPlayerGamePlayManager.instance.isOdd)
        {
            OnCardClick(GetBestOddCard());
        }
        else if (isDoubleCards && isSequentialCards)
        {
            doublelist.Sort((x, y) => y.CompareTo(x));
            int sum = doublelist.OrderByDescending(x => x).Take(2).Sum();
            int sum1 = seqCardList.OrderByDescending(x => x).Take(3).Sum();

            Debug.LogError("sum = " + sum + "  & sum1 = " + sum1);
            if (sum1 > sum)
            {
                isDoubleCards = false;
                for (int i = 0; i < sequentialList.Count; i++)
                {
                    if (i < 3)
                    {
                        if (i > 1)
                        {
                            isSequentialCards = false;
                        }
                        OnCardClick(PutBestSequentialCard());
                    }
                }
            }
            else
            {
                isSequentialCards = false;
                for (int i = 0; i < indexList.Count; i++)
                {
                    if (i < 2)
                    {
                        if (i > 0)
                        {
                            isDoubleCards = false;
                        }
                        OnCardClick(PutCard());
                    }
                }
            }
        }
        else if (isDoubleCards)
        {
            //Debug.LogError("isDoubleCards = " + isDoubleCards);
            for (int i = 0; i < indexList.Count; i++)
            {
                if (i < 2)
                {
                    if (i > 0)
                    {
                        isDoubleCards = false;
                    }
                    OnCardClick(PutCard());
                }
            }
        }
        else if (isSequentialCards)
        {
            //Debug.LogError("isSequentialCards = " + isSequentialCards);
            for (int i = 0; i < sequentialList.Count; i++)
            {
                if (i < 3)
                {
                    //Debug.LogError("sequentialList[] = " + sequentialList[i]);
                    if (i > 1)
                    {
                        isSequentialCards = false;
                    }
                    OnCardClick(PutBestSequentialCard());
                }
            }
        }
        else
        {
            OnCardClick(FindBestPutCard());
        }
    }

    public string RemoveSpecialCases(string cardName)
    {
        //Debug.LogError("cardName = " + cardName);
        string[] str = cardName.Split('_');
        //Debug.LogError("str[1] = " + str[1]);
        return str[1];
    }

    public bool GetBestEvenCard_WithDouble(string evenCardName)
    {
        List<Card> evenNum = cardsPanel.AllowEvenCards;
        if (evenNum.Count <= 1)
            return false;
        for (int i = 0; i < evenNum.Count; i++)
        {
            if (evenNum[i].name == evenCardName)
            {
                Debug.LogError(" iiiii evenCardName = " + evenCardName);
                evenNum.Remove(evenNum[i]);
            }
        }

        if (evenNum.Count > 0)
        {
            Debug.LogError("evenCardName = " + evenCardName);
            for (int i = 0; i < evenNum.Count; i++)
            {
                if (RemoveSpecialCases(evenNum[i].name) == RemoveSpecialCases(evenCardName))
                {
                    Debug.LogError("evenCardName = " + evenCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(evenNum[i].name));
                    evenNum[i].IsClickable = true;
                    return true;
                }
                else
                {
                    evenNum[i].IsClickable = false;
                }
            }
        }
        return false;
    }
    public bool GetBestOddCard_WithDouble(string oddCardName)
    {
        List<Card> oddNum = cardsPanel.AllowOddCards;
        if (oddNum.Count <= 1)
            return false;
        for (int i = 0; i < oddNum.Count; i++)
        {
            if (oddNum[i].name == oddCardName)
            {
                Debug.LogError(" iiiii oddCardName = " + oddCardName);
                oddNum.Remove(oddNum[i]);
            }
        }

        if (oddNum.Count > 0)
        {
            Debug.LogError("oddCardName = " + oddCardName);
            for (int i = 0; i < oddNum.Count; i++)
            {
                if (RemoveSpecialCases(oddNum[i].name) == RemoveSpecialCases(oddCardName))
                {
                    Debug.LogError("oddCardName = " + oddCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(oddNum[i].name));
                    oddNum[i].IsClickable = true;
                    return true;
                }
                else
                {
                    oddNum[i].IsClickable = false;
                }
            }
        }
        return false;
    }

    //public bool GetBestOddEvenCard_WithDouble(string evenCardName, bool isEven, bool isOdd)
    //{
    //    List<Card> evenOdd_Num = new List<Card>();
    //    Debug.LogError("isEven = " + isEven + "   isOdd = " + isOdd);
    //    if (isEven) {
    //        evenOdd_Num = cardsPanel.AllowEvenCards;
    //    } else if(isOdd) {
    //        evenOdd_Num = cardsPanel.AllowOddCards;
    //    }
    //    if (evenOdd_Num.Count < 0)
    //        return false;

    //    if (evenOdd_Num.Count > 1)
    //    {
    //        Debug.LogError("evenCardName = " + evenCardName);
    //        for (int i = 0; i < evenOdd_Num.Count; i++)
    //        {
    //            if(RemoveSpecialCases(evenOdd_Num[i].name) == RemoveSpecialCases(evenCardName))
    //            {
    //                Debug.LogError("evenCardName = " + evenCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(evenOdd_Num[i].name));
    //                evenOdd_Num[i].IsClickable = true;
    //                return true;
    //            }
    //            else
    //            {
    //                evenOdd_Num[i].IsClickable = false;
    //            }
    //        }
    //    }
    //    return false;
    //}

    public Card FindBestPutCard()
    {
        List<Card> allow = cardsPanel.AllowedCard;
        //allow.Sort((x, y) => y.Type.CompareTo(x.Type));
        allow.Sort((x, y) => y.point.CompareTo(x.point));

        return allow[0];
    }
    public List<int> sequentialNum, sequentialList;
    public List<string> cardName;

    public List<int> seqCardList = new List<int>();
    public List<string> seqCardNames = new List<string>();

    public List<Card> wildCards;

    bool wild1 = false, wild2 = false;
    int counter = 0;
    void GetSequentialsCards(List<int> cardsList, CardType type)
    {
        sequentialNum = new List<int>();
        cardName = new List<string>();
        sequentialList = new List<int>();
        List<Card> allow = cardsPanel.AllowedCard;
        if (cardsList.Count >= 2)
        {
            List<List<int>> sequenceNum = new List<List<int>>();
            IEnumerable<IEnumerable<int>> sequences = FindSequences(cardsList, 2);
            sequenceNum = (List<List<int>>)sequences;
            //foreach (var sequence in sequences)
            //{   //print results to consol
            //    Debug.LogError(sequence.Select(num => num.ToString()).Aggregate((a, b) => a + "," + b));
            //}
            for (int i = 0; i < sequenceNum.Count; i++)
            {
                for (int j = 0; j < sequenceNum[i].Count; j++)
                {
                    //Debug.LogError("sequenceNum[i][j] = " + sequenceNum[i][j]);
                    sequentialNum.Add(sequenceNum[i][j]);
                    cardName.Add(type + "_" + NumericToString(sequenceNum[i][j].ToString()));
                }
            }

            if (sequentialNum.Count > 1)
            {
                //Debug.LogError("Before  counter = " + counter + "   sequentialNum.Count = " + sequentialNum.Count);                
                if (sequentialNum.Count < 3)
                {
                    //Debug.LogError(" sequentialNum.Count < 3  = " + sequentialNum.Count);
                    foreach (var card in cardsPanel.cards)
                    {
                        if (card.Value == CardValue.Wild)
                        {
                            card.isSequential = true;
                            sequentialNum.Add(card.point);
                            cardName.Add(card.name);
                            isSequentialCards = true;
                            wild1 = true;
                            counter++;
                        }
                        else
                        {
                            if (counter < 1)
                            {
                                isSequentialCards = false;
                            }
                        }
                    }
                }
                else
                {
                    isSequentialCards = true;
                }
            }
            else
            {
                //Debug.LogError("   sequentialNum.Count = " + sequentialNum.Count);
                if (counter < 1)
                {
                    isSequentialCards = false;
                }
            }
            //Debug.LogError("isSequentialCards = " + isSequentialCards);
            if (isSequentialCards)
            {
                for (int i = 0; i < sequentialNum.Count; i++)
                {
                    seqCardList.Add(sequentialNum[i]);
                    seqCardNames.Add(cardName[i]);
                }
                //wild1 = false;
            }
            //Debug.LogError(" with 1 wild card isSequentialCards = " + isSequentialCards + " &  After counter = " + counter);
        }
        else
        {
            if (CheckWildCount() >= 2)
            {
                List<Card> allCards = cardsPanel.AllowedCard;
                List<Card> NumCards = new List<Card>();

                for (int i = 0; i < allCards.Count; i++)
                {
                    if (allCards[i].Type != CardType.Other)
                    {
                        NumCards.Add(allCards[i]);
                    }
                }
                if (!isUserPlayer)
                {
                    NumCards.Sort((x, y) => y.point.CompareTo(x.point));
                }
                for (int i = 0; i < NumCards.Count; i++)
                {
                    sequentialNum.Add(NumCards[i].point);
                    cardName.Add(NumCards[i].name);
                }
                for (int i = 0; i < wildCards.Count; i++)
                {
                    if (i < 2)
                    {
                        wildCards[i].isSequential = true;
                        sequentialNum.Add(wildCards[i].point);
                        cardName.Add(wildCards[i].name);
                    }
                }
                isSequentialCards = true;
                wild2 = true;
            }
            else
            {
                isSequentialCards = false;
            }
            //Debug.LogError("wild2 = " + wild2 + "   isSequentialCards = " + isSequentialCards);
            if (wild2)
            {
                isSequentialCards = true;
                for (int i = 0; i < sequentialNum.Count; i++)
                {
                    seqCardList.Add(sequentialNum[i]);
                    seqCardNames.Add(cardName[i]);
                }
                //wild2 = false;
            }
            else if (wild1)
            {
                isSequentialCards = true;
            }

            ////Debug.LogError(" with 2 wild card    isSequentialCards = " + isSequentialCards);
        }

        if (!isSequentialCards)
        {
            return;
        }

        for (int i = 0; i < seqCardNames.Count; i++)
        {
            for (int j = 0; j < cardsPanel.cards.Count; j++)
            {
                //Debug.LogError("sadge  cardName[i] = " + cardName[i]);
                if (seqCardNames[i] == cardsPanel.cards[j].name)
                {
                    //Debug.LogError("cardName[i] = " + cardName[i]);
                    Debug.LogError("cardsPanel.cards[j].name = " + cardsPanel.cards[j].name);
                    sequentialList.Add(cardsPanel.cards.IndexOf(cardsPanel.cards[j]));
                }
            }
        }

        for (int i = 0; i < sequentialList.Count; i++)
        {
            Debug.LogError("sequentialList[i] = " + sequentialList[i]);
            cardsPanel.cards[sequentialList[i]].isSequential = true;
        }
    }

    int CheckWildCount()
    {
        wildCards = new List<Card>();
        foreach (var wildCard in cardsPanel.cards)
        {
            if (wildCard.Value == CardValue.Wild)
            {
                wildCards.Add(wildCard);
            }
        }

        return wildCards.Count;
    }

    void CheckDoubleWilds()
    {
        List<Card> wildCards = new List<Card>();
        foreach (var wildCard in cardsPanel.cards)
        {
            if (wildCard.Value == CardValue.Wild)
            {
                wildCards.Add(wildCard);
            }
        }
        if (wildCards.Count >= 2)
        {
            List<Card> allCards = cardsPanel.AllowedCard;
            List<Card> NumCards = new List<Card>();

            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].Type != CardType.Other)
                {
                    NumCards.Add(allCards[i]);
                }
            }
            if (!isUserPlayer)
            {
                NumCards.Sort((x, y) => y.point.CompareTo(x.point));
            }
            for (int i = 0; i < NumCards.Count; i++)
            {
                sequentialNum.Add(NumCards[i].point);
                cardName.Add(NumCards[i].name);
            }
            for (int i = 0; i < wildCards.Count; i++)
            {
                if (i < 2)
                {
                    wildCards[i].isSequential = true;
                    sequentialNum.Add(wildCards[i].point);
                    cardName.Add(wildCards[i].name);
                }
            }
            isSequentialCards = true;
            wild2 = true;
        }
        else
        {
            isSequentialCards = false;
        }
        Debug.LogError("wild2 = " + wild2 + "   isSequentialCards = " + isSequentialCards);
        if (wild2)
        {
            isSequentialCards = true;
            for (int i = 0; i < sequentialNum.Count; i++)
            {
                seqCardList.Add(sequentialNum[i]);
                seqCardNames.Add(cardName[i]);
            }
            //wild2 = false;
        }
        else if (wild1)
        {
            isSequentialCards = true;
        }

        Debug.LogError(" with 2 wild card    isSequentialCards = " + isSequentialCards);

        if (!isSequentialCards)
        {
            return;
        }

        for (int i = 0; i < seqCardNames.Count; i++)
        {
            for (int j = 0; j < cardsPanel.cards.Count; j++)
            {
                //Debug.LogError("sadge  cardName[i] = " + cardName[i]);
                if (seqCardNames[i] == cardsPanel.cards[j].name)
                {
                    //Debug.LogError("cardName[i] = " + cardName[i]);
                    Debug.LogError("cardsPanel.cards[j].name = " + cardsPanel.cards[j].name);
                    sequentialList.Add(cardsPanel.cards.IndexOf(cardsPanel.cards[j]));
                }
            }
        }

        for (int i = 0; i < sequentialList.Count; i++)
        {
            Debug.LogError("sequentialList[i] = " + sequentialList[i]);
            cardsPanel.cards[sequentialList[i]].isSequential = true;
        }
    }

    private IEnumerable<IEnumerable<int>> FindSequences(IEnumerable<int> items, int minSequenceLength)
    {
        //Convert item list to dictionary
        var itemDict = new Dictionary<int, int>();
        foreach (int val in items)
        {
            itemDict[val] = val;
        }
        var allSequences = new List<List<int>>();
        //for each val in items, find longest sequence including that value
        foreach (var item in items)
        {
            var sequence = FindLongestSequenceIncludingValue(itemDict, item);
            allSequences.Add(sequence);
            //remove items from dict to prevent duplicate sequences
            sequence.ForEach(i => itemDict.Remove(i));
        }
        //return only sequences longer than 3
        return allSequences.Where(sequence => sequence.Count >= minSequenceLength).ToList();
    }

    //Find sequence around start param value
    private List<int> FindLongestSequenceIncludingValue(Dictionary<int, int> itemDict, int value)
    {
        var result = new List<int>();
        //check if num exists in dictionary
        if (!itemDict.ContainsKey(value))
            return result;

        //initialize sequence list
        result.Add(value);

        //find values greater than starting value
        //and add to end of sequence
        var indexUp = value + 1;
        while (itemDict.ContainsKey(indexUp))
        {
            result.Add(itemDict[indexUp]);
            indexUp++;
        }

        //find values lower than starting value 
        //and add to start of sequence
        var indexDown = value - 1;
        while (itemDict.ContainsKey(indexDown))
        {
            result.Insert(0, itemDict[indexDown]);
            indexDown--;
        }
        return result;
    }

    public Card GetBestOddCard()
    {
        List<Card> oddNum = cardsPanel.AllowOddCards;
        if (oddNum.Count <= 0)
            return null;

        oddNum.Sort((x, y) => y.point.CompareTo(x.point));
        string cardName = oddNum[0].name;
        List<Card> dubOddCards = new List<Card>();
        for (int i = 1; i < oddNum.Count; i++)
        {
            if (cardName == oddNum[i].name)
            {
                dubOddCards.Add(oddNum[i]);
                Debug.LogError("oddNum[i].name = " + oddNum[i].name);
            }
        }
        return oddNum[0];
    }

    public Card GetBestEvenCard()
    {
        List<Card> evenNum = cardsPanel.AllowEvenCards;
        if (evenNum.Count <= 0)
            return null;

        evenNum.Sort((x, y) => y.point.CompareTo(x.point));
        if (evenNum[0].point <= 1)
            return null;

        string cardName = evenNum[0].name;
        List<Card> dubEvenCards = new List<Card>();
        for (int i = 1; i < evenNum.Count; i++)
        {
            if (cardName == evenNum[i].name)
            {
                dubEvenCards.Add(evenNum[i]);
                Debug.LogError("evenNum[i].name = " + evenNum[i].name);
            }
        }
        return evenNum[0];
    }

    public List<int> indexList, doublelist, sameColorCardsList;
    public bool isUnSequence = false;

    public void GetDoubleCard()
    {
        Debug.Log("AI " + gameObject.name);
        if (MultiPlayerGamePlayManager.instance.isEven || MultiPlayerGamePlayManager.instance.isEven)
        {
            return;
        }

        for (int i = 0; i < cardsPanel.cards.Count; i++)
        {
            cardsPanel.cards[i].IsDouble = false;
            cardsPanel.cards[i].isSequential = false;
        }
        isDoubleCards = false;
        isSequentialCards = false;

        sequentialNum = new List<int>();
        cardName = new List<string>();
        sequentialList = new List<int>();

        wildCards = new List<Card>();
        seqCardList = new List<int>();
        seqCardNames = new List<string>();

        counter = 0;
        unseqCount = 0;

        indexList = new List<int>();
        doublelist = new List<int>();
        sameColorCardsList = new List<int>();
        //Debug.LogError("GetDoubleCard() ... .. . Player Name = "+ gameObject.name);                       

        List<Card> allows = cardsPanel.AllowedCard;
        var zippedLists = allows.Zip(Enumerable.Range(0, allows.Count), (s, i) => new { s.Type, i });
        var finalResults = from a in zippedLists
                           group a by a.Type into g
                           select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };

        foreach (var item in finalResults)
        {
            if (item.key != CardType.Other)
            {
                if (item.result.count >= 1)
                {
                    //Debug.LogError("xDDDDDDD   item.key = " + item.key);
                    //Debug.LogError("xDDDDDDDD     item.result.count = " + item.result.count);

                    sameColorCardsList = new List<int>();//item.result.list;
                    for (int i = 0; i < cardsPanel.cards.Count; i++)
                    {
                        if (cardsPanel.cards[i].Type == item.key)
                        {
                            if (cardsPanel.cards[i].cardValue > 0)
                                sameColorCardsList.Add(cardsPanel.cards[i].cardValue);

                            //Debug.LogError("cardsPanel.cards[i].cardValue = " + cardsPanel.cards[i].cardValue);
                        }
                    }
                    if (sameColorCardsList.Count >= 1)
                    {
                        GetSequentialsCards(sameColorCardsList, item.key);
                        CheckUnSeqCardsWithWild(sameColorCardsList, item.key);
                    }
                }
            }
        }

        wild1 = false;
        wild2 = false;

        var zippedList = cardVal.Zip(Enumerable.Range(0, cardVal.Count), (s, i) => new { s, i });
        var finalResult = from a in zippedList
                          group a by a.s into g
                          select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };
        foreach (var item in finalResult)
        {
            if (item.result.count > 1 && item.result.count <= 3 && item.key > 0)
            {
                //Debug.LogError("item.key = " + item.key);
                //Debug.LogError("item.result.count = " + item.result.count);

                List<int> list = item.result.list;
                for (int i = 0; i < list.Count; i++)
                {
                    doublelist.Add(item.key);

                    cardsPanel.cards[list[i]].IsDouble = true;
                    ////Debug.LogError("list[i] = " + list[i]);
                    if (list.Count > 2)
                    {
                        if (list[0] == list[2])
                        {
                            cardsPanel.cards[list[i]].IsDouble = false;
                            list.RemoveAt(2);
                        }
                    }
                    indexList.Add(list[i]);
                }
            }
        }

        if (indexList.Count > 0)
        {
            isDoubleCards = true;
        }
    }

    void CheckUnSeqCardsWithWild(List<int> list, CardType type)
    {
        int count = 0;
        List<Card> allows = cardsPanel.AllowedCard;
        if (CheckWildCount() >= 1)
        {
            //var zippedLists = allows.Zip(Enumerable.Range(0, allows.Count), (s, i) => new { s.Type, i });
            //var finalResults = from a in zippedLists
            //                    group a by a.Type into g
            //                    select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };

            //foreach (var item in finalResults)
            //{
            //    if (item.key != CardType.Other)
            //    {
            //        if (item.result.count > 1)
            //        {
            //            Debug.LogError("xDD  item.key = " + item.key);
            //            Debug.LogError("xDD  item.result.count = " + item.result.count);

            //            sameColorCardsList = new List<int>();
            //            for (int i = 0; i < cardsPanel.cards.Count; i++)
            //            {
            //                if (cardsPanel.cards[i].Type == item.key)
            //                {
            //                    sameColorCardsList.Add(cardsPanel.cards[i].cardValue);
            //                }
            //            }

            if (list.Count > 1)
            {
                MissingNum(sameColorCardsList, type);
                if (isUnSequence)
                {
                    for (int i = 0; i < wildCards.Count; i++)
                    {
                        if (i < 2)
                        {
                            wildCards[i].isSequential = true;
                            seqCardList.Add(wildCards[i].point);
                            seqCardNames.Add(wildCards[i].name);
                            isSequentialCards = true;
                            count++;
                        }
                        else
                        {
                            if (count < 1)
                                isSequentialCards = false;
                        }
                    }
                }
            }
            //        }
            //    }
            //}
            //if (isUnSequence)
            //{
            //    for (int i = 0; i < wildCards.Count; i++)
            //    {
            //        if (i < 2)
            //        {
            //            wildCards[i].isSequential = true;
            //            seqCardList.Add(wildCards[i].point);
            //            seqCardNames.Add(wildCards[i].name);
            //            isSequentialCards = true;
            //            count++;
            //        }
            //        else
            //        {
            //            if(count < 1)
            //                isSequentialCards = false;
            //        }                    
            //    }
            //}
        }
        //Debug.LogError("CheckUnSeqCardsWithWild() isSequentialCards = "+ isSequentialCards);
        if (!isSequentialCards)
        {
            return;
        }

        for (int i = 0; i < seqCardNames.Count; i++)
        {
            for (int j = 0; j < cardsPanel.cards.Count; j++)
            {
                //Debug.LogError("sadge  cardName[i] = " + cardName[i]);
                if (seqCardNames[i] == cardsPanel.cards[j].name)
                {
                    //Debug.LogError("cardName[i] = " + cardName[i]);
                    Debug.LogError("cardsPanel.cards[j].name = " + cardsPanel.cards[j].name);
                    sequentialList.Add(cardsPanel.cards.IndexOf(cardsPanel.cards[j]));
                }
            }
        }

        for (int i = 0; i < sequentialList.Count; i++)
        {
            //Debug.LogError("sequentialList[i] = " + sequentialList[i]);
            cardsPanel.cards[sequentialList[i]].isSequential = true;
        }
    }

    int unseqCount = 0;
    void MissingNum(List<int> myList, CardType type)
    {
        isUnSequence = false;
        myList.Sort();
        int a, b;
        List<int> myList2 = new List<int>();
        List<int> remaining = new List<int>();
        for (int i = 0; i < myList.Count; i++)
        {
            a = myList[i];
            if (i < myList.Count - 1)
            {
                b = myList[i + 1];
            }
            else
            {
                break;
            }

            myList2 = Enumerable.Range(a, b - a + 1).ToList();
            remaining = myList2.Except(myList).ToList();
            if (remaining.Count == 1)
            {
                Debug.LogError("First Remaing Num = " + remaining[0]);
                seqCardList.Add(a);
                seqCardNames.Add(type + "_" + NumericToString(a.ToString()));
                seqCardList.Add(b);
                seqCardNames.Add(type + "_" + NumericToString(b.ToString()));
                isUnSequence = true;
                unseqCount++;
            }
        }
        if (unseqCount < 1)
        {
            isUnSequence = false;
        }
        Debug.LogError("MissingNum()    isUnSequence   = " + isUnSequence);
    }

    public Card PutCard()
    {
        List<Card> doubleAllow = cardsPanel.AllowDoubleCards;
        doubleAllow.Sort((x, y) => y.point.CompareTo(x.point));

        doublelist.Sort((x, y) => y.CompareTo(x));
        int sum = doublelist.OrderByDescending(x => x).Take(2).Sum();

        List<int> tempList = new List<int>();
        for (int i = 0; i < cardsPanel.cards.Count; i++)
        {
            tempList.Add(cardsPanel.cards[i].point);
        }

        if (sum < tempList.Max())
        {
            //Debug.LogError("double sum = "+ sum + "  & tempList.Max() = " + tempList.Max());
            isDoubleCards = false;
            OnCardClick(FindBestPutCard());
            return null;
        }
        else
        {
            return doubleAllow[0];
        }
    }

    public Card PutBestSequentialCard()
    {
        List<Card> doubleAllow = cardsPanel.AllowSequentialCards;
        doubleAllow.Sort((x, y) => y.point.CompareTo(x.point));
        //for (int i = 0; i < doubleAllow.Count; i++)
        //{
        //    Debug.LogError("doubleAllow[" + i + "] = " + doubleAllow[i]);
        //}
        //Debug.LogError("doubleAllow[" + 0 + "] = " + doubleAllow[0]);
        return doubleAllow[0];
    }

    public void ChooseBestColor()
    {
        CardType temp = CardType.Other;
        if (cardsPanel.cards.Count == 1)
        {
            temp = cardsPanel.cards[0].Type;
        }
        else
        {
            int max = 1;
            for (int i = 0; i < 5; i++)
            {
                if (cardsPanel.GetCount((CardType)i) > max)
                {
                    max = cardsPanel.GetCount((CardType)i);
                    temp = (CardType)i;
                }
            }
        }

        if (temp == CardType.Other)
        {
            MultiPlayerGamePlayManager.instance.SelectColor(Random.Range(1, 5));
        }
        else
        {
            if (Random.value < 0.7f)
                MultiPlayerGamePlayManager.instance.SelectColor((int)temp);
            else
                MultiPlayerGamePlayManager.instance.SelectColor(Random.Range(1, 5));
        }
    }

    public void GetListOfValues()
    {
        cardVal = new List<int>();
        for (int i = 0; i < cardsPanel.cards.Count; i++)
        {
            //if (cardsPanel.cards[i].Type != CardType.Other)
            //{
            cardVal.Add(cardsPanel.cards[i].cardValue);
            //indexList.Add(i);
            //}
        }
        //foreach(var items in cardsPanel.cards)
        //{
        //    Debug.LogError("item.name = "+ items.name);
        //}
    }

    public int GetTotalPoints()
    {
        int total = 0;
        foreach (var c in cardsPanel.cards)
        {
            total += c.point;
        }
        if (pointTxt != null)
            pointTxt.text = total.ToString();
        return total;
    }

    private String NumericToString(String Number)
    {
        int _Number = Convert.ToInt32(Number);
        String name = "";
        switch (_Number)
        {
            case 1:
                name = "One";
                break;
            case 2:
                name = "Two";
                break;
            case 3:
                name = "Three";
                break;
            case 4:
                name = "Four";
                break;
            case 5:
                name = "Five";
                break;
            case 6:
                name = "Six";
                break;
            case 7:
                name = "Seven";
                break;
            case 8:
                name = "Eight";
                break;
            case 9:
                name = "Nine";
                break;
            case 0:
                name = "Zero";
                break;
        }
        return name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Debug.Log("Is Writing", this.gameObject);
            stream.SendNext(playerName);
            stream.SendNext(PlayerAvatarIndex);
        }
        else
        {
            //PlayerLastAnswer.text = (string)stream.ReceiveNext();
            //lightState = (bool)stream.ReceiveNext();
            //TurnLight.color = lightState ? Color.green : Color.red;
            //Debug.Log("Is Reading", this.gameObject);
            playerName = (string)stream.ReceiveNext();
            PlayerAvatarIndex = (int)stream.ReceiveNext();
            avatarName.text = playerName;
            avatarName.GetComponent<EllipsisText>().UpdateText();
            avatarImage.sprite = Resources.Load<Sprite>("Avatar/" + PlayerAvatarIndex);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //throw new NotImplementedException();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        string propertiesKey = PV.Owner.NickName;
        //Dictionary<string, string> propertiesValue;

        if (propertiesThatChanged.ContainsKey(propertiesKey))
        {
            //propertiesValue = (Dictionary<string, string>)propertiesThatChanged[propertiesKey];

            //foreach (KeyValuePair<string, string> pair in propertiesValue)
            //{
            Debug.Log("punplayer OnRoomPropertiesUpdate key=" + propertiesKey +
        "value=" + (string)propertiesThatChanged[propertiesKey]);
            MultiPlayerGamePlayManager.instance.PlayerSharedDatas[PV.Owner.ActorNumber - 1].CardsString
                = (string)propertiesThatChanged[propertiesKey];
            //}

        }
        //for (int i = 0; i < PlayerSharedDatas.Count; i++)
        //{
        //    string str1 = Launcher.instance.GetRoomCustomProperty(PlayerSharedDatas[i].PhotonName);

        //    if ((str1 != null) && (PlayerSharedDatas[i].PhotonName != ""))
        //    {
        //        Debug.Log(PlayerSharedDatas[i].PhotonName + "=" + str1);
        //        //PlayerSharedDatas[i].CardsString = str1;
        //    }
        //}
        //if (propertiesThatChanged.ContainsKey(propertiesKey))
        //{
        //    if (propertiesThatChanged[propertiesKey] == null)
        //    {
        //        propertiesValue = new Dictionary<int, float>();
        //        return;
        //    }

        //    propertiesValue = (Dictionary<int, float>)propertiesThatChanged[propertiesKey];

        //    foreach (KeyValuePair<int, float> pair in propertiesValue)
        //    {
        //        SetBlockHeightRemote(pair.Key, pair.Value);
        //    }
        //}
    }


}
