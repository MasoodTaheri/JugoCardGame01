using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public List<int> list;
    public List<int> indexList;

    //void Start()
    //{
    //    indexList = new List<int>();
    //    var q = list.GroupBy(x => x)
    //                .Select((g, index) => new { Value = g.Key, Count = g.Count(), index })
    //                .OrderByDescending(x => x.Count);

    //    foreach (var x in q)
    //    {
    //        Debug.LogError("Value: " + x.Value + "   Count: " + x.Count + "  & index : " + x.index);

    //        //if (x.Count > 1 && x.Count <= 3)// && x.Value >= 3)
    //        //{
    //        //    Debug.LogError(" 1 -- 3   Value: " + x.Value);
    //        //    int i = list.IndexOf(x.Value);
    //        //    indexList.Add(i);
    //        //}
    //    }
    //}

    //void Start()
    //{
    //    //var AS = new[] { "red", "Grey", "blue", "Grey", "red" };
    //    var zippedList = list.Zip(Enumerable.Range(0, list.Count), (s, i) => new { s, i });
    //    var finalResult = from a in zippedList
    //                        group a by a.s into g
    //                        select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };

    //    foreach (var item in finalResult)
    //    {
    //        Debug.LogError("item.key = " + item.key);
    //        Debug.LogError("item.result.count = " + item.result.count);

    //        List<int> list = item.result.list;
    //        for(int i = 0; i < list.Count; i++)
    //        {
    //            Debug.LogError("list[i] = " + list[i]);
    //        }
    //    }
    //}


    //public List<int> ints = new List<int>{ 6, 2, 5, 2, 99, 55 };
    //public List<int> table;

    //void Start()
    //{
    //    table = ints.OrderByDescending(x => x).ToList();
    //    foreach (var item in table)
    //    {
    //        Debug.LogError(item);
    //    }
    //}

    public List<int> myList;
    public List<int> getList;

    private void Start()
    {
        ////myList = new List<int>() { 1, 2, 3, 5, 8, 9 };
        //myList.Sort();
        //Debug.LogError("Numbers... ");
        //foreach (int val in myList)
        //{
        //    Debug.LogError(val);
        //}
        //int a = myList.OrderBy(x => x).First();
        //int b = myList.OrderBy(x => x).Last();
        //List<int> myList2 = Enumerable.Range(a, b - a + 1).ToList();
        //List<int> remaining = myList2.Except(myList).ToList();
        //Debug.LogError("Remaining numbers... ");
        //foreach (int res in remaining)
        //{
        //    Debug.LogError(res);
        //}


        //List<int> data = new List<int> { 1, 2, 1, 2, 3, 1, 2, 3, 4, 0, 1, 4, 5, 6 };

        //int groupId = 0;
        //var groups = data.Select
        //                 ((item, index)
        //                  => new
        //                   {
        //                       Item = item
        //                     ,
        //                       Group = index > 0 && item <= data[index - 1] ? ++groupId : groupId
        //                   }
        //                 );

        //List<List<int>> GetList = groups.GroupBy(g => g.Group)
        //                             .Select(x => x.Select(y => y.Item).ToList())
        //                             .ToList();

        //for(int i = 0; i < GetList.Count; i++)
        //{
        //    Debug.LogError("xDDDDDDDDDDDDDDDDD    GetList[" + i + "].Count = " + GetList[i].Count);
        //    for (int j = 0; j < GetList[i].Count; j++)
        //    {
        //        Debug.LogError("GetList["+i+"]"+"["+j+"] = "+ GetList[i][j]);
        //    }
        //}


        //List<int> data = new List<int> { 1, 2, 1, 1, 2, 3, 3, 4, 5, 1, 2, 3, 4, 0, 1, 4, 5, 6 };
        //List<List<int>> resultLists = new List<List<int>>();
        //int last = 0;
        //int count = 0;

        //var res = data.Where((p, i) =>
        //{
        //    if (i > 0)
        //    {
        //        if (p > last && p != last)
        //        {
        //            resultLists[count].Add(p);
        //        }
        //        else
        //        {
        //            count++;
        //            resultLists.Add(new List<int>());
        //            resultLists[count].Add(p);
        //        }
        //    }
        //    else
        //    {
        //        resultLists.Add(new List<int>());
        //        resultLists[count].Add(p);
        //    }
        //    last = p;
        //    return true;
        //}).ToList();

        //for (int i = 0; i < resultLists.Count; i++)
        //{
        //    Debug.LogError("xDDDDDDDDDDDDDDDDD    resultLists[" + i + "].Count = " + resultLists[i].Count);
        //    for (int j = 0; j < resultLists[i].Count; j++)
        //    {
        //        Debug.LogError("resultLists[" + i + "]" + "[" + j + "] = " + resultLists[i][j]);
        //    }
        //}


        //var getList = new List<int> { 21, 4, 7, 9, 12, 22, 17, 8, 2, 20, 23 };
        //foreach (var subsequence in list
        //    .OrderBy(i => i)
        //    .Distinct()
        //    .DetectSequenceWhere((first, second) => first + 1 == second)
        //    .Where(seq => seq.Count() >= 3))
        //{
        //    Debug.LogError("Found subsequence {0}", string.Join(", ", subsequence.Select(i => i.ToString()).ToArray()));
        //}


        //var items = new[] { -1, 0, 1, 21, -2, 4, 7, 9, 12, 22, 17, 8, 2, 20, 23 };
        //IEnumerable<IEnumerable<int>> sequences = ConsecutiveSequences(items, 3);

        //foreach (var sequence in sequences)
        //{   //print results to consol
        //    Debug.LogError(sequence.Select(num => num.ToString()).Aggregate((a, b) => a + "," + b));
        //}

        //CheckMissingNum();
        Main();
    }

    public void MissingNum()
    {
        
    }

    int findMissing(int[] array, int n1)
    {
        int low = 0, high = n1 - 1;
        int mid1;
        while (high > low)
        {
            mid1 = low + (high - low) / 2;
            // Verify if middle element is consistent
            if (array[mid1] - mid1 == array[0])
            {
                // Here, no inconsistency till middle elements
                // When missing element is just after
                // the middle element
                if (array[mid1 + 1] - array[mid1] > 1)
                    return array[mid1] + 1;
                else
                {
                    // Go right
                    low = mid1 + 1;
                }
            }
            else
            {
                // Here inconsistency found
                // When missing element is just before
                // the middle element
                if (array[mid1] - array[mid1 - 1] > 1)
                    return array[mid1] - 1;
                else
                {
                    // Go left
                    high = mid1 - 1;
                }
            }
        }
        // Here, no missing element found
        return -1;
    }
    // Driver code
    int Main()
    {
        int[] array = { 1, 2,  5, 8, 9, 11};
        Debug.LogError("array.Length = " + array.Length);
        Debug.LogError("array[0] = " + array[0]);
        int n1 = array.Length / array[0];
        Debug.LogError(findMissing(array, n1));

        return n1;        
    }

    public void CheckMissingNum()
    {
        //List<int> myList = new List<int>() { 1, 2, 3, 5, 8, 9 };
        Console.WriteLine("Numbers... ");
        foreach (int val in myList)
        {
            Console.WriteLine(val);
        }
        int a = myList.OrderBy(x => x).First();
        int b = myList.OrderBy(x => x).Last();
        List<int> myList2 = Enumerable.Range(a, b - a + 1).ToList();
        List<int> remaining = myList2.Except(myList).ToList();
        Debug.LogError("Remaining numbers... ");
        foreach (int res in remaining)
        {
            Debug.LogError(res);
        }
    }

    //private IEnumerable<IEnumerable<int>> ConsecutiveSequences(this IEnumerable<int> input, int minLength = 3)
    //{
    //    int order = 0;
    //    var inorder = new SortedSet<int>(input);
    //    return from item in new[] { new { order = 0, val = inorder.First() } }
    //               .Concat(
    //                 inorder.Zip(inorder.Skip(1), (x, val) =>
    //                         new { order = x + 1 == val ? order : ++order, val }))
    //           group item.val by item.order into list
    //           where list.Count() >= minLength
    //           select list;
    //}

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

}

//public static class SequenceDetector
//{
//    public static IEnumerable<IEnumerable<T>> DetectSequenceWhere<T>(this IEnumerable<T> sequence, Func<T, T, bool> inSequenceSelector)
//    {
//        List<T> subsequence = null;
//        // We can only have a sequence with 2 or more items
//        T last = sequence.FirstOrDefault();
//        foreach (var item in sequence.Skip(1))
//        {
//            if (inSequenceSelector(last, item))
//            {
//                // These form part of a sequence
//                if (subsequence == null)
//                {
//                    subsequence = new List<T>();
//                    subsequence.Add(last);
//                }
//                subsequence.Add(item);
//            }
//            else if (subsequence != null)
//            {
//                // We have a previous seq to return
//                yield return subsequence;
//                subsequence = null;
//            }
//            last = item;
//        }
//        if (subsequence != null)
//        {
//            // Return any trailing seq
//            yield return subsequence;
//        }
//    }
//}






//using System;
//using System.CodeDom.Compiler;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;
//using Random = UnityEngine.Random;
////using UnityEngine.UIElements;

//public class Player : MonoBehaviour
//{
//    public GameObject CardPanelBG;
//    public PlayerCards cardsPanel;
//    public string playerName;
//    public bool isUserPlayer, isUserClicked;
//    public Image avatarImage;
//    public Text avatarName;
//    public Text messageLbl;
//    public ParticleSystem starParticleSystem;
//    public Image timerImage;
//    public GameObject timerOjbect;
//    public Text pointTxt;

//    public List<int> cardVal;

//    private float totalTimer = 15f;

//    [HideInInspector]
//    public bool pickFromDeck, unoClicked, choosingColor;
//    [HideInInspector]
//    public bool isInRoom = true;

//    public bool isDoubleCards, isSequentialCards, isOddEvenDoubles;

//    void Start()
//    {
//        Timer = false;
//    }

//    public void SetAvatarProfile(AvatarProfile p)
//    {
//        playerName = p.avatarName;
//        if (avatarName != null)
//        {
//            avatarName.text = p.avatarName;
//            avatarName.GetComponent<EllipsisText>().UpdateText();
//        }
//        if (avatarImage != null)
//            avatarImage.sprite = Resources.Load<Sprite>("Avatar/" + p.avatarIndex);
//    }

//    public bool Timer
//    {
//        get
//        {
//            return timerOjbect.activeInHierarchy;
//        }
//        set
//        {
//            CancelInvoke("UpdateTimer");
//            timerOjbect.SetActive(value);
//            if (value)
//            {
//                timerImage.fillAmount = 1f;
//                InvokeRepeating("UpdateTimer", 0f, .1f);
//            }
//            else
//            {
//                timerImage.fillAmount = 0f;
//            }
//        }
//    }

//    void UpdateTimer()
//    {
//        timerImage.fillAmount -= 0.1f / totalTimer;
//        if (timerImage.fillAmount <= 0)
//        {
//            //if (choosingColor)
//            //{
//            //    if (isUserPlayer)
//            //    {
//            //        GamePlayManager.instance.colorChoose.HidePopup();
//            //    }
//            //    ChooseBestColor();
//            //}
//            if (GamePlayManager.instance.IsDeckArrow)
//            {
//                GamePlayManager.instance.OnDeckClick();
//            }
//            else if (cardsPanel.AllowedCard.Count > 0)
//            {
//                isUserClicked = false;
//                OnCardClick(FindBestPutCard());
//            }
//            //else if (cardsPanel.AllowOddCards.Count > 0)
//            //{
//            //    isUserClicked = false;
//            //    OnCardClick(GetBestOddCard());
//            //}
//            //else if (cardsPanel.AllowEvenCards.Count > 0)
//            //{
//            //    isUserClicked = false;
//            //    OnCardClick(GetBestEvenCard());
//            //}
//            else
//            {
//                OnTurnEnd();
//            }
//        }
//    }
    
//    public void OnTurn()
//    {
//        unoClicked = false;
//        pickFromDeck = false;
//        Timer = true;
//        GetListOfValues();
//        if (GamePlayManager.instance.isEven || GamePlayManager.instance.isOdd)
//        {
//            GamePlayManager.instance.turnCount++;
//            if (GamePlayManager.instance.turnCount > 3)
//            {
//                GamePlayManager.instance.isEven = false;
//                GamePlayManager.instance.isOdd = false;
//                GamePlayManager.instance.turnCount = 0;
//            }
//            //Debug.LogError("turnCount = "+ GamePlayManager.instance.turnCount);
//            //Debug.LogError("GamePlayManager.instance.isEven = " + GamePlayManager.instance.isEven + "  & GamePlayManager.instance.isOdd = " + GamePlayManager.instance.isOdd);
//        }
//        if (!GamePlayManager.instance.isEven && !GamePlayManager.instance.isOdd)
//        {
//            foreach (var card in cardsPanel.cards)
//            {
//                card.isSequential = false;
//                card.IsDouble = false;
//            }
//            isSequentialCards = false;
//            isDoubleCards = false;

//            GetSequentialCards();
//            GetDoubleCard();            
//        }

//        if (isUserPlayer)
//        {
//            UpdateCardColor();
//            if (cardsPanel.AllowedCard.Count == 0)
//            {
//                GamePlayManager.instance.EnableDeckClick();
//            }
//        }
//        else
//        {
//            StartCoroutine(DoComputerTurn());
//        }
//    }

//    public void UpdateCardColor()
//    {       
//        if (isUserPlayer)
//        {
//            foreach (var item in cardsPanel.cards)
//            {
//                //item.SetCardsPos(true);
//                StartCoroutine(item.SetCardsPos(true, 0f));
//            }
//            if (GamePlayManager.instance.isEven)
//            {
//                Debug.LogError("Even Num = "+ GamePlayManager.instance.isEven);
//                foreach (var item in cardsPanel.AllowEvenCards)
//                {
//                    item.SetGaryColor(false);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(false);
//                    StartCoroutine(item.SetCardsPos(false, 0.5f));
//                }
//                foreach (var item in cardsPanel.DisallowedEvenCard)
//                {
//                    item.SetGaryColor(true);
//                    item.IsClickable = false;
//                    //item.SetCardsPos(true);
//                    StartCoroutine(item.SetCardsPos(true, 0f));
//                }
//            }
//            else if (GamePlayManager.instance.isOdd)
//            {
//                Debug.LogError("Odd Num = " + GamePlayManager.instance.isOdd);
//                foreach (var item in cardsPanel.AllowOddCards)
//                {
//                    item.SetGaryColor(false);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(false);
//                    StartCoroutine(item.SetCardsPos(false, 0.5f));
//                }
//                foreach (var item in cardsPanel.DisallowedOddCard)
//                {
//                    item.SetGaryColor(true);
//                    item.IsClickable = false;
//                    //item.SetCardsPos(true);
//                    StartCoroutine(item.SetCardsPos(true, 0f));
//                }
//            }
//            else if (isDoubleCards || isSequentialCards)
//            {
//                Debug.LogError("UpdateCardColor()    isDoubleCards = " + isDoubleCards + "  & isSequentialCards = " + isSequentialCards);
//                foreach (var item in cardsPanel.AllowDoubleCards)
//                {
//                    Debug.LogError("item = "+ item.name);
//                    //item.SetGaryColor(false);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(false);
//                    StartCoroutine(item.SetCardsPos(false, 0.5f));
//                }
//                foreach (var item in cardsPanel.DisallowDoubleCards)
//                {
//                    //item.SetGaryColor(true);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(true);
//                    StartCoroutine(item.SetCardsPos(true, 0f));
//                }

//                foreach (var item in cardsPanel.AllowSequentialCards)
//                {
//                    //item.SetGaryColor(false);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(false);
//                    StartCoroutine(item.SetCardsPos(false, 0.5f));
//                }
//                foreach (var item in cardsPanel.DisallowSequentialCards)
//                {
//                    //item.SetGaryColor(true);
//                    item.IsClickable = true;
//                    //item.SetCardsPos(true);
//                    StartCoroutine(item.SetCardsPos(true, 0f));
//                }
//            }
//            else
//            {
//                foreach (var item in cardsPanel.AllowedCard)
//                {
//                    item.SetGaryColor(false);
//                    item.IsClickable = true;
//                }
//                foreach (var item in cardsPanel.DisallowedCard)
//                {
//                    item.SetGaryColor(true);
//                    item.IsClickable = false;
//                }
//            }

//            if (cardsPanel.AllowedCard.Count > 0 && /*GetTotalPoints() <= 10) */ cardsPanel.cards.Count == 2)
//            {
//                GamePlayManager.instance.EnableUnoBtn();
//            }
//            else
//            {
//                GamePlayManager.instance.DisableUnoBtn();
//            }
//        }
//    }

//    public void AddCard(Card c)
//    {
//        cardsPanel.cards.Add(c);
//        c.transform.SetParent(cardsPanel.transform);
//        if (isUserPlayer)
//        {
//            isUserClicked = true;
//            c.onClick = OnCardClick;
//            c.IsClickable = false;
//        }
//    }

//    public void RemoveCard(Card c)
//    {
//        cardsPanel.cards.Remove(c);
//        c.onClick = null;
//        c.IsClickable = false;
//    }

//    int count = 0;
//    public void OnCardClick(Card c)
//    {
//        if (c == null) {
//            isUserClicked = false;
//        }
//        if (Timer)
//        {
//            //Debug.LogError("isDoubleCards = " + isDoubleCards + "  &  = isSequentialCards = " + isSequentialCards);
//            if (isUserPlayer && (isDoubleCards || isSequentialCards))
//            {
//                Debug.LogError("c.IsDouble = " + c.IsDouble);              
//                if (!c.IsDouble && !c.isSequential)
//                {
//                    isDoubleCards = false;
//                    isSequentialCards = false;
//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                    OnTurnEnd();
//                }
//                else if (c.isSequential)
//                {
//                    isDoubleCards = false;
//                    Debug.LogError("isSequentialCards = " + isSequentialCards);
//                    foreach (var items in cardsPanel.cards)
//                    {
//                        if(items.isSequential)
//                        {
//                            items.IsClickable = true;
//                        }
//                        else
//                        {
//                            items.IsDouble = false;
//                            items.IsClickable = false;
//                        }
//                    }
//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                }
//                else 
//                {
//                    Debug.LogError("isDoubleCards = " + isDoubleCards + " && c.name = " + c.name);
//                    isSequentialCards = false;
//                    foreach (var item in cardsPanel.cards)
//                    {
//                        if(item.IsDouble && (RemoveSpecialCases(c.name) == RemoveSpecialCases(item.name)))
//                        { 
//                            Debug.LogError("c.name = " + c.name + "  & RemoveSpecialCases(item.name) = " + RemoveSpecialCases(item.name));
//                            item.IsClickable = true;                            
//                        } 
//                        else 
//                        {
//                            item.isSequential = false;
//                            item.IsClickable = false;
//                           // return;
//                        }
//                    }

//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                }

//                if (isSequentialCards)
//                {
//                    count++;
//                    if (count >= 2)
//                    {
//                        isSequentialCards = false;
//                        count = 0;
//                    }
//                    Debug.LogError("count = " + count);
//                }
//                else
//                {
//                    count++;
//                    if (count >= 1)
//                    {
//                        isDoubleCards = false;
//                        count = 0;
//                    }
//                }
//            }
//            else if(isUserPlayer && (GamePlayManager.instance.isEven || GamePlayManager.instance.isOdd))
//            {                
//                //Debug.LogError("c.name = " + c.name);
//                if (GamePlayManager.instance.isEven)
//                {
//                    CheckDublicateEvenCards(c);
//                    //Debug.LogError("isOddEvenDoubles = " + isOddEvenDoubles);

//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                    if(!isOddEvenDoubles)
//                        OnTurnEnd();
                    
//                    return;
//                }
//                else if (GamePlayManager.instance.isOdd)
//                {
//                    CheckDublicateOddCards(c);                    
//                    //Debug.LogError("isOddEvenDoubles = " + isOddEvenDoubles);

//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                    if (!isOddEvenDoubles)
//                        OnTurnEnd();
                    
//                    return;
//                }
//                else
//                {
//                    isOddEvenDoubles = false;
//                    GamePlayManager.instance.PutCardToWastePile(c, this);
//                    OnTurnEnd();
//                }
//            }
//            else
//            {
//                Debug.LogError("xDDDDDDDDD  isDoubleCards = " + isDoubleCards + "  &  = isSequentialCards = " + isSequentialCards);
//                GamePlayManager.instance.PutCardToWastePile(c, this);
//                if(isDoubleCards || isSequentialCards)
//                {
//                    return;
//                }
//                else if(isOddEvenDoubles)
//                {
//                    return;
//                }
//                else
//                {
//                    OnTurnEnd();
//                }
//                //if (!isDoubleCards || !isSequentialCards)
//                //    OnTurnEnd();
//            }
//        }
//    }

//    void CheckDublicateEvenCards(Card c)
//    {
//        if (GetBestEvenCard_WithDouble(c.name))
//        {
//            isOddEvenDoubles = true;
//            count++;
//            if (count > 1)
//            {
//                isOddEvenDoubles = false;
//                count = 0;
//            }
//        }
//        else
//        {
//            isOddEvenDoubles = false;
//        }
//    }

//    void CheckDublicateOddCards(Card c)
//    {
//        if (GetBestOddCard_WithDouble(c.name))
//        {
//            isOddEvenDoubles = true;
//            count++;
//            if (count > 1)
//            {
//                isOddEvenDoubles = false;
//                count = 0;
//            }
//        }
//        else
//        {
//            isOddEvenDoubles = false;
//        }
//    }

//    public void OnCardAdd(Card c)
//    {
//        if (Timer)
//        {
//            GamePlayManager.instance.PutCardToWastePile(c, this);
//            OnTurnEnd();
//        }
//    }

//    public void OnTurnEnd()
//    {
//        if (!choosingColor) Timer = false;
//        cardsPanel.UpdatePos();
//        foreach (var item in cardsPanel.cards)
//        {            
//            //item.SetCardsPos(false);
//            if(isUserPlayer)
//            {
//                StartCoroutine(item.SetCardsPos(true, 0f));
//            }            
//            item.SetGaryColor(false);
//        }
//        GetTotalPoints();
//        //GetListOfValues();
//        //GetDoubleCard();
//    }

//    public void ShowMessage(string message, bool playStarParticle = false)
//    {
//        messageLbl.text = message;
//        messageLbl.GetComponent<Animator>().SetTrigger("show");
//        if (playStarParticle)
//        {
//            starParticleSystem.gameObject.SetActive(true);
//            starParticleSystem.Emit(30);
//        }
//    }

//    public IEnumerator DoComputerTurn()
//    {
//        if (cardsPanel.AllowedCard.Count > 0)
//        {
//            StartCoroutine(ComputerTurnHasCard(0.25f));
//        }
//        else
//        {
//            yield return new WaitForSeconds(Random.Range(1f, totalTimer * .3f));
//            GamePlayManager.instance.EnableDeckClick();
//            GamePlayManager.instance.OnDeckClick();

//            if (cardsPanel.AllowedCard.Count > 0)
//            {
//                StartCoroutine(ComputerTurnHasCard(0.2f));
//            }
//        }
//    }

//    private IEnumerator ComputerTurnHasCard(float unoCoef)
//    {
//        bool unoClick = false;
//        float unoPossibality = GamePlayManager.instance.UnoProbability / 100f;

//        if (Random.value < unoPossibality && GetTotalPoints() <= 10)  //cardsPanel.cards.Count == 2)
//        {
//            yield return new WaitForSeconds(Random.Range(1f, totalTimer * unoCoef));
//            GamePlayManager.instance.OnUnoClick();
//            unoClick = true;
//        }

//        yield return new WaitForSeconds(Random.Range(1f, totalTimer * (unoClick ? unoCoef : unoCoef * 2)));
//        if (GamePlayManager.instance.isEven)
//        {
//            OnCardClick(GetBestEvenCard());
//        }
//        else if(GamePlayManager.instance.isOdd)
//        {
//            OnCardClick(GetBestOddCard());
//        }
//        else if (isDoubleCards && isSequentialCards)
//        {
//            doublelist.Sort((x, y) => y.CompareTo(x));
//            int sum = doublelist.OrderByDescending(x => x).Take(2).Sum();
//            int sum1 = sequentialNum.OrderByDescending(x => x).Take(sequentialNum.Count).Sum();

//            Debug.LogError("sum = " + sum + "  & sum1 = " + sum1);
//            if (sum1 > sum)
//            {
//                isDoubleCards = false;
//                DrawSequenceCards();
//            }
//            else
//            {
//                isSequentialCards = false;
//                DrawDublicateCards();
//            }
//        }
//        else if (isDoubleCards)
//        {
//            DrawDublicateCards();
//        }
//        else if (isSequentialCards)
//        {
//            DrawSequenceCards();
//        }        
//        else
//        {
//            OnCardClick(FindBestPutCard());
//        }
//    }

//    void DrawDublicateCards()
//    {
//        Debug.LogError("isDoubleCards = " + isDoubleCards);
//        for (int i = 0; i < indexList.Count; i++)
//        {
//            if (i < 2)
//            {
//                if (i > 0)
//                {
//                    isDoubleCards = false;
//                }
//                OnCardClick(PutCard());
//            }
//        }
//    }

//    void DrawSequenceCards()
//    {
//        Debug.LogError("isSequentialCards = " + isSequentialCards);
//        for (int i = 0; i < sequentialList.Count; i++)
//        {
//            if (i < 3)
//            {
//                if (i > 1)
//                {
//                    isSequentialCards = false;
//                }
//                OnCardClick(PutBestSequentialCard());
//            }
//        }
//    }

//    public string RemoveSpecialCases(string cardName)
//    {
//        //Debug.LogError("cardName = " + cardName);
//        string[] str = cardName.Split('_');
//        //Debug.LogError("str[1] = " + str[1]);
//        return str[1];
//    }

//    public bool GetBestEvenCard_WithDouble(string evenCardName)
//    {
//        List<Card> evenNum = cardsPanel.AllowEvenCards;
//        if (evenNum.Count <= 1)
//            return false;
//        for(int i = 0; i < evenNum.Count; i++)
//        {
//            if(evenNum[i].name == evenCardName)
//            {
//                evenNum.Remove(evenNum[i]);
//            }
//        }
        
//        if (evenNum.Count > 1)
//        {
//            //Debug.LogError("evenCardName = " + evenCardName);
//            for (int i = 0; i < evenNum.Count; i++)
//            {
//                if (RemoveSpecialCases(evenNum[i].name) == RemoveSpecialCases(evenCardName))
//                {
//                    //Debug.LogError("evenCardName = " + evenCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(evenNum[i].name));
//                    evenNum[i].IsClickable = true;
//                    return true;
//                }
//                else
//                {
//                    evenNum[i].IsClickable = false;
//                }
//            }
//        }
//        return false;
//    }
//    public bool GetBestOddCard_WithDouble(string oddCardName)
//    {
//        List<Card> oddNum = cardsPanel.AllowOddCards;
//        if (oddNum.Count <= 1)
//            return false;
//        for (int i = 0; i < oddNum.Count; i++)
//        {
//            if (oddNum[i].name == oddCardName)
//            {
//                oddNum.Remove(oddNum[i]);
//            }
//        }

//        if (oddNum.Count > 1)
//        {
//            //Debug.LogError("oddCardName = " + oddCardName);
//            for (int i = 0; i < oddNum.Count; i++)
//            {
//                if (RemoveSpecialCases(oddNum[i].name) == RemoveSpecialCases(oddCardName))
//                {
//                    //Debug.LogError("oddCardName = " + oddCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(oddNum[i].name));
//                    oddNum[i].IsClickable = true;
//                    return true;
//                }
//                else
//                {
//                    oddNum[i].IsClickable = false;
//                }
//            }
//        }
//        return false;
//    }

//    //public bool GetBestOddEvenCard_WithDouble(string evenCardName, bool isEven, bool isOdd)
//    //{
//    //    List<Card> evenOdd_Num = new List<Card>();
//    //    Debug.LogError("isEven = " + isEven + "   isOdd = " + isOdd);
//    //    if (isEven) {
//    //        evenOdd_Num = cardsPanel.AllowEvenCards;
//    //    } else if(isOdd) {
//    //        evenOdd_Num = cardsPanel.AllowOddCards;
//    //    }
//    //    if (evenOdd_Num.Count < 0)
//    //        return false;

//    //    if (evenOdd_Num.Count > 1)
//    //    {
//    //        Debug.LogError("evenCardName = " + evenCardName);
//    //        for (int i = 0; i < evenOdd_Num.Count; i++)
//    //        {
//    //            if(RemoveSpecialCases(evenOdd_Num[i].name) == RemoveSpecialCases(evenCardName))
//    //            {
//    //                Debug.LogError("evenCardName = " + evenCardName + "   RemoveSpecialCases(evenOdd_Num[i].name) &  = " + RemoveSpecialCases(evenOdd_Num[i].name));
//    //                evenOdd_Num[i].IsClickable = true;
//    //                return true;
//    //            }
//    //            else
//    //            {
//    //                evenOdd_Num[i].IsClickable = false;
//    //            }
//    //        }
//    //    }
//    //    return false;
//    //}

//    public Card FindBestPutCard()
//    {
//        List<Card> allow = cardsPanel.AllowedCard;
//        //allow.Sort((x, y) => y.Type.CompareTo(x.Type));
//        allow.Sort((x, y) => y.point.CompareTo(x.point));

//        return allow[0];
//    }
//    public List<int> sequentialNum, sequentialList;
//    public List<string> cardName;
//    bool checkWild = false;
//    void GetSequentialsCards(List<int> cardsList, CardType type, List<int> indexOfCard, bool isSequenceFail = false)
//    {
//        sequentialNum = new List<int>();
//        cardName = new List<string>();
//        sequentialList = new List<int>();
//        List<Card> allow = cardsPanel.AllowedCard;

//        if(cardsList.Count >= 2)
//        {
//            List<List<int>> sequenceNum = new List<List<int>>();
//            IEnumerable<IEnumerable<int>> sequences = FindSequences(cardsList, 2);
//            sequenceNum = (List<List<int>>)sequences;
//            //foreach (var sequence in sequences)
//            //{   //print results to consol
//            //    Debug.LogError(sequence.Select(num => num.ToString()).Aggregate((a, b) => a + "," + b));
//            //}
//            for (int i = 0; i < sequenceNum.Count; i++)
//            {
//                for (int j = 0; j < sequenceNum[i].Count; j++)
//                {
//                    Debug.LogError("sequenceNum[i][j] = " + sequenceNum[i][j]);
//                    sequentialNum.Add(sequenceNum[i][j]);
//                    cardName.Add(type + "_" + NumericToString(sequenceNum[i][j].ToString()));
//                }
//            }

//            int counter = 0;
//            if (sequentialNum.Count > 1)
//            {
//                isSequentialCards = true;
//                if (sequentialNum.Count < 3)
//                {
//                    foreach (var card in cardsPanel.cards)
//                    {
//                        if (card.Value == CardValue.Wild)
//                        {
//                            card.isSequential = true;
//                            sequentialNum.Add(card.cardValue);
//                            cardName.Add(card.name);
//                            isSequentialCards = true;
//                            checkWild = true;
//                            counter++;
//                        }
//                        else
//                        {
//                            if (counter < 1 && !checkWild)
//                            {
//                                isSequentialCards = false;                                
//                            }
//                            checkWild = false;
//                        }
//                    }
//                }
//            }

//            Debug.LogError(" with 1 wild card isSequentialCards = " + isSequentialCards);
//        }
//        else
//        {
//            List<Card> wildCards = new List<Card>();
//            foreach (var wildCard in cardsPanel.cards)
//            {
//                if (wildCard.Value == CardValue.Wild)
//                {
//                    wildCards.Add(wildCard);
//                    if (wildCards.Count >= 2)
//                    {
//                        List<Card> allCards = cardsPanel.AllowedCard;
//                        List<Card> NumCards = new List<Card>();

//                        for (int i = 0; i < allCards.Count; i++)
//                        {
//                            if (allCards[i].Type != CardType.Other)
//                            {
//                                NumCards.Add(allCards[i]);
//                            }
//                        }

//                        //NumCards.Sort((x, y) => y.point.CompareTo(x.point));
//                        for(int i = 0; i < NumCards.Count; i++)
//                        {
//                            sequentialNum.Add(NumCards[i].cardValue);
//                            cardName.Add(NumCards[i].name);
//                        }
                        
//                        isSequentialCards = true;
//                    }
//                }
//            }

//            if (wildCards.Count >= 2)
//            {
//                for (int i = 0; i < wildCards.Count; i++)
//                {
//                    if (i < 2)
//                    {
//                        wildCards[i].isSequential = true;
//                        sequentialNum.Add(wildCards[i].cardValue);
//                        cardName.Add(wildCards[i].name);
//                    }
//                    isSequentialCards = true;
//                }
//            }
//            else
//            {
//                isSequentialCards = false;
//                return;
//            }
//            Debug.LogError(" with 2 wild card    isSequentialCards = " + isSequentialCards);
//            //return;
//        }


//        if (!isSequentialCards)
//        {
//            return;
//        }

//        for(int i = 0; i < cardName.Count; i++)
//        {
//            for(int j = 0; j < cardsPanel.cards.Count; j++)
//            {
//                Debug.LogError("cardName[i] = "+ cardName[i] + "  & cardsPanel.cards[j].name = " + cardsPanel.cards[j].name);
//                if(cardName[i] == cardsPanel.cards[j].name)
//                {
//                    sequentialList.Add(cardsPanel.cards.IndexOf(cardsPanel.cards[j]));
//                    break;
//                }
//            }
//        }

//        for (int i = 0; i < sequentialList.Count; i++)
//        {
//            Debug.LogError("sequentialList[i] = "+ sequentialList[i]);
//            cardsPanel.cards[sequentialList[i]].isSequential = true;
//        }
//    }

//    private IEnumerable<IEnumerable<int>> FindSequences(IEnumerable<int> items, int minSequenceLength)
//    {
//        //Convert item list to dictionary
//        var itemDict = new Dictionary<int, int>();
//        foreach (int val in items)
//        {
//            itemDict[val] = val;
//        }
//        var allSequences = new List<List<int>>();
//        //for each val in items, find longest sequence including that value
//        foreach (var item in items)
//        {
//            var sequence = FindLongestSequenceIncludingValue(itemDict, item);
//            allSequences.Add(sequence);
//            //remove items from dict to prevent duplicate sequences
//            sequence.ForEach(i => itemDict.Remove(i));
//        }
//        //return only sequences longer than 3
//        return allSequences.Where(sequence => sequence.Count >= minSequenceLength).ToList();
//    }

//    //Find sequence around start param value
//    private List<int> FindLongestSequenceIncludingValue(Dictionary<int, int> itemDict, int value)
//    {
//        var result = new List<int>();
//        //check if num exists in dictionary
//        if (!itemDict.ContainsKey(value))
//            return result;

//        //initialize sequence list
//        result.Add(value);

//        //find values greater than starting value
//        //and add to end of sequence
//        var indexUp = value + 1;
//        while (itemDict.ContainsKey(indexUp))
//        {
//            result.Add(itemDict[indexUp]);
//            indexUp++;
//        }

//        //find values lower than starting value 
//        //and add to start of sequence
//        var indexDown = value - 1;
//        while (itemDict.ContainsKey(indexDown))
//        {
//            result.Insert(0, itemDict[indexDown]);
//            indexDown--;
//        }
//        return result;
//    }

//    public Card GetBestOddCard()
//    {
//        List<Card> oddNum = cardsPanel.AllowOddCards;
//        if (oddNum.Count <= 0)
//            return null;

//        oddNum.Sort((x, y) => y.point.CompareTo(x.point));
//        string cardName = oddNum[0].name;
//        List<Card> dubOddCards = new List<Card>();
//        for (int i = 1; i < oddNum.Count; i++)
//        {
//            if (cardName == oddNum[i].name)
//            {
//                dubOddCards.Add(oddNum[i]);
//                Debug.LogError("oddNum[i].name = " + oddNum[i].name);
//            }
//        }
//        return oddNum[0];
//    }

//    public Card GetBestEvenCard()
//    {
//        List<Card> evenNum = cardsPanel.AllowEvenCards;
//        if (evenNum.Count <= 0)
//            return null;

//        evenNum.Sort((x, y) => y.point.CompareTo(x.point));
//        string cardName = evenNum[0].name;
//        List<Card> dubEvenCards = new List<Card>();
//        for(int i = 1; i< evenNum.Count; i++)
//        {
//            if(cardName == evenNum[i].name)
//            {
//                dubEvenCards.Add(evenNum[i]);
//                Debug.LogError("evenNum[i].name = "+ evenNum[i].name);
//            }
//        }
//        return evenNum[0];
//    }

//    public List<int> indexList, doublelist, sameColorCardsList;
//    public void GetSequentialCards()
//    {            
//        sameColorCardsList = new List<int>();
//        Debug.LogError("GetSequentialCards() ... .. . Player Name = " + gameObject.name);    

//        List<Card> allows = cardsPanel.AllowedCard;
//        var zippedLists = allows.Zip(Enumerable.Range(0, allows.Count), (s, i) => new { s.Type, i });
//        var finalResults = from a in zippedLists
//                           group a by a.Type into g
//                           select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };

//        foreach (var item in finalResults)
//        {
//            if (item.key != CardType.Other)
//            {
//                if (item.result.count >= 1)
//                {
//                    Debug.LogError("xDDDDDDD   item.key = " + item.key);
//                    Debug.LogError("xDDDDDDDD     item.result.count = " + item.result.count);

//                    sameColorCardsList = new List<int>();//item.result.list;
//                    List<int> temp = new List<int>();
//                    for (int i = 0; i < cardsPanel.cards.Count; i++)
//                    {
//                        if(cardsPanel.cards[i].Type == item.key)
//                        {
//                            sameColorCardsList.Add(cardsPanel.cards[i].cardValue);
//                            temp.Add(cardsPanel.cards.IndexOf(cardsPanel.cards[i]));
//                            if (sameColorCardsList.Count >= 1)
//                            {                                
//                                GetSequentialsCards(sameColorCardsList, item.key, temp);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }

//    public void GetDoubleCard()
//    {
//        indexList = new List<int>();
//        doublelist = new List<int>();
//        Debug.LogError("GetDoubleCard() ... .. . Player Name = " + gameObject.name);

//        var zippedList = cardVal.Zip(Enumerable.Range(0, cardVal.Count), (s, i) => new { s, i });
//        var finalResult = from a in zippedList
//                          group a by a.s into g
//                          select new { key = g.Key, result = new { list = g.Select(o => o.i).ToList(), count = g.Count() } };
//        foreach (var item in finalResult)
//        {
//            if (item.result.count > 1 && item.result.count <= 3 && item.key > 0)
//            {
//                Debug.LogError("item.key = " + item.key);
//                Debug.LogError("item.result.count = " + item.result.count);

//                List<int> list = item.result.list;
//                for (int i = 0; i < list.Count; i++)
//                {
//                    doublelist.Add(item.key);

//                    cardsPanel.cards[list[i]].IsDouble = true;
//                    Debug.LogError("list[i] = " + list[i]);
//                    if (list.Count > 2)
//                    {
//                        if (list[0] == list[2])
//                        {
//                            cardsPanel.cards[list[i]].IsDouble = false;
//                            list.RemoveAt(2);
//                        }
//                    }
//                    indexList.Add(list[i]);
//                }
//            }
//        }

//        if (indexList.Count > 0)
//        {
//            isDoubleCards = true;
//        }
//    }

//    public Card PutCard()
//    {
//        List<Card> doubleAllow = cardsPanel.AllowDoubleCards;
//        doubleAllow.Sort((x, y) => y.point.CompareTo(x.point));

//        doublelist.Sort((x, y) => y.CompareTo(x));
//        int sum = doublelist.OrderByDescending(x => x).Take(2).Sum();

//        List<int> tempList = new List<int>();
//        for (int i = 0; i < cardsPanel.cards.Count; i++)
//        {
//            tempList.Add(cardsPanel.cards[i].point);
//        }

//        if (sum < tempList.Max())
//        {
//            //Debug.LogError("double sum = "+ sum + "  & tempList.Max() = " + tempList.Max());
//            isDoubleCards = false;
//            OnCardClick(FindBestPutCard());            
//            return null;
//        }
//        else
//        {
//            return doubleAllow[0];
//        }
//    }

//    public Card PutBestSequentialCard()
//    {
//        List<Card> doubleAllow = cardsPanel.AllowSequentialCards;
//        doubleAllow.Sort((x, y) => y.point.CompareTo(x.point));
//        //for (int i = 0; i < doubleAllow.Count; i++)
//        //{
//        //    Debug.LogError("doubleAllow[" + i + "] = " + doubleAllow[i]);
//        //}
//        //Debug.LogError("doubleAllow[" + 0 + "] = " + doubleAllow[0]);
//        return doubleAllow[0];
//    }

//    public void ChooseBestColor()
//    {
//        CardType temp = CardType.Other;
//        if (cardsPanel.cards.Count == 1)
//        {
//            temp = cardsPanel.cards[0].Type;
//        }
//        else
//        {
//            int max = 1;
//            for (int i = 0; i < 5; i++)
//            {
//                if (cardsPanel.GetCount((CardType)i) > max)
//                {
//                    max = cardsPanel.GetCount((CardType)i);
//                    temp = (CardType)i;
//                }
//            }
//        }

//        if (temp == CardType.Other)
//        {
//            GamePlayManager.instance.SelectColor(Random.Range(1, 5));
//        }
//        else
//        {
//            if (Random.value < 0.7f)
//                GamePlayManager.instance.SelectColor((int)temp);
//            else
//                GamePlayManager.instance.SelectColor(Random.Range(1, 5));
//        }
//    }
        
//    public void GetListOfValues()
//    {        
//        cardVal = new List<int>();
//        for (int i = 0; i < cardsPanel.cards.Count; i++)
//        {
//            //if (cardsPanel.cards[i].Type != CardType.Other)
//            //{
//                cardVal.Add(cardsPanel.cards[i].cardValue);
//                //indexList.Add(i);
//            //}
//        }
//        //foreach(var items in cardsPanel.cards)
//        //{
//        //    Debug.LogError("item.name = "+ items.name);
//        //}
//    }

//    public int GetTotalPoints()
//    {
//        int total = 0;
//        foreach(var c in cardsPanel.cards)
//        {
//            total += c.point;
//        }
//        if (pointTxt != null)
//            pointTxt.text = total.ToString();
//        return total;
//    }

//    private String NumericToString(String Number)
//    {
//        int _Number = Convert.ToInt32(Number);
//        String name = "";
//        switch (_Number)
//        {
//            case 1:
//                name = "One";
//                break;
//            case 2:
//                name = "Two";
//                break;
//            case 3:
//                name = "Three";
//                break;
//            case 4:
//                name = "Four";
//                break;
//            case 5:
//                name = "Five";
//                break;
//            case 6:
//                name = "Six";
//                break;
//            case 7:
//                name = "Seven";
//                break;
//            case 8:
//                name = "Eight";
//                break;
//            case 9:
//                name = "Nine";
//                break;
//            case 0:
//                name = "Zero";
//                break;
//        }
//        return name;
//    }
//}
