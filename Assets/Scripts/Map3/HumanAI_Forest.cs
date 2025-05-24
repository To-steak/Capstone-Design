using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Transactions;
using UnityEngine.Events;
using System;
using Unity.Mathematics;


public class HumanAI_Forest : MonoBehaviour
{
    private float Health;
    private bool nearByTree = false;
    private float loggingDamage; // 10damage per loggingDelay
    private float loggingDelay = 2;
    private float curLoggingTime = 0;

    private Action onComplete;
    private UnityEvent onLogging;

    private ForestManager _forestManager;
    public NavMeshAgent agent;
    [SerializeField] private Animator _anim;

    private Transform[] waypoints;

    private enum state_ForestHumanAI
    {
        patrol,
        idle,
        logging
    }
    private state_ForestHumanAI curState;

    GameObject[] waypointsGameObject;
    private void Awake()
    {
        _forestManager = GameObject.Find("ForestManager").GetComponent<ForestManager>();
        if (_forestManager == null)
        {
            Debug.LogWarning("This scene has not contain ForestManager");
        }
        
        waypointsGameObject = GameObject.FindGameObjectsWithTag("Interactable");
        waypoints = new Transform[waypointsGameObject.Length];
        for (int i = 0; i < waypointsGameObject.Length; i++)
        {
            waypoints[i] = waypointsGameObject[i].transform;
        }
    }

    private void Start()
    {
        //onComplete += SetWaypoint;
        //SetWaypoint();

        agent.speed = _forestManager.GetHumanAISpeed();
        loggingDamage = _forestManager.GetHumanAILogDamage();
        Health = _forestManager.GetHumanAIHP();
    }

    Transform nearestWayPoint;
    Transform prevNearestWayPoint;

    bool SetWaypoint()
    {
        prevNearestWayPoint = nearestWayPoint;
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
            return true;
        }
        else
        {
            agent.SetDestination(prevNearestWayPoint.position);
            return false;
        }
    }

    bool CheckTrees()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypointsGameObject[i].transform.GetChild(0).gameObject.activeSelf) { return true; }
        }
        return false;
    }

    

    float delay = 1;
    void Update()
    {
        if (nearByTree) { curState = state_ForestHumanAI.logging; }
        else if (!nearByTree && CheckTrees()) { curState = state_ForestHumanAI.patrol; }
        else if (!nearByTree && !CheckTrees()) { curState = state_ForestHumanAI.idle; }
        //Debug.Log(curState);
        //Debug.Log(nearByTree);

        switch (curState)
        {
            case state_ForestHumanAI.patrol:
                _anim.SetBool("isPatrolling", true);
                _anim.SetBool("isLogging", false);
                curLoggingTime = 0;
                delay -= Time.deltaTime;
                if (delay <= 0)
                {
                    SetWaypoint();
                    delay = 1;
                }
                break;
            case state_ForestHumanAI.idle:
                _anim.SetBool("isPatrolling", false);
                _anim.SetBool("isLogging", false);
                curLoggingTime = 0;
                break;
            case state_ForestHumanAI.logging:
                _anim.SetBool("isPatrolling", false);
                _anim.SetBool("isLogging", true);
                Logging();
                break;
        
        }

    }


    private void Logging()
    {
        this.transform.LookAt(new Vector3(targetTree.transform.position.x, this.transform.position.y, targetTree.transform.position.z));
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
