using UnityEngine;
[System.Serializable]
public class RecoverCondition:SisterConditionBase
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
     //   �������Ă��Ȃ��x��������,//�S�Ă������Ă�͓̂��Ă͂܂�Ȃ��Ƃ�
        �w��Ȃ�

    }

    public RecoverStatus condition = RecoverStatus.�w��Ȃ�;
   // public EnemyStatus.KindofEnemy Type = EnemyStatus.KindofEnemy.Soldier;

    [Tooltip("�C�ӂ̎x�����؂�Ă�Ƃ������ŏƍ��Ɏg��")]
    public SisMagic.SupportType needSupport = SisMagic.SupportType.�Ȃ�;

    public int percentage;
    [Tooltip("true�ŏ�AFalse�ŉ�")]
    public bool highOrLow;//���̍��ڂ��ȏォ�ȉ����B
    //  public byte WeakPointJudge;




    public AttackJudge.UseAction ActBase = AttackJudge.UseAction.�Ȃɂ����Ȃ�;
    [Tooltip("�x�����ʂŉ񕜖��@����������ꍇ�Ɏg��")]
    public SisMagic.SupportType useSupport = SisMagic.SupportType.�Ȃ�;

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

    public AdditionalJudge nextCondition = AdditionalJudge.�w��Ȃ�;

     public bool upDown;//���邢�͒Ⴂ������������




}
