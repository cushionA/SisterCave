using UnityEngine;
[System.Serializable]
public class RecoverCondition:SisterConditionBase
{



    /// <summary>
    /// �񕜖��@�̓Ǝ��s���I��
    /// �񕜖��@�ŉ����d������̂����ׂ�������
    /// ����Ɉ�ԓ��Ă͂܂���̂�����
    /// �񕜗ʂ��ł��������A�ł��񕜂��邩�Ȃ�
    /// �\�[�g�O�ɂ���ŏȂ�
    /// </summary>
    [Header("�񕜖��@���i�荞�ޓ�ڂ̏���")]
    [EnumFlags]
    public Magic.HealEffectType secondActJudge;




    /// <summary>
    /// ��������
    /// UI�ɂ����ɏ��Ȃ����ڂ����\��
    /// �o���A�A�T�e���C�g�A�h�䋭�����炢��������
    /// �I���\�ȍ��ڂ͏��Ȃ�
    /// </summary>
    [Tooltip("�x�����ʂŉ񕜖��@����������ꍇ�Ɏg��")]
    [EnumFlags]
    public Magic.SupportType healSupport = Magic.SupportType.�Ȃ�;



}
