using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
using MoreMountains.Tools;
using System;
using System.Threading;
using UnityEditor.Tilemaps;
using UnityEngine;
using static Equip;
using static FunkyCode.Light2D;
using static Micosmo.SensorToolkit.NavMeshSensor;
using static UnityEditor.PlayerSettings;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// ��蒼��
    /// 
    /// ���͂���p�����[�^�Z�b�g�A�A�j���Đ��܂ł������Œl������肷���{�̏����ɂ܂Ƃ߂�
    /// 
    /// �K�v�ȋ@�\
    /// �E�㋭���ߍU���i�ŏ��Ƀ{�^���������炻�̃{�^�����������܂őҋ@�B�����ꂽ��b��������B�����Ă�Ԃ�Input��return���Ȃ��Ƒ����������Ⴄ�j
    /// �E��̏ꍇ���Ŗ��@�����i�r�����Ԃ܂ŃL���X�g��ԁA�L���X�g������������Ă�Δ������Ȃ��B���ꂩ���b�H�@moving������X�^�~�i�͉񕜂��Ȃ��j
    /// �E����ł��߃L�����Z���B�r�����L�����Z���B�r�����͈ړ��L�[�A���邢�͎��E�ύX�Ń��b�N�I���؂�ւ�
    /// �E�A�j���[�V�����p�����[�^��؂�ւ��Ċe�A�N�V�������g�p�B�L�����Z���\�|�C���g�̓A�j���[�V�����C�x���g�Œʒm
    /// �E�A�j���̏I����҂��ƂōU�������I��
    /// 
    /// </summary>
  //  [AddComponentMenu("Corgi Engine/Character/Abilities/WeaponAbillity")]
    public class WeaponAbillity : MyAbillityBase
    {

        #region ��`



        /// <summary>
        /// �U���̎�ނ�\��
        /// ����ɉ����ă`���[�W���Ă邩�ōU���̎�ނ�\��
        /// 1���������������ŁA2�����������`���[�W
        /// </summary>
        public enum ActType
        {
            noAttack = 0,//�Ȃɂ��Ȃ�
            sAttack = 1,
            bAttack = 4,
            aAttack = 7,//�󒆎�
            fAttack = 10,//�󒆋�
            arts = 13,
            magic = 16//����ɍU���ԍ��ŉr���̎�ނ����߂�B���@�̉r���^�C�v�Ȃǂ��猈�߂�
        }

        /// <summary>
        /// ���݂̍s���̃f�[�^
        /// </summary>
        public struct NowActionData
        {

            /// <summary>
            /// �Ȃ�̃A�N�V������
            /// </summary>
            public ActType nowType;

            /// <summary>
            /// ���݂̃A�N�V�����̏�Ԃ�\��
            /// 0�Ȃ炻�̂܂܁A1�Ȃ�`���[�W�A2�Ȃ�`���[�W�A�^�b�N
            /// </summary>
            public int stateNum;

            /// <summary>
            /// ���Ԗڂ̃��[�V������
            /// ���[�V������1���琔���n�߂�
            /// </summary>
            public int motionNum;

            /// <summary>
            /// ���͂̏��
            /// </summary>
            public Equip.InputData inputData;

            /// <summary>
            /// �`���[�W�����J�n�������̎���
            /// </summary>
            public float chargeStartTime;

            /// <summary>
            /// �R���{�U��؂�����
            /// </summary>
            public bool isComboEnd;

            /// <summary>
            /// �����U���ł��邩�ǂ���
            /// </summary>
            public bool isFall;

            /// <summary>
            /// �g�p���閂�@
            /// </summary>
            public PlayerMagic useMagic;

            /// <summary>
            /// ���̍s���ŏ����MP
            /// </summary>
            public float useMP;

            /// <summary>
            /// ���݃��b�N�I�����Ă�G�̔ԍ�
            /// </summary>
            public int lockEnemy;
        }




        #endregion


        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        //       protected const string _attackParameterName = "AttackNow";
        //     protected int _attackAnimationParameter;

        protected const string _typeParameterName = "AttackType";
        protected int _typeAnimationParameter;

        protected const string _numberParameterName = "AttackNumber";
        protected int _numberAnimationParameter;


        [SerializeField]
        //�����蔻�肪�o��܂ł͐U��Ԃ�\�ɂ���
        CircleCollider2D _attackCircle;

        [SerializeField]
        BoxCollider2D _attackBox;

        [SerializeField]
        MyDamageOntouch _damage;


        /// <summary>
        /// �U�����̈ړ��@�\
        /// �����Ă�����ɏ]���ē���
        /// 
        /// �v���P
        /// �ǌ��o�͂���Ȃ��A�ړ������ς����
        /// </summary>
        [SerializeField]
        MyAttackMove _rush;

        [SerializeField]
        PlyerController pc;

        /// <summary>
        /// ���݃^�[�Q�b�g�ɂ��Ă�I�u�W�F�N�g
        /// playerController������Ă����
        /// </summary>
        public GameObject targetEnemy;

        //�����p�����[�^
        #region
        //------------------------------------------�����p�����[�^


        /// <summary>
        /// �g�p����U���̃f�[�^
        /// </summary>
        AttackValue useData;




        //   float gravity;//�d�͂�����
        /// <summary>
        /// �󒆍U���𖳌��ɏo���Ȃ��悤�ɂ���
        /// </summary>
        bool isAirEnd;

        /// <summary>
        /// ���݂̍s���̃f�[�^
        /// </summary>
        NowActionData nowAction;










        //�R���{�����B�o�����[�̐�
        int comboLimit;





        bool fire1Key;
        bool fire2Key;
        bool artsKey;



        /// <summary>
        /// �^�̂Ƃ��U��Ԃ�\
        /// </summary>
        bool _flipable;

        AtEffectCon _atEf;

        /// <summary>
        /// ���@�Ƃ����ˏo����n�_
        /// </summary>
        [SerializeField]
        Transform firePosition;

        /// <summary>
        /// �U���֘A�̏����̃L�����Z���g�[�N��
        /// ���f���Ɏg��
        /// </summary>
        CancellationTokenSource AttackToken;



        #endregion








        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            //�U����Ԃ���Ȃ��Ƃ�
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                //���t���[���n�ʂɍ����������ǂ������m�F
                //�܂��͒n�ʂ��痣�ꂽ�����m�F����
                if (_controller.State.JustGotGrounded || (_controller.State.WasGroundedLastFrame && !_controller.State.IsGrounded))
                {
                    //�ڒn��Ԃ��؂�ւ��������̓��Z�b�g
                    InputReset();

                    //�󒆍U����������x�����悤��
                    isAirEnd = false;
                }
            }

            //�U�����̐U�����
            AttackFlip();
        }





        /// <summary>
        /// �A�r���e�B�T�C�N���̊J�n���ɌĂяo����A�����œ��̗͂L�����m�F���܂��B
        /// </summary>
        protected override void HandleInput()
        {

            //�U���̓���
            AttackInput();


        }






        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_typeParameterName, AnimatorControllerParameterType.Int, out _typeAnimationParameter);
            RegisterAnimatorParameter(_numberParameterName, AnimatorControllerParameterType.Int, out _numberAnimationParameter);
        }

        /// <summary>
        /// �A�r���e�B�̃T�C�N�����I���������_�B
        /// ���݂̂��Ⴊ�ށA�����̏�Ԃ��A�j���[�^�[�ɑ���B
        /// </summary>
        public override void UpdateAnimator()
        {

            //���̃X�e�[�g��Attack�ł��邩�ǂ�����Bool����ւ��Ă�
            // MMAnimatorExtensions.UpdateAnimatorBool(_animator, _attackAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Attack), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _typeAnimationParameter, (int)nowAction.nowType + nowAction.stateNum, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _numberAnimationParameter, nowAction.motionNum, _character._animatorParameters);
        }


        #region ���͏���



        /// �V���͏����̗���
        /// 
        /// �E�܂��{�^��������
        /// �E�������{�^���ɉ����čU�����[�V�����̓��̓^�C�v���擾
        /// �E�^�C�v�ɉ����ă`���[�W���[�V�����Ƃ��������Ȃ�
        /// �E���͊m���̓`���[�W���ǂ����ŕ��򂷂�
        /// 
        /// 
        /// �A�j���[�V�����p�����[�^�͍U���̎�ށA�`���[�W���ǂ����A�U�������ҋ@�����Ō��܂�
        /// �U���̎�ނƃ`���[�W���ǂ����A�ҋ@�����ǂ����A�S��AttackType��int�ōς܂������
        /// 
        /// ///


        #region ���C���̓��̗͂���



        /// <summary>
        /// �U�����͂̎�t
        /// 
        /// ���͎�t��Ԃ̎�ނ͈ȉ�
        /// 
        /// �E���U����
        /// �E�U�����͌��m���i�`���[�W�Ƃ��B�L�����Z�����͂��󂯕t����j
        /// �E�U�������i�����󂯕t���Ȃ��j
        /// �E�U����i���[�V�����L�����Z�����͎�t�B�����ȊO�B�Ȃ񂩃{�^��������������j
        /// 
        /// �񓯊��͂�߂Ƃ���
        /// ���̓^�C�v�i�����A�����đ҂A�����ė����܂Łj�ɂ���ď����𕪂���
        /// </summary>
        void AttackInput()
        {

            //�ʏ��ԂłȂ��āA�U�����ł��Ȃ��Ȃ�߂�
            //�����`���[�W���Ȃ�L�����Z��
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {

                //�U�����łȂ��Ă��܉������͂���Ă�Ȃ�i�`���[�W���A�r�����Ȃ�j������
                //�܂��U�����̃`���[�W���͒��Ƀm�[�}������Ȃ��Ȃ��āA�܂�X�^���Ƃ��������̓`���[�W��ԃL�����Z������
                if (nowAction.inputData.motionInput != Equip.InputType.non)
                {
                    ChargeEnd(true);
                }
                    return;
            }

            //�U���������͓͂ǂ�
            fire1Key = (_inputManager.sAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
            fire2Key = (_inputManager.bAttackButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);
            artsKey = (_inputManager.ArtsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown);




            //�ł��m�[�}������Ȃ��Ƃ��̐�ɂ͍s���Ȃ�
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                return;
            }

            //�����ł͒ʏ��ԁA�U���\��ԈȊO�ł͓��͂��������Ȃ�
            //�������L�����Z���\���݂̂͗�O�ŁA�U���̓��͂�ړ��ɂ���ă��[�V�������I��点��

            //�܂��������͂���ĂȂ��Ȃ�
            if(nowAction.inputData.motionInput == Equip.InputType.non)
            {

                InitialInput();

                //�܂��������͂��Ȃ��Ȃ�
                if (nowAction.inputData.motionInput == Equip.InputType.non)
                {
                    return;
                }
                //���܂����Ȃ珉����������
                else
                {
                    //�ړ����~
                    _characterHorizontalMovement.SetHorizontalMove(0);
                    _controller.SetForce(Vector2.zero);


                    if(nowAction.nowType == ActType.magic)
                    {
                        //���[�V�����ԍ��[���͉r�������̖��@�Ȃ̂ő������s
                        if (nowAction.motionNum == 0)
                        {
                            //���@�J�n
                            MagicAct();
                            return;
                        }

                        //�`���[�W���Ԃ����݂ɂ���
                        nowAction.chargeStartTime = GManager.instance.nowTime;

                        //�X�e�[�g���`���[�W�i�K��
                        //���@�ł����ꂢ��H
                        nowAction.stateNum = 1;

                        //�`���[�W�J�n
                        //�����ɂ͖��@���ǂ�����
                        ChargeStart(true);

                    }
                    //�m�[�}������Ȃ��Ȃ�
                    else if (nowAction.inputData.motionInput != Equip.InputType.normal)
                    {
                        //�`���[�W���Ԃ����݂ɂ���
                        nowAction.chargeStartTime = GManager.instance.nowTime;

                        //�X�e�[�g���`���[�W�i�K��
                        nowAction.stateNum = 1;

                        //�`���[�W�J�n
                        //�����ɂ͖��@���ǂ�����
                        ChargeStart(false);
                    }
                }
            }

            //��������͓��̓^�C�v�m���̏���
            InputController();

        }




        /// <summary>
        /// �ŏ��ɓ��͂��󂯎��A����ɉ����ē��̓^�C�v���󂯎��
        /// ��̏�����i�߂�
        /// 
        /// �i���o�[���w�肷��Ɩ����I�ɋN���ł���
        /// </summary>
        void InitialInput(int numSelect = 99)
        {


            if (artsKey || numSelect == 3)
            {
                if (_controller.State.IsGrounded)
                {

                    //MP�`�F�b�N
                    if (GManager.instance.twinHand || GManager.instance.equipShield.weaponArts)
                    {
                        nowAction.useMP = GManager.instance.equipWeapon.artsMP[nowAction.motionNum];
                    }
                    else
                    {
                        nowAction.useMP = GManager.instance.equipShield.artsMP[nowAction.motionNum]; 
                    }

                    //mp����Ȃ��Ȃ�߂�
                    if(GManager.instance.mp < nowAction.useMP)
                    {
                        return;
                    }

                        nowAction.nowType = ActType.arts;
                }
            }
            else if (fire2Key || numSelect ==2)
            {
                //���@���g�������Ȃ�
                if (GManager.instance.equipWeapon.isMagic)
                {
                    //�g�����@���Ȃ��Ȃ�߂�
                    if(pc.useMagic == null)
                    {
                        return;
                    }

                    //MP�`�F�b�N�ʉ߂��Ȃ��Ȃ�߂�
                    //MP������o����
                    nowAction.useMP = pc.useMagic.useMP * GManager.instance.equipWeapon.magicMultipler.mpMultipler;

                    //mp����Ȃ��Ȃ�߂�
                    if (GManager.instance.mp < nowAction.useMP)
                    {
                        return;
                    }

                    //nowAction�ɖ��@�̃f�[�^�Z�b�g
                    nowAction.useMagic = pc.useMagic;
                    MagicDataSet(nowAction.useMagic,true);
                }
                //�����łȂ��Ȃ狭�U��
                else
                {
                    if (_controller.State.IsGrounded)
                    {
                        nowAction.nowType = ActType.bAttack;
                    }
                    else
                    {
                        nowAction.nowType = ActType.fAttack;
                    }
                }

            }
            else if (fire1Key || numSelect == 1)
            {
                if (_controller.State.IsGrounded)
                {
                    nowAction.nowType = ActType.sAttack;
                }
                else
                {
                    //�󒆍U���o���؂��Ă�Ȃ�����I���
                    if (isAirEnd)
                    {
                        return;
                    }
                    nowAction.nowType = ActType.aAttack;
                }
            }
            else
            {
                return;
            }

            //�`���[�W�\���A���͕������l������
            if(nowAction.nowType != ActType.magic)
            {
                nowAction.inputData= GetInputType(nowAction.nowType,GManager.instance.twinHand,nowAction.motionNum);
            }
        }

        /// <summary>
        /// ���̓f�[�^��Ԃ�
        /// �A�N�V�����Ɨ��莝���ƃ��[�V�����ԍ�����
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isTwinHand"></param>
        /// <param name="actionNum"></param>
        /// <returns></returns>
        Equip.InputData GetInputType(ActType action,bool isTwinHand,int actionNum)
        {
            if(action == ActType.arts)
            {
                //���莝����
                return (isTwinHand || GManager.instance.equipShield.weaponArts) ? GManager.instance.equipWeapon.artsValue.inputData[actionNum] : GManager.instance.equipShield.artsValue.inputData[actionNum];
            }
            else if (action == ActType.bAttack)
            {
                return isTwinHand ? GManager.instance.equipWeapon.twinBValue.inputData[actionNum] : GManager.instance.equipWeapon.bValue.inputData[actionNum];
            }
            else if (action == ActType.sAttack)
            {
                return isTwinHand ? GManager.instance.equipWeapon.twinSValue.inputData[actionNum] : GManager.instance.equipWeapon.sValue.inputData[actionNum];
            }
            else if (action == ActType.aAttack)
            {
                return isTwinHand ? GManager.instance.equipWeapon.twinAirValue.inputData[actionNum] : GManager.instance.equipWeapon.airValue.inputData[actionNum];
            }
            else
            {
                return isTwinHand ? GManager.instance.equipWeapon.twinStrikeValue.inputData[actionNum] : GManager.instance.equipWeapon.strikeValue.inputData[actionNum];
            }
        }


        /// <summary>
        /// ���̓^�C�v���Ƃɓ��͂����s����
        /// 
        /// �`���[�W���͈ʒu�������Ȃ��悤�ɂ���H
        /// �󒆂ł�����{�^���ŃL�����Z���\
        /// </summary>
        void InputController()
        {
            //�C���v�b�g�^�C�v�ɂ���ď����𕪂���
            if (nowAction.inputData.motionInput == Equip.InputType.normal)
            {
                //�������s
                //�U�����s
                AttackAct();
            }
            else if (nowAction.inputData.motionInput == Equip.InputType.chargeAttack)
            {
                ChargeInputExe();
            }
            else if (nowAction.inputData.motionInput == Equip.InputType.waitableCharge)
            {
                WaitableChargeInputExe();
            }
            else if (nowAction.inputData.motionInput == Equip.InputType.magic)
            {


                MagicInputExe();
            }
        }

        #endregion

        #region ���͂̎�ނ��Ƃ̃C���v�b�g����




        /// <summary>
        /// �`���[�W���͂����s����
        /// ���͌�w��b��
        /// </summary>
        void ChargeInputExe()
        {
            //�`���[�W�����̃`�F�b�N
            if (ChargeCancelJudge())
            {
                //�`���[�W�I���Ȃ�߂낤��
                return;
            }



            //�`���[�W�^�C���𒴂����Ȃ�
            if ((GManager.instance.nowTime - nowAction.chargeStartTime) >= nowAction.inputData.chargeTime)
            {
                //��Ԃ��W���[�W�I���ɕύX
                //�U�����s
                //�ƌ����Ă���Ԃ�ς������_�Ń��[�V�������Đ������
                //�̂�ChangeState�Ƃ����邾�����A�U�����s��
                //���ƍU���ړ�������
                nowAction.stateNum = 2;

                //�`���[�W�I�����čU���J�n
                ChargeEnd(false);
            }

            //�������Ԃ𖞂����ĂȂ��ă{�^���𗣂����Ȃ�
            if(!ChargeInputCheck())
            {
                //��Ԃ𖢃`���[�W��
                nowAction.stateNum = 0;
                ChargeEnd(false);

                //����ɒʏ�U�����s�ֈڍs
            }
        }

        /// <summary>
        /// �ҋ@�\�`���[�W���͂����s����
        /// ���͌�w��b��
        /// </summary>
        void WaitableChargeInputExe()
        {
            //�`���[�W�����̃`�F�b�N
            if (ChargeCancelJudge())
            {
                //�`���[�W�I���Ȃ�߂낤��
                return;
            }

            //�{�^�������܂ł̓`���[�W
            if (!ChargeInputCheck())
            {
                //�������Ƃ��`���[�W�^�C���𒴂��Ă�Ȃ�
                if ((GManager.instance.nowTime - nowAction.chargeStartTime) >= nowAction.inputData.chargeTime)
                {
                    //��Ԃ��`���[�W�I���ɕύX
                    //�U�����s
                    //�ƌ����Ă���Ԃ�ς������_�Ń��[�V�������Đ������
                    //�̂�ChangeState�Ƃ����邾�����A�U�����s��
                    //���ƍU���ړ�������
                    nowAction.stateNum = 2;


                    //�`���[�W�I�����čU���J�n
                    ChargeEnd(false);

                }
                //�����ĂȂ������Ȃ�
                else
                {
                    //��Ԃ𖢃`���[�W��
                    nowAction.stateNum = 0;
                    ChargeEnd(false);

                    //����ɒʏ�U�����s�ֈڍs
                }

            }
        }

        /// <summary>
        /// ���@�̓��͂����s����
        /// ���͌�w��b���o�ߌ�A�{�^���𗣂����ƂŔ���
        /// </summary>
        void MagicInputExe()
        {
            //�`���[�W�����̃`�F�b�N
            if (ChargeCancelJudge())
            {
                //�`���[�W�I���Ȃ�߂낤��
                return;
            }

            //�{�^���𗣂��܂ł͑ҋ@��������
            //�{�^�������Ă��r���I���܂ł͏���ɐi��
            if (!ChargeInputCheck())
            {
                //�������Ƃ��`���[�W�^�C���𒴂��Ă�Ȃ�
                if ((GManager.instance.nowTime - nowAction.chargeStartTime) >= nowAction.inputData.chargeTime)
                {
                    //��Ԃ��W���[�W�I���ɕύX
                    //�U�����s
                    //�ƌ����Ă���Ԃ�ς������_�Ń��[�V�������Đ������
                    //�̂�ChangeState�Ƃ����邾�����A�U�����s��
                    //���ƍU���ړ�������

                    //�����͖��@�͓Ǝ������l���Ȃ��Ƃ�
                    nowAction.stateNum = 0;


                    //�`���[�W�I�����čU���J�n
                    ChargeEnd(false);

                }

            }
        }

        #endregion

        #region�@���ʋ@�\


        /// <summary>
        /// �`���[�W���A�U�����ɐU��������s��
        /// �U�����蔭���܂ł͐U�������悤�ɂ�����
        /// ���̂��߂ɂǂ��ŌĂׂ΂����̂��c
        /// </summary>
        void AttackFlip()
        {
            //�U������łĂȂ��U�������A
            //����ӂ��ɍU���A�j���[�V�����C�x���g�ł����邩
            //���ꂩ�񓯊��ōU������o��̂�҂��Ƃɂ���A�U������
            if(_movement.CurrentState == CharacterStates.MovementStates.charging || (_movement.CurrentState == CharacterStates.MovementStates.Attack && _flipable))
            {

            //�������ĂĉE�ɓ��͂���Ă�Ȃ�
            if(_horizontalInput > 0 && !_character.IsFacingRight)
            {
                //�U�����
                _character.Flip();
            }
            //�E�����Ăč��ɓ��͂���Ă�Ȃ�
            else if(_horizontalInput < 0 && _character.IsFacingRight)
            {
                //�U�����
                _character.Flip();
            }
            }

        }




        /// <summary>
        /// �`���[�W�̓��͂��s���Ă��邩�̃`�F�b�N
        /// </summary>
        /// <returns></returns>
        bool ChargeInputCheck()
        {
            if (nowAction.nowType == ActType.sAttack || nowAction.nowType == ActType.aAttack)
            {
                return fire1Key;
            }
            //���U���������U�������@�Ȃ�
            else if (nowAction.nowType == ActType.bAttack || nowAction.nowType == ActType.fAttack || nowAction.nowType == ActType.magic)
            {
                return fire2Key;
            }
            //�ŗL�Z�Ȃ�
            else if (nowAction.nowType == ActType.arts)
            {
                return artsKey;
            }
            return false;
        }

        /// <summary>
        /// �`���[�W��Ԃ��r����ԂɈڍs���鏈��
        /// �ړ����b�N
        /// </summary>
        /// <param name="isCast"></param>
        void ChargeStart(bool isCast)
        {
            if (isCast)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Cast);
            }
            else
            {
                _movement.ChangeState(CharacterStates.MovementStates.charging);
            }
            //�ړ������b�N
            _characterHorizontalMovement.MoveLock();

            //�d�͂�����
            _controller.GravityActive(false);

            //���@�U�������b�N�I���U���Ȃ烍�b�N�����J�n
            if (nowAction.nowType == ActType.magic || useData.baseData.moveData.lockAttack)
            {
                //���@�̎˒��͈͂��U���̎˒�������
                float range;

                //���b�N����
                LockOnController(99,range,_character.IsFacingRight).Forget();
            }

        }


        /// <summary>
        /// ���炩�̌`�Ń`���[�W���I���鎞�̏���
        /// isStop���^�Ȃ璆�~�œ��͏��͏�����
        /// �U�Ȃ�~���I���ōU��(���@)�����J�n
        /// </summary>
        void ChargeEnd(bool isStop)
        {
            //�ړ����A�����b�N
            _characterHorizontalMovement.MoveUnLock();



            if (isStop)
            {
            if (nowAction.nowType == ActType.magic)
            {
                //�r���G�t�F�N�g��~
                _atEf.CastStop(nowAction.useMagic.magicLevel, nowAction.useMagic.magicElement);
            }

                //���͏�������
                InputReset();
            //�d�͂�L����
            _controller.GravityActive(true);


            }
            else
            {
                if (nowAction.nowType == ActType.magic)
                {
                    //�r���G�t�F�N�g����
                    _atEf.CastEnd(nowAction.useMagic.magicLevel, nowAction.useMagic.magicElement);
                    //���@���s
                    MagicAct();
                }
                else
                {
                    //�U�����s
                    AttackAct();
                }

            }
        }

        /// <summary>
        /// ���͏�������������
        /// </summary>
        void InputReset(bool isStop = true)
        {
            //���̓L�����Z��
            nowAction.inputData.motionInput = Equip.InputType.non;

            if (isStop)
            {
                //���[�V������0�ɖ߂�
                nowAction.motionNum = 0;
            }
            //��Ԃ��[����
            nowAction.stateNum = 0;

            //�U�������ĂȂ�
            nowAction.nowType = ActType.noAttack;



    }


        /// <summary>
        /// ����{�^���Ń`���[�W�L�����Z������@�\
        /// �`���[�W���͋��ʂŎg����
        /// </summary>
        bool ChargeCancelJudge()
        {
            //�r���ł��`���[�W�ł��Ȃ��Ȃ�`���[�W�I�����Ė߂�
            if(_movement.CurrentState != CharacterStates.MovementStates.Cast && _movement.CurrentState != CharacterStates.MovementStates.charging)
            {
                ChargeEnd(true);
                return true;
            }

            //��������{�^���������ꂽ��߂�
            if(_inputManager.AvoidButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                ChargeEnd(true);
                return true;
            }
            return false;
        }


        /// <summary>
        /// ���b�N�I������
        /// �`���[�W��ҋ@���̂݌Ă�
        /// 0.2�b�Ɉ�񃍃b�N����������A���͂Ɋ�Â��ă��b�N�I�����鑊���I��
        /// </summary>
        async UniTaskVoid LockOnController(int number,float lockRange,bool isRight)
        {


            //�r�����`���[�W������Ȃ��Ȃ�߂�
            if(_movement.CurrentState != CharacterStates.MovementStates.charging && _movement.CurrentState != CharacterStates.MovementStates.Cast)
            {
                //�ԍ���99�A�܂�ŏ��Ȃ��ԋ߂������
                if(number == 99)
                {
                    number = SManager.instance.PlayerLockEnemySelect(99,lockRange, isRight,false);
                }

                //�����č��̔ԍ��̓G��ݒ�
                nowAction.lockEnemy = number;


                return;
            }

            //�E�����Ă��̂ɍ������Ă�Ȃ�
            //���邢�͍������Ă��̂ɉE�����Ă�Ȃ�
            //�Ƃɂ�����̃t���O���H������Ă�Ȃ�
            //��ԋ߂��G���Ď擾
            if(isRight != _character.IsFacingRight)
            {
                isRight = _character.IsFacingRight;
                number = 99;
            }






            //�^�[�Q�b�g���X�g������̋����ȓ��̓G���l������
            //���͂����邽�тɂ��ꂪ���̃^�[�Q�b�g��
            //����Ɍ����ƌ����Ă�����̓G�������擾����
            //�t���̓G�����b�N�I�����������flip���Ă�
            //�U������ƈ�ԋ߂����������̓G�ɕύX�����

            //�c���͂������
            //�܂��͍ŏ��Ȃ�
            if (_inputManager.SiteMovement.y != 0 || number == 99)
            {
                //����͂��ŏ��i�i���o�[99�j�Ȃ�߂����
                if(_inputManager.SiteMovement.y > 0)
                {
                    number = SManager.instance.PlayerLockEnemySelect(99, lockRange, isRight, false);
                }
                //�����͂Ȃ牓�����
                else
                {
                    number = SManager.instance.PlayerLockEnemySelect(99, lockRange, isRight, true);
                }

            }
            //�����͂������
            else if (_inputManager.SiteMovement.x != 0)
            {
                //�E���͂Ȃ��߂����
                if (_inputManager.SiteMovement.x > 0)
                {
                    number = SManager.instance.PlayerLockEnemySelect(number, lockRange, isRight, false);
                }
                //�����͂Ȃ��������
                else
                {
                   number = SManager.instance.PlayerLockEnemySelect(number, lockRange, isRight, false);
                }

            }


            
            //�ċA�Ăяo���̂��߂�0.2�b�҂�
            await UniTask.Delay(TimeSpan.FromSeconds(2.5), cancellationToken: AttackToken.Token);

            //�ċA�Ăяo��
            LockOnController(number, lockRange, isRight).Forget();

        }






        #endregion





        #endregion


        #region �U�����s�����E�A���R���{����

        /// 
        /// �U����Ԃ��n��
        /// ���ƐU������\�ɂ��čU�����茟�o���\�b�h���Ă�
        /// �`���[�W���ǂ����ŋ����������H
        /// �ʂɕ�����Ȃ����AstateNum�ł킩��܂�
        /// �U���ړ���U���G�t�F�N�g�̋N�����s��
        /// 
        /// ���@���s�ƍU�����s�̓��ޕK�v
        /// 
        /// /// 



        /// <summary>
        /// �U���̊J�n�Ə�Ԃ̕ω�
        /// ��������I���҂����\�b�h���ĂԂ�
        /// �������ǂ����ŋ������ς��
        /// ���Ɨ����Ȃ�A�j���C�x���g�̋������ς��
        /// </summary>
        void AttackAct()
        {

            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            _movement.ChangeState(CharacterStates.MovementStates.Attack);


            //�U�����͏d�͂�����
            _controller.GravityActive(false);

            //�ǂ̍U�����Ăяo����
            #region

            //�f�[�^���Z�b�g
            AttackPrepare(nowAction.nowType,(nowAction.stateNum == 2),nowAction.motionNum);


            //�ǉ��G�t�F�N�g���̂Ƃ���Ȃ�����
            int adType = 0;

            //�U���G�t�F�N�g����
            //�U���G�t�F�N�g�����A�j���C�x���g��
            //�ʓr�ݒ肷��
            _atEf.EffectPrepare(useData.baseData.EffectLevel, adType, useData.baseData.actionImfo.mainElement, useData.baseData.motionType);


            #endregion


            nowAction.motionNum++;

            //���݂̃��[�V�������R���{���E�Ȃ�
            if (nowAction.motionNum >= comboLimit)
            {
            nowAction.isComboEnd = true;

                //�����ɋ󒆍U���Ȃ�
                if(nowAction.nowType == ActType.aAttack)
                {
                    //�󒆍U�����I����
                    isAirEnd = true;
                }
            }

            //���b�N�U���Ȃ�
            if (useData.baseData.moveData.lockAttack)
            {

                targetEnemy = SManager.instance._targetList[nowAction.lockEnemy].targetObj;

                //���̋���
                float distance = GManager.instance.PlayerPosition.x - targetEnemy.transform.position.x;


                //�G�Ƃ̋������ړ��͈͓��ŁA���b�N�I������Ȃ�ړ�������G�̑O�̈ʒu�܂łɏk�߂�
                useData.baseData.moveData._moveDistance = (distance < useData.baseData.moveData._moveDistance) ? distance - 10 : useData.baseData.moveData._moveDistance;
            }





            if ((useData.baseData.actionImfo.feature & AttackValueBase.AttackFeature.fallAttack) > 0)
            {
                //�����U���������܂ނ��ǂ���
                nowAction.isFall = true;

                //���n���[�V����������̂Ŋm���ɃR���{
                //���n���[�V�����͍U��������
                //�����Ɛݒ肵�Ƃ��΂��ꂢ��񂯂ǂ�
                //���������킩��₷����
                useData.baseData.isCombo = true;
            }

            //�U���ړ��J�n
            //�����U�����ǂ����ŋ������ς�邪�A�������͕̂���A�r���e�B������
            _rush.RushStart(useData.baseData.moveData._moveDuration, useData.baseData.moveData._moveDistance, useData.baseData.moveData._contactType,nowAction.isFall , useData.baseData.moveData.startMoveTime, useData.baseData.moveData.backAttack);
            
            GManager.instance.StaminaUse(useData.useStamina);
            GManager.instance.isStUse = true;

            //�w���X���U������
            _health.HealthStateChange(false, DefenseData.DefState.�U����);

            //�w���X���A�[�}�[�t����
            _health.HealthStateChange(false, DefenseData.DefState.�A�[�}�[�t��);



            //�K�[�h�U���Ȃ�K�[�h����ƃX�p�A�}����J�n
            if ((useData.baseData.actionImfo.feature & AttackValueBase.AttackFeature.guardAttack) > 0)
            {
                //�K�[�h�U���J�n
                _health.HealthStateChange(false, DefenseData.DefState.�K�[�h��);
                _health.HealthStateChange(false, DefenseData.DefState.�X�[�p�[�A�[�}�[);
            }
            //�X�p�A�}�U���Ȃ�X�p�A�}�J�n
            else if ((useData.baseData.actionImfo.feature & AttackValueBase.AttackFeature.superArmor) > 0)
            {
                //�X�p�A�}�J�n
                _health.HealthStateChange(false, DefenseData.DefState.�X�[�p�[�A�[�}�[);
            }

            //�U��Ԃ�\���f�J�n
            AttackFlipEndJudge().Forget();
        }


        /// <summary>
        /// �R���{�U�������s����
        /// ���ɂǂ̍U�����o���̂�
        /// ���݂̍U���̃^�C�v���玟�̍U�����m�F
        /// �����`���[�W�Ŏ����`���[�W�U������Ȃ�ⓚ���p�Ń`���[�W�U��
        /// </summary>
        void ComboAttackJudge()
        {
            //����
            ComboAttackPrepare();

            //�`���[�W�R���{�U�����o��
            if(nowAction.stateNum == 2 && NextChargiableCheck())
            {
                //�`���[�W���
                nowAction.stateNum = 2;
            }
            else
            {
                //�`���[�W��Ԃ���Ȃ�
                nowAction.stateNum = 0;
            }

            //�R���{�U�����s
            AttackAct();
        }

        /// <summary>
        /// ���̍U���̓`���[�W�\�����`�F�b�N
        /// </summary>
        /// <returns></returns>
        bool NextChargiableCheck()
        {
            //���̓��̓^�C�v���ӂ�����Ȃ��Ȃ�`���[�W���[�V�������o��
            return GetInputType(nowAction.nowType, GManager.instance.twinHand, nowAction.motionNum).motionInput != InputType.normal;
        }


        //�R���{�A�^�b�N���n�߂邽�߂ɕK�v�ȏ���
        void ComboAttackPrepare()
        {


            //�����U���Ȃ�
            if (nowAction.isFall)
            {
                nowAction.isFall = false;
                //�w���X��Ԃ�߂�
                AttackHealthStateEnd();
                //�d�͂��߂�
                _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;
            }

        }


        #endregion


        #region ���@����

        /// 
        /// 
        /// �r���X�e�[�g�ɍs���Ă��甭��
        /// �G�t�F�N�g��烂�[�V�����̏��͎g�p���@����擾
        /// ���@�Ǘ��̓v���C���[�R���g���[���[�ł�点�邩
        /// 
        /// 




        ///<sammary>
        /// ���@�̃��[�V�����f�[�^������
        /// isCast�^�Ȃ�r���i�K
        /// ���ƃG�t�F�N�g���J�n����
        /// </sammary>
        void MagicDataSet(PlayerMagic useMagic,bool isCast)
        {
            if (isCast)
            {
                nowAction.nowType = ActType.magic;

                /// ���͂̏��
                nowAction.inputData.motionInput = InputType.magic;

                //�r�����Ԑݒ�
                //����̉r�����x�{����������
                nowAction.inputData.chargeTime = useMagic.castTime * GManager.instance.equipWeapon.magicMultipler.castSpeedMultipler;

                nowAction.motionNum = (int)useMagic.castType;
                
                //�r���G�t�F�N�g�J�n
                _atEf.CastStart(useMagic.magicLevel,useMagic.magicElement);
            }
            else
            {
                nowAction.motionNum = (int)useMagic.fireType;
            }

        }



        /// <summary>
        /// �U���̊J�n�Ə�Ԃ̕ω�
        /// ��������I���҂����\�b�h���ĂԂ�
        /// �������ǂ����ŋ������ς��
        /// ���Ɨ����Ȃ�A�j���C�x���g�̋������ς��
        /// </summary>
        void MagicAct()
        {
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            _movement.ChangeState(CharacterStates.MovementStates.Attack);


            //���@�U�����͏d�͂�����
            _controller.GravityActive(false);

            //�ǂ̍U�����Ăяo����
            #region

            //nowAction�ɖ��@�̃f�[�^�Z�b�g
            MagicDataSet(pc.useMagic, false);

            /// 
            /// ���@�̍U���͂͂ǂ����߂邩
            /// �e�ۂ��o�t�{����e���ɐq�˂�DamageOnTouch�ɗ^����
            /// �e�ۂɔ\�͒l�ŕ␳��^������@�́H�@���킩��Ƃ�΂���
            /// ���ꂩ���@�X�e�[�^�X�݂����Ȃ̂�p�ӂ���
            /// �r�����x�A�З́A���Amp����݂����Ȃ̂��X�e�[�^�X�ł��ꂼ��������ł���悤�ɂ���
            /// �␳���x���Ɣ\�͒l�ŌW�����ω�����
            /// 
            /// 
            /// ///


            //�e�یĂяo���̓A�j���C�x���g�ɔC�����H


            //�ǉ��G�t�F�N�g���̂Ƃ���Ȃ�����
            int adType = 0;


            //�K�v�ȗv�f
            //���b�N�I�������G���(nowAction�ɁB�ӂ��̍U���ł��e�ێˏo�U���Ȃ炨�Ȃ����Ƃ��Ȃ�����H)
            //���b�N�I������
            //���@�U���J�n�A�j���C�x���g
            //�e�ێˏo�n�_
            //MP����͒e�ێˏo�J�n�ɂ��邩

            #endregion






            //�U���ړ��J�n
            //�����U�����ǂ����ŋ������ς�邪�A�������͕̂���A�r���e�B������
            _rush.RushStart(nowAction.useMagic.moveData._moveDuration, nowAction.useMagic.moveData._moveDistance, nowAction.useMagic.moveData._contactType, nowAction.isFall, nowAction.useMagic.moveData.startMoveTime, nowAction.useMagic.moveData.backAttack);

            //�X�^�~�i�ǂ����悤����
            GManager.instance.StaminaUse(nowAction.useMagic.useStamina);
            GManager.instance.isStUse = true;

            //�w���X���U������
            _health.HealthStateChange(false, DefenseData.DefState.�U����);

            if (nowAction.useMagic.magicArmor > 0)
            {
                //�w���X���A�[�}�[�t����
                _health.HealthStateChange(false, DefenseData.DefState.�A�[�}�[�t��);
            }


            //�K�[�h���肪�o�閂�@�Ȃ�K�[�h����ƃX�p�A�}����J�n
            if ((nowAction.useMagic.magicFeature & AttackValueBase.AttackFeature.guardAttack) > 0)
            {
                //�K�[�h�U���J�n
                _health.HealthStateChange(false, DefenseData.DefState.�K�[�h��);
                _health.HealthStateChange(false, DefenseData.DefState.�X�[�p�[�A�[�}�[);
            }
            //�X�p�A�}���肪�o�閂�@�Ȃ�X�p�A�}�J�n
            else if ((nowAction.useMagic.magicFeature & AttackValueBase.AttackFeature.superArmor) > 0)
            {
                //�X�p�A�}�J�n
                _health.HealthStateChange(false, DefenseData.DefState.�X�[�p�[�A�[�}�[);
            }

            //�U��Ԃ�\���f�J�n
            AttackFlipEndJudge().Forget();
        }



        #endregion

        #region ���[�V�����ԍ��ƍU���^�C�v�Ɋ�Â��čU���f�[�^���擾

        /// <summary>
        /// �U���̏���
        /// ���@�Ƃ͂����ŏ��������򂷂�
        /// �����܂ł͂����̓��͑҂��󂯂���
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attackNum"></param>
        void AttackPrepare(ActType type,bool isCharge , int attackNum)
        {
            //���@����Ȃ��Ȃ�
            if (type != ActType.magic)
            {
                useData = GetAttackImfo(type, isCharge,attackNum, GManager.instance.twinHand);
                //�_���[�W�ɃA�N�V�����f�[�^��n��
                
                _damage._attackData.actionData = useData.baseData.actionImfo;
                //�����蔻��̋L�^�����Z�b�g
                //���̂ւ��damageOn�̍U���������\�b�h�ɂ܂Ƃ߂Ă�������
                _damage.CollidRestoreResset();
            }
            else
            {

            }
        }



        ///<summary>
        ///�U������Ƃ��ɌĂ�
        /// �U���f�[�^��Ԃ��Ɠ����ɃR���{���E���ݒ肷��
        /// </summary>
        AttackValue GetAttackImfo(ActType type,bool isCharge,int attackNum,bool twinHand)//�f�t�H���a��
        {
            GManager.instance.isShieldAttack = false;
            //�R���{�̍ŏ��ɃR���{����q���邩�m�F����B


            MotionChargeImfo container;

            if (type == ActType.sAttack)
            {
                container = twinHand ? GManager.instance.equipWeapon.twinSValue : GManager.instance.equipWeapon.sValue;
            }
            else if (type == ActType.bAttack)
            {

                container = twinHand ? GManager.instance.equipWeapon.twinBValue : GManager.instance.equipWeapon.bValue;
            }
            else if (type == ActType.aAttack)
            {
                container = twinHand ? GManager.instance.equipWeapon.twinAirValue : GManager.instance.equipWeapon.airValue;
            }
            else if (type == ActType.fAttack)
            {

                container = twinHand ? GManager.instance.equipWeapon.twinStrikeValue : GManager.instance.equipWeapon.strikeValue;
            }
            else// if (type == ActType.arts)
            {
                //���莝��������D��Ȃ畐��
                //�����łȂ��Ȃ珂
                container = (twinHand || GManager.instance.equipShield.weaponArts) ? GManager.instance.equipWeapon.artsValue : GManager.instance.equipShield.artsValue;
            }

            if (!isCharge)
            {
                comboLimit = container.normalComboLimit;
                return container.normalValue[attackNum];
            }
            else
            {
                comboLimit = container.chargeComboLimit;
                return container.chargeValue[attackNum];
            }

        }




        #endregion


        #region�@�U���Ǘ��p�A�j���C�x���g

        /// <summary>
        /// �U�����ɌĂ΂��A�j���C�x���g
        /// �L�����Z���\�_�̒ʒm�A���邢�͗����J�n�̒ʒm
        /// </summary>
        public void Continue()
        {

            if(nowAction.nowType == ActType.arts)
            {
            GManager.instance.MpReduce(nowAction.useMP);
            }



                //�d�͂�L����
                _controller.GravityActive(true);


            //�����U���͂����ŗ����J�n
            if (nowAction.isFall)
            {


                //1.4�{�̏d�͂�������
                _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity * 1.4f;

                //�����U���I���҂�
                //�����ҋ@���Ĉ�莞�Ԍo�ߌ�������t���Ă�������
                //������n�ʂɂ��ĂȂ������璅�n�A�j���X���[��
                FallAttackEndWait().Forget();

            }
            //����ȊO�Ȃ�U�����̃A�[�}�[����������
            //��������L�����Z���\��
            else
            {
                //�w���X�����Ƃɖ߂�
                AttackHealthStateEnd();
                


                //���[�V�����I���������Ă�
                AttackEndWait().Forget();

                //�R���{����Ȃ��Ȃ�L�����Z�����Ă�
                if (!useData.baseData.isCombo)
                {
                    CancelInputWait().Forget();
                }

            }
        }

        /// <summary>
        /// �U���p�̃w���X�̃X�e�[�g��S��������
        /// fall�̂Ƃ��͍U���I�����ɌĂ�
        /// </summary>
        void AttackHealthStateEnd()
        {
            _health.HealthStateChange(true, DefenseData.DefState.�K�[�h��);
            _health.HealthStateChange(true, DefenseData.DefState.�X�[�p�[�A�[�}�[);
            _health.HealthStateChange(true, DefenseData.DefState.�A�[�}�[�t��);
            _health.HealthStateChange(true, DefenseData.DefState.�U����);
        }


        /// <summary>
        /// �U�����ɌĂ΂��A�j���C�x���g
        /// �L�����Z���\�_�̒ʒm�A���邢�͗����J�n�̒ʒm
        /// </summary>
        public void MagicContinue()
        {

                GManager.instance.MpReduce(nowAction.useMP);

            //�e�یĂяo���̓A�j���C�x���g�ɔC�����H
            _atEf.BulletCall(nowAction.useMagic.effects,firePosition.position,firePosition.rotation, nowAction.useMagic.flashEffect);


            //�d�͂�L����
            _controller.GravityActive(true);


            //�����U���͂����ŗ����J�n
            if (nowAction.isFall)
            {


                //1.4�{�̏d�͂�������
                _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity * 1.4f;

                //�����U���I���҂�
                //�����ҋ@���Ĉ�莞�Ԍo�ߌ�������t���Ă�������
                //������n�ʂɂ��ĂȂ������璅�n�A�j���X���[��
                FallAttackEndWait().Forget();

            }
            //����ȊO�Ȃ�U�����̃A�[�}�[����������
            //��������L�����Z���\��
            else
            {
                //�w���X�����Ƃɖ߂�
                AttackHealthStateEnd();



                //���[�V�����I���������Ă�
                AttackEndWait().Forget();

                //�R���{����Ȃ��Ȃ�L�����Z�����Ă�
                if (!useData.baseData.isCombo)
                {
                    CancelInputWait().Forget();
                }

            }
        }




        #endregion


        #region �`���[�W�E�U�����̐U���������


        /// <summary>
        /// �U���U��������\�Ȏ��Ԃ̐��������
        /// �U�����肪�o��܂ł͐U������\
        /// ����P�̂œ���ł���
        /// ���Ńt���O�Ǘ����Ȃ��Ă���
        /// �ǂ����ŏ��͐^������
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid AttackFlipEndJudge()
        {
            //�܂��ŏ��ɐU������\��
            _flipable = true;

            //�����蔻�肪�o��̂�҂�
            await UniTask.WaitUntil(() => (_attackBox.enabled || _attackCircle.enabled),cancellationToken:AttackToken.Token);

            //�U������Ȃ�����
            _flipable = false;

        }


        #endregion



        #region ���[�V�����I���E�L�����Z���ҋ@




        /// <summary>
        /// ���[�V�����̏I���҂�������
        /// �U���I��
        /// �����R���{�h�����K�v
        /// 
        /// �R���{�U���ł͕K�������Ń��[�V�����I���҂����Ă��玟�ɂȂ���
        /// </summary>
        /// <returns></returns>
        private async UniTask AttackEndWait()
        {


            // ���݂̃��[�V�����I���܂őҋ@
            await UniTask.WaitUntil(() => {
                return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f ;
            }, cancellationToken: AttackToken.Token);

            //���͂��ꂽ�����łɍU����Ԃ���Ȃ�������߂�
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                return;
            }

            //�R���{�Ȃ�
            //�`���[�W���Ă邩�ŕ��򂷂���
            //���݃`���[�W���ĂĎ��Ƀ`���[�W�U��������Ȃ�`���[�W��
            if (useData.baseData.isCombo)
            {
                //���݃`���[�W�U�����A�����`���[�W�����邩�𒲂ׂ�
                //�����ɍU�����s
                ComboAttackJudge();
            }
            else
            {
                //�U���I��
                AttackEnd();
            }
        }



        /// <summary>
        /// ���͂ɂ��U����Ԃ̃L�����Z��������
        /// �����R���{�U���ł͌Ă΂�Ȃ�
        /// �R���e�B�j���[�ŌĂ�
        /// </summary>
        /// <returns></returns>
        private async UniTaskVoid CancelInputWait()
        {

            // ���́A���邢�̓A�j���I����҂�
            await UniTask.WaitUntil(() => AnyKey(), cancellationToken: AttackToken.Token);

            //���͂��ꂽ�����łɍU����Ԃ���Ȃ�������߂�
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                return;
            }

            AttackEnd();

            //���@���Ƃ��̃��\�b�h�Ă΂�Ȃ�������S���Ĕ��f���Ă���

            ActType nextType = ActType.noAttack;

            //���̓��͂̔ԍ�
            //����œ��͏����̓r������n�܂�
            int inputNum = 99;

            //���͂��ꂽ���U���{�^���������Ă邩
            if (fire1Key)
            {
                inputNum = 1;
                nextType = _controller.State.IsGrounded ? ActType.sAttack : ActType.aAttack;

                //�󒆍U���I���Ȃ�L�����Z��
                if (isAirEnd && nextType == ActType.aAttack)
                {
                    inputNum = 99;
                    nextType = ActType.noAttack;
                }
            }
            else if (fire2Key)
            {
                inputNum = 2;
                nextType = _controller.State.IsGrounded ? ActType.bAttack : ActType.fAttack;
            }
            else if (artsKey)
            {
                inputNum = 3;
                nextType = ActType.arts;
            }

            //�U�����Ȃ����ʂ̍U��������̂�������ԍ���߂�
            if (nextType == ActType.noAttack || nextType != nowAction.nowType)
            {
                nowAction.isComboEnd = true;
            }




            //�U���I��
            AttackEnd();

            //�U�����͂������Ȃ�
            if (inputNum < 4)
            {
                //���͂ɏ]���Ď��̏����Z�b�g
                InitialInput(inputNum);
                //�܂��������͂��Ȃ��Ȃ�
                if (nowAction.inputData.motionInput == Equip.InputType.non)
                {
                    return;
                }
                //���܂����Ȃ珉����������
                else
                {
                    //�ړ����~
                    _characterHorizontalMovement.SetHorizontalMove(0);
                    _controller.SetForce(Vector2.zero);


                    //�m�[�}������Ȃ��Ȃ�
                    if (nowAction.inputData.motionInput != Equip.InputType.normal)
                    {
                        //�`���[�W���Ԃ����݂ɂ���
                        nowAction.chargeStartTime = GManager.instance.nowTime;

                        //�X�e�[�g���`���[�W�i�K��
                        nowAction.stateNum = 1;

                        //�`���[�W�J�n
                        //�����ɂ͖��@���ǂ�����
                        ChargeStart(nowAction.nowType == ActType.magic);
                    }
                }


            }

        }

        /// <summary>
        /// �����{�^�����������Ă��邩�𒲂ׂ�
        /// </summary>
        /// <returns></returns>
        bool AnyKey()
        {

                if (_inputManager.CheckButtonUsing())
                {
                    return true;
                }
                else
                {
                return (_horizontalInput != 0 || _verticalInput != 0);

                }
        }


        /// <summary>
        /// ������̒n�ʌ������邩�����
        /// </summary>
        async UniTaskVoid FallAttackEndWait()
        {

            

            //�n�ʂɂ��܂ő҂�
            //���邢��2.5�b���܂ő҂�
            await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(2.5), cancellationToken: AttackToken.Token),
                UniTask.WaitUntil(() => _controller.State.IsGrounded, cancellationToken: AttackToken.Token));


            //�n�ʂ��Ă��璅�n���[�V����
            if (_controller.State.IsGrounded)
            {
                //0.01�b�҂�
                await UniTask.Delay(10, cancellationToken: AttackToken.Token);

                
                
                //�����ɃR���{�i���n���[�V�����j�U�����s
                //���n�ŏՌ��g���o��ނȂ炱���Ȃ�
                ComboAttackJudge();

            }

            //�����łȂ��Ȃ�i���n�o�����Ȃ�j�ӂ��ɂ��̂܂܏I��
            //�R���{�⒅�n���[�V���������f�H
            else
            {
                nowAction.isComboEnd = true;
                AttackEnd();
            }

        }








        #endregion


        #region �U���I���E���f����



        /// <summary>
        ///  �U���I�����\�b�h
        /// </summary>
        /// <param name="conti"></param>
       public void AttackEnd()
        {
            

            //��Ԃ����Ƃɖ߂�
           if(_condition.CurrentState != CharacterStates.CharacterConditions.Stunned)
            {
                if (_controller.State.IsGrounded)
                {
                    
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                }
                else
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Falling);
                }
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

           //�^�[�Q�b�g���폜
            targetEnemy = null;

           //�����U���Ȃ�
            if (nowAction.isFall)
            { 
                nowAction.isFall = false;
                //�w���X��Ԃ�߂�
                AttackHealthStateEnd();
                //�d�͂��߂�
                _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;
            }

            //�R���{�I����������ԍ���߂�

                //���̓��Z�b�g
            InputReset(nowAction.isComboEnd);


        nowAction.isComboEnd = false;



        //�x��ăX�^�~�i��
        StaminaRecover().Forget();

        }



        /// <summary>
        /// �U���㏭���҂��ăX�^�~�i�񕜊J�n
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid StaminaRecover()
        {
            await UniTask.Delay(1000, cancellationToken: AttackToken.Token);

            //�A���U�����ĂȂ���Ή񕜊J�n
            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                GManager.instance.isStUse = false;
            }
        }


        /// <summary>
        /// ��ɃX�^���������ɌĂ΂�郁�\�b�h
        /// �s�����~���������Ă���
        /// �L�����Z���g�[�N�����g����
        /// </summary>
        public override void StopAbillity()
        {

            //�`���[�W�I��
            ChargeEnd(true);

            //�d�͂�L����
            _controller.GravityActive(true);



            //�w���X�����Ƃɖ߂�
            AttackHealthStateEnd();


            //�����U���Ȃ�
            if (nowAction.isFall)
            {
                nowAction.isFall = false;
                //�w���X��Ԃ�߂�
                AttackHealthStateEnd();
                //�d�͂��߂�
                _controller.DefaultParameters.Gravity = -GManager.instance.pStatus.firstGravity;
            }

            //���̓��Z�b�g
            InputReset(true);

            nowAction.isComboEnd = false;

            AttackToken.Cancel();
            AttackToken = new CancellationTokenSource();
        }





        #endregion






    }










}