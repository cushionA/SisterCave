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
    /// エフェクトコントローラー
    /// サウンドも統合するか？
    /// </summary>da
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }

        public enum SelectState
        {
            Idle,
            moving,//通常移動
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
        /// 音を待つやつ
        /// </summary>
        struct EffectControllPropaty
        {
            /// <summary>
            /// 何番目の要素か
            /// </summary>
            int _number;

            /// <summary>
            /// リピートする場合何秒待つのか
            /// </summary>
            float _waitTime;

            /// <summary>
            /// 
            /// </summary>
            EffectCondition.StateEffect _EffectCondition;

        }

        /// <summary>
        /// エフェクトを待つやつ
        /// </summary>
        struct SoundControllPropaty
        {
            /// <summary>
            /// 何番目の要素か
            /// </summary>
            int _number;

            /// <summary>
            /// リピートする場合何秒待つのか
            /// </summary>
            float _waitTime;

            /// <summary>
            /// 
            /// </summary>
            EffectCondition.StateEffect _EffectCondition;

        }


        /// <summary>
        /// 外からは編集できず、設定はメソッドを使ってやるようにする
        /// </summary>
        [SerializeField]
        private List<EffectCondition> _stateList = new List<EffectCondition>();

        List<EffectControllPropaty> _waitEffect = new List<EffectControllPropaty>();

        List<SoundControllPropaty> _waitSound = new List<SoundControllPropaty>();

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }

        /// <summary>
        /// 状態に当てはまるエフェクトの条件を照会し、再生まで行く
        /// </summary>
        protected void EffectSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {
                if (state == _stateList[i]._useState)
                {
                    //エフェクトが一つ以上登録されてるなら
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
                    //音が一つ以上登録されてるなら
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
        /// エフェクトを再生する処理
        /// </summary>
        protected void EffectPlay(EffectCondition.StateEffect _condition)
        {
            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {

            }

        }
        /// <summary>
        /// 音を再生
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
        /// 状態に関するエフェクト
        /// 死とかスタンで出るエフェクト
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
        /// まずはステートをここで使えるものに変換する
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
        /// アニメイベント
        /// このナンバーはリストにおいて対象が何番目の要素であるかということ
        /// </summary>
        /// <param name="number"></param>
        public void EffectStart(int number)
        {

        }

        /// <summary>
        /// アニメイベント
        /// このナンバーはリストにおいて対象が何番目の要素であるかということ
        /// </summary>
        /// <param name="number"></param>
        public void SoundStart(int number)
        {

        }


    }
}
