using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Micosmo.SensorToolkit;
using System.Threading;
using System;
using System.Linq;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// �K�v�ȋ@�\
    /// 
    /// �E���͂̂��̂����o
    /// �E�퓬���Ȃǂ̏�ԕω��Ŕ͈͂⊴�m����^�O���C���[���ς��
    /// �E���͎̂擾���Ȃ��B���͎擾����B�G�������炢�����ǂ��������m�点��
    /// �E�p���X�̓}�j���A���Ŕ���
    /// �E�V�X�^�[����͊����ɂ���������
    /// �E�ː����ʂ��Ă邩�̊m�F���ł���悤��
    /// �E���Ƃł���΃��[�v����ꏊ�̏�Q���̃`�F�b�N�Ƃ���
    /// 
    /// �͈͕ύX���\�b�h�A�p���X�N�����\�b�h
    /// ��퓬���̌p���Z���T�[�A�퓬���̖ړI�Z���T�[�i���ӓG���A�ː����f�Ȃǁj
    /// �Z���T�[�̋@�\���ׂȂ���
    /// 
    /// �d�l
    /// �E�͈̓Z���T�[�ƃg���K�[�Z���T�[�Ō��m�����G�ɂ���Ɏ����Z���T�[�Ō��m���Ă���
    /// �E���̗��҂𓝍�����̂�Boolean�Z���T�[�ŁA����͓��͂��󂯂�݂̂Ńp���X�͂��Ȃ�
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/SensorAbility")]
    public class SensorAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�V�X�^�[����̃Z���T�["; }


        
        [SerializeField]
        ///<summary>
        ///�����`�F�b�N�Ɏg���Z���T�[
        /// �퓬���̓I�t��
        /// </summary>
        LOSSensor2D rangeSight;

        [SerializeField]
        ///<summary>
        ///�����`�F�b�N�Ɏg���Z���T�[
        /// ����̓g���K�[�p
        /// </summary>
        LOSSensor2D triggerSight;

        /// <summary>
        /// �͈̓Z���T�[
        /// ����̓p���X���Ȃ�
        /// 
        /// �͈͂�ς��邽�߂����Ɏ���
        /// </summary>
        [SerializeField]
        RangeSensor2D range;


        /// <summary>
        /// �����p�̃Z���T�[
        /// 
        /// >�u�[�� �Z���T�[���p���X����K�v�͂���܂���B
        /// >���̓Z���T�[�̃C�x���g���T�u�X�N���C�u���A���͐M�����ύX�����Ƃ����ɍX�V���܂��B
        /// </summary>
        [SerializeField]
        BooleanSensor sensor;


        //--------------------------------------------------------------------------



        /// <summary>
        /// ���͂̃I�u�W�F�N�g�����Ńp���X���΂��͈�
        /// </summary>
        [SerializeField] float fieldRange = 70;


        //����͋���
        [SerializeField] float pulseWait = 3;



        //-----------------------------------------------------------------------

        //�퓬���̃Z���T�[�A�A�O���b�V�u�T�[�`�̃p�����[�^�[
        /// <summary>
        /// �퓬���p���X���΂��͈�
        /// </summary>
        [SerializeField] float aggresiveRange = 150;


        /// <summary>
        /// �퓬���Ɏg���G�̐�
        /// </summary>
        int enemyCount;


        /// <summary>
        /// �g���K�[�i����j�Z���T�[���g����
        /// </summary>
        bool useTrigger;

        /// <summary>
        /// �͈̓Z���T�[���g����
        /// </summary>
        bool useRange;


        /// <summary>
        /// �G���J�E���g�̃v���p�e�B
        /// </summary>
        public int EnemyCount {  get => enemyCount; private set => enemyCount = value; }


        /// <summary>
        /// �L�����Z���g�[�N��
        /// </summary>
        CancellationTokenSource sensorCancel;

        //----------------------------------------------------------------------- �Z���T�[�ݒ�


        /// <summary>
        /// �V�X�^�[����Ȃ���T�[�`���
        /// �퓬���̃J�E���g���Ȃ���K�v�ɂȂ�
        /// </summary>
        [SerializeField]
        bool isSister;


        /// <summary>
        /// �G���������狳���Ă�������
        /// </summary>
        [SerializeField]
        NPCControllerAbillity charaController;

        //-----------------------------------------------------------------------




        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {

            //�e�Z���T�[���g�����m�F
            useRange = rangeSight != null;
            useTrigger = triggerSight != null;

            if (isSister)
            {
                SisterSensor().Forget();
            }
            else
            {
                NormalSensor().Forget() ;
            }
        }






        #region �Z���T�[�Q

        /// <summary>
        /// �퓬���̃Z���T�[
        /// ��b�Ɉ����͂̓G���𑪂�
        /// �퓬���[�h�˓����ɕK�v�Ȃ�Ă�
        /// </summary>
        async UniTaskVoid AggresiveSerch()
        {
            //��b���Ƃ�
            await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken: sensorCancel.Token);

            UseSensor();

            //���𐔂��ċ����Ă�����
            enemyCount = sensor.GetDetections(SManager.instance.enemyTag).Count;

            //�J��Ԃ�
            AggresiveSerch().Forget();
        }



        /// <summary>
        /// �ʏ펞�Ɏg���Z���T�[
        /// �G�l�~�[���g���G��T�������̃Z���T�[
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid NormalSensor()
        {
            //�w�莞�ԑҋ@����
            await UniTask.Delay(TimeSpan.FromSeconds(pulseWait), cancellationToken: sensorCancel.Token);

            //�p���X����
            UseSensor();

            //�Z���T�[�łȂɂ��������Ȃ��
            if (sensor.GetDetections().Any())
            {
                charaController.FindEnemy();

            }
            //�����Ȃ��Ȃ�J��Ԃ�
            else
            {
                NormalSensor().Forget();
            }

        }


        async UniTaskVoid SisterSensor()
        {
            //�w�莞�ԑҋ@����
            await UniTask.Delay(TimeSpan.FromSeconds(pulseWait), cancellationToken: sensorCancel.Token);

            //�p���X����
            UseSensor();

            bool battleStart = false;

            //�����������炵�킯����
            if (sensor.GetDetections().Any())
            {
                battleStart = SortingDetectObjects();
            }

            //�퓬�J�n�Ȃ�
            if (battleStart)
            {
                enemyCount = 0;
                //�͈͕ύX
                range.Circle.Radius = aggresiveRange;
                AggresiveSerch().Forget();
            }
            else
            {
                //�ČĂяo��
                SisterSensor().Forget();
            }


        }




        #endregion


        #region �L�������g�����\�b�h

        /// <summary>
        /// �퓬�J�n���ɌĂяo�����
        /// �ʏ�Z���T�[���~���ĕK�v�Ȃ�퓬�Z���T�[���N������
        /// 
        /// �U���Ƃ��󂯂Đ퓬�J�n�������ɐF�X�~�߂�̂ɕK�v
        /// </summary>
        public void BattleStart()
        {
            sensorCancel.Cancel();
            sensorCancel = new CancellationTokenSource();

            //�퓬�Z���T�[���K�v�Ȃ̂͌���V�X�^�[���񂾂�
            if (isSister)
            {
                enemyCount = 0;

                //�͈͕ύX
                range.Circle.Radius = aggresiveRange;
                AggresiveSerch().Forget();
            }

        }


        /// <summary>
        /// �퓬�I���A�ʏ�Z���T�[���Ăяo��
        /// </summary>
        public void BattleEnd()
        {


            //�퓬�Z���T�[������̂ł��������L�����Z��������
            if (isSister)
            {
                sensorCancel.Cancel();
                sensorCancel = new CancellationTokenSource();
                range.Circle.Radius = fieldRange;

                SisterSensor().Forget();
            }
            else
            {
                NormalSensor().Forget();
            }
        }





        #endregion


        /// <summary>
        /// FieldSerch�n��
        /// </summary>
        #region


        /// <summary>
        /// ���������̂��d��������
        /// �V�X�^�[�����p
        /// </summary>
        /// <param name="collision"></param>
        bool SortingDetectObjects()
        {

            bool findEnemy = false; 

            //�G�^�O������Ȃ�
            if (sensor.GetDetections(SManager.instance.enemyTag).Any())
            {
                findEnemy = true;
                charaController.FindEnemy();
            }
            //�댯�^�O������Ȃ�x��
            else if (sensor.GetDetections(SManager.instance.dangerTag).Any())
            {
                charaController.ReportObject(true,sensor.GetNearestDetection(SManager.instance.dangerTag));
            }
            //�����^�O����������
            else if (sensor.GetDetections(SManager.instance.reactionTag).Any())
            {
                charaController.ReportObject(false, sensor.GetNearestDetection(SManager.instance.reactionTag));
            }
            //�����Ȃ��Ȃ牽���Ȃ���Ԃ�
            charaController.ReportObject(false, null);
            return findEnemy;
        }

        #endregion








        /// <summary>
        /// �Z���T�[����
        /// </summary>
        void UseSensor()
        {
            //�g���K�[�Z���T�[���g���Ȃ�
            if (useTrigger)
            {
                triggerSight.Pulse();
            }

            //�͈̓Z���T�[���g���Ȃ�
            if (useRange)
            {
                //���̓Z���T�[�܂ňꊇ��
                rangeSight.PulseAll();
            }

        }




    }
}

