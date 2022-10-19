using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBlock : MonoBehaviour
{
    [SerializeField] BoxCollider2D bc;


    //UŒ‚’Ê‚è”²‚¯‘jŽ~
    // Update is called once per frame
    //   async UniTaskVoid FixedUpdate()
    // {
    //  await UniTask.RunOnThreadPool(() => attackBlock());
    //}
    private void FixedUpdate()
    {
        transform.position = transform.root.position;
        attackBlock();
    }

    void attackBlock()
    {
        if (!GManager.instance.fallAttack)
        {
            if ((GManager.instance.isGuard || GManager.instance.isAttack || (!GManager.instance.blowDown && GManager.instance.isDamage)) && !bc.enabled)
            {
                bc.enabled = true;
            }
            else if ((!GManager.instance.isGuard && !GManager.instance.isAttack && !GManager.instance.isDamage) && bc.enabled)
            {
                bc.enabled = false;
            }
        }
        else if (bc.enabled)
        {
            bc.enabled = false;
        }
    }

    void attackBlock2()
    {

        
    }
}
