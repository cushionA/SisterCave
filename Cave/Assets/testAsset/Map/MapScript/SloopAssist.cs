using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SloopAssist : MonoBehaviour
{
    // string playerTag = "Player";
    [SerializeField] int sloopDirection;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GManager.instance.playerTag)
        {
            if (Mathf.Sign(GManager.instance.pm.xSpeed) == -sloopDirection)
            {
                GManager.instance.pm.isSloopDown = true;
                GManager.instance.pm.ySpeed = -GManager.instance.pm.sloopForce;
            }
            else if (Mathf.Sign(GManager.instance.pm.xSpeed) == sloopDirection)
            {
                //Debug.Log("‚ ‚¢‚³");
                GManager.instance.pm.isSloopDown = false;
                GManager.instance.pm.ySpeed = GManager.instance.pm.sloopForce;
            }
            else
            {
                GManager.instance.pm.isSloopDown = false;
            }

        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GManager.instance.playerTag)
        {
            if (Mathf.Sign(GManager.instance.pm.xSpeed) != sloopDirection)
            {
                GManager.instance.pm.isSloopDown = true;
                GManager.instance.pm.ySpeed = -GManager.instance.pm.sloopForce;
            }
            else if (Mathf.Sign(GManager.instance.pm.xSpeed) == sloopDirection)
            {
                //Debug.Log("‚ ‚¢‚³");
                GManager.instance.pm.isSloopDown = false;
                GManager.instance.pm.ySpeed = GManager.instance.pm.sloopForce;
            }
            else
            {
                GManager.instance.pm.isSloopDown = false;

            }
        }
    }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == GManager.instance.playerTag)
            {

                GManager.instance.pm.isSloopDown = false;
            }
        }
    }
