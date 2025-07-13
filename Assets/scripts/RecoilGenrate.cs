using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilGenrate : MonoBehaviour
{

    //loacrions 
    Vector3 currentRotation;
    Vector3 targetRotaion;

    //hipfirelocations
    public float recoilx;
    public float recoily;
    public float recoilz;

    public float aiMrecoilx;
    public float aiMrecoily;
    public float aiMrecoilz;

    //settings
    public float snappines;
    public float returnSpeed;


    void Start()
    {
        
    }

   
    void Update()
    {
        targetRotaion = Vector3.Lerp(targetRotaion,Vector3.zero, returnSpeed * Time.deltaTime);
        
        currentRotation = Vector3.Slerp(currentRotation,targetRotaion,snappines * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void  RecoilFIre(bool isaiming)
    {
        
       
        
        if (!isaiming)
        {
            targetRotaion = new Vector3(Random.Range(-recoilx, recoilx), Random.Range(-recoily, recoily), Random.Range(-recoilz, recoilz));
        }
        else
        {
            targetRotaion = new Vector3(Random.Range(-aiMrecoilx, aiMrecoilx), Random.Range(-aiMrecoily, aiMrecoily), Random.Range(-aiMrecoilz, aiMrecoilz));
        }
       

    }

}
