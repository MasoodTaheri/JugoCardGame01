using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunPlayerRefs : MonoBehaviourPunCallbacks
{
    public int id;
    public PhotonView pv;
    public PunPlayer PhotonViewOf;
    public int actornumber;
    public string PhotonName;
    public void Start()
    {
        pv = GetComponent<PhotonView>();
        //PunPlayer[] allplayer = 
        //Debug.Log("allplayer length=" + allplayer.Length);
        //StartCoroutine(Findplayer());
    }

    public void Findplayer()
    {
        if (pv == null)
            pv = GetComponent<PhotonView>();
        //yield return new WaitForSeconds(2.0f);
        //Debug.Log("Findplayer", gameObject);

        actornumber = pv.Owner.ActorNumber;
        PhotonName = pv.Owner.NickName;


        PhotonViewOf =
            MultiPlayerGamePlayManager.instance.players[actornumber - 1];
        MultiPlayerGamePlayManager.instance.players[actornumber - 1].PV = pv;

        gameObject.name = pv.Owner.ActorNumber + PhotonName
            + ((pv.IsMine && PhotonNetwork.IsMasterClient) ? "(Master)" : "")
            + ((pv.IsMine) ? "Mine" : "");
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        string propertiesKey = pv.Owner.NickName;
        //Dictionary<string, string> propertiesValue;

        if (propertiesThatChanged.ContainsKey(propertiesKey))
        {
            //propertiesValue = (Dictionary<string, string>)propertiesThatChanged[propertiesKey];

            //foreach (KeyValuePair<string, string> pair in propertiesValue)
            //{
                Debug.Log("punplayer OnRoomPropertiesUpdate key=" + propertiesKey +
            "value=" + (string)propertiesThatChanged[propertiesKey]);
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
