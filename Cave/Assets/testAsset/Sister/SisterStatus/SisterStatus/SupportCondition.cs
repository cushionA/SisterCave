using MoreMountains.CorgiEngine;
using UnityEngine;
[System.Serializable]
public class SupportCondition:SisterConditionBase
{




    /// <summary>
    /// �ǂ�Ȏ�ނ̃T�|�[�g���g�p���邩
    /// ����̓\�[�g����ɔ��f
    /// �܂ނ��܂܂Ȃ����ǂ���Ŕ��f���邩�t���O�݂����Ȃ̂͂���Ȃ�
    /// </summary>
    [Header("�g�p����T�|�[�g�̃^�C�v")]
    [EnumFlags]
    public Magic.SupportType secondActCondition = Magic.SupportType.�Ȃ�;




    /// <summary>
    /// �g�����@�̑���
    ///�@�G���`����ݒu�A�����Ŏg��
    /// </summary>
    public AtEffectCon.Element useElement = AtEffectCon.Element.�w��Ȃ�;





}
