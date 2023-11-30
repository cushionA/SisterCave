using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static CombatManager;
using static SisterMoveSetting;
using static SisterParameter;


namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// 普通に状況判断して動くためのスクリプト
	/// ワープ、攻撃、コンビネーション（各動作は分ける？）は別スクリプトに
	/// あるいはコンビネーションは主人公に持たせる？
	/// 移動機能の改修
	/// ・移動と方向転換は共通のコントローラーに集める
	/// ・各メソッドでは状態変化と移動方向だけ指定
    /// 
    /// 
    /// 機能再定義
    /// 
    /// ・移動ステータスに応じて移動判断をする
    /// ・センサーで敵密度をはかる？
    /// ・移動ステータスの条件に応じてメソッドを呼び出す
    /// ・判断したらオプションに応じて移動地点を決める
    /// ・ワープ指定で、さらにワープできるならワープする。ワープしたらもうそこで停止
    /// ・条件判断は攻撃中だろうがお構いなく行う。そして真になったら移動する
    /// ・一回移動したら一回確実に攻撃する。攻撃などで攻撃中断したらさらに移動
    /// ・壁に当たった時の行動も
    /// 
    /// ・攻撃メソッドでは攻撃前に今移動中かをこのスクリプトに問い合わせて止まるようにする
    /// ・攻撃中は影響しない
    /// 
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/EditBrain")]
    public class EditBrain : NPCControllerAbillity
    {

        #region 定義


        /// <summary>
        /// 現在の状態
        /// これによって動きが変わる
        /// </summary>
        public enum SisterState
        {
            のんびり,
            警戒,
            戦い,
            最初
        }

        /// <summary>
        /// 現在の移動方式
        /// のんびりや警戒、環境物反応など
        /// 状況に応じた判断で移動モードと移動方向だけを決める
        /// 戦闘時は走りと停止だけ
        /// そして共通の移動メソッドを利用して移動する
        /// </summary>
        public enum MoveState
        {
            停止,
            歩き,
            走り,
            最初//最初の時は移動せず判断に行く
        }





        #endregion

        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "TODO_HELPBOX_TEXT."; }



        [Header("警戒ステートの時間")]
        public float patrolTime;


        public PlyerController pc;



        #region 挙動管理情報

        [Header("シスターさんのステータス")]
        public SisterStatus status;

        [Header("移動パラメータ")]
        [SerializeField]
        SisterParameter parameter;


        /// <summary>
        /// 現在のモードで移動に使う情報
        /// </summary>
        StateMoveStatus moveStatus;


        /// <summary>
        /// 今移動の基準にしてる相手
        /// </summary>
        TargetData nowTarget;

        //[HideInInspector]
        public SisterState nowState = SisterState.のんびり;
        
        [HideInInspector] public MoveState nowMove = MoveState.最初;


        [Header("現在どのタイプの魔法を使おうとしてるのか")]
        [HideInInspector] public MoveType nowMode;

        #endregion


        // === キャッシュ ======================================
        // === アビリティ ==========================================

        #region



        //横移動は継承もとにある

        public PlayerJump _jump;

        public PlayerRunning _characterRun;


        public MyWakeUp _wakeup;


        public MyDamageOntouch _damage;

        public WarpAbility _warp;

        public FireAbility _fire;


        public PlayerCrouch _squat;


        //シスターさんの固有アクション

        public SensorAbility _sensor;


        // アニメーションパラメーター
        protected const string _stateParameterName = "_nowPlay";
        protected int _stateAnimationParameter;

        #endregion





        // === 内部パラメータ ======================================

        #region

        /// <summary>
        /// 移動後にどれくらいダメージ受けたか
        /// </summary>
        float totalDamage;

        /// <summary>
        /// 移動後にどれだけ時間が経ったか測るためのもの
        /// 経過時間が条件で動くときに使う
        /// 移動終了後に記録する
        /// </summary>
        float totalTime;

        /// <summary>
        /// 環境物
        /// これがあるとイベントの引き金になる
        /// モードが切り替わったり離れたりすると消える
        /// レポートセンサーで報告されなくなったら消える
        /// </summary>
        GameObject eventObject;

        /// <summary>
        /// 進行方向に穴があるか調べるための変数
        /// この位置からレイを出す
        /// </summary>
        Vector2 holeCheckPoint;

        /// <summary>
        /// 地面があるか、あるいは穴があるかを確かめるために使うレイキャストの長さ
        /// </summary>
        float groundCheckLength = 30;

        #endregion



        // === ステータスパラメータ ======================================

        #region

        /// <summary>
        /// 攻撃倍率
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

        [HideInInspector]
        //ステータス
        //hpはヘルスでいい
        public float maxHp;
        [HideInInspector]
        public float maxMp;

        /// <summary>
        /// 自動回復分含めたMPの総量
        /// MPの自動回復はここから引いていく
        /// </summary>
        float mpStorage;


        [HideInInspector]
        public float mp;

        #endregion

        /// <summary>
        /// キャンセル用のトークン
        /// </summary>
        CancellationTokenSource moveJudgeCancel;

        // === コード（AIの内容） ================



        

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;


            nowState = SisterState.最初;
            //最初の処理
            ParameterSet(status);
            StateChange(SisterState.のんびり);
        }



        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            GManager.instance.sisMpSlider.value = mp / maxMp;
            Brain();
        }






        public void Brain()
        {

            JumpController();
        }



        #region 状態管理コード







        /// <summary>
        /// ステート変化前の処理
        /// 
        /// 戦闘:ダメージを受けるか敵を見つけるで起動。敵の全滅か戦闘状態の敵がいなくなったら元に戻る。あるいは一番近い敵が戦闘終了距離に来たら？
        /// 警戒:戦闘後か危険物見つけた時に起動。時間経過で解除
        /// のんびり:何にもないときはこれ
        /// 
        /// ファストトラベル後とかのために全部リセットする機能も用意しとこう
        /// </summary>
        /// <param name="nextState"></param>
        public void StateChange(SisterState nextState)
        {

            //現在の状態から変わらない上書きであるか
            bool notChange = nextState == nowState;

            if (nextState == SisterState.戦い)
            {
                //変更されているなら
                if (!notChange)
                {
                    nowState = nextState;

                    //イベントオブジェクトを消す
                    eventObject = null;

                    //攻撃モジュールに戦闘開始を伝える
                    _fire.StateInitialize(true);

                    //戦闘センサー起動
                    _sensor.BattleStart();


                    //キャンセルしてトークン入れ替え
                    TokenReset();


                    //行動傾向を優先の物に設定
                    CombatModeChange(parameter.priority);


                    //攻撃移動開始
                    CombatMoveJudge(isFirst: true).Forget();

                    //攻撃状態解除判断メソッド起動
                    BattleEndJudge().Forget();

                }
            }
            //戦闘以外なら
            else
            {
                //前が戦いか最初ならもろもろの処理を
                if (nowState == SisterState.戦い || nowState == SisterState.最初)
                {
                    //非戦闘センサー起動
                    _sensor.BattleEnd();
                    //攻撃モジュールに戦闘終了を伝える
                    _fire.StateInitialize(false);

                    //基準キャラ情報消去
                    nowTarget = null;

                    //キャンセルしてトークン入れ替え
                    TokenReset();

                    //通常移動開始
                    MoveController().Forget();


                }

                //変わってるなら
                if (!notChange)
                {


                    //次警戒なら警戒時限解除メソッドを起動
                    if(nextState == SisterState.警戒)
                    {
                        //警戒状態の時限解除
                        PatrolEndJudge(isFirst:true).Forget();
                    }

                    //状態変更
                    nowState = nextState;
                }
            }


        }


        /// <summary>
        /// 戦闘中の行動モードを変更する
        /// 同時に移動に使用する情報も入れ替える
        /// </summary>
        /// <param name="nextMode"></param>
        void CombatModeChange(MoveType nextMode)
        {
            nowMode = nextMode;

            if(nextMode == MoveType.攻撃)
            {
                //移動ステータスを設定
                moveStatus = parameter.sisterMoveSetting.AttackMoveSetting;
            }
            else if(nextMode == MoveType.支援)
            {
                //移動ステータスを設定
                moveStatus = parameter.sisterMoveSetting.SupportMoveSetting;
            }
            else
            {
                //移動ステータスを設定
                moveStatus = parameter.sisterMoveSetting.HealMoveSetting;
            }

        }

        /// <summary>
        /// キャンセルした後
        /// キャンセルトークンを再度入れ直す
        /// 同時に全状態で使うメソッドの軌道も行う
        /// </summary>
        void TokenReset()
        {
            //キャンセルしてトークン入れ替え
            moveJudgeCancel.Cancel();
            moveJudgeCancel = new CancellationTokenSource();

            //キャンセルトークンを入れ替えたので距離維持ワープも呼び直す
            DistanceKeepWarp().Forget();

            //MP回復
            MPRecover().Forget();


        }

        /// <summary>
        /// 移動などのデータをセット
        /// 最初に呼ぶ
        /// </summary>
        /// <param name="status"></param>
        void ParameterSet(SisterStatus status)
        {
            ///<summary>
            ///　リスト
            /// </summary>
            #region
            /*
		 CharacterJump _characterJump;
		 PlayerRoll _rolling;
		 CharacterRun _characterRun;
		 EnemyFly _flying;
		 GuardAbillity _guard;
		 MyWakeUp _wakeup;
		 EAttackCon _attack;

			*/
            #endregion

            GravitySet(status.firstGravity);

            _characterHorizontalMovement.WalkSpeed = status.walkSpeed;
            if (_characterRun != null)
            {
                _characterRun.RunSpeed = status.dashSpeed;
            }



            if (_jump != null)
            {
                _jump.CoyoteTime = status.jumpCool;
                _jump.JumpHeight = status.jumpRes;
                _jump.NumberOfJumps = 2;

            }

            maxMp = status.maxMp;
            mp = maxMp;

            //MPの総量を設定
            mpStorage = status.mpStorage;

            _health.MaximumHealth = status.maxHp;
            _health.CurrentHealth = (int)maxHp;
        }


        #region 戦闘状態解除メソッド



        /// <summary>
        /// 戦闘終了の判断を行う
        /// 敵が二秒間いなければ警戒フェイズに移る
        /// この時点で攻撃は中止しよう。Fire編集時に攻撃中止メソッドを作る
        /// </summary>
        private async UniTaskVoid BattleEndJudge()
        {
            //一秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken:moveJudgeCancel.Token) ;

            //敵がゼロになるか
            //最短距離にいる敵との距離が戦闘終了距離以遠になったら
            if (BattleEndCondition())
            {
                //さらに三秒待って
                await UniTask.Delay(TimeSpan.FromSeconds(3f),cancellationToken: moveJudgeCancel.Token);

                //それでも条件を満たしていたら
                if (BattleEndCondition())
                {
                    //警戒状態へ
                    StateChange(SisterState.警戒);
                    return;
                }
            }

            //そうでなければ再帰呼び出し
            BattleEndJudge().Forget();
        }

        /// <summary>
        /// 戦闘終了になる条件
        /// 最も近い距離にいる敵が戦闘維持距離の外にいるか、あるいは敵リストが空になったなら
        /// </summary>
        /// <returns></returns>
        bool BattleEndCondition()
        {
            return !SManager.instance._targetList.Any() ||
                 Vector2.SqrMagnitude(SManager.instance.sisterPosition - SManager.instance.GetClosestEnemy()._condition.targetPosition) < Mathf.Pow(status.battleEndDistance, 2);
        }


        #endregion


        #region 警戒状態解除メソッド



        /// <summary>
        /// 警戒状態の終了待ちを行う
        /// 既定の時間待って、その後は警戒すべきオブジェクトがある限り10秒延長
        /// これとは関係ないけど警戒オブジェクトの中身で会話の内容変わってもいいかも
        /// </summary>
        private async UniTaskVoid PatrolEndJudge(bool isFirst)
        {
            if (isFirst)
            {
                //警戒状態の持続時間分待つ
                await UniTask.Delay(TimeSpan.FromSeconds(patrolTime), cancellationToken: moveJudgeCancel.Token);
            }



            //警戒すべき環境オブジェクトがないなら
            if (eventObject == null)
            {

                StateChange(SisterState.のんびり);
                return;

            }
            //まだあるなら
            else
            {
                //さらに10秒待って
                await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: moveJudgeCancel.Token);
            }

            //再帰呼び出し
            PatrolEndJudge(false).Forget();
        }


        #endregion

        #endregion





        ///保留点
        ///
        ///・
        ///・fireAbillityでターゲットにするのは周辺センサーで獲得した敵だけにするか
        ///・fireAbillityとの連携
        ///・一回行動するまでは再移動しない？
        ///・ヘルス関連処理改善するかぁ？


        ///実装要素
        /// 
        /// ・ターゲット判断　○
        /// ・移動判断　〇
        /// ・ワープ移動判断　○
        /// ・ダメージイベント　〇
        /// ・壁イベント　〇
        /// ・移動完了後イベント　〇
        /// ・移動中断イベント　〇
        /// 
        /// 
        /// ・敵発見時の行動　
        /// 

        ///　流れのまとめ
        ///
        ///まず最初に移動判断
        ///移動判断から移動（ワープか徒歩）
        ///移動中は中断条件と壁激突イベントを判断
        ///移動後は周囲の状況から再移動するかを判断できる
        ///移動後は攻撃などする。この時ダメージを受けるとダメージイベントを受ける
        ///再移動条件を満たすかダメージイベントで再移動する

        ///移動中に使うイベント
        ///・中断
        ///・壁（穴）激突
        ///・再移動条件

        ///移動後に使うやつ
        ///・再移動判断
        ///・ダメージイベント

        ///悩んでること
        ///・最初はどういう風に処理をスタートさせるか。いきなり移動判断開始で、移動後から攻撃を始める？
        ///・ポジションについたのをどう表現するか。enumの状態かフラグか
        ///・移動終了後の処理



        #region 戦闘移動ステータス関連

        #region 移動判断


        //ダメージあとの判断との兼ね合いはダメージ判断開始時に非同期キャンセルが丸い
        //

        /// <summary>
        /// 行動するかの判断をして、立ち位置や移動方法を決めるメソッド
        /// 最初は無条件で通る
        /// 移動中は呼ばれないようにする？
        /// </summary>
        /// <param name="isFirst"></param>
        async UniTaskVoid CombatMoveJudge(bool isFirst)
        {

            //最初じゃないなら条件判断
            //最初だけは条件関係なく位置取りを開始する
            if (!isFirst)
            {
                //状態ごとに決めた秒数待つ
                await UniTask.Delay(TimeSpan.FromSeconds(moveStatus.judgePace));


                //判断で偽なら戻る
                if (!JudgeMoveStart(nowTarget, moveStatus.rejudgeCondition, moveStatus.orCondition))
                {
                    //再帰呼び出し
                    CombatMoveJudge(false).Forget();
                    return;
                }
            }

            //移動挟むとダメージリセット
            totalDamage = 0;


            //条件クリアしたら次のターゲットを設定
            nowTarget = TargetSelect(moveStatus.markTarget);


            //ここから移動先の場所を決める
            //オプションと距離で決めて、移動開始したら範囲で待つ
            //x軸の位置決定
            float standPosition = StandPositionSet(nowTarget._condition.targetPosition.x,moveStatus.keepDistance,moveStatus.moveOption);


            //ワープするかどうか
            //これが真なら移動メソッドではなくワープ移動を呼び出す？
            //ワープ処理はまたいろいろ考える。モノにぶつかったり落ちたりしないように気を付けないと
            bool useWarp = false;

            //ワープするかどうか診断
            //ワープ使うなら判断
            if (moveStatus.warpCondition.condition != RejudgeCondition.判定しない)
            {
                //ワープするかどうか
                useWarp = MoveStartJudgeExe(nowTarget,moveStatus.warpCondition);
            }

            //停止中だと魔法が使えちゃうので別のにする
            //これでFireAbillityをロック
            nowMove = MoveState.最初;

            //ワープ使うならワープメソッド
            if (useWarp)
            {
                WarpMoveAct(standPosition);
            }
            //使わないなら通常移動
            else
            {
                NormalCombatMove(standPosition, moveStatus.MoveStopCondition,false,true).Forget();
            }

        }


        #region ターゲットを選ぶ

        /// <summary>
        /// 移動の基準にするターゲットを選ぶ
        /// 各条件に従い標的のデータを取得する
        /// </summary>
        /// <returns></returns>
        TargetData TargetSelect(MarkCharacterCondition condition)
        {
            if(condition == MarkCharacterCondition.プレイヤー)
            {
                return SManager.instance._targetList[0];
            }
            else if (condition == MarkCharacterCondition.一番近い敵)
            {
                return SManager.instance.GetClosestEnemy();
            }
            else if (condition == MarkCharacterCondition.一番遠い敵)
            {
                //遠いやつを返してもらう
                return SManager.instance.GetClosestEnemy(true);
            }
            else if (condition == MarkCharacterCondition.ランダムな敵)
            {
                int count = SManager.instance._targetList.Count;

                //ランダムな敵を
               return SManager.instance._targetList[_fire.RandomValue(0, count - 1)];

            }
            else if (condition == MarkCharacterCondition.強敵)
            {
                return SManager.instance.GetStrongestEnemy();
            }
            else if (condition == MarkCharacterCondition.一番高いところにいる敵)
            {
                return SManager.instance.GetHighestEnemy();
            }

            return null;
        }


        #endregion


        #region 立ち位置再判断条件の判断

        /// <summary>
        /// 移動開始を判断する
        /// 
        /// 必要なのは
        /// ・敵データ
        /// ・判断条件の配列
        /// ・Or条件かどうかのbool
        /// 
        /// ここではあくまで動くか判断するだけ
        /// それだけなので移動条件とかはいらない
        /// あと標的の距離が絡む条件があったらその敵死亡したら再判断するイベントかフラグ飛ばさないとね
        /// </summary>
        bool JudgeMoveStart(TargetData target, RejudgeStruct[] condition,bool isOrJudge)
        {
            //ステータスを設定
            moveStatus = parameter.sisterMoveSetting.AttackMoveSetting;

            //100じゃないならパーセント判断してね
            if ( condition[0].percentage != 100)
            {
                //確率検査は開幕で頼む
                if (condition[0].percentage > _fire.RandomValue(0,100))
                {
                    return false;
                }
            }

            //一個でも適合する条件があったよってこと
            bool isMatch = false;

            for(int i=0; i< 3; i++)
            {
                //条件判断
                if (MoveStartJudgeExe(target, condition[i]))
                {
                    isMatch = true;
                }
                //もしand条件が偽ならその時点でサヨナラ
                else if (!isOrJudge)
                {
                    return false;
                }

            }

            //for文くぐったあと一個でも合致する条件あったなら真
            return isMatch;

        }


        /// <summary>
        /// 実際の判断を行う
        /// </summary>
        bool MoveStartJudgeExe(TargetData target, RejudgeStruct condition)
        {
            if (condition.condition == RejudgeCondition.基準の距離から外れた時)
            {

                //ターゲットいないなら真を返してね
                if(nowTarget == null)
                {
                    return true;
                }

                //ここでパラメーターから維持距離を引っ張ってくる
                float baseDistance = moveStatus.keepDistance * moveStatus.keepDistance;

                float distance = Vector2.SqrMagnitude(SManager.instance.sisterPosition - target._condition.targetPosition);

                //パーセンテージがゼロ以上なら距離が以上離れた時
                //以下なら以内に入った時
                 return condition.value > 0 ? distance > baseDistance : baseDistance > distance ;
            }
            else if (condition.condition == RejudgeCondition.敵に狙われた時)
            {
                //シスターさんを狙ってる敵がいる
                return EnemyManager.instance._targetList[1].targetAllCount > 0;
            }

            //これは敵数変動イベントをうまく使ってできないか
            //敵の数変わったフラグが二秒間真になってるとか
            //戦闘マネージャーにやらせてもいいかも
            else if (condition.condition == RejudgeCondition.敵の数が変動)
            {

            }
            else if (condition.condition == RejudgeCondition.一定の距離に敵がいる時)
            {
                //ここでパラメーターから維持距離を引っ張ってくる
                float baseDistance = condition.value * condition.value;

                float distance = Vector2.SqrMagnitude(SManager.instance.sisterPosition - target._condition.targetPosition);

                //パーセンテージがゼロ以上なら距離が以上離れた時
                //以下なら以内に入った時
                return condition.value > 0 ? distance > baseDistance : baseDistance > distance;
            }
            else if (condition.condition == RejudgeCondition.時間経過)
            {

                //再移動後に時間を記録してる
                //0以上なら以上、以下なら以下
                return condition.value > 0 ? totalTime >= condition.value : totalTime <= Mathf.Abs(condition.value);
            }
            else if (condition.condition == RejudgeCondition.周辺に敵が何体いるか)
            {
                //以上か以下かは値がゼロ以上かで決まる
                return condition.value > 0 ? _sensor.EnemyCount >= condition.value : _sensor.EnemyCount <= -condition.value;
            }
            else if (condition.condition == RejudgeCondition.自分のMPが指定の割合の時)
            {
                //以上か以下かは値がゼロ以上かで決まる
                return condition.value > 0 ? (mp / status.maxMp) > condition.value / 100 : (mp / status.maxMp) < Mathf.Abs(condition.value) / 100;
            }
            else if (condition.condition == RejudgeCondition.移動後に指定のダメージを受けた時)
            {
                //移動後のダメージを記録してるので判断
                return condition.value > 0 ?  totalDamage >= Mathf.Abs(condition.value) : totalDamage <= Mathf.Abs(condition.value);
            }

            return false;
        }


        #endregion


        #region 立ち位置決定


        /// <summary>
        /// 条件に従って立ち位置を決める
        /// 敵の位置から離れる距離を足した位置か引いた位置か
        /// どちらに行くかを教える
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="setDistance"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        float StandPositionSet(float xPosition,float setDistance, PositionJudgeOption condition)
        {

            //プラス
            float plusPosition = xPosition + setDistance;

            //マイナス
            float minusPosition = xPosition - setDistance;

            float usePosition = 0;

            //これが真ならマイナスの方を返す
            //すなわち左
            bool isLeft = false;

            //近い方に行く
            if(condition == PositionJudgeOption.オプション無し)
            {
                usePosition = SManager.instance.sisterPosition.x;
                //マイナスの方が距離が近い（数値が小さい）ならマイナスを渡す
                isLeft = Mathf.Abs(plusPosition - usePosition) > Mathf.Abs(minusPosition - usePosition);

            }
            else if(condition == PositionJudgeOption.プレイヤーの近くの位置に行く)
            {
                usePosition = GManager.instance.PlayerPosition.x;

                //マイナスの方が距離が近い（数値が小さい）ならマイナスを渡す
                isLeft = Mathf.Abs(plusPosition - usePosition) > Mathf.Abs(minusPosition - usePosition);
            }
            else if (condition == PositionJudgeOption.プレイヤーの遠くの位置に行く)
            {
                usePosition = GManager.instance.PlayerPosition.x;

                //プラスの方が距離が近い（数値が小さい）ならマイナスを渡す。遠くだから
                isLeft = Mathf.Abs(plusPosition - usePosition) < Mathf.Abs(minusPosition - usePosition);
            }
            else if (condition == PositionJudgeOption.基準キャラの背後に回る)
            {
                //右向いてるなら左（マイナス）に行く
                isLeft = nowTarget.targetObj.transform.localScale.x > 0;
            }
            else if (condition == PositionJudgeOption.基準キャラの正面に立つ)
            {
                //左向いてるなら左（マイナス）に行く
                isLeft = nowTarget.targetObj.transform.localScale.x < 0;
            }
            else if (condition == PositionJudgeOption.基準キャラから敵が多い方に行く)
            {
                isLeft = SManager.instance.MoreEnemySide(xPosition);
            }
            else if (condition == PositionJudgeOption.基準キャラから敵が少ない方に行く)
            {
                isLeft = !SManager.instance.MoreEnemySide(xPosition);
            }
            else if (condition == PositionJudgeOption.壁が近い方に行く)
            {
                //左の方が壁が近いか
                isLeft = WallDistanceCheck();
            }
            else if(condition == PositionJudgeOption.壁が遠い方に行く)
            {
                //左の方が壁が遠いか
                isLeft = !WallDistanceCheck();
            }

            //左なら引いた距離、右なら足した距離
            return isLeft ? minusPosition : plusPosition;

        }


        /// <summary>
        /// 左右どちらに移動すると壁が近いかを返す
        /// 真を返す時左の方が近い
        /// レイキャストを使って左右の壁を探す
        /// </summary>
        /// <returns></returns>
        bool WallDistanceCheck()
        {
            Vector2 basePosition = SManager.instance.sisterPosition;
            
            //右の壁までの距離、一応最初は一万入れておく
            float distanceR = 10000f;


            //左の壁までの距離、一応最初は一万入れておく
            float distanceL = 10000;

            //右の壁を検知
            RaycastHit2D result = Physics2D.Raycast(basePosition, Vector2.right, Mathf.Infinity, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

            //仮に右に壁があったらその距離を格納する
            if (result.collider != null)
            {
                distanceR = Mathf.Abs(result.point.x - basePosition.x);
            }

            //左の壁を検知
            result = Physics2D.Raycast(basePosition, Vector2.left, Mathf.Infinity, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

            //仮に左に壁があったらその距離を格納する
            if (result.collider != null)
            {
                distanceL = Mathf.Abs(result.point.x - basePosition.x);
            }


            //左の方が右より壁への距離が短ければ真
            return distanceL < distanceR;

        }


        #endregion

        #endregion

        #region 追加条件関連



        #region ダメージ受けた時のイベント


        /// <summary>
        /// ダメージを受けた時、ダメージイベントを発動するかを判断する
        /// ダメージイベント
        /// </summary>
        /// <param name="isBack"></param>
        void DamageEventController(bool isBack)
        {

            //真も偽もそのまま戦うなら処理しない
            //ついでに今が移動中なら呼ばないようにもすべき
            if(moveStatus.damageFalseEvent == DamageMoveEvent.そのまま戦う && moveStatus.damageTrueEvent == DamageMoveEvent.そのまま戦う)
            {
                return;
            }

            //背後攻撃の場合だけisBackで見る
            bool isTrue = (moveStatus.damageCondition.condition != RejudgeCondition.背後からの攻撃を受けた時) ? MoveStartJudgeExe(nowTarget,moveStatus.damageCondition) : isBack;

            //パーセンテージが100以下で条件真なら確率チェックを
            if (isTrue && moveStatus.damageCondition.percentage < 100)
            {
                isTrue = moveStatus.damageCondition.percentage < _fire.RandomValue(0, 100);
            }

            DamageMoveEvent useEvent = isTrue ? moveStatus.damageTrueEvent : moveStatus.damageFalseEvent;

            //使用するイベントがそのまま戦うならリターン
            if(useEvent == DamageMoveEvent.そのまま戦う)
            {
                return;
            }

            //それ以外なら今の移動判断をキャンセルして
            //キャンセル
            moveJudgeCancel.Cancel();
            moveJudgeCancel = new CancellationTokenSource();

            //被弾イベントの実行を
            DamageEventExe(useEvent);
        }


        /// <summary>
        /// ダメージイベントの実行
        /// 移動系は行先を決めてワープするかも決めてから移動実行するだけ
        /// </summary>
        /// <param name="useEvent"></param>
        void DamageEventExe(DamageMoveEvent useEvent)
        {

            var eventNum = (int)useEvent;

            //ここで移動地点とワープするかを決めて動かす
            if (eventNum <= (int)DamageMoveEvent.緊急逃走ワープ)
            {

                float escapePoint = EscapePointSerch(false);

                //ワープと同じ番号ならワープ
                if(eventNum == (int)DamageMoveEvent.緊急逃走ワープ)
                {
                    WarpMoveAct(escapePoint);
                }
                //ワープじゃないなら
                else
                {
                    NormalCombatMove(escapePoint,moveStatus.MoveStopCondition,true,true).Forget();
                }

            }
            if (eventNum <= (int)DamageMoveEvent.プレイヤーのもとに緊急ワープ)
            {
                //プレイヤーの位置に逃げる
                float escapePoint = GManager.instance.PlayerPosition.x;

                //ワープと同じ番号ならワープ
                if (eventNum == (int)DamageMoveEvent.プレイヤーのもとに緊急ワープ)
                {
                    WarpMoveAct(escapePoint);
                }
                //ワープじゃないなら
                else
                {
                    NormalCombatMove(escapePoint, moveStatus.MoveStopCondition, true, true).Forget();
                }
            }
            else if (eventNum <= (int)DamageMoveEvent.ランダムにモードチェンジ)
            {
                //ランダムとそれ以外で分けるよ
                if(eventNum == (int)DamageMoveEvent.ランダムにモードチェンジ)
                {

                }
                else
                {

                }

            }
            else if (useEvent == DamageMoveEvent.再移動)
            {
                //ターゲット決め直して移動
                //条件満たしてるかとかは関係なく動くのでfirstは真で
                CombatMoveJudge(true).Forget();
            }
        }


        /// <summary>
        /// 逃走する場所を探す
        /// x座標を返すメソッド
        /// 
        /// どちらに逃げると敵が少ないかとか、どちらに逃げると壁が遠いかとかあると思うんだよね
        /// 壁が両方なかったら敵数判断するか？
        /// そんで、周囲の敵数調べてまだいるならもう一度だけ逃げるとか
        /// 
        /// ReverseEscapeが真なら今向いてる方向の逆に逃げていく
        /// </summary>
        float EscapePointSerch(bool reverseEscape)
        {
                float basePosition = SManager.instance.sisterPosition.x;
            
            //逆に逃げる指定ないなら
            if (!reverseEscape)
            {
                bool isRight = SManager.instance.MoreEnemySide(basePosition);

                //敵が左に多いなら現在地点より右に行く
                return basePosition + (isRight ? (moveStatus.keepDistance * 1.2f) : (-moveStatus.keepDistance * 1.2f));
            }
            //あるなら
            else
            {
                //右向いてるなら左へ、左向いてるなら右へ
                return basePosition + (_character.IsFacingRight ? (-moveStatus.keepDistance * 1.2f) : (moveStatus.keepDistance * 1.2f));
            }
        }



        #endregion


        #region 壁にぶつかった時のイベント


        /// <summary>
        /// 壁にぶつかった時のイベントを受け取る
        /// 移動中のみ判別する
        /// </summary>
        /// <param name="isReverse">すでに逆側に移動中であるか</param>
        void WallCollisionEventJudge(bool isEscape, bool isReverse)
        {
            //条件に合うかを見る
            bool isMatch;

            if (moveStatus.wallCondition.condition == RejudgeCondition.逃走中であるか)
            {
                isMatch = isEscape;
            }
            else
            {
                isMatch = MoveStartJudgeExe(nowTarget, moveStatus.wallCondition);
            }


            //使うイベントを見る
            WallCollisionEvent useEvent = isMatch ? moveStatus.trueWallEvent : moveStatus.falseWallEvent;


            //壁のぼりするなら
            if (isMatch ? moveStatus.trueWallClimb : moveStatus.falseWallClimb)
            {
                //壁のぼりできるか調べて
                if (WallClimbCheck())
                {
                    //壁のぼり
                    //壁のぼる();

                    //そして戻る
                    return;
                }
            }

            //壁のぼりしないなら壁イベント
            WallCollisionEventExe(useEvent, isEscape, isReverse);
        }


        /// <summary>
        /// 壁衝突イベントの実行
        /// 
        /// 
        ///  停止,反対側に移動,反対側にワープ,登れるなら壁によじ登る
        /// </summary>
        /// <param name="useEvent"></param>
        /// <param name="isReverse">すでに逆側に移動中であるか。延々とワープとか移動繰り返したくないよね</param>
        void WallCollisionEventExe(WallCollisionEvent useEvent,bool isEscape,bool isReverse)
        {
            if(useEvent == WallCollisionEvent.停止)
            {
                //移動終了処理
                return;
            }
            else if((int)useEvent <= (int)WallCollisionEvent.反対側にワープ)
            {
                //すでに反対側に移動中なら停止処理
                if (isReverse)
                {
                    //移動停止処理

                    return;
                }

                //今の自分と基準敵の立ち位置をベースに逆側に行けるように目的地を決める
                //そして移動を選択する
                //また、逃走中はリバースを真にして位置を返してもらう

                float standPosition;
                if (isEscape)
                {
                    standPosition = EscapePointSerch(true);
                }
                else
                {
                    standPosition = nowTarget._condition.targetPosition.x;

                    //自分が基準キャラより左にいるなら右に向かう
                    standPosition += (SManager.instance.sisterPosition.x < standPosition) ? moveStatus.keepDistance : -moveStatus.keepDistance;
                }

                if(useEvent == WallCollisionEvent.反対側にワープ)
                {
                    WarpMoveAct(standPosition);
                }
                else
                {
                    //戻っていってるからisReverseは真
                    NormalCombatMove(standPosition,moveStatus.MoveStopCondition,isEscape,isReverse = true).Forget();
                }

            }

        }


        /// <summary>
        /// 壁のぼりできるかのチェック
        /// </summary>
        bool WallClimbCheck()
        {
            return false;
        }


        /// <summary>
        /// 穴か壁があるかを調べる
        /// 穴と壁を調べた後の行動は逃走中と普通の移動時で変える？
        /// </summary>
        protected virtual bool CheckWallAndHole()
        {
            // 地面についている時だけ
            //ついてないなら戻る
            if (!_controller.State.IsGrounded)
            {
                return false;
            }


            //なにか壁にぶつかってるなら
            if(_controller.CurrentWallCollider != null)
            {
                return true;
            }


            // レイキャストを発射する位置を決める
            //自分より少し前からレイキャストをする
            if(_character.IsFacingRight)
            {
                //右向いてるなら少し右に
                holeCheckPoint.Set(SManager.instance.sisterPosition.x + (_controller.Bounds.x / 2 + 20f), (SManager.instance.sisterPosition.y));
            }
            else
            {
                holeCheckPoint.Set(SManager.instance.sisterPosition.x - (_controller.Bounds.x / 2 + 20f), (SManager.instance.sisterPosition.y));
            }

            //レイが何も当たらなければ穴がある
            return Physics2D.Raycast(holeCheckPoint, Vector2.down, groundCheckLength, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask).collider == null;


        }

        #endregion








        #endregion

        #endregion




        ///　
        /// 
        /// ・崖とか穴はジャンプできないところは壁と同じ扱いにする
        /// ・ダメージイベントの逃走やワープは移動後そのまま戦い始める
        /// 
        /// ///

        #region 戦闘移動実行処理


        #region 通常移動


        ///<summary>
        ///ワープじゃないふつうの移動
        ///一度呼び出したら勝手に動作する
        ///戦闘位置、あるいは逃走などで目指す場所に走る
        ///逃走の場合は停止条件はなし
        /// </summary>
        ///<param name="standPosition">目標の位置</param>
        ///<param name="stopCondition">停止する条件</param>
        ///<param name="isEscape">逃走しているかどうか。壁に当たった時の条件が判断されない</param>
        ///<param name="isReverse"></param>
        ///<param name="isFirst"></param>
        async UniTaskVoid NormalCombatMove(float standPosition,RejudgeStruct stopCondition,bool isEscape,bool isReverse,bool isFirst = false)
        {
            //最初じゃないなら
            if (!isFirst)
            {
                //0.3ごとに
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: moveJudgeCancel.Token);
            }

            //目的地への距離
            float distance = (standPosition - SManager.instance.sisterPosition.x);


            //停止条件満たしてないならダッシュ
            if (!StopConditionJudge(distance, moveStatus.adjustRange,stopCondition))
            {  

                //穴見つけたり壁にぶつかったりしたら
                if (CheckWallAndHole())
                {
                    //ひとまず停止
                    MoveAct(MoveState.停止,0);


                        WallCollisionEventJudge(isEscape,isReverse);

                    return;
                }


                //目指すポイントが右なら1で移動
                MoveAct(MoveState.走り, direction: distance >= 0 ? 1 : -1);

                //再帰呼び出し
                NormalCombatMove(standPosition,stopCondition,isReverse,isEscape).Forget();
            }
            //満たしたなら
            else
            {

                //移動終了
                CombatMoveEnd();
            }


        }

        #endregion


        #region ワープ処理

        /// <summary>
        /// 戦闘位置、あるいは目指す場所にワープ
        /// ワープ移動を実行する
        /// 
        /// ワープしてエフェクトを出す
        /// 壁衝突検知は通常移動中にしか判断されないから壁際にワープしても安心
        /// </summary>
        void WarpMoveAct(float standPosition)
        {

            //ワープ実行
            PointWarpExe(WarpPointSerch(standPosition));

            //移動終了
            CombatMoveEnd();
        }

        /// <summary>
        /// ワープの位置探索メソッド
        /// 途中で壁(Wallタグとか？)にぶつかったらもうそちら側は探索しない
        /// ワープ地点見つけられなかったら戻る？
        /// あと、もともと自分がいた方向（右にワープなら左方向）は元の位置に戻るまで探索する？
        /// 
        /// レイキャストセンサー使ってレイの通り道に何があるのかを調べるのはもう相当あり
        /// 壁にぶつかったら壁の位置で止まればよくない？
        /// 壁だけを検出するレイセンサー飛ばして、壁にぶつかるかを調べる
        /// 壁にぶつかったらそこで止まる
        /// 壁があるかを調べる…レイセンサーいらなくない？
        /// キャストでいいか、地面はどうしよう
        /// 着地地点にadjust足した範囲に壁があるかを調べる
        /// そして壁がない範囲で地面がある場所を探す
        /// 
        /// 追加でアイデアだけど、壁にぶつかった周囲の一定範囲にワープポイントがあるかを検索して、もしあるならそこに飛ぶ?
        /// とかするとでこぼこの段差でもいいとこに飛べるよね
        /// 
        /// いやそんなことしなくても現在地点から十ごとのy座標に一本ずつレイキャスト（目的地に調整分足した長さ）を飛ばす
        /// その三本線のライン上に壁にぶつからなくて、なおかつ30以内に地面があるポイントで一番目的地に近いモノを探す
        /// 三本とも壁にぶつからなかったらふつうに探す
        /// 
        /// じゃあまず四本放って、一番目的地に近いポイントで壁にぶつかる高度を探す
        /// 
        /// </summary>
        Vector2 WarpPointSerch(float standPosition)
        {
            ///暫定の一番近いポイント
            ///最初は今の座標を入れる
            Vector2 startPoint = SManager.instance.sisterPosition;

            Vector2 nearestpoint = startPoint;

            //現在地より右に目的地があるなら右を検査する
            bool isRight = (startPoint.x < standPosition);

            //レイキャストの長さ
            float checkRange = Math.Abs(startPoint.x - standPosition);

            RaycastHit2D result;

            //四回レイでチェックする
            //レイが壁にぶつからなかった時点で終わりでいい
            for (int i = 0; i < 4; i++)
            {
                result = Physics2D.Raycast(startPoint, (isRight ? Vector2.right : Vector2.left), checkRange, 0);//ちゃんと地形のマスク用意して、0の代わりに

                //何も当たらなかった場合
                //これで終わり
                if (result.collider == null)
                {
                    //現在判定中の座標を設定
                    nearestpoint.Set(standPosition, startPoint.y);
                    break;
                }
                //当たるものがあった場合
                else
                {
                    //仮に衝突地点の方が、暫定の地点より目的地への距離が短いなら
                    if (Math.Abs(standPosition - nearestpoint.x) > Math.Abs(standPosition - result.point.x))
                    {
                        //新しく近いポイントが更新される
                        nearestpoint.Set(result.point.x, startPoint.y);
                    }

                }

                //周回の最後には高度を足す
                //次はもう少し高いポイントで検査することで地形の凸凹にも対応
                //一周ごとに10足していく
                startPoint.MMSetY(startPoint.y + 10);

            }


            //次の検査のためにワープ予定地店への距離を10等分する
            checkRange = Math.Abs(nearestpoint.x - startPoint.x) / 10;


            //ここで出たnearestPointを元に、地面をチェックして最適なワープ地点を探す
            //見つからなければプレイヤーのもとに飛ぶ
            //ワープ予定地から現在地までの距離を十等分して検査する
            for (int i = 0; i < 10; i++)
            {
                //30下以内に地面があるかチェックする
                result = Physics2D.Raycast(nearestpoint, Vector2.down, 30, _controller.PlatformMask | _controller.MovingPlatformMask | _controller.OneWayPlatformMask | _controller.MovingOneWayPlatformMask);

                //地面がないなら次のポイントでチェックする
                if (result.collider == null)
                {
                    //次のポイントは目的地が右なら左に帰ってくるし、左なら右に戻る
                    nearestpoint.MMSetX(nearestpoint.x + (isRight ? -checkRange : checkRange));
                }
                //地形に当たったなら
                else
                {
                    return nearestpoint;
                }



            }
            //最後まで地面が見つからなかったらプレイヤーのもとに行く
            return GManager.instance.PlayerPosition;

        }


        /// <summary>
        /// 具体的に座標を指定してワープする処理
        /// モーション再生や待機なども含む
        /// </summary>
        /// <param name="point"></param>
        void PointWarpExe(Vector2 point)
        {


            //ワープ開始
            //移動にロック
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            //移動も停止
            _characterHorizontalMovement.SetHorizontalMove(0);

            //一瞬停止
            _controller.SetForce(Vector2.zero);
            
            //ここでアニメーション再生
            //同時に、エフェクトを呼ぶアニメーションイベントによりエフェクトを呼ばれる


            //そして移動
            //これはアニメイベントにするか？
            transform.position = point;


            //で、ワープアニメが終わって、地面の上にいるなら状態をIdleに戻すような待機メソッドを
        }




        #endregion

        #region 移動終了後処理


        /// <summary>
        /// 戦闘中の移動、ワープと徒歩両方の後に呼ぶ
        /// </summary>
        void CombatMoveEnd()
        {
            //ひとまず停止
            _characterHorizontalMovement.SetHorizontalMove(0);
            _controller.SetHorizontalForce(0);

            //再移動条件を満たすならもう一度移動開始
            if (!MoveStartJudgeExe(nowTarget, moveStatus.RelocationCondition))
            {
                //条件関係なく移動開始するので真にする
                CombatMoveJudge(true).Forget();
                return;
            }


            //現在の時間を記録
            totalTime = GManager.instance.nowTime;
            //移動終了後に総ダメージもリセット
            totalDamage = 0;

            //移動終了したならここで魔法が使えるようにしていく
            
            //移動した後は現在の移動を停止にする
            nowMove = MoveState.停止;

            //基準のキャラが右にいるか左にいるかを出す
            int direction = (int)Mathf.Sign(nowTarget._condition.targetPosition.x - SManager.instance.sisterPosition.x);

            //基準のキャラへと振り向き
            SisFlip(direction);
        }


        #endregion


        #region 停止条件判断


        /// <summary>
        /// 停止条件があるかないかで判断を変える
        /// </summary>
        /// <param name="distance">現在の目的地との距離</param>
        /// <param name="adjust">この範囲以内なら目的地に着いたとする数値</param>
        /// <param name="stopCondition">停止条件</param>
        /// <returns></returns>
        bool StopConditionJudge(float distance, float adjust, RejudgeStruct stopCondition)
        {

            //停止条件がないなら距離を満たしたかを返す
            if (stopCondition.condition != RejudgeCondition.判定しない)
            {
                return (Mathf.Abs(distance) <= adjust);
            }
            //あるなら距離か停止条件か
            else
            {
                return (Mathf.Abs(distance) <= adjust) || MoveStartJudgeExe(nowTarget,stopCondition);
            }
        }


        #endregion


        #endregion



        #region 非戦闘時移動ステータス設定メソッド


        /// <summary>
        /// 非戦闘時はずっと回ってる
        /// 状態ごとの向かう座標設定とダッシュや停止などの移動方法を割り出す
        /// あとは方向なども
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid MoveController()
        {

            //0.3秒ごとに
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: moveJudgeCancel.Token);

            //目指す目標の位置
            float targetPosition;

            //行く方向
            int direction;

            //使用する移動方法
            MoveState useState;

            if (nowState == SisterState.のんびり)
            {
                //のんびりの時はターゲットが環境物とプレイヤーにわかれる
                //のでここで判断
                targetPosition = TargetPointJudge();

                useState = PlayMoveJudge(targetPosition);

            }
            //警戒
            else
            {
                //警戒時は常にプレイヤーの位置を
                targetPosition = GManager.instance.PlayerPosition.x;


                useState = PatrolMoveJudge(targetPosition);
            }

            //ターゲットの方が右にいるなら1を、そうでないなら-1を返す
            direction = (targetPosition - SManager.instance.sisterPosition.x) >= 0 ? 1 : -1;

            //移動の情報と方向を入れて移動実行
            MoveAct(useState, direction);

            //再帰呼び出し
            MoveController().Forget();

        }



        #region　警戒状態の移動ステータス算出

        /// <summary>
        /// ターゲットの位置から警戒移動中の移動方法などを決める
        /// ダッシュなど
        /// </summary>
        /// <returns></returns>
        MoveState PatrolMoveJudge(float targetPosition)
        {

            //ターゲットと自分の距離の差を出す

            float distance = Mathf.Abs(SManager.instance.sisterPosition.x - targetPosition);

            if (distance > status.walkDistance)
            {
                return MoveState.走り;
            }
            //近すぎる時は止まる
            else if (distance < 15)
            {
                return MoveState.停止;
            }
            //近くにいる時
            //警戒は基本ダッシュでwalkDistanceは使わない
            else// if (distance < status.patrolDistance)
            {

                float playerSpeed = pc.NowSpeed();



                    //同じ向きで、なおかつ走ってるなら
                    //プレイヤーと進行方向一致なら追いかける
                    if (((playerSpeed > 0 && _character.IsFacingRight) || (playerSpeed < 0 && !_character.IsFacingRight)) && Mathf.Abs(playerSpeed) >= 100)
                    {

                        //走ってるなら走りで追いかける
                            return MoveState.走り;

                    }
                    else  if (distance > status.patrolDistance)
                    {
                        return MoveState.歩き;
                    }
                    else
                    {
                        return MoveState.停止;
                    }


            }

        }

        #endregion

        #region のんびりの時の移動ステータス算出

        /// <summary>
        /// のんびりの時の移動判断
        /// 基本はプレイヤーに追従
        /// 環境物があったら近寄る
        /// でも、プレイヤーの位置が一定以上離れたら環境物から離れてプレイヤーのとこに走る
        /// 
        /// </summary>
        /// <returns></returns>
        MoveState PlayMoveJudge(float targetPosition)
        {

            float distance = Mathf.Abs(targetPosition - SManager.instance.sisterPosition.x);


            //停止状態ならまた走り出すには結構離れないといけないので
            //処理分ける
            //止まってるっていうことは十分に接近したということだから
            if(nowMove == MoveState.停止)
            {
                //一度止まったら遊んでいい範囲を出るまで
                //止まったまま
                if (distance < status.playDistance)
                {
                    return MoveState.停止;
                }
                //遊んでいい距離でたら走る
                else
                {
                    return MoveState.走り;

                }
            }

            else
            {
                //近くに寄ったら止まるか歩くか
                if (distance <= status.patrolDistance + status.adjust)
                {
                        return MoveState.停止;

                }
                //遊んでていい距離の中で、プレイヤーにくっついてないとき
                else if (distance <= status.walkDistance)
                {
                    return MoveState.歩き;

                }
                //歩くべき距離から離れたら
                else
                {
                    return MoveState.走り;

                }
            }


        }


        /// <summary>
        /// ターゲット選択して位置を返すメソッド
        /// 
        /// 環境物があったら近寄る
        /// でも、プレイヤーの位置が一定以上離れたら環境物から離れてプレイヤーのとこに走る
        /// </summary>
        /// <returns></returns>
        float TargetPointJudge()
        {

            //プレイヤーとの距離
            float distance = Mathf.Abs(GManager.instance.PlayerPosition.x - SManager.instance.sisterPosition.x);


            //イベントオブジェクトがある時
            //そしてプレイヤーが近くにいる時（遠くだったらダメ）
            //遊んでいい距離以内にいるなら環境物の位置を返す
            if (eventObject != null && distance <= status.playDistance)
            {
                return eventObject.transform.position.x;
            }
            else
            {
                return GManager.instance.PlayerPosition.x;
            }
        }


        #endregion

        #endregion


        #region 共通移動処理



        /// <summary>
        /// 共通移動処理
        /// 等間隔で呼び出されて移動を変更する
        /// 
        /// 状態ごとに移動状態と方向をセットする
        /// 魔法使用中や連携使用時は停止する機能がある
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="direction"></param>
        void MoveAct(MoveState condition,int direction)
        {

            //通常時以外、そして地に足ついていなければ停止になって戻る
            if(_condition.CurrentState != CharacterStates.CharacterConditions.Normal || !_controller.State.IsGrounded)
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
                _controller.SetHorizontalForce(0);

                //ここでは移動データは触らない。攻撃が暴発するかも
               // nowMove = MoveState.停止;
                return;
            }
                //振り向き
                SisFlip(direction);

            //停止時は止まった後方向転換する
           if(condition == MoveState.停止)
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
                _controller.SetHorizontalForce(0);
                //直立
                _movement.ChangeState(CharacterStates.MovementStates.Idle);

                //状態を更新
                nowMove = condition;

                return;
            }

            //方向変更
            _characterHorizontalMovement.SetHorizontalMove(direction);

            //前回とステートが同じなら方向だけ変えて戻る
            if (nowMove == condition)
            {

                  return;
            }

            if (condition == MoveState.歩き)
            {
                _characterRun.RunStart();
            }
            else if (condition == MoveState.走り)
            {
                if (nowMove == MoveState.走り)
                {
                    _characterRun.RunStop();
                }
            }
            //状態を更新
            nowMove = condition;

        }

        /// <summary>
        /// 振り向き処理
        /// </summary>
        /// <param name="dire"></param>
        void SisFlip(float dire)
        {

            //方向が右で右向いてるなら振り向かない
            //左向いてて方向が左ならやはり振り向かない
            //direが0なら戻る
            if ((dire > 0 && _character.IsFacingRight) || (dire < 0 && !_character.IsFacingRight)|| dire == 0)
            {
                return;
            }

            _character.Flip();
        }


        /// <summary>
        /// プレイヤーと離れた時、プレイヤーのもとにワープする
        /// 放置狩りを防いだり、取り残されないようにするためメソッド
        /// 
        /// モード変更でトークンが消えてしまう
        /// 戦闘モード開始と戦闘モード終了、いずれのモード変更でも呼ばれる
        /// </summary>
        async UniTaskVoid DistanceKeepWarp()
        {
            await UniTask.WaitUntil(()=> (Vector2.SqrMagnitude(GManager.instance.PlayerPosition - SManager.instance.sisterPosition)) > Mathf.Pow(status.warpDistance,2),
                cancellationToken: moveJudgeCancel.Token);

            //もし今通常状態じゃないなら
            if(_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                //通常状態になるまで待って
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal,cancellationToken: moveJudgeCancel.Token);
            }

            Vector2 warpPoint = GManager.instance.PlayerPosition;

            //プレイヤーの背中側に現れるように転移位置を調整
            warpPoint.MMSetX(pc.transform.localPosition.x > 0 ? warpPoint.x - 10 : warpPoint.x + 10);

            PointWarpExe(warpPoint);

        }

        #endregion











        public void JumpController()
        {

            //isVerticalは自分でオンオフする
            if (!_controller.State.IsGrounded && _movement.CurrentState == CharacterStates.MovementStates.Jumping)
            {
                if (!isVertical)
                {
                    _horizontalInput = Mathf.Sign(transform.localScale.x);
                }
                if (!disEnable && _jump.JumpEnableJudge() == true)
                {
                    _jump.JumpStart();
                }
            }
            else
            {
                isVertical = false;
            }

        }

        public void JumpStart()
        {
            if (!disEnable && _controller.State.IsGrounded)
            {
                _jump.JumpStart();
            }
        }



        /// <summary>
        /// レイヤー変更。Avoidなんかと使ってもいいが単体でも使える
        /// </summary>
        /// <param name="layerNumber"></param>
        public void SetLayer(int layerNumber)
        {

            this.gameObject.layer = layerNumber;

        }

        /// <summary>
        /// 重力設定
        /// </summary>
        /// <param name="gravity"></param>
        public void GravitySet(float gravity)
        {
            //rb.gravityScale = gravity;
            _controller.DefaultParameters.Gravity = gravity;
        }








        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {

                //トンネルの中をしゃがみトリガーで満たす
                if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
                {
                    _squat.Crouch();
                }
                //elseでこれしゃがみ解除あるぞ
                if (collision.tag == jumpTag && _controller.State.IsGrounded)
                {
                    //GetComponentはなるべくせぬように
                    if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        jumpTrigger = true;

                    }
                }

            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {
                //トンネルの中をしゃがみトリガーで満たす

                if (collision.tag == squatTag && _movement.CurrentState != CharacterStates.MovementStates.Crouching && _movement.CurrentState != CharacterStates.MovementStates.Crawling)
                {
                    _squat.Crouch();
                }
                if (collision.tag == jumpTag && _controller.State.IsGrounded)
                {
                    if (collision.gameObject.GetComponent<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        jumpTrigger = true;

                    }
                }

            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_controller.State.IsGrounded)
            {
                //トンネルの中をしゃがみトリガーで満たす
                if (collision.tag == squatTag && (_movement.CurrentState != CharacterStates.MovementStates.Crouching || _movement.CurrentState == CharacterStates.MovementStates.Crawling))
                {
                    _squat.ExitCrouch();
                }
                if (collision.tag == jumpTag)
                {
                    jumpTrigger = false;

                }
            }
        }









        public void WarpEffect()
        {
            Vector3 posi = new Vector3(transform.position.x, transform.position.y - 11, 40);

            //Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

            Addressables.InstantiateAsync("WarpCircle", posi, Quaternion.Euler(-98, 0, 0));//.Result;//発生位置をPlayer
            GManager.instance.PlaySound("Warp", transform.position);
        }



        bool CheckEnd(string Name)
        {

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || _animator.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
            {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。
                return true;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {   // 待機時間を作りたいならば、ここの値を大きくする。
                return true;
            }
            //AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

            ////Debug.Log($"アニメ終了");

            return false;

            // return !(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            //  (_currentStateName);
        }








        public void MPReset()
        {
            mp = maxMp;
        }




        /// <summary>
        /// 魔力回復
        /// 魔力の総量がある間は回復し続ける
        /// </summary>
        async UniTaskVoid MPRecover()
        {
            //MPの総量が尽きたらもう回復しない
            if (mpStorage <= 0)
            {
                //もしかしたら総量を回復させるアイテムとか作るかも
                //その時はここの処理は変える
                return;
            }

            //現在のMPが最大MP以上の時
            if (mp >= maxMp)
            {
                //mpが回復可能になるまで待つ
                await UniTask.WaitUntil(() => mp < maxMp, cancellationToken: moveJudgeCancel.Token);
            }

            //2.5秒に一回だけ回復
            await UniTask.Delay(TimeSpan.FromSeconds(2.5f), cancellationToken: moveJudgeCancel.Token);

            //行動してないとき魔力回復
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
            {

                //クールタイム中は魔力回復1.2倍？
                float recoverAmo = SManager.instance.sisStatus.mpRecover + SManager.instance.sisStatus.additionalRecover;

                mp += (recoverAmo);

                //回復した分総量からは減らす
                mpStorage -= recoverAmo;
            }

            //限界量は下回らないように
            if (mpStorage < 0)
            {
                mpStorage = 0;

            }

            //再帰呼び出し
            MPRecover().Forget();
        }


        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_stateParameterName, AnimatorControllerParameterType.Bool, out _stateAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //のんびり1、警戒2、戦い3
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _stateAnimationParameter, nowState != SisterState.のんびり, _character._animatorParameters);
        }


        #region センサーと環境物関連の処理


        /// <summary>
        /// 敵を見つけた時に呼び出すメソッド
        /// </summary>
        public override void FindEnemy()
        {
            //戦闘モードへ
            StateChange(SisterState.戦い);
        }


        /// <summary>
        /// 発見したオブジェクトを報告する
        /// </summary>
        /// <param name="isDanger">危険物かどうか</param>
        /// <param name="obj">見つけたもの</param>
        public override void ReportObject(bool isDanger, GameObject obj)
        {

            //現在警戒状態なら
            if(nowState == SisterState.警戒)
            {
                //警戒オブジェクト以外の報告は受け付けない
                if (!isDanger)
                {
                    eventObject = null;

                    return;
                }
            }

            //危険物を見つけた時は警戒モードに
            //警戒モードは一定時間何もないと自動解除
            if (isDanger)
            {
                StateChange(SisterState.警戒);
            }


            //nullが渡された場合は空っぽになる
            eventObject = obj;
        }

        #endregion



        #region コンバットマネージャーの連携

        /// <summary>
        /// 敵のターゲットリスト内の自分のデータを更新する
        /// _conditionは常に最新のデータを保つ
        /// ずっと呼び出される
        /// </summary>
        /// <param name="num"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TargetDataUpdate(int num)
        {

            EnemyManager.instance._targetList[num]._condition.targetPosition = SManager.instance.sisterPosition;
            EnemyManager.instance._targetList[num]._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            EnemyManager.instance._targetList[num]._condition.hpNum = _health.CurrentHealth;
            EnemyManager.instance._targetList[num]._condition.hpRatio = mp / status.maxMp;

            EnemyManager.instance._targetList[num]._condition.buf = false;
            EnemyManager.instance._targetList[num]._condition.de= false;
        }


        /// <summary>
        /// 自分のターゲットデータを敵のターゲットリストに送る
        /// このNPCのIDの扱いはどうしよう
        /// Id利用するようなイベントはプレイヤー側にはない
        /// </summary>
        public override void TargetDataAdd(int myId)
        {

            TargetData imfo = new TargetData();

            imfo._baseData = status._charaData;

            imfo._condition.targetPosition = SManager.instance.sisterPosition;
            imfo._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            imfo._condition.hpNum = _health.CurrentHealth;
            imfo._condition.hpRatio = mp / status.maxMp;

            imfo._condition.buffImfo = false;
            imfo._condition.debuffImfo = false;

            imfo.targetObj = this.gameObject;

            imfo.targetID = myId;

            EnemyManager.instance._targetList.Add(imfo);
        }




        /// <summary>
        /// ターゲットリストから削除されたエネミーを消し去る
        /// そしてヘイトリストやらを調整
        /// プレイヤーはなんか別の処理入れてもいいかもな
        /// あと敵の死を通知するメソッドとしても使える
        /// 敵数変動イベントはここでやる
        /// 
        /// </summary>
        /// <param name="deletEnemy"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TargetListChange(int deletEnemy,int deadID)
        {
            //ターゲットと一致したら再判断を行う
            if (nowTarget.targetID == deadID)
            {
                //キャンセル
                moveJudgeCancel.Cancel();
                moveJudgeCancel = new CancellationTokenSource();
                
                //強制的に再判断
                CombatMoveJudge(true).Forget();

                //トークンけしたので再呼び出し
                DistanceKeepWarp().Forget();
            }

            //さらに攻撃アビリティにも消滅した敵のIDを教えてあげる

        }


        /// <summary>
        /// 自分が持つIDを返す
        /// 味方間でイベントをやり取りする上で大事
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int ReturnID()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 味方が狙うターゲットを決定した際
        /// 指揮するイベントを持ってたら飛ばす
        /// コンバットマネージャーを通じて仲間に通達が行く
        /// でも敵側でしか実装しないかも
        /// </summary>
        public override void CommandEvent(EnemyStatus.TargetingEvent _event, int level, int targetNum, int commanderID)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 周囲の敵数をFireAbillityに教えてやる
        /// あちらから呼び出して使う
        /// </summary>
        /// <returns></returns>
        public int EnemyCountReport()
        {
            return _sensor.EnemyCount;
        }


        #endregion


        #region 被弾関連の処理

        /// <summary>
        /// このキャラがガードしてるかどうかを伝える
        /// 常にガードしてる
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool GuardReport()
        {
            return true;
        }



        public override void BuffCalc(FireBullet _fire)
        {
            throw new System.NotImplementedException();
        }





        /// <summary>
        /// パリィは出来ないキャラクター
        /// 一度だけジャストガード判定をまとう魔法とかあるなら
        /// パリィ判定は魔法によって出す可能性だけある
        /// そういうのを実装するときにまた決める
        /// それまでは空白でいい
        /// </summary>
        /// <param name="isBreake"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ParryStart(bool isBreake)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 常にガード中なのでこれがダメージ音になる
        /// 固有のバリア的なガード音を鳴らすようにする
        /// </summary>
        public override void GuardSound()
        {

        }


        /// <summary>
        /// これはスタン回復時にアーマーをマックスにするためのメソッド
        /// なのでスタンしないキャラだから呼ばれることはない
        /// </summary>
        public override void ArmorReset()
        {
            //空でいい
        }


        /// <summary>
        /// このキャラは基本的にスタンはしないので空っぽで良し
        /// </summary>
        /// <param name="stunState"></param>
        public override void StartStun(MyWakeUp.StunnType stunState)
        {

        }

        /// <summary>
        /// 攻撃を受けた時の魔力削り
        /// ショックに応じて魔力が減る
        /// </summary>
        /// <param name="shock"></param>
        /// <param name="isBlow"></param>
        /// <param name="isBack"></param>
        /// <returns></returns>
        public override MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack)
        {

            //吹き飛ばしか背後攻撃なら1.1倍の削りを受ける
            shock = isBlow || isBack ? shock * 1.1f : shock;

            //mpから半分カットしたショックを引く
            //ここのカット率はステータスで上下してもいいかも
            mp -= (shock/2);

            //0以下なら0に
            mp = mp < 0 ? 0 : mp;


            //スタンはしない
            return MyWakeUp.StunnType.notStunned;
        }


        /// <summary>
        /// 死以外はなし
        /// HPを見て確認してね
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetStunState()
        {
            //スタンはなし
            return 0;
        }

        /// <summary>
        /// シスターさんのアーマーは魔力
        /// 
        /// 魔法しか使わないしパリィされることはないので呼ばれることはない
        /// </summary>
        /// <returns></returns>
        public override bool ParryArmorJudge()
        {
            return false;
        }



        /// <summary>
        /// 空中ダウンを判定
        /// 常にfalse
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            return false;
        }



        /// <summary>
        /// 最終的なダメージを計算
        /// 常にガード状態
        /// </summary>

        public override void DamageCalc()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 防御力を計算
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DefCalc()
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// 死亡モーション開始
        /// </summary>
        /// <param name="stunnState"></param>
        public override void DeadMotionStart(MyWakeUp.StunnType stunnState)
        {

        }


        /// <summary>
        /// ダメージ受けた時のイベント
        /// 移動後の合計ダメージを加算、そして作戦の被弾イベントを起動
        /// </summary>
        /// <param name="isStunn"></param>
        /// <param name="enemy"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DamageEvent(bool isStunn, GameObject enemy,int damage,bool back)
        {
            //これポジションについた戦闘中だけにしてね
            if (nowState == SisterState.戦い && nowMove == MoveState.停止)
            {
                
                //トータルダメージを追加
                totalDamage += damage;

            }
            //戦闘中でなければ戦闘モードに
            else if (nowState != SisterState.戦い)
            {
                StateChange(SisterState.戦い);
            }

            //もしかしたらダメージも渡すかも
            //体力の何パーセントとか
            DamageEventController(back);
        }

        /// <summary>
        /// ダメージ受けたことを通知？
        /// </summary>
        /// <param name="isBack"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void HitReport(bool isBack)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// バリアが消えて一定時間消える
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Die()
        {
            throw new System.NotImplementedException();
        }

#endregion

    }
}
