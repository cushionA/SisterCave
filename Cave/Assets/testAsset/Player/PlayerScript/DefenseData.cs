using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseData
{


    public float phyCut;//�J�b�g��
	public float holyCut;//���B
	public float darkCut;//�ŁB
	public float fireCut;//����
	public float thunderCut;//����

	public float guardPower;//�󂯒l

    //�@�������h��́B�̗͂ŏオ��
    public float Def = 70;
    //�h�˖h��B�ؗ͂ŏオ��
    public float pierDef = 70;
    //�Ō��h��A�Z�ʂŏオ��
    public float strDef = 70;
    //�_���h��A�؂ƌ����ŏオ��B
    public float holyDef = 70;
    //�Ŗh��B�����ŏオ��
    public float darkDef = 70;
    //���h��B�����Ɛ����ŏオ��
    public float fireDef = 70;
    //���h��B�����Ǝ��v�ŏオ��B
    public float thunderDef = 70;

    /// <summary>
    /// �v���C���[�݂̂���
    /// </summary>
    public bool nowParry;

    public bool isGuard;

    /// <summary>
    /// true�̎��U�����󂯂�ƃ_���[�W�������Ȃ�B
    /// �e��̂�calc�Ō��肷��B
    /// </summary>
    public bool isDangerous;

    public bool attackNow;

    public float nowArmor;
}
