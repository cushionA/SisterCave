using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode;
public class AttackData
{
	//��������U���̐��l
	//-------------------------------------------------
	[HideInInspector]
	public float phyAtk;//�����U���B���ꂪ1�ȏ�Ȃ�a���Ō��̑�������

	[HideInInspector]
	public float holyAtk;//���B�ؗ͂ƌ������֌W�B�����͂�����
	[HideInInspector]
	public float darkAtk;//�ŁB���͂ƋZ�ʂ��֌W
	[HideInInspector]
	public float fireAtk;//����
	[HideInInspector]
	public float thunderAtk;//����

	[HideInInspector]
	public float mValue;

	///<summary>
	/// �ŏI�I�ȉ񕜗�
	/// </summary>
	[HideInInspector]
	public float recoverAmount;

	/// <summary>
	/// �A�[�}�[���
	/// </summary>
	[HideInInspector]
	public float shock;//�A�[�}�[���

	/// <summary>
	/// �d���U�����ǂ���
	/// </summary>
	[HideInInspector]
	public bool isHeavy = false;

	/// <summary>
	/// �U������
	/// </summary>
	[HideInInspector]
	public byte _attackType;//�A�[�}�[���

	/// <summary>
	/// �U������
	/// </summary>
	[HideInInspector]
	public byte _phyType;//�A�[�}�[���




	/// <summary>
	/// �^����_���[�W�S�̂̃o�t
	/// </summary>
	[HideInInspector]
	public float attackBuff = 1;


	//-------------------------------------------------

	//������΂��邩�ǂ���
	[HideInInspector]
	public bool isBlow;

	//�e����邩�ǂ���
	[HideInInspector]
	public bool isLight;
	[HideInInspector]
	public Vector2 blowPower;
	//true.�p���B�s��
	[HideInInspector]
	public bool disParry;
	/// <summary>
	/// �p���B�̃A�[�}�[���ɑ΂����R
	/// </summary>
	[HideInInspector]
	public float _parryResist;
}
