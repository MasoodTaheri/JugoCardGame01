using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagInventory : MonoBehaviour
{
    public GameObject ItemSlot;
    public Transform Container;
    private List<GameObject> UnUsedItemSlots=new List<GameObject>();

    private void OnEnable()
    {
        InitBagInventory();
    }

    private void InitBagInventory()
    {
        foreach (var item in Datamanager.Instance.data.AllPurchedData)
        {
            GameObject obj = GetUnusedItemSlotPrefab();
            obj.SetActive(true);
            var slot = obj.GetComponent<ItemSlot>();
            slot.SetItemSlot(Datamanager.Instance.FetchSprite(item.IconName),item.Value.ToString());
        }
    }

    private void OnDisable()
    {
        DisableAllItemSlots();
    }

    void DisableAllItemSlots()
    {
        for (int i = 0; i < UnUsedItemSlots.Count; i++)
        {
            UnUsedItemSlots[i].gameObject.SetActive(false);
        }
    }
    private GameObject GetUnusedItemSlotPrefab()
    {
        for (int i = 0; i < UnUsedItemSlots.Count; i++)
        {
            if (!UnUsedItemSlots[i].activeSelf)
            {
                return UnUsedItemSlots[i];
            }
        }
        GameObject obj = Instantiate(ItemSlot.gameObject, Container);
        UnUsedItemSlots.Add(obj);
        return obj;
    }
}
