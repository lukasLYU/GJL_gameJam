using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class exitScript : MonoBehaviour
{
    public string nextLvl;
    public timeScript time;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "playerBEAN" || collision.collider.name == "fakePlayer")
        {
            Debug.Log("go next lvl pewpewpewp");
            //time.finishLvl();
            SceneManager.LoadScene(nextLvl);
        }
    }
}
