using DarkTonic.MasterAudio;
using MoreMountains.InventoryEngine;

using UnityEngine;
using UnityEngine.AddressableAssets;

public class Magic : Item
{

	[Header("装備画面で選択できるか")]
	public bool selectable;

	[HideInInspector]
	public enum MagicType
	{
		Attack,//攻撃
		Recover,//回復
		Support,//支援
		help//条件で勝手に発動？
	}
	public MagicType mType;


	[HideInInspector]
	public enum FIREBULLET
	{
		ANGLE,//角度だけangleで指定できる
		HOMING,//完全追尾
		HOMING_Z,//指定した角度内に標的があるとき追尾。
		RAIN,//最初に発射地点から標的までの角度を計算してその角度で全弾撃ちおろす
		STOP,//動かない
	}
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

	public int magicLevel;


	public float phyBase;//物理攻撃。これが1以上なら斬撃打撃の属性つける
    public float holyBase;//光。筋力と賢さが関係。生命力だから
    public float darkBase;//闇。魔力と技量が関係
    public float fireBase;//魔力
    public float thunderBase;//魔力
	//public float recoverBase;//回復

	public float phyAtk;//物理攻撃。これが1以上なら斬撃打撃の属性つける
	public float holyAtk;//光。筋力と賢さが関係。生命力だから
	public float darkAtk;//闇。魔力と技量が関係
	public float fireAtk;//魔力
	public float thunderAtk;//魔力

	[Header("表示用攻撃力")]
	public float displayAtk;
	[Header("回復の基礎量")]
	public float recoverBase;//回復

	
	///<summary>
	/// 最終的な回復量
	/// </summary>
	public float recoverAmount;

	[Header("詠唱時間")]
	/// <summary>
	/// スキルや秘伝書（早口言葉）、技量で少なくなる？
	/// </summary>
	public float castTime;

	public float useMP;//消費MP
   [Header("効果時間")]
    public float effectTime;//効果時間
	[HideInInspector] public bool effectNow;//効果中かどうか

    [Header("射撃オブジェクトリスト")]
    ///<summary>
    ///生成する弾丸オブジェクト
    ///</summary>
    public AssetReference effects;



	//public Transform firePosition;//発射位置
    //各能力補正
	//これ触媒補正でいいっす
	//でもそれだと丸パクリじゃんねぇ
    public AnimationCurve powerCurve;
    public AnimationCurve skillCurve;
    public AnimationCurve intCurve;

    public float shock;//アーマー削り

    [Header("モーション値")]
    public float mValue;//攻撃するときリストの中からその都度モーション値設定。弾丸部分と爆発部分で威力違うとかできる
						//二つ目のエフェクトの起動時とかに

	public FIREBULLET fireType = FIREBULLET.HOMING;

	/// <summary>
	/// ヒット回数
	/// </summary>
	[HideInInspector]
	public int _hitLimit = 1;

	[Header("貫通弾")]
	///<summary>
	///オンにすれば弾が貫通
	///</summary>
	public bool penetration = false;

	[Header("弾丸の生存時間")]
	/// <summary>
	/// これだけ経つと弾丸が消える
	/// </summary>
	public float lifeTime = 3.0f;

	[Header("初期速度")]
	///<summary>
	///射出される速度
	///</summary>
	public float speedV = 10.0f;

	[Header("速度の加速度")]
	/// <summary>
	/// プラスにすればだんだん早く、マイナスにすれば遅く
	/// </summary>
	public float speedA = 0.0f;

	[Header("射出角度")]
	///<summary>
	///　最初に放たれる角度上とか斜めとか水平とか
	///</summary>
	public float angle = 0.0f;
	[Header("追尾時間")]
	/// <summary>
	/// 追いかける時間。最初だけ追尾させれば方向だけ合わせる
	/// </summary>
	public float homingTime = 0.0f;

	[Header("追尾の強さ")]
	///<summary>
	///追尾の強さ。プラスなら追尾。マイナスなら逸れる
	///</summary>
	public float homingAngleV = 180.0f;

	[Header("追尾の加速度")]
	/// <summary>
	/// これがプラスだと時間ごとに追尾力が上がる。マイナスなら下がる
	/// </summary>
	public float homingAngleA = 20.0f;

	[Header("射出開始までの時間")]
	/// <summary>
	/// これがあると一定秒数待つよ
	/// </summary>
	public float waitTime = 0f;

	[Header("弾丸生成までの時間")]
	/// <summary>
	/// これがあると一定秒数待つよ
	/// </summary>
	public float delayTime = 0f;

	[Header("子弾かどうか")]
	/// <summary>
	/// 子弾かどうか
	/// </summary>
	public bool isChild;

	/*	[Header("追加エフェクトの大きさ")]
		/// <summary>
		/// localScaleで設定
		/// </summary>
		public Vector3 hitEffectScale = Vector3.one;*/

	[Header("弾丸の回転")]
	public float rotateVt = 360.0f;

	[Header("回転オン")]
	/// <summary>
	/// 回転するか否か。真で回転
	/// </summary>
	public bool isRotate;

	[Header("弾丸の拡大率")]
	/// <summary>
	/// 大きくしたり小さくしたり
	/// </summary>
	public Vector2 bulletScaleV = Vector3.zero;

	[Header("弾丸の拡大の加速率")]
	/// <summary>
	/// だんだん大きくしたり
	/// </summary>
	public Vector2 bulletScaleA = Vector3.zero;

	[Header("吹き飛ばすかどうか")]
	/// <summary>
	/// //吹っ飛ばし攻撃
	/// </summary>
	public bool isBlow;

	[Header("子弾が吹き飛ばすかどうか")]
	/// <summary>
	/// //吹っ飛ばし攻撃
	/// </summary>
	public bool cBlow;

	[Header("位置をサーチして発生するか")]
	/// <summary>
	/// ターゲットの位置に発生するかどうか
	/// </summary>
	public bool isChaice;

	[Header("吹っ飛ばし力")]
	/// <summary>
	/// 吹っ飛ばす力
	/// </summary>
	public Vector2 blowPower;



	[Header("弾丸の数")]
	/// <summary>
	/// 一度にばらまかれる数
	/// </summary>
	public int bulletNumber;

	[Header("弾丸の縦位置")]
	/// <summary>
	/// ばらまかれる際の縦のランダム要素
	/// </summary>
	public int VRandom;

	[Header("弾丸の横位置")]
	/// <summary>
	/// ばらまかれる際の横のランダム要素
	/// </summary>
	public int HRandom;

	[Header("詠唱中のエフェクト")]
	/// <summary>
	// 詠唱中のエフェクト
	/// </summary>
	public AssetReference castEffect;

	[Header("詠唱終了のエフェクト")]
	/// <summary>
	// 詠唱中のエフェクト
	/// </summary>
	public AssetReference castBreak;

	/*	[Header("発生エフェクト")]
		/// <summary>
		// 発生時のエフェクト
		/// </summary>
		public AssetReference fireEffect;*/
	[Header("発生サウンド")]
	[SoundGroup]
	/// <summary>
	// 発生時のサウンド
	/// </summary>
	public string fireSound;

	[Header("始動サウンド")]
	[SoundGroup]

	/// <summary>
	// wait切れの時のサウンド
	/// </summary>
	public string moveSound;

	[Header("恒常サウンド")]
	[SoundGroup]
	
	/// <summary>
	// 存在してる間のサウンド
	/// </summary>
	public string existSound;

	[Header("衝突サウンド")]
	[SoundGroup]
	/// <summary>
	// 衝突時のサウンド
	/// </summary>
	public string hitSound;

	[HideInInspector]
	public enum FIRETYPE
	{
		NoCast,
		Short,
		Normal,
		Long,
		Rain,
		Set,//置く
		Special
	}
	[HideInInspector]
	public enum CASTTYPE
	{
		NoCast,
		Short,
		Normal,
		Long,
		Rain,
		Set,
		Special
	}


	[Header("発動モーション名")]
	/// <summary>
	// 発動のアニメ
	/// </summary>
	public FIRETYPE FireType;

	[Header("詠唱モーション名")]
	/// <summary>
	// 詠唱のアニメ
	/// </summary>
	public CASTTYPE castType;

	[Header("ヒット時のエフェクト")]
	/// <summary>
	/// こちらに爆発処理とか乗せる
	/// </summary>
	public AssetReference hitEffect;

	[Header("ヒット時のエフェクトあるか")]
	/// <summary>
	/// こちらに爆発処理とか乗せる
	/// </summary>
	public bool isHit;

	[Header("攻撃属性")]
	[Tooltip("物理1,2,4、以下聖8闇16炎32雷64")]
	///<Sammary>
	/// 攻撃の属性
	/// 物理統合、以下聖闇炎雷
	///</Sammary>>
	public byte attackType;

	[Header("初期回転")]
	///</Sammary>>
	public Vector3 startRotation;
}
