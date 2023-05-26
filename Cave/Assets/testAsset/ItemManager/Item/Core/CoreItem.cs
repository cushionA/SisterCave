using System;
using UnityEngine;
using MoreMountains.InventoryEngine;
using System.Collections.Generic;
using PathologicalGames;

[Serializable]
[CreateAssetMenu(fileName = "CoreItem", menuName = "CreateCore")]
public class CoreItem : InventoryItem
{
    //まぁコア的な処理を

    //アーマー、HP、MP、詠唱時間、各種攻撃力、各種防御力、装備重量、時には移動速度、ジャンプ時間も？、あとは盾受けした時のダメージ削減倍率や
    //ものによっては特殊効果つけてもいいかも
    //一例（二段ジャンプ、空中回避で短距離ワープ（いわゆる空中回避。ワープなのはモーションとかコード作りたくないから）、エフェクト出てガードが強化される、パリィ持続時間が倍率強化される、パリィ時数秒攻撃力増加、カウンターダメージが上昇）
    //さらに（アーマー削りが強くなる、魔法扱いの追加エフェクトが攻撃エフェクトに置き換わる、）
    //あるいは特定装備を身に着けてる時、特定攻撃属性だけ特殊アクションとか特殊効果とか
    //ノーマルアニメスピードみたいな変数用意して移動速度の倍率とかを入れる？装備重量なども合わせて
    //    
    //特殊効果はスキルセットとして、スキルセットがあるかの条件判断を装備時にやる
    //数値いじったりフラグ動かしたりの処理。数値はもちろんフラグ立ってたらアクションが入れ替わる
    //

     public float additionalHp;

    //　MP
    public float additionalMp;

    public float additionalStamina;

    public float additionalWeight;

    //生命力
    public float additionalVitality;
    //持久力
    public float additionalEndurance;
    //MPと魔法もてる数
    public float additionalCapacity;
    //　力
    public float additionalPower;
    //技量
    public float additionalSkill;
    //　魔法力。賢さ
    public float additionalInt;




    //　無属性防御力。体力で上がる
    public float Def;
    //刺突防御。筋力で上がる
    public float pierDef;
    //打撃防御、技量で上がる
    public float strDef;
    //神聖防御、筋と賢さで上がる。
    public float holyDef;
    //闇防御。賢さで上がる
    public float darkDef;
    //炎防御。賢さと生命で上がる
    public float fireDef;
    //雷防御。賢さと持久で上がる。
    public float thunderDef;



    //isGuardの時使う

    //小怯み。普段は基本ゼロ。攻撃時だけ
    public float additionalArmor;
    //   [HideInInspector]public float nowArmor;

    //public float nockBackPower;

    //大怯み、吹っ飛び。基本均一でアーマー値を足す

    // public float attackBuff = 1.0f;
    //攻撃バフ値
    //[HideInInspector] public float defBuff;防御力は直接加算

    public List<EffectCondition> _useList;
    public List<PrefabPool> _usePrefab;
    //  public enum 

}
