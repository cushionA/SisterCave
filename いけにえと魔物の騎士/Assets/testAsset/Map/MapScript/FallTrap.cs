using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallTrap : MonoBehaviour
{
    Rigidbody2D rb;

    public Vector2 upSpeed;
    public Vector2 downSpeed;
    [SerializeField] GameObject trap;

    [HideInInspector]public bool isDown;
    [HideInInspector]public float waitTime;
    [HideInInspector]public bool isFirst;

    float nPosition;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nPosition = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDown)
        {
            if (!isFirst)
            {
                SetLayer(14);
                isFirst = true;
            }

            waitTime += Time.fixedDeltaTime;
            //rb.AddForce(downSpeed, ForceMode2D.Force);
            if (waitTime >= 2.0f)
            {
                rb.velocity = downSpeed;

            }
            else
            {
                rb.velocity = new Vector2(0, 0);
            }

        }
        else if(isDown)
        {
            if (!isFirst)
            {
                SetLayer(8);
                isFirst = true;
            }

            waitTime += Time.fixedDeltaTime;

            //rb.AddForce(upSpeed, ForceMode2D.Force);

            if (waitTime >= 2.0f)
            {
                rb.velocity = upSpeed;
            }
            else
            {
                rb.velocity = new Vector2(0, 0);

            }


        }
        //縦にしか動かしたくないときはRigidBodyのFreezePosition.Xを活用


        if(transform.position.y >= nPosition && isDown)
        {
            isFirst = false;
            waitTime = 0.0f;
            isDown = false;
            
        }
    }

    public void SetLayer(int layerNumber)
    {

        trap.layer = layerNumber;

    }

}
