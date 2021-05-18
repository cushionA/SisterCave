using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBlock : MonoBehaviour
{
    [SerializeField] BoxCollider2D bc;


    //çUåÇí ÇËî≤ÇØëjé~
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GManager.instance.fallAttack)
        {
            if (GManager.instance.isAttack && !bc.enabled)
            {
                bc.enabled = true;
            }
            else if (!GManager.instance.isAttack && bc.enabled)
            {
                bc.enabled = false;
            }
        }
        else if (bc.enabled)
        {
            bc.enabled = false;
        }
    }
}
