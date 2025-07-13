using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trailSpawner : MonoBehaviour
{
    public GameObject[] trailsOnj;
    public float delay;
    public Vector3 skycentre;
    public Vector3 spaawnlimit;

    public int maxTrail;
    private int currentTrial;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(spawntrail),0f,delay);
    }
    void spawntrail()
    {
        if (currentTrial >= maxTrail) 
            return;

        GameObject prefab = trailsOnj[Random.Range(0,trailsOnj.Length)];

        Vector3 randPos = skycentre + new Vector3((Random.Range(-spaawnlimit.x/2,spaawnlimit.x/2)), (Random.Range(-spaawnlimit.y / 2, spaawnlimit.y / 2)), (Random.Range(-spaawnlimit.z / 2, spaawnlimit.z / 2)));
        GameObject trail = Instantiate(prefab, randPos, Quaternion.Euler(0, 0, 0f));
        //Random.Range(45/4, (45+90)/4)
        currentTrial++;

        ParticleSystem ps = trail.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            float life = ps.main.startLifetime.constantMax;
            Destroy(trail,life);
            currentTrial--;
        }

        

    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
