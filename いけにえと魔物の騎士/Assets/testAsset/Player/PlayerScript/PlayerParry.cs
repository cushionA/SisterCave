using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    //public bool guardHit;//ガードにヒット中にスタミナブレイクしたらってことか
    public GameObject Player;
    Rigidbody2D rb;
   // BoxCollider2D parry;
    float defenceTime;
    PlayerMove pm;
    bool isParryEnd;

    private void Start()
    {
        rb = Player.GetComponent<Rigidbody2D>();
        pm = GetComponentInParent<PlayerMove>();
    }

    private void FixedUpdate()
    {
        if (GManager.instance.isGuard && !isParryEnd)
        {
            defenceTime = Time.fixedDeltaTime;
            if (!GManager.instance.pStatus.equipWeapon.twinHand)
            {
                if (defenceTime >= GManager.instance.pStatus.equipShield.parryStart && !GManager.instance.isParry)
                {
                    GManager.instance.isParry = true;
                 //   parry.enabled = true;
                   // pm.isStop = true;
                    //pm.Stop();
                }
/*                else if (defenceTime - GManager.instance.pStatus.equipShield.parryStart <= GManager.instance.pStatus.equipShield.parryTime && !isStart)
                {
                   // pm.isStop = true;
                   // pm.Stop();
                    isStart = true;
                }*/
                else if (defenceTime - GManager.instance.pStatus.equipShield.parryStart > GManager.instance.pStatus.equipShield.parryTime)
                {
                    //parry.enabled = false;
                    //pm.isStop = false;
                    GManager.instance.isParry = false;
                     defenceTime = 0.0f;
                    isParryEnd = true;
                  
                }
            }
            else
            {
                if (defenceTime >= GManager.instance.pStatus.equipWeapon.parryStart && !GManager.instance.isParry)
                {
                    GManager.instance.isParry = true;
                    //   parry.enabled = true;
                    // pm.isStop = true;
                    //pm.Stop();
                }
            /*    else if (defenceTime - GManager.instance.pStatus.equipWeapon.parryStart <= GManager.instance.pStatus.equipWeapon.parryTime)
                {
                    pm.isStop = true;
                    pm.Stop();
                }*/
                else if (defenceTime - GManager.instance.pStatus.equipWeapon.parryStart > GManager.instance.pStatus.equipWeapon.parryTime)
                {
                    //parry.enabled = false;
                    //pm.isStop = false;
                    GManager.instance.isParry = false;
                     defenceTime = 0.0f;
                    isParryEnd = true;
                }
            }
        }
        else if(!GManager.instance.isGuard)
        {
            isParryEnd = false;

        }
    }


}
