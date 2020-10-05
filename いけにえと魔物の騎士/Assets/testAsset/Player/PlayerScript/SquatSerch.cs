using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquatSerch : MonoBehaviour
{
    PlayerMove pm;
    bool squatLock;
    string squatTag = "SquatWall";

    void Start()
    {
        pm = GetComponentInParent<PlayerMove>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (squatLock)
        {
            pm.isSquat = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //トンネルの中をしゃがみトリガーで満たす
        if(collision.tag == squatTag && pm.isSquat)
        {
            squatLock = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == squatTag && pm.isSquat)
        {
            squatLock = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == squatTag && pm.isSquat)
        {
            squatLock = false;
        }
    }

}
