using UnityEngine;
[System.Serializable]
public struct SupportCondition
{

    /// <summary>
    /// ����͖��@���g�p������Ĕ�����s�킸�Ɏg�����߂̃L���b�V��
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    [HideInInspector]
    public enum SupportStatus
    {
        �v���C���[����Ԉُ�ɂ���������,//����͓łƂ��ʂɂ��Ă�����������
�@�@�@�@�C�ӂ̎x�����؂�Ă���Ƃ�,
        �v���C���[�̗̑͂��K��l�̎�,
        �v���C���[��MP���K��l�ɒB������,
        ������MP���K��l�ɒB������,
        �G�^�C�v,
        ���G�����邩�ǂ���,
        //��Ԉُ킪�؂�Ă���G������,
        �������Ă��Ȃ��x��������,//�S�Ă������Ă�͓̂��Ă͂܂�Ȃ��Ƃ�
        �w��Ȃ�

    }

    public SupportStatus sCondition;
    public EnemyStatus.KindofEnemy Type;
    public SisMagic.SupportType needSupport;

     public int percentage;
    [Tooltip("true�ŏ�AFalse�ŉ�")]
    public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum MagicJudge
    {
        �e��x�����@,
        �U���X�e�[�g��,
        �񕜃X�e�[�g��,
        �Ȃɂ����Ȃ�

    }
    public MagicJudge ActBase;
    public SisMagic.SupportType useSupport;

    public enum AdditionalJudge
    {
        �r������,
        �������ʎ���,
        MP�g�p��,
        �w��Ȃ�
    }

    public AdditionalJudge nextCondition;

     public bool upDown;//���邢�͒Ⴂ������������




}
