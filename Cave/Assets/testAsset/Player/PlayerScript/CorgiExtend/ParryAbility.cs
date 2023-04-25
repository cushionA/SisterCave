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
    /// _parry_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/ParryAbility")]
    public class ParryAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�p���B����悤�ɂȂ�"; }

        [SerializeField]
        GuardAbillity _guard;
        float defenceTime;
        // PlayerMove pm;
        bool noGuard;//�K�[�h���������ĂȂ��B���˃p���֎~

        // Animation parameters
        protected const string _parryParameterName = "ParryState";
        protected int _parryAnimationParameter;

        int parryNumber = 0;

        /// <summary>
        /// �O�Ƀu���b�L���O���Ă邩
        /// </summary>
        bool blocking;


        [SerializeField]
        int _avoidNumber = 15;

        int _initialLayer;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            _initialLayer =  transform.root.gameObject.layer;

            }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            ParryAct();
        }



        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        protected virtual void ParryAct()
        {
            // if the ability is not permitted
            if (!AbilityPermitted)
            {
                // we do nothing and exit
                return;
            }

            if (isPlayer)
            {
               // Debug.Log($"��{_health._parryNow}");
                PlayerParryJudge();
            }
            else
            {
                if ((_movement.CurrentState != CharacterStates.MovementStates.Guard && _movement.CurrentState != CharacterStates.MovementStates.GuardMove))
                {
                    ParryJudgeEnd();
                }
            }

            if (parryNumber != 0)
            {
                //���[�V�����̏I��������
                if (CheckEnd())
                {
                    ParryEnd();
                }
                
          //      Debug.Log("������");
            }
        }



        public void PlayerParryJudge()
        {

           // Debug.Log($"�ˁ[{_movement.CurrentState}");
            //�K�[�h�ɂ܂��q�b�g���ĂȂ�
            if (!_guard.guardHit)
            {
                //�K�[�h���ŁA�Ȃ����p���B���ĂȂ�


                if ((_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove) && !noGuard
                     && _condition.CurrentState == CharacterStates.CharacterConditions.Normal)
                {
                    //�K�[�h���Ԍv��
                         defenceTime += _controller.DeltaTime;

                    if (!GManager.instance.equipWeapon.twinHand)
                        {
                        Debug.Log("���ʑ�");
                            if ((defenceTime >= GManager.instance.equipShield.parryStart || GManager.instance.blocking)  && !_health._parryNow)
                            {
                          
                                GManager.instance.PlaySound("ParryStart", GManager.instance.Player.transform.position);
                                ParryJudgeStart();
                                defenceTime = GManager.instance.equipShield.parryStart;
                                blocking = false;
                            }
                            //�p���B�p�����Ԃ��I��������
                            else if (defenceTime - GManager.instance.equipShield.parryStart > GManager.instance.equipShield.parryTime)
                            {
                                defenceTime = 0.0f;
                                ParryJudgeEnd();
                            noGuard = true;

                            }
                        }
                        else
                        {

                            if ((defenceTime >= GManager.instance.equipWeapon.parryStart || GManager.instance.blocking) && !_health._parryNow)
                            {
                                GManager.instance.PlaySound("ParryStart", GManager.instance.Player.transform.position);
                                ParryJudgeStart();
                                defenceTime = GManager.instance.equipWeapon.parryStart;
                                GManager.instance.blocking = false;

                            }

                            else if (defenceTime - GManager.instance.equipWeapon.parryStart > GManager.instance.equipWeapon.parryTime)
                            {
                            ; Debug.Log($"dgg{defenceTime}");
                            defenceTime = 0.0f;
                                ParryJudgeEnd();
                            noGuard = true;
                        }
                        }
                }
               
                else if (_movement.CurrentState != CharacterStates.MovementStates.Guard && _movement.CurrentState != CharacterStates.MovementStates.GuardMove)
                {
                 //   Debug.Log($"��������");
                    if (!GManager.instance.blocking)
                    {
                        defenceTime = 0;
                    }
                    else
                    {
                        defenceTime += _controller.DeltaTime;
                        //�u���b�L���O����͎O�b�c��
                        if (defenceTime >= 3)
                        {
                            GManager.instance.blocking = false;
                            defenceTime = 0;
                        }
                    }

                    ParryJudgeEnd();
                    noGuard = false;
                }
            } 
            //�K�[�h���q�b�g���Ă鎞�A�����p���B�~�X�������Ă��Ƃ�:noGuard
            else if (_guard.guardHit && !_health._parryNow)
            {
                noGuard = true;
                defenceTime += _controller.DeltaTime;
                GManager.instance.anotherMove = true;

            }

        }


        public void ParryJudgeStart()
        {
            _health._parryNow = true;
            
        }
        public void ParryJudgeEnd()
        {
            _health._parryNow = false;
        }


        public void ParryStart(int num = 2)
        {
         //   Debug.Log($"������7{_movement.CurrentState}");
            _movement.ChangeState(CharacterStates.MovementStates.Parry);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
        //    
            //���C���[��������C���[��
            //�X�^�~�i�񕜂Ƃ����������邩
            parryNumber = num;
            if(num == 1 && isPlayer)
            {
                blocking = true;
                GManager.instance.PlaySound(MyCode.SoundManager.instance.blockingSound, transform.position);
            }
            else
            {
                blocking = false;
                GManager.instance.PlaySound(MyCode.SoundManager.instance.parrySound, transform.position);
            }
            ParryJudgeEnd();
            ParryAvoid();
        }

        void ParryEnd()
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            _health._parryNow = false;
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }
            ParryAvoid();
            parryNumber = 0;
        }


        bool CheckEnd()
        {

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


        void ParryAvoid()
        {
            if (_initialLayer == transform.root.gameObject.layer)
            {
                transform.root.gameObject.layer = _avoidNumber;
            }
            else
            {
               transform.root.gameObject.layer = _initialLayer;
            }
        }


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_parryParameterName, AnimatorControllerParameterType.Int, out _parryAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {
            //���̃X�e�[�g��Attack�ł��邩�ǂ�����Bool����ւ��Ă�

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _parryAnimationParameter, parryNumber, _character._animatorParameters);
        
        }
    }
}
