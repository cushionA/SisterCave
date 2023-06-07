using System;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;


namespace MyCode
{
    [Serializable]
[CreateAssetMenu(fileName = "Weapon", menuName = "CreateWeapon")]
    public class Weapon : Equip
    {

        [Header("双刀武器かどうか")]
        public bool isTwin;
        [Header("魔術の触媒かどうか")]
        public bool isMagic;



        [HideInInspector] public float MagicAssistBase = 100;
        public float MagicAssist;//魔法威力修正
        public float castSkill = 1;

        //各能力補正

        public List<AnimationCurve> MAssistCurve;
        public AnimationCurve CastCurve;




        //チャージ時間長くしたり減らしたりできる状態があるとおもろいからパブリック
        [Header("強攻撃のチャージ時間")]
        public float chargeRes;




        //--------------------------------------------モーション値、追加アーマー、強靭削りの管理
        #region//武器パラメータ
        [Header("弱攻撃の値")]
        /// <summary>
        /// 弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> sValue;

        [Header("強攻撃の値")]
        /// <summary>
        /// 強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> bValue;

        [Header("ため攻撃の値")]
        /// <summary>
        /// 強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> chargeValue;

        [Header("空中弱攻撃の値")]
        /// <summary>
        /// 空中弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> airValue;

        [Header("空中強攻撃の値")]
        /// <summary>
        /// 空中強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> strikeValue;

        [Header("両手弱攻撃の値")]
        /// <summary>
        /// 両手弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> twinSValue;

        [Header("強攻撃の値")]
        /// <summary>
        /// 両手強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> twinBValue;

        [Header("ため攻撃の値")]
        /// <summary>
        /// 両手ため攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> twinChargeValue;

        [Header("両手空中弱攻撃の値")]
        /// <summary>
        /// 両手空中弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> twinAirValue;

        [Header("空中強攻撃の値")]
        /// <summary>
        /// 両手空中強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> twinStrikeValue;

        [Header("武器固有技の値")]
        /// <summary>
        /// 武器固有攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public List<AttackValue> artsValue;
        #endregion
        //------------------------------------------内部パラメータ


        [HideInInspector] public bool twinHand;//後々拡張する。両手持ちしてるかどうか





        /// <summary>
        /// この武器で使うアニメーターコントローラー
        /// 0が片手持ち、1が両手持ち
        /// なので片手持ちの0の方に盾のアニメを入れる
        /// </summary>
        public RuntimeAnimatorController[] _useContoroller;

        /// <summary>
        /// コアが合致した場合などに入れる武器
        /// </summary>
        public Weapon _alterWeapon;

        /// <summary>
        /// 特殊相性のコア
        /// </summary>
        public CoreItem ExCore;

        [Header("盾固有技のアニメーション")]
        ///<summary>
        ///盾固有技のアニメ
        /// </summary>
        public AnimationClip[] artsAnime;
    }
}