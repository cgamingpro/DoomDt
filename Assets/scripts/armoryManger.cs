using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class armoryManger : MonoBehaviour
{

    public GameObject[] targets;
    public float delay;
    public Vector3 boxcenter;
    public Vector3 boxDimension;

    public int maxTrail;
    private int currentTrial;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(spawntrail), 0f, delay);
        boxcenter = transform.position;
    }
    void spawntrail()
    {
        if (currentTrial >= maxTrail)
            return;

        GameObject prefab = targets[0];

        Vector3 randPos = boxcenter + new Vector3((Random.Range(-boxDimension.x / 2, boxDimension.x / 2)), (Random.Range(-boxDimension.y / 2, boxDimension.y / 2)), (Random.Range(-boxDimension.z / 2, boxDimension.z / 2)));
        GameObject t2 = Instantiate(prefab, randPos, Quaternion.identity);
        t2.transform.SetParent(transform);
        //Random.Range(45/4, (45+90)/4)
        

        
        currentTrial = transform.childCount - 1;


    }
    private void Update()
    {
        currentTrial = transform.childCount - 1;
    }
}
