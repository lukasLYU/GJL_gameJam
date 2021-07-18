using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class die : MonoBehaviour
{
    public Transform currentPlayel;
    public AudioClip tazer;
    public AudioSource reee;

    private void Start()
    {
        reee.Stop();
    }

    private void OnCollisionEnter(Collision collision){
        if (collision.collider.name == currentPlayel.transform.name)
        {
            SceneManager.LoadScene("menu");
        }
    }
    public void killPlayer(playerController playerController){
        reee.loop = false;
        reee.Play();
        reee.clip = tazer;
        reee.Play();
        SceneManager.LoadScene("menu");
        Cursor.lockState = CursorLockMode.None;
    }
}
