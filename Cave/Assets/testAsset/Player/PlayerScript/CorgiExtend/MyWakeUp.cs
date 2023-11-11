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
        
        ///��΂ɒ�~���Ăق����Ȃ�
        float lastTime;

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



        float blowTime;
        [SerializeField]
        PlayerRoll pr;

        public enum StunnType
        {
            Falter = 1,
            Parried = 2,
            GuardBreake = 3,
            Down = 4,
            NDie = 6,//���ʂ̎�
            BlowDie = 7,//������ю�
            notStunned = 0

        }


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
            //�����ŋN���オ��A�j���[�V�����A�܂��͂��߂����[�V�������I���������A�����ďI�������Ȃ�X�^�������������
            DoSomething();
        }

        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {

            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����


            if (_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && isPlayer)
            {
                if (nowType == 5)
                {
                    //   _inputManager.AvoidButton.State.ChangeState(MMInput.ButtonStates.Off);
                    if (Mathf.Sign(_horizontalInput) != Mathf.Sign(transform.root.localScale.x) && _horizontalInput != 0)
                    {
                        _character.Flip();
                    }
                       Recover(true);
                    
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
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned))
            {
                // we do nothing and exit
                return;
            }

         //   transform.root.gameObject.layer = 15;
          //  _horizontalInput = 0;
            if (nowType == 1)
            {
                //�A�j���I�������X�^������
                //���ꂩ4�ȊO�S���܂Ƃ߂ăA�j���҂����ł�������
                //���̃^�C�v�����Ƃ���ŃA�j���̖��O�؂�ւ��Ă��悩��
                blowTime += _controller.DeltaTime;
                //0.1�b�ȏ�Œn�ʂɂ�����
                if (blowTime >= 0.15)
                {
                   _controller.SetHorizontalForce(0);
                    blowTime = 0;
                }

                    if (CheckEnd("Falter"))
                {
                    blowTime = 0;
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
            else if (nowType == 6)
            {

                if (CheckEnd("NDie"))
                {
                    Die();
                }
                else if (isPlayer)
                {
                    Die();
                }
            }
            else if (nowType == 4 || nowType == 7)
            {

                    blowTime += _controller.DeltaTime;
                //0.1�b�ȏ�Œn�ʂɂ�����
                if (blowTime >= 0.4 && _controller.State.IsGrounded)
                {
                    _health._blowNow = false;
                    blowTime = 0;
                    _controller.SetForce(Vector2.zero);
                    if (nowType == 4)
                    {
                        nowType = 5;
                        if (isPlayer)
                            transform.root.gameObject.layer = 7;
                    }
                    else
                    {

                        nowType = 8;

                    }
                }

            }
            else if (nowType == 5)
            {
                _controller.SetForce(Vector2.zero);
                if (CheckEnd("WakeUp"))
                    {
                        Recover();
                    }
                //���݃��[�V�����o����߂�
                else if (CheckEnd("Falter"))
                {
                    nowType = 4;
                }
            }
            else if (nowType == 8)
            {
                _controller.SetForce(Vector2.zero);
                if (CheckEnd("DDie"))
                {
                    Die();
                }
                //���݃��[�V�����o�����蒼��
                else if (CheckEnd("Falter"))
                {
                    nowType = 7;
                }
                else if (isPlayer)
                {
                    Die();
                }
            }


        }

        /// <summary>
        /// 1�����߂�2���p���B3���K�[�h�u���C�N4���������
        /// </summary>
        /// <param name="type"></param>
        public void StartStunn(StunnType type)
        {

                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Stunned);
                _characterHorizontalMovement.SetHorizontalMove(0);
                
                _characterHorizontalMovement.ReadInput = false;
                if (type == StunnType.Falter)
                {
                    nowType = 1;
                   // _controller.SetForce(Vector2.zero);
                }
                else if (type == StunnType.Parried)
                {
                    nowType = 2;
                    _controller.SetForce(Vector2.zero);
                }
                else if (type == StunnType.GuardBreake)
                {
                    nowType = 3;
                    _controller.SetForce(Vector2.zero);
                }
                else if(type == StunnType.Down)
                {
                    _health._blowNow = true;
                    //���ݐ�����΂���Ă܂�
                    nowType = 4;
                       if(isPlayer)
                    transform.root.gameObject.layer = 15;
                }
                else if (type == StunnType.NDie)
                {

                _health._blowNow = true;
                _controller.SetForce(Vector2.zero);
                    nowType = 6;
                }
                else if (type == StunnType.BlowDie)
                {
                    //���ݐ�����΂���Ă܂�
                    nowType = 7;
                    transform.root.gameObject.layer = 15;
                }
            
        }




        public void Die()
        {
            _health.Die();
            if (isPlayer)
            {

                GManager.instance.HPReset();
                transform.root.gameObject.layer = 7;
                Recover();
            }
            else
            {
                Destroy(_character.gameObject);
                
            }

        }



        public int GetStunState()
        {
            return nowType;
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
            
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // �����ɓ��B�����normalizedTime��"Default"�̌o�ߎ��Ԃ��E���Ă��܂��̂ŁAResult�ɑJ�ڊ�������܂ł�return����B
                return false;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // �ҋ@���Ԃ���肽���Ȃ�΁A�����̒l��傫������B
                if (lastTime > 0 && lastTime == _animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
                {
                    lastTime = 0;
                    return true;
                }
                else 
                {
                    lastTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    return false;
                }
            }
            //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"�A�j���I��");
            lastTime = 0;
            return true;

            // return !(sAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }

        void Recover(bool cancel = false)
        {

            nowType = 0;
            
            //�A�[�}�[���Z�b�g���Ă�
            _characterHorizontalMovement.ReadInput = true;
            _health.ArmorReset();
            if (!cancel)
            { _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
             //  _characterHorizontalMovement.ReadInput = true;
            }
            else
            {

                _movement.ChangeState(CharacterStates.MovementStates.Nostate);
                _character.banIdle = true;
                _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

                pr.ForceRoll();

                //Debug.Log($"tgr{pr == null}");

            }
            
        }

    }

}
