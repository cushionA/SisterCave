using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Micosmo.SensorToolkit;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// �V�X�^�[����̃Z���T�[
    /// ���E�Z���T�[�����̓R���C�_�[�̃I�u�W�F�N�g���甭�M
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/SensorAbility")]
    public class SensorAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�V�X�^�[����̃Z���T�["; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 

        
        [SerializeField]
        /// <summary>
        /// ���E�̃R���C�_�[
        /// </summary>
        GameObject _sight;

        LOSSensor2D _sightSensor;

        RangeSensor2D range;

        //--------------------------------------------------------------------------
        //�t�B�[���h�T�[�`�ɕK�v�ȃp�����[�^�[
        [SerializeField]
        LOSSensor2D se;
        //  string dangerTag = "Danger";
        [SerializeField] BrainAbility sister;
        /// <summary>
        /// ���͂̃I�u�W�F�N�g�����Ńp���X���΂��͈�
        /// </summary>
        [SerializeField] float fieldRange = 70;
        //����͋���
        [SerializeField] float pulseWait = 3;
        float pulseTime;
        bool isSerch;


        //-----------------------------------------------------------------------
        //�G�l�~�[�T�[�`�B���E�Z���T�[�ɕK�v�ȃp�����[�^�[

        //�댯���̓g���b�v�G���A�̓�����Ƃ��ɂ����H
        //�댯�t���O���ĂĂ��̊Ԃ͓����Ȃ��Ƃ��V�����ҋ@�X�e�[�g����đҋ@������Ƃ�

        //  public float SerchRadius;
        [SerializeField]
        private LayerMask layerMask;

        //-----------------------------------------------------------------------
        //�퓬���̃Z���T�[�A�A�O���b�V�u�T�[�`�̃p�����[�^�[
        /// <summary>
        /// �퓬���p���X���΂��͈�
        /// </summary>
        [SerializeField] float aggresiveRange = 150;

        private readonly UniTaskCompletionSource
            uniTaskCompletionSource = new UniTaskCompletionSource();

        public UniTask SerchAsync => uniTaskCompletionSource.Task;
        



        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _sightSensor = _sight.MMGetComponentNoAlloc<LOSSensor2D>();
            range = (RangeSensor2D)se.InputSensor;
            RangeChange();
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            DoSomething();

        }



        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted)
            {
                // we do nothing and exit
                return;
            }

            if(sister.nowState != BrainAbility.SisterState.�킢)
            {
                FieldSerch();
            }
        }

        /// <summary>
        /// �퓬���̃Z���T�[
        /// FireAbillity����Ăяo���Ďg��
        /// </summary>
        #region
        public async void AggresiveSerch()
        {

                //Debug.Log("�@�\���Ă܂���[");
                SensorPulse();

                
            SManager.instance.TargetAdd(se.GetDetectionsByDistance(SManager.instance.enemyTag));
            await SManager.instance._addAsync;
         //   Debug.Log("���ł�");
            //�����I���ʒm
            uniTaskCompletionSource.TrySetResult();
                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
        }
        public void SerchEnemy()
        {
            //�������߂����ɓG����ёւ���
            SensorPulse();
            SManager.instance.InitialAdd(se.GetDetectionsByDistance(SManager.instance.enemyTag));
        }
        #endregion

        /// <summary>
        /// FieldSerch�n��
        /// </summary>
        #region
        void FieldSerch()
        {

            if (!isSerch) 
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("�@�\���Ă܂���[");
                    SensorPulse();
                    isSerch = true;
                    pulseTime = 0;
                }
            }
            else if (isSerch)
            {
                DetectObject();
                isSerch = false;
            }
        }

        public void DetectObject()
        {
            //�G�^�O������Ȃ�
            if (se.GetDetections(SManager.instance.enemyTag).Count >= 1)
            {
                Debug.Log("La");
                RangeChange();
                SerchEnemy();
                sister.StateChange(BrainAbility.SisterState.�킢);
                
                SManager.instance.GetClosestEnemyX();


                sister.reJudgeTime = 150;
            }
            //�댯�^�O������Ȃ�x��
            else if (se.GetDetections(SManager.instance.dangerTag).Count >= 1)
            {
                sister.nowState = BrainAbility.SisterState.�x��;
                //���̕ӂ͈ړ������̏�����
                //�r�t�H�[�i���o�[0�ōŏ����Ă��ƁB�X�e�[�g�i���o�[3�Œ�~����n�܂�

                sister.reJudgeTime = 0;
                sister.changeable = true;
                SManager.instance.playObject = null;
                sister.isPlay = false;
            }
            //�V�у^�O�������ėV��łȂ��Ȃ�
            else if (se.GetDetections(SManager.instance.reactionTag).Count >= 1 && !sister.isPlay)
            {
                SManager.instance.playObject = se.GetNearestDetectionToPoint(SManager.instance.Sister.transform.position);
                sister.isPlay = true;
                sister.playPosition = SManager.instance.playObject.transform.position.x;
                sister.playDirection = SManager.instance.playObject.transform.localScale.x;
            }
        }

        #endregion




        /// <summary>
        /// ������Z���T�[�̃R���C�_�[����Ăяo����
        /// </summary>
        /// <param name="collision"></param>
        public void FindObject()
        {
            GameObject obj = _sightSensor.GetNearestDetection();

            GameObject pick = null;
            for (int i= 0; i < _sightSensor.Detections.Count;i++)
            {
                pick  = _sightSensor.GetDetections()[i];

                //�댯�����������炻�����D��
                if (pick.tag == SManager.instance.dangerTag)
                {
                    obj = pick;
                    continue;
                }
                //�G�̃I�u�W�F�N�g���������炻�����ŗD��
                if (pick.tag == SManager.instance.enemyTag)
                {
                    obj = pick;
                    break;
                }
            }

            if (obj.tag == SManager.instance.enemyTag)
            {
                if (sister.nowState != BrainAbility.SisterState.�킢)
                {
                    sister.StateChange(BrainAbility.SisterState.�킢);
                    RangeChange();
                    //�����Ƀ|�W�V�������f�ł���悤��

                    SerchEnemy();
                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
                    pulseTime = 0;
                    SManager.instance.GetClosestEnemyX();

                    //������AgrSerch�ɔC����B�������Ă����B�ŏ��Ɍ��m�����̂͋߂�������ǂ��������X�V�����
                }
            }
            else if (obj.tag == SManager.instance.dangerTag)
            {

                if (sister.nowState == BrainAbility.SisterState.�̂�т�)
                {
                    sister.nowState = BrainAbility.SisterState.�x��;


                    sister.changeable = true;
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                }
            }
            else if (obj.tag == SManager.instance.reactionTag)
            {
                if (sister.nowState == BrainAbility.SisterState.�̂�т�)
                {
                    SManager.instance.playObject = se.GetNearestDetectionToPoint(SManager.instance.Sister.transform.position);
                    sister.isPlay = true;
                    sister.playPosition = SManager.instance.playObject.transform.position.x;
                    sister.playDirection = SManager.instance.playObject.transform.localScale.x;
                }
            }

        }

        public void RangeChange()
        {
            
            if(sister.nowState == BrainAbility.SisterState.�킢)
            {
               
                range.Circle.Radius = aggresiveRange;
                if (_sight != null)
                    _sight.SetActive(false);
            }
            else
            {
                range.Circle.Radius = fieldRange;
                if (_sight != null)
                    _sight.SetActive(true);
            }

        }


        void SensorPulse()
        {
            range.Pulse();
            se.Pulse();
        }


    }
}

