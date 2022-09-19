using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using SensorToolkit;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// 敵のセンサー
    /// 視界センサーだけはコライダーのオブジェクトから発信
    /// 疑問点、NPCやシスターさんに対するヘイト管理もするか
    /// あと攻撃中はサーチいる？　敵が距離離れたら解除でよくね
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/ESensorAbility")]
    public class ESensorAbillity : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "敵さんのセンサー"; }
        [SerializeField]
        PolygonCollider2D _sight;

        //--------------------------------------------------------------------------
        //フィールドサーチに必要なパラメーター
        [SerializeField]
        RangeSensor2D se;
        //  string dangerTag = "Danger";
        [SerializeField] EnemyAIBase brain;
        /// <summary>
        /// 周囲のオブジェクト検索でパルスを飛ばす範囲
        /// </summary>
        [SerializeField] float fieldRange = 70;
        //これは共通
        [SerializeField] float pulseWait = 3;
        float pulseTime;



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
            if (se.SensorRange != 0)
            {
                Serch();
            }

        }

        /// <summary>
        /// AggresiveSerch系統
        /// </summary>
        #region
        private void Serch()
        {
            if (!SManager.instance.isSerch)
            {
                pulseTime += _controller.DeltaTime;
                if (pulseTime >= pulseWait)
                {
                    //Debug.Log("機能してますよー");
                    se.Pulse();
                    SerchEnemy();

                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
                    pulseTime = 0;
                }
            }
        }

/// <summary>
/// もしかしたらシスターさんとかNPCとかに対する処理を足すかも
/// </summary>
        public void SerchEnemy()
        {
            //ターゲットリストをまっさらに

            //距離が近い順に敵を並び替える

            if (se.DetectedObjects.Count != 0)
            {
                brain.StartCombat();
            }

            //サーチタイプ、近いやつ優先とか色々つけてターゲット切り替えるのありかもですm

//            for (int i = 0; i < se.DetectedObjectsOrderedByDistance.Count; i++)
       //     {
  //              SManager.instance.TargetAdd(se.DetectedObjectsOrderedByDistance[i]);

    ///        }

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

                if (CheckFoundObject(collision.gameObject))
                {
                brain.StartCombat();
                    RangeChange();
                    //即座にポジション判断できるように
                    //Debug.Log("機能してますよー");
                    se.Pulse();
                    SerchEnemy();
                    //isSerchがつくと勝手にSマネージャーが敵リストの面倒を見てくれる
                    pulseTime = 0;

                    //検索はAgrSerchに任せる。いや入れていい。最初に検知されるのは近いやつだしどうせすぐ更新される
                }
        }




        #endregion

        public void RangeChange()
        {
            if (brain.isAggressive)
            {
                se.SensorRange = aggresiveRange;

                if(_sight != null)
               _sight.enabled = false;
            }
            else
            {
                se.SensorRange = fieldRange;
                if (_sight != null)
                _sight.enabled = true;
            }

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
