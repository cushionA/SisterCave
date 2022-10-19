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
    [AddComponentMenu("Corgi Engine/Character/Abilities/EAttackCon")]
    public class EAttackCon : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 



        protected const string _numParameterName = "AttackNumber";
        protected int _numAnimationParameter;
        [SerializeField]
        protected EnemyAIBase _base; 


        int motionNum;
        [HideInInspector]
        public bool nowAttack;




        public  void AttackEnd()
        {
         //
            motionNum = 0;
            _controller.DefaultParameters.Gravity = -_base.status.firstGravity;
            nowAttack = false;
            if(_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
               // Debug.Log("��������");
            }

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
        public void AttackTrigger(int num = 0)
        {
            //�U��������Ȃ���Ⴀ
 _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
                 motionNum = num;
                _movement.ChangeState(CharacterStates.MovementStates.Attack);
                //�󒆂Ȃ�d�͏���
                if (!_controller.State.IsGrounded)
                {
                    _controller.DefaultParameters.Gravity = 0f;
                }
            nowAttack = true;
           
            //  Debug.Log($"aaaaa{motionNum}");
        }


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {

            RegisterAnimatorParameter(_numParameterName, AnimatorControllerParameterType.Int, out _numAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {
            //num�p�����[�^�[�ŃA�j���Đ��̑Ώۂ��ς��B
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _numAnimationParameter, motionNum, _character._animatorParameters);
       //   Debug.Log($"��������{motionNum}");
            //�R���{�n�̘A���Đ���AI�̕��Œ�`
            //�U���̃N�[���^�C���c�Ƃ��������̂ւ�͂�������
        }


    }
}

