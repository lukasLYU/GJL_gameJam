using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    public GameObject thisDoor;
    public void openSezami() {
        Debug.Log("opening le door");
        Destroy(thisDoor);
    }
}
