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
    /// UŒ‚”{—¦ŒvZ‚Ég‚¤”’l
    /// </summary>
	public struct AttackMultipler
	{
        /// <summary>
        /// ‘SUŒ‚‚É‘Î‚·‚é”{—¦
        /// </summary>
        public float allAtkMultipler;

        /// <summary>
        /// UŒ‚”{—¦
        /// </summary>
        public float phyAtkMultipler;
        /// <summary>
        /// ¹UŒ‚”{—¦
        /// </summary>
        public float holyAtkMultipler;
        /// <summary>
        /// ˆÅUŒ‚”{—¦
        /// </summary>
        public float darkAtkMultipler;
        /// <summary>
        /// ‰ŠUŒ‚”{—¦
        /// </summary>
        public float fireAtkMultipler;

        /// <summary>
        /// —‹UŒ‚”{—¦
        /// </summary>
        public float thunderAtkMultipler;
    }


    #endregion


    //‚±‚±‚©‚çUŒ‚‚Ì”’l
    //-------------------------------------------------

    /// <summary>
    /// UŒ‚ŠÖ˜A‚ÌƒXƒe[ƒ^ƒX
    /// </summary>
    [HideInInspector]
	public AttackStatus attackStatus;

    /// <summary>
    /// UŒ‚”{—¦
    /// </summary>
    public AttackMultipler multipler;


	///<summary>
	/// ÅI“I‚È‰ñ•œ—Ê
	/// </summary>
	[HideInInspector]
	public float recoverAmount;




	/// <summary>
	/// —^‚¦‚éƒ_ƒ[ƒW‘S‘Ì‚Ìƒoƒt
	/// ‡Z‚µ‚Ä“n‚·
	/// </summary>
	[HideInInspector]
	public float attackBuff;


	//-------------------------------------------------

	/// <summary>
	/// ƒ‚[ƒVƒ‡ƒ“’l‚È‚Ç‚Ìƒf[ƒ^
	/// UŒ‚‚ÉŠÖ˜A
	/// ‘ã“ü‚Å’l“n‚µ‚·‚é
	/// </summary>
	public AttackValueBase.ActionData actionData;


}
