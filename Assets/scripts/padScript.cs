using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class padScript : MonoBehaviour
{
    public door whatDoor;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name != "ground")
        {
            Debug.Log("opening the door");
            whatDoor.openSezami();
        }
    }
}
