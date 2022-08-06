using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_CTRL : MonoBehaviour
{
    public void StartButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void EndButtonClick()
    {
        Application.Quit();
    }
}
