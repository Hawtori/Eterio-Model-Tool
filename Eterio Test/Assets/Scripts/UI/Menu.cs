using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject helpMenu;

    public GameObject mainMenuItems;
    public GameObject information;

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenHelp()
    {
        helpMenu.SetActive(true);
    }

    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }

    public void ModelViewScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenInformation()
    {
        mainMenuItems.SetActive(false);
        information.SetActive(true);
    }

    public void CloseInformation()
    {
        mainMenuItems.SetActive(true);
        information.SetActive(false);
    }
}
