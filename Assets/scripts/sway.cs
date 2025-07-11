using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sway : MonoBehaviour
{

    /// <summary>
    /// origianll used ot add local sway , but since the script and ads script conflict ,
    /// ther are two solutions , either i place it on parent which i don't wanna do , so 
    /// i will be passing the sway amount to teh weapon script ,  which removes teh smooth control , 
    /// 
    /// </summary>
    public float amount;
    public float clamp;
    public float smooth;
    

    Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float Mousex = -Input.GetAxisRaw("Mouse X") * amount;
        float Mousey = -Input.GetAxisRaw("Mouse Y") * amount;
        Mousex = Mathf.Clamp(Mousex, -clamp, clamp);
        Mousey = Mathf.Clamp(Mousey, -clamp, clamp);

        Vector3 finalpostion  = new Vector3(Mousex, Mousey, 0);
        
        transform.localPosition = Vector3.Lerp(transform.localPosition,finalpostion + initialPosition, Time.deltaTime * smooth);
    }
}
