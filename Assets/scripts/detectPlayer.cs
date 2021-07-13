using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class detectPlayer : MonoBehaviour
{
    public patrol[] patrols;
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collider)
    {
        Debug.Log(collider.collider.name);
    }
}
