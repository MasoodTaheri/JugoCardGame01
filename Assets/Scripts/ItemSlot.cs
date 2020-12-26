using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image Icon;
    public Text value;

    public void SetItemSlot(Sprite img, string val)
    {
        Icon.sprite = img;
        value.text = val;
    }
}
