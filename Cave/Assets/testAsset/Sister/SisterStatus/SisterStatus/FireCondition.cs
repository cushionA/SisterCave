using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FireCondition
{

    /// <summary>
    /// ����͖��@���g�p������Ĕ�����s�킸�Ɏg�����߂̃L���b�V��
    /// ���̏����Ɩ��@�\���Ŕ��f�����͕̂ۑ����Ƃ�
    ///  �܌����������ɂ��Ă邩������̂͑�����AI�ݒ肵�Ȃ������Ƃ�����
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    public bool AutoWait;//�����Ŏg�p���@�̎g�pMP���񕜂���N�[���^�C�������
    public int CoolTime;//���@���g������̃N�[���^�C��

    [HideInInspector]
    public enum ActJudge
    {
        �a������,
        �h�ˑ���,
        �Ō�����,
        ������,
        �ő���,
        ������,
        ������,
        �ő���,
        �ړ����x�ቺ�U��,
        �U���͒ቺ�U��,
        �h��͒ቺ�U��,
        �����w��Ȃ�,
        �x���s���Ɉڍs,
        �񕜍s���Ɉڍs,
        �Ȃɂ����Ȃ�
    }

    public ActJudge condition = ActJudge.�Ȃɂ����Ȃ�;

    //[HideInInspector] public int percentage;
 //   [HideInInspector] public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum FirstCondition
    {
        �G�𐁂���΂�,
        �ђʂ���,
        �ݒu�U��,
        �͈͍U��,
        �ǔ�����,
        �T�[�`�U��,
        �w��Ȃ�

    }

    public FirstCondition firstCondition = FirstCondition.�w��Ȃ�;

    public enum AdditionalCondition
    {
        //  �G�̎�_,//��Ԉُ�܂�
�@�@�@�@���ː�,
        �r������,
        �U����,
        ���l,
        MP�g�p��,
        �w��Ȃ�
    }

    public AdditionalCondition nextCondition = AdditionalCondition.�w��Ȃ�;

    [Tooltip("�Ⴂ�������������B����ق����łȂ������BFalse�łȂ��ق�")]
     public bool upDown;//���邢�͒Ⴂ�������������BFalse�łȂ��ق�





}
