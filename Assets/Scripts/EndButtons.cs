using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButtons : MonoBehaviour
{
    public static bool isLeftDead;

    [SerializeField] GameObject playerLeft;
    [SerializeField] GameObject playerRight;

    public void AgainButtonClick()
    {
        SceneManager.LoadScene(1);
    }
    public void MenuButtonClick()
    {
        SceneManager.LoadScene("Start");
    }

    private void Start()
    {
        if (isLeftDead)
        {
            playerRight.SetActive(true);
        }
        else
        {
            playerLeft.SetActive(true);
        }
    }

}
      

