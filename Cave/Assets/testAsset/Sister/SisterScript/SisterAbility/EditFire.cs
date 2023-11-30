using Cysharp.Threading.Tasks;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static CombatManager;
using static SisterConditionBase;

/// 攻撃中は場所変えないようにする。中断でエフェクト残る
/// あと角度これでいいか確認
/// z軸位置もな
/// 作戦に追加する要素
/// boolリストでMP節約とか先制攻撃とか個別につけさせるか
/// あるいはMP使用度みたいなのをintで作りチェックついてる箱で１、２とか数値変えるか
/// それか複数作戦持たせる？
/// 火炎放射は動かずホーミングでClosestEnemyをターゲットに
/// 敵側でトリガーが発生しない
/// それぞれの条件に魔法を入れる
/// なかったら判断して探す
/// 止まり木に触れてAIいじる画面に行くと一旦条件魔法を白紙にする
/// 
///
///改修必要な部分
///・Brainから呼ばれるイベントを追加するかも
///・攻撃の条件待機を積めてない。待機条件を発射段階で取得するためにuseNumはやっぱ持ってないとダメかも
///・MP判断や射程距離判断、アーマー判断をうまく詰めてない
///・射撃メソッドのランダム周りの使用は修正必要
///・連携アクションと仲良くするために頑張らないと。中断と再開の仕様なー
///・要修正とか未実装で検索したらダメなとこ出てくるよ
///
///

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// 普通に状況判断して動くためのスクリプト
    /// ワープ、攻撃、コンビネーション（各動作は分ける？）は別スクリプトに
    /// あるいはコンビネーションは主人公に持たせる？
    /// 流れとしては判断、詠唱、攻撃状態に遷移しモーション変化、アニメーションイベント、魔法使用、アニメーションイベントで終了
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/EditFire")]
    public class EditFire : MyAbillityBase
    {

        public override string HelpBoxText() { return "シスターさんの攻撃"; }


        // Animation parameters
        //0がなんもなし、１が魔法、２がコンビネーション、３が詠唱
        protected const string _actParameterName = "actType";
        protected int _actAnimationParameter;

        /// <summary>
        /// モーションを選ぶ
        /// </summary>
        protected const string _motionParameterName = "motionNum";
        protected int _motionAnimationParameter;

        //条件判断や攻撃のパラメーター
        public SisterParameter sister;

        /// <summary>
        /// シスターさんの基礎AI
        /// </summary>
        [HideInInspector]
        public BrainAbility sb;



        /// <summary>
        /// クールタイム待機のため専用のフラグ
        /// </summary>
        bool disEnable;




        /// <summary>
        /// 今の行動のクールタイムを記録
        /// </summary>
        float coolTime;

        /// <summary>
        /// 攻撃、回復、などの優先行動を入れ替える
        /// </summary>
        float stateJudge = 30;



        /// <summary>
        /// ターゲットや魔法の候補を格納するリスト
        /// </summary>
        List<int> candidateList = new List<int>();




        /// <summary>
        /// 何番目の詠唱なのか
        /// </summary>
        int actionNum;


        [SerializeField] CombinationAbility ca;


        /// <summary>
        /// これが真なら次の機会にステート変更する
        /// </summary>
        bool stateChange = false;



        //ビット演算でクールタイムを破棄する
        SisterConditionBase.SkipCondition skipCondition = SkipCondition.なし;

        public AtEffectCon atEf;



        //protected RewiredCorgiEngineInputManager _inputManager;


        //-------------------------------------------バフの数値

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



        /// <summary>
        /// 弾丸生成中にターゲットが消えてもいいように位置を覚えておく
        /// </summary>
        Vector2 _tgPosition;


        CancellationTokenSource magicCancel;


        /// <summary>
        /// ターゲッとの番号を保存
        /// </summary>
        int targetNum;

        /// <summary>
        /// 使用する魔法の番号を記録
        /// </summary>
        int useMagicNum;


        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            sister.nowMove = sister.priority;

            //Brainじゃないんですわ
            //インスペクタでやれ
    //        sb = _character.FindAbility<BrainAbility>();
      //      atEf = _character.FindAbility<AtEffectCon>();

            SManager.instance.MagicEffectSet(atEf);
        }




        //戦闘開始時や終了時に呼ばれて動くメソッド
        //ここから攻撃の判断などが呼ばれる
        //FireAbillityはあくまで攻撃を専門とするBrainの補助機能
        //なんでこれ以外のイベントは持たない
        //Brainが必要とするなら持つこともあるけど
        //でもこれをBrain側から呼ぶだけ
        #region　管理メソッド

        ///<sumary>
        /// 状態ごとの初期化項目
        /// Brainが戦闘開始、終了の判断を下したときとかに呼ばれる
        /// じゃあここでターゲット判断開始とかトークンの初期化とか入れるか
        /// </sumary>
        public void StateInitialize(bool battle)
        {
            //戦闘開始
            if (battle)
            {

                //処理停止してトークン入れ替え
                magicCancel.Cancel();
                magicCancel = new CancellationTokenSource();

                //道中回復などの詠唱中ならエフェクトを消す
                if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
                {
                    atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                }

                //クールタイムの終了
                disEnable = false;

                //優先状態に状態変化
                SisterStateChange((int)sister.priority);
                //ステート管理開始
                StateController().Forget();


                //戦闘判断開始
                CombatMoveJudge().Forget();
            }
            //非戦闘
            //回復判断
            else
            {

                //処理停止してトークン入れ替え
                magicCancel.Cancel();
                magicCancel = new CancellationTokenSource();

                //回復に行動変化
                SisterStateChange((int)SisterParameter.MoveType.回復);

                //詠唱中ならエフェクトを消す
                if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
                {
                    atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

                }

                //変数の初期化
                stateJudge = 0;
                coolTime = 0;
                disEnable = false;

                if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Idle);
                    _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
                }

                //道中回復判断開始
                HealingJudge().Forget();


            }
        }

        #endregion








        //新魔法処理の流れ
        //全体的に共通化して一本の流れになってる
        //戦闘開始時にステート管理メソッドとタゲと魔法設定メソッドを呼び出す
        //そしてまずターゲットと使う魔法を設定するメソッドを呼び出す
        //それが通ったら詠唱、必要なら待機、そして魔法使用
        //通らなかったらn秒後に再判断というような感じにするか？
        //await n秒、真なら進行、偽なら再帰呼び出し繰り返し
        //そして怯んだり中断されたりしたら判断せずにawaitで状態が元に戻るのを待たせる？　await state.normal的な
        //クールタイム時(disenable)は最初のターゲット絞り込み条件判断の内容を変える
        //完成したら色々形整えようか
        //あと平常時の回復はどうしよ
        //少し時間待ち→判断→詠唱→攻撃→クールタイム待つ→判断　　のループ
        //クールタイムと少し時間待ちは結合
        //問題は連携をハサミこんだ時だね





        //流れおさらい
        //攻撃可能かを確認してから標的と魔法決定
        //mpチェック、詠唱開始
        //攻撃(ここに待機処理挟むかも)
        //終了後ステートチェンジ判定
        #region 新攻撃メソッド


        #region 詠唱と攻撃

        /// <summary>
        /// 詠唱管理メソッド
        /// 詠唱完了したら攻撃状態に入ってモーション遷移
        /// </summary>
        /// <param name="random"></param>
        public async UniTaskVoid CastMagic()
        {
            
            //ここまできたらdisenableをfalseにして前のクールタイム待ちを終了しておく
            //disenがfalseになると止まるからさ
            if (disEnable)
            {

                disEnable = false;
            }


            //魔法とターゲットのデータを獲得
            SisMagic useMagic;
            TargetData target;


            //必要なデータを取得
              (target,useMagic) = NecessaryDataGet();

            //使用する魔法のMPがないか、標的がいないなら戻る
            if (target.targetObj == null || useMagic.useMP > sb.mp)
            {
                return;
            }

            //魔法陣呼び出し
            //詠唱中へと状態変化や停止、振り向き処理など
            CastCircleCall(useMagic,target._condition.targetPosition.x);


            //時間待つ
            //ここは攻撃されると中断するか、それとも断固祈るかとか決められるように
            await UniTask.Delay(TimeSpan.FromSeconds(useMagic.castTime), cancellationToken: magicCancel.Token);




            //詠唱終了
            atEf.CastEnd(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);

            //魔法発動呼び出し
            //攻撃魔法は詠唱状態で待機したりもするので
            //状態変化は攻撃メソッドに託す
            UseMagic(target, useMagic).Forget();
        }


        /// <summary>
        /// 実際に行動するメソッド
        /// ここではターゲット確認、待機、ランダムなどいろいろ弾の挙動制御
        /// 発射、距離確認、クールタイム開始、ステートチェンジまで行う
        /// 
        /// ランダム配置周りには手を入れる
        /// </summary>
        /// <param name="hRandom"></param>
        /// <param name="vRandom"></param>
        async UniTaskVoid UseMagic(TargetData target,SisMagic useMagic)
        {



            //(特殊アクションなどで)使用する魔法のMPがないか、標的がいないなら戻る
            if (target.targetObj == null || useMagic.useMP > sb.mp)
            {
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);

                actionNum = 0;
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                return;
            }



            //クールタイム待ち
            disEnable = true;
            //攻撃開始
            _movement.ChangeState(CharacterStates.MovementStates.Attack);
            //モーションセット
            actionNum = (int)useMagic.FireType;
            //魔力消費
            sb.mp -= useMagic.useMP;





            //発射地点セット

            Vector3 goFire;

            //敵の位置にサーチ攻撃するとき
            if (useMagic.isChaice)
            {
                goFire = _tgPosition;

            }
            //それ以外なら射出点から出す
            else
            {
                goFire = sb.firePosition.position;
            }


            //降り注ぐ系のやつなら射出角度を決める
            //マネージャーに入れてるのは弾から使えるように
            if (SManager.instance.useMagic._moveSt.fireType == SisMagic.FIREBULLET.RAIN)
            {
                //山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
                //位置をランダムにすれば角度はどうでもいい説もある
                SManager.instance.useAngle = GetAim(sb.firePosition.position, _tgPosition);

            }

            //一応定義
            Vector3 restoreFirePosi = Vector3.zero;

            //弾丸を出す
            //ここ色々改造する
            for (int i = 0; i < useMagic.bulletNumber; i++)
            {
                //一回目以外で待ち時間あるなら待つ
                if (i != 0 || SManager.instance.useMagic.delayTime > 0)
                {
                    //時間待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime), cancellationToken: magicCancel.Token);

                    //ランダムな位置に発生するとき
                    //ここで元のポジをおぼえておく
                    if (hRandom != 0 || vRandom != 0)
                    {
                        restoreFirePosi.Set(goFire.x, goFire.y, goFire.z);
                    }

                }
                //一回目以外は射出店がランダムかどうかの判断もする
                else
                {

                    //ランダムな位置に発生するとき
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

                        goFire.Set(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//銃口から
                    }

                }

                //コンビネーション中なら待つ
                if (_movement.CurrentState == CharacterStates.MovementStates.Combination)
                {
                    await UniTask.WaitUntil(() => _movement.CurrentState == CharacterStates.MovementStates.Attack, cancellationToken: magicCancel.Token);
                }

                //ここから弾丸を出す
                atEf.BulletCall(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation), SManager.instance.useMagic.flashEffect);


            }




            //弾丸を生成し終わったら
            //状態をもとに戻す
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            actionNum = 0;
            _movement.ChangeState(CharacterStates.MovementStates.Idle);


            //弾丸全部出し終わったらロックカーソルを黄色に戻す
            //ここアリ－リストからナンバーで飛ばせる
            if (target.targetObj != null && sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                target.targetObj.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
            }


            //立ち位置確認
            sb.PositionJudge();


            //状態変化するなら変更する
            //クールタイム破棄とステートチェンジの関係どうしようか
            //やっぱり魔法使用後にしよう
            //じゃないと少なくとも一回は行動できない
            //あと現在優先する行動状態の時は何も変わらないのでわざわざ呼び出さない
            if (stateChange && sister.priority != sister.nowMove)
            {
                //クールタイムキャンセル
                disEnable = false;

                //状態変化
                SisterStateChange((int)sister.priority);
                StateController().Forget();
            }
            //ステートチェンジしないならクールタイムやる？
            else
            {
                CoolTimeWait(coolTime).Forget();
            }


            //魔法使用後現在の立ち位置が間違ってないかを判定
            //ここちゃんと立ち位置あってると判断で来てからターゲット判断とか呼ぼう。awaitして
            sb.PositionJudge();

            //クールタイム呼び出し、あるいはステートチェンジしたら判断メソッドを呼び出す
            CombatMoveJudge().Forget();

        }


        #endregion



        /// <summary>
        /// XとYの間で乱数を出す
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y, bool setSeed = false, int seed = 0)
        {

            return UnityEngine.Random.Range(X, Y + 1);

        }




        /// <summary>
        ///魔力回復などのために行動ごとにクールタイム設定可能 
        /// </summary>
       async UniTaskVoid CoolTimeWait(float coolTime)
        {



            //時間待つ
            //最低でも1秒
            //それかdisenableが解除されたら進む
            await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(coolTime + 1),cancellationToken:magicCancel.Token),
                UniTask.WaitUntil(()=> !disEnable,cancellationToken:magicCancel.Token));

            //すでにdisenable解除されてるなら余計なことせず戻る
            if (!disEnable)
            {
                return;
            }



                    disEnable = false;
                    skipCondition= SkipCondition.なし;
          



        }


        /// <summary>
        /// 詠唱モーションのアニメイベント
        /// 詠唱中のために音とエフェクトをセット
        /// 使用する魔法を引数にして魔法レベルと属性で見る
        /// 詠唱の音やエフェクト変えたいならここでいじる
        public void CastCircleCall(Magic useMagic,float targetPosition)
        {

            //詠唱開始、止まる
            _controller.SetHorizontalForce(0);

            //詠唱のためのモーション数値をとる
            actionNum = (int)useMagic.castType;

            //状態も変化させる
            _movement.ChangeState(CharacterStates.MovementStates.Cast);
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);

            //エフェクトの呼び出し
            //音も出る
            atEf.CastStart(useMagic.magicLevel, useMagic.magicElement);

            //ターゲットの方向を向く
            sb.SisFlip(Mathf.Sign(targetPosition - transform.position.x));

        }





        /// <summary>
        /// ちゃんとターゲットと魔法のデータが存在するかどうかを問い合わせる
        /// そして返す
        /// </summary>
        /// <returns></returns>
        (TargetData,SisMagic) NecessaryDataGet()
        {
            if(sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                return (SManager.instance._targetList[targetNum], SManager.instance.attackMagi[useMagicNum]); 
            }

            else if(sister.nowMove == SisterParameter.MoveType.支援)
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
        /// 外部から魔法中断させる
        /// </summary>
        public void MagicEnd()
        {

            _skipCondition = 0;
            //disEnable = false;
            stateJudge = 0;
            waitCast = 0;

            //エフェクト消す
            if (soundStart)
            {
                atEf.CastStop(SManager.instance.useMagic.magicLevel, SManager.instance.useMagic.magicElement);
            }

            actionNum = 0;
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }












        ///<summary>
        ///ターゲット判断に利用
        /// </summary>
        #region　AIの判断

        /// <summary>
        /// 二点間の角度を求める
        /// </summary>
        /// <param name="p1">自分の座標</param>
        /// <param name="p2">相手の座標</param>
        /// <returns></returns>
        float GetAim(Vector2 p1, Vector2 p2)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            float rad = Mathf.Atan2(dy, dx);
            return rad * Mathf.Rad2Deg;
        }





        //明日やること
        //CanList系統は全部一つにまとめられそう
        //だってターゲット設定→魔法判断で明確に流れあるし？
        //MPチェックと射程チェックできてない



        //改善点
        //CanList系統は全部一つにまとめられそう
        //MPチェックと射程チェックできてない
        //だってターゲット設定→魔法判断で明確に流れあるし？
        //それか魔法判断とターゲット設定は別々にやる？
        //ないなぁ
        #region 新判断用メソッド







        #region　ステート管理部分

        /// <summary>
        /// ステート変更
        /// クールタイムとステート発動は攻撃語択一で呼ばれるので
        /// クールタイム待ち中にステートが呼び出されることはない
        /// </summary>
        void SisterStateChange(int condition)
        {
            if (condition == 0)
            {
                sister.nowMove = SisterParameter.MoveType.攻撃;
            }
            else if(condition == 1)
            {
                sister.nowMove = SisterParameter.MoveType.支援;
            }
            else
            {
                sister.nowMove = SisterParameter.MoveType.回復;
            }
            
            //ステート変化の時間を再変更
            stateJudge = GManager.instance.nowTime;
            //クールタイムもさよなら
            disEnable = false;
        }

        /// <summary>
        /// シスターさんのステートを切り替える機能
        /// 決めた秒数ごとに切り替え
        /// 状態切り替えなどで待機秒数リセット
        /// 
        /// じゃあ変数との差を待たせるか
        /// 
        /// </summary>
        async UniTaskVoid StateController()
        {
            //現在時間から待機開始時間を引いた時間が変更時間を超えたら
            //ステート変化フラグ立てて攻撃の終わりかはじまりでそれをやるか？
            await UniTask.WaitUntil(()=> (GManager.instance.nowTime - stateJudge) >= sister.stateResetRes);

            stateChange = true;
        }

        #endregion

        #region　AI判断部分の管理


        /// <summary>
        /// 戦闘AIの本体。これでターゲット設定と魔法選択を行う
        /// 判定メソッドの返り値は条件に合うのが存在するかのbool
        /// そして最後に優先順位を決めるのがint
        /// 
        /// ターゲットと使用魔法をintで保持する
        /// </summary>
        async UniTaskVoid CombatMoveJudge()
        {

            //クールタイム中でスキップ条件もないならクールタイム終わるまで待とうね
            if(disEnable && (int)skipCondition == 0)
            {
                await UniTask.WaitUntil(()=> disEnable,cancellationToken:magicCancel.Token);
            }

            //もし通常状態じゃないならそれまで魔法開始を待つ
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal, cancellationToken: magicCancel.Token);
            }


            int useNum;


            //番号獲得
            useNum = FirstCheck();

            if(useNum == 99)
            {
                //スキップ条件があってクールタイム中なら再帰呼び出し
                if (disEnable)
                {
                    //一秒待って再帰呼び出し
                    //また条件判断するよ
                    await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken:magicCancel.Token);


                    CombatMoveJudge().Forget();
                }

                return;
            }

            //判断に使うデータ
            SisterConditionBase useCondition;


            //攻撃回復支援で使うデータを分ける
            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                useCondition = sister.targetCondition[useNum];
            }
            else if (sister.nowMove == SisterParameter.MoveType.支援)
            {
               useCondition = sister.supportPlan[useNum];
            }
            else
            {
                useCondition = sister.recoverCondition[useNum];
            }

            //何もしないなら戻る
            //クールタイムある場合は分岐してもいいかも
            if(useCondition.selectAction == UseAction.なにもしない)
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }


            //ターゲットの絞り込みを続ける
            //ターゲットの番号を割り出す
            targetNum = TargetSelectStart(useCondition.selectCondition);
            //候補者リストをクリーンナップ
            candidateList.Clear();

            //ここから魔法判断
            //条件に当てはまる魔法がないなら戻る
            if (!MagicJudge(useCondition.bulletCondition))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }

            //ここから攻撃支援回復ごとに処理がわかれる
            //当てはまらなかったら戻る
            if(!StateSpecificCheck(useNum))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }


            //ここでMPと射程距離もチェックする
            if (!NecessaryCheck(useCondition.mpCheck, useCondition.rangeCheck))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }


            

            //ターゲットに優先順位をつけて選択する
            useMagicNum = MagicSelect(useCondition.magicSort,useCondition.bulletASOder);
            //候補者リストをクリーンナップ
            candidateList.Clear();



            //クールタイム設定
            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                coolTime = sister.targetCondition[useNum].coolTime;
            }
            else if(sister.nowMove == SisterParameter.MoveType.支援)
            {
                coolTime = sister.supportPlan[useNum].coolTime;
            }
            else
            {
                coolTime = sister.recoverCondition[useNum].coolTime;
            }

            //魔法詠唱開始
            CastMagic().Forget();

        }


        #region 最初に呼ばれる行動できるかを判断するコード

        /// <summary>
        /// 最初に行動条件を満たしているのかを確認する
        /// Disenable中は スキップ条件を見て値を返す
        /// </summary>
        /// <returns></returns>
        int FirstCheck()
        {

            if (!disEnable)
            {
                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    return NormalCheckStart(sister.targetCondition);
                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
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
                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    return SkipCheckStart(sister.targetCondition);
                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
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
        /// 行動条件が当てはまるかの共通処理
        /// ステートに関係なくこれで今行動できるかを見れる
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int NormalCheckStart(SisterConditionBase[] condition)
        {
            int count = condition.Length;

            //まずはそれぞれの
            for (int i = 0;i<count;i++)
            {
                //行動条件と行動を渡して
                if (ActConditionJudge(condition[i].judgeCondition, condition[i].selectAction))
                {
                    return i;
                }
            }
            //何も当てはまらないなら99
            return 99;
        }


        int SkipCheckStart(SisterConditionBase[] condition)
        {
            int count = condition.Length;


            //五番目までやる
            //五番目までだから長さから1引いてる
            //スキップコンディションを見る処理
            for (int i = 0; i < count; i++)
            {

                //0乗は１
                //クールタイム中で、なおかつスキップコンディションに当てはまらないなら処理を飛ばす。
                //シフト演算？
                if (((int)skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
                {
                    continue;
                }

                //行動条件と行動を渡して
                if (ActConditionJudge(condition[i].judgeCondition, condition[i].selectAction))
                {
                    return i;
                }

            }
            return 99;
        }

        #endregion



        /// <summary>
        /// ステートごとに特化したチェックを行う
        /// </summary>
        /// <returns></returns>
        bool StateSpecificCheck(int useNum)
        {

            //攻撃では属性だけ
            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                //属性判断の結果を返す
                return ElementJudge(sister.targetCondition[useNum].useElement);
            }

            //サポートではサポート効果と属性で選ぶ
            else if (sister.nowMove == SisterParameter.MoveType.支援)
            {
                SupportCondition useCondition = sister.supportPlan[useNum];

                //条件が当てはまらないならfalseを返す
                if (!SupportConditionJudge(useCondition.secondActCondition))
                {
                    return false;
                }

                //属性判断の結果を返す
                return ElementJudge(useCondition.useElement);

            }
            //回復ではヒールの効果とサポート効果のおまけで選ぶ
            else
            {
                RecoverCondition useCondition = sister.recoverCondition[useNum];

                //条件が当てはまらないならfalseを返す
                if (!HealConditionJudge(useCondition.secondActJudge))
                {
                    return false;
                }

                //条件が当てはまらないならfalseを返す
                return SupportConditionJudge(useCondition.healSupport);
            }
        }


        /// <summary>
        /// ここで必要なチェックをする
        /// 射程距離とMPを調べて問題ないなら真を返す
        /// 
        /// これの目的は最適魔法の射程範囲外にターゲットがいたりmpが足りないときは
        /// 最適ではないがそれを満たす魔法を代わりに選ぶこと
        /// </summary>
        /// <param name="mpCheck"></param>
        /// <param name="RangeCheck"></param>
        /// <returns></returns>
        bool NecessaryCheck(bool mpCheck,bool RangeCheck)
        {
            //どちらもいらないなら真を返す
            if(!mpCheck && !RangeCheck)
            {
                return true;
            }

            int magicCount = candidateList.Count;




            //空っぽならリストの数字を入れる
            if (magicCount == 0)
            {

                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.attackMagi.Count;

                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.recoverMagi.Count;
                }

                //これは何番目の魔法を使うかの判断に
                //CanListをつかうから必要な処理
                for (int i = 0; i < magicCount; i++)
                {
                    //ターゲットの数だけ
                    //リストに候補を追加
                    candidateList.Add(i);
                }

            }

            Magic checkMagic;

            //距離を求める
            float distance;
            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                distance = Vector2.Distance(transform.position,SManager.instance._targetList[targetNum]._condition.targetPosition);

            }
            else 
            {
                distance = Vector2.Distance(transform.position, EnemyManager.instance._targetList[targetNum]._condition.targetPosition);
            }


            //チェック開始
            for (int i = 0; i < magicCount; i++)
            {
                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    checkMagic = SManager.instance.attackMagi[candidateList[i]];

                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
                {
                    checkMagic = SManager.instance.supportMagi[candidateList[i]];
                }
                else
                {
                    checkMagic = SManager.instance.recoverMagi[candidateList[i]];
                }

                //距離判定
                if (RangeCheck)
                {
                    //サーチ魔法にしても追跡限界距離みたいなのはあるはず
                    //なんか射程がわかるようなの積んどこう
                    //if()して当てはまるならmp調べずにコンティニュー
                }

                if (mpCheck)
                {
                    //魔力が超えてるなら
                    if(checkMagic.useMP > sb.mp)
                    {
                        //i番目の要素を排除
                        candidateList.RemoveAt(i);
                        
                    }
                }

            }


                //チェック後に要素が残ってれば真
                return candidateList.Any();
        }


        #endregion

        #region ターゲット設定に使用

        //第一判断の前にクールタイムゼロで、なおかつ何もしないが行動方針なら判断はしないようにする
        //これより上の関数、全体の管理関数でやる
        //ステートチェンジが行動の時はクールタイム使えない
        //また、何もしない時は第一判断が真なら第二判断はしないでそのまま何もしないモードへ
        //あと第一判断において、行動が状態変化や何もしないの時は無駄な処理をしないようにしてある。それは実装済み
        #region 第一判断



        /// <summary>
        /// 行動を起こすかどうかの判断。
        /// 同時にCanListを使ってターゲットの絞り込みも
        /// 
        /// 全ステートで共通して使える
        /// ここでターゲット絞り込んでcanListが1以上なら二段階目の条件で絞り込む
        /// 逆にリストがゼロなら全部候補に含む
        /// また、回復や支援ではシスターさんやプレイヤーなどの直接指定も絞り込みに含む
        /// 
        /// </summary>
        /// <param name="judgeData"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool ActConditionJudge(ActJudgeCondition[] judgeData, UseAction action)
        {

            //三未満ならステートを変更する
            bool stateChange = (int)action < 3;

            //条件なしかのフラグ
            bool noCondition = true;

            for(int i = 0;i < 3; i++)
            {
                //どれか一つでも指定なしじゃないなら条件あり
                if (judgeData[i].actCondition != ActCondition.指定なし)
                {
                    noCondition = false;
                    break;
                }
            }


            //指定なしなら判断せず返す
            if (noCondition)
            {
                //状態変えるなら変えて偽を返す
                if (stateChange)
                {
                    SisterStateChange((int)action);
                    return false;
                }
                return true;
            }
            else
            {
                

                //条件当てはまるなら状態変化か行動を実行
                if (JudgeStart(judgeData, sister.nowMove == SisterParameter.MoveType.攻撃, action))
                {

                    //状態変えるなら変えて偽を返す
                    if (stateChange)
                    {
                        SisterStateChange((int)action);
                        return false;
                    }
                    return true;
                }

                //何も当てはまらんならfalse
                return false;
            }

        }


        /// <summary>
        /// 行動判断の実行部分
        /// 条件を整理して共通処理に落とし込む準備をする
        /// ターゲットの記録などはここで行う
        /// Exe()で具体的な比較を行うために情報を加工する
        /// また、状態変化や何もしないを行動に選んでいる場合は無駄な処理を省くようにしてある
        /// </summary>
        /// <returns></returns>
        public bool JudgeStart(ActJudgeCondition[] condition,bool isAttack, UseAction action)
        {
            ///記録必要かどうか
            bool needRecord;

            bool isResult = false;


            //三つの条件はここでループさせる
            //支援、回復の場合と攻撃の場合でandとorの条件が変わる
            //支援、回復の場合はandは絶対に当てはまらないといけない条件。これが外れると行動できない
            //orは当てはまらなくていいが、当たると優先標的対象が増える。でもor三つ全部外れると行動できない

            //攻撃の場合はandは絶対に外せない条件。そして当たると優先標的が増える。外れると優先標的が減る
            //orは当てなくてもいいけど当たると当たった分だけ優先標的が増える。でもor三つ全部外れると行動できない


            for (int i = 0; i < 3; i++)
            {

                if (condition[i].range == CheckRange.Player)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);


                    //回復や支援ではOr条件の時だけ追加する
                    if (JudgeExe(SManager.instance._targetList[0], condition[i]))
                    {

                        //ターゲットを記録
                        if (needRecord)
                        {


                            if (condition[i].rule == JudgeRuLe.or条件)
                            {
                                //プレイヤーを追加
                                //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                                //回復や支援では直接指定の絞り込みもつけてやるか
                                candidateList.Add(0);
                            }

                        }

                        //一個でも当てはまる条件あるなら結果は真になる
                        //ただしその後and条件を外したら結果はfalseになる
                        isResult = true;

                    }
                    //and条件ミスったらアウト
                    else if (condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.Sister)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //回復や支援ではOr条件の時だけ追加する
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);

                    if (JudgeExe(SManager.instance._targetList[1], condition[i]))
                    {



                        //ターゲットを記録
                        if (needRecord)
                        {



                            //シスターさんを追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(1);


                        }

                        //一個でも当てはまる条件あるなら結果は真になる
                        //ただしその後and条件を外したら結果はfalseになる
                        isResult = true;
                    }

                    //and条件ミスったらアウト
                    else if (condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.OtherAlly)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //また、Orじゃないなら記録はしない
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);

                    //エネミーマネージャーがターゲットでシスターさん側を持ってる
                    int count = EnemyManager.instance._targetList.Count;

                    //シスターさんとプレイヤー以外にいないなら判断しない
                    if (count < 3)
                    {
                        //and条件なら不適合
                        if (condition[i].rule == JudgeRuLe.and条件)
                        {
                            return false;
                        }

                        continue;
                    }

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 2; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;


                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);




                        }


                    }

                    //and条件ミスったらアウト。isMatchがfalseならいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }


                }
                else if (condition[i].range == CheckRange.Ally)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //また、Orじゃないなら記録はしない
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);


                    //エネミーマネージャーがターゲットでシスターさん側を持ってる
                    int count = EnemyManager.instance._targetList.Count;

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;

                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);


                        }


                    }


                    //and条件ミスったらアウト。isMatchがfalseなら当てはまるのがいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }
                }
                //敵
                else
                {
                    //状態変化を控えていなくて攻撃中なら記録する
                    needRecord = isAttack && action == UseAction.現在の設定で行動;

                    //Sマネージャーがターゲットで敵側を持ってる
                    int count = SManager.instance._targetList.Count;

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(SManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;


                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);




                        }
                        //and条件の時、当てはまらなかった要素を含むならけしておく
                        else if (condition[i].rule == JudgeRuLe.and条件 && needRecord)
                        {
                            //含んでない要素をremoveしてもエラーじゃないので
                        //    if (candidateList.Contains(s))
                        //    {
                                candidateList.Remove(s);
                        //    }
                        }

                    }

                    //and条件ミスったらアウト。isMatchがfalseならいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }

                }
            }


            //and条件ミスるとfalse返すようになってる
            //Or条件だけで、当てはまるものが一つもなかった場合はisResultがfalseになる
            //よってここまで来て、なおかつisResultが真なら真
            return isResult;
        }



        /// <summary>
        /// ないよう編集用
        /// 
        /// 
        /// 行動判断の実行部分
        /// 条件を整理して共通処理に落とし込む準備をする
        /// ターゲットの記録などはここで行う
        /// </summary>
        /// <returns></returns>
        public bool EditJudge(ActJudgeCondition[] condition, bool isAttack, UseAction action)
        {

            ///記録必要かどうか
            bool needRecord;

            bool isResult = false;

            //三つの条件はここでループさせる
            //そしてAnd条件の場合は当てはまらない奴を消す、or条件の場合は当てはまったやつを足す
            //同一の結合条件がある場合は事前に並び替え
            //三つとも結合ならふつうのandやorに変換
            //それかexeに一気に三つの条件を渡してやらせるか？
            //でもcondition.Rangeなどの条件が必ずしも同じになるとは限らない
            //ここでやるしかない
            //中の処理を変えるのはあり

            for (int i = 0; i < 3; i++)
            {

                if (condition[i].range == CheckRange.Player)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);


                    //回復や支援ではOr条件の時だけ追加する
                    if (JudgeExe(SManager.instance._targetList[0], condition[i]))
                    {

                        //ターゲットを記録
                        if (needRecord)
                        {


                            if (condition[i].rule == JudgeRuLe.or条件)
                            {
                            //プレイヤーを追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(0);
                            }

                        }

                        //一個でも当てはまる条件あるなら結果は真になる
                        //ただしその後and条件を外したら結果はfalseになる
                        isResult = true;

                    }
                    //and条件ミスったらアウト
                    else if (condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }

                }
                else if (condition[i].range == CheckRange.Sister)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //回復や支援ではOr条件の時だけ追加する
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動)&&(condition[i].rule == JudgeRuLe.or条件);

                    if (JudgeExe(SManager.instance._targetList[1], condition[i]))
                    {
                        


                            //ターゲットを記録
                            if (needRecord)
                            {



                                    //シスターさんを追加
                                    //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                                    //回復や支援では直接指定の絞り込みもつけてやるか
                                    candidateList.Add(1);


                            }

                        //一個でも当てはまる条件あるなら結果は真になる
                        //ただしその後and条件を外したら結果はfalseになる
                        isResult = true;
                    }

                        //and条件ミスったらアウト
                        else if (condition[i].rule == JudgeRuLe.and条件)
                        {
                            return false;
                        }

                }
                else if (condition[i].range == CheckRange.OtherAlly)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //また、Orじゃないなら記録はしない
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);

                    //エネミーマネージャーがターゲットでシスターさん側を持ってる
                    int count = EnemyManager.instance._targetList.Count;

                    //シスターさんとプレイヤー以外にいないなら判断しない
                    if (count < 3)
                    {
                        //and条件なら不適合
                        if (condition[i].rule == JudgeRuLe.and条件)
                        {
                            return false;
                        }

                        continue;
                    }

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 2; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;


                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);




                        }


                    }

                    //and条件ミスったらアウト。isMatchがfalseならいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }


                }
                else if (condition[i].range == CheckRange.Ally)
                {
                    //状態変化を控えていなくて攻撃中でなければ記録する
                    //また、Orじゃないなら記録はしない
                    needRecord = (!isAttack && action == UseAction.現在の設定で行動) && (condition[i].rule == JudgeRuLe.or条件);


                    //エネミーマネージャーがターゲットでシスターさん側を持ってる
                    int count = EnemyManager.instance._targetList.Count;

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(EnemyManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;

                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);


                        }


                    }


                    //and条件ミスったらアウト。isMatchがfalseなら当てはまるのがいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }
                }
                //敵
                else
                {
                    //状態変化を控えていなくて攻撃中なら記録する
                    needRecord = isAttack && action == UseAction.現在の設定で行動;

                    //Sマネージャーがターゲットで敵側を持ってる
                    int count = SManager.instance._targetList.Count;

                    //当てはまる個体がいたかのフラグ
                    bool isMatch = false;

                    for (int s = 0; s < count; s++)
                    {

                        //ひとつひとつ条件判断していく
                        //もしいたなら記録
                        if (JudgeExe(SManager.instance._targetList[s], condition[i]))
                        {
                            //個体がいたならマッチを真に
                            isMatch = true;


                            //一個でも当てはまる条件あるなら結果は真になる
                            //ただしその後and条件を外したら結果はfalseになる
                            isResult = true;

                            //記録いらないならいた時点で終わる
                            if (!needRecord)
                            {
                                break;
                            }

                            //それ以外なら記録
                            //回復や支援ではOr条件の時だけ追加する


                            //当てはまる味方を追加
                            //シスターさんのMPが～の時、かつプレイヤーのHPが50％以上の時、とかもあるから
                            //回復や支援では直接指定の絞り込みもつけてやるか
                            candidateList.Add(s);




                        }
                        //and条件の時、当てはまらなかった要素を含むならけしておく
                        else if (condition[i].rule == JudgeRuLe.and条件 && needRecord)
                        {
                            if (candidateList.Contains(s))
                            {
                                candidateList.Remove(s);
                            }
                        }

                    }

                    //and条件ミスったらアウト。isMatchがfalseならいなかった
                    if (!isMatch && condition[i].rule == JudgeRuLe.and条件)
                    {
                        return false;
                    }

                }
            }


            //and条件ミスるとfalse返すようになってる
            //Or条件だけで、当てはまるものが一つもなかった場合はisResultがfalseになる
            //よってここまで来て、なおかつisResultが真なら真
            return isResult;

        }



        /// <summary>
        /// キャラデータに対して共通処理による真偽を返す
        /// 一体一体の敵をここで判断する
        /// </summary>
        /// <returns></returns>
        bool JudgeExe(TargetData data, ActJudgeCondition condition)
        {

            //ここで射程距離確認しない
            //攻撃行動選択の時に、行動選んでからその行動でターゲットが射程にいるかを確認する
            //       if (rangecheck)
            //          {
            ///           return false;
            //   }

            if (condition.actCondition == ActCondition.指定なし)
            {
                return true;
            }

            //まて、＆条件とかいろいろあるよな


            if ((int)condition.content < 3)
            {

                if (condition.content == CheckContent.HP)
                {
                    //同じならオッケー
                    if (data._condition.hpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //距離が以下ならHighLowが偽なら真になる
                    //以上なら真だと真になる
                    return (data._condition.hpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;

                }
                else if (condition.content == CheckContent.MP)
                {
                    //同じならオッケー
                    if (data._condition.mpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //距離が以下ならHighLowが偽なら真になる
                    //以上なら真だと真になる
                    return (data._condition.mpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }
                else if (condition.content == CheckContent.type)
                {
                    //タイプは一致してる？
                    return Convert.ToBoolean((int)data._baseData._type & condition.percentage);
                }
            }
            else if ((int)condition.content < 6)
            {
                if (condition.content == CheckContent.strength)
                {
                    //強敵かどうかと強敵を求めるかどうかが一致してるなら
                    //強敵じゃないときと強敵求める時どちらにも一致する
                    return data._baseData.isStrong == condition.highOrLow;
                }
                else if (condition.content == CheckContent.posiCondition)
                {
                    //バフを含んでいるか
                    return Convert.ToBoolean((int)data._condition.buffImfo & condition.percentage);
                }
                else if (condition.content == CheckContent.negaCondition)
                {
                    //デバフを含んでいるか
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

                    //ここキャッシュに置き変えろよほんと
                    float distance = Vector2.Distance(data._condition.targetPosition, transform.position);

                    //同じならオッケー
                    if (distance == condition.percentage)
                    {
                        return true;
                    }

                    //距離が以下ならHighLowが偽なら真になる
                    //以上なら真だと真になる
                    return (distance <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }
                else if (condition.content == CheckContent.armor)
                {
                    //同じならオッケー
                    if (data._condition.hpRatio == condition.percentage)
                    {
                        return true;
                    }

                    //距離が以下ならHighLowが偽なら真になる
                    //以上なら真だと真になる
                    return (data._condition.hpRatio <= condition.percentage) ? !condition.highOrLow : condition.highOrLow;
                }

            }

            else// if ((int)condition.content < 12)  //さらに追加されたらこの判断する
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

                    //1以上ならシスターさんとプレイヤーじゃない
                    return data._condition.target.targetNum > 1;
                }
            }

            return false;
        }






        #endregion





        //絞り込んだターゲットをソートする
        //ここまで処理が来てるということはターゲットがいるかの判断でfalseじゃなかったってこと
        //なんでcanlistが空っぽなら全体
        #region 第二判断


        /// <summary>
        /// ターゲットを選択する
        /// 候補から条件に一番当てはまるのをターゲットにするだけ
        /// 並び替えはいらなそう
        /// </summary>
        /// <param name="sortCondition"></param>
        int TargetSelectStart(TargetSelectCondition condition)
        {
            //ターゲットの数
            int targetCount = candidateList.Count;

            //指定なし、あるいは一つならリストの最初の要素を返す
            if(condition.SecondCondition == AdditionalJudge.指定なし || targetCount == 1)
            {
                return 0;
            }




            //xが何番目で、yがどんな数値かを記録
            Vector2 container = Vector2.zero;

            //攻撃中かどうか
            bool isAttack = sister.nowMove == SisterParameter.MoveType.攻撃;




            //まずはターゲットリストを整理
            //空っぽならcandidateListをリストの全体に設定
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


                //ここでもTarget候補が一つだけなら0を返す
                if (targetCount == 1)
                {
                    return 0;
                }

                for (int i = 0; i < targetCount; i++)
                {
                    //ターゲットの数だけ
                    //リストに候補を追加
                    candidateList.Add(i);
                }
            }



            float result;

            //重複があるかどうか
            bool duplication = false;


            //最初の比較対象を設定
            //0番を入れる
            container.Set(0, ReturnValue(condition.SecondCondition, 0, isAttack));

            //最適目標を検索
            for (int i = 1; i < targetCount; i++)
            {
                result = ReturnValue(condition.SecondCondition,i,isAttack); 


                if (result == container.y)
                {
                    //重複あり
                    duplication = true;
                }

                //昇順か降順かで判定を分ける
                //昇順ならより大きいもの、降順ならより小さいもの
                if ((condition.targetASOrder) ? (result > container.y):(result < container.y))
                {
                    //新しく最適対象の番号と数値を設定
                    container.Set(i, result);

                    //さらにターゲット更新で重複は白紙に
                    duplication = false;
                }
                

            }



            //そして重複があって、なおかつスペア条件の指定があるなら
            //さらに最適目標を検索
            if(duplication && condition.spareCondition != AdditionalJudge.指定なし)
            {

                bool isFirst = true;

                //前回検査で漏れた分の除外に使う数値
                float judgeNum = container.y;

                //最適目標を検索
                for (int i = 0; i < targetCount; i++)
                {
                    //まずは一個目の条件の数値を確認
                    result = ReturnValue(condition.SecondCondition, i, isAttack);


                    //もしリザルトが一個目の検査での最適数値と同じでないなら
                    //検査からは弾く
                    if(result != judgeNum)
                    {
                        //ターゲット候補からの除外を示す99を入れて処理を続ける
                        candidateList[i] = 99;
                        continue;
                    }

                    //これを超えたら予備の条件
                    result = ReturnValue(condition.spareCondition, i, isAttack);


                    //最初は無条件で入れる
                    //こっちは必要、だって最初に判定する条件が0からとは限らないもんね
                    if (isFirst)
                    {
                        isFirst = false;

                        container.Set(i,result);
                        continue;
                    }

                    //スペアの条件が昇順か降順かで判定を分ける
                    //昇順ならより大きいもの、降順ならより小さいもの
                    if ((condition.spareASOrder) ? (result > container.y) : (result < container.y))
                    {
                        //新しく最適対象の番号と数値を設定
                        container.Set(i, result);
                    }


                }

                //で、二回目の検査を経て残ったものを返す
                return (int)container.x;
            }
            //重複がないかスペアの条件がないなら
            else
            {

                //検査を経て残ったものを返す
                return (int)container.x;
            }

        }


        


        /// <summary>
        /// 比較に必要な数値を返す
        /// </summary>
        float ReturnValue(AdditionalJudge condition,int num,bool isAttack)
        {


            //ターゲットデータを取得
            //攻撃中ならSManagerから
            TargetData data = isAttack ? SManager.instance._targetList[num] : EnemyManager.instance._targetList[num];

            if(condition == AdditionalJudge.ターゲットのHP割合)
            {
                return data._condition.hpRatio;
            }
            else if (condition == AdditionalJudge.ターゲットのHP数値)
            {
                return data._condition.hpNum;
            }
            else if (condition == AdditionalJudge.ターゲットのMP割合)
            {
                return data._condition.mpRatio;
            }
            else if (condition == AdditionalJudge.ターゲットの距離)
            {
                //ここはキャッシュに置き換える
                return Vector2.Distance(transform.position,data._condition.targetPosition);
            }
            else if (condition == AdditionalJudge.ターゲットの高度)
            {
                return data._condition.targetPosition.y;
            }
            else if (condition == AdditionalJudge.ターゲットの攻撃力)
            {
                return data._baseData.displayAtk;
            }
            else if (condition == AdditionalJudge.ターゲットの防御力)
            {
                return data._baseData.displayDef;
            }
            else if (condition == AdditionalJudge.ターゲットのアーマー値)
            {

            }
            else if (condition == AdditionalJudge.ターゲットのバフ数)
            {
                //項目数でビットを全検査して状態異常の数を確認
                return BitCheck(GetEnumLength<ControllAbillity.PositiveCondition>(),(int)data._condition.buffImfo);
            }
            else if (condition == AdditionalJudge.ターゲットのデバフ数)
            {
                return BitCheck(GetEnumLength<ControllAbillity.NegativeCondition>(), (int)data._condition.debuffImfo);
            }

            return 0;
        }


        /// <summary>
        /// ビット演算でバフやデバフの数を数える
        /// </summary>
        int BitCheck(int bitNum,int data)
        {
            int count = 0;
            for (int i = 0;i<bitNum;i++)
            {
                //もし検査用ビットの分が1ならカウントを増やす
                if((bitNum & 1<< i) == 1 << i)
                {
                    count++;
                }

            }
            return count;
        }

        /// <summary>
        /// enumの項目数を獲得
        /// bit演算に使う
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int GetEnumLength<T>()
        {
            return Enum.GetValues(typeof(T)).Length;
        }


        #endregion

        #endregion




        //魔法判断では弾丸の性質で魔法を絞り込む
        //さらにソートして、独自の判断に繋げる?
        //でもソートする前に独自条件でさらに絞り込めばソートじゃなくて最適拾えるよ
        #region 魔法判断に使用


        //攻撃なのか、サポートなのか、回復なのかで検査対象変わってくる
        #region 第一段階、絞り込み


        /// <summary>
        /// まずは弾丸の性質で魔法を絞り込む
        /// ここでゼロならなにも使わない？
        /// mpチェックもする？　リストに加える時にさ
        /// これは魔法絞り込みと同時に当てはまる条件があるかどうかの検査でもある
        /// 
        /// 次はソートだけどそれぞれの魔法の固有の絞り込み処理とかもやんないとな―
        /// </summary>
        bool MagicJudge(Magic.BulletType condition)
        {
            //調査対象の魔法の数
            int magicCount;


            if(sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                //魔法の数を入れる
                magicCount = SManager.instance.attackMagi.Count;
                
            }
            else if(sister.nowMove == SisterParameter.MoveType.支援)
            {
                //魔法の数を入れる
                magicCount = SManager.instance.supportMagi.Count;
            }
            else
            {
                //魔法の数を入れる
                magicCount = SManager.instance.recoverMagi.Count;
            }

            //魔法がないなら戻る
            if(magicCount == 0)
            {
                return false;
            }


            for(int i = 0; i < magicCount; i++)
            {
                //魔法が当てはまるなら
                if (MagicCheck(i,condition))
                {
                    //リストに加えてあげる
                    candidateList.Add(i);
                }
            }

            return candidateList.Any();

        }


        /// <summary>
        /// 弾丸の特徴が条件に当てはまるかを確認する
        /// ビット演算を使う
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool MagicCheck(int num, Magic.BulletType condition)
        {
            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.attackMagi[num].bulletFeature & condition) == condition;

            }
            else if (sister.nowMove == SisterParameter.MoveType.支援)
            {
                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.supportMagi[num].bulletFeature & condition) == condition;
            }
            else
            {
                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.recoverMagi[num].bulletFeature & condition) == condition;
            }
        }



        #endregion



        //さらにここで絞り込んでからソートする
        #region 状態固有の判断に使用

        #region 支援条件


        /// <summary>
        /// 支援魔法の効果条件から魔法の候補を絞り込む
        /// 回復の二個目の条件と、支援の一個目の条件で使う
        /// 当てはまる魔法があるかも判断して返す
        /// </summary>
        bool SupportConditionJudge(Magic.SupportType　condition)
        {

            if(condition == Magic.SupportType.なし)
            {
                return true;
            }

            //調査対象の魔法の数
            //候補から割り出す
            int magicCount = candidateList.Count;

            //今支援状態か
            bool isSupport = sister.nowMove == SisterParameter.MoveType.支援;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.recoverMagi.Count;
                }
            }

            //魔法がないなら戻る
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {
                //魔法が当てはまらないなら候補から消す
                
                if (!SupportConditionCheck(candidateList[i],condition,isSupport))
                {
                    //i番目の要素をリストから削除
                    candidateList.RemoveAt(i);
                }
            }
//もし削除したのが反映されず条件判断がうまくいかないなら
//magicCountをコピーして削除するごとにカウントを減らしてそれが0かで判断してもいい
            return candidateList.Any();

        }


        /// <summary>
        /// 弾丸の特徴が条件に当てはまるかを確認する
        /// ビット演算を使う
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool SupportConditionCheck(int num, Magic.SupportType condition,bool isSupport)
        {
             if (isSupport)
            {
                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.supportMagi[num].supportEffect & condition) == condition;
            }
            else
            {
                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.recoverMagi[num].supportEffect & condition) == condition;
            }
        }


        #endregion

        #region 属性

        /// <summary>
        /// 支援魔法の効果条件から魔法の候補を絞り込む
        /// 回復の二個目の条件と、支援の一個目の条件で使う
        /// 当てはまる魔法があるかも判断して返す
        /// </summary>
        bool ElementJudge(AtEffectCon.Element condition)
        {

            if(condition == AtEffectCon.Element.指定なし)
            {
                return true;
            }

            //調査対象の魔法の数
            //候補から割り出す
            int magicCount = candidateList.Count;

            //今支援状態か
            bool isSupport = sister.nowMove == SisterParameter.MoveType.支援;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.attackMagi.Count;
                }
            }

            //魔法がないなら戻る
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {

                //魔法が当てはまらないなら候補から消す
                if (!ElementCheck(candidateList[i], condition, isSupport))
                {
                    //i番目の要素をリストから削除
                    candidateList.RemoveAt(i);
                }
            }
            //もし削除したのが反映されず条件判断がうまくいかないなら
            //magicCountをコピーして削除するごとにカウントを減らしてそれが0かで判断してもいい
            return candidateList.Any();

        }


        /// <summary>
        /// 弾丸の特徴が条件に当てはまるかを確認する
        /// ビット演算を使う
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool ElementCheck(int num, AtEffectCon.Element condition, bool isSupport)
        {
            if (isSupport)
            {
                //弾丸の特徴を照会
                return SManager.instance.supportMagi[num].magicElement == condition;
            }
            else
            {
                //弾丸の特徴を照会
                return SManager.instance.recoverMagi[num].magicElement == condition;
            }
        }

        #endregion


        #region 回復条件


        /// <summary>
        /// 支援魔法の効果条件から魔法の候補を絞り込む
        /// 回復の二個目の条件と、支援の一個目の条件で使う
        /// 当てはまる魔法があるかも判断して返す
        /// </summary>
        bool HealConditionJudge(Magic.HealEffectType condition)
        {

            if(condition == Magic.HealEffectType.なし)
            {
                return true;
            }

            //調査対象の魔法の数
            //候補から割り出す
            int magicCount = candidateList.Count;

            //今支援状態か
            bool isSupport = sister.nowMove == SisterParameter.MoveType.支援;

            if (magicCount == 0)
            {
                if (isSupport)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.recoverMagi.Count;
                }
            }

            //魔法がないなら戻る
            if (magicCount == 0)
            {
                return false;
            }


            for (int i = 0; i < magicCount; i++)
            {
                //魔法が当てはまらないなら候補から消す

                if (!HealConditionCheck(candidateList[i], condition))
                {
                    //リストから削除
                    candidateList.RemoveAt(i);
                }
            }
            //もし削除したのが反映されず条件判断がうまくいかないなら
            //magicCountをコピーして削除するごとにカウントを減らしてそれが0かで判断してもいい
            return candidateList.Any();

        }


        /// <summary>
        /// 弾丸の特徴が条件に当てはまるかを確認する
        /// ビット演算を使う
        /// </summary>
        /// <param name="num"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool HealConditionCheck(int num, Magic.HealEffectType condition)
        {

                //弾丸の特徴を照会
                //両方に共通する部分を取ったら条件と変わらない場合はアタリ
                return (SManager.instance.recoverMagi[num].healEffect & condition) == condition;

        }


        #endregion




        #endregion



        #region 魔法を共通条件から選択


        /// <summary>
        /// 魔法を選択する
        /// 候補から条件に一番当てはまるのを選ぶだけ
        /// 並び替えはいらなそう
        /// 
        /// 返すのは使用する魔法の番号
        /// </summary>
        /// <param name="sortCondition"></param>
        /// <param name="targetASOrder"></param>
        int MagicSelect(MagicSortCondition condition,bool asOrder)
        {

            //調査対象の魔法の数
            int magicCount = candidateList.Count;

            //指定なし、あるいは一つしかないならリストの最初の要素を返す
            if (condition == MagicSortCondition.指定なし || magicCount == 1)
            {
                return 0;
            }

            //xが何番目で、yがどんな数値かを記録
            Vector2 container = Vector2.zero;


            //空っぽならリストの数字を入れる
            if (magicCount == 0)
            {

                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.attackMagi.Count;

                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.supportMagi.Count;
                }
                else
                {
                    //魔法の数を入れる
                    magicCount = SManager.instance.recoverMagi.Count;
                }

                //ここでもTarget候補が一つだけなら0を返す
                if (magicCount == 1)
                {
                    return 0;
                }

                //これは何番目の魔法を使うかの判断に
                //CanListをつかうから必要な処理
                for (int i = 0; i < magicCount; i++)
                {
                    //ターゲットの数だけ
                    //リストに候補を追加
                    candidateList.Add(i);
                }

            }


            float result;

            //重複があるかどうか
            // bool duplication = false;


            //まずゼロを入れてから動く
            //最初は無条件に入れてね
            container.Set(0, ReturnMagicValue(condition, 0));

            //最適目標を検索
            for (int i = 1; i < magicCount; i++)
            {
                result = ReturnMagicValue(condition, i);



                //昇順か降順かで判定を分ける
                //昇順ならより大きいもの、降順ならより小さいもの
                if ((asOrder) ? (result > container.y) : (result < container.y))
                {
                    //新しく最適対象の番号と数値を設定
                    container.Set(i, result);

                    //さらにターゲット更新で重複は白紙に
             //       duplication = false;
                }


            }



                //検査を経て残ったものを返す
                return (int)container.x;


        }





        /// <summary>
        /// 比較に必要な数値を返す
        /// 
        /// 未実装　サイズ、リジェネ回復速度
        /// </summary>
        float ReturnMagicValue(MagicSortCondition condition, int num)
        {


            //ターゲットデータを取得
            //攻撃中ならSManagerから
            Magic data;



                if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {
                //魔法の数を入れる
                data = SManager.instance.attackMagi[candidateList[num]];

            }
            else if (sister.nowMove == SisterParameter.MoveType.支援)
            {
                //魔法の数を入れる
                data = SManager.instance.supportMagi[candidateList[num]];
            }
            else
            {
                //魔法の数を入れる
                data = SManager.instance.recoverMagi[candidateList[num]];
            }



            if (sister.nowMove == SisterParameter.MoveType.攻撃)
            {


            }
            else if (sister.nowMove == SisterParameter.MoveType.支援)
            {

            }
            else
            {

            }

            if (condition == MagicSortCondition.発射数)
            {
                return data.bulletNumber;
            }
            else if (condition == MagicSortCondition.詠唱時間)
            {
                return data.castTime;
            }
            else if (condition == MagicSortCondition.効果時間)
            {

                //弾丸の生存時間とバフ効果やらの効果時間の内で大きい方を返す
                //そうすれば長時間設置や一瞬のバフどちらでも同じように比較できる
                return Math.Max(data.effectTime,data._moveSt.lifeTime);

            }
            else if (condition == MagicSortCondition.追尾性能)
            {
                //追尾性能に角度制限がついてないなら365、ついてるなら制限角度を返す
                return data._moveSt.fireType == Magic.FIREBULLET.HOMING ? 365 : Math.Abs(data._moveSt.homingAngleV);
            }

            //こいつに関してはUIの側でちゃんと「攻撃力」とか「回復量」とか具体的なあれをあれしないとダメだよ
            else if (condition == MagicSortCondition.効果の大きさ)
            {

                if (sister.nowMove == SisterParameter.MoveType.攻撃)
                {
                    return data.displayAtk;
                }
                else if (sister.nowMove == SisterParameter.MoveType.支援)
                {
                    //これは仮の強化倍率
                    //本来なら別の数値が入る
                    return data.mValue;
                }
                else
                {
                    return data.recoverAmount;
                }
            }
            else if (condition == MagicSortCondition.削り値)
            {
                return data.shock;
            }
            else if (condition == MagicSortCondition.MP使用量)
            {
                return data.useMP;
            }
            else if (condition == MagicSortCondition.リジェネ回復速度)
            {
                //これ考えとかないとな
                //リジェネ実装後にね
            }
            else if (condition == MagicSortCondition.弾速)
            {
                //項目数でビットを全検査して状態異常の数を確認
                return data._moveSt.speedV;
            }
            else if (condition == MagicSortCondition.弾丸の大きさ)
            {
                //サイズがわかるような機能入れる

            }

            return 0;
        }



        #endregion




        #endregion




        #endregion

        #endregion





        ///<summary>
        ///　道中での回復を行う
        ///　一度全条件で検索してハズレ引いたら五秒間判定おこなわないように
        ///　それから非戦闘時に回復モードにするのをお忘れなく
        /// </summary>
        #region　非戦闘時回復用のAI






        #region　AI判断部分の管理


        /// <summary>
        /// ここで回復を管理する
        /// </summary>
        async UniTaskVoid HealingJudge()
        {

            //クールタイム中でスキップ条件もないならクールタイム終わるまで待とうね
            if (disEnable && (int)skipCondition == 0)
            {
                await UniTask.WaitUntil(() => disEnable, cancellationToken: magicCancel.Token);
            }

            //もし通常状態じゃないならそれまで魔法開始を待つ
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
            {
                await UniTask.WaitUntil(() => _condition.CurrentState == CharacterStates.CharacterConditions.Normal, cancellationToken: magicCancel.Token);
            }


            int useNum;


            //番号獲得
            useNum = FirstHealCheck();

            if (useNum == 99)
            {
                //スキップ条件があってクールタイム中なら再帰呼び出し
                if (disEnable)
                {

                    //  道中回復は四秒待って再帰呼び出し
                    //また条件判断するよ
                    await UniTask.Delay(TimeSpan.FromSeconds(4), cancellationToken: magicCancel.Token);


                    HealingJudge().Forget();
                }

                return;
            }

            //判断に使うデータ
            //完全にヒールで
            SisterConditionBase useCondition = sister.nRecoverCondition[useNum];
            

            //何もしないなら戻る
            //クールタイムある場合は分岐してもいいかも
            if (useCondition.selectAction == UseAction.なにもしない)
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }


            //ターゲットの絞り込みを続ける
            //ターゲットの番号を割り出す
            targetNum = TargetSelectStart(useCondition.selectCondition);
            candidateList.Clear();

            //ここから魔法判断
            //条件に当てはまる魔法がないなら戻る
            if (!MagicJudge(useCondition.bulletCondition))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }

            //ここから攻撃支援回復ごとに処理がわかれる
            //当てはまらなかったら戻る
            if (!HealSpecificCheck(useNum))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }


            //ここでMPと射程距離もチェックする
            if (!NecessaryCheck(useCondition.mpCheck, useCondition.rangeCheck))
            {
                //初期化して戻る
                candidateList.Clear();
                return;
            }




            //ターゲットに優先順位をつけて選択する
            useMagicNum = MagicSelect(useCondition.magicSort, useCondition.bulletASOder);
            candidateList.Clear();


                coolTime = sister.nRecoverCondition[useNum].coolTime;


            //魔法詠唱開始
            CastMagic().Forget();

        }


        #region 最初に呼ばれる行動できるかを判断するコード

        /// <summary>
        /// 最初に行動条件を満たしているのかを確認する
        /// Disenable中は スキップ条件を見て値を返す
        /// </summary>
        /// <returns></returns>
        int FirstHealCheck()
        {

            //ここでは待機回復の条件を使う
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
        /// ステートごとに特化したチェックを行う
        /// </summary>
        /// <returns></returns>
        bool HealSpecificCheck(int useNum)
        {


                RecoverCondition useCondition = sister.nRecoverCondition[useNum];

                //条件が当てはまらないならfalseを返す
                if (!HealConditionJudge(useCondition.secondActJudge))
                {
                    return false;
                }

                //条件が当てはまらないならfalseを返す
                return SupportConditionJudge(useCondition.healSupport);
            
        }


        #endregion







        #endregion












        /// <summary>
        /// バフの数値を与える
        /// 弾丸からfireAbillity.BuffCalcuで自分のインスタンスを渡して呼ぶ
        /// ここで他にもいろいろしちゃう？
        /// </summary>
        public void BuffCalc(FireBullet _fire)
        {
            _fire.attackFactor = attackFactor;
            _fire.fireATFactor = fireATFactor;
            _fire.thunderATFactor = thunderATFactor;
            _fire.darkATFactor = darkATFactor;
            _fire.holyATFactor = holyATFactor;
        }



        #region アニメ関連

        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_actParameterName, AnimatorControllerParameterType.Int, out _actAnimationParameter);
            RegisterAnimatorParameter(_motionParameterName, AnimatorControllerParameterType.Int, out _motionAnimationParameter);
        }

        /// <summary>
        /// これをオーバーライドすると、キャラクターのアニメーターにパラメータを送信することができます。
        /// これは、Characterクラスによって、Early、normal、Late process()の後に、1サイクルごとに1回呼び出される。
        /// </summary>
        public override void UpdateAnimator()
        {
            //クラウチングに気をつけろよ
            //MasicUseとCastnowを組み合わせようか
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