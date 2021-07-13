using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class patrol : MonoBehaviour
{
    public Transform[] points;
    public bool moveBack;
    public playerController controller;
    public NavMeshAgent agent;
    public bool shouldMove = true;
    public bool destroyed = false;
    public LayerMask possiblePlayers;
    bool canSeePlayer = false;
    bool setOne = false;
    bool setTwo = false;
    public float chaceLast = 1f;
    float lastTimeSeenPlayer = 0;
    float passedTime = 0;
    private Transform player;
    // 0 - patrol
    // 1 - chase
    public int state = 0;
    void Start()
    {
        
    }
    void Update(){
        if (!controller.isAi){
            if (!destroyed)
            {
                agent.isStopped = true;
                Destroy(controller.transform.gameObject.GetComponent<NavMeshAgent>());
                destroyed = true;
            }
        }
        else {
            Debug.DrawRay(transform.position + Vector3.up * 0.7f, transform.forward * 5f, Color.green);
            Debug.DrawRay(transform.position + Vector3.up * 0.7f, (transform.forward + transform.right).normalized * 5f, Color.green);
            Debug.DrawRay(transform.position + Vector3.up * 0.7f, (transform.forward - transform.right).normalized * 5f, Color.green);
           
        }
    }

    void FixedUpdate() {
        if (moveBack && controller.isAi && state == 0)
        {
            if (!setOne) {
                agent.SetDestination(points[0].position);
                setOne = true;
                setTwo = false;
            }
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.SetDestination(points[1].position);
                    moveBack = false;
                }
            }
        }
        else if (!moveBack && controller.isAi && state == 0) {
            if (!setTwo) {
                agent.SetDestination(points[1].position);
                setOne = false;
                setTwo = true;
            }
            if (!agent.pathPending) {
                if (agent.remainingDistance <= agent.stoppingDistance) {
                    moveBack = true;
                }
                else {
                    moveBack = false;
                    agent.SetDestination(points[1].position);
                }
            }
        }
        if (controller.isAi && state == 1) {
            if (canSeePlayer)
            {
                lastTimeSeenPlayer = Time.time;
            }
            if (!canSeePlayer)
            {
                if ((Time.time - lastTimeSeenPlayer) > chaceLast)
                {
                    Debug.Log("stop chacing player");
                    state = 0;
                    agent.SetDestination(points[1].position);
                }
            }
            Debug.Log(Time.time - lastTimeSeenPlayer);
            Debug.Log(canSeePlayer);
            agent.SetDestination(player.position);
        }
        RaycastHit hit;
        // i hate my self for this
        if (Physics.Raycast(transform.position + Vector3.up * 0.7f, (transform.forward + transform.right).normalized * 5f, out hit, 2000f, possiblePlayers) || Physics.Raycast(transform.position + Vector3.up * 0.7f, (transform.forward - transform.right).normalized * 5f, out hit, 2000f, possiblePlayers) || Physics.Raycast(transform.position + Vector3.up * 0.7f, transform.forward * 5f, out hit, 2000f, possiblePlayers)){
            if (hit.transform.GetComponent<playerController>().isAi == false){
                state = 1;
                Debug.Log("player seen");
                canSeePlayer = true;
                lastTimeSeenPlayer = Time.time;
                player = hit.transform;
                agent.SetDestination(player.position);
            }
        }
        else{
            canSeePlayer = false;
        }

    }
}