using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject startButtonLook;
    [SerializeField] GameObject guideButtonLook;

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
    public void GuideButtonHover()
    {
        guideButtonLook.SetActive(true);
        guideButtonLook.GetComponent<Animator>().SetTrigger("Highlighted");
    }
    public void GuideButtonOut()
    {
        guideButtonLook.SetActive(false);
        guideButtonLook.GetComponent<Animator>().SetTrigger("Normal");
    }
}
