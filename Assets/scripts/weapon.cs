using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class weapon : MonoBehaviour
{
    public float range = 300f;
    public int bulletsPerMag = 20;
    public int bulletsLeft;//total
    public int curretnAmmo;//curretn
    public float damage = 40;

    public float fireRate = 0.1f;
    float fireTimer;
    public bool isReloading = false;
    public bool noAmmo = false;
    [SerializeField] Transform fpsCam;
    Animator animator;
    //audio
    AudioSource audioSource;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip reload;

    //muzzleflash
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem sparks;//thsi is jut placeholder will reaplce it with dynamic one's 
    [SerializeField] GameObject decal;
    

    //type of fire 
    public enum shootmode { auto,semi};
    public shootmode shootingMode;
    private bool Shootinput;
    public bool canSwitchMode;

    //aimign down shit
    private Vector3 originaPositon;
    public Vector3 aimdownPositon;
    public float adsSpeed;
    public bool isAds;
    private Vector3 targetPositon;

    //weapon sway shit
    public float amount;
    public float clamp;
    public float smooth;
    private Vector3 swayoffset;
    

    // Start is called before the first frame update
    void Start()
    {
        curretnAmmo = bulletsPerMag;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originaPositon = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        if (isReloading) return;


        switchMode();
        
        if (Shootinput)
        {
            if (curretnAmmo >= 0)
            {
                Fire();

            }
            else
            {
                if (bulletsLeft > 0)
                {
                    StartCoroutine(Reload());
                }
                noAmmo = true;
            }
        }
        //manual realod
        if (curretnAmmo < bulletsPerMag && bulletsLeft > 0 && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine (Reload());
        }
        //firerate ahndle 
        if(fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
        else
        {
            animator.SetBool("fire",false);
            
        }

        ADS();

    }
   

    public void ADS()
    {

        float Mousex = -Input.GetAxisRaw("Mouse X") * amount;
        float Mousey = -Input.GetAxisRaw("Mouse Y") * amount;
        Mousex = Mathf.Clamp(Mousex, -clamp, clamp);
        Mousey = Mathf.Clamp(Mousey, -clamp, clamp);

        Vector3 finalpostion = new Vector3(Mousex, Mousey, 0);


        swayoffset = Vector3.Lerp(swayoffset, finalpostion, Time.deltaTime * smooth);

        isAds = Input.GetKey(KeyCode.Mouse1);
        Vector3 adsBase = isAds ? aimdownPositon : originaPositon;

        targetPositon = adsBase + swayoffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPositon, Time.deltaTime * adsSpeed);
        fpsCam.GetComponent<Camera>().fieldOfView = isAds ? 40f : 60f;

        /*
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isAds = true;
            transform.localPosition = Vector3.Lerp(transform.localPosition,aimdownPositon, Time.deltaTime * adsSpeed);
            fpsCam.GetComponent<Camera>().fieldOfView = 40;
        }
        else
        {
            isAds = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition,originaPositon, Time.deltaTime * adsSpeed);
            fpsCam.GetComponent<Camera>().fieldOfView = 60f;
        }
        */

    }

    private void Fire()
    {
        if (fireTimer < fireRate) return;

        RaycastHit hit;

        if(Physics.Raycast(fpsCam.position,fpsCam.transform.forward,out hit,range))
        { 
            Instantiate(sparks, hit.point, Quaternion.LookRotation(hit.point.normalized));
            GameObject decl = Instantiate(decal, hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal));
            decl.transform.SetParent(hit.transform);
            Destroy(decl,5f);

            if(hit.transform.GetComponent<healthController>())
            {
                hit.transform.GetComponent<healthController>().ApplyDamage(damage);
            }

        }

        animator.SetBool("fire", true);
        muzzleFlash.Play();

        audioSource.clip = shoot;
        audioSource.Play();


        curretnAmmo--;
        fireTimer = 0;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("reload");

        audioSource.clip = reload;
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);

        int ammoNeeded = bulletsPerMag - curretnAmmo;
        int toLOad = Mathf.Min(ammoNeeded,bulletsLeft);

        curretnAmmo += toLOad;
        bulletsLeft -= toLOad;

        isReloading = false;
    }

    private void switchMode()
    {

        if (canSwitchMode && shootingMode == shootmode.auto && Input.GetKeyDown(KeyCode.X))
        {
            shootingMode = shootmode.semi;
        }
        else if (canSwitchMode && shootingMode == shootmode.semi && Input.GetKeyDown(KeyCode.X))
        {
            shootingMode = shootmode.auto;
        }
        switch (shootingMode)
        {
            case shootmode.auto:

                Shootinput = (Input.GetButton("Fire1"));
                break;
            case shootmode.semi:
                Shootinput = (Input.GetButtonDown("Fire1"));
                break;
        }



    }
}
