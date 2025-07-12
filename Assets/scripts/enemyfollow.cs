using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyfollow : MonoBehaviour
{
    
    public NavMeshAgent agent;
    public Transform target;
    public Transform[] WayPoints;



    public enum States { patrol,attack,follow};
    public States currentState;

    int currentWaypoint;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStates();
        
    }

    void UpdateStates()
    {
        switch (currentState)
        {
            case States.patrol:
                patrol();
                break;
            case States.attack:
                attack();
                break;
            case States.follow:
                follow();
                break;
        }
    }
    void follow()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    void attack()
    {

    }

    void patrol()
    {
        if (agent.destination != WayPoints[currentWaypoint].position)
        {
            agent.destination = WayPoints[currentWaypoint].position;
        }
        if(Hasreached())
        {
            currentWaypoint++;
            if(currentWaypoint >= WayPoints.Length)
            {
                currentWaypoint = 0;
            }
        }

    }
    bool Hasreached()
    {
        return (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }
}
