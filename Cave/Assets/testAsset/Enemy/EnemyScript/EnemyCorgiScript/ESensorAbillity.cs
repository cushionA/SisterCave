using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using SensorToolkit;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// �G�̃Z���T�[
    /// ���E�Z���T�[�����̓R���C�_�[�̃I�u�W�F�N�g���甭�M
    /// �^��_�ANPC��V�X�^�[����ɑ΂���w�C�g�Ǘ������邩
    /// ���ƍU�����̓T�[�`����H�@�G���������ꂽ������ł悭��
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/ESensorAbility")]
    public class ESensorAbillity : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�G����̃Z���T�["; }
        [SerializeField]
        PolygonCollider2D _sight;

        //--------------------------------------------------------------------------
        //�t�B�[���h�T�[�`�ɕK�v�ȃp�����[�^�[
        [SerializeField]
        RangeSensor2D se;
        //  string dangerTag = "Danger";
        [SerializeField] EnemyAIBase brain;
        /// <summary>
        /// ���͂̃I�u�W�F�N�g�����Ńp���X���΂��͈�
        /// </summary>
        [SerializeField] float fieldRange = 70;
        //����͋���
        [SerializeField] float pulseWait = 3;
        float pulseTime;



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
            if (se.SensorRange != 0)
            {
                Serch();
            }

        }

        /// <summary>
        /// AggresiveSerch�n��
        /// </summary>
        #region
        private void Serch()
        {
            if (!SManager.instance.isSerch)
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("�@�\���Ă܂���[");
                    se.Pulse();
                    SerchEnemy();

                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
                    pulseTime = 0;
                }
            }
        }

/// <summary>
/// ������������V�X�^�[����Ƃ�NPC�Ƃ��ɑ΂��鏈���𑫂�����
/// </summary>
        public void SerchEnemy()
        {
            //�^�[�Q�b�g���X�g���܂������

            //�������߂����ɓG����ёւ���

            if (se.DetectedObjects.Count != 0)
            {
                brain.StartCombat();
            }

            //�T�[�`�^�C�v�A�߂���D��Ƃ��F�X���ă^�[�Q�b�g�؂�ւ���̂��肩���ł�m

//            for (int i = 0; i < se.DetectedObjectsOrderedByDistance.Count; i++)
       //     {
  //              SManager.instance.TargetAdd(se.DetectedObjectsOrderedByDistance[i]);

    ///        }

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

                if (CheckFoundObject(collision.gameObject))
                {
                brain.StartCombat();
                    RangeChange();
                    //�����Ƀ|�W�V�������f�ł���悤��
                    //Debug.Log("�@�\���Ă܂���[");
                    se.Pulse();
                    SerchEnemy();
                    //isSerch�����Ə����S�}�l�[�W���[���G���X�g�̖ʓ|�����Ă����
                    pulseTime = 0;

                    //������AgrSerch�ɔC����B�������Ă����B�ŏ��Ɍ��m�����̂͋߂�������ǂ��������X�V�����
                }
        }




        #endregion

        public void RangeChange()
        {
            if (brain.isAggressive)
            {
                se.SensorRange = aggresiveRange;

                if(_sight != null)
               _sight.enabled = false;
            }
            else
            {
                se.SensorRange = fieldRange;
                if (_sight != null)
                _sight.enabled = true;
            }

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
