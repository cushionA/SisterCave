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

    //おおまかな設計
    ///　<summary>
    //この機能は音声やエフェクトを再生するための機能です
    //私が使用しているコーギーエンジンの、キャラクターの行動（ダッシュ、ジャンプなど）や状態異常（死、スタン）をステートマシンで管理する機能を使って実装しています。
    //(例:ステートをnowState == ダッシュのような形で照会したりすることでキャラクターの状態を把握できます）
    //このコードではそのステートごとに区切って「エフェクトや音源」（以下では要素と呼びます）を再生しています。
    //またUnitaskによる非同期処理とイベントシステムを利用して、毎フレーム繰り返す処理をゼロにしています
    //加えてエフェクトの再生にはオブジェクトプールを利用し、ガベージコレクションを誘発しにくくしています。
    //
    //以下に具体的な処理の流れを説明します
    //
    //まずステートが切り替わった時に呼ばれるイベントを通じて処理を開始し、切り替わった先のステートで再生する要素があるかを確認します。
    //要素は自作構造体のリスト内で管理されており、構造体には要素のデータそのものに加えて、どのステートでどのように再生するかなどの情報が含まれています。
    //構造体の中で再生するステートを示す_useStateメンバーが、現在のステートに合致する場合は再生されます。
    //そして再生する場合は要素が繰り返し再生されるのか、そのステートにとどまる間はループするのかなどの構造体内の情報を利用して処理を振り分けます。
    //
    //即時再生ならそのまま再生、〇秒待って再生というような待機再生の場合はアニメーションイベントで呼び出しています。
    //リピートの場合はアニメーションイベントで呼び出した際の秒数を格納し、その秒数を用いて実装しています。
    //つまりステートが変わらない限り、非同期処理でおなじ秒数待って要素を再生というようなコードを繰り返し呼び出し続けて実装しました。
    //
    //最後に、次にステートが切り替わった際にループ再生している要素を停止したり、ステートの終わりに使用する設定の音源を再生してそのステートの処理は終了となります。
    //次は切り替わった先のステートで、同じように処理を繰り返していきます
    //
    //また、構造体の中身を変えることで簡単にGUI上から再生する要素を変更できます
    ///　</summary>
    ///　
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {

        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        public override string HelpBoxText() { return "この機能は音声やエフェクトを再生するための機能です。"; }


        //フィールド
        //――――――――――――――――――――――――――――――――――――――――-―――――――――――――――――――――――
#region

        //定義
        #region
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
            justGuard,
            Cast,
            Combination,
            Frozen,
            BlowDead,
            Dead,
            Faltter,
            Blow,
            Wakeup,
            GBreake,//ガードブレイク
            Null
        }

        #endregion


        //インスペクタで設定
        #region

        /// <summary>
        /// エフェクトを生成するクラス
        /// オブジェクトプール機能
        /// Spawnで生成。エフェクトの再生が終了すると自動で消滅
        /// </summary>
        [SerializeField]
        SpawnPool particlesPool;

        /// <summary>
        /// 外からは編集できず、設定はメソッドを使ってやるようにする
        /// エフェクトや音を使うステートと再生方法のリスト
        /// </summary>
        [Header("効果を使うステートと使い方の設定")]
        [SerializeField]
        private List<EffectCondition> _stateList;

        [Header("鎧着てるかどうか")]
        /// <summary>
        /// 鎧着てるかどうか
        /// 共通サウンドが金属系の音になる
        /// </summary>
        public bool isMetal;//鎧着てるかどうか

        [Header("体のサイズ")]
        /// <summary>
        /// 体の大きさ
        /// 大きいと共通サウンドやエフェクトが変化する
        /// </summary>
        public MyCode.SoundManager.SizeTag bodySize;

        [Header("エフェクトのサイズ倍率")]
        /// <summary>
        /// エフェクトのサイズ倍率
        /// </summary>
        public float sizeMultipler = 1;

        #endregion

        //内部ステータス
        #region

        /// <summary>
        /// 現在再生に利用しているステート
        /// </summary>
        SelectState _useState;

        /// <summary>
        /// アニメの再生速度を格納するやつ
        /// 再生速度から影響を受けるなら
        /// </summary>
        float speedMultipler = 1;

        /// <summary>
        /// 今現在利用中の素材を管理するためのリスト
        /// </summary>
        List<EffectCondition.StateEffect> _waitEffect = new List<EffectCondition.StateEffect>();

        List<ParticleSystem> _loopEffect = new List<ParticleSystem>();

        /// <summary>
        /// 今現在利用中の素材を管理するためのリスト
        /// </summary>
        List<EffectCondition.StateSound> _waitSound = new List<EffectCondition.StateSound>();

        /// <summary>
        /// 共通設定の再生を行う際にどう再生するか示す
        /// </summary>
        EffectCondition.EmitType generalEType;

        /// <summary>
        /// 共通設定の再生を行う際にどう再生するか示す
        /// </summary>
        EffectCondition.EmitType generalSType;


        //前回の再生情報を保存することで毎回条件判断を繰り返さないように
        MyCode.SoundManager.PreviousEffect prevE = new MyCode.SoundManager.PreviousEffect();
        MyCode.SoundManager.PreviousSound prevS = new MyCode.SoundManager.PreviousSound();

        #endregion




#endregion


        //メソッド
        //――――――――――――――――――――――――――――――――――――――――-―――――――――――――――――――――――
        #region

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            prevE.state = 10000;
            prevS.state = 10000;
        }
        
         public override void ProcessAbility()
        {
            base.ProcessAbility();

            //着地音をトリガー
            if (_controller.State.JustGotGrounded)
            {
                //吹き飛ばし中に接地したらちゃんと起き上がりへ
                if (_useState == SelectState.Blow)
                {
                    _useState = SelectState.Wakeup;

                    //前のステートのエフェクトとかを処分
                    EffectCheck();
                    SoundCheck();
                    UseSelect(_useState);
                }
                else if (_useState == SelectState.BlowDead)
                {
                    _useState = SelectState.Dead;

                    //前のステートのエフェクトとかを処分
                    EffectCheck();
                    SoundCheck();
                    UseSelect(_useState);
                }

                MyCode.SoundManager.instance.GotGround(transform,_useState,isMetal,bodySize,_character.nowGround);
            }
        }



        ///<summary>
        /// イベントシステム関連の処理
        /// ステートの切り替わり時にそのステートで再生する音源やエフェクトを呼び出す
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
        /// 状態に関するエフェクト
        /// 死とかスタンで出るエフェクト
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> eventType)
        {
           StateSelect(eventType.NewState);

            //前のステートのエフェクトとかを処分
            EffectCheck();
            SoundCheck();

        }

        #endregion

        ///<summary>
        /// ステートの切り替わり時に、そのステートで再生するエフェクトや音を選ぶための処理
        /// </summary>
        #region


        /// <summary>
        /// まずはコーギーエンジンのステートをこのコードで使えるものに変換する
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


            //状態異常のステートは動作のステートに優先する

            //スタン時はHealthから情報を取得
            if (_cState == CharacterStates.CharacterConditions.Stunned)
            {
                SelectState container;
                container = _health.GetStanState();

                if(container == SelectState.Null)
                {
                    return;
                }
                _useState = container;
                MyCode.SoundManager.instance.StanEffect(transform,_useState,_condition.PreviousState == CharacterStates.CharacterConditions.Stunned,sizeMultipler);
                
            }
            else if (_cState == CharacterStates.CharacterConditions.Dead)
            {
               _useState = _health.GetStanState() == SelectState.BlowDead ? SelectState.BlowDead : SelectState.Dead;
            }

            if(_useState != SelectState.Null)
            {
                UseSelect(_useState);
            }

        }

        /// <summary>
        /// 変換したステートで使用するエフェクトや音があるなら処理を開始する
        /// </summary>
        protected void UseSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {
                if (state == _stateList[i]._useState)
                {

                    //なんか再生するならアニメの再生速度取っておく
                    //音やエフェクトに影響するかも
                    speedMultipler =　_character._animator.GetCurrentAnimatorStateInfo(0).speed;

                    //共通設定使うなら
                    if (_stateList[i].generalEffect)
                    {
                        GeneralEPlay(state);
                    }
                    //エフェクトが一つ以上登録されてるなら
                    else if (_stateList[i]._stateEffects.Any())
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
                    

                    //共通設定使うなら
                    if (_stateList[i].generalSound)
                    {
                        GeneralSPlay(state);
                    }
                    //音が一つ以上登録されてるなら
                    else if (_stateList[i]._stateSounds.Any())
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

                    //一つでも合致する状態があれば以降のループはやらない
                    break;
                }
            }
        }

        #endregion

        ///<summary>
        /// 使用する音やエフェクトに、即時再生や待機再生などの性質に従って処理を振り分ける処理
        /// </summary>
        #region
        /// <summary>
        /// エフェクトを再生する処理
        /// </summary>
        protected void EffectStart(EffectCondition.StateEffect _condition)
        {
            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {
                //ここでエフェクトを出す
                EffectSpawn(_condition);
            }
            else
            {
                if (_waitEffect == null)
                {
                    _waitEffect = new List<EffectCondition.StateEffect>();
                }

                // 管理リストに入れる
                _waitEffect.Add(_condition);

                //リピートの場合まずここで鳴らして、二回目のタイミングをアニメ内で指定する
                //一回目から先は実質waitRepeatと同じ処理になる
                if (_condition._emitType == EffectCondition.EmitType.Repeat)
                {
                    //アニメの再生速度に合わせないなら
                    EffectSpawn(_condition);
                }
                //ループなら追加しておく
                if (_condition._emitType == EffectCondition.EmitType.Loop)
                {
                    _loopEffect.Add(EffectSpawn(_condition));
                }
            }
        }

        /// <summary>
        /// 共通エフェクトを再生する処理
        /// ループはなしで
        /// </summary>
        protected void GeneralEPlay(SelectState _state)
        {
            generalEType = EffectCondition.EmitType.Soon;
　　　　　　if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalEType = EffectCondition.EmitType.Repeat;
            }

            if (generalEType != EffectCondition.EmitType.Soon || generalEType != EffectCondition.EmitType.Repeat)
            {
                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(transform,_state,bodySize,_character.nowGround,prevE,sizeMultipler);
            }

        }

        /// <summary>
        /// 音を再生したり管理開始したり
        /// </summary>
        protected void SoundStart(EffectCondition.StateSound _condition)
        {
            if (_condition._playType == EffectCondition.EmitType.Soon)
            {
                //ここで音鳴らす
                SoundPlay(_condition);
            }
            else
            {
                if(_waitSound == null)
                {
                    _waitSound = new List<EffectCondition.StateSound>();
                }

                // 管理リストに入れる
                _waitSound.Add(_condition);

                //リピートの場合まずここで鳴らして、二回目のタイミングをアニメ内で指定する
                //一回目から先は実質waitRepeatと同じ処理になる
                if (_condition._playType == EffectCondition.EmitType.Repeat || _condition._playType == EffectCondition.EmitType.Loop)
                {
                    //アニメの再生速度に合わせないなら
                    SoundPlay(_condition);
                }
            }
        }

        /// <summary>
        /// 音を再生したり管理開始したり
        /// ループはなしで
        /// </summary>
        protected void GeneralSPlay(SelectState _state)
        {
            generalSType = EffectCondition.EmitType.Soon;
            if (_state == SelectState.Falling)
            {
                generalSType = EffectCondition.EmitType.End;
            }
            else if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalSType = EffectCondition.EmitType.Repeat;
            }

            if (generalSType != EffectCondition.EmitType.Soon || generalSType != EffectCondition.EmitType.Repeat)
            {
               prevS =  MyCode.SoundManager.instance.GeneralSoundPlay(transform,_state, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);
            }
        }

        #endregion


        ///<summary>
        /// ステートの切り替わり時に、前のステートで使用していた音やエフェクトを停止させたりするための処理
        /// </summary>
        #region


        /// <summary>
        /// ステートが変わった時のイベントで呼ぶ
        /// ループ音の再生の終了と最後に鳴らす音の再生
        /// </summary>
        void EffectCheck()
        {

            if (generalEType != EffectCondition.EmitType.None)
            {
                if (generalEType == EffectCondition.EmitType.End)
                {
                    prevE = MyCode.SoundManager.instance.GeneralEffectPlay(transform,_useState,  bodySize, _character.nowGround,prevE,sizeMultipler);

                }
                generalEType = EffectCondition.EmitType.None;
            }
            else
            {
                //再生中、待機中のエフェクトが一つでもあるなら
                if (_waitEffect.Any())
                {
                    for (int i = 0; i < _waitEffect.Count; i++)
                    {
                        //最後に出すやつなら今出す
                        if (_waitEffect[i]._emitType == EffectCondition.EmitType.End)
                        {
                            EffectSpawn(_waitEffect[i]);
                        }
                    }
                    //エフェクトの処理が終わったらリストのお掃除
                    _waitEffect.Clear();
                }

                //ループ中のエフェクトが一つでもあるなら
                if (_loopEffect.Any())
                {
                    for (int i = 0; i < _loopEffect.Count; i++)
                    {
                        particlesPool.Despawn(_loopEffect[i].transform);
                    }
                    //エフェクトの処理が終わったらリストのお掃除
                    _loopEffect.Clear();
                }

            }
        }

        /// <summary>
        /// これもステートが変わった時のイベントで呼ぶ
        /// ループ音の再生の終了と最後に鳴らす音の再生
        /// </summary>
        void SoundCheck()
        {

            if (generalSType != EffectCondition.EmitType.None)
            {
                if (generalSType == EffectCondition.EmitType.End)
                {
                    prevS =  MyCode.SoundManager.instance.GeneralSoundPlay(transform,_useState, speedMultipler, isMetal, bodySize,_character.nowGround,prevS);
                }
                generalSType = EffectCondition.EmitType.None;
            }
            else
            {
                //再生中、待機中の音が一つでもあるなら
                if (_waitSound.Any())
                {
                    for (int i = 0; i < _waitSound.Count; i++)
                    {

                        //リピートの音をフェイドさせる
                        if (_waitSound[i]._playType == EffectCondition.EmitType.Loop || _waitSound[i]._playType == EffectCondition.EmitType.WaitLoop)
                        {
                            GManager.instance.StopSound(_waitSound[i]._useSound);
                        }
                        //最後に鳴らすやつなら今鳴らす
                        else if (_waitSound[i]._playType == EffectCondition.EmitType.End)
                        {
                            SoundPlay(_waitSound[i]);
                        }
                    }
                    //音の処理が終わったらリストの掃除
                    _waitSound.Clear();

                }
            }
        }
        #endregion

        ///<summary>
        /// 音やエフェクトの再生を呼ぶアニメーションイベントとその関連処理
        /// リピートの場合同じ間隔で以後再生を繰り返す
        /// </summary>
        #region

        /// <summary>
        /// アニメイベント
        /// このナンバーはリストにおいて対象が何番目の要素であるかということ
        /// </summary>
        /// <param name="number"></param>
        public void EffectStartEvent(int number)
        {
            EffectCondition.EmitType type;

            //これがノンじゃないなら共通で再生
            if (generalEType != EffectCondition.EmitType.None)
            {

                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(transform,_useState,   bodySize, _character.nowGround,prevE,sizeMultipler);
                type = generalEType;
            }
            else
            {
                type = _waitEffect[number]._emitType;
                //ループなら追加しておく
                if (_waitEffect[number]._emitType == EffectCondition.EmitType.WaitLoop)
                {
                    //アニメの再生速度に合わせないなら
                    _loopEffect.Add(EffectSpawn(_waitEffect[number]));
                }
                else
                {
                    //音を鳴らす
                    EffectSpawn(_waitEffect[number]);
                }
            }
            //リピートする音なら
            if (type == EffectCondition.EmitType.Repeat || type == EffectCondition.EmitType.WaitRepeat)
            {
                //繰り返す素材の再生始動
                EffectRepeat(_waitEffect[number], GetCurrentTime(), _useState).Forget();
            }
        }

        /// <summary>
        /// アニメイベント
        /// このナンバーはリストにおいて対象が何番目の要素であるかということ
        /// </summary>
        /// <param name="number"></param>
        public void SoundStartEvent(int number)
        {
            EffectCondition.EmitType type;

            //これがノンじゃないなら共通で再生
            if (generalSType != EffectCondition.EmitType.None)
            {
                prevS = MyCode.SoundManager.instance.GeneralSoundPlay(transform,_useState, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);
                type = generalSType;
            }
            else
            {
                //音を鳴らす
                SoundPlay(_waitSound[number]);
                type = _waitSound[number]._playType;
            }

            //リピートする音なら
            if (type == EffectCondition.EmitType.Repeat || type == EffectCondition.EmitType.WaitRepeat)
            {
                //繰り返す素材の再生始動
                SoundRepeat(_waitSound[number],GetCurrentTime(), _useState).Forget();
            }
        }


        /// <summary>
        /// 現在のアニメーションの経過時間を取得する
        /// </summary>
        /// <returns>経過時間(秒), null のときは常に 0</returns>
         float GetCurrentTime()
        {
            if (_character._animator == null)
                return 0;

            AnimatorStateInfo stateInfo = _character._animator.GetCurrentAnimatorStateInfo(0); //現在のステートを取得
            return stateInfo.length * stateInfo.normalizedTime;
        }

        #endregion

        ///<summary>
        /// 音やエフェクトの再生と、指定した秒数でのリピート処理
        /// </summary>
        #region


        /// <summary>
        /// エフェクトのステータスに合わせて適切に生成する
        /// </summary>
        ParticleSystem EffectSpawn(EffectCondition.StateEffect effect)
        {
                  ParticleSystem ef;
            //アニメの再生速度に合わせないなら
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


            Vector3 ls = ef.transform.localScale;

            //方向無視しないし追従もしてないなら
            if (!effect.ignoreDirection && !effect._isFollow)
            {
                ls.x = Math.Sign(ls.x) == Math.Sign(_character.CharacterModel.transform.localScale.x) ? ls.x : ls.x * -1;
            }

            //サイズ倍率があるなら
            if (sizeMultipler != 1)
            {
                ls *= sizeMultipler;
            }
            ef.transform.localScale = ls;

            return ef;
        }


        /// <summary>
        /// 音のステータスに合わせて適切に鳴らす
        /// </summary>
        /// <param name="sound"></param>
        void SoundPlay(EffectCondition.StateSound sound)
        {
            //アニメの再生速度に合わせないなら
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
        /// ステートにとどまるかぎりエフェクトを繰り返す
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid EffectRepeat(EffectCondition.StateEffect effect, float waitTime, SelectState _state)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //指定秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
            EffectSpawn(effect);

            //リピートしてる音源のステートが維持されてる限りは繰り返し続ける
            if (_state == _useState)
            {
                EffectRepeat(effect, waitTime, _state).Forget();
            }
        }

        /// <summary>
        /// ステートにとどまるかぎり音声を繰り返す
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid SoundRepeat(EffectCondition.StateSound sound, float waitTime, SelectState _state)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //指定秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),cancellationToken:token);
            SoundPlay(sound);

            //リピートしてる音源のステートが維持されてる限りは繰り返し続ける
            if (_state == _useState)
            {
                SoundRepeat(sound,waitTime,_state).Forget();
            }
        }

        #endregion


        ///<summary>
        /// エフェクトや音声をセットしたりの管理コード
        /// </summary>
        #region


        ///<summary>
        ///  再生する音やエフェクトをリセットする
        ///  
        /// </summary>
        public void ResorceReset(List<EffectCondition>　_newList,List<PrefabPool> _newPrefab)
        {
            //エフェクトをリセット
            particlesPool.CleanUp();
            _stateList.Clear();

            if (_newList.Any())
            {
                _stateList = _newList;
            }

            //ステートも再判断
            prevE.state = 10000;
            prevS.state = 10000;

            if (!_newPrefab.Any())
            {
                return;
            }
            for (int i = 0;i < _newPrefab.Count;i++)
            {
               particlesPool.CreatePrefabPool(_newPrefab[i]);
            }

        }


        #endregion



        #endregion


    }
}
