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
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/MyWakeUp")]
    public class MyWakeUp : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�_�E������߂����������߂̃A�N�V�����B�v���C���[�͉���Ń_�E�����L�����Z���ł���"; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters


        //�_�E�������ǂ̏�Ԃ��A�N���オ�蒆�Ƃ�
        //������сA�|�ꂽ�A�N���オ�蒆�̎O�i�K
       // protected const string _downStateParameterName = "BlowNow";
      //  protected int _downStateAnimationParameter;


        //���߂�
        protected const string _stunTypeParameterName = "StunState";
        protected int _stunTypeAnimationParameter;


        /// <summary>
        /// 1���߂�2�p���B3�K�[�h�u���C�N�A4�_�E��
        /// </summary>
        int nowType;

        /// <summary>
        /// ���ݐ�����΂���Ă邩�ǂ���
        /// </summary>
        bool blowNow;

        float blowTime;

        PlayerRoll pr;

        public enum StunnType
        {
            Falter = 1,
            Parried = 2,
            GuardBreake = 3,
            Down = 4,
            notStunned = 0

        }


        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            pr = _controller.gameObject.GetComponent<PlayerRoll>();
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            //�����ŋN���オ��A�j���[�V�����A�܂��͂��߂����[�V�������I���������A�����ďI�������Ȃ�X�^�������������
            DoSomething();
        }

        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {

            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                if (nowType == 4 && !blowNow)
                {
                    Recover();
                    pr.actRoll = true;
                }
            }
        }




        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // �X�^�����ĂȂ��Ȃ�A��
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)

                // or if we're grounded
                || (!_controller.State.IsGrounded))
            {
                // we do nothing and exit
                return;
            }

            if(nowType == 1)
            {
                //�A�j���I�������X�^������
                //���ꂩ4�ȊO�S���܂Ƃ߂ăA�j���������ł�������
                //���̃^�C�v�����Ƃ���ŃA�j���̖��O�؂�ւ��Ă��悩��

                if (CheckEnd("Falter"))
                {
                    Recover();
                }

            }
            else if (nowType == 2)
            {
                if (CheckEnd("Parried"))
                {
                    Recover();
                }
            }
            else if(nowType == 3)
            {
                if (CheckEnd("GBreake"))
                {
                    Recover();
                }
            }
            else if (nowType == 4)
            {
                if (blowNow)
                {
                    blowTime += _controller.DeltaTime;
                    //0.1�b�ȏ�Œn�ʂɂ�����
                    if (blowTime >= 0.1 && _controller.State.IsGrounded)
                    {
                        blowNow = false;
                        blowTime = 0;
                    }
                }
                else
                {
                    //��������N���オ�菈���ł���
                    //�N���オ��A�j�����I�������X�^������
                    //���[�����O�ŃX�^���������\��
                    //Wakeup�p�����[�^�[�I���Ń_�E���i�����������j�A�j���A�����ċN���オ��ɔh������悤�ɂ���B

                    if (CheckEnd("Wakeup"))
                    {
                        Recover();
                    }

                }
            }


        }

        /// <summary>
        /// 1�����߂�2���p���B3���K�[�h�u���C�N4���������
        /// </summary>
        /// <param name="type"></param>
        public void StartStunn(StunnType type)
        {
            if (_character.CharacterHealth.CurrentHealth > 0 && _condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Stunned);
                _characterHorizontalMovement.MovementForbidden = true;
                if (type == StunnType.Falter)
                {
                    nowType = 1;
                }
                else if (type == StunnType.Parried)
                {
                    nowType = 2;
                }
                else if (type == StunnType.GuardBreake)
                {
                    nowType = 3;
                }
                else
                {
                    //���ݐ�����΂���Ă܂�
                    nowType = 4;
                    blowNow = true;
                }
            }
        }









        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {

           // RegisterAnimatorParameter(_downStateParameterName, AnimatorControllerParameterType.Bool, out _downStateAnimationParameter);
            RegisterAnimatorParameter(_stunTypeParameterName, AnimatorControllerParameterType.Int, out _stunTypeAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _stunTypeAnimationParameter, (nowType), _character._animatorParameters);
        //    if()

 //           MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _downStateAnimationParameter, (), _character._animatorParameters);

            //�_�E���̃A�j���[�^�[ 
        }

        bool CheckEnd(string Name)
        {
            
            if (!_character._animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
                return false;
            }
            if (_character._animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                return false;
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"�A�j���I��");

            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

        void Recover()
        {
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            nowType = 0;
            _characterHorizontalMovement.MovementForbidden = false;
        }

    }

}
