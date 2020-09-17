using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "Wepon", menuName = "CreateWepon")]

public class Wepon : Item
{
    public int wLevel = 0;

    public enum AttackType
    {
        Slash,//斬撃。ほどほどに通るやつが多い
        Stab,//刺突。弱点のやつと耐えるやつがいる
        Strike//打撃。弱点のやつと耐えるやつがいる。アーマーとひるませ強く
    }
    /// <summary>
    /// 剣で刺突するときとかはアニメイベントで変える
    /// </summary>
    public AttackType atType;
    

    public List<float> phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
    public List<float> holyBase;//光。筋力と賢さが関係。生命力だから
    public List<float> darkBase;//闇。魔力と技量が関係
    public List<float> fireBase;//魔力
    public List<float> thunderBase;//魔力
    public List<float> needPower;//必要筋力
    public List<float> needSkill;//必要技量
    public List<float> needInt;//必要な賢さ


    //各能力補正
    public List<AnimationCurve> powerCurve;
    public List<AnimationCurve> skillCurve;
    public List<AnimationCurve> intCurve;

    public List<float> motionValue;
    [HideInInspector]public float mValue;//攻撃するときリストの中からその都度モーション値設定

    public List<float> attackAromor;
    [HideInInspector] public float atAromor;//攻撃するときリストの中からその都度攻撃アーマー設定

}
