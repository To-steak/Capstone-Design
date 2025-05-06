using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System.Transactions;
using UnityEngine.Events;
using System;
using Unity.Mathematics;


public class HumanAI : MonoBehaviour
{
    protected float Health = 100;
    
    private Action onComplete;

    private WorldTreeManager _worldTreeManager;
    public Collider _fireLitRange;
    public NavMeshAgent agent;

    protected Transform[] waypoints;
    int waypointIndex = -1;

    private void Start()
    {
        onComplete += SetWaypoint;
        SetWaypoint();
    }

    void SetWaypoint()
    {
        int random = UnityEngine.Random.Range(0, waypoints.Length);
        waypointIndex = (waypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[random].position);
    }

    private void Awake()
    {
        _worldTreeManager = GameObject.Find("WorldTreeManager").GetComponent<WorldTreeManager>();
        if (_worldTreeManager == null)
        {
            Debug.LogWarning("This scene has not contain WorldTreeManager");
        }
        agent.speed = _worldTreeManager.HumanAISpeed;

        GameObject[] waypointsGameObject = GameObject.FindGameObjectsWithTag("Fire");
        waypoints = new Transform[waypointsGameObject.Length];
        for(int i = 0; i < waypointsGameObject.Length; i++)
        {
            waypoints[i] = waypointsGameObject[i].transform;
        }
    }

    void Update()
    {
        if(agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance - agent.stoppingDistance < 0.1f){
            onComplete.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("this : " + gameObject.name + " other : " + other.name + other.tag);
        if (other.CompareTag("Fire"))
        {
            FireObject place = other.gameObject.GetComponent<FireObject>();
            if (!place.GetLit())
            {
                place.SetLit(true);
            }
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
            Destroy(gameObject);
            _worldTreeManager.HumanHunting();
        }
    }

}
