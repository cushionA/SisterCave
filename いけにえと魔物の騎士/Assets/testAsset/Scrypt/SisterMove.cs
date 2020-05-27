using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SisterMove : MonoBehaviour
{
    public GameObject Player;

    public float neuSpeed;
    public float escapeSpeed;

    public float NeutralPos;
    public float BattlePos;
    public float BattlePos2;
    public bool isNeutral;
    public bool isBattle;
    bool isRightF;
    bool isRight;
    bool isLeftF;
    Rigidbody2D rb;
    SimpleAnimation sAni;

    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
        sAni = GetComponent<SimpleAnimation>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xSpeed = 0.0f;
        float ySpeed = 0.0f;

       Vector2 plPos = Player.transform.position;
        Vector2 sisPos = this.transform.position;

        float btDistance = plPos.x - sisPos.x;


            if(btDistance > 0)
            {
                isRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
            else 
        {
            isRight = false;
           
        }


        if (isNeutral)
        {
            if(isRight && btDistance > NeutralPos){

                xSpeed = neuSpeed;
               // sAni.Play("Move");
            }

           else if(!isRight && btDistance < -NeutralPos)
            {
                xSpeed = -neuSpeed;
               // sAni.Play("Move");
            }
            else
            {

                xSpeed = 0.0f;
            }


        }




        if (isBattle)
        {
            if (isRight && btDistance < BattlePos)
            {
                isRightF = false;
                isLeftF = false;

                xSpeed = -escapeSpeed;
                // sAni.Play("Move");
            }
            else if (isRight && btDistance > BattlePos2)
            {
                isRightF = false;
                isLeftF = false;

                xSpeed = escapeSpeed;
                // sAni.Play("Move");
            }
            else if (!isRight && btDistance > -BattlePos)
            {

                isRightF = false;
                isLeftF = false;


                xSpeed = escapeSpeed;
                // sAni.Play("Move");
            }
            else if (!isRight && btDistance < -BattlePos2)
            {

                isRightF = false;
                isLeftF = false;


                xSpeed = -escapeSpeed;
                // sAni.Play("Move");
            }

            else 
            {
                //二秒間振り向きモーション

                xSpeed = 0.0f;
                if (isRight)
                {
                    isRightF = true;

                }

                else if (!isRight)
                {
                    isLeftF = true;

                }
            
            
            }


        }


        if (xSpeed > 0 || (xSpeed <= 0 && isRightF))
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
       else if(xSpeed < 0 || (xSpeed >= 0 && isLeftF))
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }






        rb.velocity = new Vector2(xSpeed, ySpeed);

    }
}
