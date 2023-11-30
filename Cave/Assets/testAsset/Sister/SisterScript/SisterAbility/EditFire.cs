using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static CombatManager;
using static SisterConditionBase;

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
/// 
///
///���C�K�v�ȕ���
///�EBrain����Ă΂��C�x���g��ǉ����邩��
///�E�U���̏����ҋ@��ς߂ĂȂ��B�ҋ@�����𔭎˒i�K�Ŏ擾���邽�߂�useNum�͂���ώ����ĂȂ��ƃ_������
///�EMP���f��˒��������f�A�A�[�}�[���f�����܂��l�߂ĂȂ�
///�E�ˌ����\�b�h�̃����_������̎g�p�͏C���K�v
///�E�A�g�A�N�V�����ƒ��ǂ����邽�߂Ɋ撣��Ȃ��ƁB���f�ƍĊJ�̎d�l�ȁ[
///�E�v�C���Ƃ��������Ō���������_���ȂƂ��o�Ă����
///
///

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// ���ʂɏ󋵔��f���ē������߂̃X�N���v�g
    /// ���[�v�A�U���A�R���r�l�[�V�����i�e����͕�����H�j�͕ʃX�N���v�g��
    /// ���邢�̓R���r�l�[�V�����͎�l���Ɏ�������H
    /// ����Ƃ��Ă͔��f�A�r���A�U����ԂɑJ�ڂ����[�V�����ω��A�A�j���[�V�����C�x���g�A���@�g�p�A�A�j���[�V�����C�x���g�ŏI��
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/EditFire")]
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




        /// <summary>
        /// ���̍s���̃N�[���^�C�����L�^
        /// </summary>
        float coolTime;

        /// <summary>
        /// �U���A�񕜁A�Ȃǂ̗D��s�������ւ���
        /// </summary>
        float stateJudge = 30;



        /// <summary>
        /// �^�[�Q�b�g�▂�@�̌����i�[���郊�X�g
        /// </summary>
        List<int> candidateList = new List<int>();




        /// <summary>
        /// ���Ԗڂ̉r���Ȃ̂�
        /// </summary>
        int actionNum;


        [SerializeField] CombinationAbility ca;


        /// <summary>
        /// ���ꂪ�^�Ȃ玟�̋@��ɃX�e�[�g�ύX����
        /// </summary>
        bool stateChange = false;



        //�r�b�g���Z�ŃN�[���^�C����j������
        SisterConditionBase.SkipCondition skipCondition = SkipCondition.�Ȃ�;

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
        /// �g�p���閂�@�̔ԍ����L�^
        /// </summary>
        int useMagicNum;


        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            sister.nowMove = sister.priority;

            //Brain����Ȃ���ł���
            //�C���X�y�N�^�ł��
    //        sb = _character.FindAbility<BrainAbility>();
      //      atEf = _character.FindAbility<AtEffectCon>();

            SManager.instance.MagicEffectSet(atEf);
        }




        //�퓬�J�n����I�����ɌĂ΂�ē������\�b�h
        //��������U���̔��f�Ȃǂ��Ă΂��
        //FireAbillity�͂����܂ōU������Ƃ���Brain�̕⏕�@�\
        //�Ȃ�ł���ȊO�̃C�x���g�͎����Ȃ�
        //Brain���K�v�Ƃ���Ȃ玝���Ƃ����邯��
        //�ł������Brain������ĂԂ���
        #region�@�Ǘ����\�b�h

        ///<sumary>
        /// ��Ԃ��Ƃ̏���������
        /// Brain���퓬�J�n�A�I���̔��f���������Ƃ��Ƃ��ɌĂ΂��
        /// ���Ⴀ�����Ń^�[�Q�b�g���f�J�n�Ƃ��g�[�N���̏������Ƃ�����邩
        /// </sumary>
        public void StateInitialize(bool battle)
        {
            //�퓬�J�n
            if (battle)
            {

                //������~���ăg�[�N������ւ�
                magicCancel.Cancel();
                magicCancel = new CancellationTokenSource();

                //�����񕜂Ȃǂ̉r�����Ȃ�G�t�F�N�g������
                if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
                {
                    atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                }

                //�N�[���^�C���̏I��
                disEnable = false;

                //�D���Ԃɏ�ԕω�
                SisterStateChange((int)sister.priority);
                //�X�e�[�g�Ǘ��J�n
                StateController().Forget();


                //�퓬���f�J�n
                CombatMoveJudge().Forget();
            }
            //��퓬
            //�񕜔��f
            else
            {

                //������~���ăg�[�N������ւ�
                magicCancel.Cancel();
                magicCancel = new CancellationTokenSource();

                //�񕜂ɍs���ω�
                SisterStateChange((int)SisterParameter.MoveType.��);

                //�r�����Ȃ�G�t�F�N�g������
                if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
                {
                    atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                }

                //�ϐ��̏�����
                stateJudge = 0;
                coolTime = 0;
                disEnable = false;

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                }

                //�����񕜔��f�J�n
                HealingJudge().Forget();


            }
        }

        #endregion








        //�V���@�����̗���
        //�S�̓I�ɋ��ʉ����Ĉ�{�̗���ɂȂ��Ă�
        //�퓬�J�n���ɃX�e�[�g�Ǘ����\�b�h�ƃ^�Q�Ɩ��@�ݒ胁�\�b�h���Ăяo��
        //�����Ă܂��^�[�Q�b�g�Ǝg�����@��ݒ肷�郁�\�b�h���Ăяo��
        //���ꂪ�ʂ�����r���A�K�v�Ȃ�ҋ@�A�����Ė��@�g�p
        //�ʂ�Ȃ�������n�b��ɍĔ��f�Ƃ����悤�Ȋ����ɂ��邩�H
        //await n�b�A�^�Ȃ�i�s�A�U�Ȃ�ċA�Ăяo���J��Ԃ�
        //�����ċ��񂾂蒆�f���ꂽ�肵���画�f������await�ŏ�Ԃ����ɖ߂�̂�҂�����H�@await state.normal�I��
        //�N�[���^�C����(disenable)�͍ŏ��̃^�[�Q�b�g�i�荞�ݏ������f�̓��e��ς���
        //����������F�X�`�����悤��
        //���ƕ��펞�̉񕜂͂ǂ�����
        //�������ԑ҂������f���r�����U�����N�[���^�C���҂����f�@�@�̃��[�v
        //�N�[���^�C���Ə������ԑ҂��͌���
        //���͘A�g���n�T�~���񂾎�����





        //���ꂨ���炢
        //�U���\�����m�F���Ă���W�I�Ɩ��@����
        //mp�`�F�b�N�A�r���J�n
        //�U��(�����ɑҋ@�������ނ���)
        //�I����X�e�[�g�`�F���W����
        #region �V�U�����\�b�h


        #region �r���ƍU��

        /// <summary>
        /// �r���Ǘ����\�b�h
        /// �r������������U����Ԃɓ����ă��[�V�����J��
        /// </summary>
        /// <param name="random"></param>
        public async UniTaskVoid CastMagic()
        {
            
            //�����܂ł�����disenable��false�ɂ��đO�̃N�[���^�C���҂����I�����Ă���
            //disen��false�ɂȂ�Ǝ~�܂邩�炳
            if (disEnable)
            {

                disEnable = false;
            }


            //���@�ƃ^�[�Q�b�g�̃f�[�^���l��
            SisMagic useMagic;
            TargetData target;


            //�K�v�ȃf�[�^���擾
              (target,useMagic) = NecessaryDataGet();

            //�g�p���閂�@��MP���Ȃ����A�W�I�����Ȃ��Ȃ�߂�
            if (target.targetObj == null || useMagic.useMP > sb.mp)
            {
                return;
            }

            //���@�w�Ăяo��
            //�r�����ւƏ�ԕω����~�A�U����������Ȃ�
            CastCircleCall(useMagic,target._condition.targetPosition.x);


            //���ԑ҂�
            //�����͍U�������ƒ��f���邩�A����Ƃ��f�ŋF�邩�Ƃ����߂���悤��
            await UniTask.Delay(TimeSpan.FromSeconds(useMagic.castTime), cancellationToken: magicCancel.Token);




            //�r���I��
            atEf.CastEnd(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

            //���@�����Ăяo��
            //�U�����@�͉r����Ԃőҋ@�����������̂�
            //��ԕω��͍U�����\�b�h�ɑ���
            UseMagic(target, useMagic).Forget();
        }


        /// <summary>
        /// ���ۂɍs�����郁�\�b�h
        /// �����ł̓^�[�Q�b�g�m�F�A�ҋ@�A�����_���Ȃǂ��낢��e�̋�������
        /// ���ˁA�����m�F�A�N�[���^�C���J�n�A�X�e�[�g�`�F���W�܂ōs��
        /// 
        /// �����_���z�u����ɂ͎������
        /// </summary>
        /// <param name="hRandom"></param>
        /// <param name="vRandom"></param>
        async UniTaskVoid UseMagic(TargetData target,SisMagic useMagic)
        {



            //(����A�N�V�����Ȃǂ�)�g�p���閂�@��MP���Ȃ����A�W�I�����Ȃ��Ȃ�߂�
            if (target.targetObj == null || useMagic.useMP > sb.mp)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);

                actionNum = 0;
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                return;
            }



            //�N�[���^�C���҂�
            disEnable = true;
            //�U���J�n
            _movement.ChangeState(CharacterStates.MovementStates.Attack);
            //���[�V�����Z�b�g
            actionNum = (int)useMagic.FireType;
            //���͏���
            sb.mp -= useMagic.useMP;





            //���˒n�_�Z�b�g

            Vector3 goFire;

            //�G�̈ʒu�ɃT�[�`�U������Ƃ�
            if (useMagic.isChaice)
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
            //�����F�X��������
            for (int i = 0; i < useMagic.bulletNumber; i++)
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
                        restoreFirePosi.Set(goFire.x, goFire.y, goFire.z);
                    }

                }
                //���ڈȊO�͎ˏo�X�������_�����ǂ����̔��f������
                else
                {

                    //�����_���Ȉʒu�ɔ�������Ƃ�
                    if (hRandom != 0 || vRandom != 0)
                    {
                        goFire.Set(restoreFirePosi.x, restoreFirePosi.y, restoreFirePosi.z);

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
            //��Ԃ����Ƃɖ߂�
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            actionNum = 0;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);


            //�e�ۑS���o���I������烍�b�N�J�[�\�������F�ɖ߂�
            //�����A���|���X�g����i���o�[�Ŕ�΂���
            if (target.targetObj != null && sister.nowMove == SisterParameter.MoveType.�U��)
            {
                target.targetObj.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
            }


            //�����ʒu�m�F
            sb.PositionJudge();


            //��ԕω�����Ȃ�ύX����
            //�N�[���^�C���j���ƃX�e�[�g�`�F���W�̊֌W�ǂ����悤��
            //����ς薂�@�g�p��ɂ��悤
            //����Ȃ��Ə��Ȃ��Ƃ����͍s���ł��Ȃ�
            //���ƌ��ݗD�悷��s����Ԃ̎��͉����ς��Ȃ��̂ł킴�킴�Ăяo���Ȃ�
            if (stateChange && sister.priority != sister.nowMove)
            {
                //�N�[���^�C���L�����Z��
                disEnable = false;

                //��ԕω�
                SisterStateChange((int)sister.priority);
                StateController().Forget();
            }
            //�X�e�[�g�`�F���W���Ȃ��Ȃ�N�[���^�C�����H
            else
            {
                CoolTimeWait(coolTime).Forget();
            }


            //���@�g�p�㌻�݂̗����ʒu���Ԉ���ĂȂ����𔻒�
            //���������Ɨ����ʒu�����Ă�Ɣ��f�ŗ��Ă���^�[�Q�b�g���f�Ƃ��Ăڂ��Bawait����
            sb.PositionJudge();

            //�N�[���^�C���Ăяo���A���邢�̓X�e�[�g�`�F���W�����画�f���\�b�h���Ăяo��
            CombatMoveJudge().Forget();

        }


        #endregion



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
       async UniTaskVoid CoolTimeWait(float coolTime)
        {



            //���ԑ҂�
            //�Œ�ł�1�b
            //���ꂩdisenable���������ꂽ��i��
            await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(coolTime + 1),cancellationToken:magicCancel.Token),
                UniTask.WaitUntil(()=> !disEnable,cancellationToken:magicCancel.Token));

            //���ł�disenable��������Ă�Ȃ�]�v�Ȃ��Ƃ����߂�
            if (!disEnable)
            {
                return;
            }



                    disEnable = false;
                    skipCondition= SkipCondition.�Ȃ�;
          



        }


        /// <summary>
        /// �r�����[�V�����̃A�j���C�x���g
        /// �r�����̂��߂ɉ��ƃG�t�F�N�g���Z�b�g
        /// �g�p���閂�@�������ɂ��Ė��@���x���Ƒ����Ō���
        /// �r���̉���G�t�F�N�g�ς������Ȃ炱���ł�����
        public void CastCircleCall(Magic useMagic,float targetPosition)
        {

            //�r���J�n�A�~�܂�
            _controller.SetHorizontalForce(0);

            //�r���̂��߂̃��[�V�������l���Ƃ�
            actionNum = (int)useMagic.castType;

            //��Ԃ��ω�������
            _movement.ChangeState(CharacterStates.MovementStates.Cast);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            //�G�t�F�N�g�̌Ăяo��
            //�����o��
            atEf.CastStart(useMagic.magicLevel, useMagic.magicElement);

            //�^�[�Q�b�g�̕���������
            sb.SisFlip(Mathf.Sign(targetPosition - transform.position.x));

        }





        /// <summary>
        /// �����ƃ^�[�Q�b�g�Ɩ��@�̃f�[�^�����݂��邩�ǂ�����₢���킹��
        /// �����ĕԂ�
        /// </summary>
        /// <returns></returns>
        (TargetData,SisMagic) NecessaryDataGet()
        {
            if(sister.nowMove == SisterParameter.MoveType.�U��)
            {
                return (SManager.instance._targetList[targetNum], SManager.instance.attackMagi[useMagicNum]); 
            }

            else if(sister.nowMove == SisterParameter.MoveType.�x��)
            {
                return (EnemyManager.instance._targetList[targetNum],SManager.instance.supportMagi[useMagicNum]);
            }
            else
            {
                return (EnemyManager.instance._targetList[targetNum], SManager.instance.recoverMagi[useMagicNum]);
            }

        }



        #endregion



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












        ///<summary>
        ///�^�[�Q�b�g���f�ɗ��p
        /// </summary>
        #region�@AI�̔��f

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





        //������邱��
        //CanList�n���͑S����ɂ܂Ƃ߂�ꂻ��
        //�����ă^�[�Q�b�g�ݒ聨���@���f�Ŗ��m�ɗ��ꂠ�邵�H
        //MP�`�F�b�N�Ǝ˒��`�F�b�N�ł��ĂȂ�



        //���P�_
        //CanList�n���͑S����ɂ܂Ƃ߂�ꂻ��
        //MP�`�F�b�N�Ǝ˒��`�F�b�N�ł��ĂȂ�
        //�����ă^�[�Q�b�g�ݒ聨���@���f�Ŗ��m�ɗ��ꂠ�邵�H
        //���ꂩ���@���f�ƃ^�[�Q�b�g�ݒ�͕ʁX�ɂ��H
        //�Ȃ��Ȃ�
        #region �V���f�p���\�b�h







        #region�@�X�e�[�g�Ǘ�����

        /// <summary>
        /// �X�e�[�g�ύX
        /// �N�[���^�C���ƃX�e�[�g�����͍U�������ŌĂ΂��̂�
        /// �N�[���^�C���҂����ɃX�e�[�g���Ăяo����邱�Ƃ͂Ȃ�
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
            
            //�X�e�[�g�ω��̎��Ԃ��ĕύX
            stateJudge = GManager.instance.nowTime;
            //�N�[���^�C��������Ȃ�
            disEnable = false;
        }

        /// <summary>
        /// �V�X�^�[����̃X�e�[�g��؂�ւ���@�\
        /// ���߂��b�����Ƃɐ؂�ւ�
        /// ��Ԑ؂�ւ��Ȃǂőҋ@�b�����Z�b�g
        /// 
        /// ���Ⴀ�ϐ��Ƃ̍���҂����邩
        /// 
        /// </summary>
        async UniTaskVoid StateController()
        {
            //���ݎ��Ԃ���ҋ@�J�n���Ԃ����������Ԃ��ύX���Ԃ𒴂�����
            //�X�e�[�g�ω��t���O���ĂčU���̏I��肩�͂��܂�ł������邩�H
            await UniTask.WaitUntil(()=> (GManager.instance.nowTime - stateJudge) >= sister.stateResetRes);

            stateChange = true;
        }

        #endregion

        #region�@AI���f�����̊Ǘ�


        /// <summary>
        /// �퓬AI�̖{�́B����Ń^�[�Q�b�g�ݒ�Ɩ��@�I�����s��
        /// ���胁�\�b�h�̕Ԃ�l�͏����ɍ����̂����݂��邩��bool
        /// �����čŌ�ɗD�揇�ʂ����߂�̂�int
        /// 
        /// �^�[�Q�b�g�Ǝg�p���@��int�ŕێ�����
        /// </summary>
        async UniTaskVoid CombatMoveJudge()
        {

            //�N�[���^�C�����ŃX�L�b�v�������Ȃ��Ȃ�N�[���^�C���I���܂ő҂Ƃ���
            if(disEnable && (int)skipCondition == 0)
            {
                await UniTask.WaitUntil(()=> disEnable,cancellationToken:magicCancel.Token);
            }

            //�����ʏ��Ԃ���Ȃ��Ȃ炻��܂Ŗ��@�J�n��҂�
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal, cancellationToken: magicCancel.Token);
            }


            int useNum;


            //�ԍ��l��
            useNum = FirstCheck();

            if(useNum == 99)
            {
                //�X�L�b�v�����������ăN�[���^�C�����Ȃ�ċA�Ăяo��
                if (disEnable)
                {
                    //��b�҂��čċA�Ăяo��
                    //�܂��������f�����
                    await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken:magicCancel.Token);


                    CombatMoveJudge().Forget();
                }

                return;
            }

            //���f�Ɏg���f�[�^
            SisterConditionBase useCondition;


            //�U���񕜎x���Ŏg���f�[�^�𕪂���
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                useCondition = sister.targetCondition[useNum];
            }
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {
               useCondition = sister.supportPlan[useNum];
            }
            else
            {
                useCondition = sister.recoverCondition[useNum];
            }

            //�������Ȃ��Ȃ�߂�
            //�N�[���^�C������ꍇ�͕��򂵂Ă���������
            if(useCondition.selectAction == UseAction.�Ȃɂ����Ȃ�)
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }


            //�^�[�Q�b�g�̍i�荞�݂𑱂���
            //�^�[�Q�b�g�̔ԍ�������o��
            targetNum = TargetSelectStart(useCondition.selectCondition);
            //���҃��X�g���N���[���i�b�v
            candidateList.Clear();

            //�������疂�@���f
            //�����ɓ��Ă͂܂閂�@���Ȃ��Ȃ�߂�
            if (!MagicJudge(useCondition.bulletCondition))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }

            //��������U���x���񕜂��Ƃɏ������킩���
            //���Ă͂܂�Ȃ�������߂�
            if(!StateSpecificCheck(useNum))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }


            //������MP�Ǝ˒��������`�F�b�N����
            if (!NecessaryCheck(useCondition.mpCheck, useCondition.rangeCheck))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }


            

            //�^�[�Q�b�g�ɗD�揇�ʂ����đI������
            useMagicNum = MagicSelect(useCondition.magicSort,useCondition.bulletASOder);
            //���҃��X�g���N���[���i�b�v
            candidateList.Clear();



            //�N�[���^�C���ݒ�
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                coolTime = sister.targetCondition[useNum].coolTime;
            }
            else if(sister.nowMove == SisterParameter.MoveType.�x��)
            {
                coolTime = sister.supportPlan[useNum].coolTime;
            }
            else
            {
                coolTime = sister.recoverCondition[useNum].coolTime;
            }

            //���@�r���J�n
            CastMagic().Forget();

        }


        #region �ŏ��ɌĂ΂��s���ł��邩�𔻒f����R�[�h

        /// <summary>
        /// �ŏ��ɍs�������𖞂����Ă���̂����m�F����
        /// Disenable���� �X�L�b�v���������Ēl��Ԃ�
        /// </summary>
        /// <returns></returns>
        int FirstCheck()
        {

            if (!disEnable)
            {
                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    return NormalCheckStart(sister.targetCondition);
                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    return NormalCheckStart(sister.supportPlan);
                }
                else
                {
                    return NormalCheckStart(sister.recoverCondition);
                }
            }
            else
            {
                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    return SkipCheckStart(sister.targetCondition);
                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    return SkipCheckStart(sister.supportPlan);
                }
                else
                {
                    return SkipCheckStart(sister.recoverCondition);
                }
            }

        }


        /// <summary>
        /// �s�����������Ă͂܂邩�̋��ʏ���
        /// �X�e�[�g�Ɋ֌W�Ȃ�����ō��s���ł��邩�������
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int NormalCheckStart(SisterConditionBase[] condition)
        {
            int count = condition.Length;

            //�܂��͂��ꂼ���
            for (int i = 0;i<count;i++)
            {
                //�s�������ƍs����n����
                if (ActConditionJudge(condition[i].judgeCondition, condition[i].selectAction))
                {
                    return i;
                }
            }
            //�������Ă͂܂�Ȃ��Ȃ�99
            return 99;
        }


        int SkipCheckStart(SisterConditionBase[] condition)
        {
            int count = condition.Length;


            //�ܔԖڂ܂ł��
            //�ܔԖڂ܂ł����璷������1�����Ă�
            //�X�L�b�v�R���f�B�V���������鏈��
            for (int i = 0; i < count; i++)
            {

                //0��͂P
                //�N�[���^�C�����ŁA�Ȃ����X�L�b�v�R���f�B�V�����ɓ��Ă͂܂�Ȃ��Ȃ珈�����΂��B
                //�V�t�g���Z�H
                if (((int)skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                {
                    continue;
                }

                //�s�������ƍs����n����
                if (ActConditionJudge(condition[i].judgeCondition, condition[i].selectAction))
                {
                    return i;
                }

            }
            return 99;
        }

        #endregion



        /// <summary>
        /// �X�e�[�g���Ƃɓ��������`�F�b�N���s��
        /// </summary>
        /// <returns></returns>
        bool StateSpecificCheck(int useNum)
        {

            //�U���ł͑�������
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                //�������f�̌��ʂ�Ԃ�
                return ElementJudge(sister.targetCondition[useNum].useElement);
            }

            //�T�|�[�g�ł̓T�|�[�g���ʂƑ����őI��
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {
                SupportCondition useCondition = sister.supportPlan[useNum];

                //���������Ă͂܂�Ȃ��Ȃ�false��Ԃ�
                if (!SupportConditionJudge(useCondition.secondActCondition))
                {
                    return false;
                }

                //�������f�̌��ʂ�Ԃ�
                return ElementJudge(useCondition.useElement);

            }
            //�񕜂ł̓q�[���̌��ʂƃT�|�[�g���ʂ̂��܂��őI��
            else
            {
                RecoverCondition useCondition = sister.recoverCondition[useNum];

                //���������Ă͂܂�Ȃ��Ȃ�false��Ԃ�
                if (!HealConditionJudge(useCondition.secondActJudge))
                {
                    return false;
                }

                //���������Ă͂܂�Ȃ��Ȃ�false��Ԃ�
                return SupportConditionJudge(useCondition.healSupport);
            }
        }


        /// <summary>
        /// �����ŕK�v�ȃ`�F�b�N������
        /// �˒�������MP�𒲂ׂĖ��Ȃ��Ȃ�^��Ԃ�
        /// 
        /// ����̖ړI�͍œK���@�̎˒��͈͊O�Ƀ^�[�Q�b�g��������mp������Ȃ��Ƃ���
        /// �œK�ł͂Ȃ�������𖞂������@�����ɑI�Ԃ���
        /// </summary>
        /// <param name="mpCheck"></param>
        /// <param name="RangeCheck"></param>
        /// <returns></returns>
        bool NecessaryCheck(bool mpCheck,bool RangeCheck)
        {
            //�ǂ��������Ȃ��Ȃ�^��Ԃ�
            if(!mpCheck && !RangeCheck)
            {
                return true;
            }

            int magicCount = candidateList.Count;




            //����ۂȂ烊�X�g�̐���������
            if (magicCount == 0)
            {

                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.attackMagi.Count;

                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //���@�̐�������
                    magicCount = SManager.instance.recoverMagi.Count;
                }

                //����͉��Ԗڂ̖��@���g�����̔��f��
                //CanList����������K�v�ȏ���
                for (int i = 0; i < magicCount; i++)
                {
                    //�^�[�Q�b�g�̐�����
                    //���X�g�Ɍ���ǉ�
                    candidateList.Add(i);
                }

            }

            Magic checkMagic;

            //���������߂�
            float distance;
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                distance = Vector2.Distance(transform.position,SManager.instance._targetList[targetNum]._condition.targetPosition);

            }
            else 
            {
                distance = Vector2.Distance(transform.position, EnemyManager.instance._targetList[targetNum]._condition.targetPosition);
            }


            //�`�F�b�N�J�n
            for (int i = 0; i < magicCount; i++)
            {
                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    checkMagic = SManager.instance.attackMagi[candidateList[i]];

                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    checkMagic = SManager.instance.supportMagi[candidateList[i]];
                }
                else
                {
                    checkMagic = SManager.instance.recoverMagi[candidateList[i]];
                }

                //��������
                if (RangeCheck)
                {
                    //�T�[�`���@�ɂ��Ă��ǐՌ��E�����݂����Ȃ̂͂���͂�
                    //�Ȃ񂩎˒����킩��悤�Ȃ̐ς�ǂ���
                    //if()���ē��Ă͂܂�Ȃ�mp���ׂ��ɃR���e�B�j���[
                }

                if (mpCheck)
                {
                    //���͂������Ă�Ȃ�
                    if(checkMagic.useMP > sb.mp)
                    {
                        //i�Ԗڂ̗v�f��r��
                        candidateList.RemoveAt(i);
                        
                    }
                }

            }


                //�`�F�b�N��ɗv�f���c���Ă�ΐ^
                return candidateList.Any();
        }


        #endregion

        #region �^�[�Q�b�g�ݒ�Ɏg�p

        //��ꔻ�f�̑O�ɃN�[���^�C���[���ŁA�Ȃ����������Ȃ����s�����j�Ȃ画�f�͂��Ȃ��悤�ɂ���
        //�������̊֐��A�S�̂̊Ǘ��֐��ł��
        //�X�e�[�g�`�F���W���s���̎��̓N�[���^�C���g���Ȃ�
        //�܂��A�������Ȃ����͑�ꔻ�f���^�Ȃ��񔻒f�͂��Ȃ��ł��̂܂܉������Ȃ����[�h��
        //���Ƒ�ꔻ�f�ɂ����āA�s������ԕω��≽�����Ȃ��̎��͖��ʂȏ��������Ȃ��悤�ɂ��Ă���B����͎����ς�
        #region ��ꔻ�f



        /// <summary>
        /// �s�����N�������ǂ����̔��f�B
        /// ������CanList���g���ă^�[�Q�b�g�̍i�荞�݂�
        /// 
        /// �S�X�e�[�g�ŋ��ʂ��Ďg����
        /// �����Ń^�[�Q�b�g�i�荞���canList��1�ȏ�Ȃ��i�K�ڂ̏����ōi�荞��
        /// �t�Ƀ��X�g���[���Ȃ�S�����Ɋ܂�
        /// �܂��A�񕜂�x���ł̓V�X�^�[�����v���C���[�Ȃǂ̒��ڎw����i�荞�݂Ɋ܂�
        /// 
        /// </summary>
        /// <param name="judgeData"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool ActConditionJudge(ActJudgeCondition[] judgeData, UseAction action)
        {

            //�O�����Ȃ�X�e�[�g��ύX����
            bool stateChange = (int)action < 3;

            //�����Ȃ����̃t���O
            bool noCondition = true;

            for(int i = 0;i < 3; i++)
            {
                //�ǂꂩ��ł��w��Ȃ�����Ȃ��Ȃ��������
                if (judgeData[i].actCondition != ActCondition.�w��Ȃ�)
                {
                    noCondition = false;
                    break;
                }
            }


            //�w��Ȃ��Ȃ画�f�����Ԃ�
            if (noCondition)
            {
                //��ԕς���Ȃ�ς��ċU��Ԃ�
                if (stateChange)
                {
                    SisterStateChange((int)action);
                    return false;
                }
                return true;
            }
            else
            {
                

                //�������Ă͂܂�Ȃ��ԕω����s�������s
                if (JudgeStart(judgeData, sister.nowMove == SisterParameter.MoveType.�U��, action))
                {

                    //��ԕς���Ȃ�ς��ċU��Ԃ�
                    if (stateChange)
                    {
                        SisterStateChange((int)action);
                        return false;
                    }
                    return true;
                }

                //�������Ă͂܂��Ȃ�false
                return false;
            }

        }


        /// <summary>
        /// �s�����f�̎��s����
        /// �����𐮗����ċ��ʏ����ɗ��Ƃ����ޏ���������
        /// �^�[�Q�b�g�̋L�^�Ȃǂ͂����ōs��
        /// Exe()�ŋ�̓I�Ȕ�r���s�����߂ɏ������H����
        /// �܂��A��ԕω��≽�����Ȃ����s���ɑI��ł���ꍇ�͖��ʂȏ������Ȃ��悤�ɂ��Ă���
        /// </summary>
        /// <returns></returns>
        public bool JudgeStart(ActJudgeCondition[] condition,bool isAttack, UseAction action)
        {
            ///�L�^�K�v���ǂ���
            bool needRecord;

            bool isResult = false;


            //�O�̏����͂����Ń��[�v������
            //�x���A�񕜂̏ꍇ�ƍU���̏ꍇ��and��or�̏������ς��
            //�x���A�񕜂̏ꍇ��and�͐�΂ɓ��Ă͂܂�Ȃ��Ƃ����Ȃ������B���ꂪ�O���ƍs���ł��Ȃ�
            //or�͓��Ă͂܂�Ȃ��Ă������A������ƗD��W�I�Ώۂ�������B�ł�or�O�S���O���ƍs���ł��Ȃ�

            //�U���̏ꍇ��and�͐�΂ɊO���Ȃ������B�����ē�����ƗD��W�I��������B�O���ƗD��W�I������
            //or�͓��ĂȂ��Ă��������Ǔ�����Ɠ��������������D��W�I��������B�ł�or�O�S���O���ƍs���ł��Ȃ�


            for (int i = 0; i < 3; i++)
            {

                if (condition[i].range == CheckRange.Player)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);


                    //�񕜂�x���ł�Or�����̎������ǉ�����
                    if (JudgeExe(SManager.instance._targetList[0], condition[i]))
                    {

                        //�^�[�Q�b�g���L�^
                        if (needRecord)
                        {


                            if (condition[i].rule == JudgeRuLe.or����)
                            {
                                //�v���C���[��ǉ�
                                //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                                //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                                candidateList.Add(0);
                            }

                        }

                        //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                        //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                        isResult = true;

                    }
                    //and�����~�X������A�E�g
                    else if (condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.Sister)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�񕜂�x���ł�Or�����̎������ǉ�����
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);

                    if (JudgeExe(SManager.instance._targetList[1], condition[i]))
                    {



                        //�^�[�Q�b�g���L�^
                        if (needRecord)
                        {



                            //�V�X�^�[�����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(1);


                        }

                        //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                        //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                        isResult = true;
                    }

                    //and�����~�X������A�E�g
                    else if (condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.OtherAlly)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�܂��AOr����Ȃ��Ȃ�L�^�͂��Ȃ�
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);

                    //�G�l�~�[�}�l�[�W���[���^�[�Q�b�g�ŃV�X�^�[���񑤂������Ă�
                    int count = EnemyManager.instance._targetList.Count;

                    //�V�X�^�[����ƃv���C���[�ȊO�ɂ��Ȃ��Ȃ画�f���Ȃ�
                    if (count < 3)
                    {
                        //and�����Ȃ�s�K��
                        if (condition[i].rule == JudgeRuLe.and����)
                        {
                            return false;
                        }

                        continue;
                    }

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 2; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;


                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);




                        }


                    }

                    //and�����~�X������A�E�g�BisMatch��false�Ȃ炢�Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }


                }
                else if (condition[i].range == CheckRange.Ally)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�܂��AOr����Ȃ��Ȃ�L�^�͂��Ȃ�
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);


                    //�G�l�~�[�}�l�[�W���[���^�[�Q�b�g�ŃV�X�^�[���񑤂������Ă�
                    int count = EnemyManager.instance._targetList.Count;

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;

                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);


                        }


                    }


                    //and�����~�X������A�E�g�BisMatch��false�Ȃ瓖�Ă͂܂�̂����Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }
                }
                //�G
                else
                {
                    //��ԕω����T���Ă��Ȃ��čU�����Ȃ�L�^����
                    needRecord = isAttack && action == UseAction.���݂̐ݒ�ōs��;

                    //S�}�l�[�W���[���^�[�Q�b�g�œG���������Ă�
                    int count = SManager.instance._targetList.Count;

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(SManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;


                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);




                        }
                        //and�����̎��A���Ă͂܂�Ȃ������v�f���܂ނȂ炯���Ă���
                        else if (condition[i].rule == JudgeRuLe.and���� && needRecord)
                        {
                            //�܂�łȂ��v�f��remove���Ă��G���[����Ȃ��̂�
                        //    if (candidateList.Contains(s))
                        //    {
                                candidateList.Remove(s);
                        //    }
                        }

                    }

                    //and�����~�X������A�E�g�BisMatch��false�Ȃ炢�Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }

                }
            }


            //and�����~�X���false�Ԃ��悤�ɂȂ��Ă�
            //Or���������ŁA���Ă͂܂���̂�����Ȃ������ꍇ��isResult��false�ɂȂ�
            //����Ă����܂ŗ��āA�Ȃ�����isResult���^�Ȃ�^
            return isResult;
        }



        /// <summary>
        /// �Ȃ��悤�ҏW�p
        /// 
        /// 
        /// �s�����f�̎��s����
        /// �����𐮗����ċ��ʏ����ɗ��Ƃ����ޏ���������
        /// �^�[�Q�b�g�̋L�^�Ȃǂ͂����ōs��
        /// </summary>
        /// <returns></returns>
        public bool EditJudge(ActJudgeCondition[] condition, bool isAttack, UseAction action)
        {

            ///�L�^�K�v���ǂ���
            bool needRecord;

            bool isResult = false;

            //�O�̏����͂����Ń��[�v������
            //������And�����̏ꍇ�͓��Ă͂܂�Ȃ��z�������Aor�����̏ꍇ�͓��Ă͂܂�����𑫂�
            //����̌�������������ꍇ�͎��O�ɕ��ёւ�
            //�O�Ƃ������Ȃ�ӂ���and��or�ɕϊ�
            //���ꂩexe�Ɉ�C�ɎO�̏�����n���Ă�点�邩�H
            //�ł�condition.Range�Ȃǂ̏������K�����������ɂȂ�Ƃ͌���Ȃ�
            //�����ł�邵���Ȃ�
            //���̏�����ς���̂͂���

            for (int i = 0; i < 3; i++)
            {

                if (condition[i].range == CheckRange.Player)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);


                    //�񕜂�x���ł�Or�����̎������ǉ�����
                    if (JudgeExe(SManager.instance._targetList[0], condition[i]))
                    {

                        //�^�[�Q�b�g���L�^
                        if (needRecord)
                        {


                            if (condition[i].rule == JudgeRuLe.or����)
                            {
                            //�v���C���[��ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(0);
                            }

                        }

                        //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                        //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                        isResult = true;

                    }
                    //and�����~�X������A�E�g
                    else if (condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.Sister)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�񕜂�x���ł�Or�����̎������ǉ�����
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��)&&(condition[i].rule == JudgeRuLe.or����);

                    if (JudgeExe(SManager.instance._targetList[1], condition[i]))
                    {
                        


                            //�^�[�Q�b�g���L�^
                            if (needRecord)
                            {



                                    //�V�X�^�[�����ǉ�
                                    //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                                    //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                                    candidateList.Add(1);


                            }

                        //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                        //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                        isResult = true;
                    }

                        //and�����~�X������A�E�g
                        else if (condition[i].rule == JudgeRuLe.and����)
                        {
                            return false;
                        }

                }
                else if (condition[i].range == CheckRange.OtherAlly)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�܂��AOr����Ȃ��Ȃ�L�^�͂��Ȃ�
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);

                    //�G�l�~�[�}�l�[�W���[���^�[�Q�b�g�ŃV�X�^�[���񑤂������Ă�
                    int count = EnemyManager.instance._targetList.Count;

                    //�V�X�^�[����ƃv���C���[�ȊO�ɂ��Ȃ��Ȃ画�f���Ȃ�
                    if (count < 3)
                    {
                        //and�����Ȃ�s�K��
                        if (condition[i].rule == JudgeRuLe.and����)
                        {
                            return false;
                        }

                        continue;
                    }

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 2; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;


                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);




                        }


                    }

                    //and�����~�X������A�E�g�BisMatch��false�Ȃ炢�Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }


                }
                else if (condition[i].range == CheckRange.Ally)
                {
                    //��ԕω����T���Ă��Ȃ��čU�����łȂ���΋L�^����
                    //�܂��AOr����Ȃ��Ȃ�L�^�͂��Ȃ�
                    needRecord = (!isAttack && action == UseAction.���݂̐ݒ�ōs��) && (condition[i].rule == JudgeRuLe.or����);


                    //�G�l�~�[�}�l�[�W���[���^�[�Q�b�g�ŃV�X�^�[���񑤂������Ă�
                    int count = EnemyManager.instance._targetList.Count;

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;

                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);


                        }


                    }


                    //and�����~�X������A�E�g�BisMatch��false�Ȃ瓖�Ă͂܂�̂����Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }
                }
                //�G
                else
                {
                    //��ԕω����T���Ă��Ȃ��čU�����Ȃ�L�^����
                    needRecord = isAttack && action == UseAction.���݂̐ݒ�ōs��;

                    //S�}�l�[�W���[���^�[�Q�b�g�œG���������Ă�
                    int count = SManager.instance._targetList.Count;

                    //���Ă͂܂�̂��������̃t���O
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //�ЂƂЂƂ������f���Ă���
                        //���������Ȃ�L�^
                        if (JudgeExe(SManager.instance._targetList[s], condition[i]))
                        {
                            //�̂������Ȃ�}�b�`��^��
                            isMatch = true;


                            //��ł����Ă͂܂��������Ȃ猋�ʂ͐^�ɂȂ�
                            //���������̌�and�������O�����猋�ʂ�false�ɂȂ�
                            isResult = true;

                            //�L�^����Ȃ��Ȃ炢�����_�ŏI���
                            if (!needRecord)
                            {
                                break;
                            }

                            //����ȊO�Ȃ�L�^
                            //�񕜂�x���ł�Or�����̎������ǉ�����


                            //���Ă͂܂閡����ǉ�
                            //�V�X�^�[�����MP���`�̎��A���v���C���[��HP��50���ȏ�̎��A�Ƃ������邩��
                            //�񕜂�x���ł͒��ڎw��̍i�荞�݂����Ă�邩
                            candidateList.Add(s);




                        }
                        //and�����̎��A���Ă͂܂�Ȃ������v�f���܂ނȂ炯���Ă���
                        else if (condition[i].rule == JudgeRuLe.and���� && needRecord)
                        {
                            if (candidateList.Contains(s))
                            {
                                candidateList.Remove(s);
                            }
                        }

                    }

                    //and�����~�X������A�E�g�BisMatch��false�Ȃ炢�Ȃ�����
                    if (!isMatch && condition[i].rule == JudgeRuLe.and����)
                    {
                        return false;
                    }

                }
            }


            //and�����~�X���false�Ԃ��悤�ɂȂ��Ă�
            //Or���������ŁA���Ă͂܂���̂�����Ȃ������ꍇ��isResult��false�ɂȂ�
            //����Ă����܂ŗ��āA�Ȃ�����isResult���^�Ȃ�^
            return isResult;

        }



        /// <summary>
        /// �L�����f�[�^�ɑ΂��ċ��ʏ����ɂ��^�U��Ԃ�
        /// ��̈�̂̓G�������Ŕ��f����
        /// </summary>
        /// <returns></returns>
        bool JudgeExe(TargetData data, ActJudgeCondition condition)
        {

            //�����Ŏ˒������m�F���Ȃ�
            //�U���s���I���̎��ɁA�s���I��ł��炻�̍s���Ń^�[�Q�b�g���˒��ɂ��邩���m�F����
            //       if (rangecheck)
            //          {
            ///           return false;
            //   }

            if (condition.actCondition == ActCondition.�w��Ȃ�)
            {
                return true;
            }

            //�܂āA�������Ƃ����낢�날����


            if ((int)condition.content < 3)
            {

                if (condition.content == CheckContent.HP)
                {
                    //�����Ȃ�I�b�P�[
                    if (data._condition.hpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //�������ȉ��Ȃ�HighLow���U�Ȃ�^�ɂȂ�
                    //�ȏ�Ȃ�^���Ɛ^�ɂȂ�
                    return (data._condition.hpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;

                }
                else if (condition.content == CheckContent.MP)
                {
                    //�����Ȃ�I�b�P�[
                    if (data._condition.mpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //�������ȉ��Ȃ�HighLow���U�Ȃ�^�ɂȂ�
                    //�ȏ�Ȃ�^���Ɛ^�ɂȂ�
                    return (data._condition.mpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }
                else if (condition.content == CheckContent.type)
                {
                    //�^�C�v�͈�v���Ă�H
                    return Convert.ToBoolean((int)data._baseData._type & condition.percentage);
                }
            }
            else if ((int)condition.content < 6)
            {
                if (condition.content == CheckContent.strength)
                {
                    //���G���ǂ����Ƌ��G�����߂邩�ǂ�������v���Ă�Ȃ�
                    //���G����Ȃ��Ƃ��Ƌ��G���߂鎞�ǂ���ɂ���v����
                    return data._baseData.isStrong == condition.highOrLow;
                }
                else if (condition.content == CheckContent.posiCondition)
                {
                    //�o�t���܂�ł��邩
                    return Convert.ToBoolean((int)data._condition.buffImfo & condition.percentage);
                }
                else if (condition.content == CheckContent.negaCondition)
                {
                    //�f�o�t���܂�ł��邩
                    return Convert.ToBoolean((int)data._condition.buffImfo & condition.percentage);
                }

            }
            else if ((int)condition.content < 9)
            {
                if (condition.content == CheckContent.weakPoint)
                {
                    return Convert.ToBoolean((int)data._baseData.WeakPoint & condition.percentage);
                }
                else if (condition.content == CheckContent.distance)
                {

                    //�����L���b�V���ɒu���ς����ق��
                    float distance = Vector2.Distance(data._condition.targetPosition, transform.position);

                    //�����Ȃ�I�b�P�[
                    if (distance == condition.percentage)
                    {
                        return true;
                    }

                    //�������ȉ��Ȃ�HighLow���U�Ȃ�^�ɂȂ�
                    //�ȏ�Ȃ�^���Ɛ^�ɂȂ�
                    return (distance <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }
                else if (condition.content == CheckContent.armor)
                {
                    //�����Ȃ�I�b�P�[
                    if (data._condition.hpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //�������ȉ��Ȃ�HighLow���U�Ȃ�^�ɂȂ�
                    //�ȏ�Ȃ�^���Ɛ^�ɂȂ�
                    return (data._condition.hpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }

            }

            else// if ((int)condition.content < 12)  //����ɒǉ����ꂽ�炱�̔��f����
            {
                if (condition.content == CheckContent.playerHate)
                {
                    if (data._condition.target.targetSide != ControllAbillity.Side.Player)
                    {
                        return false;
                    }

                    return data._condition.target.targetNum == 0;
                }
                else if (condition.content == CheckContent.sisterHate)
                {
                    if (data._condition.target.targetSide != ControllAbillity.Side.Player)
                    {
                        return false;
                    }

                    return data._condition.target.targetNum == 1;
                }
                else if (condition.content == CheckContent.otherHate)
                {
                    if (data._condition.target.targetSide != ControllAbillity.Side.Player)
                    {
                        return false;
                    }

                    //1�ȏ�Ȃ�V�X�^�[����ƃv���C���[����Ȃ�
                    return data._condition.target.targetNum > 1;
                }
            }

            return false;
        }






        #endregion





        //�i�荞�񂾃^�[�Q�b�g���\�[�g����
        //�����܂ŏ��������Ă�Ƃ������Ƃ̓^�[�Q�b�g�����邩�̔��f��false����Ȃ��������Ă���
        //�Ȃ��canlist������ۂȂ�S��
        #region ��񔻒f


        /// <summary>
        /// �^�[�Q�b�g��I������
        /// ��₩������Ɉ�ԓ��Ă͂܂�̂��^�[�Q�b�g�ɂ��邾��
        /// ���ёւ��͂���Ȃ���
        /// </summary>
        /// <param name="sortCondition"></param>
        int TargetSelectStart(TargetSelectCondition condition)
        {
            //�^�[�Q�b�g�̐�
            int targetCount = candidateList.Count;

            //�w��Ȃ��A���邢�͈�Ȃ烊�X�g�̍ŏ��̗v�f��Ԃ�
            if(condition.SecondCondition == AdditionalJudge.�w��Ȃ� || targetCount == 1)
            {
                return 0;
            }




            //x�����ԖڂŁAy���ǂ�Ȑ��l�����L�^
            Vector2 container = Vector2.zero;

            //�U�������ǂ���
            bool isAttack = sister.nowMove == SisterParameter.MoveType.�U��;




            //�܂��̓^�[�Q�b�g���X�g�𐮗�
            //����ۂȂ�candidateList�����X�g�̑S�̂ɐݒ�
            if (targetCount == 0)
            {

                if (isAttack)
                {
                    targetCount = SManager.instance._targetList.Count;
                }
                else
                {
                    targetCount = EnemyManager.instance._targetList.Count;
                }


                //�����ł�Target��₪������Ȃ�0��Ԃ�
                if (targetCount == 1)
                {
                    return 0;
                }

                for (int i = 0; i < targetCount; i++)
                {
                    //�^�[�Q�b�g�̐�����
                    //���X�g�Ɍ���ǉ�
                    candidateList.Add(i);
                }
            }



            float result;

            //�d�������邩�ǂ���
            bool duplication = false;


            //�ŏ��̔�r�Ώۂ�ݒ�
            //0�Ԃ�����
            container.Set(0, ReturnValue(condition.SecondCondition, 0, isAttack));

            //�œK�ڕW������
            for (int i = 1; i < targetCount; i++)
            {
                result = ReturnValue(condition.SecondCondition,i,isAttack); 


                if (result == container.y)
                {
                    //�d������
                    duplication = true;
                }

                //�������~�����Ŕ���𕪂���
                //�����Ȃ���傫�����́A�~���Ȃ��菬��������
                if ((condition.targetASOrder) ? (result > container.y):(result < container.y))
                {
                    //�V�����œK�Ώۂ̔ԍ��Ɛ��l��ݒ�
                    container.Set(i, result);

                    //����Ƀ^�[�Q�b�g�X�V�ŏd���͔�����
                    duplication = false;
                }
                

            }



            //�����ďd���������āA�Ȃ����X�y�A�����̎w�肪����Ȃ�
            //����ɍœK�ڕW������
            if(duplication && condition.spareCondition != AdditionalJudge.�w��Ȃ�)
            {

                bool isFirst = true;

                //�O�񌟍��ŘR�ꂽ���̏��O�Ɏg�����l
                float judgeNum = container.y;

                //�œK�ڕW������
                for (int i = 0; i < targetCount; i++)
                {
                    //�܂��͈�ڂ̏����̐��l���m�F
                    result = ReturnValue(condition.SecondCondition, i, isAttack);


                    //�������U���g����ڂ̌����ł̍œK���l�Ɠ����łȂ��Ȃ�
                    //��������͒e��
                    if(result != judgeNum)
                    {
                        //�^�[�Q�b�g��₩��̏��O������99�����ď����𑱂���
                        candidateList[i] = 99;
                        continue;
                    }

                    //����𒴂�����\���̏���
                    result = ReturnValue(condition.spareCondition, i, isAttack);


                    //�ŏ��͖������œ����
                    //�������͕K�v�A�����čŏ��ɔ��肷�������0����Ƃ͌���Ȃ������
                    if (isFirst)
                    {
                        isFirst = false;

                        container.Set(i,result);
                        continue;
                    }

                    //�X�y�A�̏������������~�����Ŕ���𕪂���
                    //�����Ȃ���傫�����́A�~���Ȃ��菬��������
                    if ((condition.spareASOrder) ? (result > container.y) : (result < container.y))
                    {
                        //�V�����œK�Ώۂ̔ԍ��Ɛ��l��ݒ�
                        container.Set(i, result);
                    }


                }

                //�ŁA���ڂ̌������o�Ďc�������̂�Ԃ�
                return (int)container.x;
            }
            //�d�����Ȃ����X�y�A�̏������Ȃ��Ȃ�
            else
            {

                //�������o�Ďc�������̂�Ԃ�
                return (int)container.x;
            }

        }


        


        /// <summary>
        /// ��r�ɕK�v�Ȑ��l��Ԃ�
        /// </summary>
        float ReturnValue(AdditionalJudge condition,int num,bool isAttack)
        {


            //�^�[�Q�b�g�f�[�^���擾
            //�U�����Ȃ�SManager����
            TargetData data = isAttack ? SManager.instance._targetList[num] : EnemyManager.instance._targetList[num];

            if(condition == AdditionalJudge.�^�[�Q�b�g��HP����)
            {
                return data._condition.hpRatio;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g��HP���l)
            {
                return data._condition.hpNum;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g��MP����)
            {
                return data._condition.mpRatio;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̋���)
            {
                //�����̓L���b�V���ɒu��������
                return Vector2.Distance(transform.position,data._condition.targetPosition);
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̍��x)
            {
                return data._condition.targetPosition.y;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̍U����)
            {
                return data._baseData.displayAtk;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̖h���)
            {
                return data._baseData.displayDef;
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̃A�[�}�[�l)
            {

            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̃o�t��)
            {
                //���ڐ��Ńr�b�g��S�������ď�Ԉُ�̐����m�F
                return BitCheck(GetEnumLength<ControllAbillity.PositiveCondition>(),(int)data._condition.buffImfo);
            }
            else if (condition == AdditionalJudge.�^�[�Q�b�g�̃f�o�t��)
            {
                return BitCheck(GetEnumLength<ControllAbillity.NegativeCondition>(), (int)data._condition.debuffImfo);
            }

            return 0;
        }


        /// <summary>
        /// �r�b�g���Z�Ńo�t��f�o�t�̐��𐔂���
        /// </summary>
        int BitCheck(int bitNum,int data)
        {
            int count = 0;
            for (int i = 0;i<bitNum;i++)
            {
                //���������p�r�b�g�̕���1�Ȃ�J�E���g�𑝂₷
                if((bitNum & 1<< i) == 1 << i)
                {
                    count++;
                }

            }
            return count;
        }

        /// <summary>
        /// enum�̍��ڐ����l��
        /// bit���Z�Ɏg��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int GetEnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }


        #endregion

        #endregion




        //���@���f�ł͒e�ۂ̐����Ŗ��@���i�荞��
        //����Ƀ\�[�g���āA�Ǝ��̔��f�Ɍq����?
        //�ł��\�[�g����O�ɓƎ������ł���ɍi�荞�߂΃\�[�g����Ȃ��čœK�E�����
        #region ���@���f�Ɏg�p


        //�U���Ȃ̂��A�T�|�[�g�Ȃ̂��A�񕜂Ȃ̂��Ō����Ώەς���Ă���
        #region ���i�K�A�i�荞��


        /// <summary>
        /// �܂��͒e�ۂ̐����Ŗ��@���i�荞��
        /// �����Ń[���Ȃ�Ȃɂ��g��Ȃ��H
        /// mp�`�F�b�N������H�@���X�g�ɉ����鎞�ɂ�
        /// ����͖��@�i�荞�݂Ɠ����ɓ��Ă͂܂���������邩�ǂ����̌����ł�����
        /// 
        /// ���̓\�[�g�����ǂ��ꂼ��̖��@�̌ŗL�̍i�荞�ݏ����Ƃ������Ȃ��Ƃȁ\
        /// </summary>
        bool MagicJudge(Magic.BulletType condition)
        {
            //�����Ώۂ̖��@�̐�
            int magicCount;


            if(sister.nowMove == SisterParameter.MoveType.�U��)
            {
                //���@�̐�������
                magicCount = SManager.instance.attackMagi.Count;
                
            }
            else if(sister.nowMove == SisterParameter.MoveType.�x��)
            {
                //���@�̐�������
                magicCount = SManager.instance.supportMagi.Count;
            }
            else
            {
                //���@�̐�������
                magicCount = SManager.instance.recoverMagi.Count;
            }

            //���@���Ȃ��Ȃ�߂�
            if(magicCount == 0)
            {
                return false;
            }


            for(int i = 0; i < magicCount; i++)
            {
                //���@�����Ă͂܂�Ȃ�
                if (MagicCheck(i,condition))
                {
                    //���X�g�ɉ����Ă�����
                    candidateList.Add(i);
                }
            }

            return candidateList.Any();

        }


        /// <summary>
        /// �e�ۂ̓����������ɓ��Ă͂܂邩���m�F����
        /// �r�b�g���Z���g��
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool MagicCheck(int num, Magic.BulletType condition)
        {
            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.attackMagi[num].bulletFeature & condition) == condition;

            }
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {
                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.supportMagi[num].bulletFeature & condition) == condition;
            }
            else
            {
                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.recoverMagi[num].bulletFeature & condition) == condition;
            }
        }



        #endregion



        //����ɂ����ōi�荞��ł���\�[�g����
        #region ��ԌŗL�̔��f�Ɏg�p

        #region �x������


        /// <summary>
        /// �x�����@�̌��ʏ������疂�@�̌����i�荞��
        /// �񕜂̓�ڂ̏����ƁA�x���̈�ڂ̏����Ŏg��
        /// ���Ă͂܂閂�@�����邩�����f���ĕԂ�
        /// </summary>
        bool SupportConditionJudge(Magic.SupportType�@condition)
        {

            if(condition == Magic.SupportType.�Ȃ�)
            {
                return true;
            }

            //�����Ώۂ̖��@�̐�
            //��₩�犄��o��
            int magicCount = candidateList.Count;

            //���x����Ԃ�
            bool isSupport = sister.nowMove == SisterParameter.MoveType.�x��;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //���@�̐�������
                    magicCount = SManager.instance.recoverMagi.Count;
                }
            }

            //���@���Ȃ��Ȃ�߂�
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {
                //���@�����Ă͂܂�Ȃ��Ȃ��₩�����
                
                if (!SupportConditionCheck(candidateList[i],condition,isSupport))
                {
                    //i�Ԗڂ̗v�f�����X�g����폜
                    candidateList.RemoveAt(i);
                }
            }
//�����폜�����̂����f���ꂸ�������f�����܂������Ȃ��Ȃ�
//magicCount���R�s�[���č폜���邲�ƂɃJ�E���g�����炵�Ă��ꂪ0���Ŕ��f���Ă�����
            return candidateList.Any();

        }


        /// <summary>
        /// �e�ۂ̓����������ɓ��Ă͂܂邩���m�F����
        /// �r�b�g���Z���g��
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool SupportConditionCheck(int num, Magic.SupportType condition,bool isSupport)
        {
             if (isSupport)
            {
                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.supportMagi[num].supportEffect & condition) == condition;
            }
            else
            {
                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.recoverMagi[num].supportEffect & condition) == condition;
            }
        }


        #endregion

        #region ����

        /// <summary>
        /// �x�����@�̌��ʏ������疂�@�̌����i�荞��
        /// �񕜂̓�ڂ̏����ƁA�x���̈�ڂ̏����Ŏg��
        /// ���Ă͂܂閂�@�����邩�����f���ĕԂ�
        /// </summary>
        bool ElementJudge(AtEffectCon.Element condition)
        {

            if(condition == AtEffectCon.Element.�w��Ȃ�)
            {
                return true;
            }

            //�����Ώۂ̖��@�̐�
            //��₩�犄��o��
            int magicCount = candidateList.Count;

            //���x����Ԃ�
            bool isSupport = sister.nowMove == SisterParameter.MoveType.�x��;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //���@�̐�������
                    magicCount = SManager.instance.attackMagi.Count;
                }
            }

            //���@���Ȃ��Ȃ�߂�
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {

                //���@�����Ă͂܂�Ȃ��Ȃ��₩�����
                if (!ElementCheck(candidateList[i], condition, isSupport))
                {
                    //i�Ԗڂ̗v�f�����X�g����폜
                    candidateList.RemoveAt(i);
                }
            }
            //�����폜�����̂����f���ꂸ�������f�����܂������Ȃ��Ȃ�
            //magicCount���R�s�[���č폜���邲�ƂɃJ�E���g�����炵�Ă��ꂪ0���Ŕ��f���Ă�����
            return candidateList.Any();

        }


        /// <summary>
        /// �e�ۂ̓����������ɓ��Ă͂܂邩���m�F����
        /// �r�b�g���Z���g��
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool ElementCheck(int num, AtEffectCon.Element condition, bool isSupport)
        {
            if (isSupport)
            {
                //�e�ۂ̓������Ɖ�
                return SManager.instance.supportMagi[num].magicElement == condition;
            }
            else
            {
                //�e�ۂ̓������Ɖ�
                return SManager.instance.recoverMagi[num].magicElement == condition;
            }
        }

        #endregion


        #region �񕜏���


        /// <summary>
        /// �x�����@�̌��ʏ������疂�@�̌����i�荞��
        /// �񕜂̓�ڂ̏����ƁA�x���̈�ڂ̏����Ŏg��
        /// ���Ă͂܂閂�@�����邩�����f���ĕԂ�
        /// </summary>
        bool HealConditionJudge(Magic.HealEffectType condition)
        {

            if(condition == Magic.HealEffectType.�Ȃ�)
            {
                return true;
            }

            //�����Ώۂ̖��@�̐�
            //��₩�犄��o��
            int magicCount = candidateList.Count;

            //���x����Ԃ�
            bool isSupport = sister.nowMove == SisterParameter.MoveType.�x��;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //���@�̐�������
                    magicCount = SManager.instance.recoverMagi.Count;
                }
            }

            //���@���Ȃ��Ȃ�߂�
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {
                //���@�����Ă͂܂�Ȃ��Ȃ��₩�����

                if (!HealConditionCheck(candidateList[i], condition))
                {
                    //���X�g����폜
                    candidateList.RemoveAt(i);
                }
            }
            //�����폜�����̂����f���ꂸ�������f�����܂������Ȃ��Ȃ�
            //magicCount���R�s�[���č폜���邲�ƂɃJ�E���g�����炵�Ă��ꂪ0���Ŕ��f���Ă�����
            return candidateList.Any();

        }


        /// <summary>
        /// �e�ۂ̓����������ɓ��Ă͂܂邩���m�F����
        /// �r�b�g���Z���g��
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool HealConditionCheck(int num, Magic.HealEffectType condition)
        {

                //�e�ۂ̓������Ɖ�
                //�����ɋ��ʂ��镔���������������ƕς��Ȃ��ꍇ�̓A�^��
                return (SManager.instance.recoverMagi[num].healEffect & condition) == condition;

        }


        #endregion




        #endregion



        #region ���@�����ʏ�������I��


        /// <summary>
        /// ���@��I������
        /// ��₩������Ɉ�ԓ��Ă͂܂�̂�I�Ԃ���
        /// ���ёւ��͂���Ȃ���
        /// 
        /// �Ԃ��͎̂g�p���閂�@�̔ԍ�
        /// </summary>
        /// <param name="sortCondition"></param>
        /// <param name="targetASOrder"></param>
        int MagicSelect(MagicSortCondition condition,bool asOrder)
        {

            //�����Ώۂ̖��@�̐�
            int magicCount = candidateList.Count;

            //�w��Ȃ��A���邢�͈�����Ȃ��Ȃ烊�X�g�̍ŏ��̗v�f��Ԃ�
            if (condition == MagicSortCondition.�w��Ȃ� || magicCount == 1)
            {
                return 0;
            }

            //x�����ԖڂŁAy���ǂ�Ȑ��l�����L�^
            Vector2 container = Vector2.zero;


            //����ۂȂ烊�X�g�̐���������
            if (magicCount == 0)
            {

                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.attackMagi.Count;

                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    //���@�̐�������
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //���@�̐�������
                    magicCount = SManager.instance.recoverMagi.Count;
                }

                //�����ł�Target��₪������Ȃ�0��Ԃ�
                if (magicCount == 1)
                {
                    return 0;
                }

                //����͉��Ԗڂ̖��@���g�����̔��f��
                //CanList����������K�v�ȏ���
                for (int i = 0; i < magicCount; i++)
                {
                    //�^�[�Q�b�g�̐�����
                    //���X�g�Ɍ���ǉ�
                    candidateList.Add(i);
                }

            }


            float result;

            //�d�������邩�ǂ���
            // bool duplication = false;


            //�܂��[�������Ă��瓮��
            //�ŏ��͖������ɓ���Ă�
            container.Set(0, ReturnMagicValue(condition, 0));

            //�œK�ڕW������
            for (int i = 1; i < magicCount; i++)
            {
                result = ReturnMagicValue(condition, i);



                //�������~�����Ŕ���𕪂���
                //�����Ȃ���傫�����́A�~���Ȃ��菬��������
                if ((asOrder) ? (result > container.y) : (result < container.y))
                {
                    //�V�����œK�Ώۂ̔ԍ��Ɛ��l��ݒ�
                    container.Set(i, result);

                    //����Ƀ^�[�Q�b�g�X�V�ŏd���͔�����
             //       duplication = false;
                }


            }



                //�������o�Ďc�������̂�Ԃ�
                return (int)container.x;


        }





        /// <summary>
        /// ��r�ɕK�v�Ȑ��l��Ԃ�
        /// 
        /// �������@�T�C�Y�A���W�F�l�񕜑��x
        /// </summary>
        float ReturnMagicValue(MagicSortCondition condition, int num)
        {


            //�^�[�Q�b�g�f�[�^���擾
            //�U�����Ȃ�SManager����
            Magic data;



                if (sister.nowMove == SisterParameter.MoveType.�U��)
            {
                //���@�̐�������
                data = SManager.instance.attackMagi[candidateList[num]];

            }
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {
                //���@�̐�������
                data = SManager.instance.supportMagi[candidateList[num]];
            }
            else
            {
                //���@�̐�������
                data = SManager.instance.recoverMagi[candidateList[num]];
            }



            if (sister.nowMove == SisterParameter.MoveType.�U��)
            {


            }
            else if (sister.nowMove == SisterParameter.MoveType.�x��)
            {

            }
            else
            {

            }

            if (condition == MagicSortCondition.���ː�)
            {
                return data.bulletNumber;
            }
            else if (condition == MagicSortCondition.�r������)
            {
                return data.castTime;
            }
            else if (condition == MagicSortCondition.���ʎ���)
            {

                //�e�ۂ̐������Ԃƃo�t���ʂ��̌��ʎ��Ԃ̓��ő傫������Ԃ�
                //��������Β����Ԑݒu���u�̃o�t�ǂ���ł������悤�ɔ�r�ł���
                return Math.Max(data.effectTime,data._moveSt.lifeTime);

            }
            else if (condition == MagicSortCondition.�ǔ����\)
            {
                //�ǔ����\�Ɋp�x���������ĂȂ��Ȃ�365�A���Ă�Ȃ琧���p�x��Ԃ�
                return data._moveSt.fireType == Magic.FIREBULLET.HOMING ? 365 : Math.Abs(data._moveSt.homingAngleV);
            }

            //�����Ɋւ��Ă�UI�̑��ł����Ɓu�U���́v�Ƃ��u�񕜗ʁv�Ƃ���̓I�Ȃ�������ꂵ�Ȃ��ƃ_������
            else if (condition == MagicSortCondition.���ʂ̑傫��)
            {

                if (sister.nowMove == SisterParameter.MoveType.�U��)
                {
                    return data.displayAtk;
                }
                else if (sister.nowMove == SisterParameter.MoveType.�x��)
                {
                    //����͉��̋����{��
                    //�{���Ȃ�ʂ̐��l������
                    return data.mValue;
                }
                else
                {
                    return data.recoverAmount;
                }
            }
            else if (condition == MagicSortCondition.���l)
            {
                return data.shock;
            }
            else if (condition == MagicSortCondition.MP�g�p��)
            {
                return data.useMP;
            }
            else if (condition == MagicSortCondition.���W�F�l�񕜑��x)
            {
                //����l���Ƃ��Ȃ��Ƃ�
                //���W�F�l������ɂ�
            }
            else if (condition == MagicSortCondition.�e��)
            {
                //���ڐ��Ńr�b�g��S�������ď�Ԉُ�̐����m�F
                return data._moveSt.speedV;
            }
            else if (condition == MagicSortCondition.�e�ۂ̑傫��)
            {
                //�T�C�Y���킩��悤�ȋ@�\�����

            }

            return 0;
        }



        #endregion




        #endregion




        #endregion

        #endregion





        ///<summary>
        ///�@�����ł̉񕜂��s��
        ///�@��x�S�����Ō������ăn�Y����������ܕb�Ԕ��肨���Ȃ�Ȃ��悤��
        ///�@���ꂩ���퓬���ɉ񕜃��[�h�ɂ���̂����Y��Ȃ�
        /// </summary>
        #region�@��퓬���񕜗p��AI






        #region�@AI���f�����̊Ǘ�


        /// <summary>
        /// �����ŉ񕜂��Ǘ�����
        /// </summary>
        async UniTaskVoid HealingJudge()
        {

            //�N�[���^�C�����ŃX�L�b�v�������Ȃ��Ȃ�N�[���^�C���I���܂ő҂Ƃ���
            if (disEnable && (int)skipCondition == 0)
            {
                await UniTask.WaitUntil(() => disEnable, cancellationToken: magicCancel.Token);
            }

            //�����ʏ��Ԃ���Ȃ��Ȃ炻��܂Ŗ��@�J�n��҂�
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal, cancellationToken: magicCancel.Token);
            }


            int useNum;


            //�ԍ��l��
            useNum = FirstHealCheck();

            if (useNum == 99)
            {
                //�X�L�b�v�����������ăN�[���^�C�����Ȃ�ċA�Ăяo��
                if (disEnable)
                {

                    //  �����񕜂͎l�b�҂��čċA�Ăяo��
                    //�܂��������f�����
                    await UniTask.Delay(TimeSpan.FromSeconds(4), cancellationToken: magicCancel.Token);


                    HealingJudge().Forget();
                }

                return;
            }

            //���f�Ɏg���f�[�^
            //���S�Ƀq�[����
            SisterConditionBase useCondition = sister.nRecoverCondition[useNum];
            

            //�������Ȃ��Ȃ�߂�
            //�N�[���^�C������ꍇ�͕��򂵂Ă���������
            if (useCondition.selectAction == UseAction.�Ȃɂ����Ȃ�)
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }


            //�^�[�Q�b�g�̍i�荞�݂𑱂���
            //�^�[�Q�b�g�̔ԍ�������o��
            targetNum = TargetSelectStart(useCondition.selectCondition);
            candidateList.Clear();

            //�������疂�@���f
            //�����ɓ��Ă͂܂閂�@���Ȃ��Ȃ�߂�
            if (!MagicJudge(useCondition.bulletCondition))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }

            //��������U���x���񕜂��Ƃɏ������킩���
            //���Ă͂܂�Ȃ�������߂�
            if (!HealSpecificCheck(useNum))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }


            //������MP�Ǝ˒��������`�F�b�N����
            if (!NecessaryCheck(useCondition.mpCheck, useCondition.rangeCheck))
            {
                //���������Ė߂�
                candidateList.Clear();
                return;
            }




            //�^�[�Q�b�g�ɗD�揇�ʂ����đI������
            useMagicNum = MagicSelect(useCondition.magicSort, useCondition.bulletASOder);
            candidateList.Clear();


                coolTime = sister.nRecoverCondition[useNum].coolTime;


            //���@�r���J�n
            CastMagic().Forget();

        }


        #region �ŏ��ɌĂ΂��s���ł��邩�𔻒f����R�[�h

        /// <summary>
        /// �ŏ��ɍs�������𖞂����Ă���̂����m�F����
        /// Disenable���� �X�L�b�v���������Ēl��Ԃ�
        /// </summary>
        /// <returns></returns>
        int FirstHealCheck()
        {

            //�����ł͑ҋ@�񕜂̏������g��
            if (!disEnable)
            {

                    return NormalCheckStart(sister.nRecoverCondition);

            }
            else
            {
  
                    return SkipCheckStart(sister.nRecoverCondition);

            }

        }


        #endregion



        /// <summary>
        /// �X�e�[�g���Ƃɓ��������`�F�b�N���s��
        /// </summary>
        /// <returns></returns>
        bool HealSpecificCheck(int useNum)
        {


                RecoverCondition useCondition = sister.nRecoverCondition[useNum];

                //���������Ă͂܂�Ȃ��Ȃ�false��Ԃ�
                if (!HealConditionJudge(useCondition.secondActJudge))
                {
                    return false;
                }

                //���������Ă͂܂�Ȃ��Ȃ�false��Ԃ�
                return SupportConditionJudge(useCondition.healSupport);
            
        }


        #endregion







        #endregion












        /// <summary>
        /// �o�t�̐��l��^����
        /// �e�ۂ���fireAbillity.BuffCalcu�Ŏ����̃C���X�^���X��n���ČĂ�
        /// �����ő��ɂ����낢�낵���Ⴄ�H
        /// </summary>
        public void BuffCalc(FireBullet _fire)
        {
            _fire.attackFactor = attackFactor;
            _fire.fireATFactor = fireATFactor;
            _fire.thunderATFactor = thunderATFactor;
            _fire.darkATFactor = darkATFactor;
            _fire.holyATFactor = holyATFactor;
        }



        #region �A�j���֘A

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


        #endregion


    }
}