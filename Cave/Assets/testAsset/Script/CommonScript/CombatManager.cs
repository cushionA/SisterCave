using Guirao.UltimateTextDamage;
using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static ControllAbillity;
using static EnemyStatus;

public abstract class CombatManager : MonoBehaviour
{



#region ��`






/// <summary>
/// �G���p�b�N
/// �G�z�������Ă�����������l�����銴��
/// ���̃f�[�^���G�z��̉��ԖڂȂ̂��ƌ����̂��L�^���Ă������Ƃŏ��ԓ���ւ���Ă�����
/// 
/// �ʒu�A��ށAHP�����AMP�����A�o�t���Ă邩�A�f�o�t���A�U���́A�h���
/// ���̂̓G�Ƀ^�Q����Ă邩�A���x�����ƂɊǗ�
/// ���Ƃ͖{�̂ւ̎Q�Ƃ��ǂ����邩��
/// �Q�[���I�u�W�F�N�g�̃��X�g���o�����B������int�Ń��X�g�Ɍ��т���H
/// �ł��V�X�^�[���񂪕��בւ����Ⴄ���
/// �ǂ����悤
/// �Z���T�[�Ŏ擾�����Q�[���I�u�W�F�N�g��Dictionary�Ƀc�b�R��ł��̕������G�����邩�H
/// �V�X�^�[����̏ꍇ��
/// 
/// �������Enemy�����炷��ƃv���C���[���������
/// 
/// 
/// �G�͂قڒǂ�Ȃ��悤�ɂ���H
/// ���߂��͈͂����A�Z�O�����g����
/// �Z�O�����g�͂łȂ��B����Ȓǂ������Ă���Q�[������������
/// �Z�O�����g���痣�ꂽ��q�I�u�W�F�N�g����؂藣�����
/// �V�X�^�[����Ɩ�����SManager�ɃA�N�Z�X�����BNPC�}�l�[�W���[�ɕς���
/// </summary>
public class TargetData
{


    /// <summary>
    /// �G�̎��
    /// �R�m�Ƃ��ˎ�Ƃ�����������
    /// �݂����ȕs�σf�[�^
    /// </summary>
    public CharacterData _baseData;

    /// <summary>
    /// �̗͂̊����Ȃǉσf�[�^
    /// </summary>
    public ConditionData _condition;



    /// <summary>
    /// ���x�����Ƃɉ��l�Ƀ^�[�Q�b�g����Ă��邩������
    /// </summary>
    public int[] targetingCount = new int[4];

    /// <summary>
    /// ���v���l�Ƀ^�[�Q�b�g�ɂ���Ă邩
    /// </summary>
    public int targetAllCount;

    /// <summary>
    /// ������g���čŏ��U�����Ă����̂Ƃ��A�����Ă����̂��N�Ȃ̂�������
    /// </summary>
    public GameObject targetObj;

        /// <summary>
        /// �U����Ԃł��邩
        /// </summary>
        public bool isAgressive;
}

#endregion







#region�@�f�[�^�֘A

/// <summary>
/// �W�I���̃f�[�^
/// �f�[�^���A�����Ă邩��z��ɓ���Ă�
/// �A�N�Z�X�͂₵
/// �V�X�^�[���񂽂����g����悤�ɂ��邩
/// �ʐM��̃N���X����l������
/// </summary>
public List<TargetData> _targetList;


/// <summary>
/// ����������p�������X�g
/// ��������C�x���g��΂�����
/// </summary>
public List<EnemyAIBase> AllyList;




/// <summary>
/// ���w�c�̖�������]�G�̐�
/// </summary>
protected int count;


    #endregion




    #region �^�[�Q�b�g���X�g�Ǘ�

    /// <summary>
    /// �^�[�Q�b�g���X�g���쐬����
    /// �Ȍケ�̃��X�g�̓��e���ύX�����܂Ŏg��
    /// NPC������������]���@�Ƃ�����
    /// �����łǂ�����ď��̏��Ԃ�ۏ؂��邩
    /// �w�C�g�Ƃ����S���o���o���ɂȂ�
    /// �G�����񂾂肵���̂�ʒm���ĎQ�ƕς��邩
    /// </summary>
    public abstract void SetTargetList();


    /// <summary>
    /// ���X�g���A�v�f����
    /// ��{�I�Ƀ^�[�Q�b�g���X�g�̓��e�͕ς��Ȃ�
    /// �Ȃ�Ńw�C�g�̔z��Ɋւ��Ă͏���ɃG�l�~�[���ł���Ă���
    /// </summary>
    public abstract void TargetListUpdate();


    /// <summary>
    /// �^�[�Q�b�g�Z�b�g����Ƃ��ɃG�l�~�[���Ăяo�����\�b�h
    /// �ǂ���^�[�Q�b�g�ɂ���̂���������ɓ`����
    /// �f�[�^�Ǘ��ɕK�v
    /// </summary>
    /// <param name="newTarget"></param>
    /// <param name="prevTarget"></param>
    /// <param name="_event"></param>
    /// <param name="level"></param>
    /// <param name="isFirst"></param>
    public abstract void TargetSet(int newTarget, int prevTarget, TargetingEvent _event, int level, int id, bool isFirst);


    public abstract int GetTargetNumber(GameObject enemy);



    #endregion



    #region �����Ǘ��p���\�b�h



    /// <summary>
    /// �o�g���ɎQ������G���}�l�[�W���[��
    /// �Ǘ����ɓ���
    /// </summary>
    /// <param name="_inst"></param>
    public abstract int JoinBattle(EnemyAIBase _inst);

    /// <summary>
    /// �}�l�[�W���[�̊Ǘ�������o��
    /// </summary>
    /// <param name="_inst"></param>
    public abstract void EndBattle(EnemyAIBase _inst);

    /// <summary>
    /// ���񂾃G�l�~�[���Ăяo�����߂̃��\�b�h
    /// �U���I�G�l�~�[�̃��X�g�r�����玀�S��Ԃ̕ۑ��܂ōs��
    /// </summary>
    public abstract void Die(int ID, EnemyAIBase inst);

    /// <summary>
    /// �����^�[�Q�b�g��G�����Ă�
    /// �����x���A���邢�͏ヌ�x���̖��������l���邩
    /// </summary>
    /// <returns></returns>
    public abstract int TargettingCount(int level, int target);


    /// <summary>
    /// �U����~��������𔻒f����
    /// </summary>
    /// <returns></returns>
    public abstract bool AttackStopCheck(int target, int level, float cDistance, int needCount, int id);

    public abstract void TargetListSort(int deleteEnemy);

    #endregion


    #region �G�w�c�̃}�l�[�W���[�Ƃ̘A�g


    /// <summary>
    /// �����瑤��AllyList����G�ɑ��M����
    /// �G�̃^�[�Q�b�g���X�g�ɏ�������
    /// �ł����ꂾ�Ƃ��܂̃^�[�Q�b�g���X�g�̏��Ԃ��ۏ؂���Ȃ����
    /// ���ɓG���̃G�l�~�[�����񂾎��Ƃ�
    /// </summary>
   // public abstract void SendData();


    #endregion

}
