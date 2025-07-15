using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyfollow : MonoBehaviour
{
    
    NavMeshAgent agent;
    public Transform target;
    public Transform[] WayPoints;

    public Animator animator;
    public bool insight;
    public float shootDistance;
    public AIWEAPON weapon;
    public Vector3 directionTotarget;
    public float maxFollowDistance;


    




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
        checkforPlayer();
        UpdateStates();
        
    }
    void checkforPlayer()
    {
        directionTotarget = target.position - transform.position;

        RaycastHit hitinfo;
        if (Physics.Raycast(transform.position, directionTotarget.normalized, out hitinfo,directionTotarget.magnitude))
        {
            insight = hitinfo.transform.CompareTag("Player");
            Debug.DrawRay(transform.position,directionTotarget,Color.red,999);
        }
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
        AnimatorBool(false,false,true,false);
        
        if (directionTotarget.magnitude <= shootDistance && insight)
        {
            AnimatorBool(false, false, false, true);
           
            currentState = States.attack;
            agent.ResetPath();
        }
        else

        {
            if (target != null)
            {
                 agent.SetDestination(target.position);
            }
            if(directionTotarget.magnitude > maxFollowDistance)
            {
                AnimatorBool(false, true, false, false);
                
                currentState = States.patrol;
            }
        }
        
    }
    void attack()
    {
        if (!insight || directionTotarget.magnitude > shootDistance) 
        {
            AnimatorBool(false, false, true, false);
           
            currentState = States.follow;
        }
        weapon.Fire();
        lookattarget();
    }
    void lookattarget()
    {
        Vector3 lookDirection = directionTotarget;

        lookDirection.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);

    }

    void patrol()
    {

        if (agent.destination != WayPoints[currentWaypoint].position)
        {
            agent.destination = WayPoints[currentWaypoint].position;
        }
        if(WayPoints.Length == 1)
        {
            idle();
        }
        if(Hasreached())
        {
            currentWaypoint++;
            if(currentWaypoint >= WayPoints.Length)
            {
                currentWaypoint = 0;
            }
        }
        if (insight && directionTotarget.magnitude < maxFollowDistance)
        {
            currentState = States.follow;
        }

    }
    bool Hasreached()
    {
        return (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
    }
    void AnimatorBool(bool idle = false,bool walk = false,bool run = false,bool fire = false)
    {
        animator.SetBool("idle", idle);
        animator.SetBool("walk", walk);
        animator.SetBool("run", run);
        animator.SetBool("fire",fire); 
    }
    void idle()
    {
        AnimatorBool(true, false, false, false);
 
        if (insight && directionTotarget.magnitude < maxFollowDistance)
        {
            AnimatorBool(false, true, false, false);
        
            currentState = States.follow;
        }
    }
}
