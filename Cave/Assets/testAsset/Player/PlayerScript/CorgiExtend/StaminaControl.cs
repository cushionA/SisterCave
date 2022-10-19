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

        //�X�^�~�i�p���g�p�����ǂ���
        bool isUsing;

        /// <summary>
        /// �X�^�~�i�g�p�֘A�Ŏ��Ԃ��v��
        /// </summary>
       float stTime;

        CharacterStates.MovementStates lastState = CharacterStates.MovementStates.Nostate;

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
     //       StaminaController();
            StaminaJudge();
        }



        /// <summary>
        /// �X�^�~�i�Ǘ�������
        /// </summary>
        protected virtual void StaminaController()
        {
            if (NoUseJudge())
            {            Debug.Log($"�J���s�X{_movement.CurrentState}");
                //�X�^�~�i���g�p���ĂȂ���Ԃ�
                GManager.instance.isStUse = false;

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
            return (!GManager.instance.isStUse && _condition.CurrentState == CharacterStates.CharacterConditions.Normal);
        }

        /// <summary>
        /// �X�^�~�i�g�p�̏ڍׂ��l�߂�
        /// </summary>
        protected virtual void StaminaJudge()
        {

            if(lastState != _movement.CurrentState)
            {
                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;
                    isUsing = true;

                }
                else if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.jumpSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;

                }
                else if (_movement.CurrentState == CharacterStates.MovementStates.Rolling)
                {

                    GManager.instance.StaminaUse((int)(GManager.instance.rollSt * GManager.instance.stRatio));
                    GManager.instance.isStUse = true;

                }
                else if (!isUsing)
                {
                    GManager.instance.isStUse = false;
                }
                lastState = _movement.CurrentState;
            }


            if (isUsing)
            {
                stTime += _controller.DeltaTime;

                if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                {
                    //�X�^�~�i���g�p���A���邢��0.1�b���Ƃ�
                    if (stTime >= 0.1)
                    {
                        GManager.instance.StaminaUse((int)(GManager.instance.dashSt * GManager.instance.stRatio));
                        GManager.instance.isStUse = true;
                        stTime = 0;
                    }
                }
                else
                {
                    isUsing = false;
                    stTime = 0;
                    GManager.instance.isStUse = false;
                }
            }

 /*

            if(_movement.CurrentState == CharacterStates.MovementStates.Running)
            {
                stTime += _controller.DeltaTime;

                //�X�^�~�i���g�p���A���邢��0.1�b���Ƃ�
                if(!isUsing || stTime >= 0.1)
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
           else if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
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
