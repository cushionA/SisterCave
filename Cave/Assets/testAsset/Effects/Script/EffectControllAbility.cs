using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using System.Threading;
using PathologicalGames;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{

    //�����܂��Ȑ݌v
    ///�@<summary>
    //���̋@�\�͉�����G�t�F�N�g���Đ����邽�߂̋@�\�ł�
    //�����g�p���Ă���R�[�M�[�G���W���́A�L�����N�^�[�̍s���i�_�b�V���A�W�����v�Ȃǁj���Ԉُ�i���A�X�^���j���X�e�[�g�}�V���ŊǗ�����@�\���g���Ď������Ă��܂��B
    //(��:�X�e�[�g��nowState == �_�b�V���̂悤�Ȍ`�ŏƉ���肷�邱�ƂŃL�����N�^�[�̏�Ԃ�c���ł��܂��j
    //���̃R�[�h�ł͂��̃X�e�[�g���Ƃɋ�؂��āu�G�t�F�N�g�≹���v�i�ȉ��ł͗v�f�ƌĂт܂��j���Đ����Ă��܂��B
    //�܂�Unitask�ɂ��񓯊������ƃC�x���g�V�X�e���𗘗p���āA���t���[���J��Ԃ��������[���ɂ��Ă��܂�
    //�����ăG�t�F�N�g�̍Đ��ɂ̓I�u�W�F�N�g�v�[���𗘗p���A�K�x�[�W�R���N�V������U�����ɂ������Ă��܂��B
    //
    //�ȉ��ɋ�̓I�ȏ����̗����������܂�
    //
    //�܂��X�e�[�g���؂�ւ�������ɌĂ΂��C�x���g��ʂ��ď������J�n���A�؂�ւ������̃X�e�[�g�ōĐ�����v�f�����邩���m�F���܂��B
    //�v�f�͎���\���̂̃��X�g���ŊǗ�����Ă���A�\���̂ɂ͗v�f�̃f�[�^���̂��̂ɉ����āA�ǂ̃X�e�[�g�łǂ̂悤�ɍĐ����邩�Ȃǂ̏�񂪊܂܂�Ă��܂��B
    //�\���̂̒��ōĐ�����X�e�[�g������_useState�����o�[���A���݂̃X�e�[�g�ɍ��v����ꍇ�͍Đ�����܂��B
    //�����čĐ�����ꍇ�͗v�f���J��Ԃ��Đ������̂��A���̃X�e�[�g�ɂƂǂ܂�Ԃ̓��[�v����̂��Ȃǂ̍\���̓��̏��𗘗p���ď�����U�蕪���܂��B
    //
    //�����Đ��Ȃ炻�̂܂܍Đ��A�Z�b�҂��čĐ��Ƃ����悤�ȑҋ@�Đ��̏ꍇ�̓A�j���[�V�����C�x���g�ŌĂяo���Ă��܂��B
    //���s�[�g�̏ꍇ�̓A�j���[�V�����C�x���g�ŌĂяo�����ۂ̕b�����i�[���A���̕b����p���Ď������Ă��܂��B
    //�܂�X�e�[�g���ς��Ȃ�����A�񓯊������ł��Ȃ��b���҂��ėv�f���Đ��Ƃ����悤�ȃR�[�h���J��Ԃ��Ăяo�������Ď������܂����B
    //
    //�Ō�ɁA���ɃX�e�[�g���؂�ւ�����ۂɃ��[�v�Đ����Ă���v�f���~������A�X�e�[�g�̏I���Ɏg�p����ݒ�̉������Đ����Ă��̃X�e�[�g�̏����͏I���ƂȂ�܂��B
    //���͐؂�ւ������̃X�e�[�g�ŁA�����悤�ɏ������J��Ԃ��Ă����܂�
    //
    //�܂��A�\���̂̒��g��ς��邱�ƂŊȒP��GUI�ォ��Đ�����v�f��ύX�ł��܂�
    ///�@</summary>
    ///�@
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {

        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        public override string HelpBoxText() { return "���̋@�\�͉�����G�t�F�N�g���Đ����邽�߂̋@�\�ł��B"; }

        //�t�B�[���h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
        #region

        /// <summary>
        /// �G�t�F�N�g�𐶐�����N���X
        /// �I�u�W�F�N�g�v�[���@�\
        /// Spawn�Ő����B�G�t�F�N�g�̍Đ����I������Ǝ����ŏ���
        /// </summary>
        [SerializeField]
        SpawnPool particlesPool;

        public enum SelectState
        {
            Idle,
            moving,//�ʏ�ړ�
            Falling,
            Running,
            Crouching,
            Crawling,
            Jumping,
            DoubleJumping,
            Flying,
            FastFlying,
            Rolling,
            Attack,
            Guard,
            GuardMove,
            Warp,
            Parry,
            Cast,
            Combination,
            Frozen,
            Dead,
            Stunned,
            Null
        }

        /// <summary>
        /// ���ݍĐ��ɗ��p���Ă���X�e�[�g
        /// </summary>
        SelectState _useState;

        /// <summary>
        /// �A�j���̍Đ����x���i�[������
        /// �Đ����x����e�����󂯂�Ȃ�
        /// </summary>
        float speedMultipler = 1;

        /// <summary>
        /// �O����͕ҏW�ł����A�ݒ�̓��\�b�h���g���Ă��悤�ɂ���
        /// �G�t�F�N�g�≹���g���X�e�[�g�̃��X�g
        /// </summary>
        [SerializeField]
        private List<EffectCondition> _stateList = new List<EffectCondition>();



        /// <summary>
        /// �����ݗ��p���̑f�ނ��Ǘ����邽�߂̃��X�g
        /// </summary>
        List<EffectCondition.StateEffect> _waitEffect = new List<EffectCondition.StateEffect>();

        List<ParticleSystem> _loopEffect = new List<ParticleSystem>();

        /// <summary>
        /// �����ݗ��p���̑f�ނ��Ǘ����邽�߂̃��X�g
        /// </summary>
        List<EffectCondition.StateSound> _waitSound = new List<EffectCondition.StateSound>();

        #endregion


        //���\�b�h
        //�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\-�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\�\
        #region

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
        }


        ///<summary>
        /// �C�x���g�V�X�e���֘A�̏���
        /// �X�e�[�g�̐؂�ւ�莞�ɂ��̃X�e�[�g�ōĐ����鉹����G�t�F�N�g���Ăяo��
        /// </summary>
        #region
        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
            this.MMEventStartListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<MMStateChangeEvent<CharacterStates.CharacterConditions>>();
            this.MMEventStopListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
        }


        public void OnMMEvent(MMStateChangeEvent<CharacterStates.CharacterConditions> eventType)
        {
            StateSelect(CharacterStates.MovementStates.Nostate,eventType.NewState);
        }


        /// <summary>
        /// ��ԂɊւ���G�t�F�N�g
        /// ���Ƃ��X�^���ŏo��G�t�F�N�g
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> eventType)
        {
           StateSelect(eventType.NewState);

            //�O�̃X�e�[�g�̃G�t�F�N�g�Ƃ�������
            EffectCheck();
            SoundCheck();

        }

        #endregion

        ///<summary>
        /// �X�e�[�g�̐؂�ւ�莞�ɁA���̃X�e�[�g�ōĐ�����G�t�F�N�g�≹��I�Ԃ��߂̏���
        /// </summary>
        #region


        /// <summary>
        /// �܂��̓R�[�M�[�G���W���̃X�e�[�g�����̃R�[�h�Ŏg������̂ɕϊ�����
        /// </summary>
        /// <param name="_mState"></param>
        /// <param name="_cState"></param>
        void StateSelect(CharacterStates.MovementStates _mState = CharacterStates.MovementStates.Nostate,
            CharacterStates.CharacterConditions _cState = CharacterStates.CharacterConditions.Normal)
        {

            _useState = SelectState.Null;
            if (_mState != CharacterStates.MovementStates.Nostate)
            {
                _useState = (SelectState)_mState;
            }


            //��Ԉُ�̃X�e�[�g�͓���̃X�e�[�g�ɗD�悷��
            if (_cState == CharacterStates.CharacterConditions.Stunned)
            {
                _useState = SelectState.Stunned;
            }
            else if (_cState == CharacterStates.CharacterConditions.Dead)
            {
                _useState = SelectState.Dead;
            }

            if (_useState != SelectState.Null)
            {
                UseSelect(_useState);
            }

        }

        /// <summary>
        /// �ϊ������X�e�[�g�Ŏg�p����G�t�F�N�g�≹������Ȃ珈�����J�n����
        /// </summary>
        protected void UseSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {
                if (state == _stateList[i]._useState)
                {

                    //�Ȃ񂩍Đ�����Ȃ�A�j���̍Đ����x����Ă���
                    //����G�t�F�N�g�ɉe�����邩��
                    speedMultipler =�@_character._animator.GetCurrentAnimatorStateInfo(0).speed;

                    //�G�t�F�N�g����ȏ�o�^����Ă�Ȃ�
                    if (_stateList[i]._stateEffects.Any())
                    {
                        float count = _stateList[i]._stateEffects.Count;
                        if (count == 1)
                        {
                            EffectStart(_stateList[i]._stateEffects[0]);
                        }
                        else
                        {
                            for (int s = 0; s < count; s++)
                            {
                                EffectStart(_stateList[i]._stateEffects[s]);
                            }

                        }

                    }
                    //������ȏ�o�^����Ă�Ȃ�
                    if (_stateList[i]._stateSounds.Any())
                    {
                        float count = _stateList[i]._stateSounds.Count;
                        if (count == 1)
                        {
                            SoundStart(_stateList[i]._stateSounds[0]);
                        }
                        else
                        {
                            for (int s = 0; s < count; s++)
                            {
                                SoundStart(_stateList[i]._stateSounds[s]);
                            }

                        }
                    }

                    //��ł����v�����Ԃ�����Έȍ~�̃��[�v�͂��Ȃ�
                    break;
                }
            }
        }

        #endregion

        ///<summary>
        /// �g�p���鉹��G�t�F�N�g�ɁA�����Đ���ҋ@�Đ��Ȃǂ̐����ɏ]���ď�����U�蕪���鏈��
        /// </summary>
        #region
        /// <summary>
        /// �G�t�F�N�g���Đ����鏈��
        /// </summary>
        protected void EffectStart(EffectCondition.StateEffect _condition)
        {
            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {
                //�����ŉ��炷
                EffectSpawn(_condition);
            }
            else
            {
                if (_waitEffect == null)
                {
                    _waitEffect = new List<EffectCondition.StateEffect>();
                }

                // �Ǘ����X�g�ɓ����
                _waitEffect.Add(_condition);

                //���s�[�g�̏ꍇ�܂������Ŗ炵�āA���ڂ̃^�C�~���O���A�j�����Ŏw�肷��
                //���ڂ����͎���waitRepeat�Ɠ��������ɂȂ�
                if (_condition._emitType == EffectCondition.EmitType.Repeat)
                {
                    //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
                    EffectSpawn(_condition);
                }
                //���[�v�Ȃ�ǉ����Ă���
                if (_condition._emitType == EffectCondition.EmitType.Loop)
                {
                    _loopEffect.Add(EffectSpawn(_condition));
                }
            }
        }

        /// <summary>
        /// �����Đ�������Ǘ��J�n������
        /// </summary>
        protected void SoundStart(EffectCondition.StateSound _condition)
        {
            if (_condition._playType == EffectCondition.EmitType.Soon)
            {
                //�����ŉ��炷
                SoundPlay(_condition);
            }
            else
            {
                if(_waitSound == null)
                {
                    _waitSound = new List<EffectCondition.StateSound>();
                }

                // �Ǘ����X�g�ɓ����
                _waitSound.Add(_condition);

                //���s�[�g�̏ꍇ�܂������Ŗ炵�āA���ڂ̃^�C�~���O���A�j�����Ŏw�肷��
                //���ڂ����͎���waitRepeat�Ɠ��������ɂȂ�
                if (_condition._playType == EffectCondition.EmitType.Repeat || _condition._playType == EffectCondition.EmitType.Loop)
                {
                    //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
                    SoundPlay(_condition);
                }
            }
        }

        #endregion


        ///<summary>
        /// �X�e�[�g�̐؂�ւ�莞�ɁA�O�̃X�e�[�g�Ŏg�p���Ă�������G�t�F�N�g���~�������肷�邽�߂̏���
        /// </summary>
        #region


        /// <summary>
        /// �X�e�[�g���ς�������̃C�x���g�ŌĂ�
        /// ���[�v���̍Đ��̏I���ƍŌ�ɖ炷���̍Đ�
        /// </summary>
        void EffectCheck()
        {
            //�Đ����A�ҋ@���̃G�t�F�N�g����ł�����Ȃ�
            if (_waitEffect.Any())
            {
                for (int i = 0; i < _waitEffect.Count; i++)
                {
                    //�Ō�ɏo����Ȃ獡�o��
                    if (_waitEffect[i]._emitType == EffectCondition.EmitType.End)
                    {
                        EffectSpawn(_waitEffect[i]);
                    }
                }
                //�G�t�F�N�g�̏������I������烊�X�g�̂��|��
                _waitEffect = new List<EffectCondition.StateEffect>();
            }

            //���[�v���̃G�t�F�N�g����ł�����Ȃ�
            if (_loopEffect.Any())
            {
                for (int i = 0; i < _loopEffect.Count; i++)
                {
                    particlesPool.Despawn(_loopEffect[i].transform);
                }
                //�G�t�F�N�g�̏������I������烊�X�g�̂��|��
                _loopEffect = new List<ParticleSystem>();
            }


        }

        /// <summary>
        /// ������X�e�[�g���ς�������̃C�x���g�ŌĂ�
        /// ���[�v���̍Đ��̏I���ƍŌ�ɖ炷���̍Đ�
        /// </summary>
        void SoundCheck()
        {
            //�Đ����A�ҋ@���̉�����ł�����Ȃ�
            if (_waitSound.Any())
            {
                for (int i = 0; i < _waitSound.Count; i++)
                {

                    //���s�[�g�̉����t�F�C�h������
                    if (_waitSound[i]._playType == EffectCondition.EmitType.Loop || _waitSound[i]._playType == EffectCondition.EmitType.WaitLoop)
                    {
                        GManager.instance.StopSound(_waitSound[i]._useSound);
                    }
                    //�Ō�ɖ炷��Ȃ獡�炷
                    else if (_waitSound[i]._playType == EffectCondition.EmitType.End)
                    {
                        SoundPlay(_waitSound[i]);
                    }
                }
                //���̏������I������烊�X�g�̂��|��
                _waitSound = new List<EffectCondition.StateSound>();

            }
        }
        #endregion

        ///<summary>
        /// ����G�t�F�N�g�̍Đ����ĂԃA�j���[�V�����C�x���g�Ƃ��̊֘A����
        /// ���s�[�g�̏ꍇ�����Ԋu�ňȌ�Đ����J��Ԃ�
        /// </summary>
        #region

        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void EffectStartEvent(int number)
        {
            //���[�v�Ȃ�ǉ����Ă���
            if (_waitEffect[number]._emitType == EffectCondition.EmitType.WaitLoop)
            {
                //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
                _loopEffect.Add(EffectSpawn(_waitEffect[number]));
            }
            else
            {
                //����炷
                EffectSpawn(_waitEffect[number]);
            }
            //���s�[�g���鉹�Ȃ�
            if (_waitEffect[number]._emitType == EffectCondition.EmitType.Repeat || _waitEffect[number]._emitType == EffectCondition.EmitType.WaitRepeat)
            {
                //�J��Ԃ��f�ނ̍Đ��n��
                EffectRepeat(_waitEffect[number], GetCurrentTime(), _useState).Forget();
            }
        }

        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void SoundStartEvent(int number)
        {
            //����炷
            SoundPlay(_waitSound[number]);

            //���s�[�g���鉹�Ȃ�
            if (_waitSound[number]._playType == EffectCondition.EmitType.Repeat || _waitSound[number]._playType == EffectCondition.EmitType.WaitRepeat)
            {
                //�J��Ԃ��f�ނ̍Đ��n��
                SoundRepeat(_waitSound[number],GetCurrentTime(), _useState).Forget();
            }
        }


        /// <summary>
        /// ���݂̃A�j���[�V�����̌o�ߎ��Ԃ��擾����
        /// </summary>
        /// <returns>�o�ߎ���(�b), null �̂Ƃ��͏�� 0</returns>
         float GetCurrentTime()
        {
            if (_character._animator == null)
                return 0;

            AnimatorStateInfo stateInfo = _character._animator.GetCurrentAnimatorStateInfo(0); //���݂̃X�e�[�g���擾
            return stateInfo.length * stateInfo.normalizedTime;
        }

        #endregion

        ///<summary>
        /// ����G�t�F�N�g�̍Đ��ƁA�w�肵���b���ł̃��s�[�g����
        /// </summary>
        #region


        /// <summary>
        /// ���̃X�e�[�^�X�ɍ��킹�ēK�؂ɐ�������
        /// </summary>
        ParticleSystem EffectSpawn(EffectCondition.StateEffect effect)
        {
                  ParticleSystem ef;
            //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
            if (!effect._matchAnime)
            {
                
                
                if (effect._isFollow)
                {
                    ef = particlesPool.Spawn(effect._useEffect,effect._emitPosition.position, effect._emitPosition.rotation, effect._emitPosition);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, effect._emitPosition.position, effect._emitPosition.rotation);
                }

                var main = ef.main;
                main.simulationSpeed = speedMultipler;
            }
            else
            {
                if (effect._isFollow)
                {
                    ef = particlesPool.Spawn(effect._useEffect, effect._emitPosition.position, effect._emitPosition.rotation, effect._emitPosition);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, effect._emitPosition.position, effect._emitPosition.rotation);
                }
            }
            return ef;
        }


        /// <summary>
        /// ���̃X�e�[�^�X�ɍ��킹�ēK�؂ɖ炷
        /// </summary>
        /// <param name="sound"></param>
        void SoundPlay(EffectCondition.StateSound sound)
        {
            //�A�j���̍Đ����x�ɍ��킹�Ȃ��Ȃ�
            if (!sound._matchAnime)
            {
                if (sound._isFollow)
                {
                    GManager.instance.FollowSound(sound._useSound, transform);
                }
                else
                {
                    GManager.instance.PlaySound(sound._useSound, transform.position);
                }
            }
            else
            {
                if (sound._isFollow)
                {
                    GManager.instance.FollowSound(sound._useSound, transform, pitch: speedMultipler);
                }
                else
                {
                    GManager.instance.PlaySound(sound._useSound, transform.position, pitch: speedMultipler);
                }
            }
        }



        /// <summary>
        /// �X�e�[�g�ɂƂǂ܂邩����G�t�F�N�g���J��Ԃ�
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid EffectRepeat(EffectCondition.StateEffect effect, float waitTime, SelectState _state)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //�w��b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            EffectSpawn(effect);

            //���s�[�g���Ă鉹���̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            if (_state == _useState)
            {
                EffectRepeat(effect, waitTime, _state).Forget();
            }
        }

        /// <summary>
        /// �X�e�[�g�ɂƂǂ܂邩���艹�����J��Ԃ�
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid SoundRepeat(EffectCondition.StateSound sound, float waitTime, SelectState _state)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //�w��b���҂�
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),cancellationToken:token);
            SoundPlay(sound);

            //���s�[�g���Ă鉹���̃X�e�[�g���ێ�����Ă����͌J��Ԃ�������
            if (_state == _useState)
            {
                SoundRepeat(sound,waitTime,_state).Forget();
            }
        }

        #endregion



        #endregion


    }
}
