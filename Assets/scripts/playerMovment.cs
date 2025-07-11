using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.XR;
using UnityEngine;

public class playerMovment : MonoBehaviour
{
    [Header("basic shit")]
    public float moveSpeed;
    public float walkSpeed;
    
    public float sprintSpeed;
    public float gravity;
    public float jumpheight;
    [SerializeField] Transform yaw;
    CharacterController controller;

    Vector3 velocity;
    public float groundCheckDist;
    public LayerMask groundCheckLayer;
    public Transform groundcheck;
    public bool isgrounded;


    [Header("stamina shit")]
    public bool isSprinting;
    public float maxstamina = 100f;
    public float depletionStamina = 10f;
    public float regainStamina = 20f;
    public float currentStamina;


    [Header("dash shit")]
    [SerializeField] float dashMul;
    public float dashDuration;
    public float dashCooldown;
    public float lastDashTime;
    public bool isDashing;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxstamina;
    }

    // Update is called once per frame
    void Update()
    {
        isgrounded = Physics.CheckSphere(groundcheck.position, groundCheckDist, groundCheckLayer);

        if(isgrounded && velocity.y <0)
        {
            velocity.y += -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveVc = yaw.forward * vertical + yaw.right * horizontal;
        moveVc = moveVc.normalized * moveSpeed;

        if (!isDashing)
        {
            controller.Move(moveVc * Time.deltaTime);
        }

        //sprint implentation 
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && moveVc != Vector3.zero;
        if (isSprinting && vertical > 0)
        {
            moveSpeed = sprintSpeed;
        }
        else if (isSprinting && (vertical < 0 || horizontal != 0))
        {
            moveSpeed = sprintSpeed / 2;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
        //dash shit 
        if (Input.GetKeyDown(KeyCode.LeftControl) && moveVc != Vector3.zero && Time.time >= lastDashTime + dashCooldown && currentStamina > 40f)
        {
            
            StartCoroutine(Dash(moveVc));
        }
        //deplition shit
        if (isSprinting)
        {
            currentStamina -= depletionStamina * Time.deltaTime;
        }
        else if (currentStamina < maxstamina && isgrounded)
        {
            currentStamina += regainStamina * Time.deltaTime;
        }
        



        if (isgrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(gravity * jumpheight * -2f);
        }


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
    IEnumerator Dash(Vector3 dashDir)
    {
        isDashing = true;
        currentStamina -= 40f;
        lastDashTime = Time.time;
        float startTime = Time.time;
        while (Time.time < dashDuration + startTime)
        {
            controller.Move(dashDir * Time.deltaTime * dashMul * sprintSpeed);
            yield return null;
        }
        isDashing = false;
    }
}
