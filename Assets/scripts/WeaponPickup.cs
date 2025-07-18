using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField]
    enum piclupType { smg,rifle,w2};
    [SerializeField]
    piclupType currentItem;
    Transform parnt;

    private void Start()
    {
        parnt = GameObject.Find("Player").transform;
        Transform[] allChildren = parnt.GetComponentsInChildren<Transform>(true);
        if (currentItem == piclupType.smg)
        {
            foreach (Transform t in allChildren)
            {
                if (t.name == "smg")
                {
                    WeaponPrefab = t.gameObject;
                }
            }

            
            
        }
        else if(currentItem == piclupType.rifle)
        {

            foreach (Transform t in allChildren)
            {
                if (t.name == "rifle")
                {
                    WeaponPrefab = t.gameObject;
                }
            }
            //WeaponPrefab = parnt.Find("rifle").gameObject;

        }
        else if(currentItem == piclupType.w2)
        {
            foreach (Transform t in allChildren)
            {
                if (t.name == "w2")
                {
                    WeaponPrefab = t.gameObject;
                }
            }


            //WeaponPrefab = parnt.Find("w2").gameObject;
        }

        if (WeaponPrefab == null)
        {
            Debug.Log("prop spawned and its null");
        }
    }

    public GameObject WeaponPrefab;
    
    
}
