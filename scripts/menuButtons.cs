using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class menuButtons : MonoBehaviour
{
    public Button[] buttons;
    public Sprite[] screens;
    public Image currentScreen;
    public AudioSource source;
    void Start()
    {
        buttons[0].onClick.AddListener(startLevel);
        buttons[1].onClick.AddListener(quit);
        buttons[2].onClick.AddListener(changeToCreddits);
        buttons[3].onClick.AddListener(changeToInfo);
        buttons[5].gameObject.SetActive(false);
        buttons[5].onClick.AddListener(goToMenu);
    }
    void startLevel() {
        SceneManager.LoadScene("level1");
        Cursor.lockState = CursorLockMode.Locked;
        source.Stop();
        source.Play();
    }
    void quit() {
        Application.Quit();
        source.Stop();
        source.Play();
    }
    void changeToCreddits() {
        foreach (Button e in buttons)
        {
            e.gameObject.SetActive(false);
        }
        buttons[5].gameObject.SetActive(true);
        currentScreen.sprite = screens[1];
        source.Stop();
        source.Play();
    }
    void changeToInfo() {
        foreach (Button e in buttons)
        {
            e.gameObject.SetActive(false);

        }
        buttons[5].gameObject.SetActive(true);
        currentScreen.sprite = screens[2];
        source.Stop();
        source.Play();
    }
    void goToMenu() {
        foreach (Button e in buttons)
        {
            e.gameObject.SetActive(true);
        }
        buttons[5].gameObject.SetActive(false);
        currentScreen.sprite = screens[0];
        source.Stop();
        source.Play();
    }

}
