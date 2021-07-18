using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class soundmanager : MonoBehaviour{
    private string currentScene;
    public AudioSource source;
    public AudioClip[] sources;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "level1")
        {
        }
        if (currentScene == "level2") {

        }
        if (currentScene == "level3")
        {

        }
        if (currentScene == "level4")
        {
        }
        if (currentScene == "level5")
        {
        }
    }
}
