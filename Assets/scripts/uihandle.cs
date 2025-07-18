using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class uihandle : MonoBehaviour
{
    public playerMovment playerMovment;
    public Slider SliderStamina;
    PostProcessVolume PostProcessVolume;
    public Transform postOBJ;
    Vignette dashmsk;
    [SerializeField] PlayerHelapth playerHelapth;
    [SerializeField] Slider SliderHealth;
    [SerializeField] Text Text_medkitNumber;
  
    
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
        SliderStamina.value = (playerMovment.currentStamina/playerMovment.maxstamina);
       
        SliderHealth.value = (playerHelapth.currentHealth / playerHelapth.maxhealth);

        Text_medkitNumber.text = playerHelapth.medPackNumber.ToString();

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
