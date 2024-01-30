using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode;
using System;
using static Equip;

public struct AttackData
{

    #region

    /// <summary>
    /// 攻撃倍率計算に使う数値
    /// </summary>
	public struct AttackMultipler
	{
        /// <summary>
        /// 全攻撃に対する倍率
        /// </summary>
        public float allAtkMultipler;

        /// <summary>
        /// 攻撃倍率
        /// </summary>
        public float phyAtkMultipler;
        /// <summary>
        /// 聖攻撃倍率
        /// </summary>
        public float holyAtkMultipler;
        /// <summary>
        /// 闇攻撃倍率
        /// </summary>
        public float darkAtkMultipler;
        /// <summary>
        /// 炎攻撃倍率
        /// </summary>
        public float fireAtkMultipler;

        /// <summary>
        /// 雷攻撃倍率
        /// </summary>
        public float thunderAtkMultipler;
    }


    #endregion


    //ここから攻撃の数値
    //-------------------------------------------------

    /// <summary>
    /// 攻撃関連のステータス
    /// </summary>
    [HideInInspector]
	public AttackStatus attackStatus;

    /// <summary>
    /// 攻撃倍率
    /// </summary>
    public AttackMultipler multipler;


	///<summary>
	/// 最終的な回復量
	/// </summary>
	[HideInInspector]
	public float recoverAmount;




	/// <summary>
	/// 与えるダメージ全体のバフ
	/// 合算して渡す
	/// </summary>
	[HideInInspector]
	public float attackBuff;


	//-------------------------------------------------

	/// <summary>
	/// モーション値などのデータ
	/// 攻撃に関連
	/// 代入で値渡しする
	/// </summary>
	public AttackValueBase.ActionData actionData;


}
