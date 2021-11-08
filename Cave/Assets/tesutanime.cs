using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tesutanime : MonoBehaviour
{
    Animator unko;
    float peniss;
    bool mannko;

    public float ooomenn;
    public float deeemenn;
    // Start is called before the first frame update
    void Start()
    {
        unko = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        peniss += Time.fixedDeltaTime;

        if(peniss >= 3)
        {
            mannko = !mannko;
            peniss = 0;
        }
        if (mannko)
        {
            unko.Play("1");
        }
        else
        {
            unko.Play("2");
        }
       // unko.Play();
    }
}
