using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class weapon : MonoBehaviour
{
    [Header("basic")]
    public float range = 300f;
    public int bulletsPerMag = 20;
    public int bulletsLeft;//total
    public int curretnAmmo;//curretn
    public float damage = 40;
    public float fireRate = 0.1f;//just a time as reate
    float fireTimer;//used ot calcualte total time
    public float reloadTime = 2;//used to yield , should be same as relaod anime length
    public bool isReloading = false;
    public bool noAmmo = false;
    public bool isSilenced = false;
    public string WeaponName;


    [SerializeField] Transform fpsCam;
    Animator animator;

    [Header("audio shit")]
    AudioSource audioSource;
    AudioClip shoot;//base shot
    [SerializeField] AudioClip ShootNormal;
    [SerializeField] AudioClip ShootSuppresed;
    [SerializeField] AudioClip reload;

    [Header("muzzle flash and decal")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem sparks;//thsi is jut placeholder will reaplce it with dynamic one's 
    [SerializeField] GameObject decal;
    [SerializeField] GameObject bulletrail;


    //shotmode based on state
    public enum shootmode { auto, semi };
    public shootmode shootingMode;
    private bool Shootinput;
    public bool canSwitchMode;

    [Header("aimign down shit")]
    private Vector3 originaPositon;
    public Vector3 aimdownPositon;
    public float adsSpeed;
    public bool isAds;
    private Vector3 targetPositon;// used to genrate a new vector from above two

    [Header("weapon sway shit")]
    public float amount;
    public float clamp;
    public float smooth;
    private Vector3 swayoffset;//genrte from other two
    [SerializeField] private float WeaponSpread;
    float defaultBulletSpread;//same used to calcumle a combination of both
    [SerializeField] float weapnSpreadRunnin;


    [Header("UI")]
    public Text ammoCountui;
    public GameObject crossair;
    public GameObject crossairHIT;
    [SerializeField]GameObject weaponIcon;
    [SerializeField] Sprite Weaponimage;

    bool isCrossair = true;

    [Header("recoiol camera + model")]
    //hipfieresetting
    //all get's passed to the recoil script to get recoil per gun
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

    [Header("kickbak")]//this one acts ont eh current model
    [SerializeField] float kickbackStrength;
    [SerializeField] float kickbackReturn;
    Vector3 currentKickOffset;

    [SerializeField]
    LayerMask _layer;





    // Start is called before the first frame update
    void Start()
    {
        curretnAmmo = bulletsPerMag;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originaPositon = transform.localPosition;//for aiming down
        defaultBulletSpread = WeaponSpread;//to switch 
        
    }

    private void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.Rebind();//so that weapon change dosn't effect naitor in any way
        updateAmmo();

        //passing to the recoil 
        recoilGenrate.recoilx = recoilx;
        recoilGenrate.recoily = recoily;
        recoilGenrate.recoilz = recoilz;

        recoilGenrate.aiMrecoilz = aiMrecoilz;
        recoilGenrate.aiMrecoily = aiMrecoily;
        recoilGenrate.aiMrecoilx = aiMrecoilx;

        recoilGenrate.snappines = sannpiness;
        recoilGenrate.returnSpeed = returnSpeed;

        weaponIcon.GetComponent<UnityEngine.UI.Image>().sprite = Weaponimage;


    }
    private void OnDisable()
    {
        cancelreload();
        
        if (animator != null)
        {
            animator.ResetTrigger("reload");
            animator.Rebind(); // Force reset
            animator.Update(0f); // Ensure immediate rebind
        }
    }
    public void cancelreload()
    {
        StopAllCoroutines();
        isReloading = false;//samae tryig to fix the issue with stuck reload 

    }
    // Update is called once per frame
    void Update()
    {

        //only call functions can be happenign while reloading 

        SilencerImp();//switch audio based on bool
        
        ADS();//always call before reload so u don't get stuck in reload
        if (isReloading) return;

        
        switchMode();//auto or semi

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
                    StartCoroutine(Reload());//auto reloads and passed a bool
                }
                noAmmo = true;//can be passed into else to check if no ammo in stack
            }
        }
        //manual realod
        if (curretnAmmo < bulletsPerMag && bulletsLeft > 0 && Input.GetKeyDown(KeyCode.R) )
        {
            StartCoroutine(Reload());
        }
        //firerate ahndle 
        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }
        else
        {
            animator.SetBool("fire", false);

        }


    }
    public void SilencerImp()//simple switch
    {
        if (isSilenced)
        {
            shoot = ShootSuppresed;
        }
        else
        {
            shoot = ShootNormal;
        }
    }
   

    public void ADS()
    {   //handles recoil,ADS,sway,or anything that needs to change teh local positon 
        


        //variables for sway
        float Mousex = -Input.GetAxisRaw("Mouse X") * amount;
        float Mousey = -Input.GetAxisRaw("Mouse Y") * amount;
        Mousex = Mathf.Clamp(Mousex, -clamp, clamp);
        Mousey = Mathf.Clamp(Mousey, -clamp, clamp);

        Vector3 finalpostion = new Vector3(Mousex, Mousey, 0);


        swayoffset = Vector3.Lerp(swayoffset, finalpostion, Time.deltaTime * smooth);

        isAds = Input.GetKey(KeyCode.Mouse1);



        //disable crossair while in ads
        if (isAds && isCrossair)
        {
            isCrossair = false;
            crossair.SetActive(false);
        }
        else if (!isAds && !isCrossair)
        {
            isCrossair = true;
            crossair.SetActive(true);
        }



        Vector3 adsBase = isAds ? aimdownPositon : originaPositon;//simple swithc ,can easily make 2 vector and let them decide the value of adspos to get 2 type of scopes

        currentKickOffset = Vector3.Lerp(currentKickOffset,Vector3.zero, Time.deltaTime * kickbackReturn);//adds teh kickback on loacla positon

        targetPositon = adsBase + swayoffset + currentKickOffset;//a new vector mix of all effects rather then stacking them on other gameobj

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPositon, Time.deltaTime * adsSpeed);
        //jsut switchs fov , 
        fpsCam.GetComponent<Camera>().fieldOfView = isAds ? Mathf.Lerp(60,40,adsSpeed/2) : Mathf.Lerp(40,60,adsSpeed/2);

    }

    public void Fire()
    {
        if (fireTimer < fireRate) return;

        RaycastHit hit;

        Vector3 shootdir = fpsCam.transform.forward;

        float moveErrorSpread = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical");//chekcs if player is moving to add a extra bulllet spread

        if (moveErrorSpread > 0)
        {
            WeaponSpread = defaultBulletSpread + weapnSpreadRunnin;
        }
        else
        {
            WeaponSpread = defaultBulletSpread;
        }

        shootdir = shootdir + fpsCam.TransformDirection(new Vector3(Random.Range(-WeaponSpread, WeaponSpread), Random.Range(-WeaponSpread, WeaponSpread)));//applys weapon spread

        //shootdir.x += Random.Range(-WeaponSpread , WeaponSpread );
        //shootdir.y += Random.Range(-WeaponSpread , WeaponSpread );
        
        if(Physics.Raycast(fpsCam.position,shootdir,out hit,range,_layer ,QueryTriggerInteraction.Ignore))
        { 
            Instantiate(sparks, hit.point, Quaternion.LookRotation(hit.point.normalized));//spawn decal sparks


            //spawn and destroy decals 
            GameObject decl = Instantiate(decal, hit.point,Quaternion.FromToRotation(Vector3.forward,hit.normal));
            decl.transform.SetParent(hit.transform);
            Destroy(decl,5f);

            if(hit.transform.GetComponent<healthController>())
            {
                hit.transform.GetComponent<healthController>().ApplyDamage(damage);
                showHitCross();//enalbe the other crossair
                
            }
            
            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce( - hit.normal * 10);
            }
            

        }
        else
        {
            hit.point = fpsCam.position + shootdir * range;//sets it for the bullettrail line render can use as the second point
            Debug.Log(hit.point + "yep hete sky");
        }
        
        //stuff that ain't dependent on the hi 


        animator.SetBool("fire", true);
        muzzleFlash.Play();

        audioSource.clip = shoot;
        audioSource.Play();
        

        recoilGenrate.RecoilFIre(isAds);//is asds passed ot swithc between teh two modes

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
        Debug.Log("trail is ");
        GameObject btr = Instantiate(bulletrail, muzzleFlash.transform.position, Quaternion.identity);
        
        LineRenderer lr = btr.GetComponent<LineRenderer>();
        lr.SetPosition(0,muzzleFlash.transform.position);
        lr.SetPosition(1, hit);
        Destroy(btr, 0.1f);
    }




    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("reload");

        audioSource.clip = reload;
        audioSource.Play();

        //yield return new WaitForSeconds(reloadTime);

        float timer = 0f;
        while (timer < reloadTime)
        {
            if (!gameObject.activeInHierarchy) yield break; // weapon got switched
            timer += Time.deltaTime;
            yield return null;
        }

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
