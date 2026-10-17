using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; 
public class MainMenuUI : MonoBehaviour
{

   public GameObject fadeInCanvas;
   public void quitGame()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void startGame()
    {
        StartCoroutine(loadScene());
    }

    IEnumerator loadScene()
    {
       Instantiate(fadeInCanvas);
       yield return new WaitForSeconds(1.5f);
       
       AsyncOperation ao = SceneManager.LoadSceneAsync("PlayerHouse", LoadSceneMode.Single);
       while (!ao.isDone)
       {
           yield return null;
       }
    }
    
}
