using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWEAPON : MonoBehaviour
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

    [SerializeField] Transform shootOrigion;
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
  


    }
    private void OnDisable()
    {
        cancelreload();
    }
    public void cancelreload()
    {
        StopAllCoroutines();
        animator.Rebind();
        isReloading = false;//samae tryig to fix the issue with stuck reload 

    }
    // Update is called once per frame
    void Update()
    {

        //only call functions can be happenign while reloading 

        SilencerImp();//switch audio based on bool

       
        if (isReloading) return;     
        
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


    public void Fire( Transform origion)
    {
        if (fireTimer < fireRate) return;

        if (curretnAmmo > 0)
        {
            RaycastHit hit;

            Vector3 shootdir = shootOrigion.transform.forward;

            float moveErrorSpread = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical");//chekcs if player is moving to add a extra bulllet spread

            if (moveErrorSpread > 0)
            {
                WeaponSpread = defaultBulletSpread + weapnSpreadRunnin;
            }
            else
            {
                WeaponSpread = defaultBulletSpread;
            }

            shootdir = shootdir + shootOrigion.TransformDirection(new Vector3(Random.Range(-WeaponSpread, WeaponSpread), Random.Range(-WeaponSpread, WeaponSpread)));//applys weapon spread
            

            if (Physics.Raycast(origion.position + new Vector3(0,1f,0) , origion.forward, out hit, range))
            {
                Instantiate(sparks, hit.point, Quaternion.LookRotation(hit.point.normalized));//spawn decal sparks


                //spawn and destroy decals 
                GameObject decl = Instantiate(decal, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                decl.transform.SetParent(hit.transform);
                Destroy(decl, 5f);

                if (hit.transform.GetComponent<PlayerHelapth>())
                {
                    int hitPorbab = Random.Range(0,5);

                    if (hitPorbab == 2)
                    {

                        hit.transform.GetComponent<PlayerHelapth>().ApplyDamage(damage);
                    }


                }
                Debug.DrawRay(origion.position + new Vector3(0, 1f, 0), origion.forward, Color.green, 999);

            }
            else
            {
                hit.point = shootOrigion.position + shootdir * range;//sets it for the bullettrail line render can use as the second point
                
            }

            //stuff that ain't dependent on the hi 


            animator.SetBool("fire", true);
            muzzleFlash.Play();

            audioSource.clip = shoot;
            audioSource.Play();

            BulletTrail(hit.point);



            curretnAmmo--;

            fireTimer = 0;
        }
        else
        {
          
     
            StartCoroutine(Reload());//auto reloads and passed a bool
          
        }
    }

    

    private void BulletTrail(Vector3 hit)
    {
        Debug.Log("trail is ");
        GameObject btr = Instantiate(bulletrail, muzzleFlash.transform.position, Quaternion.identity);

        LineRenderer lr = btr.GetComponent<LineRenderer>();
        lr.SetPosition(0, muzzleFlash.transform.position);
        lr.SetPosition(1, hit);
        Destroy(btr, 0.1f);
    }




    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("reload");

        audioSource.clip = reload;
        audioSource.Play();

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = bulletsPerMag - curretnAmmo;
        int toLOad = Mathf.Min(ammoNeeded, bulletsLeft);

        curretnAmmo = bulletsPerMag;

        isReloading = false;
    }


  
}
