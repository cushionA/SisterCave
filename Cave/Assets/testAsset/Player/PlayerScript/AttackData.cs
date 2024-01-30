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
    /// �U���{���v�Z�Ɏg�����l
    /// </summary>
	public struct AttackMultipler
	{
        /// <summary>
        /// �S�U���ɑ΂���{��
        /// </summary>
        public float allAtkMultipler;

        /// <summary>
        /// �U���{��
        /// </summary>
        public float phyAtkMultipler;
        /// <summary>
        /// ���U���{��
        /// </summary>
        public float holyAtkMultipler;
        /// <summary>
        /// �ōU���{��
        /// </summary>
        public float darkAtkMultipler;
        /// <summary>
        /// ���U���{��
        /// </summary>
        public float fireAtkMultipler;

        /// <summary>
        /// ���U���{��
        /// </summary>
        public float thunderAtkMultipler;
    }


    #endregion


    //��������U���̐��l
    //-------------------------------------------------

    /// <summary>
    /// �U���֘A�̃X�e�[�^�X
    /// </summary>
    [HideInInspector]
	public AttackStatus attackStatus;

    /// <summary>
    /// �U���{��
    /// </summary>
    public AttackMultipler multipler;


	///<summary>
	/// �ŏI�I�ȉ񕜗�
	/// </summary>
	[HideInInspector]
	public float recoverAmount;




	/// <summary>
	/// �^����_���[�W�S�̂̃o�t
	/// ���Z���ēn��
	/// </summary>
	[HideInInspector]
	public float attackBuff;


	//-------------------------------------------------

	/// <summary>
	/// ���[�V�����l�Ȃǂ̃f�[�^
	/// �U���Ɋ֘A
	/// ����Œl�n������
	/// </summary>
	public AttackValueBase.ActionData actionData;


}
