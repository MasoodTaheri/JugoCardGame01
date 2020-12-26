using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScneFortune : MonoBehaviour
{
    public void OpenScene()
    {
        Debug.LogError("LoadScene");
        SceneManager.LoadScene("HomeScene");
    }
}
