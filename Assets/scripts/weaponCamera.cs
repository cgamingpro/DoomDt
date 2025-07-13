using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponCamera : MonoBehaviour
{
    [SerializeField] Transform fpsCam;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Camera>().fieldOfView = fpsCam.GetComponent<Camera>().fieldOfView;
    }
}
