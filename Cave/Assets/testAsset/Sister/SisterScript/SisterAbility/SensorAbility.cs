using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using SensorToolkit;

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
        PolygonCollider2D _sight;

        //--------------------------------------------------------------------------
        //�t�B�[���h�T�[�`�ɕK�v�ȃp�����[�^�[

        RangeSensor2D se;
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
        //-----------------------------------------------------------------------       
        //���ʃp�����[�^�[

        int nowState;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            sister = GetComponent<BrainAbility>();
            se = GetComponent<RangeSensor2D>();
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

            if(sister.nowState == BrainAbility.SisterState.�킢)
            {
                AggresiveSerch();
            }
            else
            {
                FieldSerch();
            }
        }

        /// <summary>
        /// AggresiveSerch�n��
        /// </summary>
        #region
        private void AggresiveSerch()
        {

            if (!SManager.instance.isSerch)
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("�@�\���Ă܂���[");
                    se.Pulse();
                    SerchEnemy();
                    SManager.instance.isSerch = true;
                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
                    pulseTime = 0;
                }
            }
        }
        public void SerchEnemy()
        {
            //�^�[�Q�b�g���X�g���܂������
            SManager.instance.targetList.Clear();
            SManager.instance.targetCondition.Clear();
            //�������߂����ɓG����ёւ���
            for (int i = 0; i < se.DetectedObjectsOrderedByDistance.Count; i++)
            {
                SManager.instance.TargetAdd(se.DetectedObjectsOrderedByDistance[i]);
                  
            }
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
                    se.Pulse();
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
            if (se.GetDetectedByTag(SManager.instance.enemyTag).Count >= 1)
            { 
                sister.nowState = BrainAbility.SisterState.�킢;//���̕ӂ͂܂���Őݒ�ł���悤�ɂ��悤
                RangeChange();
                se.Pulse();
                SManager.instance.playObject = null;
                sister.isPlay = false;
                SerchEnemy();
                SManager.instance.GetClosestEnemyX();


                sister.reJudgeTime = 150;
            }
            //�댯�^�O������Ȃ�x��
            else if (se.GetDetectedByTag(SManager.instance.dangerTag).Count >= 1)
            {
                sister.nowState = BrainAbility.SisterState.�x��;
                //���̕ӂ͈ړ������̏�����
                //�r�t�H�[�i���o�[0�ōŏ����Ă��ƁB�X�e�[�g�i���o�[3�Œ�~����n�܂�
                sister.stateNumber = 3;
                sister.beforeNumber = 0;
                sister.reJudgeTime = 0;
                sister.changeable = true;
                SManager.instance.playObject = null;
                sister.isPlay = false;
            }
            //�V�у^�O�������ėV��łȂ��Ȃ�
            else if (se.GetDetectedByTag(SManager.instance.reactionTag).Count >= 1 && !sister.isPlay)
            {
                SManager.instance.playObject = se.GetNearestToPoint(SManager.instance.Sister.transform.position);
                sister.isPlay = true;
                sister.playPosition = SManager.instance.playObject.transform.position.x;
                sister.playDirection = SManager.instance.playObject.transform.localScale.x;
            }
        }

        #endregion

        /// <summary>
        /// ���E�Z���T�[����
        /// ���E�ł�isPlay�g���K�[���Ȃ��ۂ�
        /// ���Ƃ��ꎋ�E�I�u�W�F�N�g�̃R���C�_�[����g���K�[���Ȃ��ƃ_���ۂ�
        /// </summary>
        #region
        /// <summary>
        /// ���C���΂��ĕǉz���ł͂Ȃ������ׂ�
        /// </summary>
        /// <param name="i_target">���m��������</param>
        /// <returns></returns>
        private bool CheckFoundObject(GameObject i_target)
        {
            Vector2 targetPosition = i_target.transform.position;//target�̈ʒu���擾
            Vector2 myPosition = transform.position;//�����̈ʒu




            Vector2 toTargetDir = (targetPosition - myPosition).normalized;

            if (!IsHitRay(myPosition, toTargetDir, i_target))
            {//IsHitsRay���^�Ȃ�^��Ԃ�
                return false;
            }

            return true;


        }

        private bool IsHitRay(Vector2 i_fromPosition, Vector2 i_toTargetDir, GameObject i_target)
        {
            // �����x�N�g���������ꍇ�́A���ʒu�ɂ�����̂��Ɣ��f����B
            if (i_toTargetDir.sqrMagnitude <= Mathf.Epsilon)
            {//sqr�̓x�N�g���̒�����Ԃ�
                return true;

            }

            RaycastHit2D onHitRay = Physics2D.Raycast(i_fromPosition, i_toTargetDir,/* SerchRadius,*/ layerMask.value);
            if (!onHitRay.collider)
            {
                return false;
            }
            //  ////Debug.log($"{onHitRay.transform.gameObject}");
            ////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);
            if (onHitRay.transform.gameObject != i_target)
            {//onHitRay�͓��������ꏊ
             //���������ꏊ��Player�̈ʒu�łȂ����
             //////Debug.log("������");
                return false;
            }

            return true;
        }


        /// <summary>
        /// ������Z���T�[�̃R���C�_�[����Ăяo����
        /// </summary>
        /// <param name="collision"></param>
        public void SightSensor(Collider2D collision)
        {
            if (collision.tag == SManager.instance.enemyTag)
            {
                if (CheckFoundObject(collision.gameObject) && !SManager.instance.isEscape && sister.nowState != BrainAbility.SisterState.�킢)
                {
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                    RangeChange();
                    //�����Ƀ|�W�V�������f�ł���悤��
                    sister.reJudgeTime = 150;
                    //Debug.Log("�@�\���Ă܂���[");
                    se.Pulse();
                    SerchEnemy();
                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
                    pulseTime = 0;
                    sister.nowState = BrainAbility.SisterState.�킢;//���̕ӂ͂܂���Őݒ�ł���悤�ɂ��悤
                    SManager.instance.GetClosestEnemyX();

                    //������AgrSerch�ɔC����B�������Ă����B�ŏ��Ɍ��m�����̂͋߂�������ǂ��������X�V�����
                }
            }
            else if (collision.tag == SManager.instance.dangerTag)
            {

                if (CheckFoundObject(collision.gameObject) && sister.nowState != BrainAbility.SisterState.�x��)
                {
                    sister.nowState = BrainAbility.SisterState.�x��;

                    sister.stateNumber = 3;
                    sister.beforeNumber = 0;
                    sister.reJudgeTime = 0;
                    sister.changeable = true;
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                }
            }
        }




        #endregion

        public void RangeChange()
        {
            if(sister.nowState == BrainAbility.SisterState.�킢)
            {
                se.SensorRange = aggresiveRange;
                _sight.enabled = false;
            }
            else
            {
                se.SensorRange = fieldRange;
                _sight.enabled = true;
            }

        }
        /// <summary>
        /// �ĒT�m
        /// </summary>
        public void ReSerch()
        {
            SManager.instance.targetList = null;
            SManager.instance.targetCondition = null;
            SManager.instance.target = null;
            isSerch = false;
            pulseTime = 100;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            SightSensor(collision.collider);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            SightSensor(collision.collider);
        }





    }
}

