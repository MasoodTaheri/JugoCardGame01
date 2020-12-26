using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScene : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(SwitchScene());
        
    }
   
    IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("HomeScene");
        
    }
    public void Scene1()
    {
        SceneManager.LoadScene(2);
    }
}
