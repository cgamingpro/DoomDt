using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("audio shit")]
    AudioSource audioSource;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip reload;

    [Header("muzzle flash and decal")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem sparks;//thsi is jut placeholder will reaplce it with dynamic one's 
    [SerializeField] GameObject decal;
    [SerializeField] GameObject bulletrail;



    public enum shootmode { auto, semi };
    public shootmode shootingMode;
    private bool Shootinput;
    public bool canSwitchMode;

    [Header("aimign down shit")]
    private Vector3 originaPositon;
    public Vector3 aimdownPositon;
    public float adsSpeed;
    public bool isAds;
    private Vector3 targetPositon;

    [Header("weapon sway shit")]
    public float amount;
    public float clamp;
    public float smooth;
    private Vector3 swayoffset;
    [SerializeField] private float WeaponSpread;
    float spreadORGina;
    [SerializeField] float weapnSpreadRunnin;
    

    [Header("UI")]
    public Text ammoCountui;
    public GameObject crossair;
    public GameObject crossairHIT;
    bool isCrossair = true;

    [Header("recoiol camera + model")]
    //hipfieresetting
    [SerializeField] float recoilx;
    [SerializeField] float recoily;
    [SerializeField] float recoilz;


    [SerializeField] RecoilGenrate recoilGenrate;
    [SerializeField] float sannpiness;
    [SerializeField] float returnSpeed;

    //aimdown
    [SerializeField] float aiMrecoilx;
    [SerializeField] float aiMrecoily;
    [SerializeField] float aiMrecoilz;

    [Header("kickbak")]
    [SerializeField] float kickbackStrength;
    [SerializeField] float kickbackReturn;
    Vector3 currentKickOffset;

    



    // Start is called before the first frame update
    void Start()
    {
        curretnAmmo = bulletsPerMag;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originaPositon = transform.localPosition;
        spreadORGina = WeaponSpread;
    }

    private void OnEnable()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.Rebind();
        updateAmmo();

         
        recoilGenrate.recoilx = recoilx;
        recoilGenrate.recoily = recoily;
        recoilGenrate.recoilz = recoilz;

        recoilGenrate.aiMrecoilz = aiMrecoilz;
        recoilGenrate.aiMrecoily = aiMrecoily;
        recoilGenrate.aiMrecoilx = aiMrecoilx;

        recoilGenrate.snappines = sannpiness;
        recoilGenrate.returnSpeed = returnSpeed; 
        

    }
    private void OnDisable()
    {
        cancelreload();
    }
    public void cancelreload()
    {
        StopAllCoroutines();
        animator.Rebind();
        isReloading = false;

    }
    // Update is called once per frame
    void Update()
    {
        


        ADS();
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

        
        if (isAds && isCrossair)
        {
            isCrossair = false;
            crossair.SetActive(false);
        }
        else if(!isAds && !isCrossair )
        {
            isCrossair = true;
            crossair.SetActive(true);
        }


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

        currentKickOffset = Vector3.Lerp(currentKickOffset,Vector3.zero, Time.deltaTime * kickbackReturn);

        targetPositon = adsBase + swayoffset + currentKickOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPositon, Time.deltaTime * adsSpeed);
        fpsCam.GetComponent<Camera>().fieldOfView = isAds ? Mathf.Lerp(60,40,adsSpeed) : Mathf.Lerp(40,60,adsSpeed);

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

        Vector3 shootdir = fpsCam.transform.forward;

        float moveErrorSpread = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical");

        if (moveErrorSpread > 0)
        {
            WeaponSpread = spreadORGina + weapnSpreadRunnin;
        }
        else
        {
            WeaponSpread = spreadORGina;
        }

        shootdir = shootdir + fpsCam.TransformDirection(new Vector3(Random.Range(-WeaponSpread, WeaponSpread), Random.Range(-WeaponSpread, WeaponSpread)));

        //shootdir.x += Random.Range(-WeaponSpread , WeaponSpread );
        //shootdir.y += Random.Range(-WeaponSpread , WeaponSpread );
        
        if(Physics.Raycast(fpsCam.position,shootdir,out hit,range))
        { 
            Instantiate(sparks, hit.point, Quaternion.LookRotation(hit.point.normalized));
            GameObject decl = Instantiate(decal, hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal));
            decl.transform.SetParent(hit.transform);
            Destroy(decl,5f);

            if(hit.transform.GetComponent<healthController>())
            {
                hit.transform.GetComponent<healthController>().ApplyDamage(damage);
                showHitCross();
            }

        }
        else
        {
            hit.point = fpsCam.position + shootdir * range;
        }
        

        animator.SetBool("fire", true);
        muzzleFlash.Play();

        audioSource.clip = shoot;
        audioSource.Play();

        recoilGenrate.RecoilFIre(isAds);

        BulletTrail(hit.point);

        kickBackOffsetApply();
        
        curretnAmmo--;
        updateAmmo();
        fireTimer = 0;
    }

    void kickBackOffsetApply()
    {
        if (isAds)
        {
            currentKickOffset.z -= kickbackStrength / 2;
        }
        else
        {
            currentKickOffset.z -= kickbackStrength;
        }
    }

    void showHitCross()
    {
        crossairHIT.SetActive(true);
        Invoke(nameof(hideHitCross), 0.1f);

    }
    void hideHitCross()
    {
        crossairHIT.SetActive(false);
    }
    private void BulletTrail(Vector3 hit)
    {
        GameObject btr = Instantiate(bulletrail, muzzleFlash.transform.position, Quaternion.identity);
        
        LineRenderer lr = btr.GetComponent<LineRenderer>();
        lr.SetPosition(0,muzzleFlash.transform.position);
        lr.SetPosition(1, hit);
        Destroy(btr, 1f);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("reload");

        audioSource.clip = reload;
        audioSource.Play();

        yield return new WaitForSeconds(2);

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

    public void updateAmmo()
    {
        ammoCountui.text = curretnAmmo + " / " + bulletsLeft;
    }
}
