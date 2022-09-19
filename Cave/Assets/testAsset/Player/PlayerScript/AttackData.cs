using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode;
public class AttackData
{
	//ここから攻撃の数値
	//-------------------------------------------------
	[HideInInspector]
	public float phyAtk;//物理攻撃。これが1以上なら斬撃打撃の属性つける

	[HideInInspector]
	public float holyAtk;//光。筋力と賢さが関係。生命力だから
	[HideInInspector]
	public float darkAtk;//闇。魔力と技量が関係
	[HideInInspector]
	public float fireAtk;//魔力
	[HideInInspector]
	public float thunderAtk;//魔力

	[HideInInspector]
	public float mValue;

	///<summary>
	/// 最終的な回復量
	/// </summary>
	[HideInInspector]
	public float recoverAmount;

	/// <summary>
	/// アーマー削り
	/// </summary>
	[HideInInspector]
	public float shock;//アーマー削り

	/// <summary>
	/// 重い攻撃かどうか
	/// </summary>
	[HideInInspector]
	public bool isHeavy = false;

	/// <summary>
	/// 攻撃属性
	/// </summary>
	[HideInInspector]
	public byte _attackType;//アーマー削り

	/// <summary>
	/// 攻撃属性
	/// </summary>
	[HideInInspector]
	public byte _phyType;//アーマー削り




	/// <summary>
	/// 与えるダメージ全体のバフ
	/// </summary>
	[HideInInspector]
	public float attackBuff = 1;


	//-------------------------------------------------

	//吹き飛ばせるかどうか
	[HideInInspector]
	public bool isBlow;

	//弾かれるかどうか
	[HideInInspector]
	public bool isLight;
	[HideInInspector]
	public Vector2 blowPower;
	//true.パリィ不可
	[HideInInspector]
	public bool disParry;
	/// <summary>
	/// パリィのアーマー削りに対する抵抗
	/// </summary>
	[HideInInspector]
	public float _parryResist;
}
