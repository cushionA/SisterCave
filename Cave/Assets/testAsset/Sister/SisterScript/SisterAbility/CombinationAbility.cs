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
    /// �R���r�l�[�V�����̃N���X
    /// �R���r�l�[�V�����ɂ̓^�C�v������������B
    /// �W�I������^�C�v�A�{�^�����������Ă�Ԃ��낢��I�ׂ���
    /// �{�^�����������K��������͒������t���O���R���r�l�[�V�������ɂ���
    /// �����Fire���ɃR���r�l�[�V�����s�\�t���O������
    /// ������ł͎��s�ł��Ȃ��X�g�b�p�[�������Ă������{�^������������n������悤�ɂ���
    /// �R���r�l�[�V������U���𓀌�����Fire���̎d�|�����K�v
    /// �Ō��Stop�t���O����������Fire���ɏ�����߂�
    /// Combination�̃A�j���p�����[�^�[�̐����ɂ���ăA�j���[�V�������J�ڂ���
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/CombinationAbility")]
    public class CombinationAbility : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "�R���r�l�[�V�����Ɋւ���\��"; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        protected const string _combiParameterName = "CombinationNow";
        protected int _combiAnimationParameter;

        protected RewiredCorgiEngineInputManager ReInput;


        /// <summary>
        /// �R���r�l�[�V�����̒i�K
        /// 0�̎��͉������Ȃ��A1�Ń��[�V�����J�n�A2�Ŕ���
        /// </summary>
        enum CombinationState
        {
            Idle = 0,
            Act = 1,
            End = 2
        }

        CombinationState _combState;

        /// <summary>
        /// �`�F�C���񐔋L�^�B
        /// �`�F�C���̐��ŏ����ς����̂������
        /// </summary>
        int _chainNumber;

        float castTime;

        BrainAbility sb;
        FireAbility sf;

        /// <summary>
        /// �^�Ȃ�K�v�ȓ��͂������炳�ꂽ�Ƃ�������
        /// �܂�{�^���������ꂽ���Ă���
        /// </summary>
        bool needInput;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
            ReInput = (RewiredCorgiEngineInputManager)_inputManager;
            sb = GetComponent<BrainAbility>();
            sf = GetComponent<FireAbility>();
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
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {
            //�����ŉ��{�^����������Ă��邩�ɂ���Ĉ����n����

            // here as an example we check if we're pressing down
            // on our main stick/direction pad/keyboard
            if (_combState == CombinationState.Act)
            {
                //�{�^������肪����Ă���Ȃ�^��
                needInput =  (ReInput.CombinationButton.State.CurrentState == MMInput.ButtonStates.Off);
                
            }
        }

        /// <summary>
        /// ��������ł���ꍇ�́A�������̏����𖞂����Ă��邩�ǂ������`�F�b�N���āA�A�N�V���������s�ł��邩�ǂ������m�F���܂��B
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted
                // or if we're not in our normal stance
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                // or if we're grounded
                || (!_controller.State.IsGrounded)
                // or if we're gripping
                || (_movement.CurrentState == CharacterStates.MovementStates.Gripping))
            {
                // we do nothing and exit
                return;
            }

            CombinationController();
        }

        /// <summary>
        /// ���ꂪEnd�ɂ���ăg���K�[����郁�\�b�h
        /// End�ɂȂ����甭�˃��[�V�����Ɉڂ��Ă��̏I���̃A�j���C�x���g�ŌĂ΂��B
        /// chain�i���o�[�͂����Ŋ��p����B
        /// ���邢�̓A�j���[�V�����p�����[�^�[�ɂ��ă`�F�C���ŉr�����[�V�����ς���Ƃ����Ă���������
        /// </summary>
        public void CombinationDo()
        {
            //�܂��g�p����R���r�l�[�V�������ǂꂩ�m�F

            //���[�v
            if (sf.status.equipCombination._sortNumber == 1)
            {
                Vector2 warpPosi = SManager.instance.target.transform.position;


                float exY = sb.RayGroundCheck(warpPosi);

                //�n�\����\�܈ȉ��̋����Ȃ�ݒu
                warpPosi.y = warpPosi.y - exY <= 15 ? exY : warpPosi.y;

                if (SManager.instance.target.transform.localScale.x > 0)
                {
                    warpPosi.Set(warpPosi.x - 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                    if (GManager.instance.Player.transform.localScale.x < 0)
                    {
                        GManager.instance.pm.Flip();
                    }
                }
                else
                {
                    warpPosi.Set(warpPosi.x + 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                    GManager.instance.pm.Flip();
                    if (GManager.instance.Player.transform.localScale.x > 0)
                    {
                        GManager.instance.pm.Flip();
                    }
                }
                Transform gofire = GManager.instance.PlayerEffector.transform;
                //gofire.localScale *= 0.8f;
                gofire.localScale = GManager.instance.Player.transform.localScale;
                Addressables.InstantiateAsync("WarpCircle", gofire);//.Result;//�����ʒu��Player
                GManager.instance.PlaySound("Warp", gofire.position);
                //return 1;
            }
            // return 0;

            //�Ō�ɏ�����
            _combState = CombinationState.Idle;
            needInput = false;
            sf.isStop = false;
        }

        public void CombinationStart(int chainNumber)
        {
            _combState = CombinationState.Act;
            //����ŃR���r�l�[�V�����̉r�����[�V�������n�܂�B
            //End�ɂȂ�܂Ŏn�����[�V�����ɓ���Ȃ�
            _chainNumber = chainNumber;
            //Stop�̓��[�V�����I���̏����̌��������
            sf.isStop = true;
        }


        void CombinationController()
        {
            //Act����End�𓱂�����
            if (_combState == CombinationState.Act)
            {
                //End�ɂ��鏈���������������
                //�^�C�v�ŕ����āA�r�����Ԃ̎��Ԍo�߂⑦���Ɏn�����[�V�����ɓ�����́A���Ƃ̓{�^���𗣂����Ƃ��n�����[�V�����ɓ�������
                if (sf.status.equipCombination._combiType == SisterCombination.ActType.soon)
                {
                    _combState = CombinationState.End;
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.cast)
                {
                    castTime += _controller.DeltaTime;
                    //���炵��������ނ�
                    if(castTime >= sb.status.equipCombination.castTime)
                    {
                        _combState = CombinationState.End;
                        castTime = 0;
                    }
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.longPress)
                {
                    //�������łȂ��{�^����������ĂȂ���ԂȂ�ł��H
                    if (needInput)
                    {
                        _combState = CombinationState.End;
                    }
                }
                else if (sf.status.equipCombination._combiType == SisterCombination.ActType.castAndPress)
                {
                    //�r�����I��������{�^����������Ă��Ȃ���ԂȂ�
                    castTime += _controller.DeltaTime;
                    bool nowOk = false;
                    if (castTime >= sb.status.equipCombination.castTime)
                    {
                        nowOk = true;
                    }
                    if (nowOk && needInput)
                    {
                        _combState = CombinationState.End;
                        castTime = 0;
                    }
                }
            }
        }


        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_combiParameterName, AnimatorControllerParameterType.Int, out _combiAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�N���E�`���O�ɋC�������
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _combiAnimationParameter, (int)_combState, _character._animatorParameters);
        }
    }
}
