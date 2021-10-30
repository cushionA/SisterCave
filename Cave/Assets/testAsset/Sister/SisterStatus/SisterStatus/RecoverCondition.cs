using UnityEngine;
[System.Serializable]
public struct RecoverCondition
{
    /// <summary>
    /// ����͖��@���g�p������Ĕ�����s�킸�Ɏg�����߂̃L���b�V��
    /// </summary>
    [HideInInspector] public SisMagic UseMagic;

    [HideInInspector]
    public enum RecoverStatus
    {
        �v���C���[����Ԉُ�ɂ���������,//����͓łƂ��ʂɂ��Ă�����������
        �C�ӂ̎x�����؂�Ă���Ƃ�,
        �v���C���[��HP���K��l�̎�,
        �v���C���[��MP���K��l�ɒB������,
        ������MP���K��l�ɒB������,
        �G�^�C�v,
        ���G�����邩�ǂ���,
       // ��Ԉُ킪�؂�Ă���G������,
        �������Ă��Ȃ��x��������,//�S�Ă������Ă�͓̂��Ă͂܂�Ȃ��Ƃ�
        �w��Ȃ�

    }

    public RecoverStatus condition;
    public EnemyStatus.KindofEnemy Type;

    [Tooltip("�C�ӂ̎x�����؂�Ă�Ƃ������ŏƍ��Ɏg��")]
    public SisMagic.SupportType needSupport;

    public int percentage;
    [Tooltip("true�ŏ�AFalse�ŉ�")]
    public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;

    [HideInInspector]
    public enum MagicJudge
    {
        �������@,
        �U���X�e�[�g��,
        �x���X�e�[�g��,
        �Ȃɂ����Ȃ�

    }
    public MagicJudge ActBase;
    [Tooltip("�x�����ʂŉ񕜖��@����������ꍇ�Ɏg��")]
    public SisMagic.SupportType useSupport;

    public enum AdditionalJudge
    {
        �r������,//����
        �������ʎ���,//����
        ���W�F�l�񕜗�,//����
        ���W�F�l���񕜗�,//����
        ��Ԉُ��,//����
        �񕜗�,//����
        MP�g�p��,//����
        �w��Ȃ�//����
    }

    public AdditionalJudge nextCondition;

     public bool upDown;//���邢�͒Ⴂ������������




}
