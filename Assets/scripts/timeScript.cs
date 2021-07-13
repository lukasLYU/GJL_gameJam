using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeScript : MonoBehaviour{
    public float totalTime;
    public playerVariableManager variableManager;
    public void finishLvl() {
        totalTime = Time.time;
        variableManager.timeCompletion += totalTime;
        Debug.Log(totalTime);
    }
}
