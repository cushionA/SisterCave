using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Magic : Item
{
	[HideInInspector]
	public enum FIREBULLET
	{
		ANGLE,//角度だけangleで指定できる
		HOMING,//完全追尾
		HOMING_Z,//指定した角度内に標的があるとき追尾。
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


    public float phyBase;//物理攻撃。これが1以上なら斬撃打撃の属性つける
    public float holyBase;//光。筋力と賢さが関係。生命力だから
    public float darkBase;//闇。魔力と技量が関係
    public float fireBase;//魔力
    public float thunderBase;//魔力
	public float recoverBase;//回復

	public float phyAtk;//物理攻撃。これが1以上なら斬撃打撃の属性つける
	public float holyAtk;//光。筋力と賢さが関係。生命力だから
	public float darkAtk;//闇。魔力と技量が関係
	public float fireAtk;//魔力
	public float thunderAtk;//魔力

	public float recoverAmount;//回復

	/// <summary>
	/// 詠唱時間。スキルや秘伝書（早口言葉）、技量で少なくなる？
	/// </summary>
	public float castTime;

	public float useMP;//消費MP

    public float effectTime;//効果時間
	[HideInInspector] public bool effectNow;//効果中かどうか

    [Header("射撃オブジェクトリスト")]
    ///<summary>
    ///生成する弾丸オブジェクト
    ///</summary>
    public AssetReference effects;



	public Transform firePosition;//発射位置
    //各能力補正
    public AnimationCurve powerCurve;
    public AnimationCurve skillCurve;
    public AnimationCurve intCurve;

    public float shock;//アーマー削り

    public List<float> motionValue;
    [HideInInspector]
    public float mValue;//攻撃するときリストの中からその都度モーション値設定。弾丸部分と爆発部分で威力違うとかできる
						//二つ目のエフェクトの起動時とかに

	public FIREBULLET fireType = FIREBULLET.HOMING;

	[Header("ヒット時のノックバック")]
	///<summary>
	///当たった時にノックバックさせる。
	///</summary>
	public Vector2 attackNockBackVector;

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

	[Header("ヒット時のエフェクト")]
	/// <summary>
	/// こちらに爆発処理とか乗せる
	/// </summary>
	public AssetReference hitEffect;

	[Header("追加エフェクトの大きさ")]
	/// <summary>
	/// localScaleで設定
	/// </summary>
	public Vector3 hitEffectScale = Vector3.one;
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
	public Vector3 bulletScaleV = Vector3.zero;

	[Header("弾丸の拡大の加速率")]
	/// <summary>
	/// だんだん大きくしたり
	/// </summary>
	public Vector3 bulletScaleA = Vector3.zero;

	/// <summary>
	/// //吹っ飛ばし攻撃
	/// </summary>
	public bool isBlow;

	[Header("吹っ飛ばし力")]
	/// <summary>
	/// 吹っ飛ばす力
	/// </summary>
	public Vector2 blowPower;

	/// <summary>
	/// 衝突できる回数。毎回設定しなおす
	/// </summary>
	public int hitLimmit = 1;

	[Header("弾丸の数")]
	/// <summary>
	/// 一度にばらまかれる数
	/// </summary>
	public int bulletNumber;
}
