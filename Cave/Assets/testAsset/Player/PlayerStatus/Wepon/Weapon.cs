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

        #region 定義

        /// <summary>
        /// 魔法関連のステータス
        /// 武器の詠唱速度補正や威力補正によって変わる
        /// モノによっては普通より低くなったりする
        /// </summary>
        public struct MagicMultipler
        {

            /// <summary>
            /// 詠唱速度倍率
            /// </summary>
            public float castSpeedMultipler;

            /// <summary>
            /// ダメージ倍率
            /// </summary>
            public float damageMultipler;

            /// <summary>
            /// 効果時間倍率
            /// </summary>
            public float effectTimeMultipler;

            /// <summary>
            /// 消費mpの減少倍率
            /// </summary>
            public float mpMultipler;

        }



        #endregion

        [Header("双刀武器かどうか")]
        public bool isTwin;
        [Header("魔術の触媒かどうか")]
        public bool isMagic;



        /// <summary>
        /// 魔法関連の倍率
        /// </summary>
        public MagicMultipler magicMultipler;
        public float castSkill = 1;

        //各能力補正

        public AnimationCurve[] MAssistCurve;

        public AnimationCurve CastCurve;







        //--------------------------------------------モーション値、追加アーマー、強靭削りの管理
        #region//武器パラメータ
        [Header("片手弱攻撃の値")]
        /// <summary>
        /// 弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo sValue;

        [Header("強攻撃の値")]
        /// <summary>
        /// 強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo bValue;


        [Header("空中弱攻撃の値")]
        /// <summary>
        /// 空中弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo airValue;


        [Header("空中強攻撃の値")]
        /// <summary>
        /// 空中強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo strikeValue;



        [Header("両手弱攻撃の値")]
        /// <summary>
        /// 両手弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo twinSValue;


        [Header("両手強攻撃の値")]
        /// <summary>
        /// 両手強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo twinBValue;



        [Header("両手空中弱攻撃の値")]
        /// <summary>
        /// 両手空中弱攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo twinAirValue;


        [Header("空中強攻撃の値")]
        /// <summary>
        /// 両手空中強攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo twinStrikeValue;


        [Header("武器固有技の値")]
        /// <summary>
        /// 武器固有攻撃のXモーション値、Y追加アーマー、Z強靭削り
        /// </summary>
        public MotionChargeImfo artsValue;




        #endregion
        //------------------------------------------内部パラメータ






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