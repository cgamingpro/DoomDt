using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class uihandle : MonoBehaviour
{
    public playerMovment playerMovment;
    public Slider slider;
    PostProcessVolume PostProcessVolume;
    public Transform postOBJ;
    Vignette dashmsk;
    
    private void Start()
    {
        PostProcessVolume = postOBJ.GetComponent<PostProcessVolume>();
        if(PostProcessVolume.profile.TryGetSettings(out dashmsk))
        {
            dashmsk.active = false;
        }
        else
        {
            Debug.Log("naah not get");
        }
    }
    // Update is called once per frame
    void Update()
    {
        slider.value = (playerMovment.currentStamina/playerMovment.maxstamina);

        if (playerMovment.isDashing)
        {
            dashmsk.active = true;
        }
        else
        {
            dashmsk.active = false;
        }
    }

}
