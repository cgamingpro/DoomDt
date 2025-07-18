using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    public int currentWeaponindex = 0;
    GameObject equipedWeaon;
    public List<GameObject> heldweapons = new List<GameObject>();
    public GameObject fpscam;

    public int maxWeapon = 2;
    [SerializeField] Transform weaponRoot;
    [SerializeField] GameObject[] dropweaponsarray;
    [SerializeField] string[]  dropweaponsstring;

    [SerializeField]Dictionary<string,GameObject> dropWeapons = new Dictionary<string,GameObject>();
    private void Start()
    {
        for (int i = 0; i < dropweaponsstring.Length; i++)
        {
            dropWeapons.Add(dropweaponsstring[i], dropweaponsarray[i]);
        }
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpWeapon();
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") != 0 && heldweapons.Count >1)
        {
            int dir = Input.GetAxisRaw("Mouse ScrollWheel") > 0 ? 1 : 0;
            SwitchWeapon(dir);
        }
    }
    void TryPickUpWeapon()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpscam.transform.position, fpscam.transform.forward,out hit,5))
        {
            WeaponPickup pickup = hit.transform.gameObject.GetComponent<WeaponPickup>();


            if (pickup != null)
            {   
                AddWeapon(pickup);
                Destroy(hit.transform.gameObject);
            }

            else { Debug.Log("pickup is null "); }
        }
        Debug.DrawRay( fpscam.transform.position,  fpscam.transform.forward,Color.red,5);
    }

    void SwitchWeapon(int dir)
    {
        if (heldweapons[currentWeaponindex].gameObject.GetComponent<weapon>().isReloading) return;
        if (heldweapons.Count < 1) return;

        equipedWeaon.SetActive(false);
        if (dir == 1)
        {
            currentWeaponindex += 1;
            if (currentWeaponindex > maxWeapon)
            {
                currentWeaponindex = 0;
            }

        }
        else if (dir == 0)
        {
            currentWeaponindex--;
            if (currentWeaponindex < 0)
            {
                currentWeaponindex = maxWeapon;
            }
        }
        
        EquipWeapon(currentWeaponindex);
    }

    void EquipWeapon(int index)
    {
        currentWeaponindex = index;
        equipedWeaon = heldweapons[currentWeaponindex];
        equipedWeaon.gameObject.SetActive(true);
    }
    void AddWeapon(WeaponPickup pickup)
    {
        if (heldweapons.Contains(pickup.WeaponPrefab)) return;

        Debug.Log("add weapon called");

        if (heldweapons.Count > maxWeapon)
        {/*
            heldweapons.RemoveAt(currentWeaponindex);
            currentWeaponindex--;
            if(currentWeaponindex < 0)
            {
                currentWeaponindex = 0;
            }
            */
            heldweapons[currentWeaponindex].SetActive(false);
            SpawnLastWeapon(heldweapons[currentWeaponindex]);
            heldweapons.RemoveAt(currentWeaponindex);

            // Fix index
            if (currentWeaponindex >= heldweapons.Count)
                currentWeaponindex = heldweapons.Count - 1;

            if (currentWeaponindex < 0)
                currentWeaponindex = 0;

            // Re-equip something valid if list isn't empty
            if (heldweapons.Count > 0)
                EquipWeapon(currentWeaponindex);

        }
        GameObject weapon = pickup.WeaponPrefab.gameObject;
        weapon.SetActive(false);
        heldweapons.Add(weapon);
        if (heldweapons.Count == 1) EquipWeapon(0);
    }

    void SpawnLastWeapon(GameObject obj)
    {
        GameObject GN;
        Vector3 instancePositon;
        dropWeapons.TryGetValue(obj.GetComponent<weapon>().WeaponName, out GN);
        GN.AddComponent<Rigidbody>();
        GN.AddComponent<WeaponPickup>();
        GN.GetComponent<WeaponPickup>().WeaponPrefab = obj;

        RaycastHit hit;
        Physics.Raycast(fpscam.transform.position, fpscam.transform.forward, out hit, 5);
        if(hit.point != null)
        {
            instancePositon = hit.point  + new Vector3(0f,2f, 0f);
        }
        else
        {
            instancePositon = fpscam.transform.position + new Vector3(0f, 2f, 2f);
        }
        Instantiate(GN, instancePositon, Quaternion.identity);
        Debug.Log("capsule dropped");
        
    }
}