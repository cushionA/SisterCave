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

        //    [Header("アニメーターコントローラ。OArtsのみ盾側からアニメを代入")]
        ///<summary>
        ///アニメーターコントローラ
        ///装備切り替えのたびにOArtsは初期化してね
        /// </summary>
        //  public AnimatorController WeaponMotion;




        [Header("双刀武器かどうか")]
        public bool isTwin;
        [Header("魔術の触媒かどうか")]
        public bool isMagic;


        // public List<float> phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
        // public List<float> holyBase;//光。筋力と賢さが関係。生命力だから
        // public List<float> darkBase;//闇。魔力と技量が関係
        //  public List<float> fireBase;//魔力
        //  public List<float> thunderBase;//魔力
        // public float needPower;//必要筋力
        //   public float needSkill;//必要技量
        // public float needInt;//必要な賢さ

        // Start is called before the first frame update
        // public List<float> phyCut;//カット率
        // public List<float> holyCut;//光。
        //public List<float> darkCut;//闇。
        // public List<float> fireCut;//魔力
        //public List<float> thunderCut;//魔力

        //public List<float> guardPower;//受け値

        [HideInInspector] public float MagicAssistBase = 100;
        public float MagicAssist;//魔法威力修正
        public float castSkill = 1;

        //各能力補正

        public List<AnimationCurve> MAssistCurve;
        public AnimationCurve CastCurve;

        [HideInInspector] public float mValue;//攻撃するときリストの中からその都度モーション値設定

        [HideInInspector] public float atAromor;//攻撃するときリストの中からその都度攻撃アーマー設定

        [Header("強攻撃のチャージ時間")]
        public float chargeRes;
        //チャージ時間長くしたり減らしたりできる状態があるとおもろいからパブリック

        //  public int normalStamina;
        //   public int bigStamina;
        //  public int chargeStamina;
        //   public int airStamina;
        // public int normalStaminaT;
        //   public int bigStaminaT;
        //   public int chargeStaminaT;
        //  public int airStaminaT;
        //   public int artsStamina;

        //-----------------------------------------------モーション名の管理
        #region//モーション名
        /*
        [Header("片手弱攻撃のモーション名リスト")]
        /// <summary>
        /// 片手弱攻撃のモーション名リスト
        /// </summary>
         public List<string> smallName;

        [Header("片手強攻撃のモーション名リスト")]
        /// <summary>
        /// 片手強攻撃のモーション名リスト
        /// </summary>
         public List<string> bigName;

        [Header("片手ため攻撃のモーション名リスト")]
        /// <summary>
        /// 片手ため攻撃のモーション名リスト
        /// </summary>
        public List<string> maxName;

        [Header("片手空中弱+強の名前リスト")]
        /// <summary>
        /// 片手空中弱の名前リスト
        /// </summary>
         public List<string> airName;

        [Header("片手チャージモーションの名前リスト")]
        /// <summary>
        /// 片手チャージモーションリスト
        /// </summary>
         public List<string> chargeName;

        [Header("両手弱攻撃のモーション名リスト")]
        /// <summary>
        /// 両手弱攻撃のモーション名リスト
        /// </summary>
        public List<string> twinSmallName;

        [Header("両手強攻撃のモーション名リスト")]
        /// <summary>
        /// 両手強攻撃のモーション名リスト
        /// </summary>
        public List<string> twinBigName;

        [Header("両手ため攻撃のモーション名リスト")]
        /// <summary>
        /// 両手ため攻撃のモーション名リスト
        /// </summary>
        public List<string> twinMaxName;

        [Header("両手空中弱+強の名前リスト")]
        /// <summary>
        /// 両手空中弱の名前リスト
        /// </summary>
        public List<string> twinAirName;

        [Header("両手モーションの名前リスト")]
        /// <summary>
        /// 両手チャージモーションリスト
        /// </summary>
        public List<string> twinChargeName;

        [Header("武器固有モーションの名前リスト")]
        /// <summary>
        /// 武器固有モーションリスト
        /// </summary>
        public List<string> artsName;
        */
        #endregion

        //--------------------------------------------モーション値、追加アーマー、強靭削りの管理
        #region//両手
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

        [HideInInspector]
        public bool isLight;
        //軽い攻撃。盾にはじかれる

        //  public bool shieldAct;

        [HideInInspector] public bool twinHand;//後々拡張する。両手持ちしてるかどうか


        [HideInInspector]
        /// <summary>
        /// 吹っ飛ばし攻撃
        /// </summary>
        public bool isBlow;

        [HideInInspector]
        /// <summary>
        /// コンボかどうか
        /// </summary>
        public bool isCombo;

        // [HideInInspector]
        /// <summary>
        /// 衝突できる回数。毎回設定しなおす
        /// </summary>
        // public int hitLimmit = 1;

        [Header("吹っ飛ばす力")]
        [HideInInspector] public Vector2 blowPower;

        public float guardSpeed;//ガード中の移動速度

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
    }
}