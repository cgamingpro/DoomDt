using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class cinemachinetry : MonoBehaviour
{

    public CinemachineDollyCart dollyCart;
    public CinemachinePath cinePath;
    public float duration = 5f;           // Time for full trip (0 -> 1 or 1 -> 0)

    private float elapsedTime = 0f;
    private bool goingForward = true;
    public NavMeshAgent navMeshAgent;
    public Transform target;
    [SerializeField] GameObject groundEffects;

    public enum States { grounded,airborn};
    public States currentstate;

    //offset for airborn waypoint;
    public Vector3 pt0offset = new Vector3(0f, -15f, 0f);
    public Vector3 pt1offset = new Vector3(0f, 25f, 0f);
    public Vector3 pt2offset = new Vector3(0f, -15f, 2f);

    Vector3 pt0;
    Vector3 pt1;
    Vector3 pt2;
    Vector3 pt3 = Vector3.zero;
    float tt;
    bool hastarted = false;
    bool hasended = false;
    





    //state machien kinda design 


    private void Start()
    {
        dollyCart = transform.GetComponent<CinemachineDollyCart>();
        duration += Random.Range(1, 5);
        
        
    }
    void Update()
    {




        currentstate = States.airborn;
        UpdateStates();
        
    }

    public void UpdateStates()
    {
        switch (currentstate)
        {
            case States.grounded:
                grounded();
                break;
            case States.airborn:
                airborn();
                break;

        }
    }

    public void grounded()
    {



    }
    public void airborn()
    {
        bool inthepath = false; 
        // Update timer
        elapsedTime += Time.deltaTime;
        tt = Mathf.Clamp01(elapsedTime / duration);



        dollyCart.m_Position = tt;
        
        if(tt >=1 )
        {   
            genrateWaypoint();
            elapsedTime = 0f;
            GameObject gg = Instantiate(groundEffects, pt0 - pt0offset ,Quaternion.identity);
            Destroy( gg ,5);
            
        }

        if(tt == 0 )
        {
            GameObject gg = Instantiate(groundEffects, pt2 - pt2offset, Quaternion.identity);
            Destroy(gg, 5);
        }
        

       
        
       
    }

    public void genrateWaypoint()
    {
       
        Vector3 rendomRange = Random.insideUnitSphere * 100;
        rendomRange.y = 0f;

        if(Vector3.Distance(target.position,pt3) > 100)
        {
            pt0 = pt3;
        }
        else
        {
            pt0 = (target.position + rendomRange) + pt0offset;
        }


        pt2 = target.position + pt2offset;
        pt1 = target.position + (rendomRange/2) + pt1offset;
        //pt1 = ((pt0 + pt2) / 2) + pt1offset;

        pt3 = pt2;


        Transform pathTransform = cinePath.transform;

        // Convert world space to local space
        cinePath.m_Waypoints[0].position = pathTransform.InverseTransformPoint(pt0);
        cinePath.m_Waypoints[1].position = pathTransform.InverseTransformPoint(pt1);
        cinePath.m_Waypoints[2].position = pathTransform.InverseTransformPoint(pt2);

    }



}
