using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Waitforallplayers : MonoBehaviourPunCallbacks
{
    public Image img;
    public bool Status;
    public Vector3 RotateVec;
    public Text CurrnetPlayerTxt;
    public Text RemainTimeTxt;
    //public float RemainTime;
    // Start is called before the first frame update
    void Start()
    {
        //RemainTime = Launcher.instance.WaitBeforeStartGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (Status)
        {
            img.transform.eulerAngles += RotateVec * Time.deltaTime;
            //RemainTime -= Time.deltaTime;
            //if (RemainTime >= 0)
            //    RemainTimeTxt.text = "Remain Time " + (int)RemainTime;
        }
    }
    public void OnShow()
    {
        Status = true;
        //RemainTime = Launcher.instance.WaitBeforeStartGame;
        StartCoroutine(WaitForStartIE());
    }

    public void OnHide()
    {
        Status = false;

    }


    public IEnumerator WaitForStartIE()
    {
        while (!PhotonNetwork.IsConnected)
            yield return null;
        while (PhotonNetwork.CurrentRoom == null)
            yield return null;

        int MaxWaitTime = Launcher.instance.WaitBeforeStartGame;
        int CurrentWaitTime = 0;
        if (PhotonNetwork.IsMasterClient)
        {

            while ((CurrentWaitTime <= MaxWaitTime) &&
                (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers))
            {
                yield return new WaitForSeconds(1.0f);

                CurrentWaitTime++;
                //StartGameData.text = string.Format("Game Will start in {0} seconds- current player is {1}",
                //    (MaxWaitTime - CurrentWaitTime), PhotonNetwork.CurrentRoom.PlayerCount);
                //Debug.Log("MasterClinet wait for" + (MaxWaitTime - CurrentWaitTime));
                RemainTimeTxt.text = "Remain Time " + (int)(MaxWaitTime - CurrentWaitTime);
            }

            if (PhotonNetwork.CurrentRoom.PlayerCount >= Launcher.instance.MinimumPlayerInRoom)
            {
                //StartGameData.gameObject.SetActive(false);
                //if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.CurrentRoom.IsVisible = false;
                Launcher.instance.GetallplayerAroundTable();
                Launcher.instance.SetAllPlayerAroundTable();
                Launcher.instance.RaseEvent(Launcher.PhotonEvent_StartGame);
            }
            else
            {
                Launcher.instance.RaseEvent(Launcher.PhotonEvent_LeaveMatch);
                Debug.Log("We can't find a room with minimum player");
            }
        }
        else
        {
            //while ((CurrentWaitTime <= MaxWaitTime) && !IsPlayerGenerated)
            //{
            //    yield return new WaitForSeconds(1.0f);
            //    CurrentWaitTime++;
            //    StartGameData.text = string.Format("Game Will start in {0} seconds- current player is {1}",
            //         (MaxWaitTime - CurrentWaitTime), PhotonNetwork.CurrentRoom.PlayerCount);
            //}
            //StartGameData.gameObject.SetActive(false);
        }

        Debug.Log("WaitForStartIE");
    }
}
