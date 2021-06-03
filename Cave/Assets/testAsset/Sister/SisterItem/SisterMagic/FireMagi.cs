using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FireMagi : MonoBehaviour
{
   GameObject sister;

    Rigidbody2D rb;
    ParticleSystem sr;

    public AssetReference _explode;
    public GameObject obj;
    bool coll;
    float bre;



    // Start is called before the first frame update
    void Start()
    {
        sister = GameObject.Find("Effect");

        //transform.parent = sister.transform;

        rb = GetComponent<Rigidbody2D>();

        transform.position = sister.transform.position;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
            rb.velocity = new Vector2(10, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*  rb.velocity = new Vector2(0, 0);
          sr.Stop();

          chilObj.SetActive(true);
          coll = true;
          */
        //ReturnToPool();


        Vector3 place = this.transform.position;

        Quaternion roll = Quaternion.Euler(0f, 0f, 0f);

        Addressables.InstantiateAsync(_explode,place,roll);

        Addressables.ReleaseInstance(this.gameObject);

    }



}
