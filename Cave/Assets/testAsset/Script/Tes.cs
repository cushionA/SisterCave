using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tes : MonoBehaviour
{

    float SSS;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.log("s");

        SSS += Time.fixedDeltaTime;

            if(SSS > 3.0)
        {
            //Debug.log("sss");
            SSS = 0.0f;

        }

    }


}
