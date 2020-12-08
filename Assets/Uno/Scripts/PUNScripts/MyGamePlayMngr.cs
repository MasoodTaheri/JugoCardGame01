using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGamePlayMngr : MonoBehaviour
{
    public GameObject cardprefab, myContent, tablePos;
    GameObject temp;
    public static MyGamePlayMngr instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            temp = Instantiate(cardprefab, myContent.transform);
        }
    }
    public float totalTimer;
    public Text timer;
    bool mytimer = true;
    public GameObject playerPrefab;
    public Transform[] transforms;
    private void Update()
    {
        if (mytimer)
        {
            totalTimer -= Time.deltaTime;
            timer.text = totalTimer.ToString();
        }
        if (totalTimer <= 0f)
        {
            mytimer = false;
        }
    }
}
