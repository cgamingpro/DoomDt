using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmoCount : MonoBehaviour
{
    [SerializeField] weapon weapon;
    [SerializeField] Vector3 normalPos;
    [SerializeField] Vector3 adsPOs;
    // Start is called before the first frame update
    void Start()
    {
        normalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon.isAds)
        {
            
           transform.localPosition =  Vector3.Lerp(transform.localPosition, adsPOs, Time.deltaTime * weapon.adsSpeed);
            
        }
        else
        {
            transform.localPosition =  Vector3.Lerp(transform.localPosition, normalPos,Time.deltaTime * weapon.adsSpeed);
            
        }
    }
}
