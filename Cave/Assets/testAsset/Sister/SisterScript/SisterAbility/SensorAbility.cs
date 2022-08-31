using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using SensorToolkit;

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
        PolygonCollider2D _sight;

        //--------------------------------------------------------------------------
        //フィールドサーチに必要なパラメーター

        RangeSensor2D se;
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
        //-----------------------------------------------------------------------       
        //共通パラメーター

        int nowState;

        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            sister = GetComponent<BrainAbility>();
            se = GetComponent<RangeSensor2D>();
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

            if(sister.nowState == BrainAbility.SisterState.戦い)
            {
                AggresiveSerch();
            }
            else
            {
                FieldSerch();
            }
        }

        /// <summary>
        /// AggresiveSerch系統
        /// </summary>
        #region
        private void AggresiveSerch()
        {

            if (!SManager.instance.isSerch)
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("機能してますよー");
                    se.Pulse();
                    SerchEnemy();
                    SManager.instance.isSerch = true;
                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
                    pulseTime = 0;
                }
            }
        }
        public void SerchEnemy()
        {
            //ターゲットリストをまっさらに
            SManager.instance.targetList.Clear();
            SManager.instance.targetCondition.Clear();
            //距離が近い順に敵を並び替える
            for (int i = 0; i < se.DetectedObjectsOrderedByDistance.Count; i++)
            {
                SManager.instance.TargetAdd(se.DetectedObjectsOrderedByDistance[i]);
                  
            }
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
                    se.Pulse();
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
            if (se.GetDetectedByTag(SManager.instance.enemyTag).Count >= 1)
            { 
                sister.nowState = BrainAbility.SisterState.戦い;//この辺はまた後で設定できるようにしよう
                RangeChange();
                se.Pulse();
                SManager.instance.playObject = null;
                sister.isPlay = false;
                SerchEnemy();
                SManager.instance.GetClosestEnemyX();


                sister.reJudgeTime = 150;
            }
            //危険タグがあるなら警戒
            else if (se.GetDetectedByTag(SManager.instance.dangerTag).Count >= 1)
            {
                sister.nowState = BrainAbility.SisterState.警戒;
                //この辺は移動条件の初期化
                //ビフォーナンバー0で最初ってこと。ステートナンバー3で停止から始まる
                sister.stateNumber = 3;
                sister.beforeNumber = 0;
                sister.reJudgeTime = 0;
                sister.changeable = true;
                SManager.instance.playObject = null;
                sister.isPlay = false;
            }
            //遊びタグがあって遊んでないなら
            else if (se.GetDetectedByTag(SManager.instance.reactionTag).Count >= 1 && !sister.isPlay)
            {
                SManager.instance.playObject = se.GetNearestToPoint(SManager.instance.Sister.transform.position);
                sister.isPlay = true;
                sister.playPosition = SManager.instance.playObject.transform.position.x;
                sister.playDirection = SManager.instance.playObject.transform.localScale.x;
            }
        }

        #endregion

        /// <summary>
        /// 視界センサー周辺
        /// 視界ではisPlayトリガーしないぽい
        /// あとこれ視界オブジェクトのコライダーからトリガーしないとダメぽい
        /// </summary>
        #region
        /// <summary>
        /// レイを飛ばして壁越しではないか調べる
        /// </summary>
        /// <param name="i_target">感知した物体</param>
        /// <returns></returns>
        private bool CheckFoundObject(GameObject i_target)
        {
            Vector2 targetPosition = i_target.transform.position;//targetの位置を取得
            Vector2 myPosition = transform.position;//自分の位置




            Vector2 toTargetDir = (targetPosition - myPosition).normalized;

            if (!IsHitRay(myPosition, toTargetDir, i_target))
            {//IsHitsRayが真なら真を返す
                return false;
            }

            return true;


        }

        private bool IsHitRay(Vector2 i_fromPosition, Vector2 i_toTargetDir, GameObject i_target)
        {
            // 方向ベクトルが無い場合は、同位置にあるものだと判断する。
            if (i_toTargetDir.sqrMagnitude <= Mathf.Epsilon)
            {//sqrはベクトルの長さを返す
                return true;

            }

            RaycastHit2D onHitRay = Physics2D.Raycast(i_fromPosition, i_toTargetDir,/* SerchRadius,*/ layerMask.value);
            if (!onHitRay.collider)
            {
                return false;
            }
            //  ////Debug.log($"{onHitRay.transform.gameObject}");
            ////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);
            if (onHitRay.transform.gameObject != i_target)
            {//onHitRayは当たった場所
             //当たった場所がPlayerの位置でなければ
             //////Debug.log("あいに");
                return false;
            }

            return true;
        }


        /// <summary>
        /// これをセンサーのコライダーから呼び出すか
        /// </summary>
        /// <param name="collision"></param>
        public void SightSensor(Collider2D collision)
        {
            if (collision.tag == SManager.instance.enemyTag)
            {
                if (CheckFoundObject(collision.gameObject) && !SManager.instance.isEscape && sister.nowState != BrainAbility.SisterState.戦い)
                {
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                    RangeChange();
                    //即座にポジション判断できるように
                    sister.reJudgeTime = 150;
                    //Debug.Log("機能してますよー");
                    se.Pulse();
                    SerchEnemy();
                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
                    pulseTime = 0;
                    sister.nowState = BrainAbility.SisterState.戦い;//この辺はまた後で設定できるようにしよう
                    SManager.instance.GetClosestEnemyX();

                    //検索はAgrSerchに任せる。いや入れていい。最初に検知されるのは近いやつだしどうせすぐ更新される
                }
            }
            else if (collision.tag == SManager.instance.dangerTag)
            {

                if (CheckFoundObject(collision.gameObject) && sister.nowState != BrainAbility.SisterState.警戒)
                {
                    sister.nowState = BrainAbility.SisterState.警戒;

                    sister.stateNumber = 3;
                    sister.beforeNumber = 0;
                    sister.reJudgeTime = 0;
                    sister.changeable = true;
                    SManager.instance.playObject = null;
                    sister.isPlay = false;
                }
            }
        }




        #endregion

        public void RangeChange()
        {
            if(sister.nowState == BrainAbility.SisterState.戦い)
            {
                se.SensorRange = aggresiveRange;
                _sight.enabled = false;
            }
            else
            {
                se.SensorRange = fieldRange;
                _sight.enabled = true;
            }

        }
        /// <summary>
        /// 再探知
        /// </summary>
        public void ReSerch()
        {
            SManager.instance.targetList = null;
            SManager.instance.targetCondition = null;
            SManager.instance.target = null;
            isSerch = false;
            pulseTime = 100;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            SightSensor(collision.collider);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            SightSensor(collision.collider);
        }





    }
}

