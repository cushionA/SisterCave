using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    //public bool guardHit;//ガードにヒット中にスタミナブレイクしたらってことか
    //public GameObject Player;
    //Rigidbody2D rb;
   // BoxCollider2D parry;
    float defenceTime;
   // PlayerMove pm;
    bool isParryEnd;
    bool noGuard;//ガードが成功してない。おねパリ禁止

    private void Start()
    {
       // rb = Player.GetComponent<Rigidbody2D>();
      //  pm = GetComponentInParent<PlayerMove>();
    }

    private void FixedUpdate()
    {


        if (!GManager.instance.guardHit)
        {
            if (GManager.instance.isGuard && !isParryEnd)
            {
                defenceTime += Time.fixedDeltaTime;
                if (!noGuard)
                {
                    if (!GManager.instance.equipWeapon.twinHand)
                    {
                        if ((defenceTime >= GManager.instance.equipShield.parryStart || GManager.instance.blocking) && !GManager.instance.isParry)
                        {
                            GManager.instance.PlaySound("ParryStart", GManager.instance.Player.transform.position);
                            GManager.instance.isParry = true;
                            defenceTime = GManager.instance.equipShield.parryStart;
                            //   parry.enabled = true;
                            // pm.isStop = true;
                            //pm.Stop();
                            GManager.instance.blocking = false;
                        }
                        /*                else if (defenceTime - GManager.instance.equipShield.parryStart <= GManager.instance.equipShield.parryTime && !isStart)
                                        {
                                           // pm.isStop = true;
                                           // pm.Stop();
                                            isStart = true;
                                        }*/
                        else if (defenceTime - GManager.instance.equipShield.parryStart > GManager.instance.equipShield.parryTime)
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
                        if ((defenceTime >= GManager.instance.equipWeapon.parryStart || GManager.instance.blocking) && !GManager.instance.isParry)
                        {
                            GManager.instance.PlaySound("ParryStart", GManager.instance.Player.transform.position);
                            GManager.instance.isParry = true;
                            //   parry.enabled = true;
                            // pm.isStop = true;
                            //pm.Stop();
                            defenceTime = GManager.instance.equipWeapon.parryStart;
                            GManager.instance.blocking = false;
                        }
                        /*    else if (defenceTime - GManager.instance.equipWeapon.parryStart <= GManager.instance.equipWeapon.parryTime)
                            {
                                pm.isStop = true;
                                pm.Stop();
                            }*/
                        else if (defenceTime - GManager.instance.equipWeapon.parryStart > GManager.instance.equipWeapon.parryTime)
                        {
                            //parry.enabled = false;
                            //pm.isStop = false;
                            GManager.instance.isParry = false;
                               defenceTime = 0.0f;
                            isParryEnd = true;
                        }
                    }
                    

                }
            }
            else if (!GManager.instance.isGuard)
            {
                isParryEnd = false;
                if (!GManager.instance.blocking)
                {
                    defenceTime = 0;
                }
                else
                {
                    defenceTime += Time.fixedDeltaTime;
                    if(defenceTime >= 3)
                    {
                        GManager.instance.blocking = false;
                        defenceTime = 0;
                    }
                }
                
                GManager.instance.isParry = false;
                noGuard = false;
            }
        }
        else if (GManager.instance.guardHit)
        {
            defenceTime += Time.fixedDeltaTime;
            GManager.instance.anotherMove = true;
            if (!GManager.instance.isGBreak)
            {
                
                //エネミーガードで入れた数値で動く
                GManager.instance.blowVector.Set(GManager.instance.nockBack, 0);
                GManager.instance.pm.rb.AddForce(GManager.instance.blowVector, ForceMode2D.Force);
            }
            if (defenceTime >= 0.25)
            {
                GManager.instance.anotherMove = false;
                GManager.instance.guardHit = false;
                defenceTime = 0;
                noGuard = true;
            }
        }

    }


}
