using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using UnityEngine.UI;

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
    /// �A�C�R���������邱����
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
        protected const string _combiParameterName = "CombinationState";
        protected int _combiAnimationParameter;

        //protected RewiredCorgiEngineInputManager _inputManager;

        float combinationCool;
        bool combiEnable = true;
        /// <summary>
        /// �A�g�Ώېݒ�Ŏg�����߂̃_�~�[
        /// �U���I���͂���Ȃ��̂ŃR���r�l�[�V�����p�̏�����ǉ����悤
        /// �񕜂Ɉڍs�Ƃ��ɃR���r�l�[�V������ǉ����ăR���r�l�[�V�����p�ɍ��ւ���B
        /// </summary>
        [SerializeField] FireCondition dammy;

        [SerializeField]
        Text cCounter;

        [SerializeField]
        Transform cIcon;

        [SerializeField]
        Slider cSlider;


        /// <summary>
        /// �R���r�l�[�V�����̒i�K
        /// 0�̎��͉������Ȃ��A1�őҋ@���[�V�����J�n�A2�Ŕ������[�V����
        /// </summary>
        enum CombinationState
        {
            Idle = 0,
            Act = 1,
            End = 2
        }

        CombinationState _combState;



        /// <summary>
        /// �A�g�������i�ڂ�
        /// </summary>
        int conboChain = 0;

        float castTime;
        /// <summary>
        /// �`�F�C���̏I�����ϑ�
        /// </summary>
        float chainEndTime;

        BrainAbility sb;
        FireAbility fire;

        /// <summary>
        /// �^�Ȃ�K�v�ȓ��͂������炳�ꂽ�Ƃ�������
        /// �܂�{�^���������ꂽ���Ă���
        /// </summary>
        bool needInput;

        /// <summary>
        /// �r���𒆎~���ăR���r�l�[�V�������Ă邩
        /// </summary>
        bool castStopping;

        [SerializeField]
        RewiredCorgiEngineInputManager _input;

        /// <summary>
        /// �R���r�l�[�V�����O�̃^�[�Q�b�g
        /// </summary>
        GameObject prevTarget;

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;

            sb = _character.FindAbility<BrainAbility>();
            fire = _character.FindAbility<FireAbility>();
            _inputManager = _input;
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {


            base.ProcessAbility();
            CombiCool();
            CombinationController();
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
                needInput =  (_inputManager.CombinationButton.State.CurrentState == MMInput.ButtonStates.Off);
                
            }
            //�������R���r�l�[�V�����ŃX�g�b�v����Ă�Ȃ���͋֎~��
            if (_movement.CurrentState == CharacterStates.MovementStates.Combination || _movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                return;
            }

            if (sb.status.equipCombination != null)
            {
             //  
                if (_inputManager.CombinationButton.State.CurrentState == MMInput.ButtonStates.ButtonDown
                    && _controller.State.IsGrounded)
                {
                    //       Debug.Log("kl");
                    chainEndTime = 0;
                    //�����̈Ӑ}�Ƃ��Ă̓R���r�l�[�V�������ݒ肳��ĂĒn�ʂɂ��ăN�[���^�C��������Ȃ��Ė��@����������Ȃ�
                    //�R���{�\�t���O�̓R���{���͕s�ɂȂ��Ă�B�`�F�C�������čŌ�ɉ��߂Ė߂����B
                    if (combiEnable && conboChain == 0) //&& !(SManager.instance.actNow && !SManager.instance.castNow))
                    {
                        Debug.Log($"���������{conboChain}");
                        //�R���{���ɃR���{��t���Ԑ؂��҂��Ԍv���̃^�C�}�[���ē��͂��邽�тɃ��Z�b�g
                        combinationCool = 0;
                        //SManager.instance.castNow = false;
                        combiEnable = false;
                        Combination();
                        //combiEnable = false;
                        cCounter.text = ((int)sb.status.equipCombination.coolTime).ToString();
                        if (cIcon.gameObject.activeSelf)
                        {
                            cIcon.gameObject.SetActive(false);
                        }
                        cSlider.value = 0;
                    }
                    //�R���r�l�[�V�����̃N�[���^�C�����������ĂȂ���MP�g��
                    else if (!combiEnable && combinationCool >= 1f && sb.status.equipCombination.useMP <= sb.mp)
                    {
                        	Debug.Log($"���������{conboChain}");
                      //  combinationCool = 0;
                      //  conboChain = 0;
                        //combiEnable = true;

                        //���񂾂�MP����
                        if (conboChain == 0)
                        {
                            sb.mp -= sb.status.equipCombination.useMP;
                        }
                        Combination();
                   //     cCounter.text = ((int)sb.status.equipCombination.coolTime).ToString();
                        if (cIcon.gameObject.activeSelf)
                        {
                            cIcon.gameObject.SetActive(false);
                        }
                        cSlider.value = 0;
                    }
                }

// Debug.Log($"kds{_inputManager.CombinationButton.State.CurrentState}");
                
            }
        }



        /// <summary>
        /// ���ꂪEnd�ɂ���ăg���K�[����郁�\�b�h�B��̓I�ȏ���
        /// End�ɂȂ����甭�˃��[�V�����Ɉڂ��Ă��̏I���̃A�j���C�x���g�ŌĂ΂��B
        /// chain�i���o�[�͂����Ŋ��p����B
        /// ���邢�̓A�j���[�V�����p�����[�^�[�ɂ��ă`�F�C���ŉr�����[�V�����ς���Ƃ����Ă���������
        /// </summary>
        public void CombinationDo()
        {
            
            
            //�܂��g�p����R���r�l�[�V�������ǂꂩ�m�F
           // Debug.Log($"sdsdddddddd{sb.status.equipCombination._sortNumber}");
            //���[�v
            if (sb.status.equipCombination._sortNumber == 1)
            { //Debug.Log($"sdsdsd{SManager.instance.target == null}");
                Vector2 warpPosi =  SManager.instance.target.transform.position;
              

                float exY = sb.RayGroundCheck(warpPosi);

                //�n�\����\�܈ȉ��̋����Ȃ�ڒn
                warpPosi.y = warpPosi.y - exY <= 15 ? exY : warpPosi.y;
                        
                if (SManager.instance.target.transform.localScale.x > 0)
                {
                    warpPosi.Set(warpPosi.x - 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                    if (GManager.instance.Player.transform.localScale.x < 0)
                    {
                        GManager.instance.pc.PlayerFlip();
                    }
                }
                else
                {
                    warpPosi.Set(warpPosi.x + 10, warpPosi.y);

                    GManager.instance.Player.transform.position = warpPosi;
                  //  GManager.instance.pm.Flip();
                    if (GManager.instance.Player.transform.localScale.x > 0)
                    {
                        GManager.instance.pc.PlayerFlip();
                    }
                }
                GManager.instance.pc.PlayerStop();
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
            if (castStopping)
            {
                castStopping = false;
                _movement.ChangeState(CharacterStates.MovementStates.Cast);
            }
            else
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
            }
            if(prevTarget != null)
            {
                SManager.instance.target = prevTarget;
                prevTarget = null;
            }
        }

        public void CombinationStart()
        {
            _combState = CombinationState.Act;
            //����ŃR���r�l�[�V�����̉r�����[�V�������n�܂�B
            //End�ɂȂ�܂Ŏn�����[�V�����ɓ���Ȃ�
            //Stop�̓��[�V�����I���̏����̌��������
            if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
            {
                castStopping = true;
            }
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
            _movement.ChangeState(CharacterStates.MovementStates.Combination);
        }


        /// <summary>
        /// �R���r�l�[�V�����^�C�v�ɂ���Č��ʔ������R���g���[��
        /// </summary>
        void CombinationController()
        {
            //Act����End�𓱂�����
            //�r���Ƃ��҂��Ă�Ԃ̏��������Ă�������CombinationDo()�݂����ɏꍇ��������
            if (_combState == CombinationState.Act)
            {
                //End�ɂ��鏈���������������
                //�^�C�v�ŕ����āA�r�����Ԃ̎��Ԍo�߂⑦���Ɏn�����[�V�����ɓ�����́A���Ƃ̓{�^���𗣂����Ƃ��n�����[�V�����ɓ�������
                if (sb.status.equipCombination._combiType == SisterCombination.ActType.soon)
                {
               //     Debug.Log("ssssss");
                    _combState = CombinationState.End;
                }
                else if (sb.status.equipCombination._combiType == SisterCombination.ActType.cast)
                {
                    castTime += _controller.DeltaTime;
                    //���炵��������ނ�
                    //�r�����đ����ɊJ�n�n
                    if(castTime >= sb.status.equipCombination.castTime)
                    {
                        _combState = CombinationState.End;
                        castTime = 0;
                    }
                }
                else if (sb.status.equipCombination._combiType == SisterCombination.ActType.longPress)
                {
                    //�������łȂ��{�^����������ĂȂ���ԂȂ�ł��H
                    //�{�^�������āA�����ꂽ���n��
                    if (needInput)
                    {
                        _combState = CombinationState.End;
                    }
                }
                else if (sb.status.equipCombination._combiType == SisterCombination.ActType.castAndPress)
                {
                    //�r�����I��������{�^����������Ă��Ȃ���ԂȂ甭��
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

            if(_combState == CombinationState.End)
            {
             //   Debug.Log("gggg");
                //  if (CheckEnd())
                //      {
                CombinationDo();
            //    }
            }
        }



        public void CombiCool()
        {
            //�������ĂȂ��Ƃ�����
            if (!combiEnable)
            {
                //	Debug.Log($"��ؔ����");
                //�A�g�̃N�[���^�C���v��
                combinationCool += _controller.DeltaTime;

                cSlider.value = combinationCool / sb.status.equipCombination.coolTime;

                if (combinationCool >= sb.status.equipCombination.coolTime)
                {
                    combinationCool = 0;
                    conboChain = 0;
                    combiEnable = true;
                    cIcon.gameObject.SetActive(true);

                }
            }
            //�����ŃR���r�l�[�V�����\�Ƃ�
            if (conboChain > 0 && _movement.CurrentState != CharacterStates.MovementStates.Combination)
            {
                //
                //�`�F�C�����ł��s�����ĂȂ��Ƃ�

                chainEndTime += _controller.DeltaTime;

                if (chainEndTime >= 3.5f)
                {
                    //Debug.Log($"���َq�����{conboChain}");

                    conboChain = 0;
                    chainEndTime = 0;

                }
            }

            if (!combiEnable && conboChain == 0)
            {
                cCounter.text = (Mathf.Round(sb.status.equipCombination.coolTime - combinationCool)).ToString();
            }

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


        /// <summary>
        /// ���͂���Ă΂��
        /// �^�[�Q�b�g�̐ݒ�ƃ`�F�C���̊Ǘ�
        /// �����ď����̎n��
        /// </summary>
        void Combination()
        {

            if (sb.status.equipCombination.isTargeting)
            {
                //�^�[�Q�b�g���K�v�Ȃ�w��
                if (SManager.instance.target != null)
                {
                    prevTarget = SManager.instance.target;
                    SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
                    SManager.instance.target = null;
                    fire.TargetSelect(sb.status.equipCombination.mainTarget[conboChain], dammy);
                   
                }
                if (SManager.instance.target == null)
                {
                    fire.TargetSelect(sb.status.equipCombination.subTarget[conboChain], dammy);
                }

                if (SManager.instance.target == null)
                {
                    SManager.instance.target = transform.root.gameObject;
                }
            }
            Debug.Log($"��������������{ SManager.instance.target}");
            CombinationStart();
           // Debug.Log("��������");
            //	�����ƃ^�[�Q�b�g�����邩�A�^�[�Q�b�g��K�v�Ƃ��Ȃ��Ȃ�
            if (SManager.instance.target != null || !sb.status.equipCombination.isTargeting)
            {
                conboChain++;
               // Debug.Log($"���������{conboChain}");
                if (conboChain >= sb.status.equipCombination.chainNumber)
                {
                    conboChain = 0;
                    //combiEnable = false;
                }
                //_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
               // SManager.instance.target = null;
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
