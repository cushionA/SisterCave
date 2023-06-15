using DarkTonic.MasterAudio;
using MoreMountains.InventoryEngine;
using PathologicalGames;
using RenownedGames.Apex;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Magic : Item
{


    #region 定義

	/// <summary>
	/// 魔法のタイプ
	/// </summary>
	public enum MagicType
	{
		Attack,//攻撃
		Recover,//回復
		Support,//支援
		help//条件で勝手に発動？
	}


	public enum FIREBULLET
	{
		ANGLE,//角度だけangleで指定できる
		HOMING,//完全追尾
		HOMING_Z,//指定した角度内に標的があるとき追尾。
		RAIN,//最初に発射地点から標的までの角度を計算してその角度で全弾撃ちおろす
		STOP,//動かない
	}

	/// <summary>
	/// 弾丸のステータスで変わんないとこだけまとめる
	/// </summary>
	 [Serializable]
	public struct BMoveStatus
    {

	public FIREBULLET fireType;

		[Header("弾丸の生存時間")]
		/// <summary>
		/// これだけ経つと弾丸が消える
		/// </summary>
		public float lifeTime;
	
		[Header("初期速度")]
	///<summary>
	///射出される速度
	///</summary>
	public float speedV;

	[Header("速度の加速度")]
	/// <summary>
	/// プラスにすればだんだん早く、マイナスにすれば遅く
	/// </summary>
	public float speedA;

	[Header("射出角度")]
	///<summary>
	///　最初に放たれる角度上とか斜めとか水平とか
	///</summary>
	public float angle;
	[Header("追尾時間")]
	/// <summary>
	/// 追いかける時間。最初だけ追尾させれば方向だけ合わせる
	/// </summary>
	public float homingTime;

	[Header("追尾の強さ")]
	///<summary>
	///追尾の強さ。プラスなら追尾。マイナスなら逸れる
	///</summary>
	public float homingAngleV;

	[Header("追尾の加速度")]
	/// <summary>
	/// これがプラスだと時間ごとに追尾力が上がる。マイナスなら下がる
	/// </summary>
	public float homingAngleA;

	[Header("射出開始までの時間")]
	/// <summary>
	/// これがあると一定秒数待つよ
	/// </summary>
	public float waitTime;

	[Header("弾丸の回転")]
	public float rotateVt;

		[Header("回転オン")]
		/// <summary>
		/// 回転するか否か。真で回転
		/// </summary>
		public bool isRotate;

		[Header("弾丸の拡大率")]
		/// <summary>
		/// 大きくしたり小さくしたり
		/// </summary>
		public Vector2 bulletScaleV;

		[Header("弾丸の拡大の加速率")]
		/// <summary>
		/// だんだん大きくしたり
		/// </summary>
		public Vector2 bulletScaleA;
	}

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
    #endregion



    #region 内部情報
    [Header("装備画面で選択できるか(子弾は無理)")]
	public bool selectable = true;

	/// <summary>
	/// 魔法のレベル
	/// </summary>
	[Header("魔法レベル")]
	public AttackValue.AttackLevel magicLevel;

	[Header("魔法属性")]
    /// <summary>
    /// 剣で刺突するときとかはアニメイベントで変える
    /// </summary>
    public MoreMountains.CorgiEngine.AtEffectCon.Element magicElement;

	[Header("物理属性")]
	/// <summary>
	/// 剣で刺突するときとかはアニメイベントで変える
	/// </summary>
	public Equip.AttackType phyElement;

	[Header("魔法の種別。回復など")]
	public MagicType mType;





	[Foldout("内部ステータス")]
	public float phyBase;//物理攻撃。これが1以上なら斬撃打撃の属性つける

	[Foldout("内部ステータス")]
	public float holyBase;//光。筋力と賢さが関係。生命力だから
	[Foldout("内部ステータス")]
	public float darkBase;//闇。魔力と技量が関係
	[Foldout("内部ステータス")]
	public float fireBase;//魔力
	[Foldout("内部ステータス")]
	public float thunderBase;//魔力

	[Foldout("内部ステータス")]
	[Header("表示用攻撃力。Atベースに威力倍率をかけて")]
	public float displayAtk;

	[Foldout("内部ステータス")]
	[Header("回復の基礎量")]
	public float recoverBase;//回復

	[Foldout("内部ステータス")]
	public float shock;//アーマー削り

	[Foldout("内部ステータス")]
	[Header("モーション値")]
    public float mValue;

	//各能力補正
	[Foldout("内部ステータス")]
	public AnimationCurve powerCurve;

	[Foldout("内部ステータス")]
	public AnimationCurve skillCurve;

	[Foldout("内部ステータス")]
	public AnimationCurve intCurve;

	[Foldout("内部ステータス")]
	[Header("効果時間")]
    public float effectTime;//効果時間

	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	public float phyAtk;//物理攻撃。これが1以上なら斬撃打撃の属性つける
	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	public float holyAtk;//光。筋力と賢さが関係。生命力だから
	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	public float darkAtk;//闇。魔力と技量が関係
	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	public float fireAtk;//魔力
	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	public float thunderAtk;//魔力
	[Foldout("補正乗せた後の数値インスペクタからいじらない")]
	///<summary>
	/// 最終的な回復量
	/// </summary>
	public float recoverAmount;



	[Foldout("発動コスト")]
	[Header("詠唱時間")]
	/// <summary>
	/// スキルや秘伝書（早口言葉）、技量で少なくなる？
	/// </summary>
	public float castTime;

	[Foldout("発動コスト")]
	[Header("消費MP")]
	public float useMP;//消費MP

    #endregion



    
    #region エフェクト関連

    [Foldout("エフェクト関連")]
	[Header("射撃オブジェクト")]
    ///<summary>
    ///生成する弾丸オブジェクト
    ///</summary>
    public ParticleSystem effects;

	[Foldout("エフェクト関連")]
	[Header("発射時のエフェクト")]
	/// <summary>
	/// いわゆるマズルフラッシュ
	/// </summary>
	public ParticleSystem flashEffect;

	[Foldout("エフェクト関連")]
	[Header("ヒット時のエフェクト")]
	/// <summary>
	/// こちらに爆発処理とか乗せる
	/// </summary>
	public ParticleSystem hitEffect;

	[Foldout("エフェクト関連")]
	/// <summary>
	/// 子弾丸の魔法
	/// </summary>
	public Magic childM;


	[Foldout("エフェクト関連")]
	[Tooltip("ヒット時とかのエフェクトも含むので配列")]
	[Header("この魔法のエフェクトのプレハブ")]
	public PrefabPool[] _usePrefab;

    #endregion


    #region 弾丸の動きについて


	[Foldout("挙動設定")]
	[Header("弾丸の動きの設定")]
	public BMoveStatus _moveSt;

	[Foldout("挙動設定")]
	[Header("位置をサーチして発生するか")]
	/// <summary>
	/// ターゲットの位置に発生するかどうか
	/// </summary>
	public bool isChaice;

	[Foldout("挙動設定")]
	[Header("弾丸の数")]
	/// <summary>
	/// 一度にばらまかれる数
	/// </summary>
	public int bulletNumber;

	[Foldout("挙動設定")]
	[Header("弾丸の縦位置")]
	/// <summary>
	/// ばらまかれる際の縦のランダム要素
	/// </summary>
	public int VRandom;

	[Foldout("挙動設定")]
	[Header("弾丸の横位置")]
	/// <summary>
	/// ばらまかれる際の横のランダム要素
	/// </summary>
	public int HRandom;

	[Foldout("挙動設定")]
	[Header("弾丸生成までの時間")]
	/// <summary>
	/// これがあると一定秒数待つよ
	/// </summary>
	public float delayTime = 0f;

	[Foldout("挙動設定")]
	[Header("初期回転")]
	///</Sammary>>
	public Vector3 startRotation;

	#endregion

	[HideInInspector] public bool effectNow;//効果中かどうか


    //弾丸の性質
    #region

	[Foldout("弾丸の性質")]
	[Header("子弾かどうか")]
	/// <summary>
	/// 子弾かどうか
	/// </summary>
	public bool isChild;

	[Foldout("弾丸の性質")]
	[Header("貫通弾")]
	///<summary>
	///オンにすれば弾が貫通
	///</summary>
	public bool penetration = false;


	[Foldout("弾丸の性質")]
	[Header("吹き飛ばすかどうか")]
	/// <summary>
	/// //吹っ飛ばし攻撃
	/// </summary>
	public bool isBlow;

	[Foldout("弾丸の性質")]
	[Header("子弾が吹き飛ばすかどうか")]
	/// <summary>
	/// //吹っ飛ばし攻撃
	/// </summary>
	public bool cBlow;

    /// <summary>
    /// ヒット回数
    /// </summary>
    [Foldout("弾丸の性質")]	
	[Header("ヒット回数")]
	public int _hitLimit = 1;


	[Foldout("弾丸の性質")]
	[Header("吹っ飛ばし力")]
	/// <summary>
	/// 吹っ飛ばす力
	/// </summary>
	public Vector2 blowPower;

	#endregion



	//サウンド、アニメーション設定
	#region

	[Foldout("サウンド・アニメ設定")]
	[Header("発生サウンド")]
	[SoundGroup]
	/// <summary>
	// 発生時のサウンド
	/// </summary>
	public string fireSound;

	[Foldout("サウンド・アニメ設定")]
	[Header("動き始めの音")]
	[SoundGroup]
	public string moveSound;

	[Foldout("サウンド・アニメ設定")]
	[Header("ずっと鳴る音")]
	[SoundGroup]
	/// <summary>
	// 存在してる間のサウンド
	/// </summary>
	public string existSound;

	[Foldout("サウンド・アニメ設定")]
	[Header("衝突サウンド")]
	[SoundGroup]
	/// <summary>
	// 衝突時のサウンド
	/// </summary>
	public string hitSound;

	[Foldout("サウンド・アニメ設定")]
	[Header("発動モーション名")]
	/// <summary>
	// 発動のアニメ
	/// </summary>
	public FIRETYPE FireType;

	[Foldout("サウンド・アニメ設定")]
	[Header("詠唱モーション名")]
	/// <summary>
	// 詠唱のアニメ
	/// </summary>
	public CASTTYPE castType;

    #endregion







}
