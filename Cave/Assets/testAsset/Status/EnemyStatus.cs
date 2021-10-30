using DarkTonic.MasterAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "CreateEnemyStatus")]
public class EnemyStatus : CharacterStatus
{
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

	[Header("加速速度")]
	///<summary>
	///yこの値が大きいほど加速が早くなる
	///</summary>
	public Vector2 addSpeed;

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

	/*[Header("待機状態で振り向くか否か")]
	///<summary>
	///	真なら振り向く	
	///</summary>
	public bool waitDirectionChange;
	*/
	[Header("戦闘中の移動速度")]
	public Vector2 combatSpeed;

	[Header("攻撃状態で維持する距離")]
	///<summary>
	///戦闘中はこの距離を目指して動く
	///</summary>
	public List<Vector2> agrDistance;

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

	/*
	[Header("歩く速さ")]
	///<summary>
	///歩き速度
	///</summary>
	public Vector2 walkSpeed;*/

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

	[Header("横のジャンプ力")]
	///<summary>
	///AddForceでやる。空飛ぶキャラはきもちジャンプ力低め？
	///</summary>
	public float jumpMove;

	[Header("縦のジャンプ力")]
	///<summary>
	///AddForceでやる。空飛ぶキャラはきもちジャンプ力低め？
	///</summary>
	public float jumpPower;

	[Header("ジャンプの継続時間")]
	public float jumpRes = 0.4f;

	[Header("ジャンプのクールタイム")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public float jumpCool;

	[Header("回避時の速度")]
	public Vector2 avoidSpeed;

	[Header("回避の継続時間")]
	public float avoidRes;

	[Header("回避のクールタイム")]
	public float avoidCool;

	[Header("デフォルトの重力")]
	///<summary>
	///最初の重力。飛行キャラは小さく
	///</summary>
	public float firstGravity;//重力。gravityScaleに入れる？　空中キャラはダウンした時だけ重力入る

	[Header("ダウンする時間")]
	///<summary>
	///ダウンする時間
	///</summary>
	public float downRes;

	[Header("負の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);

	[Header("正の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);


	[Header("射撃オブジェクトリスト")]
	///<summary>
	///射撃で打ち出すオブジェクト
	///</summary>
	public List<EnemyMagic> enemyFire;//エネミーの魔法

	[Header("ドッグパイル")]
	///<summary>
	///移動の中心となるオブジェクト。敵を引き連れたり
	///</summary>
	public GameObject dogPile;



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

	public bool strong;//強敵か否か
	public bool boss;

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


	public KindofEnemy kind;

    public enum AttackType
    {
        Slash,//斬撃
        Stab,//刺突
        Strike//打撃
    }


	[HideInInspector]
	public enum WeakPoint
	{
		Slash,
		Stab,
		Strike,
		Holy,
		Dark,
		Fire,
		Thunder,
		Poison
	}

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

	[Header("攻撃名")]
	public List<string> attackName;
	[Header("選択可能な番号")]
	///<Summary>
	///コンボの起点と非コンボの単発攻撃のみ
	///ここからランダムにアタックナンバーに入れたりする
	///</Summary>
	public List<int> serectableNumber;

	[Header("参照用の攻撃力")]
	/// <summary>
	/// 参照用の攻撃力
	/// </summary>
	public int atkDisplay;
	[Header("参照用の防御力")]
	/// <summary>
	/// 参照用の防御力
	/// </summary>
	public int defDisplay;

	public List<EnemyValue> atValue;


	/// <summary>
	/// //パリィ可能攻撃
	/// </summary>
	[HideInInspector]
	public bool parriable;

	[Header("カメラ範囲に行動を拘束されない")]
	public bool unBaind;

	[Header("攻撃属性")]
	[Tooltip("物理2,4,8、以下聖16闇32炎64雷128")]
	///<Sammary>
	/// 攻撃の属性
	/// 物理統合。以下聖闇炎雷
	///</Sammary>>
	public byte attackType;

	[Header("パリィ不可エフェクトのサイズ")]
	///<Sammary>
	/// このサイズでパリィ不可エフェクトが出る
	///</Sammary>>
	public Vector3 disparriableScale;

	[Header("ガード音")]
	[SoundGroup]

	/// <summary>
	// ガード時の音
	/// </summary>
	public string guardSound;

}
