using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Transactions;
using UnityEngine.Events;
using System;
using Unity.Mathematics;


public class HumanAI_Forest : MonoBehaviour
{
    private float Health = 100;
    private bool nearByTree = false;
    private float loggingDamage = 20; // 10damage per loggingDelay
    private float loggingDelay = 2;
    private float curLoggingTime = 0;

    private Action onComplete;
    private UnityEvent onLogging;

    private ForestManager _forestManager;
    public NavMeshAgent agent;

    private Transform[] waypoints;

    private enum state_ForestHumanAI
    {
        patrol,
        idle,
        logging
    }
    private state_ForestHumanAI curState;

    private void Start()
    {
        //onComplete += SetWaypoint;
        //SetWaypoint();
    }

    Transform nearestWayPoint;
    void SetWaypoint()
    {
        nearestWayPoint = null;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!waypoints[i].transform.GetChild(0).gameObject.activeSelf) { continue; }
            if (nearestWayPoint == null)
            {
                nearestWayPoint = waypoints[i];
            }
            else if (Vector3.Distance(nearestWayPoint.position, this.transform.position) > Vector3.Distance(waypoints[i].position, this.transform.position))
            {
                nearestWayPoint = waypoints[i];
            }
        }

        if (nearestWayPoint != null)
        {
            agent.SetDestination(nearestWayPoint.position);
        }

    }

    bool CheckTrees()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].transform.GetChild(0).gameObject.activeSelf) { return true; }
        }
        return false;
    }

    private void Awake()
    {
        _forestManager = GameObject.Find("ForestManager").GetComponent<ForestManager>();
        if (_forestManager == null)
        {
            Debug.LogWarning("This scene has not contain ForestManager");
        }
        agent.speed = _forestManager.HumanAISpeed;

        GameObject[] waypointsGameObject = GameObject.FindGameObjectsWithTag("Interactable");
        waypoints = new Transform[waypointsGameObject.Length];
        for(int i = 0; i < waypointsGameObject.Length; i++)
        {
            waypoints[i] = waypointsGameObject[i].transform;
        }
    }

    float delay = 1;
    void Update()
    {
        if (nearByTree) { curState = state_ForestHumanAI.logging; }
        else if (!nearByTree && CheckTrees()) { curState = state_ForestHumanAI.patrol; }
        else if (!nearByTree && !CheckTrees()) { curState = state_ForestHumanAI.idle; }
        Debug.Log(curState);
        Debug.Log(nearByTree);

        switch (curState)
        {
            case state_ForestHumanAI.patrol:
                curLoggingTime = 0;
                delay -= Time.deltaTime;
                if (delay <= 0)
                {
                    SetWaypoint();
                    delay = 1;
                }
                break;
            case state_ForestHumanAI.idle:
                curLoggingTime = 0;
                break;
            case state_ForestHumanAI.logging:
                Logging();
                break;
        
        }

    }


    private void Logging()
    {
        curLoggingTime += Time.deltaTime;

        if(curLoggingTime >= loggingDelay)
        {
            curLoggingTime = 0;
            targetTree.GetComponent<Tree_Forest>().AddHP(-loggingDamage);
        }
    }

    GameObject targetTree;
    private void FixedUpdate()
    {
        nearByTree = false;
        targetTree = null;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            nearByTree = true;
            targetTree = other.gameObject;
            Debug.Log("nearbytree");
        }

    }

    public void SetHealth(float amount)
    {
        Health = amount;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log("HumanAI Health Changed " + Health);

        if (Health <= 0)
        {
            _forestManager.HumanHunting();
            Destroy(gameObject);
            
        }
    }

}
