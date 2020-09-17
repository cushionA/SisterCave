using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "CreateEnemyStatus")]
public class EnemyStatus : CharacterStatus
{
    public float money;
    //落とすお金
     [SerializeField]List<Item> dropItem;
    public enum KindofEnemy
    {
        Soldier,//陸の雑兵
        Fly,//飛ぶやつ
        Shooter,//遠距離
        Knight,//盾持ち
        Trap//待ち構えてるやつ
    }
    public KindofEnemy kind;

    public enum AttackType
    {
        Slash,//斬撃
        Stab,//刺突
        Strike//打撃
    }
    public AttackType atType;

    public Item GetDrop(int n)
    {
        if (n <= dropItem.Count - 1 && n >= 0)
        {
            return dropItem[n];
            //1から3までドロップ決めて1（0）を一番出やすくする？
        }
        return null;//この範囲を超えたら当然ドロップはなし
    }

}
