using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using PathologicalGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{

    //おおまかな設計
    //　この機能は音声やエフェクトを状況に応じて再生するための機能です
    //　
    //　私が使用しているアセットの、キャラクターの行動（ダッシュ、ジャンプなど）や状態異常（死、スタン）をステートマシンで管理する機能を使って実装しています。
    //　(例:ステートをnowState == ダッシュのような形で照会したりすることでキャラクターの状態を把握できます）
    //　このコードではそのステートごとに区切って「エフェクトや音源」（以下では要素と呼びます）を再生しています。
    //　ステートの切り替わりはイベントシステムで通知されます。
    //　またUnitaskによる非同期処理も利用して、毎フレーム繰り返す処理をへらしています
    //　加えてエフェクトの再生にはオブジェクトプールを利用し、処理負荷も軽減しています。
    //　
    //　以下に具体的な処理の流れを説明します
    //　
    //　まずステートが切り替わった時（ダッシュ開始時など）に呼ばれるイベントを通じて処理を開始し、切り替わった先のステート、たとえばダッシュ中に再生する要素があるかを確認します。
    //　要素は自作構造体のリスト内で管理されており、その構造体には要素のデータそのものに加えて、どのステートでどのような方式で再生するかなどの情報が含まれています。
    //　構造体の中で再生するステートを示す_useStateメンバーが、現在のステートに合致する場合は再生されます。
    //　そして再生する場合は要素が繰り返し再生されるのか、そのステートにとどまる間はループするのかなどの構造体内の情報を利用して処理を振り分けます。
    //　
    //　再生方式については即時再生ならステートが切り替わった瞬間に再生、〇秒待って再生というような待機再生の場合はアニメーションイベントで呼び出しています。
    //　リピートの場合はアニメーションイベントで呼び出した際の秒数を格納し、その秒数を使って実装しました。
    //　つまりステートが変わらない限り、非同期処理でおなじ秒数待ってまた要素を再生というようなコードを繰り返し呼び出し続けています。
    //　
    //　加えて、再生情報の構造体の中で「共通エフェクト/共通音声を利用する」という意味を持ったboolフラグがオンになっている場合は、他のクラスが一括管理している共通要素を再生してくれます。
    //　例えばダッシュなどの音やエフェクトは様々なキャラで共通していてもおかしくはないので、共通設定とすることで設定の手間が減ります。
    //　さらに素材を共有することでいろんなキャラクターがよく使うエフェクトを各キャラが余分にプールしないようにしてあります。
    //　
    //　　
    //　最後に、次にステートが切り替わった際にループ再生している要素を停止したり、ステートの終わりに使用する設定の音源を再生してそのステートの処理は終了となります。
    //　次は切り替わった先のステートで、同じように処理を繰り返していきます
    //　
    //　また、構造体の中身を変えることで簡単にGUI上から再生する要素を変更できます
    //
    //--------------------------------------------------------------
    //
    // 《コード内で用いられているほかのクラスについて》
    //
    //　・MyCode.SoundManagerクラス
    //　共通エフェクトと共通サウンドを管理しているクラスです。
    //　現在のステートを渡せばそれに合う要素を再生してくれる。
    //　しかし例えばダッシュ中などで、一定期間何度も同じステートの要素を生成してもらうような場合が多いため
    //  前回再生してもらった要素の名前をキャッシュして、同じ状況なら条件判断無しでその名前の要素を再生するようにしています。
    //
    //  ・EffectConditionクラス
    //　このクラスはステートごとの要素を管理するために必要なクラスで、大きく分けて三つのデータをもっています。
    //  まず一つはエフェクトの再生条件データ（共通素材を使うのか、いくつエフェクトをもってどう再生するのか）。
    //　二つ目がサウンドの再生条件データ。
    //  そして最後に要素をどのステートで再生するのかというデータです。
    //　ステートが切り替わった際には、このコードの内部にあるEffectconditionクラスのリストから現在のステートに合致するEffectconditionメンバーを探し
    //　もしみつかったのならばその再生情報を参照して要素を利用します。　
    //      
    //  ・SpawnPool / PrefabPoolクラス　
    //　このクラスはアセットで、オブジェクトプールシステムの機能を提供するクラスです。
    //  途中でプールの内容を変更できるように改造し、装備変更などにも対応させています。
    //  SpawnPoolがオブジェクトをスポーン、デスポーンするクラスで、PrefabPoolクラスがSpawnPoolに格納するプールオブジェクトのクラスになります。
    //  装備や魔法を変更した際には装備情報に持たせたPrefabPoolクラスリストから新たなプールを生成して使用します。
    [AddComponentMenu("Corgi Engine/Character/Abilities/EffectControllAbility")]
    public class EffectControllAbility : MyAbillityBase, MMEventListener<MMStateChangeEvent<CharacterStates.CharacterConditions>>,
        MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>
    {

        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        public override string HelpBoxText() { return "この機能は音声やエフェクトを再生するための機能です。"; }


        //フィールド
        //――――――――――――――――――――――――――――――――――――――――-―――――――――――――――――――――――
#region

        #region 定義

        /// <summary>
        /// 現在プレイヤーがどのような状態にあるのかを見るための列挙型
        /// </summary>
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



        #region インスペクタで設定

        /// <summary>
        /// エフェクトを生成するクラス
        /// オブジェクトプール機能
        /// Spawnで生成。エフェクトの再生が終了すると自動で消滅
        /// </summary>
        [SerializeField]
        SpawnPool particlesPool;


        /// <summary>
        /// 外からは編集できず、設定はメソッドを使って行うようにする
        /// エフェクトや音を使うステートと再生方法のリスト
        /// </summary>
        [Header("効果を使うステートと使い方の設定")]
        [SerializeField]
        private List<EffectCondition> _stateList;

        [Header("鎧着てるかどうか")]
        /// <summary>
        /// 鎧着てるかどうか
        /// 着ていると共通サウンドが金属系の音になる
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


        [Header("エフェクト生成ポイント")]
        /// <summary>
        ///どこでエフェクトを生成するか
        /// </summary>
        public Transform[] effecter;


        #endregion



        #region　内部ステータス

        /// <summary>
        /// 現在再生に利用しているステート
        /// </summary>
        SelectState _useState = SelectState.Null;

        /// <summary>
        /// アニメの再生速度を格納するやつ
        /// 再生速度から影響を受けるなら
        /// </summary>
        float speedMultipler = 1;

        /// <summary>
        /// 今現在利用中の素材を管理するためのリスト
        /// </summary>
        [SerializeField]
        List<EffectCondition.StateEffect> _waitEffect = new List<EffectCondition.StateEffect>();

        /// <summary>
        /// ループエフェクト
        /// ステートの変わり目で破棄するために保持する
        /// </summary>
        List<ParticleSystem> _loopEffect = new List<ParticleSystem>();

        /// <summary>
        /// 今現在利用中の音を管理するためのリスト
        /// </summary>
        List<EffectCondition.StateSound> _waitSound = new List<EffectCondition.StateSound>();

        /// <summary>
        /// 共通設定の再生を行う際にどう再生するか示す
        /// </summary>
        [SerializeField]
        EffectCondition.EmitType generalEType = EffectCondition.EmitType.None;

        /// <summary>
        /// 共通設定の再生を行う際にどう再生するか示す
        /// </summary>
        EffectCondition.EmitType generalSType = EffectCondition.EmitType.None;


        //前回の再生情報を保存することで毎回条件判断を繰り返さないようにするキャッシュ
        MyCode.SoundManager.PreviousEffect prevE = new MyCode.SoundManager.PreviousEffect();
        MyCode.SoundManager.PreviousSound prevS = new MyCode.SoundManager.PreviousSound();


        /// <summary>
        /// 待ち時間計測用
        /// </summary>
        float waitTimer;

        /// <summary>
        /// 共通サウンドをすでに現在のステートで利用したかどうか
        /// </summary>
        bool pubSUsed;

        /// <summary>
        /// 共通エフェクトをすでに利用したかどうか
        /// </summary>
        bool pubEUsed;


        /// <summary>
        /// 一度だけ使えるキー
        /// 状態が変化するときに変わる
        /// </summary>
        int oneTimeKey = 0;


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

            //サウンドマネージャーに渡すステート情報が初回から合致しないように
            //ありえない数字を入れておく
            prevE.state = 10000;
            prevS.state = 10000;
            
        }
        
         public override void ProcessAbility()
        {
            base.ProcessAbility();




            //着地音をトリガー
            //吹き飛ばされているのなら地面についた時点で起き上がりに状態変更
            if (_controller.State.JustGotGrounded)
            {
                //吹き飛ばし中に接地したらちゃんと起き上がりへ
                if (_useState == SelectState.Blow)
                {
                    _useState = SelectState.Wakeup;

                    //前のステートのエフェクトなどを処分
                    EffectCheck();
                    
                    SoundCheck();
                    UseSelect(_useState);
                }
                //死んだ状態での吹き飛ばし
                else if (_useState == SelectState.BlowDead)
                {
                    _useState = SelectState.Dead;


                    //前のステートのエフェクトなどを処分
                    EffectCheck();
                    SoundCheck();
                    UseSelect(_useState);
                }
                //着地音とエフェクトを使用する
                //落下攻撃中や吹き飛ばしからの着地の場合音やエフェクトが変わる
                MyCode.SoundManager.instance.GotGround(effecter[0],_useState,isMetal,bodySize,_character.nowGround);
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


        /// <summary>
        /// 死やスタンなどの状態変化を読み取るイベント
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.CharacterConditions> eventType)
        {
            //状態が変化したのなら新しい状態で再生要素を探す。
            if (eventType.Target == this.gameObject)
            {
                StateSelect(CharacterStates.MovementStates.Nostate, eventType.NewState);
            }
        }

        /// <summary>
        /// ダッシュやジャンプなどの行動の状態変化を読み取るイベント
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> eventType)
        {
            if (eventType.Target == this.gameObject) 
            {
                //状態が変化したのなら新しい状態で再生要素を探す。
                StateSelect(eventType.NewState);

            }
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

            SelectState newState = SelectState.Null;

            if (_mState != CharacterStates.MovementStates.Nostate)
            {
                newState = (SelectState)_mState;
            }

           // Debug.Log($"むかし{_movement.PreviousState}いま{_movement.CurrentState}");
            //状態異常のステートは動作のステートに優先するので
            //状態異常があるのならuseStateを上書き

            //スタン時はHealthからスタンの種類を取得
            if (_cState == CharacterStates.CharacterConditions.Stunned)
            {
                SelectState container;
                container = _health.GetStunState();

                if(container == SelectState.Null)
                {
                    return;
                }
                newState = container;
                MyCode.SoundManager.instance.StanEffect(effecter[0],newState,_condition.PreviousState == CharacterStates.CharacterConditions.Stunned,sizeMultipler);
                
            }
            else if (_cState == CharacterStates.CharacterConditions.Dead)
            {
               newState = _health.GetStunState() == SelectState.BlowDead ? SelectState.BlowDead : SelectState.Dead;
            }

            if(newState == SelectState.Null || _useState == newState)
            {
                return;
                
            }
            //前のステートのエフェクトとかを処分
            EffectCheck();
            SoundCheck();

            _useState = newState;

            //現在の時間を記録。リピート処理に使う
            waitTimer = Time.time;

            //乱数を記録して現在の状態を固有のものに
            int keyNum = RandomValue(0,100);
            if (keyNum == oneTimeKey)
            {
                oneTimeKey *= 2;
            }
            else
            {
                oneTimeKey = keyNum;
            }

 
           
            //現在のステートに基づき使用できる要素があるか探す
            UseSelect(_useState);
        }

        /// <summary>
        /// 現在のステートで、使用できるエフェクトや音があるなら処理を開始する
        /// </summary>
        protected void UseSelect(SelectState state)
        {
            for (int i = 0; i < _stateList.Count;i++)
            {

 

                //リスト内のEffectconditionの使用するステートが現在のステートに合致するなら
                if (state == _stateList[i]._useState)
                {

                    //現在のアニメの再生速度取っておく
                    //音やエフェクトの再生速度をアニメの再生速度に合わせるようになっているのなら使用する
                    speedMultipler =　_character._animator.GetCurrentAnimatorStateInfo(0).speed;

                    //共通設定使うなら
                    if (_stateList[i].generalEffect)
                    {
                        GeneralEPlay(state);
                    }

                    //固有エフェクトが一つ以上登録されてるなら
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
                    //固有の音が一つ以上登録されてるなら
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

                    //一つでも合致する状態があれば以降のループは回さない
                    break;
                }
            }
        }

        #endregion

        ///<summary>
        /// 使用する音やエフェクトに、即時再生や待機再生などの性質に従って処理を振り分ける処理
        /// </summary>
        #region　ステートで使用する要素を再生方式に従って再生する処理

        /// <summary>
        /// エフェクトを再生する処理
        /// </summary>
        protected void EffectStart(EffectCondition.StateEffect _condition)
        {

            if (_condition._emitType == EffectCondition.EmitType.Soon)
            {
                //すぐに再生ならここでエフェクトを出す
                EffectSpawn(_condition);
            }
            else
            {
                // 管理リストに入れる
                _waitEffect.Add(_condition);

                //リピートの場合まずここで鳴らして、二回目のタイミングをアニメ内で指定する
                //一回目から先は実質waitRepeatと同じ処理になる
                if (_condition._emitType == EffectCondition.EmitType.Repeat)
                {
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
        /// </summary>
        protected void GeneralEPlay(SelectState _state)
        {
            generalEType = EffectCondition.EmitType.Soon;

            //共通素材では再生方式をステートごとに指定する
　　　　　　if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalEType = EffectCondition.EmitType.Repeat;
            }

            if (generalEType != EffectCondition.EmitType.Soon || generalEType != EffectCondition.EmitType.Repeat)
            {
                //再生すると同時に使用した素材の情報を保持して次回の再生に使う
                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_state,bodySize,_character.nowGround,prevE,sizeMultipler,transform.localScale.x > 0);
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
            if (_state == SelectState.moving || _state == SelectState.Running || _state == SelectState.FastFlying ||
                _state == SelectState.Flying || _state == SelectState.Crawling || _state == SelectState.GuardMove)
            {
                generalSType = EffectCondition.EmitType.Repeat;
            }

            if (generalSType != EffectCondition.EmitType.Soon || generalSType != EffectCondition.EmitType.Repeat)
            {
                prevS =  MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0],_state, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);
            }
        }

        #endregion


        ///<summary>
        /// ステートの切り替わり時に、前のステートで使用していた音やエフェクトを停止させたりするための処理
        /// </summary>
        #region　ステート終了時のあと処理


        /// <summary>
        /// ステートが変わった時のイベントで呼ぶ
        /// ループエフェクトの再生の終了と最後に鳴らすエフェクトの再生
        /// </summary>
        void EffectCheck()
        {
            //共通素材を使用した場合の後処理
            if (generalEType != EffectCondition.EmitType.None)
            {
                
                Debug.Log($"あｄｆげｇｄ{generalEType}{_movement.PreviousState}");
                //ステートの終わりに使用する素材であるのなら
                if (generalEType == EffectCondition.EmitType.End)
                {
                    prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_useState,  bodySize, _character.nowGround,prevE,sizeMultipler, transform.localScale.x > 0);

                }
                generalEType = EffectCondition.EmitType.None;

                //共通素材利用フラグを利用前に戻す
                pubEUsed = false;
            }
           // 固有素材を使用した場合の後処理
                //再生中、待機中のエフェクトが一つでもあるなら
                if (_waitEffect.Any())
                {
                    for (int i = 0; i < _waitEffect.Count; i++)
                    {
                        //未使用状態に戻す
                        _waitEffect[i].isUsed = false;

                        //最後に出す素材は今出す
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

        /// <summary>
        /// これもステートが変わった時のイベントで呼ぶ
        /// ループ音の再生の終了と最後に鳴らす音の再生
        /// </summary>
        void SoundCheck()
        {
            //共通素材を使用した場合の後処理
            if (generalSType != EffectCondition.EmitType.None)
            {
                if (generalSType == EffectCondition.EmitType.End)
                {
                    prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS);
                }

                generalSType = EffectCondition.EmitType.None;

                //共通素材利用フラグを利用前に戻す
                pubSUsed = false;
            }
            else
            {
                //再生中、待機中の音が一つでもあるなら
                if (_waitSound.Any())
                {
                    for (int i = 0; i < _waitSound.Count; i++)
                    {
                        //未使用状態に戻す
                        _waitSound[i].isUsed = false;

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

      

            //これがノンじゃないなら共通で再生
            if (generalEType != EffectCondition.EmitType.None)
            {
                //すでに共通素材を利用したなら戻る
                //アニメイベントが飛ばないよう複数設定しても何度も同じエフェクトは呼べない
                if (pubEUsed)
                {
                    return;
                }

                pubEUsed = true;

                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0],_useState,   bodySize, _character.nowGround,prevE,sizeMultipler, transform.localScale.x > 0);

                //リピート処理を開始
                if (generalEType == EffectCondition.EmitType.Repeat || generalEType == EffectCondition.EmitType.WaitRepeat)
                {
                    GeneralERepeat(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS, Time.time - waitTimer, oneTimeKey).Forget();
                }
            }
            else
            {
                //使用済みエフェクトなら戻る
                //アニメイベントが飛ばないよう複数設定しても何度も同じエフェクトは呼べない
                if (_waitEffect == null || !_waitEffect.Any() || _waitEffect[number].isUsed)
                {
                    return;
                }


                _waitEffect[number].isUsed = true;

                //ループなら追加しておく
                if (_waitEffect[number]._emitType == EffectCondition.EmitType.WaitLoop)
                {
                    //アニメの再生速度に合わせないなら
                    _loopEffect.Add(EffectSpawn(_waitEffect[number]));
                }
                else
                {
                    //エフェクトを再生
                    EffectSpawn(_waitEffect[number]);
                }

                //リピートするエフェクトなら
                if (_waitEffect[number]._emitType == EffectCondition.EmitType.Repeat || _waitEffect[number]._emitType == EffectCondition.EmitType.WaitRepeat)
                {
                    //繰り返す素材の再生始動
                    EffectRepeat(_waitEffect[number], Time.time - waitTimer, _useState, oneTimeKey).Forget();
                }
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
                //すでに共通素材を利用したなら戻る
                if (pubSUsed)
                {
                    return;
                }

                pubSUsed = true;

                prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0],_useState, speedMultipler, isMetal, bodySize, _character.nowGround,prevS);


                if (generalSType == EffectCondition.EmitType.Repeat || generalSType == EffectCondition.EmitType.WaitRepeat)
                {

                    GeneralSRepeat(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS, Time.time - waitTimer,oneTimeKey).Forget();
                }
            }
            else
            {
                //使用済みサウンドなら戻る
                if (_waitSound ==null || !_waitSound.Any() || _waitSound[number].isUsed)
                {
                    return;
                }

                _waitSound[number].isUsed = true;


                //音を鳴らす
                SoundPlay(_waitSound[number]);
                type = _waitSound[number]._playType;

                //リピートする音なら
                if (type == EffectCondition.EmitType.Repeat || type == EffectCondition.EmitType.WaitRepeat)
                {
                    //繰り返す素材の再生始動
                    SoundRepeat(_waitSound[number], Time.time - waitTimer, _useState, oneTimeKey).Forget();
                }

            }


        }




        #endregion


        #region　再生とリピート再生処理


        /// <summary>
        /// XとYの間で乱数を出す
        /// 乱数はリピート処理で確認のために使う
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y)
        {

            return UnityEngine.Random.Range(X, Y + 1);

        }


        /// <summary>
        /// エフェクトのステータスに合わせて適切に生成する
        /// </summary>
        ParticleSystem EffectSpawn(EffectCondition.StateEffect effect)
        {
            //エフェクトを出したい場所を指定しないなら標準のエフェクト生成地点を使う
            Transform posi = effecter[effect._emitPosition];

                  ParticleSystem ef;
            //アニメの再生速度に合わせないなら
            if (!effect._matchAnime)
            {
                
                
                if (effect._isFollow)
                {
                    ef = particlesPool.ControlSpawn(effect._useEffect,posi.position, posi.rotation, posi);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, posi.position, posi.rotation);
                }

                var main = ef.main;
                main.simulationSpeed = speedMultipler;
            }
            else
            {
                if (effect._isFollow)
                {
                    ef = particlesPool.ControlSpawn(effect._useEffect, posi.position, posi.rotation, posi);
                }
                else
                {
                    ef = particlesPool.Spawn(effect._useEffect, posi.position, posi.rotation);
                }
            }


            Vector3 ls = ef.transform.localScale;

            //方向無視してて追従もしてないなら
            //何故ここでは方向無視で反転するのかというと、それはキャラのルート下にエフェクト生成するコンポーネント
            //があるせいでプレイヤーのローカルスケールにエフェクトが影響を受けてしまうから
            if (effect.ignoreDirection && !effect._isFollow && transform.localScale.x < 0)
            {
              
                ls.x = ls.x * -1;
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
                    GManager.instance.FollowSound(sound._useSound, effecter[0], pitch: speedMultipler);
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
        /// <param name="effect">使用するエフェクトの情報</param>
        /// <param name="waitTime">再生間隔</param>
        /// <param name="_state">現在の状態</param>
        /// <returns></returns>
        async UniTaskVoid EffectRepeat(EffectCondition.StateEffect effect, float waitTime, SelectState _state,int key)
        {

            var token = this.GetCancellationTokenOnDestroy();


            //指定秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);


            //リピートしてるエフェクトのステートが維持されてる限りは繰り返し続ける
            //同じ状態でも乱数が変わっていて、一度状態が更新されてるなら排除
            if (_state == _useState && oneTimeKey == key)
            { 
                EffectSpawn(effect);
                EffectRepeat(effect, waitTime, _state,key).Forget();
            }


        }



        async UniTaskVoid GeneralERepeat(Transform pos, SelectState state, float multipler, bool isMetal, MyCode.SoundManager.SizeTag _size, MyCharacter.GroundFeature ground, MyCode.SoundManager.PreviousSound prevS, float waitTime,int key)
        {
            var token = this.GetCancellationTokenOnDestroy();

            //指定秒数待つ
            await (UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token));

            //リピートしてるエフェクトのステートが維持されてる限りは繰り返し続ける
            //同じ状態でも乱数が変わっていて、一度状態が更新されてるなら排除
            if (state == _useState && oneTimeKey == key)
            {
               
                prevE = MyCode.SoundManager.instance.GeneralEffectPlay(effecter[0], _useState, bodySize, _character.nowGround, prevE, sizeMultipler, transform.localScale.x > 0);
                GeneralERepeat(pos, state, multipler, isMetal, _size, ground, prevS, waitTime,key).Forget();
            }

        }



        /// <summary>
        /// ステートにとどまるかぎり音声を繰り返す
        /// </summary>
        /// <param name="sound"></param>
        /// <param name="waitTime"></param>
        /// <param name="_state"></param>
        /// <returns></returns>
        async UniTaskVoid SoundRepeat(EffectCondition.StateSound sound, float waitTime, SelectState _state,int key)
        {

            var token = this.GetCancellationTokenOnDestroy();

            //指定秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),cancellationToken:token);

            //リピートしてる音源のステートが維持されてる限りは繰り返し続ける
            //同じ状態でも乱数が変わっていて、一度状態が更新されてるなら排除
            if (_state == _useState && oneTimeKey == key)
            {
                SoundPlay(sound);
                SoundRepeat(sound,waitTime,_state,key).Forget();
            }
        }

        async UniTaskVoid GeneralSRepeat(Transform pos, SelectState state, float multipler, bool isMetal, MyCode.SoundManager.SizeTag _size, MyCharacter.GroundFeature ground, MyCode.SoundManager.PreviousSound prevS,float waitTime,int key)
        {
            var token = this.GetCancellationTokenOnDestroy();

            //指定秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);

            //リピートしてる音源のステートが維持されてる限りは繰り返し続ける
            //同じ状態でも乱数が変わっていて、一度状態が更新されてるなら排除
            if (state == _useState && oneTimeKey == key)
            {            
                prevS = MyCode.SoundManager.instance.GeneralSoundPlay(effecter[0], _useState, speedMultipler, isMetal, bodySize, _character.nowGround, prevS);
                GeneralSRepeat(pos,state,multipler,isMetal,_size,ground,prevS,waitTime,key).Forget();
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

            if (_newPrefab.Any())
            {
                for (int i = 0; i < _newPrefab.Count; i++)
                {

                    particlesPool.CreatePrefabPool(particlesPool.ObjectSetting(_newPrefab[i]));
                    
                }

            }

            
        }


        #endregion



        #endregion


    }
}
