using UnityEngine;
[System.Serializable]
public class SupportCondition
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
       // �������Ă��Ȃ��x��������,//�S�Ă������Ă�͓̂��Ă͂܂�Ȃ��Ƃ�
        �w��Ȃ�

    }

    public SupportStatus sCondition = SupportStatus.�w��Ȃ�;
   // public EnemyStatus.KindofEnemy Type = EnemyStatus.KindofEnemy.Soldier;
    public SisMagic.SupportType needSupport = SisMagic.SupportType.�Ȃ�;

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
    public MagicJudge ActBase= MagicJudge.�Ȃɂ����Ȃ�;
    public SisMagic.SupportType useSupport = SisMagic.SupportType.�Ȃ�;

    public enum AdditionalJudge
    {
        �r������,
        �������ʎ���,
        MP�g�p��,
        �w��Ȃ�
    }

    public AdditionalJudge nextCondition = AdditionalJudge.�w��Ȃ�;

     public bool upDown;//���邢�͒Ⴂ������������




}
