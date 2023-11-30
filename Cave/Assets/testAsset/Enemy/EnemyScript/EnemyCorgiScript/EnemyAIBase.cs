
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using Unity.Mathematics;
using MonKey.Extensions;
using static EnemyStatus;
using static CombatManager;
using static RenownedGames.ApexEditor.SerializedMember;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{


    /// <summary>
    /// もっとインスペクタだけで動かせるように改良
    /// あといくつか、近づいて殴るだけとかは汎用AIとして用意しとこう
    /// そこにステータスで移動速度とか間合いとか追従協力属性だったりの変更で対応できる
    /// 
    /// 
    /// 課題
    /// レベルマネージャーによってセグメントごと消されたらどうするか
    /// 次のセグメントには追いかけてこないのか
    /// セグメントとの親子関係を消去するか
    /// プレイヤーがセグメント離れたらトークン停止してスリープ？
    /// 
    /// </summary>
    public class EnemyAIBase : NPCControllerAbillity
    {

        #region 実装


        /// <summary>
        /// ターゲットの情報
        /// 最低限
        /// 行動決めたりするのにも使う？
        /// じゃあ番号位置バフ付きデバフ付き
        /// 弱点、属性、タイプくらいかな
        /// </summary>
        public struct TargetImfomation
        {
            /// <summary>
            /// ターゲットの位置
            /// </summary>
            public Vector2 targetPosition;

            /// <summary>
            /// ターゲットの番号
            /// </summary>
            public int targetNum;

            public int targetID;

        }


        #endregion



        //アビリティセットをルート下においてやる

        //「//sAni.」を「sAni.」で置き換える。アニメ作ったら

        //アニメの種類
        //必須
        //移動（Move）、歩き（Walk）、走り（Dash）、怯み（Falter）、ダウン（Down）、起き上がり（Wakeup）
        //吹き飛び（Blow）、弾かれ（Bounce）、後ろ歩き（BackMove）、戦闘待機（Pose）、立つ（Stand）、死亡（NDie,DDie）
        //
        //必要に応じて
        //落下（Fall）、ガード（Guard）、ガード移動（GuardMove）、後ろガード移動（BackGuard）、ガードブレイク（GuardBreak）
        //回避（Avoid）、ジャンプ（Jump）
        //
        //旧パラメータ
        #region
        [SerializeField] GameObject effectController;





        [Header("射出地点")]
        ///<summary>
        ///弾丸出す場所
        ///</summary>
        public Transform firePosition;

        /// <summary>
        /// ジャスガ不可エフェクトを出すとこ
        /// </summary>
        [SerializeField]
        Transform dispary;

        /// <summary>
        /// カメラに写ってるか
        /// </summary>
        [HideInInspector] public bool cameraRendered;



        [Header("エネミーのステータス")]
        public EnemyStatus status;



        // === 外部パラメータ ======================================

        /// <summary>
        /// 攻撃モードであるかどうか
        /// </summary>
        /*[HideInInspector]*/
        public bool isAggressive;//攻撃モードの敵

        /// <summary>
        /// 戦闘とかでもち場を離れたとき戻るためのフラグ
        /// </summary>
        protected bool posiReset;



        protected float escapeTime;//数えるだけ



        /// <summary>
        /// 目標の間合いにいる
        /// </summary>
        protected bool isReach;


        protected int initialLayer;


        /// <summary>
        /// ガード判定するかどうか
        /// これが真の間は毎回移動開始時にガード判定をする
        /// </summary>
        protected bool guardJudge;


        //protected bool isDamage;

        /// <summary>
        /// 今動けるかどうか
        /// </summary>
        protected bool isMovable;
        [HideInInspector] public EnemyStatus.MoveState ground = EnemyStatus.MoveState.wakeup;
        [HideInInspector] public EnemyStatus.MoveState air = EnemyStatus.MoveState.wakeup;

        List<Transform> mattTrans = new List<Transform>();


        protected int attackNumber;//何番の攻撃をしているのか

        /// <summary>
        /// 攻撃のクールタイムを消化したか
        /// </summary>
        protected bool isAtEnable = true;
        /// <summary>
        /// 攻撃の値
        /// </summary>
        [HideInInspector] public EnemyValue atV;


        /// <summary>
        /// 攻撃間隔時間をはかるのに使うFloat
        /// </summary>
        float attackChanceTime;

        /// <summary>
        /// 技ごとのクールタイム
        /// </summary>
        List<float> skillCoolTime;

        /// <summary>
        /// 技の数
        /// </summary>
        int skillCount;

        /// <summary>
        /// 直近で使ったスキルの番号
        /// </summary>
        int useSkillNum;

        /// <summary>
        /// 補足行動をする
        /// 
        /// </summary>
        int suppleNumber;

        /// <summary>
        /// 敵に当たったか
        /// 攻撃終了時にFalseする
        /// </summary>
        bool isHit;

        /// <summary>
        /// 個体識別ID 
        /// 戦闘参加時に割り当ててもらう
        /// </summary>
        int myID;
  

        // === キャッシュ ==========================================

        [Header("ドッグパイル")]
        ///<summary>
        ///移動の中心となるオブジェクト。敵を引き連れたり
        ///</summary>
        public GameObject dogPile;

        protected Rigidbody2D dRb;//ドッグパイルのリジッドボディ
       protected Vector2 startPosition;//開始位置と待機時テリトリーの起点
        protected Vector2 basePosition;

        #region ターゲット関連

        /// <summary>
        /// ターゲットとの距離
        /// </summary>
        protected Vector2 distance;//

        //自分の位置
        protected Vector2 myPosition;

        /// <summary>
        /// 最初に向いてた向き
        /// </summary>
        protected int firstDirection;

        protected int direction;
        protected int directionY;

        /// <summary>
        /// 現在ターゲットにしてる相手の情報
        /// </summary>
        protected TargetImfomation targetImfo;

        /// <summary>
        /// ヘイトリスト
        /// 数値を入れる
        /// 初期化は最初と、敵数変動イベントでやる
        /// 消えた敵の番号を通知してくれるから削除する
        /// </summary>
        public List<float> _hateList;

        /// <summary>
        /// 攻撃お控えモードになる
        /// フラグ
        /// </summary>
        public bool attackStop;

        #endregion

 

        /// <summary>
        /// 元の位置に戻った時どちらを向いているか
        /// </summary>
        protected int baseDirection;

        // === 内部パラメータ ======================================






        protected float waitCast;

        [HideInInspector] public bool guardHit;

        /// <summary>
        /// 今次の移動判断で逃走する状態かどうか
        /// </summary>
        bool isEscape;


        protected int lastArmor = 0;

        /// <summary>
        /// アーマー機能中かどうか
        /// 攻撃の途中のアニメで有効にして亜―マ発生のタイミングを選ぶ
        /// </summary>
        bool isArmor;


        /// <summary>
        /// 次の移動判定でガードする確率
        /// </summary>
        int guardProballity = 100;


        /// <summary>
        /// これがオンの間ガード使用中
        /// あと走らない
        /// ガード中止や逃走で解除
        /// </summary>
        protected bool useGuard;
        //-----------------------------------------------------

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

        protected float attackBuff = 1;//攻撃倍率
                                       //ステータス
                                       //HPはヘルスだろ
        public float maxHp = 100;

        //この辺いらないかも
        //個別に持たせた倍率で操作すればよくね？

        //　無属性防御力。体力で上がる
        public float Def = 70;
        //刺突防御。筋力で上がる
        public float pierDef = 70;
        //打撃防御、技量で上がる
        public float strDef = 70;
        //神聖防御、筋と賢さで上がる。
        public float holyDef = 70;
        //闇防御。賢さで上がる
        public float darkDef = 70;
        //炎防御。賢さと生命で上がる
        public float fireDef = 70;
        //雷防御。賢さと持久で上がる。
        public float thunderDef = 70;


        [HideInInspector]
        public float nowArmor;


        //-------------------------------------------
        [Header("戦闘中の移動速度")]
        public Vector2 combatSpeed;
        [Header("参照用の攻撃力")]
        /// <summary>
        /// 参照用の攻撃力
        /// </summary>
        public int atkDisplay;
        [Header("参照用の防御力")]
        /// <summary>
        /// 参照用の防御力
        /// </summary>
        public int defDisplay;
        //---------------------------------------------



        [SerializeField]
        protected SpriteRenderer td;




        /// <summary>
        /// エフェクト操作用のマテリアル
        /// </summary>
        [SerializeField]
        protected Renderer parentMatt;
        /// <summary>
        /// マテリアル操作始動フラグ
        /// </summary>
        protected int materialSet;
        /// <summary>
        /// 操作対象のマテリアル管理ナンバー。0から
        /// </summary>
        int mattControllNum;
        /// <summary>
        /// 操作対象のスプライト一覧
        /// </summary>
        public Transform[] spriteList;
        /*
        /// <summary>
        /// マテリアルがセットされたか
        /// </summary>
        protected bool spriteSet;*/
        protected List<Renderer> controllTarget = new List<Renderer>();
        /// <summary>
        /// マテリアル操作のために必要な時間計測
        /// </summary>
        float materialConTime;


        #endregion

        //新パラメータ
        #region



        public PlayerJump _jump;
        //　protected int _numberOfJumps = 0;

        public PlayerRoll _rolling;

        public PlayerRunning _characterRun;

        public EnemyFly _flying;

        public GuardAbillity _guard;

        public MyWakeUp _wakeup;

        public AtEffectCon _atEf;

        public EAttackCon _attack;

        [SerializeField]
        public MyDamageOntouch _damage;

        public ESensorAbillity _sensor;
        private bool isVertical;

        public ParryAbility _parry;

        public MyAttackMove _rush;


        #endregion



        protected const string _suppleParameterName = "suppleAct";
        protected int _suppleAnimationParameter;

        protected const string _combatParameterName = "isCombat";
        protected int _combatAnimationParameter;


        /// <summary>
        /// 今シスターさんのセンサー内にいるかどうか
        /// </summary>
        bool _seenNow;



        /// <summary>
        /// シスターさんがこの敵を見失ってる時間
        /// </summary>
        float loseSightTime;



        /// <summary>
        /// キャラクターの変化データ
        /// </summary>
        ConditionData nowCondition;


        public CancellationTokenSource patrolToken = new CancellationTokenSource();
        public CancellationTokenSource agrToken = new CancellationTokenSource();

        #region モード関連の変数

        /// <summary>
        /// 現在のモード
        /// ModeBehaiviorで変わる
        /// 0から
        /// </summary>
        protected int nowMode = 0;

        /// <summary>
        /// チェンジ可能なモードの数
        /// </summary>
        int modeCount;

        /// <summary>
        /// そのモードになった時間
        /// </summary>
        float modeTime;

        #endregion



        // === コード（Monobehaviour基本機能の実装） ================
        protected override void Initialization()
        {
            base.Initialization();

            //ステータス設定

            ParameterSet(status);
            ArmorReset();
            HPReset();


            //情報保存
            startPosition = transform.position;
            firstDirection = Math.Sign(transform.localScale.x);
            myPosition.Set(startPosition.x, startPosition.y);
            initialLayer = this.gameObject.layer;


            if (dogPile != null)
            {
                basePosition = dogPile.transform.position;
                baseDirection = Math.Sign(dogPile.transform.localScale.x);
                dRb = dogPile.GetComponent<Rigidbody2D>();
            }
            else
            {
                basePosition = startPosition;
                baseDirection = firstDirection;

            }

            //モードの数を確認
            //モード変更条件が一つもないならモード変更しない
            modeCount = status.modeChangeCondition.Length;

            //モードになった時間も初期化
            modeTime = GManager.instance.nowTime;

            //
            skillCount = status.AttackCondition.Length;
            skillCoolTime = new List<float>(skillCount);

            for (int i = 0; i < skillCount; i++)
            {

                if (status.AttackCondition[i].skilLevel == 0)
                {
                    skillCoolTime[i] = -100;
                }
                else
                {
                    skillCoolTime[i] = GManager.instance.nowTime;
                }
            }

            //最初にどちらかのモードを起動
            if (!isAggressive)
            {
                PatrolAction();
            }
            else
            {
                CombatAction();
            }

        }


        /// <summary>
        /// 最初のマテリアルの無効有効設定
        /// </summary>
        protected void MaterialSet()
        {
            //親マテリアルの情報を見る
            //	Debug.Log($"{parentMatt.material}");
            //全部のスプライトを集めて設定する
            for (int i = 0; i < spriteList.Length; i++)
            {

                GetAllChildren(spriteList[i]);
                //	await UniTask.WaitForFixedUpdate();
            }

            Material coppy = controllTarget[0].material;

            //何らかの条件でいじり方変える
            if (true)
            {
                coppy.EnableKeyword("Fade_ON");
                coppy.DisableKeyword("BLUR_ON");
                coppy.DisableKeyword("MOTIONBLUR_ON");
            }

            for (int i = 0; i <= controllTarget.Count - 1; i++)
            {
                controllTarget[i].material.CopyPropertiesFromMaterial(coppy);

            }

            controllTarget.Clear();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            Brain();

            LoseSightWait();
            //	Debug.Log($"かどうか{_animator.name}");


        }





        #region 管理コード

        /// <summary>
        /// 共通で毎フレーム繰り返す処理
        /// </summary>
        public void Brain()
        {
            //Debug.Log($"dsfwe");
            //カメラに写ってないときは止まります。
            if (!cameraRendered && !status.unBaind)
            {


                //縛られないなら先に
                return;
            }

            //位置情報更新
            myPosition.Set(transform.position.x, transform.position.y);



            //移動可能かどうか確認
            //ターゲットナンバー99は敵不在の意味なので停止
            if (guardHit || _condition.CurrentState != CharacterStates.CharacterConditions.Normal || targetImfo.targetNum == 99)
            {

                isMovable = false;

            }
            //空中回避とか空中攻撃あるかも
            /*
            else if (!_controller.State.IsGrounded && status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                isMovable = false;
        }*/
            else
            {

                isMovable = true;
            }

            //ドッグパイルありますか

            Debug.Log($"チェック{myPosition.x >= startPosition.x + status.waitDistance.x}");

            //うごける時だけ
            if (isMovable)
            {


                ////////Debug.log($"ジャンプ中{nowJump}");
                ////////Debug.log($"回避中{_movement.CurrentState != CharacterStates.MovementStates.Rolling}");
                if (isAggressive)
                {
                    //距離更新
                    distance = targetImfo.targetPosition - myPosition;
                    direction = distance.x >= 0 ? 1 : -1;

                    //ターゲットが逃げてるかチェック
                    EscapeCheck();






                    //戦闘中繰り返す処理
                    CombatLoop();

                }

                else if (!isAggressive)
                {

                    if (posiReset)
                    {
                        PositionReset();
                    }

                    //警戒中繰り返す処理
                    PatrolLoop();
                }
            }



            //トリガーで呼びましょう
            JumpController();


            MaterialControll();



        }

        ///<summary
        ///　ターゲットを設定してその情報を獲得する
        ///　考慮すべき情報は敵へのヘイト、そして同じ敵を狙ってる味方が何人いるか
        ///　そして近いか遠いか？
        ///　
        ///</summary>
       　async UniTaskVoid  TargetSet(bool isFirst = false)
        {

            //最初は攻撃してきたやつがターゲットなので

            //最初以外決めた時間待ってチェック
            //またはナンバーが99で標的死亡、即時チェックへ
            await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(status.hateBehaivior.TargetChangeTime), cancellationToken: agrToken.Token),
                UniTask.WaitUntil(()=> targetImfo.targetNum == 99, cancellationToken: agrToken.Token));

            int count = EnemyManager.instance._targetList.Count;

            //カウントがゼロの時戦闘終了
            if(count== 0)
            {
                CombatEnd();
            }


            int utillityCount = status.hateBehaivior.priorityTarget.Length;

            //優先タイプあるならヘイト加算処理
            if (utillityCount > 0)
            {
                
                for (int i = 0; i < utillityCount; i++)
                {

                    //一人一人かけていく感じか
                    for (int s = 0; s < count; s++)
                    {

                        //まずはターゲット補正


                        //キャラの属性一致するか指定なしなら
                        if (status.hateBehaivior.priorityTarget[s].targetType != CharacterStatus.CharaType.none &&
                            status.hateBehaivior.priorityTarget[s].targetType != EnemyManager.instance._targetList[i]._baseData._type)
                        {
                            return;
                        }
                        //キャラの種類が一致するか指定なしなら先へ
                        if (status.hateBehaivior.priorityTarget[s].targetKind != CharacterStatus.KindofEnemy.none &&
                            status.hateBehaivior.priorityTarget[s].targetKind != EnemyManager.instance._targetList[i]._baseData._kind)
                        {
                            return;
                        }

                        /*
                        //ヘイト加算処理
                        float addHate;

                        //増減の最低保証
                        float insurance = status.hateBehaivior.priorityTarget[s].initialHate * (status.hateBehaivior.priorityTarget[s].addRatio - 0.5f);

                        //ヘイトが0以下の時は倍率かけてもあんま意味ない
                        if (_hateList[i] >= 0)
                        {

                            //加算なら最初のヘイトに倍率かけたのを足す
                            //減算ならもう何もしない
                            if (status.hateBehaivior.priorityTarget[s].addRatio > 1)
                            {
                            addHate =  insurance;
                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {
                            //加算する分を計算
                            addHate = _hateList[i] * (status.hateBehaivior.priorityTarget[s].addRatio - 1);

                            //増加分が最低保証より小さいなら
                            addHate = insurance > addHate ? addHate : insurance;
                        }

                        //加算完了
                        _hateList[i] += addHate;
                        */


                        float addHate = status.hateBehaivior.priorityTarget[s].initialHate / 2;
                        //加算完了
                        _hateList[i] = !status.hateBehaivior.priorityTarget[s].isDecrease ? _hateList[i] + addHate : _hateList[i] - addHate;



                    }
                }
            }



            //ここから条件効果
            utillityCount = status.hateBehaivior.hateEffect.Length;

            if (utillityCount > 0)
            {
                Vector2[] container;
                    Vector2 set = new Vector2(99,0);
                for (int i = 0; i < utillityCount; i++)
                {
                    //比較しない、平等な条件
                    bool isFair = false;

                    container = new Vector2[3] {set,set,set};
                    bool half = (int)status.hateBehaivior.hateEffect[i]._influence > 3;

                    for (int s = 0; s < count; s++)
                    {



                        if (!half)
                        {
                            if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.交戦距離)
                            {


                                //交戦距離と敵との距離の差を出す
                                float jDistance = Vector2.SqrMagnitude(myPosition - EnemyManager.instance._targetList[s]._condition.targetPosition);

                                //交戦距離より先
                                //実際の距離の方が大きいなら以遠になる
                                if (status.hateBehaivior.hateEffect[i].isHigh && Vector2.SqrMagnitude(Vector2.zero - status.agrDistance[nowMode]) - jDistance < 0)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //距離が近い方優先
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }

                                }

                                //以内
                                else if (!status.hateBehaivior.hateEffect[i].isHigh && Vector2.SqrMagnitude(Vector2.zero - status.agrDistance[nowMode]) - jDistance >= 0)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //距離が近い方優先
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }
                                }




                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.距離順)
                            {

                                //交戦距離と敵との距離の差を出す
                                float jDistance = Math.Abs(Vector2.SqrMagnitude(myPosition - EnemyManager.instance._targetList[s]._condition.targetPosition));

                                //遠い順
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //距離が近い方優先
                                        if (container[t].x == 99 || (container[t].y > jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }

                                }

                                //近い順
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //距離が遠い方優先
                                        if (container[t].x == 99 || (container[t].y < jDistance))
                                        {
                                            container[t].Set(s, jDistance);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.HP割合)
                            {
                                    float ratio = EnemyManager.instance._targetList[s]._condition.hpRatio;

                                //高い順
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {


                                        //割合が高い方優先
                                        if (container[t].x == 99 || (container[t].y < ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }

                                //低い順
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //割合が低い方優先
                                        if (container[t].x == 99 || (container[t].y > ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.MP割合)
                            {

                                float ratio = EnemyManager.instance._targetList[s]._condition.mpRatio;

                                //高い順
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {


                                        //割合が高い方優先
                                        if (container[t].x == 99 || (container[t].y < ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }

                                //低い順
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //割合が低い方優先
                                        if (container[t].x == 99 || (container[t].y > ratio))
                                        {
                                            container[t].Set(s, ratio);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.強化状態)
                            {
                                isFair = true;
                                //高い順。バフあり
                                if (status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.buffImfo != 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //とりあえず三人集める
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //もう三人取ったならsループを終了する
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }

                                //低い順。バフなし
                                else if(!status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.buffImfo == 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //もう三人取ったならsループを終了する
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }
                            }
                            
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.デバフ状態)
                            {
                                isFair = true;

                                //高い順
                                if (status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.debuffImfo != 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //とりあえず三人集める
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //もう三人取ったならsループを終了する
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }

                                //低い順。デバフなし
                                else if (!status.hateBehaivior.hateEffect[i].isHigh && EnemyManager.instance._targetList[s]._condition.debuffImfo == 0)
                                {
                                    bool isEnd = true;
                                    for (int t = 0; t < 3; t++)
                                    {
                                        if (container[t].x == 99)
                                        {
                                            isEnd = false;
                                            container[t].Set(s, 0);
                                            break;
                                        }
                                    }
                                    //もう三人取ったならsループを終了する
                                    if (isEnd)
                                    {
                                        break;
                                    }
                                }
                            }

                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.攻撃力)
                            {

                                //攻撃力をみる
                                float value = EnemyManager.instance._targetList[s]._baseData.displayAtk;

                                //高い順
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //高い方優先
                                        if (container[t].x == 99 || (container[t].y < value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }

                                }

                                //近い順
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //低い方優先
                                        if (container[t].x == 99 || (container[t].y > value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }
                                }
                            }
                            else if (status.hateBehaivior.hateEffect[i]._influence == EnemyStatus.HateInfluence.防御力)
                            {
                                //防御力を見る
                                float value = EnemyManager.instance._targetList[s]._baseData.displayAtk;

                                //高い順
                                if (status.hateBehaivior.hateEffect[i].isHigh)
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //高い方優先
                                        if (container[t].x == 99 || (container[t].y < value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }

                                }

                                //近い順
                                else
                                {
                                    for (int t = 0; t < 3; t++)
                                    {
                                        //低い方優先
                                        if (container[t].x == 99 || (container[t].y > value))
                                        {
                                            container[t].Set(s, value);
                                            break;
                                        }
                                    }
                                }
                            }

                        }


                    }

                    //最後にヘイト処理
                    for (int s = 1; s < 4; s++)
                    {
                        //xが99なら流しで
                        if (container[s].x == 99)
                        {
                            continue;
                        }

                        //優劣がついてるなら
                        if (!isFair)
                        {
                            _hateList[(int)container[s].x] += 30 / s;
                        }
                        else
                        {
                            _hateList[(int)container[s].x] += 20;
                        }
                    }
                }

            }



            //ヘイト値加工したからターゲット決定
            float nowHate = 0;

            for (int i = 0; i < count; i++)
            {
                //最初かヘイトが上なら
                if(i == 0 || _hateList[i] >= nowHate)
                {
                    nowHate = _hateList[i];
                    utillityCount = i;
                }

            }


            //ターゲットをマネージャーに報告
            //そしてターゲットのIDをゲット
            targetImfo.targetID = EnemyManager.instance.TargetSet(utillityCount,targetImfo.targetNum,status.hateBehaivior._event,(int)status.hateBehaivior.level,myID,isFirst);

            //ターゲット決定後の処理
            //ターゲット決定メソッドをマネージャーから呼び出したり
            //ターゲット決定メソッドにはターゲット決定時イベントを持たせる
            //あとは攻撃ロックモードにしたり
            targetImfo.targetNum = utillityCount;


            //攻撃ロック数が設定されててなおかつ数があふれてれば
            //ターゲットセットの時だけこれ判断する
            //攻撃我慢する人に選ばれたやつは距離とかに気を遣って動かないように

            int needCount = status.hateBehaivior.attackLockCount;

            if (needCount != 0 &&
                needCount <= EnemyManager.instance.TargettingCount((int)status.hateBehaivior.level, targetImfo.targetNum))
            {

                //距離を見て、条件満たすなら攻撃お控えモードにチェンジ
                //お控えフラグ立てて次のモード変更で抑制
                attackStop = (EnemyManager.instance.AttackStopCheck(targetImfo.targetNum, (int)status.hateBehaivior.level, Vector2.SqrMagnitude(Vector2.zero - distance), needCount,myID));


            }
            //再呼び出し
            TargetSet().Forget();
        }


        /// <summary>
        /// 攻撃間隔が経過したら繰り返される処理
        /// 戦闘中のAI部分
        /// これはオーバーライドしない
        /// </summary>
        async UniTaskVoid AttackBehaivior()
        {

            //攻撃頻度だけ待つ
            await UniTask.Delay(TimeSpan.FromSeconds(status.attackFrquency),cancellationToken:agrToken.Token);

            //状態が正常になるまで待つ
            await UniTask.WaitUntil(()=>(isMovable && isAtEnable),cancellationToken:agrToken.Token);

            //攻撃中じゃないなら

                float time = GManager.instance.nowTime;
                int count = status.AttackCondition.Length;

                //使用する攻撃の番号
                int useNum = 100;

                //使用するスキルのレベル
                //高い方が優先
                int useLevel = -1;
                //条件見ていく
                for (int i = 0; i < count; i++)
                {



                    //全部のモードで出せなくて、かつ条件が今のモードでないなら
                    if (status.AttackCondition[i]._attackMode != EnemyStatus.Mode.AllMode && (int)status.AttackCondition[i]._attackMode != nowMode)
                    {
                        continue;
                    }

                    //ちゃんと必要なだけ待ってないなら
                    if (status.AttackCondition[i].coolTime > time - skillCoolTime[i])
                    {
                        continue;
                    }

                    float dist = Math.Abs(distance.x);

                    //距離関係あって、なおかつxとyの距離の間にいないなら
                    if ((status.AttackCondition[i].useDistance != Vector2.zero) && !(status.AttackCondition[i].useDistance.x <= dist && status.AttackCondition[i].useDistance.y >= dist))
                    {
                        dist = Math.Abs(distance.y);

                        //高度差関係あってなおかつ差があるなら
                        if ((status.AttackCondition[i].useHeight != 0) && (status.AttackCondition[i].useHeight < dist))
                        {
                            continue;
                        }

                    }
                    //確率を外したなら
                    if (status.AttackCondition[i].probability < RandomValue(0, 100))
                    {
                        //クールタイムもリセット
                        skillCoolTime[i] = time;
                        continue;
                    }
                    //スキルレベルが低いなら優先しない
                    if (status.AttackCondition[i].skilLevel < useLevel)
                    {
                        continue;
                    }

                    useLevel = status.AttackCondition[i].skilLevel;

                    useNum = i;
                }

                //番号が100から変わってるなら
                if (useNum != 100)
                {
                    Attack(useNum);
                }

                //選べなかったならまた呼ぶ
                //選べた場合はループしない
                else
                {
                    AttackBehaivior().Forget();
                }
        }



        /// <summary>
        /// 戦闘モードを判断する
        /// 戦闘中のAI部分
        /// これはオーバーライドしない
        /// 体力の割合は自分の敵情報からとる
        /// </summary>
        async UniTaskVoid ModeBehaivior()
        {

                //1秒に一回チェック
                await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: agrToken.Token);
                
                //現在の時間
                float time = GManager.instance.nowTime;

                float testratio = 1;

                //変える先のモード
                int changeMode = 100;

                //直線距離
                float realDistance = Vector2.SqrMagnitude(myPosition - targetImfo.targetPosition);

                for (int i =0;i< modeCount;i++)
                {

                    //攻撃抑制フラグが立ってるなら攻撃抑制モード始動
                    //そしてループ終了
                    if (status.modeChangeCondition[i].isAttackStop && attackStop)
                    {
                        changeMode = i;
                        break;
                    }

                    //遷移元のモードが関係あってなおかつ異なっている
                    if(status.modeChangeCondition[i]._nowMode!= EnemyStatus.Mode.AllMode && (int)status.modeChangeCondition[i]._nowMode != nowMode)
                    {
                        continue;
                    }
                    //モード遷移に時間が関係あり、なおかつ時間が経過してない
                    else if (status.modeChangeCondition[i].changeTime != 0 && status.modeChangeCondition[i].changeTime > time - modeTime)
                    {
                        continue;
                    }
                    //HP割合が関係あって、指定以下の割合でないなら
                    else if (status.modeChangeCondition[i].healthRatio != 0 && status.modeChangeCondition[i].healthRatio < testratio)
                    {
                        continue;
                    }
                    //直線距離が関係あってなおかつ満たさないなら
                    //xより近いかyより離れている
                    else if (status.modeChangeCondition[i].changeDistance != Vector2.zero && (status.modeChangeCondition[i].changeDistance.x > realDistance || status.modeChangeCondition[i].changeDistance.y < realDistance) )
                    {
                        continue;
                    }
                    //今変更予定のモードよりレベルが低いならコンティニュー
                    if (changeMode != 100 && status.modeChangeCondition[i].modeLevel < status.modeChangeCondition[changeMode].modeLevel)
                    {
                        continue;
                    }

                    //以上の条件全てに当てはまらなかったら暫定候補に
                    changeMode = i;
                }

                if(changeMode != 100)
                {
                    nowMode = changeMode;

                    //モード変更時間も確定
                    modeTime = time;

                    //モードレベル最大ならもう遷移しない
                    if (status.modeChangeCondition[nowMode].modeLevel == 5)
                    {
                        modeCount = 0;
                    }

                    //モード変更イベントを呼ぶ
                    ModeChangeEvent(changeMode);
                }

                //モード変更を呼ぶ
                ModeBehaivior().Forget();
            
        }


        /// <summary>
        /// 攻撃停止状態にするためのチェックに使う
        /// </summary>
        /// <param name="target"></param>
        /// <param name="level"></param>
        /// <param name="cDistance"></param>
        /// <returns></returns>
        public bool ATBlockDistanceCheck(int target,int level, float cDistance,int id)
        {
            //ターゲットが違うかレベルが違うなら戻る
            if(target != targetImfo.targetNum || level > (int)status.hateBehaivior.level || myID == id)
            {
                return false;
            }

            //チェックの主体の方が距離が遠いなら
            //同じは多分自分だからノーカンで
            return cDistance > Vector2.SqrMagnitude(distance);

        }



        #endregion

        //オーバーライドして使えば様々なタイミングに処理を差し込める
        #region 自分のイベント群





        /// <summary>
        /// 攻撃後に何かするならここに入れる
        /// イベント
        /// </summary>
        protected void AttackEvent()
        {
            //コンボなら最終段まで何もしない
            if (atV.isCombo)
            {
                return;
            }

            //飛行敵は被弾後確実に逃げる
            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                EscapeJudge(100);
            }
            //そうでないなら確率で
            else
            {
                EscapeJudge(atV.escapePercentage);

            }

            //攻撃間隔リセット
            attackChanceTime = GManager.instance.nowTime;

            //スキルのクールタイムリセット
            skillCoolTime[useSkillNum] = GManager.instance.nowTime;
        }


        /// <summary>
        /// 攻撃がヒットした時にどのようにヒットしたかを含めて教える
        /// </summary>
        /// <param name="isBack">当てた相手が自分の後ろにいる時は真</param>
        public override void HitReport(bool isBack)
        {
            isHit = true;

        }

        /// <summary>
        /// 攻撃戦闘開始時に起動する処理
		/// AgrMoveなど
        /// </summary>
        protected void CombatAction()
        {
            if(skillCount > 0)
            {
                AttackBehaivior().Forget();
            }
            if(modeCount > 0)
            {
                ModeBehaivior().Forget();
            }
            TargetSet(true).Forget();

            //バトルに参加
            EnemyManager.instance.JoinBattle(this);

        }


        /// <summary>
        /// 戦闘中に繰り返す処理
		/// Update部分
        /// ベース呼び出す必要なし
        /// </summary>
        protected void CombatLoop()
        {


        }

        /// <summary>
        /// モード変更時に何かする処理
        /// 引数から変更したモードを知れる
        /// </summary>
        /// <param name="mode"></param>
        protected void ModeChangeEvent(int mode)
        {

        }

        /// <summary>
        /// 攻撃受けた後のイベント
        /// Enemyは敵のゲームオブジェクト
        /// </summary>
        public override void DamageEvent(bool isStun, GameObject enemy, int damage, bool back)
        {

            if (!isAggressive)
            {
                StartCombat(enemy);
            }

 

            //アーマー回復をリセット
            lastArmor = 0;

            //ヒットストップ
            if (isStun)
            {
                _controller.SetForce(Vector2.zero);
            }

            //飛行敵は被弾後確実に逃げる
            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                EscapeJudge();
            }


        }


        /// <summary>
        /// 待機状態でどのような行動を行うかを書く部分
        /// ここではループしない処理を
        /// </summary>
        protected void PatrolAction()
        {
            //陸
            if(status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                //スピードあるなら
                if (status.patrolSpeed.x != 0)
                {
                    PatrolMove().Forget();
                }
                //待ち時間あるならくるくる向き変える
                else if (status.waitRes != 0)
                {
                    Wait().Forget();
                }
            }
            else
            {
                //スピードあるなら
                if (status.patrolSpeed != Vector2.zero)
                {
                    PatrolFly().Forget();
                }
                //待ち時間あるならくるくる向き変える
                else if (status.waitRes != 0)
                {
                    Wait().Forget();
                }
            }


        }

        /// <summary>
        /// 警戒行動中にループさせたい処理を入れる
        /// アップデート部分
        /// ベース呼び出す必要なし
        /// </summary>
        protected void PatrolLoop()
        {

        }



        /// <summary>
        /// ターゲットリストから削除されたエネミーを消し去る
        /// そしてヘイトリストやらを調整
        /// プレイヤーはなんか別の処理入れてもいいかもな
        /// あと敵の死を通知するメソッドとしても使える
        /// </summary>
        /// <param name="deletEnemy"></param>
        public override void TargetListChange(int deleteEnemy,int deadID)
        {
            //ヘイトリストから削除
            _hateList.RemoveAt(deleteEnemy);

            //ターゲット消えたならターゲット変更
            if(targetImfo.targetID == deadID)
            {
                targetImfo.targetNum = 99;
            }
            //ターゲットが削除要素より先にあるなら
            //一つ前へずらすよ
            else if (targetImfo.targetNum > deleteEnemy)
            {
                targetImfo.targetNum--;
            }
        }


        #endregion


        #region 味方との連携イベント群


        /// <summary>
        /// IDを確認
        /// これを使って味方と連携するので重複確認大事
        /// </summary>
        /// <returns></returns>
        public override int ReturnID()
        {
            return myID;
        }


        /// <summary>
        /// 味方がターゲットを決定した際
        /// イベントを持ってたら飛ばす
        /// </summary>
        public override void CommandEvent(TargetingEvent _event,int level, int targetNum,int commanderID)
        {
            //コマンダーが自分か、またはレベルが低いなら無視
            if(commanderID == myID || (int)status.hateBehaivior.level > level)
            {
                return;
            }
        }


        /// <summary>
        /// 味方との連携用
        /// エネミーマネージャーから呼び出す
        /// 引数にイベント種類渡して
        /// あとは体力が何割まで減ったらイベント飛ばす
        /// ダメージイベントとかもあっていいな
        /// </summary>
        public void AllyDeathEvent()
        {

        }


        #endregion


        #region 汎用ツール

        /// <summary>
        /// XとYの間でランダムな値を返す
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public int RandomValue(int X, int Y)
        {
            return UnityEngine.Random.Range(X, Y + 1);
        }


        /// <summary>
        /// アニメーションの終了を検知
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        bool CheckEnd(string Name = null)
        {
            if (Name != null)
            {

                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(Name))// || sAni.GetCurrentAnimatorStateInfo(0).IsName("OStand"))
                {   // ここに到達直後はnormalizedTimeが"Default"の経過時間を拾ってしまうので、Resultに遷移完了するまではreturnする。

                    return true;
                }
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {   // 待機時間を作りたいならば、ここの値を大きくする。
                    //	Debug.Log("あん2");
                    return true;
                }
                //AnimatorClipInfo[] clipInfo = sAni.GetCurrentAnimatorClipInfo(0);

                ////Debug.Log($"アニメ終了");

                return false;
            }
            else
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                {   // 待機時間を作りたいならば、ここの値を大きくする。
                    //	Debug.Log("あん2");
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// n秒待って処理というようなのを実装
        /// </summary>
        /// <returns></returns>
        protected async UniTaskVoid WaitAction(float waitTime, Action _action)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);

            _action();
        }

        /// <summary>
        /// 攻撃状態かそうでないかで挙動が変わる
		/// n秒待って処理というようなのを実装
        /// </summary>
        /// <returns></returns>
        protected async UniTaskVoid StateWaitAction(float waitTime, Action _action, bool agrAct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);
            if (agrAct && !isAggressive)
            {
                return;
            }
            else if (!agrAct && isAggressive)
            {
                return;
            }
            _action();
        }





        #endregion


        #region　挙動変更
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
            _controller.DefaultParameters.Gravity = -gravity;
        }

        #endregion


        #region アーマー関連


        /// <summary>
        /// アーマーをリセット
        /// 怯み中なら振り返る
        /// </summary>
        public override void ArmorReset()
        {
            nowArmor = status.Armor;

        }


        /// <summary>
        /// 被弾後に時間経過でアーマーを回復する
        /// これは多分普通のループ処理でやった方がいい？
        /// </summary>
        async UniTaskVoid ArmorRecover()
        {

            //三秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(3f), cancellationToken: agrToken.Token);

            //攻撃状態でなくスタンしてもいないなら
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Stunned && _movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                //五回回復したらフル回復
                if (lastArmor >= 4)
                {
                    ArmorReset();
                    lastArmor = 0;
                }
                else
                {
                    //五回までは６分の一ずつ
                    nowArmor = nowArmor + status.Armor / 6 > status.Armor ? status.Armor : nowArmor + status.Armor / 6;
                    lastArmor++;
                }
            }

            ArmorRecover().Forget();


        }


        /// <summary>
        /// 攻撃食らったとき
        /// アーマー値に応じてイベントとばす
        /// </summary>
        public override MyWakeUp.StunnType ArmorControll(float shock, bool isDown, bool isBack)
        {

            MyWakeUp.StunnType result = 0;

            if (_movement.CurrentState != CharacterStates.MovementStates.Attack)
            {
                atV.aditionalArmor = 0;
            }
            if ((_movement.CurrentState == CharacterStates.MovementStates.GuardMove || _movement.CurrentState == CharacterStates.MovementStates.Guard) && isBack)
            {
                _guard.GuardEnd();
            }
            if (!isBack && (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove))
            {
                _guard.GuardHit();
                if (isDown)
                {
                    nowArmor -= ((shock * 2) * status.guardPower / 100) * 1.2f;

                }
                else
                {
                    nowArmor -= (shock * 2) * ((100 - status.guardPower) / 100);
                }
                if (nowArmor <= 0)
                {
                    _guard.GuardEnd();
                }
            }

            else
            {
                if (!isArmor || atV.aditionalArmor == 0)
                {
                    nowArmor -= shock;

                }
                else
                {
                    nowArmor -= (shock - atV.aditionalArmor) < 0 ? 0 : (shock - atV.aditionalArmor);
                    atV.aditionalArmor = (atV.aditionalArmor - shock) < 0 ? 0 : atV.aditionalArmor - shock;

                }
            }

            if (nowArmor <= 0)
            {


                if (isDown)
                {
                    result = (MyWakeUp.StunnType.Down);
                }
                else
                {

                    if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
                    {
                        result = MyWakeUp.StunnType.GuardBreake;
                    }
                    //パリィは別発生
                    else
                    {
                        result = MyWakeUp.StunnType.Falter;
                    }

                }
                isAtEnable = false;
            }
            else
            {
                result = MyWakeUp.StunnType.notStunned;
            }


            return result;
        }


        /// <summary>
        /// ジャスガされた時アーマー値に応じてイベントとばす
        /// </summary>
        public override bool ParryArmorJudge()
        {

            nowArmor -= Mathf.Ceil(status.Armor * ((100 - atV.parryResist) / 100));

            if (nowArmor <= 0)
            {

                //	_wakeup.StartStunn(MyWakeUp.StunnType.Parried);
                return true;
            }
            else
            {
                Debug.Log($"あと{nowArmor}");
                return false;
            }
        }


        /// <summary>
        /// スタン開始
        /// ここで色々止めれるかも
        /// </summary>
        /// <param name="stunState"></param>
        public override void StartStun(MyWakeUp.StunnType stunState)
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
            {
                AttackEnd(true);
            }
            _wakeup.StartStunn(stunState);
        }



        /// <summary>
        /// 現在のスタン状況を返す
        /// </summary>
        /// <returns></returns>
        public override int GetStunState()
        {
            return _wakeup.GetStunState();
        }


        /// <summary>
        /// 空中で攻撃を受けた時ダウンするかどうかの判断
        /// </summary>
        /// <param name="stunnState"></param>
        /// <returns></returns>
        public override bool AirDownJudge(MyWakeUp.StunnType stunnState)
        {
            if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                if (stunnState == MyWakeUp.StunnType.Falter)
                {
                    return true;
                    //吹き飛ばし処理
                }
                else if (stunnState == MyWakeUp.StunnType.notStunned)
                {
                    //攻撃中でないなら特殊ダウン
                    if (!AttackCheck())
                    {
                        return true;
                        //吹き飛ばし処理
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// 死亡アニメーションを再生する
        /// 吹っ飛んで死ぬのか普通に死ぬのか
		/// エネミーに関してはこの時点で死亡したとみなして敵情報から抹消したりする
        /// </summary>
        /// <param name="stunState"></param>
        public override void DeadMotionStart(MyWakeUp.StunnType stunState)
        {
            //体力ゼロの時点で感知されないレイヤーに
            this.gameObject.layer = 15;

            EnemyManager.instance.Die(myID,this);

            //ターゲットではなくなる
            if (SManager.instance.target == this.gameObject)
            {
                SManager.instance.target = null;
            }

            //ターゲットリストから自分を削除
            SManager.instance.RemoveEnemy(this.gameObject);

            //エフェクトも消して
            TargetEffectCon(1);



            //空飛ぶ敵はダウンして死ぬ
            if ((status._charaData._kind == EnemyStatus.KindofEnemy.Fly))
            {
                _controller.SetHorizontalForce(0);
                stunState = MyWakeUp.StunnType.BlowDie;
            }

            _wakeup.StartStunn(stunState);

        }




        #endregion

        #region ジャンプトリガー
        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {



            if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
            {
                //ジャンプ方向合ってるなら
                if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
                {
                    JumpAct();
                    Debug.Log($"sss");
                }
            }

        }
        protected virtual void OnTriggerStay2D(Collider2D collision)
        {
            if (_condition.CurrentState != CharacterStates.CharacterConditions.Dead)
            {

                if (isAggressive && collision.tag == EnemyManager.instance.JumpTag && _controller.State.IsGrounded)
                {
                    if (collision.gameObject.MMGetComponentNoAlloc<JumpTrigger>().jumpDirection == transform.localScale.x)
                    {
                        JumpAct();
                        Debug.Log($"dfggrferfer");
                    }
                }
            }
        }

        #endregion


        #region データセット

        /// <summary>
        /// ジャンプ力などを変数に反映していく
        /// </summary>
        /// <param name="status"></param>
        protected void ParameterSet(EnemyStatus status)
        {
            ///<summary>
            ///　リスト
            /// </summary>
            /// 
            _characterHorizontalMovement.FlipCharacterToFaceDirection = false;

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
            if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
            {
                GravitySet(status.firstGravity);
                #region
                /*
				 		/// the speed of the character when it's walking
		[Tooltip("the speed of the character when it's walking")]
		public float WalkSpeed = 6f;
		/// the multiplier to apply to the horizontal movement
		//　読み取り専用。コードから変えてね
		[MMReadOnly]
		[Tooltip("水平方向の移動に適用する倍率")]
		public float MovementSpeedMultiplier = 1f;
		/// the multiplier to apply to the horizontal movement, dedicated to abilities
		[MMReadOnly]
		[Tooltip("水平方向の移動に適用する倍率で、アビリティに特化しています。")]
		public float AbilityMovementSpeedMultiplier = 1f;
        /// the multiplier to apply when pushing
        [MMReadOnly]
		[Tooltip("the multiplier to apply when pushing")]
		public float PushSpeedMultiplier = 1f;
        /// the multiplier that gets set and applied by CharacterSpeed
        [MMReadOnly]
        [Tooltip("the multiplier that gets set and applied by CharacterSpeed")]
        public float StateSpeedMultiplier = 1f;
        /// if this is true, the character will automatically flip to face its movement direction
        [Tooltip("if this is true, the character will automatically flip to face its movement direction")]
        public bool FlipCharacterToFaceDirection = true;


        /// the current horizontal movement force
		public float HorizontalMovementForce { get { return _horizontalMovementForce; }}
        /// if this is true, movement will be forbidden (as well as flip)
        public bool MovementForbidden { get; set; }

        [Header("Input")]

		/// if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()
		//  これが真の場合、入力ソースからの入力を取得します。そうでない場合は、SetHorizontalMove() で設定する必要があります。
		[Tooltip("if this is true, will get input from an input source, otherwise you'll have to set it via SetHorizontalMove()")]
		public bool ReadInput = true;
		/// if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff
		/// これが真の場合、加速度は適用されず、瞬時に全速力となります（ロックマンの動きを想像してください）。
		/// 注意：瞬間的な加速を持つキャラクターは、通常のキャラクターのようにX軸でノックバックを受けることはできません。
		[Tooltip("if this is true, no acceleration will be applied to the movement, which will instantly be full speed (think Megaman movement). Attention : a character with instant acceleration won't be able to get knockbacked on the x axis as a regular character would, it's a tradeoff")]
		public bool InstantAcceleration = false;
		/// the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)
		/// 入力を考慮する閾値（小さなジョイスティックノイズを除去するため通常0.1f）(検知しない値を確認)
		[Tooltip("the threshold after which input is considered (usually 0.1f to eliminate small joystick noise)")]
		public float InputThreshold = 0.1f;
        /// how much air control the player has
        [Range(0f, 1f)]
		[Tooltip("how much air control the player has")]
		public float AirControl = 1f;
		/// whether or not the player can flip in the air
		[Tooltip("whether or not the player can flip in the air")]
		public bool AllowFlipInTheAir = true;
		/// whether or not this ability should keep taking care of horizontal movement after death
		[Tooltip("whether or not this ability should keep taking care of horizontal movement after death")]
		public bool ActiveAfterDeath = false;

        [Header("Touching the Ground")]
		/// the MMFeedbacks to play when the character hits the ground
		/// キャラクターが地面に衝突したときに再生されるMMFeedbacks
		[Tooltip("the MMFeedbacks to play when the character hits the ground")]
		public MMFeedbacks TouchTheGroundFeedback;
		/// the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground
		/// キャラクタが空中にいる間、地面に触れてもフィードバックが再生されるまでの時間（秒）です。
		[Tooltip("the duration (in seconds) during which the character has to be airborne before a feedback can be played when touching the ground")]
		public float MinimumAirTimeBeforeFeedback = 0.2f;

        [Header("Walls")]
		/// Whether or not the state should be reset to Idle when colliding laterally with a wall
		/// 壁に横から衝突したときに状態をIdleに戻すかどうか
		[Tooltip("Whether or not the state should be reset to Idle when colliding laterally with a wall")]
		public bool StopWalkingWhenCollidingWithAWall = false;
				 */
                #endregion
                _characterHorizontalMovement.WalkSpeed = status.patrolSpeed.x;
                if (_characterRun != null)
                {
                    _characterRun.RunSpeed = status.combatSpeed.x;
                }

            }
            else
            {
                GravitySet(status.firstGravity);
                if (_flying != null)
                {
                    //Debug.Log($"あああｓｄ{_flying.nFlySpeed == null}{status.patrolSpeed.x}");
                    _flying.SpeedSet(status.patrolSpeed.x, status.patrolSpeed.y, false);
                    _flying.SpeedSet(status.combatSpeed.x, status.combatSpeed.y, true);
                    //	_flying.FastFly(false, false);
                    //Debug.Log($"hhhhd{_flying.FlySpeed.x}");
                }
                else
                {
                    Debug.Log("あああｓｄ");
                }
            }

            if (_rolling != null)
            {
                #region
                /*
				         /// 何秒転がる
		[Tooltip("ローリング時間")]
        public float RollDuration = 0.5f;
        /// the speed of the roll (a multiplier of the regular walk speed)
        [Tooltip("転がる速さが通常の歩く速さの何倍か")]
        public float RollSpeed = 3f;
        /// if this is true, horizontal input won't be read, and the character won't be able to change direction during a roll
        [Tooltip("trueの場合、水平方向の入力は読み込まれず、ロール中に方向を変えることはできません。")]
        public bool BlockHorizontalInput = false;
        /// if this is true, no damage will be applied during the roll, and the character will be able to go through enemies
        /// //このパラメーターがかかわる処理を見れば無敵処理がわかる
        [Tooltip("trueの場合、ロール中にダメージが与えられず、敵をスルーできるようになります。")]
        public bool PreventDamageCollisionsDuringRoll = false;

        //方向
        [Header("Direction")]

        /// the roll's aim properties
        [Tooltip("the roll's aim properties")]
        public MMAim Aim;
        /// the minimum amount of input required to apply a direction to the roll
        [Tooltip(" ロールに方向を与えるために必要な最小限の入力量")]
        public float MinimumInputThreshold = 0.1f;
        /// if this is true, the character will flip when rolling and facing the roll's opposite direction
        [Tooltip("これが真なら、キャラクターはロール時に反転し、ロールの反対側を向きます。")]
        public bool FlipCharacterIfNeeded = true;

        //これコルーチンなんだ
        public enum SuccessiveRollsResetMethods { Grounded, Time }

        [Header("Cooldown")]
        /// the duration of the cooldown between 2 rolls (in seconds)
        [Tooltip("次のローリングまでに必要な時間")]
        public float RollCooldown = 1f;

        [Header("Uses")]
        /// whether or not rolls can be performed infinitely
        [Tooltip("無限にローリングできるか")]
        public bool LimitedRolls = false;
        /// the amount of successive rolls a character can perform, only if rolls are not infinite
        [Tooltip("the amount of successive rolls a character can perform, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public int SuccessiveRollsAmount = 1;
        /// the amount of rollss left (runtime value only), only if rolls are not infinite
        [Tooltip("ローリングの残り回数")]
        [MMCondition("LimitedRolls", true)]
        [MMReadOnly]
        public int SuccessiveRollsLeft = 1;
        /// when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite
        [Tooltip("when in time reset mode, the duration, in seconds, after which the amount of rolls left gets reset, only if rolls are not infinite")]
        [MMCondition("LimitedRolls", true)]
        public float SuccessiveRollsResetDuration = 2f;

				*/
                #endregion

                _rolling.RollDuration = status.avoidRes;
                _rolling.RollSpeed = status.avoidSpeed;
                _rolling.BlockHorizontalInput = true;
                _rolling.PreventDamageCollisionsDuringRoll = true;
                _rolling.RollCooldown = status.avoidCool;


            }

            if (_jump != null)
            {
                #region
                /*
                  		/// the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)
		[Tooltip("the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)")]
		public int NumberOfJumps = 2;
		/// defines how high the character can jump
		[Tooltip("defines how high the character can jump")]
		public float JumpHeight = 3.025f;
		/// basic rules for jumps : where can the player jump ?
		[Tooltip("basic rules for jumps : where can the player jump ?")]
		public JumpBehavior JumpRestrictions = JumpBehavior.CanJumpAnywhere;
		/// if this is true, camera offset will be reset on jump
		[Tooltip("if this is true, camera offset will be reset on jump")]
		public bool ResetCameraOffsetOnJump = false;
		/// if this is true, this character can jump down one way platforms by doing down + jump
		[Tooltip("if this is true, this character can jump down one way platforms by doing down + jump")]
		public bool CanJumpDownOneWayPlatforms = true;

		[Header("Proportional jumps")]

		/// if true, the jump duration/height will be proportional to the duration of the button's press
		[Tooltip("if true, the jump duration/height will be proportional to the duration of the button's press")]
		public bool JumpIsProportionalToThePressTime = true;
		/// the minimum time in the air allowed when jumping - this is used for pressure controlled jumps
		[Tooltip("the minimum time in the air allowed when jumping - this is used for pressure controlled jumps")]
		public float JumpMinimumAirTime = 0.1f;
		/// the amount by which we'll modify the current speed when the jump button gets released
		[Tooltip("the amount by which we'll modify the current speed when the jump button gets released")]
		public float JumpReleaseForceFactor = 2f;

		[Header("Quality of Life")]

		/// a timeframe during which, after leaving the ground, the character can still trigger a jump
		[Tooltip("a timeframe during which, after leaving the ground, the character can still trigger a jump")]
		public float CoyoteTime = 0f;

		/// if the character lands, and the jump button's been pressed during that InputBufferDuration, a new jump will be triggered 
		[Tooltip("キャラクターが着地し、そのInputBufferDurationの間にジャンプボタンが押された場合、新しいジャンプが開始されます。")]
		public float InputBufferDuration = 0f;

		[Header("Collisions")]

		/// duration (in seconds) we need to disable collisions when jumping down a 1 way platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping down a 1 way platform")]
		public float OneWayPlatformsJumpCollisionOffDuration = 0.3f;
		/// duration (in seconds) we need to disable collisions when jumping off a moving platform
		[Tooltip("duration (in seconds) we need to disable collisions when jumping off a moving platform")]
		public float MovingPlatformsJumpCollisionOffDuration = 0.05f;

		[Header("Air Jump")]

		/// the MMFeedbacks to play when jumping in the air
		[Tooltip("the MMFeedbacks to play when jumping in the air")]
		public MMFeedbacks AirJumpFeedbacks;

		/// the number of jumps left to the character
		[MMReadOnly]
		[Tooltip("the number of jumps left to the character")]
		public int NumberOfJumpsLeft;

		/// whether or not the jump happened this frame
		public bool JumpHappenedThisFrame { get; set; }
		/// whether or not the jump can be stopped
		public bool CanJumpStop { get; set; }
                 
				 */
                #endregion
                _jump.CoyoteTime = status.jumpCool;
                _jump.JumpHeight = status.jumpRes;
                _jump.NumberOfJumps = status.jumpLimit;

            }
            maxHp = status.maxHp;

            //この辺いらないかも
            //個別に持たせた倍率で操作すればよくね？

            //　無属性防御力。体力で上がる
            Def = status.Def;
            //刺突防御。筋力で上がる
            pierDef = status.pierDef;
            //打撃防御、技量で上がる
            strDef = status.strDef;
            //神聖防御、筋と賢さで上がる。
            holyDef = status.holyDef;
            //闇防御。賢さで上がる
            darkDef = status.darkDef;
            //炎防御。賢さと生命で上がる
            fireDef = 70;
            //雷防御。賢さと持久で上がる。
            thunderDef = 70;

            _health.InitialHealth = (int)maxHp;
            _health.MaximumHealth = (int)maxHp;
            _health.CurrentHealth = _health.InitialHealth;
            //	Debug.Log($"tanomu{_health.CurrentHealth}");
            nowArmor = status.Armor;
        }


        /// <summary>
        /// 毎フレーム使用
        /// 自分のデータを送信
        /// </summary>
        /// <param name="num"></param>
        public override void TargetDataUpdate(int num)
        {
            SManager.instance._targetList[num]._condition.hpRatio =_health.CurrentHealth / status.maxHp;

            SManager.instance._targetList[num]._condition.targetPosition = myPosition;
            SManager.instance._targetList[num]._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            SManager.instance._targetList[num]._condition.hpNum = _health.CurrentHealth;
            SManager.instance._targetList[num]._condition.isBuffOn = false;
            SManager.instance._targetList[num]._condition.isDebuffOn = false;
        }

        /// <summary>
        /// これは最初の一回きり
        /// ターゲットデータの追加
        /// </summary>
        /// <param name="data"></param>
        public override void  TargetDataAdd(int newID)
        {

            TargetData imfo = new TargetData();

            imfo._baseData = status._charaData;

            imfo._condition.targetPosition = myPosition;
            imfo._condition.hpRatio = _health.CurrentHealth / status.maxHp;
            imfo._condition.hpNum = _health.CurrentHealth;
            imfo._condition.isBuffOn = false;
            imfo._condition.isDebuffOn = false;

            imfo.targetObj = this.gameObject;

            imfo.targetID = newID;

            //ID割り当て
            myID = newID;

            SManager.instance._targetList.Add(imfo);
        }




        #endregion



        #region　ステータス関連

        public void HPReset()
        {
            _health.SetHealth((int)status.maxHp, this.gameObject);
        }



        /// <summary>
        /// ダメージ計算
        /// これAtV通す必要ある？
        /// </summary>
        /// <param name="isFriend">真なら味方</param>
        public override void DamageCalc()
        {
            //GManager.instance.isDamage = true;
            //status.hitLimmit--;
            //mValueはモーション値


            _damage._attackData._attackType = atV.mainElement;
            _damage._attackData.phyType = atV.phyElement;

            if (status.phyAtk > 0)
            {
                _damage._attackData.phyAtk = status.phyAtk * attackFactor;

                if (atV.shock >= 40)
                {
                    _damage._attackData.isHeavy = true;
                }
                else
                {
                    _damage._attackData.isHeavy = false;
                }

            }
            //神聖
            if (status.holyAtk > 0)
            {
                _damage._attackData.holyAtk = status.holyAtk * holyATFactor;

            }
            //闇
            if (status.darkAtk > 0)
            {
                _damage._attackData.darkAtk = status.darkAtk * darkATFactor;

            }
            //炎
            if (status.fireAtk > 0)
            {
                _damage._attackData.fireAtk = status.fireAtk * fireATFactor;

            }
            //雷
            if (status.thunderAtk > 0)
            {
                _damage._attackData.thunderAtk = status.thunderAtk * thunderATFactor;

            }
            _damage._attackData.shock = atV.shock;
            _damage._attackData.mValue = atV.mValue;
            _damage._attackData.disParry = atV.disParry;
            _damage._attackData.attackBuff = attackBuff;
            //damage = Mathf.Floor(damage * attackBuff);
            _damage._attackData._parryResist = atV.parryResist;
            _damage._attackData.isBlow = atV.isBlow;

            _damage._attackData.isLight = atV.isLight;
            _damage._attackData.blowPower.Set(atV.blowPower.x, atV.blowPower.y);
        }

        /// <summary>
        /// 自分のダメージ中フラグ立ててこちらの防御力を教えてあげるの
        /// </summary>
        public override void DefCalc()
        {


            _health.InitialHealth = (int)maxHp;
            _health._defData.Def = Def;
            _health._defData.pierDef = pierDef;
            _health._defData.strDef = strDef;
            _health._defData.fireDef = fireDef;
            _health._defData.holyDef = holyDef;
            _health._defData.darkDef = darkDef;

            _health._defData.phyCut = status.phyCut;
            _health._defData.fireCut = status.fireCut;
            _health._defData.holyCut = status.holyCut;
            _health._defData.darkCut = status.darkCut;

            _health._defData.guardPower = status.guardPower;


            if (_condition.CurrentState == CharacterStates.CharacterConditions.Stunned)
            {
                _health._defData.isDangerous = true;

            }
            _health._defData.attackNow = _movement.CurrentState == CharacterStates.MovementStates.Attack ? true : false;

        }

        /// <summary>
        /// 現在ガード中かどうかを知らせる
        /// </summary>
        public override bool GuardReport()
        {
            return _movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove;
        }

        /// <summary>
        /// バフの数値を与える
        /// 弾丸から呼ぶ
        /// </summary>
        public override void BuffCalc(FireBullet _fire)
        {
            _fire.attackFactor = attackFactor;
            _fire.fireATFactor = fireATFactor;
            _fire.thunderATFactor = thunderATFactor;
            _fire.darkATFactor = darkATFactor;
            _fire.holyATFactor = holyATFactor;
        }

        public override void ParryStart(bool isBreake)
        {
            _parry.ParryStart(isBreake);
            _guard.GuardEnd();

            //アーマー回復
            nowArmor = nowArmor + (status.Armor / 3) > status.Armor ? status.Armor : nowArmor + (status.Armor / 3);
        }

        /// <summary>
        /// 死ぬときに何かする処理
        /// 自分をターゲットリストから削除したり…は死亡モーション開始でやってるか
        /// </summary>
        public override void Die()
        {


            if (SManager.instance.target == this.gameObject)
            {
                SManager.instance.target = null;

            }
        }


        #endregion



        ///<sammary>
        /// 　攻撃関連の処理
        /// 　攻撃に必要なこと
        /// 　まず攻撃のパラメータを設定。番号を指定し攻撃開始
        /// 　落下攻撃などには引数付きの攻撃終了条件設定アニメイベントを入れる。これでアーマーも有効にする
        /// 　アニメイベントにはほかにエフェクトを出すやつと音を出すやつがある。
        /// 　そして攻撃終了条件を満たしてるか、あるいはアニメ終わったかで攻撃終了確認
        /// 　そして補足行動あるならやるが、補足行動は厳密には攻撃処理と切り離して行う。
        /// 　補足行動フラグは応用すれば挑発とか敵にさせれる
        /// 　攻撃ごとにつけるクールタイムが終了したらまた攻撃するように
        /// 　魔術系の攻撃の詠唱は攻撃判定ない攻撃にしてコンボで魔術発動に
        /// 　攻撃値にisShootとか入れるか
        ///</sammary>
        #region 攻撃関連の処理

        




        /// <summary>
        /// 攻撃前の初期化
        /// </summary>
        public void AttackPrepare()
        {
            //_movement.CurrentState != CharacterStates.MovementStates.Attack = true;
            atV.coolTime = status.atValue[attackNumber].coolTime;
            atV.isBlow = status.atValue[attackNumber].isBlow;
            atV.mValue = status.atValue[attackNumber].mValue;
            atV.aditionalArmor = status.atValue[attackNumber].aditionalArmor;
            atV.isLight = status.atValue[attackNumber].isLight;
            atV.disParry = status.atValue[attackNumber].disParry;
            atV.blowPower = status.atValue[attackNumber].blowPower;
            atV.shock = status.atValue[attackNumber].shock;
            atV.type = status.atValue[attackNumber].type;
            atV.isCombo = status.atValue[attackNumber].isCombo;
            atV.escapePercentage = status.atValue[attackNumber].escapePercentage;
            atV.parryResist = status.atValue[attackNumber].parryResist;
            //	atV.attackEffect = status.atValue[attackNumber].attackEffect;
            atV.suppleNumber = status.atValue[attackNumber].suppleNumber;

            //突進用の初期化
            atV._moveDuration = status.atValue[attackNumber]._moveDuration;
            atV._moveDistance = status.atValue[attackNumber]._moveDistance;
            atV._contactType = status.atValue[attackNumber]._contactType;
            atV.fallAttack = status.atValue[attackNumber].fallAttack;
            atV.startMoveTime = status.atValue[attackNumber].startMoveTime;
            atV.lockAttack = status.atValue[attackNumber].lockAttack;
            atV.backAttack = status.atValue[attackNumber].backAttack;

            atV.mainElement = status.atValue[attackNumber].mainElement;
            atV.phyElement = status.atValue[attackNumber].phyElement;
            atV.motionType = status.atValue[attackNumber].motionType;
            atV.EffectLevel = status.atValue[attackNumber].EffectLevel;
            atV.endCondition = status.atValue[attackNumber].endCondition;

            //ヒット数制御関連
            _damage._attackData._hitLimit = status.atValue[attackNumber]._hitLimit;
            _damage.CollidRestoreResset();

            _health._superArumor = status.atValue[attackNumber].superArmor;
            _health._guardAttack = status.atValue[attackNumber].guardAttack;



            int adType = 0;

            if (atV.disParry)
            {
                adType = 1;
            }

            _atEf.EffectPrepare(status.atValue[attackNumber].EffectLevel, adType, status.atValue[attackNumber].mainElement, status.atValue[attackNumber].motionType);
        }


        /// <summary>
        /// 弾丸を発射する
        /// 詠唱アニメ後にこれでよくね？
        /// </summary>
        /// <param name="i"></param>
        /// <param name="random"></param>
        public void ActionFire(int i, float random = 0.0f)
        {
            //ランダムに入れてもいいけど普通に入れてもいい
            i = i < 0 ? 0 : i;
            i = i > status.enemyFire.Count - 1 ? status.enemyFire.Count : i;


            waitCast += _controller.DeltaTime;
            if (waitCast >= status.enemyFire[i].castTime)
                if (random != 0)
                {
                    firePosition.position.Set
                        (firePosition.position.x + random, firePosition.position.y + random, firePosition.position.z);//銃口から
                }
            Transform goFire = firePosition;

            for (int x = 0; x >= status.enemyFire[i].bulletNumber; x++)
            {
                Addressables.InstantiateAsync(status.enemyFire[i].effects, goFire.position, Quaternion.identity);//.Result;//発生位置をPlayer
            }
            //go.GetComponent<EnemyFireBullet>().ownwer = transform;

        }


        /// <summary>
        /// 攻撃
        /// 改修要請
        /// isShoot = trueの時の処理つくる？。いらない？つまりたまうちってこったな
        /// リヴァースで逆向きで
        /// </summary>
        /// <param name="select">攻撃番号を指定するかどうか</param>
        /// <param name="number">指定する攻撃</param>
        /// <param name="reverse">逆に振り返って攻撃</param>
        public void Attack( int number = 0,bool isUseSkill = true)
        {

            //クールタイム消化してて動けるなら
            if (!isAtEnable || !isMovable || number > status.atValue.Count || number <= 0)
            {//Debug.Log($"一回きり{isAtEnable}{isMovable}{ number > status.atValue.Count}{number <= 0}");
                return;
            }

            if (isUseSkill)
            {
                useSkillNum = number;
            }


            //ロックかける
            isMovable = false;
            isAtEnable = false;

            if (_movement.CurrentState == CharacterStates.MovementStates.Guard || _movement.CurrentState == CharacterStates.MovementStates.GuardMove)
            {
                //guardJudge = false;
                _guard.GuardEnd();
            }



            //敵の方向く
            if (!atV.backAttack)
            {
                NormalFlip(direction);

            }
            else
            {

                NormalFlip(-direction);

            }


            AttackPrepare();

            //アニメーションの用意
            //攻撃処理はアニメーションに任せてるのでここで攻撃開始
            _attack.AttackTrigger(number);



            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                _flying.SetHorizontalMove(0);
                _flying.SetVerticalMove(0);
            }
            else
            {
                _characterHorizontalMovement.SetHorizontalMove(0);
            }

            //停止
            _controller.SetForce(Vector2.zero);

            //距離が移動範囲内で、ロックオンするなら距離を変える
            float moveDistance = (atV.lockAttack && Mathf.Abs(distance.x) < atV._moveDistance) ? distance.x : atV._moveDistance;

            //攻撃の踏み込み移動開始
            _rush.RushStart(atV._moveDuration, moveDistance * direction, atV._contactType, atV.fallAttack, atV.startMoveTime, atV.backAttack);


            //終了条件の監視を始める
            WaitAttack(atV.endCondition).Forget();

            //    ExecuteAttack().Forget();



        }


        //弾丸処理の例
        //ロードして
        #region
        /*
		 		/// <summary>
		/// for文ではないが	bcountを超えるまでuseMagicが真なので発動し続ける
		/// 弾丸を作るメソッド
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		 void MagicUse(int hRandom, int vRandom)
		{

			if (!fireStart || delayNow)
			{
				return;
			}
			bCount += 1;
			Debug.Log("きてる");
			//	Debug.Log($"ハザード{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
			//魔法使用中MagicUseでかつ弾丸生成中でなければ

			//弾の発射というか生成位置
			Vector3 goFire = sb.firePosition.position;
			//弾が一発目なら
			if (bCount == 1)
			{
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//山なりの弾道で打ちたいときとか射出角度決めれたらいいかも
					//位置をランダムにすれば角度はどうでもいい説もある
					SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//敵の位置にサーチ攻撃するとき
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.target.myPosition.x, SManager.instance.target.myPosition.y, SManager.instance.target.myPosition.y);

			}
			//ランダムな位置に発生するとき
			else if (hRandom != 0 || vRandom != 0)
			{
				//Transform goFire = firePosition;


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
				//	xRandom = RandomValue(RandomValue(-random,0),RandomValue(0, random));
				//	yRandom = RandomValue(RandomValue(-random, 0), RandomValue(0, random));

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//銃口から

			}
			//Debug.Log($"魔法の名前5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//発生位置をPlayer
			//即座に発生する弾丸の一発目なら
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Debug.Log("aaa");
                UnityEngine.Object h = Addressables.LoadAssetAsync<UnityEngine.Object>(SManager.instance.useMagic.effects).Result;
				 GameObject t =  Instantiate(h, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)) as GameObject;//.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject, SManager.instance.target);
				t.MMGetComponentNoAlloc<FireBullet>().InitializedBullet(this.gameObject,SManager.instance.target);
			}
			//2発目以降の弾で生成中じゃないなら
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}
			//弾丸を生成し終わったら
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"テンペスト{SManager.instance.useMagic.name}標的{SManager.instance.target}動作{sister.nowMove}");
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				//disEnable = true;
				coolTime = SManager.instance.useMagic.coolTime;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

				actionNum = 0;

				SManager.instance.useMagic = null;
				fireStart = false;
				SManager.instance.target.GetComponent<EnemyAIBase>().TargetEffectCon(3);
			}
				
		}


		 */
        #endregion








        /// <summary>
        /// 攻撃終了の条件を入れつつアーマーを設定
        /// アニメーションイベント
        /// ここからアーマー判定が開始
        /// </summary>
        public void ConditionAttack()
        {
            //このナンバーによって重力かけたりとかやる？

            isArmor = true;
        }

        /// <summary>
        /// 補足行動の起動を行うメソッド
        /// </summary>
        /// <param name="useNumber"></param>
        public void SActTrigger(int useNumber)
        {
            suppleNumber = atV.suppleNumber;
            _condition.ChangeState(CharacterStates.CharacterConditions.Moving);
        }

        /// <summary>
        /// どのモーションをやるかどうか
        /// </summary>
        /// <param name="number"></param>
        public void SetAttackNumber(int number)
        {
            number = number > status.serectableNumber.Count - 1 ? status.serectableNumber.Count - 1 : number;
            number = number < 0 ? 0 : number;
            attackNumber = status.serectableNumber[number];
        }

        /// <summary>
        /// クールタイムのや攻撃終了条件を待つ間次の攻撃を待つ
        /// </summary>
        public async UniTaskVoid WaitAttack(EnemyValue.AttackEndCondition endCon)
        {
            int result = 0;

            if (endCon == EnemyValue.AttackEndCondition.モーション終了)
            {

                string name = $"Attack{attackNumber}";
                
                // モーション終了かスタンで強制解除を待機
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                 UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常にアニメーションが終了して終わったなら続ける
                if (result != 0)
                {
                    return;
                }
            }

            else if (endCon == EnemyValue.AttackEndCondition.着地か時間経過)
            {
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => _controller.State.IsGrounded, cancellationToken: destroyCancellationToken),
                    UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                    UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常に条件を満たしたなら先へ
                if (result == 2)
                {
                    return;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.移動か時間経過)
            {
                float stPosi = myPosition.x;
                float moveDis = atV._moveDistance;

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => Math.Abs(myPosition.x - stPosi) >= moveDis, cancellationToken: destroyCancellationToken),
                UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常に条件を満たしたなら先へ
                if (result == 2)
                {
                    return;
                }
            }

            else if (endCon == EnemyValue.AttackEndCondition.ヒットかモーション終了)
            {
                string name = $"Attack{attackNumber}";

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => isHit, cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常に条件を満たしたなら先へ
                if (result == 2)
                {
                    return;
                }
                //ヒットしてないならコンボ中断
                else if (result == 1)
                {
                    atV.isCombo = false;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.ヒットか時間経過)
            {

                result = await UniTask.WhenAny(UniTask.WaitUntil(() => isHit, cancellationToken: destroyCancellationToken),
                UniTask.Delay(TimeSpan.FromSeconds(4f), cancellationToken: destroyCancellationToken),
                UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常に条件を満たしたなら先へ
                if (result == 2)
                {
                    return;
                }
                //ヒットしてないならコンボ中断
                else if (result == 1)
                {
                    atV.isCombo = false;
                }
            }
            else if (endCon == EnemyValue.AttackEndCondition.補足行動の終了)
            {
                string name = $"Attack{attackNumber}";
                // モーション終了かスタンで強制解除を待機
                result = await UniTask.WhenAny(UniTask.WaitUntil(() => CheckEnd(name), cancellationToken: destroyCancellationToken),
                    UniTask.WaitUntil(() => _movement.CurrentState != CharacterStates.MovementStates.Attack, cancellationToken: destroyCancellationToken));

                //正常にアニメーションが終了して終わったなら続ける
                if (result != 0)
                {
                    return;
                }

                //ここから補足アニメ再生
                //補足行動には
                //後隙アニメ、ジャスト（特殊）ガード、回避、テレポートとか？
                //テレポートとかは攻撃後イベントでもいいわ
                //それかテレポート挟んでコンボ継続とかあるならこれじゃないとダメか
                SActTrigger(atV.suppleNumber);


            }

            //      if (!CheckEnd($"SuppleAction{suppleNumber}"))


            _attack.AttackEnd();
            NormalFlip(direction);
            isHit = false;

            _movement.ChangeState(CharacterStates.MovementStates.Idle);

            if (atV.isCombo)
            {
                //マイナス以下のアーマーなら強化してやる
                atV.aditionalArmor = atV.aditionalArmor < 0 ? 30 : atV.aditionalArmor;

                isAtEnable = true;
                isMovable = true;
                attackNumber++;
                Attack(attackNumber,false);
            }
            //コンボ無し終了
            else
            {
                atV.aditionalArmor = 0;
                isArmor = false;

                attackNumber = 0;

                //クールタイムが必要なら
                if (atV.coolTime > 0)
                {
                    //攻撃終了後はクールタイム消化
                    WaitAction(atV.coolTime, (() => isAtEnable = true)).Forget();

                }
                //いらないなら攻撃可能に
                else
                {
                    isAtEnable = true;
                }

                //攻撃判断を再開
                AttackBehaivior().Forget();

                //攻撃終了時の処理
                AttackEvent();

            }
        }

        /// <summary>
        /// アニメ中断
        /// </summary>
        public void AttackEnd(bool stun = false)
        {
            if (!stun)
            {
                _movement.ChangeState(CharacterStates.MovementStates.Idle);
                _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            }

            atV.aditionalArmor = 0;
            isAtEnable = true;

            attackNumber = 0;

            _attack.AttackEnd();
            GravitySet(status.firstGravity);
            isArmor = false;

            //攻撃判断を再開
            AttackBehaivior().Forget();
        }

        /// <summary>
        /// パリィ不可の行動
        /// エフェクト出す時間も指定できるようにしたいね
        /// LifTimeいじるか
        /// </summary>
        public void DisParriableAct()
        {
            GManager.instance.PlaySound("DisenableParry", transform.position);
            Vector3 Scale = Vector3.zero;
            if (transform.localScale.x > 0)
            {
                Scale.Set(status.disparriableScale.x, status.disparriableScale.y, status.disparriableScale.z);
            }
            else
            {
                Scale.Set(status.disparriableScale.x * -1f, status.disparriableScale.y, status.disparriableScale.z);
            }

            dispary.transform.localScale = Scale;
            dispary.transform.rotation = Quaternion.Euler(-10, 0, 0);
            //	Addressables.InstantiateAsync("DisParriableEffect", dispary.transform);
        }

        //ヘルスのために攻撃状態か否かを返す
        public bool AttackCheck()
        {
            return _movement.CurrentState == CharacterStates.MovementStates.Attack;
        }

        #endregion









        #region 音関連




        public override void GuardSound()
        {
            MyCode.SoundManager.instance.GuardSound(status.isMetal, status.shieldType, transform.position);
        }





        #endregion



        #region 移動関連処理

        public void NormalFlip(float direction)
        {
            if (direction == 0 || (_condition.CurrentState == CharacterStates.CharacterConditions.Dead || _condition.CurrentState == CharacterStates.CharacterConditions.Stunned))
            {
                return;
            }

            //今の向きと向きたい向きが違うなら
            if (direction != MathF.Sign(transform.localScale.x))
            {

                _character.Flip();
            }

        }

        /// <summary>
        /// アニメの都合上反対側向きたいときのメソッド
        /// </summary>
        public void AnimeFlip()
        {
            //_characterFly.SetHorizontalMove(1f);
            //このへん使って振り向き行おう

            NormalFlip(Mathf.Sign(-transform.localScale.x));
        }


        #region 警戒状態で呼ぶメソッド

        /// <summary>
        /// 最初の位置に戻る。
        /// </summary>
        public void PositionReset()
        {
            //動けて、なおかつポジションリセット中なら
            if (isMovable && posiReset)
            {


                if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
                {
                    int returneDireX = 0;

                    int returneDireY = 0;
                    //戻る方向を割り出す
                    if (Math.Abs(myPosition.x - basePosition.x) >= 3)
                    {
                        returneDireX = myPosition.x <= basePosition.x ? 1 : -1;
                    }

                    if (Math.Abs(myPosition.y - basePosition.y) >= 3)
                    {
                        returneDireY = myPosition.y <= basePosition.y ? 1 : -1;
                    }


                    //戻る場所との距離を出す
                    //二乗で
                    float returnDis = Vector2.SqrMagnitude(myPosition - basePosition);

                    if (returnDis <= 25)
                    {
                        _flying.SetHorizontalMove(0);
                        _flying.SetVerticalMove(0);
                        posiReset = false;

                    }
                    else
                    {
                        NormalFlip(returneDireX);
                        _flying.SetHorizontalMove(returneDireX);
                        _flying.SetVerticalMove(returneDireY);

                    }


                    //DirectionとDistanceでやれない？

                }
                else
                {
                    //戻る方向を割り出す
                    int returneDire = myPosition.x <= basePosition.x ? 1 : -1;

                    //戻る場所との距離を出す
                    float returnDis = Math.Abs(myPosition.x - basePosition.x);

                    if (returnDis <= 5)
                    {
                        _characterHorizontalMovement.SetHorizontalMove(0);
                        posiReset = false;
                    }
                    else
                    {
                        NormalFlip(returneDire);
                        _characterHorizontalMovement.SetHorizontalMove(returneDire);
                    }
                }



            }
        }


        /// <summary>
        /// 待機中の哨戒行動
        /// 非同期で最初に一度だけ呼び出す
        /// ダメージ食らったら中断してダメージ処理終了後にもう一度呼び出し
        /// パトロールトークンが必要
        /// </summary>
        public async UniTaskVoid PatrolMove()
        {
            if (posiReset)
            {
                //元の位置に戻るまでは待つ
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //これは左向きで配置されてる時のために必要
            int myDire = Math.Sign(transform.localScale.x);



            //右向いてて目的地より左なら
            if (myDire > 0 && myPosition.x < startPosition.x + status.waitDistance.x)
            {           

                _characterHorizontalMovement.SetHorizontalMove(1);

                //見回り範囲より先に行くのを待つ
                await UniTask.WaitUntil(() => (myPosition.x >= startPosition.x + status.waitDistance.x), cancellationToken: patrolToken.Token);
                
                _characterHorizontalMovement.SetHorizontalMove(0);
                //秒数待つ
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

             NormalFlip(-1);
                PatrolMove().Forget();

            }
            else if(myPosition.x > startPosition.x - status.waitDistance.x)
            {
               
                _characterHorizontalMovement.SetHorizontalMove(-1);

                //見回り範囲より先に行くのを待つ
                await UniTask.WaitUntil(() => (myPosition.x <= startPosition.x - status.waitDistance.x), cancellationToken: patrolToken.Token);


                _characterHorizontalMovement.SetHorizontalMove(0);
                //秒数待つ
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

                NormalFlip(1);
                PatrolMove().Forget();
            }

        }

        /// <summary>
        /// 待機中空を飛ぶ
        /// patrolMoveは横移動。これは縦移動。いや縦横どちらもだわ
        /// 待機と選択で積む
        /// 最初だけ呼ぶ
        /// </summary>
        public async UniTaskVoid PatrolFly()
        {
            if (posiReset)
            {
                //元の位置に戻るまでは待つ
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //これは左向きで配置されてる時のために必要
            int faceDire = Math.Sign(transform.localScale.x);


            int pDireX = 0;
            int pDireY = 0;

            if (status.waitDistance.x != 0)
            {
                //右向いてて見回り範囲の右端以内なら向きは1
                pDireX =  faceDire > 0 && myPosition.x < startPosition.x + status.waitDistance.x  ? 1 : -1;
            }

            if (status.waitDistance.y != 0)
            {
                pDireY = myPosition.y < startPosition.y + status.waitDistance.y ? 1 : -1;
            }

            NormalFlip(pDireX);

            _flying.SetHorizontalMove(pDireX);
            _flying.SetVerticalMove(pDireY);


            //Xにだけ動くとき
            if (pDireY == 0 && pDireX != 0)
            {

                //見回り範囲より先に行くのを待つ
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x), cancellationToken: patrolToken.Token);

                //止まれ
                _flying.SetHorizontalMove(0);

                //条件満たしたら秒数待つ
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

            }
            //yにだけ動くとき
            else if (pDireX == 0)
            {
                //見回り範囲より先に行くのを待つ
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.y - startPosition.y) >= status.waitDistance.y), cancellationToken: patrolToken.Token);

                //止まれ
                _flying.SetVerticalMove(0);

                //条件満たしたら秒数待つ
                await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);


                //逆向く
                NormalFlip(-faceDire);
            }
            //両方に動く
            else
            {
                //見回り範囲より先に行くのを待つ
                await UniTask.WaitUntil(() => (Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x) || (Math.Abs(myPosition.y - startPosition.y) >= status.waitDistance.y), cancellationToken: patrolToken.Token);

                if ((Math.Abs(myPosition.x - startPosition.x) >= status.waitDistance.x))
                {
                    //止まれ
                    _flying.SetHorizontalMove(0);
                    _flying.SetVerticalMove(0);

                    //xの条件満たしたら秒数待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

                    //逆向く
                    NormalFlip(-faceDire);
                }

                //yの条件を満たしたなら方向だけ変える

            }


            //もう一度呼ぶ
            PatrolMove().Forget();


        }


        /// <summary>
        /// 待機中停止してきょろきょろ振り向きだけ。こいつはパトロールしないからwaitTimeは使いまわしでいい
        /// パトロールと選択でどっちか積む
        /// 最初だけ呼ぶ
        /// </summary>
        public async UniTaskVoid Wait()
        {
            if (posiReset)
            {
                //元の位置に戻るまでは待つ
                await UniTask.WaitUntil(() => !posiReset, cancellationToken: patrolToken.Token);
            }

            //秒数待つ
            await UniTask.Delay(TimeSpan.FromSeconds(status.waitRes), cancellationToken: patrolToken.Token);

            NormalFlip(-transform.localScale.x);

            Wait().Forget();
        }

        #endregion


        /// <summary>
        /// プレイヤーが逃げたか確認
		/// 一定時間逃げ続けるか担当範囲から離れたら追うのやめる
		/// これは普通にやるか
        /// 逃げられた後他にターゲットいなければ戦闘終了？
        /// </summary>
        public void EscapeCheck()
        {

            if (Mathf.Abs(distance.x) >= status.escapeDistance.x || Mathf.Abs(distance.y) >= status.escapeDistance.y)
            {
                //相手が逃走開始したなら時間記録
                if (escapeTime == 0)
                {
                    escapeTime = GManager.instance.nowTime;
                }

                if (GManager.instance.nowTime - escapeTime >= status.chaseRes)
                {
                    CombatEnd();
                    escapeTime = 0.0f;
                }
            }
            else
            {
                escapeTime = 0.0f;
            }
        }


        //落下死確認。改造してまた使う
        void fallCheck()
        {
            if (myPosition.y < -30.0f)
            {

            }
        }

        /// <summary>
        /// この敵が次逃げるかどうかを決める
        /// </summary>
        protected void EscapeJudge(int probably = 100)
        {
            if (RandomValue(0, 100) <= probably)
            {
                isEscape = true;

                //ガード使用中ならやめる
                if (guardJudge)
                {
                    guardJudge = false;
                    guardProballity = 0;
                    useGuard = false;
                    _guard.GuardEnd();
                }
            }

        }


        //基本的に戦闘系の移動はCombatStart（）みたいなのにMove系を入れる
        //そして被弾時や攻撃後にJudge系から再開
        //戦闘移動を利用しない系統の敵はstatusの移動速度から推測する？
        //それかboolで持っといてもいいけどね


        #region 地上の敵の戦闘移動関連


        /// <summary>
        /// 戦闘移動を開始するメソッド
		/// 最初に呼び出す
        /// </summary>
        protected void CombatMoveStart()
        {
            AgrMove(true).Forget();
            BattleFlip().Forget();
        }

        /// <summary>
        /// 戦闘中の距離の取り方
        /// nowModeで距離の取り方のパターンを選択できるね
        ///　戦闘開始時に最初だけ呼ぶ
        ///　被弾などで停止したらトークンで再判断させてあげる
        ///　アーマー処理にトークン処理差し込むか
        ///　被弾などで処理停止→AgrJudge（）を呼んで
        ///　そこからAgrMoveもまた出てくる
        /// </summary>
        protected async UniTaskVoid AgrMove(bool wakeUp = false)
        {

            if (!isMovable)
            {
                //スタンしたならすぐ振り向けるように
                wakeUp = _condition.CurrentState == CharacterStates.CharacterConditions.Stunned;
                //動けるようになるまで待つ
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }
            //保留
            GuardJudge();

            if (!wakeUp)
            {

                //時間経過したら判定
                await UniTask.WaitForSeconds(status.judgePace, cancellationToken: agrToken.Token);
            }
            AgrJudge();

            //またAgrMoveを呼び出す
            AgrMove().Forget();

        }


        /// <summary>
        /// 純粋に距離から移動方向などを算出するだけでいい
        /// isEscapeは攻撃や被弾時に再呼び出しするなら自分で値を入れて操れる
        /// battleFlipは速度とかそういうのも操作するように
        /// </summary>
        public void AgrJudge()
        {
            //ダッシュ可能なキャラかどうか
            bool isDashable = status.combatSpeed.x > 0;

            //判断に使う距離
            float useDis = Mathf.Abs(distance.x);



            if (!isEscape)
            {
                //ガード使用状態じゃないなら
                if (!useGuard)
                {
                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40))
                    {

                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //交戦距離より離れてる
                    else if (useDis > status.agrDistance[nowMode].x)//近づく方じゃね？
                    {
                        //歩く距離より近いか走れないなら歩く
                        if (useDis <= status.walkDistance.x || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.accessWalk;
                        }
                        //歩く距離より遠いなら走る
                        else
                        {
                            ground = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //交戦距離より近いなら
                    else if (useDis < status.agrDistance[nowMode].x)//遠ざかる
                    {
                        //歩き距離なら敵を見たまま撃つ
                        //動かない弓兵とかは移動速度ゼロに
                        if (Mathf.Abs((useDis - status.agrDistance[nowMode].x)) <= status.walkDistance.x / 2 || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            ground = EnemyStatus.MoveState.leaveDash;

                        }
                    }
                }
                else
                {
                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
                    {
                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //交戦距離より離れてる
                    else if (useDis > status.agrDistance[nowMode].x)//近づく方じゃね？
                    {

                        ground = EnemyStatus.MoveState.accessWalk;


                    }
                    //交戦距離より近いなら
                    else if (useDis < status.agrDistance[nowMode].x)//遠ざかる
                    {

                        ground = EnemyStatus.MoveState.leaveWalk;

                    }
                }
            }
            else
            {

                if (status.agrDistance[nowMode].x <= 30 || !isDashable)
                {
                    ground = EnemyStatus.MoveState.leaveWalk;
                }
                else if (Mathf.Abs((useDis / status.agrDistance[nowMode].x)) >= 0.6)
                {
                    //逃げる時はガード終わらせる
                    ground = EnemyStatus.MoveState.leaveDash;

                }

                isEscape = false;
            }
            AgrMoveSet();



        }


        /// <summary>
        /// 現在の移動モードに合わせて方向や
        /// 移動速度をセットする
        /// BattleFripとAgrJudgeから呼ばれる
        /// </summary>
        void AgrMoveSet()
        {


            if (ground == EnemyStatus.MoveState.stay)
            {
                //バトルフリップはステイ中だけにする

                _characterHorizontalMovement.SetHorizontalMove(0f);
                NormalFlip(direction);

            }


            else if (ground == EnemyStatus.MoveState.leaveDash || ground == EnemyStatus.MoveState.escape)
            {

                _characterRun.RunStart();
                _characterHorizontalMovement.SetHorizontalMove(-direction);
                NormalFlip(-direction);
            }
            else if (ground == EnemyStatus.MoveState.accessDash)

            {
                _characterRun.RunStart();
                _characterHorizontalMovement.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.accessWalk)

            {
                _characterRun.RunStop();
                _characterHorizontalMovement.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.leaveWalk)

            {
                _characterRun.RunStop();
                _characterHorizontalMovement.SetHorizontalMove(-direction);
                NormalFlip(direction);
            }



        }



        /// <summary>
        /// 戦闘中の挙動を司るメソッド
        /// </summary>
        async UniTaskVoid BattleFlip()
        {
            if (!isMovable)
            {

                //動けるようになるまで待つ
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }

            //0.5秒ごとに
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: agrToken.Token);
            AgrMoveSet();

            //距離に到達したかどうかを調べる
            isReach = Math.Abs(distance.x) <= status.agrDistance[nowMode].x;

            //再帰
            BattleFlip().Forget();


        }

        #endregion


        #region 空中の敵の戦闘で使う


        /// <summary>
        /// 戦闘飛行を開始するメソッド
        /// </summary>
        protected void CombatFlyStart()
        {
            AgrFly(true).Forget();
            BattleFlyFlip().Forget();
        }

        /// <summary>
        /// 戦闘中の距離の取り方
        /// nowModeで距離の取り方のパターンを選択できるね
        ///　戦闘開始時に最初だけ呼ぶ
        ///　被弾などで停止したらトークンで再判断させてあげる
        ///　アーマー処理にトークン処理差し込むか
        ///　被弾などで処理停止→AgrJudge（）を呼んで
        ///　そこからAgrMoveもまた出てくる
        /// </summary>
        protected async UniTaskVoid AgrFly(bool wakeUp = false)
        {

            if (!isMovable)
            {
                //スタンしたならすぐ振り向けるように
                wakeUp = _condition.CurrentState == CharacterStates.CharacterConditions.Stunned;
                //動けるようになるまで待つ
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }
            AirGuardJudge();
            //	Debug.Log($"知りたい{ground}");


            if (!wakeUp)
            {
                //時間経過したら判定
                await UniTask.WaitForSeconds(status.judgePace, cancellationToken: agrToken.Token);
            }
            AgrFlyJudge();

            //またAgrFlyを呼び出す
            AgrFly().Forget();
        }



        /// <summary>
        /// 純粋に距離から移動方向などを算出するだけでいい
        /// isEscapeは攻撃や被弾時に再呼び出しするなら自分で値を入れて操れる
        /// battleFlipは速度とかそういうのも操作するように
        /// </summary>
        public void AgrFlyJudge()
        {


            //Escapeは後でまとめて
            if (!isEscape)
            {
                //判断に使う距離
                float useDis = Mathf.Abs(distance.x);

                //ガード使用状態じゃないなら
                if (!useGuard)
                {
                    //ダッシュ可能なキャラかどうか
                    bool isDashable = status.combatSpeed.x > 0;

                    #region 横判定

                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40))
                    {

                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //交戦距離より離れてる
                    else if (useDis > status.agrDistance[nowMode].x)//近づく方じゃね？
                    {
                        //歩く距離より近いか走れないなら歩く
                        if (useDis <= status.walkDistance.x || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.accessWalk;
                        }
                        //歩く距離より遠いなら走る
                        else
                        {
                            ground = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //交戦距離より近いなら
                    else if (useDis < status.agrDistance[nowMode].x)//遠ざかる
                    {
                        //歩き距離なら敵を見たまま撃つ
                        //動かない弓兵とかは移動速度ゼロに
                        if (Mathf.Abs((useDis - status.agrDistance[nowMode].x)) <= status.walkDistance.x / 2 || !isDashable)
                        {
                            ground = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            ground = EnemyStatus.MoveState.leaveDash;

                        }
                    }
                    #endregion

                    #region 縦判定

                    //距離に攻撃距離を足すことでプレイヤーから上の位置への距離になる
                    useDis = distance.y + status.agrDistance[nowMode].y;

                    isDashable = status.combatSpeed.y > 0;

                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (useDis <= status.adjust)
                    {
                        air = EnemyStatus.MoveState.stay;

                    }

                    //上にいる時下に行く
                    else if (useDis < 0)
                    {
                        //歩く距離より近いか走れないなら歩く
                        if (Mathf.Abs(useDis) <= status.walkDistance.y || !isDashable)
                        {
                            air = EnemyStatus.MoveState.accessWalk;
                        }
                        //歩く距離より遠いなら走る
                        else
                        {
                            air = EnemyStatus.MoveState.accessDash;
                        }
                    }
                    //下にいる時上に行く 
                    else
                    {
                        //歩き距離なら敵を見たまま撃つ
                        //動かない弓兵とかは移動速度ゼロに
                        if (useDis <= status.walkDistance.y || !isDashable)
                        {
                            air = EnemyStatus.MoveState.leaveWalk;
                        }
                        else
                        {
                            air = EnemyStatus.MoveState.leaveDash;

                        }
                    }

                    #endregion
                }
                else
                {

                    #region 横判定
                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (((useDis < status.agrDistance[nowMode].x + status.adjust) && RandomValue(0, 100) >= 40) || guardHit)
                    {
                        ground = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }

                    //交戦距離より離れてる
                    else if (useDis > status.agrDistance[nowMode].x)//近づく方じゃね？
                    {

                        ground = EnemyStatus.MoveState.accessWalk;


                    }
                    //交戦距離より近いなら
                    else if (useDis < status.agrDistance[nowMode].x)//遠ざかる
                    {

                        ground = EnemyStatus.MoveState.leaveWalk;

                    }
                    #endregion

                    #region 縦判定
                    //距離に攻撃距離を足すことでプレイヤーから上の位置への距離になる
                    useDis = distance.y + status.agrDistance[nowMode].y;

                    //交戦距離以内で一定の確率で止まる？
                    //停止確率みたいなの持たせるか
                    if (useDis <= status.adjust || guardHit)
                    {
                        air = EnemyStatus.MoveState.stay;
                        //	flipWaitTime = 10;
                    }
                    //上にいる時下に行く
                    else if (useDis < 0)
                    {

                        air = EnemyStatus.MoveState.accessWalk;
                    }

                    else
                    {

                        air = EnemyStatus.MoveState.leaveWalk;

                    }

                    #endregion

                }
            }
            //逃げる
            else
            {
                //これは逃げ方とかに変えた方がよくない？
                if (atV.escapePercentage > 0)
                {
                    if (atV.escapePercentage == 1)
                    {
                        ground = EnemyStatus.MoveState.leaveDash;
                        air = EnemyStatus.MoveState.leaveDash;
                    }
                    else if (atV.escapePercentage == 2)
                    {
                        ground = EnemyStatus.MoveState.leaveWalk;
                        air = EnemyStatus.MoveState.leaveWalk;
                    }
                    else if (atV.escapePercentage == 3)
                    {
                        ground = EnemyStatus.MoveState.leaveDash;
                        air = EnemyStatus.MoveState.stay;
                    }
                    else if (atV.escapePercentage == 4)
                    {
                        ground = EnemyStatus.MoveState.leaveWalk;
                        air = EnemyStatus.MoveState.stay;
                    }

                }
                isEscape = false;
            }


            AgrFlySet();






        }



        /// <summary>
        /// 現在の移動モードに合わせて方向や
        /// 移動速度をセットする
        /// BattleFripとAgrJudgeから呼ばれる
        /// </summary>
        void AgrFlySet()
        {


            if (ground == EnemyStatus.MoveState.stay)
            {
                //バトルフリップはステイ中だけにする
                _flying.FastFly(false, true);
                _characterHorizontalMovement.SetHorizontalMove(0f);
                NormalFlip(direction);

            }


            else if (ground == EnemyStatus.MoveState.leaveDash)
            {

                _flying.FastFly();
                _flying.SetHorizontalMove(-direction);
                NormalFlip(-direction);
            }
            else if (ground == EnemyStatus.MoveState.accessDash)

            {
                _flying.FastFly();
                _flying.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.accessWalk)

            {
                _flying.FastFly(false, true);
                _flying.SetHorizontalMove(direction);
                NormalFlip(direction);
            }
            else if (ground == EnemyStatus.MoveState.leaveWalk)

            {
                _flying.FastFly(false, true);
                _flying.SetHorizontalMove(-direction);
                NormalFlip(direction);
            }



            if (air == EnemyStatus.MoveState.stay)
            {
                //バトルフリップはステイ中だけにする
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(0f);

            }


            else if (air == EnemyStatus.MoveState.leaveDash)
            {

                _flying.FastFly(true);
                _flying.SetVerticalMove(1);
            }
            else if (air == EnemyStatus.MoveState.accessDash)

            {
                _flying.FastFly(true);
                _flying.SetVerticalMove(-1);
            }
            else if (air == EnemyStatus.MoveState.accessWalk)

            {
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(-1);
            }
            else if (air == EnemyStatus.MoveState.leaveWalk)

            {
                _flying.FastFly(true, true);
                _flying.SetVerticalMove(1);
            }


        }



        /// <summary>
        /// 戦闘中の挙動を司るメソッド
        /// </summary>
        async UniTaskVoid BattleFlyFlip()
        {
            if (!isMovable)
            {

                //動けるようになるまで待つ
                await UniTask.WaitUntil(() => isMovable, cancellationToken: agrToken.Token);
            }

            //0.5秒ごとに
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: agrToken.Token);
            AgrFlySet();

            //戦闘距離に到達しているか
            isReach = Math.Abs(distance.x) <= status.agrDistance[nowMode].x && Math.Abs(distance.y) <= status.agrDistance[nowMode].y;




            //再帰
            BattleFlyFlip().Forget();

        }

        #endregion

        /// <summary>
        /// 回避。方向指定も可能だが戦闘時に限りdirectionで前方。-directionで後ろに
        /// </summary>
        /// <param name="direction"></param>
        public void Avoid(float direction)
        {
            if (!isMovable)
            {
                _rolling.StartRoll();
            }

        }





        /// <summary>
        ///	地上キャラ用のジャンプ。ジャンプ状態は自動解除。ジャンプキャンセルとセット
        ///	バックジャンプが真なら後ろに振り向いて飛んでいく
        /// </summary>	
        public void JumpAct(bool verticalJump = false, bool backJump = false)
        {

            if (isMovable)
            {
                if (!verticalJump)
                {
                    isVertical = verticalJump;
                    if (backJump)
                    {
                        float dire = -1;
                        if (transform.localScale.x > 0)
                        {
                            dire = 1;
                        }
                        NormalFlip(dire);
                    }
                }
                if (status._charaData._kind != EnemyStatus.KindofEnemy.Fly)
                {
                    if (_controller.State.IsGrounded)
                    {
                        _jump.JumpStart();
                    }
                }
                else
                {
                    _jump.JumpStart();
                }


            }
        }

        /// <summary>
        /// トリガーで呼ぶように
        /// </summary>
        /// <param name="isVertical"></param>
        public void JumpController()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Jumping || _movement.CurrentState == CharacterStates.MovementStates.DoubleJumping)
            {
                if (!isVertical)
                {
                    _characterHorizontalMovement.SetHorizontalMove(Mathf.Sign(transform.localScale.x));
                }

            }

            //ダブルジャンプするかどうかは任意で決めます
            /*else if (!_controller.State.IsGrounded)
			{
				if (_jump.JumpEnableJudge() == true)
				{
					Debug.Log($"fffffffffff");
					_jump.JumpStart();
				}
			}*/
        }





        #endregion
        #region　死亡時のマテリアル関連

        protected void MaterialControll()
        {
            if (materialSet > 0)
            {


                //親マテリアルの情報を見る
                //	Debug.Log($"{parentMatt.material}");
                if (materialSet == 1)
                {

                    GetAllChildren(spriteList[mattControllNum]);
                    //	await UniTask.WaitForFixedUpdate();

                    materialSet++;
                }
                if (materialSet > 1)
                {
                    //Debug.Log($"Hello{controllTarget[2].material.name}");
                    materialConTime += _controller.DeltaTime;
                    float test = Mathf.Lerp(0f, 1, materialConTime / 2);
                    for (int i = 0; i <= controllTarget.Count - 1; i++)
                    {


                        controllTarget[i].material.SetFloat("_FadeAmount", test);

                    }


                }

            }

        }
        /// <summary>
        /// 死ぬときのマテリアル操作始動用メソッド。リストのゼロはよく使うやつにするのが無難
        /// 二秒で消える
        /// </summary>
        /// <param name="controllNumber"></param>
        protected void MattControllStart(int controllNumber = 0)
        {
            materialSet = 1;
            MyCode.SoundManager.instance.DeathEffect(effectController.transform.position);
            if (controllNumber != mattControllNum)
            {
                materialConTime = 0;
                controllTarget = null;
                controllTarget = new List<Renderer>();
                mattControllNum = controllNumber;
                mattTrans = null;
            }
        }



        /// <summary>
        /// マテリアルを取得するやつ
        /// Weaponは武器を探すフラグ
        /// スプライトリストの外から武器を持ってくる
        /// </summary>
        /// <param name="parent">マテリアルを収集するオブジェクトの親</param>
        /// <param name="transforms">使用するリスト</param>
        /// <param name="Weapon">一番上のオブジェクトであるかどうか。選択したスプライトリストじゃないとこに武器のマテリアルがある</param>
        private void GetAllChildren(Transform parent, bool Weapon = false)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                //まず子オブジェクトをトランスフォームのリストに追加
                //子オブジェクトの子オブジェクトまで探索

                Transform child = parent.GetChild(i);

                GetAllChildren(child, true);
                //レンダラーを取り出す（あとでこいつを操作する）
                Renderer sr = child.gameObject.MMGetComponentNoAlloc<Renderer>();
                if (sr != null)
                {
                    //リストに追加
                    //Debug.Log(sr.name);
                    controllTarget.Add(sr);
                }
            }

            //一番上のオブジェクトの時武器と盾を探す
            if (!Weapon)
            {
                Transform die = transform.MMFindDeepChildBreadthFirst("Attack");
                if (die != null)
                {
                    GetAllChildren(die, true);
                }
                die = transform.Find("Guard");
                if (die != null)
                {
                    GetAllChildren(die, true);
                }

            }

        }


        #endregion


        #region ガード関連

        /// <summary>
        /// 引数はガードする確率
        /// ガード中移動してたら
        /// ガードフラグ立ってる間はジャッジのたびにガード確率チェックしてガード歩きするかを決める？
        /// </summary>
        protected void GuardFlagOn(int Proballity = 100)
        {

            //後ろ歩きガードどうする？
            guardProballity = Proballity;
            guardJudge = true;

        }

        protected void GuardJudge()
        {
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && guardJudge && _controller.State.IsGrounded)
            {
                useGuard = false;
                if (RandomValue(0, 100) <= guardProballity)
                {
                    //走るのやめてガード
                    if (_movement.CurrentState == CharacterStates.MovementStates.Running)
                    {

                        _characterRun.RunStop();
                    }
                    _guard.ActGuard();
                    useGuard = true;
                }



            }
        }

        protected void AirGuardJudge()
        {
            if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal && guardJudge)
            {
                useGuard = false;
                if (RandomValue(0, 100) <= guardProballity)
                {
                    //走るのやめてガード
                    if (_movement.CurrentState == CharacterStates.MovementStates.FastFlying)
                    {
                        _flying.FastFly(false, true);
                        _flying.FastFly(true, true);
                    }
                    _guard.ActGuard();
                    useGuard = true;
                }



            }
        }


        /// <summary>
        /// 引数はガードする確率
        /// ガード中移動してたら
        /// ガードフラグ立ってる間はジャッジのたびにガード確率チェックしてガード歩きするかを決める？
        /// </summary>
        protected void GuardFlagOff()
        {

            //後ろ歩きガードどうする？
            guardProballity = 0;
            guardJudge = false;

        }

        #endregion

        #region シスターさんの索敵関連のメソッド

        ///<summary>
        /// ロックカーソルの色を変える
        /// 0で始動（黄色）、1で消す、2で標的表示（赤）それ以外で黄色に戻す
        /// </summary>
        public void TargetEffectCon(int state = 0)
        {
            if (state == 0)
            {
                //赤を黄色に戻さないようにすでにカーソルついてるなら戻りなさい
                if (td.gameObject.activeSelf)
                {
                    return;
                }
                td.gameObject.SetActive(true);
                td.color = EnemyManager.instance.stateClor[0];
            }
            else if (state == 1)
            {
                td.gameObject.SetActive(false);
            }
            else if (state == 2)
            {
                td.gameObject.SetActive(true);
                td.color = EnemyManager.instance.stateClor[1];
            }
            else
            {
                // td.enabled = true;
                td.color = EnemyManager.instance.stateClor[0];
            }
        }

        /// <summary>
        /// センサーから離れている時間を計測する
        /// </summary>
        void LoseSightWait()
        {
            if (!_seenNow)
            {
                loseSightTime += _controller.DeltaTime;
            }
        }


        /// <summary>
        /// シスターさんのセンサー内に入っているかどうか
        /// seenがFalseならセンサーから出ている
        /// </summary>
        /// <param name="seen"></param>
        public void SisterRecognition(bool seen)
        {
            if (seen)
            {
                _seenNow = true;
                loseSightTime = 0;
            }
            else
            {
                _seenNow = false;
            }
        }


        /// <summary>
        /// この敵がシスターさんに現在認識されているかどうか
        /// 一定時間センサー内から離れた敵や死んだ敵は認識しない
        /// 時間はセンサーから離れた時にフラグ立てられて測り始める？
        /// それかロック矢印つけるコードでいろいろしていいかも
        /// 真なら見失ってる
        /// </summary>
        /// <returns></returns>
        public bool SisterCheck()
        {
            return (loseSightTime > 8 || _condition.CurrentState == CharacterStates.CharacterConditions.Dead);

        }


        #endregion



        #region　戦闘状態切り替え

        /// <summary>
        /// 攻撃状態開始
        /// 敵の情報を集めてセンサーの範囲も変更
        /// </summary>
        public void StartCombat(GameObject TriggerEnemy)
        {

            //敵いないなら
            //例えば飛び道具とかなら起動しない
            //その代わり敵を特定できない攻撃として、距離が一定以内なら攻撃してきた相手の位置まで行くように
            //そこで見つけたら戦闘開始でいいじゃん
            if(TriggerEnemy == null)
            {
                return;
            }

            //トークンを停止して新品に
            patrolToken.Cancel();
            patrolToken = new CancellationTokenSource();


            //後で戻れるようにオンにしないとな
            posiReset = true;



            if (dogPile != null)
            {
                basePosition = dogPile.transform.position;
                baseDirection = Math.Sign(dogPile.transform.localScale.x);
            }

            //戦闘開始時はdirectionは０です

            //この照合はString、オブジェクト名でしていいかも？
            targetImfo.targetNum = EnemyManager.instance.GetTargetNumber(TriggerEnemy);

            int num = targetImfo.targetNum;

            //敵の位置を保存し距離や方向も決める
            targetImfo.targetPosition = EnemyManager.instance._targetList[num]._condition.targetPosition;
            distance = targetImfo.targetPosition - myPosition;
            direction = distance.x >= 0 ? 1 : -1;

            //ヘイトも増やす？
           // _hateList[num] 

            isAggressive = true;
            NormalFlip(direction);


            //ここセンサー消さない？
            //いらんでしょ
            _sensor.RangeChange();


            //アーマー回復開始
            ArmorRecover().Forget();


            //戦闘開始時に呼び出す処理も
            CombatAction();
        }

        /// <summary>
        /// 戦闘終了
        /// </summary>
        public void CombatEnd()
        {

            isAggressive = false;

            //トークンを止めて新品に
            agrToken.Cancel();
            agrToken = new CancellationTokenSource();

            if (status._charaData._kind == EnemyStatus.KindofEnemy.Fly)
            {
                _flying.FastFly(false, true);
                _flying.FastFly(true, true);
            }
            else
            {
                _characterRun.RunStop();
                _guard.GuardEnd();
            }

            ground = EnemyStatus.MoveState.wakeup;
            air = EnemyStatus.MoveState.wakeup;
            _sensor.RangeChange();

            //警戒状態で呼び出すアクション
            PatrolAction();
        }

        #endregion


        /// <summary>
        ///  必要なアニメーターパラメーターがあれば、アニメーターパラメーターリストに追加します。
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            //    RegisterAnimatorParameter(_attackParameterName, AnimatorControllerParameterType.Bool, out _attackAnimationParameter);
            RegisterAnimatorParameter(_suppleParameterName, AnimatorControllerParameterType.Int, out _suppleAnimationParameter);
            RegisterAnimatorParameter(_combatParameterName, AnimatorControllerParameterType.Bool, out _combatAnimationParameter);
        }

        /// <summary>
        /// アビリティのサイクルが終了した時点。
        /// 現在のしゃがむ、這うの状態をアニメーターに送る。
        /// </summary>
        public override void UpdateAnimator()
        {
            //今のステートがAttackであるかどうかでBool入れ替えてる

            MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _suppleAnimationParameter, suppleNumber, _character._animatorParameters);
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _combatAnimationParameter, isAggressive, _character._animatorParameters);
        }





    }
}

