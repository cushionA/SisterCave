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
    [AddComponentMenu("Corgi Engine/Character/Abilities/TODO_REPLACE_WITH_ABILITY_NAME")]
    public class EAttackCon : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        protected const string _todoParameterName = "AttackNow";
        protected int _todoAnimationParameter;
        protected const string _numParameterName = "AttackNumber";
        protected int _numAnimationParameter;



        int motionNum;


        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

        }


        //�ǂ����Ă΂�Ȃ��������
        /*
        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���čs���ς�����ł���˂�
            //InputReaction�t���O�Ƃ���������

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (ReIManager.PrimaryMovement.y < -ReIManager.Threshold.y)
            {
                
            }//DoSomething();
            
        }
        */
        /// <summary>
        /// AI���炱���Ăяo��
        /// </summary>
        protected virtual void AttackAct()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                // or if we're gripping
                || (_movement.CurrentState != CharacterStates.MovementStates.Attack))
            {
                // we do nothing and exit
                return;
            }
            //�I������}�W�łǂ����悤
            //���Ƃ����p���B���荷�����߂�?bool atCancel�Ƃ���
            //�P���ɃA�j���p�����[�^�[�ς���΂����������B����Ő؂�ւ���
            //�X�e�[�g�ŏW�������Attack�p�����[�^�[�^�Ȃ�ړ��A�����ł�float���l��
            //statemachinebehavior�ł��


        }

        public void AttackTrigger(int num = 0)
        {
            //�U��������Ȃ���Ⴀ
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                motionNum = num;
                _movement.ChangeState(CharacterStates.MovementStates.Attack);
            }
        }


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_todoParameterName, AnimatorControllerParameterType.Bool, out _todoAnimationParameter);
            RegisterAnimatorParameter(_numParameterName, AnimatorControllerParameterType.Bool, out _numAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {
            //num�p�����[�^�[�ŃA�j���Đ��̑Ώۂ��ς��B
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _todoAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorFloat(_animator, _numAnimationParameter, (motionNum), _character._animatorParameters);

            //�R���{�n�̘A���Đ���AI�̕��Œ�`
            //�U���̃N�[���^�C���c�Ƃ��������̂ւ�͂�������
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

    }
}

