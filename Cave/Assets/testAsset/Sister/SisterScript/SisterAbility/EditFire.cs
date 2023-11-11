using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


/// �U�����͏ꏊ�ς��Ȃ��悤�ɂ���B���f�ŃG�t�F�N�g�c��
/// ���Ɗp�x����ł������m�F
/// z���ʒu����
/// ���ɒǉ�����v�f
/// bool���X�g��MP�ߖ�Ƃ��搧�U���Ƃ��ʂɂ������邩
/// ���邢��MP�g�p�x�݂����Ȃ̂�int�ō��`�F�b�N���Ă锠�łP�A�Q�Ƃ����l�ς��邩
/// ���ꂩ������펝������H
/// �Ή����˂͓������z�[�~���O��ClosestEnemy���^�[�Q�b�g��
/// �G���Ńg���K�[���������Ȃ�
/// ���ꂼ��̏����ɖ��@������
/// �Ȃ������画�f���ĒT��
/// �~�܂�؂ɐG���AI�������ʂɍs���ƈ�U�������@�𔒎��ɂ���

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// ���ʂɏ󋵔��f���ē������߂̃X�N���v�g
    /// ���[�v�A�U���A�R���r�l�[�V�����i�e����͕�����H�j�͕ʃX�N���v�g��
    /// ���邢�̓R���r�l�[�V�����͎�l���Ɏ�������H
    /// ����Ƃ��Ă͔��f�A�r���A�U����ԂɑJ�ڂ����[�V�����ω��A�A�j���[�V�����C�x���g�A���@�g�p�A�A�j���[�V�����C�x���g�ŏI��
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/FireAbility")]
    public class EditFire : MyAbillityBase
    {

        public override string HelpBoxText() { return "�V�X�^�[����̍U��"; }


        // Animation parameters
        //0���Ȃ���Ȃ��A�P�����@�A�Q���R���r�l�[�V�����A�R���r��
        protected const string _actParameterName = "actType";
        protected int _actAnimationParameter;

        /// <summary>
        /// ���[�V������I��
        /// </summary>
        protected const string _motionParameterName = "motionNum";
        protected int _motionAnimationParameter;

        //�������f��U���̃p�����[�^�[
        public SisterParameter sister;

        /// <summary>
        /// �V�X�^�[����̊�bAI
        /// </summary>
        [HideInInspector]
        public BrainAbility sb;

        /// <summary>
        /// �N�[���^�C���ҋ@�̂��ߐ�p�̃t���O
        /// </summary>
        bool disEnable;


        List<SisMagic> useSupport;//���g�p�̎x��
        List<float> effectiveTime;//�x�����@�A���W�F�l�A�U���̎��Ԃ��͂���
        /// <summary>
        /// �r�����ԑ҂�
        /// </summary>
        [HideInInspector]
        public float waitCast;

        float coolTime;
        /// <summary>
        /// �U���A�񕜁A�Ȃǂ̗D��s�������ւ���
        /// </summary>
        float stateJudge = 30;

        /// <summary>
        /// �G���m�A���f�̎��Ԍv��
        /// </summary>
        float targetJudge = 30;

        /// <summary>
        /// ���ڂ̏����Ń^�[�Q�b�g�����������m�F����
        /// </summary>
        [HideInInspector]
        public int judgeSequence;


        /// <summary>
        /// �r���̉��炷�T�C��
        /// 1�͂܂��Đ����ĂȂ��B2�͍Đ����B3�͏I���
        /// </summary>
        bool soundStart;
        /// <summary>
        /// ���e�ۂ̐�
        /// </summary>
        int bCount;
        float recoverTime;

        List<int> targetCanList = new List<int>();

        List<int> magicCanList = new List<int>();



        /// <summary>
        /// ���Ԗڂ̉r���Ȃ̂�
        /// </summary>
        int actionNum;

        [SerializeField] CombinationAbility ca;






        bool fireStart;

        /// <summary>
        /// �퓬�J�n���̏������̂��߂̃t���O
        /// </summary>
        [HideInInspector]
        public bool isReset;
        //�񕜎���
        float healJudge;


        /// <summary>
        /// �e�ې����̍Œ����ǂ���
        /// </summary>
        bool delayNow;
        //�r�b�g���Z�ŃN�[���^�C����j������
        int _skipCondition;

        public AtEffectCon atEf;



        //protected RewiredCorgiEngineInputManager _inputManager;


        //-------------------------------------------�o�t�̐��l

        /// <summary>
        /// �U���{��
        /// </summary>
        [HideInInspector]
        public float attackFactor = 1;
        [HideInInspector]
        public float fireATFactor = 1;
        [HideInInspector]
        public float thunderATFactor = 1;
        [HideInInspector]
        public float darkATFactor = 1;
        [HideInInspector]
        public float holyATFactor = 1;



        /// <summary>
        /// �e�ې������Ƀ^�[�Q�b�g�������Ă������悤�Ɉʒu���o���Ă���
        /// </summary>
        Vector2 _tgPosition;


        CancellationTokenSource magicCancel;


        /// <summary>
        /// �^�[�Q�b�Ƃ̔ԍ���ۑ�
        /// </summary>
        int targetNum;


        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            sister.nowMove = sister.priority;

            //Brain����Ȃ���ł���
            sb = _character.FindAbility<BrainAbility>();
            atEf = _character.FindAbility<AtEffectCon>();

            SManager.instance.MagicEffectSet(atEf);
        }


        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();


            FireAct();
        }


        /// <summary>
        /// �U���̎��s
        /// �Ȃ�ׂ��񓯊�
        /// �ł�
        /// </summary>
        async void FireAct()
        {



            //�U������
            //�񓯊��ŉr����҂H
            //�R���r�l�[�V�����������݂ǂ������
            //�R���r�l�[�V����������Ȃ��Ȃ���������ށH
            //�L�����Z���g�[�N���͎��S���ɏ����悤�ɂ��邩
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {

                //MagicUse(SManager.instance.useMagic.HRandom, SManager.instance.useMagic.VRandom);
                return;
            }




            //�퓬���A���ʒu�ɂ��Ă���Ȃ�
            else if (sb.nowState == BrainAbility.SisterState.�킢)
            {

                //�������@�Ȃ��Ȃ�߂�
                if (sb.status.equipMagic == null)
                {
                    return;
                }



                //�^�[�Q�b�g����������r����߂�
                castCheck();



                stateJudge += _controller.DeltaTime;

                ///�^�[�Q�b�g�Ǝg�p���@�ݒ�
                //�N�[���^�C�����ł��X�L�b�v����������Ȃ瓮��
                #region

                //�ʏ��Ԃ���Ȃ��Ȃ�߂�
                if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
                {
                    return;
                }


                    //�ʒu�ɂ��Ă��Ȃ����A�N�[���^�C���������łȂ����X�L�b�v�R���f�B�V�������Ȃ��Ȃ�
                    if ((disEnable && _skipCondition == 0) || !sb.nowPosition)
                    {
                        return;
                    }

                    bool reset = false;


                //����̓v���C�I���e�B������Ȃ�ŏ��Ɉ�莞�Ԃ��ƂɃ��[�h���Z�b�g����񓯊����Ă�
                    //��莞�Ԍo�߂Ő퓬�v�l���A�s�����Ȃ��łȂ�����͗D�悷���Ԃɖ߂�
                    if (stateJudge >= sister.stateResetRes && sister.priority != SisterParameter.MoveType.�Ȃ�)
                    {
                        //�X�e�[�g���Z�b�g
                        sister.nowMove = sister.priority;
                        stateJudge = 0.0f;
                        reset = true;
                    }



                    //���Ԍo�߂��^�[�Q�b�g���ł��X�e�[�g�`�F���W�ōĔ��f
                    if (targetJudge >= sister.targetResetRes + 2 || SManager.instance.target == null || reset)
                    {

                    }
                    TargetReset(reset);

                #endregion

                //�����ƃ^�[�Q�b�g�Ǝg�p���@���ݒ肳��Ă��邩�̃`�F�b�N
                //���ꂪ�I���Δ���

                if (SManager.instance.target != null && SManager.instance.useMagic != null)
                {
                    //MP�Ȃ��Ȃ瓦����
                    if (sb.mp < SManager.instance.useMagic.useMP)
                    {
                        sb.PositionJudge();
                        return;
                    }

                    //	bool isWrong = false;
                    //�g�p���@���U���ŁA���^�[�Q�b�g���G�ł���B
                    if (SManager.instance.useMagic.mType == SisMagic.MagicType.Attack)
                    {
                        if (sister.nowMove != SisterParameter.MoveType.�U��)
                        {
                            return;
                        }

                        coolTime = sister.attackCT[judgeSequence];
                        _skipCondition = sister.atSkipList[judgeSequence];
                    }
                    else if (SManager.instance.useMagic.mType == SisMagic.MagicType.Recover)
                    {
                        if (sister.nowMove != SisterParameter.MoveType.��)
                        {
                            return;
                            //isWrong = true;
                        }
                        coolTime = sister.healCT[judgeSequence];
                        _skipCondition = sister.hSkipList[judgeSequence];
                    }
                    else
                    {
                        if (sister.nowMove != SisterParameter.MoveType.�x��)
                        {
                            return;
                        }
                        coolTime = sister.supportCT[judgeSequence];
                        _skipCondition = sister.sSkipList[judgeSequence];
                    }



                    //�^�[�Q�b�g�����Ďg�p���閂�@�������Ďg�p���@�ƃ^�[�Q�b�g�����ݍ����Ă���Ȃ�

                    ActionFire();

                }


            }

            //���ݐ킢������Ȃ��Ȃ�
            else if (sb.nowState != BrainAbility.SisterState.�킢)
            {
                //�ꉞ���Z�b�g�Ɛ퓬�J�n���Ƀ��Z�b�g���Ă��炦��悤�d����


                if (sister.autoHeal && !disEnable)
                {
                    //bool healAct = false;


                    if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
                    {
                        healJudge += _controller.DeltaTime;
                        if (healJudge >= 3f)
                        {

                            for (int i = 0; i < sister.recoverCondition.Length; i++)
                            {
                                if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                                {
                                    continue;
                                }
                                if (HealJudge(sister.nRecoverCondition[i]))
                                {
                                    RecoverAct(sister.nRecoverCondition[i]);
                                    judgeSequence = i;

                                    break;
                                }
                            }


                            healJudge = 0;
                        }
                        else if (healJudge < 3f)
                        {

                            SManager.instance.target = null;
                        }
                    }
                    if (SManager.instance.target != null && SManager.instance.useMagic != null)
                    {

                        if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
                        {
                            //		Debug.Log("hhhhhhj");
                            actionNum = (int)SManager.instance.useMagic.castType;
                            _movement.ChangeState(CharacterStates.MovementStates.Cast);
                            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
                        }
                        coolTime = sister.autHealCT[judgeSequence];
                        _skipCondition = sister.ahSkipList[judgeSequence];

                        ActionFire();
                    }
                }

            }



        }

        /// <summary>
        /// �r���Ǘ����\�b�h
        /// �r������������U����Ԃɓ����ă��[�V�����J��
        /// </summary>
        /// <param name="random"></param>
        public async UniTaskVoid ActionFire()
        {
            if (disEnable)
            {

                disEnable = false;
            }


            //�g�p���閂�@��MP���Ȃ����A�W�I�����Ȃ��Ȃ�߂�
            if (sb.mp < SManager.instance.useMagic.useMP && SManager.instance.target == null)
            {
                //�Ƃ肠�����J�����������
                atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                actionNum = 0;


                return;
            }


            //�r���J�n�A�~�܂�
            _controller.SetHorizontalForce(0);

            actionNum = (int)SManager.instance.useMagic.castType;
            _movement.ChangeState(CharacterStates.MovementStates.Cast);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);


            CastCircle();

            //���ԑ҂�
            //�����͍U�������ƒ��f���邩�A����Ƃ��f�ŋF�邩�Ƃ����߂���悤��
            await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.castTime), cancellationToken: magicCancel.Token);




            //�r���I��
            atEf.CastEnd(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

            //���@�����Ăяo��

        }

        /// <summary>
        /// X��Y�̊Ԃŗ������o��
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y, bool setSeed = false, int seed = 0)
        {

            return UnityEngine.Random.Range(X, Y + 1);

        }




        /// <summary>
        ///���͉񕜂Ȃǂ̂��߂ɍs�����ƂɃN�[���^�C���ݒ�\ 
        /// </summary>
       async UniTaskVoid CoolTimeWait()
        {

                //���@�g�p�㌻�݂̗����ʒu���Ԉ���ĂȂ����𔻒�
                sb.PositionJudge();

            //���ԑ҂�
            await UniTask.Delay(TimeSpan.FromSeconds(coolTime + 0.5),cancellationToken:magicCancel.Token);

            if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
            {
                await UniTask.WaitUntil(() => _condition.CurrentState != CharacterStates.CharacterConditions.Moving,cancellationToken: magicCancel.Token);
            }

                    disEnable = false;
                    _skipCondition = 0;
                    waitCast = 0;



        }



        /// <summary>
        /// �r�����[�V�����̃A�j���C�x���g
        /// �r�����̂��߂ɉ��ƃG�t�F�N�g���Z�b�g
        /// �g�p���閂�@�������ɂ��Ė��@���x���Ƒ����Ō���
        /// �r���̉���G�t�F�N�g�ς������Ȃ炱���ł�����
        public void CastCircle()
        {


            atEf.CastStart(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
            float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
            sb.SisFlip(dir);
            soundStart = true;

        }


        /// <summary>
        /// �O�����疂�@���f������
        /// </summary>
        public void MagicEnd()
        {

            _skipCondition = 0;
            //disEnable = false;
            stateJudge = 0;
            waitCast = 0;

            //�G�t�F�N�g����
            if (soundStart)
            {
                atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
            }

            actionNum = 0;
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }


        /// <summary>
        /// MP�����@�g�p�ł��Ȃ��قǌ������āA�Ȃ�����MP�x�����O�ł��邩�ǂ������m�F���郁�\�b�h
        /// </summary>
        public bool MPCheck()
        {

            return (SManager.instance.useMagic.useMP > sb.mp && bCount < 2);

        }



        /// <summary>
        /// for���ł͂Ȃ���	bcount�𒴂���܂�useMagic���^�Ȃ̂Ŕ�����������
        /// �e�ۂ���郁�\�b�h
        /// </summary>
        /// <param name="hRandom"></param>
        /// <param name="vRandom"></param>
        async UniTaskVoid MagicUse(int hRandom, int vRandom)
        {

            //�^�[�Q�b�g���Ȃ��Ȃ�߂�
            if ()
            {
                bCount = 0;
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);

                actionNum = 0;
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                SManager.instance.useMagic = null;

                return;
            }


            //�N�[���^�C���҂�
            disEnable = true;
            //�U���J�n
            _movement.ChangeState(CharacterStates.MovementStates.Attack);
           
            //���[�V�����Z�b�g
            actionNum = (int)SManager.instance.useMagic.FireType;


            //���͏���
                sb.mp -= SManager.instance.useMagic.useMP;


            //Target�̈ʒu���L���b�V���B�G������ł������悤��
            _tgPosition.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y);


            //���˒n�_�Z�b�g

            Vector3 goFire;

            //�G�̈ʒu�ɃT�[�`�U������Ƃ�
            if (SManager.instance.useMagic.isChaice)
            {
                goFire = _tgPosition;

            }
            //����ȊO�Ȃ�ˏo�_����o��
            else
            {
                goFire = sb.firePosition.position;
            }


            //�~�蒍���n�̂�Ȃ�ˏo�p�x�����߂�
            //�}�l�[�W���[�ɓ���Ă�̂͒e����g����悤��
            if (SManager.instance.useMagic._moveSt.fireType == SisMagic.FIREBULLET.RAIN)
            {
                //�R�Ȃ�̒e���őł������Ƃ��Ƃ��ˏo�p�x���߂ꂽ�炢������
                //�ʒu�������_���ɂ���Ίp�x�͂ǂ��ł�������������
                SManager.instance.useAngle = GetAim(sb.firePosition.position, _tgPosition);

            }

            //�ꉞ��`
            Vector3 restoreFirePosi = Vector3.zero;

            //�e�ۂ��o��
            for (int i = 0;i < SManager.instance.useMagic.bulletNumber;i++)
            {
                //���ڈȊO�ő҂����Ԃ���Ȃ�҂�
                if (i != 0 || SManager.instance.useMagic.delayTime > 0)
                {
                    //���ԑ҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime), cancellationToken: magicCancel.Token);

                    //�����_���Ȉʒu�ɔ�������Ƃ�
                    //�����Ō��̃|�W�����ڂ��Ă���
                    if (hRandom != 0 || vRandom != 0)
                    {
                        restoreFirePosi.Set(goFire.x,goFire.y,goFire.z);
                    }

                }
                //���ڈȊO�͎ˏo�X�������_�����ǂ����̔��f������
                else
                {

                    //�����_���Ȉʒu�ɔ�������Ƃ�
                    if (hRandom != 0 || vRandom != 0)
                    {
                        goFire.Set(restoreFirePosi.x,restoreFirePosi.y,restoreFirePosi.z);

                        float xRandom = 0;
                        float yRandom = 0;
                        if (hRandom != 0)
                        {

                            xRandom = RandomValue(-hRandom, hRandom);

                        }
                        if (vRandom != 0)
                        {
                            yRandom = RandomValue(-vRandom, vRandom);
                        }

                        goFire.Set(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//�e������
                    }

                }

                //�R���r�l�[�V�������Ȃ�҂�
                if (_movement.CurrentState == CharacterStates.MovementStates.Combination)
                {
                    await UniTask.WaitUntil(() => _movement.CurrentState == CharacterStates.MovementStates.Attack, cancellationToken: magicCancel.Token);
                }

                //��������e�ۂ��o��
                atEf.BulletCall(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation), SManager.instance.useMagic.flashEffect);


            }




            //�e�ۂ𐶐����I�������


                bCount = 0;
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);

                actionNum = 0;
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                SManager.instance.useMagic = null;


                //�e�ۑS���o���I������烍�b�N�J�[�\�������F�ɖ߂�
                //�����A���|���X�g����i���o�[�Ŕ�΂���
                if (SManager.instance.restoreTarget != null && SManager.instance.target != GManager.instance.Player)
                {
                    SManager.instance.restoreTarget.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
                }

                //�����ʒu�m�F
                sb.PositionJudge();

        }





        ///<summary>
        ///���f�ɗ��p
        /// </summary>
        #region

        ///<summary>
        ///�W�I�ƍs�����Đݒ肷��
        ///���Z�b�g���͍U���̏ꍇ�^�[�Q�b�g������JudgeSequence��ς���
        ///�����ƃV�[�P���X��������ւ���
        /// </summary>
        void TargetReset(bool _reset)
        {
            //�U���X�e�[�g��
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {


                if (_reset)
                {
                    SManager.instance.target = null;
                    judgeSequence = 0;
                }


                //�^�[�Q�b�g�����Ȃ��Ȃ�^�[�Q�b�g��T���܂��B
                if (SManager.instance.target == null)
                {

                    //�ܔԖڂ܂ł��
                    //�ܔԖڂ܂ł����璷������1�����Ă�
                    //�X�L�b�v�R���f�B�V���������鏈��
                    for (int i = 0; i < sister.targetCondition.Length; i++)
                    {
                        //	int skiCheck =  (int)Mathf.Pow(2, i)
                        //0��͂P
                        //�N�[���^�C�����ŁA�Ȃ����X�L�b�v�R���f�B�V�����ɓ��Ă͂܂�Ȃ��Ȃ珈�����΂��B
                        //�V�t�g���Z�H
                        if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                        {
                            continue;
                        }

                        SManager.instance.target = TargetSelect(sister.targetCondition[i], sister.AttackCondition[i]);

                        if (SManager.instance.target != null)
                        {
                            //	Debug.Log($"{SManager.instance.target.name}");
                            judgeSequence = i;
                            break;
                        }


                    }
                    //	Debug.Log($"s2");



                    //����ł��^�[�Q�b�g�����Ȃ�������⑫�s��
                    if (SManager.instance.target == null && !disEnable)
                    {
                        if (sister.AttackCondition[5].condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || sister.AttackCondition[5].condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                        {
                            SisterStateChange((int)sister.AttackCondition[5].condition);
                            return;
                        }
                        else if (sister.AttackCondition[5].condition != FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                        {
                            SManager.instance.target = SManager.instance.targetList[RandomValue(0, SManager.instance.targetList.Count - 1)];
                            judgeSequence = 5;
                        }
                    }

                    //EnemyRecord��targetCondition�͈�v���Ă�B
                    //�G���X�V
                }

                if (SManager.instance.target != null)
                {

                    //�N�[���^�C�������ł��ĂȂ��ă��Z�b�g����Ȃ��Ȃ�_��
                    if (!_reset && disEnable)
                    {
                        return;
                    }

                    AttackAct(sister.AttackCondition[judgeSequence]);

                }


            }








            //�x���̎��͑Ώۂ͌��܂��Ă�̂ŏ����ɓ��Ă͂܂�󋵂�
            //�����ē��Ă͂܂�x�������邩�𒲂ׂ�
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {
                SManager.instance.target = GManager.instance.Player;

                //���Z�b�g�Ȃ画�f���Ȃ���
                if (_reset)
                {

                    for (int i = 0; i < sister.supportPlan.Length; i++)
                    {
                        if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                        {
                            continue;
                        }

                        //	�N�[���^�C�����ł͂Ȃ��Ō�̏����Ȃ疳�����ł����͒ʂ�
                        if (i == sister.supportPlan.Length - 1 && !disEnable)
                        {
                            SupportAct(sister.supportPlan[i]);
                            judgeSequence = i;
                        }
                        else if (SupportJudge(sister.supportPlan[i]))
                        {

                            SupportAct(sister.supportPlan[i]);
                            judgeSequence = i;
                            break;
                        }
                    }
                }

                //���Z�b�g����Ȃ��Ȃ瓯��JudgeSequence�Ŕ��f
                else
                {
                    if (SupportJudge(sister.supportPlan[judgeSequence]))
                    {
                        SupportAct(sister.supportPlan[judgeSequence]);

                    }
                }

            }
            //�x���Ɠ���
            else if (sister.nowMove == SisterParameter.MoveType.��)
            {
                SManager.instance.target = GManager.instance.Player;

                if (_reset)
                {
                    for (int i = 0; i < sister.recoverCondition.Length; i++)
                    {
                        if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                        {
                            continue;
                        }

                        //	�N�[���^�C�����ł͂Ȃ��Ō�̏����Ȃ疳�����ł����͒ʂ�
                        if (i == sister.recoverCondition.Length - 1 && !disEnable)
                        {
                            RecoverAct(sister.recoverCondition[i]);
                            judgeSequence = i;
                        }
                        else if (HealJudge(sister.recoverCondition[i]))
                        {
                            RecoverAct(sister.recoverCondition[i]);
                            judgeSequence = i;

                            break;
                        }
                    }
                }
                //���Z�b�g����Ȃ��Ȃ瓯��JudgeSequence�Ŕ��f
                else
                {
                    if (HealJudge(sister.recoverCondition[judgeSequence]))
                    {
                        RecoverAct(sister.recoverCondition[judgeSequence]);
                    }
                }
            }
        }

        /// <summary>
        /// �U���X�e�[�g�Ń^�[�Q�b�g������
        /// </summary>
        /// <param name="condition"></param>
        public GameObject TargetSelect(AttackJudge condition, FireCondition act)
        {

            //�ŏ��Ƀ��X�g���N���A
            targetCanList.Clear();


            switch (condition.condition)
            {

                //�w��Ȃ��̓v���C���[�̃w�C�g�Ō��߂邩�H
                //�U���Ƃ����ꂽ�Ƃ������I�ɂ����Ƃ�

                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.�w��Ȃ�:
                    //	�����_���o�����[�g���ă��R�[�h����w��

                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                    {
                        
                        SisterStateChange((int)act.condition);
                        return null;
                    }
                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                    {
                        return null;
                    }

                    //9999�͑S�Ă̈�
                    targetCanList.Add(9999);

                    return SecondTargetJudge(condition);

                //-----------------------------------------------------------------------------------------------------

                case AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������:
                    //	�����_���o�����[�g���ă��R�[�h����w��
                    if (condition.highOrLow)
                    {
                        if (sb.pc.HPRatio() >= condition.percentage / 100f)
                        {
                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }

                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);

                            return SecondTargetJudge(condition);
                        }
                    }
                    else
                    {

                        if (sb.pc.HPRatio() <= condition.percentage / 100f)
                        {

                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {
                                //kuroko++;
                                //Debug.Log($"����{sister.nowMove}");

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }

                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);

                            return SecondTargetJudge(condition);
                        }
                    }
                    break;
                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������:
                    //	�����_���o�����[�g���ă��R�[�h����w��
                    if (condition.highOrLow)
                    {
                        if (GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f)
                        {
                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }
                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);

                            return SecondTargetJudge(condition);
                        }
                    }
                    else
                    {
                        if (GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f)
                        {

                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }

                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);

                            return SecondTargetJudge(condition);
                        }
                    }
                    break;
                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������://������
                                                              //	�����_���o�����[�g���ă��R�[�h����w��

                    //9999�͑S�Ă̈�
                    targetCanList.Add(9999);

                    return SecondTargetJudge(condition);

                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G://������
                                                         //	�����_���o�����[�g���ă��R�[�h����w��

                    //9999�͑S�Ă̈�
                    targetCanList.Add(9999);
                    return SecondTargetJudge(condition);

                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.������MP���K��l�ɒB������:
                    if (condition.highOrLow)
                    {
                        if (sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f)
                        {
                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }

                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);
                            return SecondTargetJudge(condition);
                        }
                    }
                    else
                    {

                        if (sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f)
                        {
                            if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                            {

                                SisterStateChange((int)act.condition);
                                return null;
                            }
                            else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                            {
                                return null;
                            }

                            //9999�͑S�Ă̈�
                            targetCanList.Add(9999);
                            return SecondTargetJudge(condition);
                        }
                    }
                    break;
                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.���G�̑���:
                    //���G��D��


                    if (condition.highOrLow)
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            if (SManager.instance.targetCondition[i].status.strong)
                            {
                                if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                {

                                    SisterStateChange((int)act.condition);
                                    return null;
                                }
                                else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                {
                                    return null;
                                }
                                targetCanList.Add(i);
                                //break;

                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {

                            if (!SManager.instance.targetCondition[i].status.strong)
                            {
                                if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                {

                                    SisterStateChange((int)act.condition);
                                    return null;
                                }
                                else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                {
                                    return null;
                                }
                                targetCanList.Add(i);
                                //break;

                            }
                        }
                    }
                    return SecondTargetJudge(condition);
                //�����ɓ񎟏����O��������CanList�������ɊJ�n


                //-----------------------------------------------------------------------------------------------------
                case AttackJudge.TargetJudge.�G�^�C�v:
                    //   Soldier,//���̎G��




                    //�I�ԓG�^�C�v�����ׂĂƑI������Ă�Ȃ�
                    if (condition.percentage == 0b00011111)
                    {

                        if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                        {

                            SisterStateChange((int)act.condition);
                            return null;
                        }
                        else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                        {
                            return null;
                        }


                        //9999�͑S�Ă̈�
                        targetCanList.Add(9999);

                        return SecondTargetJudge(condition);
                        //break;
                    }
                    else
                    {
                        //int test;
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            //test = ;


                            if ((condition.percentage & 0b00000001) == 0b00000001)
                            {

                                if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Soldier)
                                {
                                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                    {

                                        SisterStateChange((int)act.condition);
                                        return null;
                                    }
                                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                    {
                                        return null;
                                    }
                                    targetCanList.Add(i);

                                    continue;
                                }
                            }
                            //test = 0b01000000;
                            if ((condition.percentage & 0b00000010) == 0b00000010)
                            {
                                //	Debug.Log($"���������Ă�{SManager.instance.targetCondition == null}{SManager.instance.targetCondition.Count}��{i}");
                                if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Fly)
                                {
                                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                    {

                                        SisterStateChange((int)act.condition);
                                        return null;
                                    }
                                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                    {
                                        return null;
                                    }
                                    targetCanList.Add(i);

                                    continue;
                                }
                            }
                            if ((condition.percentage & 0b00000100) == 0b00000100)
                            {
                                if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Shooter)
                                {
                                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                    {

                                        SisterStateChange((int)act.condition);
                                        return null;
                                    }
                                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                    {
                                        return null;
                                    }
                                    //	siroko++;
                                    //		Debug.Log($"���̐���{siroko}");
                                    targetCanList.Add(i);

                                    continue;
                                }
                            }
                            if ((condition.percentage & 0b00001000) == 0b00001000)
                            {
                                if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Knight)
                                {
                                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                    {

                                        SisterStateChange((int)act.condition);
                                        return null;
                                    }
                                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                    {
                                        return null;
                                    }
                                    //	siroko++;
                                    //		Debug.Log($"���̐���{siroko}");
                                    targetCanList.Add(i);

                                    continue;
                                }
                            }
                            if ((condition.percentage & 0b00010000) == 0b00010000)
                            {
                                if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Trap)
                                {
                                    if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                                    {

                                        SisterStateChange((int)act.condition);
                                        return null;
                                    }
                                    else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                                    {
                                        return null;
                                    }
                                    targetCanList.Add(i);

                                    continue;
                                }
                            }
                        }
                        return SecondTargetJudge(condition);
                    }


                    //-----------------------------------------------------------------------------------------------------
            }
            return null;
        }

        /// <summary>
        /// �U���X�e�[�g�Ŏg�p���@������
        /// </summary>
        /// <param name="condition"></param>
        public void AttackAct(FireCondition condition)
        {


            if (condition.UseMagic == null)
            {
                magicCanList.Clear();
                switch (condition.condition)
                {
                    case FireCondition.ActJudge.�a������:


                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.slash)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�h�ˑ���:

                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.stab)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�Ō�����:

                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].magicElement == AtEffectCon.Element.strike)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.������:

                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].holyBase > 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�ő���:

                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].darkBase > 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.������:


                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].fireBase > 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        //Debug.Log($"asgd{magicCanList[0].name}");
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.������:
                        //Debug.Log($"ssssss");

                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].thunderBase > 0)
                            {
                                //		Debug.Log($"���i�K{SManager.instance.attackMagi[i].name}");
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�ő���:
                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].thunderBase >= 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�����w��Ȃ�:

                        magicCanList.Add(9999);

                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�ړ����x�ቺ�U��://������
                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].thunderBase >= 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�U���͒ቺ�U��://������
                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].thunderBase >= 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                    //-----------------------------------------------------------------------------------------------------
                    case FireCondition.ActJudge.�h��͒ቺ�U��://������
                        for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
                        {
                            if (SManager.instance.attackMagi[i].thunderBase >= 0)
                            {
                                magicCanList.Add(i);
                                break;
                            }
                        }
                        secondATMagicJudge(condition);
                        break;
                }
            }
            else
            {

                if (condition.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || condition.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
                {

                    SisterStateChange((int)condition.condition);
                    return;
                }
                else if (condition.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
                {
                    return;
                }
                SManager.instance.useMagic = condition.UseMagic;
            }
        }

        public bool SupportJudge(SupportCondition condition)
        {

            switch (condition.sCondition)
            {
                /*	case SupportCondition.SupportStatus.�������Ă��Ȃ��x��������:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�:

                    if (condition.highOrLow)
                    {
                        return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������:

                    if (condition.highOrLow)
                    {
                        return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.������MP���K��l�ɒB������:

                    if (condition.highOrLow)
                    {
                        return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������:

                    return GManager.instance.badCondition ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.���G�����邩�ǂ���:
                    //���G��D��

                    if (condition.highOrLow)
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            if (SManager.instance.targetCondition[i].status.strong)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            if (SManager.instance.targetCondition[i].status.strong)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                //	break;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�G�^�C�v:
                    //   Soldier,//���̎G��
                    if ((0b00011111 & condition.percentage) == 0b00011111)
                    {

                        return true;
                    }
                    //int test;
                    for (int i = 0; i < SManager.instance.targetList.Count; i++)
                    {
                        //test = ;


                        if ((0b00000001 & condition.percentage) == 0b00000001)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Soldier)
                            {
                                return true;
                            }
                        }
                        //test = 0b01000000;
                        else if ((0b00000010 & condition.percentage) == 0b00000010)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Fly)
                            {
                                return true;
                            }
                        }
                        else if ((0b00000100 & condition.percentage) == 0b00000100)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Shooter)
                            {
                                return true;
                            }
                        }
                        else if ((0b00001000 & condition.percentage) == 0b00001000)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Knight)
                            {
                                return true;
                            }
                        }
                        else if ((0b00010000 & condition.percentage) == 0b00010000)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Trap)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�:

                    for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                    {
                        if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
                        {
                            magicCanList.Add(i);
                        }
                    }
                    return magicCanList != null ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case SupportCondition.SupportStatus.�w��Ȃ�:
                    //	�����_���o�����[�g���ă��R�[�h����w��
                    return true;
                    //-----------------------------------------------------------------------------------------------------
            }
            return false;
        }

        public void SupportAct(SupportCondition condition)
        {
            if (sister.nowMove != SisterParameter.MoveType.�x�� || !SManager.instance.supportMagi.Any() || condition.ActBase == SupportCondition.MagicJudge.�Ȃɂ����Ȃ�)
            {
                return;
            }



            if (condition.UseMagic == null)
            {
                magicCanList.Clear();

                if (condition.ActBase == SupportCondition.MagicJudge.�񕜃X�e�[�g��)
                {
                    SisterStateChange((int)condition.ActBase);
                }
                else if (condition.ActBase == SupportCondition.MagicJudge.�U���X�e�[�g��)
                {
                    SisterStateChange((int)condition.ActBase);

                }
                else if (condition.ActBase == SupportCondition.MagicJudge.�e��x�����@)
                {

                    for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                    {
                        //�g�p�������x���̃^�C�v�ɂ������Ȃ�
                        if (SManager.instance.supportMagi[i].sType == condition.useSupport)
                        {
                            magicCanList.Add(i);
                        }
                    }
                }


                if (SManager.instance.supportMagi.Count == 0)
                {
                    return;
                }
                else
                {
                    if (condition.nextCondition == SupportCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        SManager.instance.useMagic = SManager.instance.supportMagi[magicCanList[0]];
                        condition.UseMagic = SManager.instance.supportMagi[magicCanList[0]];
                        magicCanList.Clear();
                    }
                    else
                    {
                        int selectNumber = 150;
                        if (condition.nextCondition == SupportCondition.AdditionalJudge.MP�g�p��)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].useMP > SManager.instance.supportMagi[selectNumber].useMP)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].useMP < SManager.instance.supportMagi[selectNumber].useMP)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == SupportCondition.AdditionalJudge.�r������)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].castTime > SManager.instance.supportMagi[selectNumber].castTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].castTime < SManager.instance.supportMagi[selectNumber].castTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == SupportCondition.AdditionalJudge.�������ʎ���)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].effectTime > SManager.instance.supportMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].effectTime < SManager.instance.supportMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == SupportCondition.AdditionalJudge.�����{��)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].mValue > SManager.instance.supportMagi[selectNumber].mValue)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.supportMagi[magicCanList[i]].mValue < SManager.instance.supportMagi[selectNumber].mValue)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        SManager.instance.useMagic = SManager.instance.supportMagi[selectNumber];

                    }
                }
            }
            else
            {

                SManager.instance.useMagic = condition.UseMagic;
            }
        }

        public bool HealJudge(RecoverCondition condition)
        {

            //Debug.Log($"�񕜔��f{condition.condition}�^�Ȃ��{condition.highOrLow}");

            switch (condition.condition)
            {

                /*	case RecoverCondition.RecoverStatus.�������Ă��Ȃ��x��������:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�:

                    if (condition.highOrLow)
                    {
                        return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������:

                    if (condition.highOrLow)
                    {
                        return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.������MP���K��l�ɒB������:

                    if (condition.highOrLow)
                    {
                        return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
                    }
                    else
                    {
                        return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
                    }
                //return GManager.instance.hp == GManager.instance.maxHp ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������:

                    return GManager.instance.badCondition ? true : false;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.���G�����邩�ǂ���:
                    //���G��D��

                    if (condition.highOrLow)
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            if (SManager.instance.targetCondition[i].status.strong)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < SManager.instance.targetList.Count; i++)
                        {
                            if (SManager.instance.targetCondition[i].status.strong)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                //	break;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�G�^�C�v:
                    //   Soldier,//���̎G��
                    if ((0b00011111 & condition.percentage) == 0b00011111)
                    {
                        return true;
                    }
                    //int test;
                    for (int i = 0; i < SManager.instance.targetList.Count; i++)
                    {
                        //test = ;


                        if ((0b00000001 & condition.percentage) == 0b00000001)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Soldier)
                            {
                                return true;
                            }
                        }
                        //test = 0b01000000;
                        else if ((0b00000010 & condition.percentage) == 0b00000010)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Fly)
                            {
                                return true;
                            }
                        }
                        else if ((0b00000100 & condition.percentage) == 0b00000100)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Shooter)
                            {
                                return true;
                            }
                        }
                        else if ((0b00001000 & condition.percentage) == 0b00001000)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Knight)
                            {
                                return true;
                            }
                        }
                        else if ((0b00010000 & condition.percentage) == 0b00010000)
                        {
                            if (SManager.instance.targetCondition[i].status._charaData._kind == CharacterStatus.KindofEnemy.Trap)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�:

                    for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
                    {
                        if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
                        {
                            return false;
                        }
                    }
                    return true;
                //-----------------------------------------------------------------------------------------------------
                case RecoverCondition.RecoverStatus.�w��Ȃ�:
                    //	�����_���o�����[�g���ă��R�[�h����w��
                    return true;
                    //-----------------------------------------------------------------------------------------------------
            }
            return false;
        }

        public void RecoverAct(RecoverCondition condition)
        {


            if (sister.nowMove != SisterParameter.MoveType.�� && sb.nowState == BrainAbility.SisterState.�킢 || condition.ActBase == RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�)
            {
                //	judgeSequence = 0;
                return;
            }
            //-----------------------------------------------------------------------------------------------------
            else if (condition.ActBase == RecoverCondition.MagicJudge.�x���X�e�[�g��)
            {
                SisterStateChange((int)condition.ActBase);
                return;
            }
            else if (condition.ActBase == RecoverCondition.MagicJudge.�U���X�e�[�g��)
            {
                SisterStateChange((int)condition.ActBase);
                return;
            }



            if (condition.UseMagic == null)
            {
                magicCanList.Clear();


                if (SManager.instance.recoverMagi.Count == 0)
                {

                    SManager.instance.useMagic = null;
                    return;
                }

                if (condition.useSupport != SisMagic.SupportType.�Ȃ�)
                {
                    for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                    {
                        //�w�肷�������ʂ�����Ȃ�
                        if (SManager.instance.recoverMagi[i].sType == condition.useSupport)
                        {
                            magicCanList.Add(i);
                            //Debug.Log($"���ׂ܂�{SManager.instance.recoverMagi[i].name}");
                        }
                    }
                }


                if (!magicCanList.Any())
                {
                    if (condition.nextCondition == RecoverCondition.AdditionalJudge.�w��Ȃ�)
                    {
                        SManager.instance.useMagic = SManager.instance.recoverMagi[magicCanList[0]];

                        condition.UseMagic = SManager.instance.recoverMagi[magicCanList[0]];
                        magicCanList.Clear();

                    }
                    else
                    {
                        int selectNumber = 150;
                        if (condition.nextCondition == RecoverCondition.AdditionalJudge.MP�g�p��)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[magicCanList[i]]].useMP > SManager.instance.recoverMagi[selectNumber].useMP)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].useMP < SManager.instance.recoverMagi[selectNumber].useMP)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�r������)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[magicCanList[i]]].castTime > SManager.instance.recoverMagi[selectNumber].castTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].castTime < SManager.instance.recoverMagi[selectNumber].castTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l�񕜗�)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount > SManager.instance.recoverMagi[selectNumber].regeneAmount)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount < SManager.instance.recoverMagi[selectNumber].regeneAmount)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l���񕜗�)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount * SManager.instance.recoverMagi[magicCanList[i]].effectTime > SManager.instance.recoverMagi[selectNumber].regeneAmount * SManager.instance.recoverMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].regeneAmount * SManager.instance.recoverMagi[magicCanList[i]].effectTime < SManager.instance.recoverMagi[selectNumber].regeneAmount * SManager.instance.recoverMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�������ʎ���)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].effectTime > SManager.instance.recoverMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].effectTime < SManager.instance.recoverMagi[selectNumber].effectTime)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�񕜗�)
                        {
                            if (condition.upDown)
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].recoverBase > SManager.instance.recoverMagi[selectNumber].recoverBase)
                                    {
                                        selectNumber = magicCanList[i];
                                    }

                                }
                            }
                            else
                            {
                                for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                                {
                                    if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].recoverBase < SManager.instance.recoverMagi[selectNumber].recoverBase)
                                    {
                                        selectNumber = magicCanList[i];
                                    }
                                }
                            }
                        }
                        else if (condition.nextCondition == RecoverCondition.AdditionalJudge.��Ԉُ��)
                        {
                            for (int i = 0; i < SManager.instance.recoverMagi.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.recoverMagi[magicCanList[i]].cureCondition)
                                {
                                    selectNumber = magicCanList[i];
                                    break;
                                }

                            }

                        }
                        SManager.instance.useMagic = SManager.instance.recoverMagi[selectNumber];
                        condition.UseMagic = SManager.instance.recoverMagi[selectNumber];
                    }
                }
            }
            else
            {
                SManager.instance.useMagic = condition.UseMagic;
                magicCanList.Clear();
            }

            SManager.instance.target = GManager.instance.Player;

        }

        /// <summary>
        /// ��_�Ԃ̊p�x�����߂�
        /// </summary>
        /// <param name="p1">�����̍��W</param>
        /// <param name="p2">����̍��W</param>
        /// <returns></returns>
        float GetAim(Vector2 p1, Vector2 p2)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            float rad = Mathf.Atan2(dy, dx);
            return rad * Mathf.Rad2Deg;
        }


        /// <summary>
        ///�@���X�g�����A���̒����獇�v��������̓G��I�яo���B
        /// </summary>
        /// <param name="targetList"></param>
        /// <param name="condition"></param>
        /// <param name="SManager.instance.targetCondition"></param>
        GameObject SecondTargetJudge(AttackJudge condition)
        {

            //	Debug.Log($"sddf{targetCanList[0].name}");
            //	Debug.Log($"sdgs{targetList[0].name}");


            if (targetCanList.Count == 0 || targetCanList == null)
            {
                return null;
            }
            else if (targetCanList.Count >= 1)
            {

                if (condition.wp != AttackJudge.WeakPoint.�w��Ȃ�)
                {
                    ///<Summary>
                    ///�������f
                    /// </Summary>
                    #region

                    if ((int)condition.wp < 4)
                    {
                        if (condition.wp != AttackJudge.WeakPoint.�a������)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Slash))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }

                        }
                        else if (condition.wp != AttackJudge.WeakPoint.�h�ˑ���)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Stab))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }

                        }
                        else if (condition.wp != AttackJudge.WeakPoint.�Ō�����)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }
                        }
                    }

                    else
                    {
                        if (condition.wp != AttackJudge.WeakPoint.������)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Fire))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }
                        }
                        else if (condition.wp != AttackJudge.WeakPoint.������)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Thunder))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }
                        }
                        else if (condition.wp != AttackJudge.WeakPoint.������)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Holy))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }
                        }
                        else if (condition.wp != AttackJudge.WeakPoint.�ő���)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (!SManager.instance.targetCondition[targetCanList[i]].status.wp.Contains(EnemyStatus.WeakPoint.Dark))
                                {
                                    targetCanList.Remove(targetCanList[i]);
                                }
                            }
                        }
                    }


                    #endregion

                }

            }

            if (targetCanList.Count == 0 || targetCanList == null)
            {

                return null;

            }

            ///<summary>
            ///�ǉ��������f
            /// </summary>
            else
            {
                int selectNumber = 150;
                if (condition.nextCondition != AttackJudge.AdditionalJudge.�w��Ȃ�)
                {
                    if (condition.nextCondition != AttackJudge.AdditionalJudge.�G��HP)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]]._health.CurrentHealth > SManager.instance.targetCondition[selectNumber]._health.CurrentHealth)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]]._health.CurrentHealth < SManager.instance.targetCondition[selectNumber]._health.CurrentHealth)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̍U����)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status._charaData.displayAtk > SManager.instance.targetCondition[selectNumber].status._charaData.displayAtk)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status._charaData.displayAtk < SManager.instance.targetCondition[selectNumber].status._charaData.displayAtk)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̖h���)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status._charaData.displayDef > SManager.instance.targetCondition[selectNumber].status._charaData.displayDef)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetCondition[targetCanList[i]].status._charaData.displayDef < SManager.instance.targetCondition[selectNumber].status._charaData.displayDef)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̍��x)//�^�Ȃ獂��
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetList[targetCanList[i]].transform.position.y > SManager.instance.targetList[selectNumber].transform.position.y)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.targetList[targetCanList[i]].transform.position.y < SManager.instance.targetList[selectNumber].transform.position.y)
                                {
                                    selectNumber = targetCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̋���)
                    {
                        float distance = 0;
                        if (condition.upDown)
                        {
                            //�߂�
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x) < distance)
                                {
                                    selectNumber = targetCanList[i];
                                    distance = Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x);
                                }
                            }
                        }
                        else
                        {
                            //����
                            for (int i = 0; i < targetCanList.Count; i++)
                            {
                                if (selectNumber == 150 || Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x) > distance)
                                {
                                    selectNumber = targetCanList[i];
                                    distance = Mathf.Abs(SManager.instance.targetList[targetCanList[i]].transform.position.x - this.gameObject.transform.position.x);
                                }
                            }
                        }
                    }

                    if (SManager.instance.target != null)
                    {
                        SManager.instance.targetCondition[selectNumber].TargetEffectCon(2);
                    }
                    return SManager.instance.targetList[selectNumber];
                }
                else
                {
                    if (SManager.instance.target != null)
                    {
                        SManager.instance.targetCondition[0].TargetEffectCon(2);
                    }
                    return SManager.instance.targetList[0];
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="magicList"></param>
        void secondATMagicJudge(FireCondition condition)
        {
            //	Debug.Log($"�������{magicList[0].name}");
            if (magicCanList.Count == 0)
            {
                return;
            }
            else
            {
                //	Debug.Log("�m�F");
                //������
                if (condition.firstCondition != FireCondition.FirstCondition.�w��Ȃ�)
                {

                    if (condition.firstCondition == FireCondition.FirstCondition.�G�𐁂���΂�)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (!SManager.instance.attackMagi[magicCanList[i]].isBlow && !SManager.instance.attackMagi[magicCanList[i]].cBlow)
                            {
                                //Debug.Log("�폜");
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                    }
                    //-----------------------------------------------------------------------------------------------------
                    else if (condition.firstCondition == FireCondition.FirstCondition.�͈͍U��)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (!SManager.instance.attackMagi[magicCanList[i]].isExprode)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�ђʂ���)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (!SManager.instance.attackMagi[magicCanList[i]].penetration)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�ǔ�����)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.ANGLE || SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�ݒu�U��)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.speedV != 0)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�ǔ�����)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.ANGLE || SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�͈͍U��)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (SManager.instance.attackMagi[magicCanList[i]]._moveSt.fireType == Magic.FIREBULLET.RAIN)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                    else if (condition.firstCondition == FireCondition.FirstCondition.�T�[�`�U��)
                    {
                        for (int i = 0; i < magicCanList.Count; i++)
                        {
                            if (!SManager.instance.attackMagi[magicCanList[i]].isChaice)
                            {
                                magicCanList.Remove(magicCanList[i]);
                            }
                        }
                        //break;
                    }
                }
            }

            if (magicCanList.Count == 0)
            {

                return;
            }
            else
            {
                //	Debug.Log("��O�i�K");
                if (condition.nextCondition == FireCondition.AdditionalCondition.�w��Ȃ�)
                {

                    SManager.instance.useMagic = SManager.instance.attackMagi[0];
                    condition.UseMagic = SManager.instance.attackMagi[0];
                    magicCanList.Clear();
                }
                else
                {
                    int selectNumber = 150;
                    if (condition.nextCondition == FireCondition.AdditionalCondition.MP�g�p��)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].useMP > SManager.instance.attackMagi[selectNumber].useMP)
                                {
                                    selectNumber = magicCanList[i];
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].useMP < SManager.instance.attackMagi[selectNumber].useMP)
                                {
                                    selectNumber = magicCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition == FireCondition.AdditionalCondition.�U����)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].displayAtk > SManager.instance.attackMagi[selectNumber].displayAtk)
                                {
                                    selectNumber = magicCanList[i];
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].displayAtk < SManager.instance.attackMagi[selectNumber].displayAtk)
                                {
                                    selectNumber = magicCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition == FireCondition.AdditionalCondition.���ː�)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].bulletNumber > SManager.instance.attackMagi[selectNumber].bulletNumber)
                                {
                                    selectNumber = magicCanList[i];
                                }

                            }

                        }
                        else
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].bulletNumber < SManager.instance.attackMagi[selectNumber].bulletNumber)
                                {
                                    selectNumber = magicCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition == FireCondition.AdditionalCondition.���l)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].shock > SManager.instance.attackMagi[selectNumber].shock)
                                {
                                    selectNumber = magicCanList[i];
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].shock < SManager.instance.attackMagi[selectNumber].shock)
                                {
                                    selectNumber = magicCanList[i];
                                }
                            }
                        }
                    }
                    else if (condition.nextCondition == FireCondition.AdditionalCondition.�r������)
                    {
                        if (condition.upDown)
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].castTime > SManager.instance.attackMagi[selectNumber].castTime)
                                {
                                    selectNumber = magicCanList[i];
                                }

                            }
                        }
                        else
                        {
                            for (int i = 0; i < magicCanList.Count; i++)
                            {
                                if (selectNumber == 150 || SManager.instance.attackMagi[magicCanList[i]].castTime < SManager.instance.attackMagi[selectNumber].castTime)
                                {
                                    selectNumber = magicCanList[i];
                                }
                            }
                        }
                    }

                    SManager.instance.useMagic = SManager.instance.attackMagi[selectNumber];
                    condition.UseMagic = SManager.instance.attackMagi[selectNumber];
                    magicCanList.Clear();
                }
            }
        }


        /// <summary>
        /// �U���X�e�[�g����̃X�e�[�g�ύX
        /// </summary>
        void SisterStateChange(int condition)
        {
            if (condition == 0)
            {
                sister.nowMove = SisterParameter.MoveType.�U��;
            }
            else if(condition == 1)
            {
                sister.nowMove = SisterParameter.MoveType.�x��;
            }
            else
            {
                sister.nowMove = SisterParameter.MoveType.��;
            }
            stateJudge = 0.0f;
            targetJudge = 1000;
            judgeSequence = 0;
            SManager.instance.useMagic = null;
        }



        public bool ActConditionJudge(AttackJudge judgeData,int action)
        {
            
            if (action == 3)
            {
                return false;
            }

            //�O�ȉ��Ȃ�X�e�[�g��ύX����
            bool stateChange = action < 3;


            //�����܂ł������̏���
            int allyCondition = ((int)AttackJudge.ActCondition.�G��HP�䗦�̏����𖞂�����) - 1;

            //�w��Ȃ����ǂ���
            if (judgeData.actCondition == AttackJudge.ActCondition.�w��Ȃ�)
            {

                if (stateChange)
                {
                    SisterStateChange(action);
                    return false;
                }
                return true;
            }

                //�����̏����Ȃ�
                //�U������Ȃ��Ƃ�������⃊�X�g������
                if ((int)judgeData.actCondition <= allyCondition)
            {

                if(judgeData.actCondition == AttackJudge.ActCondition.�v���C���[��HP���䗦�̏����𖞂�����)
                {
                    //�ӂ��ɏ������邵���Ȃ�����
                    if ()
                    {
                        return false;
                    }

                    if (stateChange)
                    {
                        SisterStateChange(action);
                        return false;
                    }

                    //�U�����Ȃ���ɓ����
                    if (sister.nowMove == SisterParameter.MoveType.�U��)
                    {
                        //�����ǂ����悤
                        //
                    }

                    return true;
                }
                else if (judgeData.actCondition == AttackJudge.ActCondition)
                {

                }


            }
            else
            {
                if (stateChange)
                {
                    SisterStateChange(action);
                    return false;
                }

                //�U�����Ȃ���ɓ����
                if (sister.nowMove != SisterParameter.MoveType.�U��)
                {

                }
            }

            return false;
        }


        #endregion


        /// <summary>
        /// �G�����Ȃ��Ƃ��S�ă��Z�b�g
        /// </summary>
        void castCheck()
        {

            if (_movement.CurrentState == CharacterStates.MovementStates.Cast && SManager.instance.target == null)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                waitCast = 0;

                atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                actionNum = 0;

                waitCast = 0;
            }

        }




        async UniTaskVoid DelayInstantiate(ParticleSystem key, Vector3 position, Quaternion rotation, ParticleSystem flash)
        {
            delayNow = true;
            await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime));
            atEf.BulletCall(key, position, rotation, flash);
            delayNow = false;
        }

        /// <summary>
        /// �o�t�̐��l��^����
        /// �e�ۂ���Ă�
        /// </summary>
        public void BuffCalc(FireBullet _fire)
        {
            _fire.attackFactor = attackFactor;
            _fire.fireATFactor = fireATFactor;
            _fire.thunderATFactor = thunderATFactor;
            _fire.darkATFactor = darkATFactor;
            _fire.holyATFactor = holyATFactor;
        }

        ///<summary>
        ///  Brain�Ƃ̘A�g���s��
        /// </summary>
        #region

        ///<sumary>
        /// ��Ԃ��Ƃ̏���������
        /// </sumary>
        public void StateInitialize(bool battle)
        {
            if (battle)
            {
                judgeSequence = 0;
                targetJudge = 1000;
                stateJudge = 1000;

            }
            else
            {
                //����͓����񕜊J�n�Ƃ��̃T�E���h���I��点�Ă�

                if (soundStart)
                {
                    atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
                    soundStart = false;

                }

                stateJudge = 0;
                coolTime = 0;
                disEnable = false;
                _skipCondition = 0;
                if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                }

                isReset = false;
            }
        }

        #endregion

        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_actParameterName, AnimatorControllerParameterType.Int, out _actAnimationParameter);
            RegisterAnimatorParameter(_motionParameterName, AnimatorControllerParameterType.Int, out _motionAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�N���E�`���O�ɋC�������
            //MasicUse��Castnow��g�ݍ��킹�悤��
            int state = 0;
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                state = 2;
            }
            else if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
            {
                state = 1;
            }

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _actAnimationParameter, (state), _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _motionAnimationParameter, (actionNum), _character._animatorParameters);
        }




    }
}