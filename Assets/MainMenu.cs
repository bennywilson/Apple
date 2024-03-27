using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    enum EMenuState
    {
        MainMenu,
        ReadStory,
        Credits,
        QuitGame,
        Num,
    };
    EMenuState CurMenuState = EMenuState.MainMenu;

    public GameObject MainMenuScreen;
    public GameObject ReadyStoryScreen;
    public GameObject CreditScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnReadStory()
    {
        MainMenuScreen.SetActive(false);
        ReadyStoryScreen.SetActive(true);
        CreditScreen.SetActive(false);
    }

    public void OnCredits()
    {
        MainMenuScreen.SetActive(false);
        ReadyStoryScreen.SetActive(false);
        CreditScreen.SetActive(true);
    }

    public void OnQuitGame()
    {
        Debug.Log("QuitGame()");
    }

    public void OnBackToMainMenu()
    {
        MainMenuScreen.SetActive(true);
        ReadyStoryScreen.SetActive(false);
        CreditScreen.SetActive(false);
    }
}
