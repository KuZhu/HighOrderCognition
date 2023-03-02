using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButtons : MonoBehaviour
{
    public void AgainButtonClick()
    {
        SceneManager.LoadScene(1);
    }
    public void MenuButtonClick()
    {
        SceneManager.LoadScene("Start");
    }
    
}
      

