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
    /// �V�X�^�[����̎��Ȉړ����[�v
    /// isWarp�Ń��[�v�J�n���ăA�j���[�V�����C�x���g�őJ�ځA�I��
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/WarpAbility")]
    public class WarpAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "warp_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        protected const string _warpParameterName = "_nowWarp";
        protected int _warpAnimationParameter;

        //----------------------------------------------------------------------------

        [SerializeField]
        BrainAbility _sister;

        //----------------------------------------------------------------------------

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
            WarpEnd();
        }





        /// <summary>
        /// �퓬���ɓ����郏�[�v
        /// </summary>
        public void WarpStart()
        {
            if (_sister.mp >= 5)
            {
                _sister.mp -= 5;
            }
            Debug.Log("����");
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

        }

        //�A�j���[�V�����C�x���g
        public void WarpEnd()
        {
            if (_movement.CurrentState != CharacterStates.MovementStates.Warp)
            {
                return;
            }

            if (CheckEnd("Warp"))
            {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

        }
        bool CheckEnd(string Name)
        {

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                return false;
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"�A�j���I��");

            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }
        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_warpParameterName, AnimatorControllerParameterType.Bool, out _warpAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�N���E�`���O�ɋC�������
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _warpAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Warp), _character._animatorParameters);
        }
    }
}
