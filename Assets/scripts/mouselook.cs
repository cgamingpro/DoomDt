
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouselook : MonoBehaviour
{
    public float mouseSensX;
    public float mouseSensY;
    [SerializeField]
    Transform yaw;
    [SerializeField]
    Transform pitch;

    float rotationx;
    float rotationy;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensX * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y")* mouseSensY * Time.deltaTime;
        rotationx -= mouseY;
        rotationy += mouseX;
        rotationx = Mathf.Clamp(rotationx, -90, 90);
        yaw.localRotation = Quaternion.Euler(0f, rotationy, 0f);
        pitch.localRotation = Quaternion.Euler(rotationx, 0f, 0f);
        
    }
    
    

}
