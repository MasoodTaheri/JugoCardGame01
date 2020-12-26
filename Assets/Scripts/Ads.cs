using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ads : MonoBehaviour
{
    public void OpenScene()
    {
        SceneManager.LoadScene("Ads");
    }
    public void OpenScene2()
    {
        SceneManager.LoadScene("Fortune");
    }
    public void OpenFortune()
    {
        SceneManager.LoadScene("FortuneWheel");
    }
}
