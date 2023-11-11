using DarkTonic.MasterAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static EnemyStatus;
using static FunkyCode.Rendering.Lightmap.LightSprite;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "CreateEnemyStatus")]
public class EnemyStatus : CharacterStatus
{

	#region 定義

	/// <summary>
	/// 攻撃を使用する条件をまとめた
	/// 優先度が高いものから先に積むべき
	/// </summary>
	[Serializable]
	public struct AttackBehavior
	{
        /// <summary>
        /// この攻撃を選べるモード
        /// </summary>
        [Header("どのモードで使うか")]
        public Mode _attackMode;

        /// <summary>
        /// この行動ができる間隔
        /// </summary>
        [Header("攻撃使用感覚")]
        public float coolTime;

        /// <summary>
        /// xからyの距離でこの攻撃を使う
        /// 00なら無視
        /// </summary>
        [Header("使用距離（00で無視）")]
        public Vector2 useDistance;

        /// <summary>
        /// xからyの高度差でこの攻撃を使う
        /// 00なら無視
        /// </summary>
        [Header("使用高度差（0で無視）")]
        public float useHeight;

        /// <summary>
        /// この攻撃を使える可能性
        /// </summary>
        [Header("使用確率")]
        public int probability;

        /// <summary>
        /// 選択する攻撃番号
        /// </summary>
        [Header("使用する攻撃")]
        public int attackNum;

		/// <summary>
		/// 技のレベル
		/// 高いほど優先的に使う
		/// </summary>
		public int skilLevel;
	}







    /// <summary>
    /// ターゲットに対してのバフ
    /// 
    /// </summary>
    [Serializable]
	public struct TargetBuff
	{
        [Header("狙う敵の所属")]
        public CharaType targetType;

        [Header("狙う敵の種別")]
        public KindofEnemy targetKind;

        /// <sum
        /// 加算倍率
        /// マイナスなら引かれる
        /// x + (x * ratio)
        /// プラスでもマイナスでも最低20は増減させる保証
        ///
        ///これは加算する数値にする
        ///初期値の半分ずつ増えてく
        ///
        //[Header("加算される倍率")]
        // public float addRatio;


        /// <summary>
        /// 標的への最初のヘイト
        /// </summary>
        [Header("加算かどうか。真なら減算")]
        public bool isDecrease;

		/// <summary>
		/// 標的への最初のヘイト
		/// </summary>
		[Header("ターゲットのヘイトの初期値")]
		public float initialHate;

	}

    /// <summary>
    /// ヘイトに影響する要素
    /// 情報は随時更新だが
    /// これらで影響を与えるのは一定間隔ごとにマネージャーが一括でやる
    /// ターゲットの数だけ
    /// 
    /// 距離はマネージャーは座標だけ持ってそれぞれの敵がmyPosiから計算
    /// 同一への敵視数は敵視レベルごとにターゲットに紐づけて保存してあるのを全て合わせた数
    /// 優先レベル強度は敵視レベルごとにターゲットに紐づけて保存してあるのを使う
	/// 
    /// </summary>
    public enum HateInfluence
	{
		交戦距離,//交戦距離以遠なら？
		距離順,
		HP割合,
		MP割合,
		強化状態,
		デバフ状態,
		攻撃力,
		防御力,


			//この辺は攻撃頻度抑えたいやつだよね
//		同一対象への敵視数,
	//	優先レベル強度,//自分より上のレベルが同じタゲの時にヘイト下がる

	}


	/// <summary>
	/// ターゲット選定時に他の敵に対して起こすイベント
	/// </summary>
	public enum TargetingEvent
	{
		なし,//なんもなし
		同標的の下位味方のヘイトを下げる,//同じ敵を見てる自分よりレベルが低いやつのヘイトを下げる
		下位味方の統率モード始動,//もし自分が一番レベルの高い個体なら発動可能。統率モード起動メソッドを実装してる味方に刺さる。統率モードになると同じ敵を狙うように
		味方全体の標的へのヘイトを急増,//警報系の敵が使う。周囲の敵、警報機能に登録した敵をアクティブにすると同時にこれを発動とか

	}


	/// <summary>
	/// ヘイトをかける原因と数を指定
	/// 
	/// </summary>
	[Serializable]
	public struct HateEffect
	{
		[Header("ヘイトに影響を与える要素")]
		public HateInfluence _influence;


		[Header("真なら以上、以遠、多い高い順")]
        public bool isHigh;


		//原則三人にするのでカウント無し
		//[Header("何人まで補正かけるか")]
	//	[Tooltip("上限30をその人数で割っていく三人で二人目なら20とか")]
		//public int count;

    }





    /*
#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(HateEffect))]
	public class HateEffectDrawer : PropertyDrawer
	{


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            // ヘッダーを表示する
            EditorGUILayout.LabelField(label.text);
            // ヘッダーを表示する
            //EditorGUILayout.LabelField(label.text, EditorGUI.LabelField);

            var influenceProp = property.FindPropertyRelative("_influence");

			EditorGUI.PropertyField(position, influenceProp);

            var xProp = property.FindPropertyRelative("x");
            var yProp = property.FindPropertyRelative("y");
            var zProp = property.FindPropertyRelative("z");
            float height = EditorGUIUtility.singleLineHeight;

            var headerRect = new Rect(position.x, position.y, position.width, height);
            var influenceRect = new Rect(position.x, position.y + height, position.width, height);
            var xRect = new Rect(position.x, position.y + height * 5, position.width, height);
            var yRect = new Rect(position.x, position.y + height * 10, position.width, height);
            var zRect = new Rect(position.x, position.y + height * 15, position.width, height);

            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);
            EditorGUI.PropertyField(influenceRect, influenceProp);
            EditorGUI.PropertyField(xRect, xProp);
            EditorGUI.PropertyField(yRect, yProp);
            EditorGUI.PropertyField(zRect, zProp);

        }

    }

#endif
	*/


    /// <summary>
    /// ヘイト管理に使う
    /// この条件に沿ってターゲットにヘイト倍率かけていく
    /// 基本的にターゲットにする敵に対しては毎回ヘイトチェックの時にヘイトが増えていく
    /// </summary>
    [Serializable]
    public struct HateCondition
	{

        /// <summary>
        /// どれくらいの間隔でターゲット変えるか
        /// </summary>
        [Header("ターゲット変更間隔")]
        public float TargetChangeTime;

        /// <summary>
        /// 優先的に狙う標的
        /// </summary>
        [Header("標的リスト")]
        public TargetBuff[] priorityTarget;


		[Header("ヘイト管理に関する条件")]
		public HateEffect[] hateEffect;

		
        /// <summary>
        /// 同じ標的を狙ってる味方が何人いると攻撃をロックするか
		/// 0は無視
		/// ロックされるとロックモード起動になる
		/// 解除条件はそれぞれ？　それか近い順にする？　近づくまでは
        /// </summary>
        [Header("ロックに必要な同じ標的の味方の数。0は無視")]
        public int attackLockCount;

		[Header("敵視のレベル")]
		public hateLevel level;

		[Header("ターゲット切り替え時のイベント")]
		public TargetingEvent _event;
    }




    //落とすお金
     [SerializeField]List<Item> dropItem;


	[HideInInspector]public enum MoveState
	{
		accessWalk,//接近
		accessDash,
		leaveDash,//離れる
		leaveWalk,
		stay,//止まる
		escape,//逃げる
		wakeup//デフォルト

	}

    #endregion

    #region ステータス

    public float Atk;
	//　無属性
	public float phyAtk;
	//神聖
	public float holyAtk;
	//闇
	public float darkAtk;
	//炎
	public float fireAtk;
	//雷
	public float thunderAtk;


	public float phyCut;//カット率
	public float holyCut;//光。
	public float darkCut;//闇。
	public float fireCut;//魔力
	public float thunderCut;//魔力

	public float guardPower;//受け値

	[Header("移動設定判断")]
	///<summary>
	///移動ステート切り替え
	///</summary>
	public float judgePace = 3.0f;

	[Header("鎧着てるかどうか")]
	/// <summary>
	/// 鎧着てるかどうか
	/// </summary>
	 public bool isMetal;//鎧着てるかどうか


	public Equip.GuardType shieldType;


    /// <summary>
    /// 体の大きさ
    /// </summary>
    public MyCode.SoundManager.SizeTag _bodySize;

    #endregion



    #region 移動などの挙動


    [Header("待機中のスピード")]
	///<summary>
	///y方向に小刻みに小さな半径で揺らせば翼で飛んでるように見える
	///</summary>
	public Vector2 patrolSpeed;

	[Header("待機状態のテリトリー")]
	///<summary>
	///この間をうろうろしたりする
	///</summary>
	public Vector2 waitDistance;

	[Header("待機状態で縄張りの端に来た時停止する時間")]
	///<summary>
	///端まで来たら一定時間そのまま待ってる。
	///</summary>
	public float waitRes;

	[Header("戦闘中の移動速度")]
	public Vector2 combatSpeed;

	[Header("攻撃状態で維持する距離")]
	///<summary>
	///戦闘中はこの距離を目指して動く
	///</summary>
	public Vector2[] agrDistance;

	[Header("攻撃状態で歩く距離")]
	///<summary>
	///戦闘中はこの距離なら歩く
	///</summary>
	public Vector2 walkDistance;



	[Header("停止範囲")]
	///<summary>
	///目的距離にゆとり持たせる。
	///</summary>
	public float adjust;


	[Header("戦闘時のテリトリー")]
	///<summary>
	///この距離より遠くなるとプレイヤーを見失う
	///</summary>
	public Vector2 escapeDistance;//プレイヤーを取り逃がす距離

	[Header("追跡時間")]
	///<summary>
	///この時間が経過するまで、範囲外に逃げたプレイヤーを追跡する。
	///これと範囲を短くすればパイルに縛られた挙動。
	///</summary>
	public float chaseRes;//逃げたプレイヤーを追いかける時間




	[Header("ジャンプの継続時間、高さ")]
	public float jumpRes = 0.4f;

	[Header("ジャンプのクールタイム")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public float jumpCool = 2;

	[Header("ジャンプの回数")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public int jumpLimit;

	[Header("回避時の速度倍率")]
	public float avoidSpeed;

	[Header("回避の継続時間")]
	public float avoidRes;

	[Header("回避のクールタイム")]
	public float avoidCool;

	[Header("デフォルトの重力")]
	///<summary>
	///最初の重力。飛行キャラは小さく
	///</summary>
	public float firstGravity;//重力。gravityScaleに入れる？　空中キャラはダウンした時だけ重力入る



	//獲得金額
	public float money;

	/// <summary>
	/// 強敵か否か
	/// </summary>
	public bool strong;//強敵か否か

    #endregion


    [Header("射撃オブジェクトリスト")]
	///<summary>
	///射撃で打ち出すオブジェクト
	///</summary>
	public List<EnemyMagic> enemyFire;//エネミーの魔法






	/// <summary>
	/// 弱点
	/// </summary>
	public List<WeakPoint> wp;


	public Item GetDrop(int n)
    {
        if (n <= dropItem.Count - 1 && n >= 0)
        {
            return dropItem[n];
            //1から3までドロップ決めて1（0）を一番出やすくする？
        }
        return null;//この範囲を超えたら当然ドロップはなし
    }



	[Header("汎用アクションリスト")]
	///<summary>
	///攻撃以外のモーション
	///ダウン、落下、ノックバック、吹っ飛び、ガード、回避、ガード、ガード歩き、ガード後ずさり
	///walkSpeedでif(isGuard)みたいなんで
	///後ずさり、歩き、走り、ジャンプ（飛行キャラは全部同じValue入れる？）
	///待機、起動、構え、はじかれ、ガードブレイク
	///</summary>
	public Dictionary<string, string> motionIndex;


	[Header("選択可能な番号")]
	///<Summary>
	///コンボの起点と非コンボの単発攻撃のみ
	///ここからランダムにアタックナンバーに入れたりする
	///</Summary>
	public List<int> serectableNumber;



    /// <summary>
    /// 使用する攻撃のデータ
    /// </summary>
    [Header("使用する攻撃のデータ")]
    public List<EnemyValue> atValue;

    /// <summary>
    /// 攻撃行動の条件
    /// </summary>
    [Header("攻撃発動の条件")]
    public AttackBehavior[] AttackCondition;

	[Header("攻撃頻度")]
	public float attackFrquency = 0;

    /// <summary>
    /// モードチェンジの条件
    /// </summary>
    [Header("モードチェンジの条件")]
    public ModeBehavior[] modeChangeCondition;





    [Header("ヘイト管理関係の条件")]
    public HateCondition hateBehaivior;

	[Header("カメラ範囲に行動を拘束されない")]
	public bool unBaind;
    
	[Header("パリィ不可エフェクトのサイズ")]
	///<Sammary>
	/// このサイズでパリィ不可エフェクトが出る
	///</Sammary>>
	public Vector3 disparriableScale;



}
