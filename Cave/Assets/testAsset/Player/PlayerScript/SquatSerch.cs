using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquatSerch : MonoBehaviour
{
    bool squatLock;
    string squatTag = "SquatWall";



    // Update is called once per frame
    void FixedUpdate()
    {
        if (squatLock)
        {
            GManager.instance.pm.isSquat = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //トンネルの中をしゃがみトリガーで満たす
        if(collision.tag == squatTag && GManager.instance.pm.isSquat)
        {
            squatLock = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == squatTag && GManager.instance.pm.isSquat)
        {
            squatLock = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == squatTag && GManager.instance.pm.isSquat)
        {
            squatLock = false;
        }
    }

}
