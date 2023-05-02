using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;
using UnityEditor;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// �G�t�F�N�g�R���g���[���[
    /// �T�E���h���������邩�H
    /// </summary>da
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

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
        /// ����҂��
        /// </summary>
        struct EffectControllPropaty
        {
            /// <summary>
            /// ���Ԗڂ̗v�f��
            /// </summary>
            int _number;

            /// <summary>
            /// ���s�[�g����ꍇ���b�҂̂�
            /// </summary>
            float _waitTime;

            /// <summary>
            /// 
            /// </summary>
            EffectCondition.StateEffect _EffectCondition;

        }

        /// <summary>
        /// �G�t�F�N�g��҂��
        /// </summary>
        struct SoundControllPropaty
        {
            /// <summary>
            /// ���Ԗڂ̗v�f��
            /// </summary>
            int _number;

            /// <summary>
            /// ���s�[�g����ꍇ���b�҂̂�
            /// </summary>
            float _waitTime;

            /// <summary>
            /// 
            /// </summary>
            EffectCondition.StateEffect _EffectCondition;

        }


        /// <summary>
        /// �O����͕ҏW�ł����A�ݒ�̓��\�b�h���g���Ă��悤�ɂ���
        /// </summary>
        [SerializeField]
        private List<EffectCondition> _stateList = new List<EffectCondition>();

        List<EffectControllPropaty> _waitEffect = new List<EffectControllPropaty>();

        List<SoundControllPropaty> _waitSound = new List<SoundControllPropaty>();

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// ��Ԃɓ��Ă͂܂�G�t�F�N�g�̏������Ɖ�A�Đ��܂ōs��
        /// </summary>
        protected void EffectSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {
                if (state == _stateList[i]._useState)
                {
                    //�G�t�F�N�g����ȏ�o�^����Ă�Ȃ�
                    if (_stateList[i]._stateEffects.Count > 0)
                    {
                        if (_stateList[i]._stateEffects.Count == 1)
                        {
                            EffectPlay(_stateList[i]._stateEffects[0]);
                        }
                        else
                        {
                            for (int s = 0; s < _stateList[i]._stateEffects.Count; s++)
                            {
                                EffectPlay(_stateList[i]._stateEffects[s]);
                            }

                        }

                    }
                    //������ȏ�o�^����Ă�Ȃ�
                    if (_stateList[i]._stateSounds.Count > 0)
                    {
                        if (_stateList[i]._stateSounds.Count == 1)
                        {
                            SoundPlay(_stateList[i]._stateSounds[0]);
                        }
                        else
                        {
                            for (int s = 0; s < _stateList[i]._stateSounds.Count; s++)
                            {
                                SoundPlay(_stateList[i]._stateSounds[s]);
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// �G�t�F�N�g���Đ����鏈��
        /// </summary>
        protected void EffectPlay(EffectCondition.StateEffect _condition)
        {
            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {

            }

        }
        /// <summary>
        /// �����Đ�
        /// </summary>
        protected void SoundPlay(EffectCondition.StateSound _condition)
        {


        }


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
            ControllEntry(CharacterStates.MovementStates.Nostate,eventType.NewState);
        }


        /// <summary>
        /// ��ԂɊւ���G�t�F�N�g
        /// ���Ƃ��X�^���ŏo��G�t�F�N�g
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> eventType)
        {
           ControllEntry(eventType.NewState);

        }

        /*		
            public MMStateChangeEvent(MMStateMachine<T> stateMachine)
		{
			Target = stateMachine.Target;
			TargetStateMachine = stateMachine;
			NewState = stateMachine.CurrentState;
			PreviousState = stateMachine.PreviousState;
		}
        */

        /// <summary>
        /// �܂��̓X�e�[�g�������Ŏg������̂ɕϊ�����
        /// </summary>
        /// <param name="_mState"></param>
        /// <param name="_cState"></param>
        void ControllEntry(CharacterStates.MovementStates _mState = CharacterStates.MovementStates.Nostate,
            CharacterStates.CharacterConditions _cState = CharacterStates.CharacterConditions.Normal)
        {

            SelectState _useState = SelectState.Null;
            if(_mState != CharacterStates.MovementStates.Nostate)
            {
                _useState = (SelectState)_mState;
            }
            else
            {
                if(_cState == CharacterStates.CharacterConditions.Stunned)
                {
                    _useState = SelectState.Stunned;
                }
                else if (_cState == CharacterStates.CharacterConditions.Dead)
                {
                    _useState = SelectState.Dead;
                }
            }
            if(_useState != SelectState.Null)
            {
                EffectSelect(_useState);
            }
            
        }


        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void EffectStart(int number)
        {

        }

        /// <summary>
        /// �A�j���C�x���g
        /// ���̃i���o�[�̓��X�g�ɂ����đΏۂ����Ԗڂ̗v�f�ł��邩�Ƃ�������
        /// </summary>
        /// <param name="number"></param>
        public void SoundStart(int number)
        {

        }


    }
}
