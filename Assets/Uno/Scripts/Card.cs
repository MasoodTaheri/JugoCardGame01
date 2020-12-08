using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Photon.Pun;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class Card : MonoBehaviourPun, IPunObservable, IPointerClickHandler
{
    public bool _isOpen = true, _isDouble = false, _isSequential = false;
    public bool _isClickable;
    public CardType _type;
    public CardValue _value;

    [Space(20)]
    public Text label1;
    public Text label2;
    public Text label3;
    public float moveSpeed = 10f;

    [HideInInspector]
    public int point;
    [HideInInspector]
    public int cardValue;

    public PhotonView photonView;
    public Vector3 smoothMove;
    Quaternion smoothRotation;

    RectTransform rectTransform;

    public CardType Type
    {
        get
        { 
            return _type; 
        }
        set
        {
            _type = value;
        }
    }
    public CardValue Value
    {
        get
        { 
            return _value; 
        }
        set
        {
            _value = value;
        }
    }
    public bool IsOpen
    {
        get
        {
            return _isOpen; 
        }
        set
        {
            _isOpen = value;
            UpdateCard();
        }
    }

    public bool IsDouble
    {
        get
        {
            return _isDouble;
        }
        set
        {
            _isDouble = value;
        }
    }

    public bool isSequential
    {
        get
        {
            return _isSequential;
        }
        set
        {
            _isSequential = value;
        }
    }

    public bool IsClickable
    {
        get
        {
            return _isClickable;
        }
        set
        {
            _isClickable = value;
        }
    }
    public Action<Card> onClick;

    public void SetTargetPosAndRot(Vector3 pos, float rotZ)
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);

        smoothMove = pos;

        float t = Vector2.Distance(transform.localPosition, pos) * moveSpeed / 1000f;
        LeanTween.moveLocal(gameObject, pos, t);
        LeanTween.rotateLocal(gameObject, new Vector3(0, 0, rotZ), t);
    }

    void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;

        smoothMove = transform.position;
        rectTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            UpdateCard();
#endif

        //if (!photonView.IsMine)
        //{
        //    SmoothMovement();
        //}
    }

    private void SmoothMovement()
    {
        Debug.LogError("smoothMove = "+ smoothMove);
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
        //transform.rotation = Quaternion.Lerp(transform.rotation, smoothRotation, Time.deltaTime * 10);
    }

    public void UpdateCard()
    {
        //string txt = "";
        string sprite = "NewCards/SpecialCards/BlankCard";
        if (IsOpen)
        {
            if (Type == CardType.Other)
            {
                if (Value == CardValue.DrawTwo)
                    sprite = "NewCards/SpecialCards/DrawTwo";    // "Cards/DrawFour";
                else if (Value == CardValue.Wild)
                    sprite = "NewCards/SpecialCards/Wild";
                else if (Value == CardValue.Odd)
                    sprite = "NewCards/SpecialCards/Odd";
                else if (Value == CardValue.Even)
                    sprite = "NewCards/SpecialCards/Even";
                else if (Value == CardValue.Ego)
                    sprite = "NewCards/SpecialCards/Ego";
            }
            else
            {
                int value = (int)Value;
                if (value <= 9)
                {
                    if(value <= 0)
                        sprite = "NewCards/SpecialCards/Jugo";
                    else
                        sprite = "NewCards/Cards/" + Type + "/" + value;
                }

                //int value = (int)Value;
                //if (value <= 9)
                //{
                //    sprite = "Cards/Number_" + (int)Type;
                //    //if (value == 6 || value == 9) sprite += "_Underline";
                //    txt = value + "";
                //}
            }
        }

        GetComponent<Image>().sprite = Resources.Load<Sprite>(sprite);
        label2.color = Type.GetColor();

        //label1.text = txt;
        //label2.text = (Value == CardValue.DrawTwo) ? "" : txt;
        //label3.text = txt;
    }

    public void CalcPoint()
    {
        //if (Type == CardType.Other)
        //{
        //    point = 10;
        //}
        if (Value == CardValue.Ego)
        {
            point = 15;
        }
        else if (Value == CardValue.Wild || Value == CardValue.DrawTwo || Value == CardValue.Odd || Value == CardValue.Even)
        {
            point = 10;
        }
        else
        {
            point = (int)Value;
        }
    }

    public void GetCardValue()
    {
        if (Value != CardValue.Wild && Value != CardValue.DrawTwo && Value != CardValue.Odd && Value != CardValue.Even && Value != CardValue.Ego)
        {
            cardValue = (int)Value;
           // Debug.LogError("cardValue = "+ cardValue);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOpen && IsClickable && onClick != null)
        {
            onClick.Invoke(this);
        }
    }
    public bool IsAllowCard()
    {
        return true;//Type == GamePlayManager.instance.CurrentType ||
            //Value == GamePlayManager.instance.CurrentValue ||
            //Type == CardType.Other;
    }
    public bool IsAllowOddCard()
    {
        if(Type == CardType.Other)
        {
            return false;
        }
        
        int value = (int)Value;
        if (value % 2 == 1) // Get Odd Num....... .
        {
            return true;
        }

        return false;   // Value == GamePlayManager.instance.CurrentValue;
    }
    
    public bool IsAllowEvenCard()
    {
        if (Type == CardType.Other)
        {
            return false;
        }

        int value = (int)Value;
        if (value % 2 == 0) // Get Even Num....... .
        {
            return true;
        }

        return false;
    }

    public bool IsDoubleCards()
    {
        return IsDouble ? true : false;
    }

    public bool IsSequentialCards()
    {
        return isSequential ? true : false;
    }

    public void SetGaryColor(bool b)
    {
        if (b && IsOpen)
        {
            GetComponent<Image>().color = Color.gray;
            label1.color = Color.gray;
            label2.color = Type.GetGrayColor();
            label3.color = Color.gray;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
            label1.color = Color.white;
            label2.color = Type.GetColor();
            label3.color = Color.white;
        }
    }

    public IEnumerator SetCardsPos(bool b, float time)
    {
        yield return new WaitForSeconds(time);
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 temp = rt.localPosition;
        //Debug.LogError("Vector2 temp = " + temp);

        if (b && IsOpen)
        {
            rt.localPosition = temp;
        }
        else
        {
            //Debug.LogError("rt.localPosition = " + rt.localPosition);
            rt.localPosition = new Vector2(rt.transform.localPosition.x, 50f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(GameManager.currentGameMode == GameMode.MultiPlayer)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                //stream.SendNext(transform.rotation);
            }
            else if (stream.IsReading)
            {
                smoothMove = (Vector3)stream.ReceiveNext();
                //smoothRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
