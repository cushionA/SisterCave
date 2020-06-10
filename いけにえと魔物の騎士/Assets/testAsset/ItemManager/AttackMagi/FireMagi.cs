using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagi : MonoBehaviour
{
   GameObject sister;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        sister = GameObject.Find("SisterSan");

        transform.parent = sister.transform;

        rb = GetComponent<Rigidbody2D>();
        transform.localPosition = new Vector2(2, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(10, 0);


    }




}
