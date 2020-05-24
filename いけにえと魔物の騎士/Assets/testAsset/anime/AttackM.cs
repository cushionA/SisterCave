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
    float fire1Key;
    SimpleAnimation sAni;


    bool isAEnd;



    // Start is called before the first frame update
    void Start()
    {

        pm = Player.GetComponent<PlayerMove>();
        sAni = GetComponent<SimpleAnimation>();
    }

    // Update is called once per frame

    private void Update()
    {
        fire1Key = Input.GetAxisRaw("Fire1");

        
    }

    private void FixedUpdate()
    {
        if (pm.isEnAt && fire1Key > 0 && pm.isGround) {

            isAttack = true;
            sAni.Play("Attack");

        }


        if (!isAEnd) {}

        else if (isAEnd)
        {

            isAttack = false;

        }




    }

    void AttackEnd()
    {

        isAEnd = true;

    }



}
