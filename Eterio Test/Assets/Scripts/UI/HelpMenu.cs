using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    public List<GameObject> mainHelpScreen;

    public List<GameObject> helpTexts;

    public List<GameObject> mainScreenButtons;

    private bool isHelpShowing = false;

    public void ExitButton()
    {
        if (!isHelpShowing)
        {
            foreach(GameObject btns in mainScreenButtons) btns.SetActive(true);

            gameObject.SetActive(false);
        }
        
        foreach(GameObject text in helpTexts)
        {
            text.SetActive(false);
            isHelpShowing = false;
        }

        foreach(GameObject objects in mainHelpScreen)
        {
            objects.SetActive(true);
        }
    }

    public void OpenHelp()
    {
        gameObject.SetActive(true);
        foreach (GameObject btns in mainScreenButtons) btns.SetActive(false);
    }

    public void ShowHelpText(int index)
    {
        foreach (GameObject objects in mainHelpScreen)
        {
            objects.SetActive(false);
        }
        helpTexts[index].SetActive(true);
        isHelpShowing = true;
    }
}
