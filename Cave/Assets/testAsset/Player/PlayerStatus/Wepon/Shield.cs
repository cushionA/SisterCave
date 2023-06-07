using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Shield", menuName = "CreateShield")]
public class Shield : Equip
{




    [Header("ゲーム画面で表示するスプライト")]
    public Sprite[] back = new Sprite[2];


    public bool weaponArts;//武器の特殊技を使えるかどうか


    [Header("特殊攻撃の値")]
    /// <summary>
    /// 特殊攻撃のXモーション値、Y追加アーマー、Z強靭削り
    /// </summary>
    public List<AttackValue> artsValue;


    [Header("盾固有技のアニメーション")]
    ///<summary>
    ///盾固有技のアニメ
    /// </summary>
    public AnimationClip[] artsAnime;
}
