using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class weaponSwitching : MonoBehaviour
{
    [SerializeField]private GameObject[] weapons;
    private int currentWeaponIndex = 0;
    [SerializeField] float switchSpeed = 0.2f;
    public bool isSwitching = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        
        for (int i = 0; i < weapons.Count() ; i++)
        { 
            weapons[i].SetActive(false);
        }
        weapons[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && !isSwitching)
        {
            currentWeaponIndex++;
            
            if (currentWeaponIndex >= weapons.Length)
            {
                currentWeaponIndex = 0;
                
            }

            StartCoroutine(switchWeapons(currentWeaponIndex));
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !isSwitching)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weapons.Length - 1;
            }
            StartCoroutine(switchWeapons(currentWeaponIndex));
        }
        
    }

    IEnumerator switchWeapons(int index)
    {
        isSwitching = true;
        yield return new WaitForSeconds(switchSpeed);
        SwitchWeapon(index);
        isSwitching = false;
    }
    public void SwitchWeapon(int index)
    {
        for (int i = 0; i < weapons.Count(); i++)
        {
            if (weapons[i].GetComponent<weapon>().isReloading)
            {
                weapons[i].GetComponent<weapon>().cancelreload();
            }
            weapons[i].SetActive(false);
        }
        weapons[index].SetActive(true);
    }
}
