using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// �X�^�~�i�Ǘ����s���R�[�h
    /// ���ݑ����Ă��邩�Ȃǂ̏�Ԃ����ƂɃX�^�~�i������������
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/StaminaControl")]
    public class StaminaControl : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�X�^�~�i�Ǘ��Ɏg���R�[�h"; }

        //���łɃX�^�~�i���g�p�������ǂ���
        bool isUsed;

        /// <summary>
        /// �X�^�~�i�g�p�֘A�Ŏ��Ԃ��v��
        /// </summary>
       float stTime;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

        }

        /// <summary>
        /// 
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            StaminaController();
        }



        /// <summary>
        /// �X�^�~�i�Ǘ�������
        /// </summary>
        protected virtual void StaminaController()
        {
            if (NoUseJudge())
            {
                //�X�^�~�i���g�p���ĂȂ���Ԃ�
                GManager.instance.isStUse = false;
                isUsed = false;
                stTime = 0f;

            }

            else
            {
                //�X�^�~�i�g�p���ɂ���
                GManager.instance.isStUse = true;

            }

        }


        /// <summary>
        /// �X�^�~�i�𗘗p���ĂȂ���Ԃ��ǂ����𔻒f
        /// </summary>
        /// <returns></returns>
        protected virtual bool NoUseJudge()
        {
            //�K�[�h�Ɋւ��Ă̓K�[�h�����X�^�~�i�g�p���A�܂��̓_�E�����łȂ����
            return (_movement.CurrentState == CharacterStates.MovementStates.Crouching || _movement.CurrentState == CharacterStates.MovementStates.Falling
                   || _movement.CurrentState == CharacterStates.MovementStates.Idle || !(_movement.CurrentState == CharacterStates.MovementStates.Guard && GManager.instance.isStUse)
                   || _condition.CurrentState != CharacterStates.CharacterConditions.Stunned);
        }

        /// <summary>
        /// �X�^�~�i�g�p�̏ڍׂ��l�߂�
        /// </summary>
        protected virtual void StaminaJudge()
        {
            if(_movement.CurrentState == CharacterStates.MovementStates.Running)
            {
                stTime += _controller.DeltaTime;

                //�X�^�~�i���g�p���A���邢��0.1�b���Ƃ�
                if(!isUsed || stTime >= 0.1)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                    isUsed = true;
                    stTime = 0;
                }
                
            }
            else if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
            {
                if (!isUsed)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.jumpSt * GManager.instance.stRatio));
                    isUsed = true;
                }
            }
            else if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
            {
                if (!isUsed)
                {
                    GManager.instance.StaminaUse((int)(GManager.instance.rollSt * GManager.instance.stRatio));
                    isUsed = true;
                }
            }

            //����֘A��WeaponAbillity�ŏ�����Ă���
            //�����K�[�h������
       /*     else if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                if (!isUsed)
                {
                    //����U���Ă��X�^�~�i�������Ȃ��Ƃ��͑��������̂���
                    GManager.instance.StaminaUse((int)GManager.instance.useAtValue.useStamina);
                    isUsed = true;
                }
            }*/
            /*   else if (_movement.CurrentState == CharacterStates.MovementStates.)
               {
                   if (!isUsed)
                   {
                       GManager.instance.StaminaUse((int)(GManager.instance. * GManager.instance.stRatio));
                       isUsed = true;
                   }

               }*/

        }

    }
}
