using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackM : MonoBehaviour
{
    public GameObject Player;


    [HideInInspector] public bool isAttack;
    [HideInInspector] public bool SecAttack;
    [HideInInspector] public bool ThiAttack;


    PlayerMove pm;
    bool fire1Key;
    SimpleAnimation sAni;
    float delayTime;

    bool isAEnd;
    bool isA2End;



    // Start is called before the first frame update
    void Start()
    {

        pm = Player.GetComponent<PlayerMove>();
        sAni = GetComponent<SimpleAnimation>();
    }

    // Update is called once per frame

    private void Update()
    {
        fire1Key = Input.GetButtonDown("Fire1");


    }

    private void FixedUpdate()
    {
        if (pm.isEnAt && fire1Key && !isAttack && pm.isEnAt)
        {
            delayTime = 0.0f;
            isAttack = true;
            sAni.Play("Attack");

        }

        if (isAEnd)
        {
           delayTime += Time.fixedDeltaTime;
            
            if(delayTime < 1.5f && fire1Key)
            {

                sAni.Play("Attack2");
                isAEnd = false;
            }
            else if( delayTime >= 1.5)
            {

                isAttack = false;
                isAEnd = false;
            }

        }

        if (isA2End)
        {
            isAttack = false;
            isA2End = false;

        }


    }

    void Attack1End()
    {

        isAEnd = true;

    }

    void Attack2End()
    {

        isA2End = true;

    }

}
