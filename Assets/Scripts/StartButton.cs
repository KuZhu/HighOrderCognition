using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject startButtonLook;

    public void StartButtonHover()
    {
        startButtonLook.SetActive(true);
        startButtonLook.GetComponent<Animator>().SetTrigger("Highlighted");
    }

    public void StartButtonOut()
    {
        startButtonLook.GetComponent<Animator>().SetTrigger("Normal");
        startButtonLook.SetActive(false);
    }

    public void StarButtonClick()
    {
        SceneManager.LoadScene(1);
    }
}
