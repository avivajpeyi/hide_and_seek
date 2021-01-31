using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadMenu()
         {
             SceneManager.LoadScene("HomeMenu");
         }
    
    public void LoadHidingGame()
    {
        SceneManager.LoadScene("HiderGameScene");
    }
    
    public void LoadSeekingGame()
    {
        SceneManager.LoadScene("SeekerGameScene");
    }
}
