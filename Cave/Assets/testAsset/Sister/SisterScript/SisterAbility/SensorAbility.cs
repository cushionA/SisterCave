using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Micosmo.SensorToolkit;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// シスターさんのセンサー
    /// 視界センサーだけはコライダーのオブジェクトから発信
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/SensorAbility")]
    public class SensorAbility : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "シスターさんのセンサー"; }

        //   [Header("武器データ")]
        /// declare your parameters here
        ///WeaponHandle参考にして 

        
        [SerializeField]
        /// <summary>
        /// 視界のコライダー
        /// </summary>
        GameObject _sight;

        LOSSensor2D _sightSensor;

        RangeSensor2D range;

        //--------------------------------------------------------------------------
        //フィールドサーチに必要なパラメーター
        [SerializeField]
        LOSSensor2D se;
        //  string dangerTag = "Danger";
        [SerializeField] BrainAbility sister;
        /// <summary>
        /// 周囲のオブジェクト検索でパルスを飛ばす範囲
        /// </summary>
        [SerializeField] float fieldRange = 70;
        //これは共通
        [SerializeField] float pulseWait = 3;
        float pulseTime;
        bool isSerch;


        //-----------------------------------------------------------------------
        //エネミーサーチ。視界センサーに必要なパラメーター

        //危険物はトラップエリアの入り口とかにおく？
        //危険フラグ立ててその間は動かないとか新しく待機ステート作って待機させるとか

        //  public float SerchRadius;
        [SerializeField]
        private LayerMask layerMask;

        //-----------------------------------------------------------------------
        //戦闘中のセンサー、アグレッシブサーチのパラメーター
        /// <summary>
        /// 戦闘中パルスを飛ばす範囲
        /// </summary>
        [SerializeField] float aggresiveRange = 150;

        private readonly UniTaskCompletionSource
            uniTaskCompletionSource = new UniTaskCompletionSource();

        public UniTask SerchAsync => uniTaskCompletionSource.Task;
        



        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _sightSensor = _sight.MMGetComponentNoAlloc<LOSSensor2D>();
            range = (RangeSensor2D)se.InputSensor;
            RangeChange();
        }

        /// <summary>
        /// 1フレームごとに、しゃがんでいるかどうか、まだしゃがんでいるべきかをチェックします
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            DoSomething();

        }



        /// <summary>
        /// 押し込んでいる場合は、いくつかの条件を満たしているかどうかをチェックして、アクションを実行できるかどうかを確認します。
        /// </summary>
        protected virtual void DoSomething()
        {
            // if the ability is not permitted
            if (!AbilityPermitted)
            {
                // we do nothing and exit
                return;
            }

            if(sister.nowState != BrainAbility.SisterState.戦い)
            {
                FieldSerch();
            }
        }

        /// <summary>
        /// 戦闘中のセンサー
        /// FireAbillityから呼び出して使う
        /// </summary>
        #region
        public async void AggresiveSerch()
        {

                //Debug.Log("機能してますよー");
                SensorPulse();

                
            SManager.instance.TargetAdd(se.GetDetectionsByDistance(SManager.instance.enemyTag));
            await SManager.instance._addAsync;
         //   Debug.Log("ｓでｆ");
            //処理終了通知
            uniTaskCompletionSource.TrySetResult();
                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
        }
        public void SerchEnemy()
        {
            //距離が近い順に敵を並び替える
            SensorPulse();
            SManager.instance.InitialAdd(se.GetDetectionsByDistance(SManager.instance.enemyTag));
        }
        #endregion

        /// <summary>
        /// FieldSerch系統
        /// </summary>
        #region
        void FieldSerch()
        {

            if (!isSerch) 
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("機能してますよー");
                    SensorPulse();
                    isSerch = true;
                    pulseTime = 0;
                }
            }
            else if (isSerch)
            {
                DetectObject();
                isSerch = false;
            }
        }

        public void DetectObject()
        {
            //敵タグがあるなら
            if (se.GetDetections(SManager.instance.enemyTag).Count >= 1)
            {
                Debug.Log("La");
                RangeChange();
                SerchEnemy();
                sister.StateChange(BrainAbility.SisterState.戦い);
                
                SManager.instance.GetClosestEnemyX();


                sister.reJudgeTime = 150;
            }
            //危険タグがあるなら警戒
            else if (se.GetDetections(SManager.instance.dangerTag).Count >= 1)
            {
                sister.nowState = BrainAbility.SisterState.警戒;
                //この辺は移動条件の初期化
                //ビフォーナンバー0で最初ってこと。ステートナンバー3で停止から始まる

                sister.reJudgeTime = 0;
                sister.changeable = true;
                SManager.instance.playObject = null;
                sister.isPlay = false;
            }
            //遊びタグがあって遊んでないなら
            else if (se.GetDetections(SManager.instance.reactionTag).Count >= 1 && !sister.isPlay)
            {
                SManager.instance.playObject = se.GetNearestDetectionToPoint(SManager.instance.Sister.transform.position);
                sister.isPlay = true;
                sister.playPosition = SManager.instance.playObject.transform.position.x;
                sister.playDirection = SManager.instance.playObject.transform.localScale.x;
            }
        }

        #endregion




        /// <summary>
        /// これをセンサーのコライダーから呼び出すか
        /// </summary>
        /// <param name="collision"></param>
        public void FindObject()
        {
            GameObject obj = _sightSensor.GetNearestDetection();

            GameObject pick = null;
            for (int i= 0; i < _sightSensor.Detections.Count;i++)
            {
                pick  = _sightSensor.GetDetections()[i];

                //危険物があったらそっち優先
                if (pick.tag == SManager.instance.dangerTag)
                {
                    obj = pick;
                    continue;
                }
                //敵のオブジェクトがあったらそっち最優先
                if (pick.tag == SManager.instance.enemyTag)
                {
                    obj = pick;
                    break;
                }
            }

            if (obj.tag == SManager.instance.enemyTag)
            {
                if (sister.nowState != BrainAbility.SisterState.戦い)
                {
                    sister.StateChange(BrainAbility.SisterState.戦い);
                    RangeChange();
                    //即座にポジション判断できるように

                    SerchEnemy();
                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
                    pulseTime = 0;
                    SManager.instance.GetClosestEnemyX();

                    //検索はAgrSerchに任せる。いや入れていい。最初に検知されるのは近いやつだしどうせすぐ更新される
                }
            }
            else if (obj.tag == SManager.instance.dangerTag)
            {

                if (sister.nowState == BrainAbility.SisterState.のんびり)
                {
                    sister.nowState = BrainAbility.SisterState.警戒;


                    sister.changeable = true;
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                }
            }
            else if (obj.tag == SManager.instance.reactionTag)
            {
                if (sister.nowState == BrainAbility.SisterState.のんびり)
                {
                    SManager.instance.playObject = se.GetNearestDetectionToPoint(SManager.instance.Sister.transform.position);
                    sister.isPlay = true;
                    sister.playPosition = SManager.instance.playObject.transform.position.x;
                    sister.playDirection = SManager.instance.playObject.transform.localScale.x;
                }
            }

        }

        public void RangeChange()
        {
            
            if(sister.nowState == BrainAbility.SisterState.戦い)
            {
               
                range.Circle.Radius = aggresiveRange;
                if (_sight != null)
                    _sight.SetActive(false);
            }
            else
            {
                range.Circle.Radius = fieldRange;
                if (_sight != null)
                    _sight.SetActive(true);
            }

        }


        void SensorPulse()
        {
            range.Pulse();
            se.Pulse();
        }


    }
}

