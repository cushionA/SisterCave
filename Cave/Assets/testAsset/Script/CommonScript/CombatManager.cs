using Guirao.UltimateTextDamage;
using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static ControllAbillity;
using static EnemyStatus;

public class CombatManager : MonoBehaviour
{



#region ��`






/// <summary>
/// 
/// �퓬�֘A�̏����܂Ƃ߂ĊǗ�����N���X
/// �G��v���C���[���Ȃǐw�c�Ɉ�����̃N���X���p�������N���X��p�ӂ���
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
    /// ����𗘗p���č����l�Ɍ����Ă邩��₢���킹�\
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

        /// <summary>
        /// ���ǂ̓G���^�[�Q�b�g���Ă邩
        /// </summary>
        public int targetNum;

        /// <summary>
        /// �̔��ʗp��ID
        /// ���S���̊֌W�����ȂǂɎg��
        /// </summary>
        public int targetID;
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
/// ��p�������X�g
/// ��������C�x���g��΂�����
/// </summary>
public List<ControllAbillity> AllyList;




/// <summary>
/// ���w�c�̖�������]�G�̐�
/// </summary>
protected int count;


    /// <summary>
    /// �G���̃R���o�b�g�}�l�[�W���[
    /// �f�[�^������肷��
    /// </summary>
    protected CombatManager TargetManagerInstance;

    #endregion




    #region �^�[�Q�b�g���X�g�Ǘ�

    /// <summary>
    /// �^�[�Q�b�g���X�g���쐬����
    /// �Ȍケ�̃��X�g�̓��e���ύX�����܂Ŏg��
    /// NPC������������]���@�Ƃ�����
    /// ����g���@���܂���
    /// TargetAdd�ł���Ă��������������悤
    /// </summary>
    public void SetTargetList()
    {
        count = EnemyManager.instance.AllyList.Count;


        _targetList = new List<TargetData>(count);

        TargetListUpdate();
    }


    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// ���X�g���A�v�f����
    ///�G���̃}�l�[�W���[�ɓ��������āA�������Ally���X�g����
    ///�������̃^�[�Q�b�g���X�g�𖄂߂Ă��炤
    ///
    /// </summary>
    public void TargetListUpdate()
    {

        if (count > 0)
        {

            for (int i = 0; i < count; i++)
            {
                //�G�̗F�R���X�g��ʂ��Ďw������
                //�G�̃^�[�Q�b�g�f�[�^���X�V������
                TargetManagerInstance.AllyList[i].TargetDataUpdate(i);

            }

        }
    }


    //������̖������ɂ܂�����C�x���g�Ƃ�����̂��Ȃ��_���Ă܂���C�x���g
    //���Ɖ��l�Ƀw�C�g�������Ă邩�̃J�E���^�[�����܂��@�\�ɑg�ݍ��݂���
    //�����ăV�X�^�[����̃^�[�Q�e�B���O��
    //�܂��ʂɃv���C���[���͐l���ōU�������Ƃ����Ȃ�����J�E���g�̕��@�͂����ƒP���ł�������
    //������̂��Ȃ��_���Ă܂���C�x���g�͂���Ȃ���
    //�����̃^�[�Q�b�g�f�[�^�ɍ����l�ɑ_���Ă܂���J�E���g�����邩�炻�����𒲂ׂ�΂���


    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// �^�[�Q�b�g�Z�b�g����Ƃ��ɃG�l�~�[���Ăяo�����\�b�h
    /// �f�[�^�Ǘ��ɕK�v
    /// �N��_���Ă����A�N�ɑ_����ς������Ȃ�
    /// �^�[�Q�b�g��񍐂�����ID��Ԃ��Ă�����
    /// </summary>
    /// <param name="newTarget"></param>
    /// <param name="prevTarget"></param>
    /// <param name="_event"></param>
    /// <param name="level"></param>
    /// <param name="isFirst"></param>
    public int TargetSet(int newTarget, int prevTarget, TargetingEvent _event, int level, int id, bool isFirst)
    {
        //����̓^�[�Q�b�g�i���o�[����΃[���Ȃ̂�
        //�}�C�i�X�ɂȂ�Ȃ��悤��
        if (isFirst)
        {
            //���^�[�Q�b�g���猸�炷
            _targetList[prevTarget].targetingCount[level]--;
            _targetList[prevTarget].targetAllCount--;
        }

        //�G�����Z
        _targetList[newTarget].targetingCount[level]++;
        _targetList[prevTarget].targetAllCount++;



        //�w�߃C�x���g����Ȃ�C�x���g�J�n
        if (_event != TargetingEvent.�Ȃ�)
        {
            int count = AllyList.Count;
            for (int i = 0; i < count; i++)
            {
                AllyList[i].CommandEvent(_event, newTarget, level, id);
            }
        }

        //ID��Ԃ��Ă�����
        return _targetList[newTarget].targetID;
    }

    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// �G�I�u�W�F�N�g����i���o�[���l��
    /// ���Ԗڂ̔z��ɂ��邩
    /// �Ȃɂ����Ȃ����-1
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public int GetTargetNumberByObject(GameObject triggerEnemy)
    {

        int count = _targetList.Count;

        //�Ȃ���Ȃ�������-1
        int target = -1;

        for (int i = 0; i < count; i++)
        {
            
            //�^�[�Q�b�g������
            if (triggerEnemy == _targetList[i].targetObj)
            {
                target = i;
                break;
            }
        }

        return target;
    }


    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// �GID����i���o�[���l��
    /// ���Ԗڂ̔z��ɂ��邩�̃i���o�[
    /// �������Ȃ����-1��Ԃ�
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>

    public int GetTargetNumberByID(int targetID)
    {

        int count = _targetList.Count;

        //�Ȃ���Ȃ�������Ƃ肠�����v���C���[�_�����Ƃ����Ӗ������߂�0
        int target = -1;

        for (int i = 0; i < count; i++)
        {

            //�^�[�Q�b�g������
            if (targetID == _targetList[i].targetID)
            {
                target = i;
                break;
            }
        }

        return target;
    }


    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// �G�v�f�̍폜�ɉ����ēG�̐퓬�}�l�[�W���[����Ăяo�����
    /// �G���������̒��Ԏ��ɂ܂�����Ƌ����Ă����`
    /// �����瑤�̃^�[�Q�b�g���X�g�̕��ёւ����s��
    /// �L�����ɂ�����𔽉f
    /// </summary>
    /// <param name="deleteEnemy">�������G�̔ԍ�</param>
    public void TargetListSort(int deleteEnemy)
    {

        //���S�����G��ID���n��
        //����ɂ���Č��݃^�[�Q�e�B���O���Ă�G��
        int deadID = _targetList[deleteEnemy].targetID;

        _targetList.RemoveAt(deleteEnemy);

        int aCount = AllyList.Count;
        for (int i = 0; i < aCount; i++)
        {
            AllyList[i].TargetListChange(deleteEnemy, deadID);
        }

    }



    #endregion



    #region �����Ǘ��p���\�b�h


    /// <summary>
    /// �o�g���ɎQ������G���}�l�[�W���[�̊Ǘ����ɓ���
    /// 
    /// ����isAggressive�ł͂Ȃ��Z�O�����g����Ǘ����邩
    /// ��퓬��Ԃ̓G���V�X�^�[���񂽂��ɂƂ��Ă͍U���Ώ�
    /// ���ꂩ����
    /// ���邢��isRender�ŊǗ��J�n���āH�@��ʊO�Ŕ�A�N�e�B�u�ɂȂ����������H
    /// 
    /// ���ʉ��\
    /// </summary>
    /// <param name="_inst"></param>
    public void JoinBattle(ControllAbillity _inst)
    {


        //ID�𔭍s
        int newID = UnityEngine.Random.Range(1, 300);
        bool isUnique = false;
        int count = AllyList.Count;
        while (!isUnique)
        {



            for (int i = 0; i < count; i++)
            {
                if (AllyList[i].ReturnID() == newID)
                {
                    isUnique = false;
                    newID = UnityEngine.Random.Range(1, 300);
                    break;
                }
                //�Ō�܂ōs������^
                else if (i == count - 1)
                {
                    isUnique = true;
                }
            }
        }


        //ID���s�����璇��
        AllyList.Add(_inst);

        //�^�[�Q�b�g���X�g���X�V
        //Smanager�Ɏ����̏���n��
        AllyList[count].TargetDataAdd(newID);

    }


    /// <summary>
    /// �}�l�[�W���[�̊Ǘ�������o��
    /// ���ꂢ��Ȃ�����
    /// ���Ƃ��퓬��ԉ�������Ă�����̕W�I�ł��邱�Ƃ͂����Ȃ����
    /// ���ȊO�ł̓}�l�[�W���[���甲����Ȃ��H
    /// ���邢�̓Z�O�����g������܂�
    /// ���Ƃ͐��]�Ŕ�����������邩
    /// �Q�Ԃ�ꍇ�̃t���O�������ɓ����H�ʂɗp�ӂ��邩�A���\�b�h
    /// </summary>
    /// <param name="_inst"></param>
    public virtual void EndBattle(ControllAbillity _inst)
    {

    }




    /// <summary>
    /// �I�[�o�[���C�h���Ȃ�
    /// 
    /// ���񂾃G�l�~�[���Ăяo�����߂̃��\�b�h
    /// �U���I�G�l�~�[�̃��X�g�r�����玀�S��Ԃ̕ۑ��܂ōs��
    /// </summary>
    public void Die(int ID, EnemyAIBase inst)
    {
        int num = AllyList.IndexOf(inst);

        //�U���I�I�u�W�F�N�g�̃��X�g����j��
        //�s�ӑł��ꌂ���S�̏ꍇ�͏���������H
        AllyList.RemoveAt(num);

        //��������G�̃^�[�Q�b�g���X�g���ёւ�
        TargetManagerInstance.TargetListSort(num);


        //�������疡�������񂾂Ƃ��C�x���g��΂��H
        //�E��������Ƀw�C�g�������Ƃ�
        //������������瓦����Ƃ�

    }




    /// <summary>
    /// �I�[�o�[���C�h����
    /// 
    /// �����^�[�Q�b�g��G�����Ă�
    /// �����x���A���邢�͏ヌ�x���̖��������l���邩������
    /// </summary>
    /// <returns></returns>
    public virtual int TargettingCount(int level, int target)
    {
        //���̉񐔂����J��Ԃ�
        int count = 4 - level;

        //absolute�͎ז�����Ȃ�
        if (count == 1)
        {
            return 0;
        }

        int sum = 0;

        for (int i = level; i <= count; i++)
        {
            sum += _targetList[target].targetingCount[i];
        }

        return sum;
    }




    /// <summary>
    /// �U����~��������𔻒f����
    /// �I�[�o�[���C�h����
    /// </summary>
    /// <returns></returns>
    public virtual bool AttackStopCheck(int target, int level, float cDistance, int needCount, int id)
    {
        return false;
    }


    #endregion



}
