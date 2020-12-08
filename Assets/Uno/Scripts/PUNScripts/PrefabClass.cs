using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrefabClass : MonoBehaviour
{
    

    public void OnClick()
    {

        this.transform.SetParent(MyGamePlayMngr.instance.tablePos.transform);

    }
    


}
