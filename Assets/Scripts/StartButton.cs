using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject ButtonLook;
    [SerializeField] GameObject GuideImage;
    public void ButtonHover()
    {
        ButtonLook.SetActive(true);
        ButtonLook.GetComponent<Animator>().SetTrigger("Highlighted");
    }

    public void ButtonOut()
    {
        ButtonLook.GetComponent<Animator>().SetTrigger("Normal");
        ButtonLook.SetActive(false);
    }

    public void StarButtonClick()
    {
        SceneManager.LoadScene(1);
    }
    public void GuideButtonClick()
    {
        GuideImage.SetActive(true);
    }
    public void ExitButtonClick()
    {
        Application.Quit();
    }

    public void BackButtonClick()
    {
        GuideImage.SetActive(false);
    }

}
