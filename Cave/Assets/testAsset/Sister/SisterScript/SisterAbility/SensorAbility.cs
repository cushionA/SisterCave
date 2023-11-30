using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using Micosmo.SensorToolkit;
using System.Threading;
using System;
using System.Linq;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// 必要な機能
    /// 
    /// ・周囲のものを検出
    /// ・戦闘時などの状態変化で範囲や感知するタグレイヤーも変わる
    /// ・物体は取得しない。数は取得する。敵がいくらいたかどうかだけ知らせる
    /// ・パルスはマニュアルで発動
    /// ・シスターさんは環境物にも反応する
    /// ・射線が通ってるかの確認もできるように
    /// ・あとできればワープする場所の障害物のチェックとかも
    /// 
    /// 範囲変更メソッド、パルス起動メソッド
    /// 非戦闘時の継続センサー、戦闘時の目的センサー（周辺敵数、射線判断など）
    /// センサーの機能調べなおそ
    /// 
    /// 仕様
    /// ・範囲センサーとトリガーセンサーで検知した敵にさらに視線センサーで検知していく
    /// ・この両者を統合するのがBooleanセンサーで、これは入力を受けるのみでパルスはしない
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/SensorAbility")]
    public class SensorAbility : MyAbillityBase
    {
        /// このメソッドは、ヘルプボックスのテキストを表示するためにのみ使用されます。
        /// 能力のインスペクタの冒頭にある
        public override string HelpBoxText() { return "シスターさんのセンサー"; }


        
        [SerializeField]
        ///<summary>
        ///視線チェックに使うセンサー
        /// 戦闘時はオフに
        /// </summary>
        LOSSensor2D rangeSight;

        [SerializeField]
        ///<summary>
        ///視線チェックに使うセンサー
        /// これはトリガー用
        /// </summary>
        LOSSensor2D triggerSight;

        /// <summary>
        /// 範囲センサー
        /// これはパルスしない
        /// 
        /// 範囲を変えるためだけに持つ
        /// </summary>
        [SerializeField]
        RangeSensor2D range;


        /// <summary>
        /// 統合用のセンサー
        /// 
        /// >ブール センサーをパルスする必要はありません。
        /// >入力センサーのイベントをサブスクライブし、入力信号が変更されるとすぐに更新します。
        /// </summary>
        [SerializeField]
        BooleanSensor sensor;


        //--------------------------------------------------------------------------



        /// <summary>
        /// 周囲のオブジェクト検索でパルスを飛ばす範囲
        /// </summary>
        [SerializeField] float fieldRange = 70;


        //これは共通
        [SerializeField] float pulseWait = 3;



        //-----------------------------------------------------------------------

        //戦闘中のセンサー、アグレッシブサーチのパラメーター
        /// <summary>
        /// 戦闘中パルスを飛ばす範囲
        /// </summary>
        [SerializeField] float aggresiveRange = 150;


        /// <summary>
        /// 戦闘中に使う敵の数
        /// </summary>
        int enemyCount;


        /// <summary>
        /// トリガー（視野）センサーを使うか
        /// </summary>
        bool useTrigger;

        /// <summary>
        /// 範囲センサーを使うか
        /// </summary>
        bool useRange;


        /// <summary>
        /// 敵数カウントのプロパティ
        /// </summary>
        public int EnemyCount {  get => enemyCount; private set => enemyCount = value; }


        /// <summary>
        /// キャンセルトークン
        /// </summary>
        CancellationTokenSource sensorCancel;

        //----------------------------------------------------------------------- センサー設定


        /// <summary>
        /// シスターさんなら環境サーチやら
        /// 戦闘中のカウントやらなんやら必要になる
        /// </summary>
        [SerializeField]
        bool isSister;


        /// <summary>
        /// 敵を見つけたら教えてあげるやつ
        /// </summary>
        [SerializeField]
        NPCControllerAbillity charaController;

        //-----------------------------------------------------------------------




        /// <summary>
        ///　ここで、パラメータを初期化する必要があります。
        /// </summary>
        protected override void Initialization()
        {

            //各センサーを使うか確認
            useRange = rangeSight != null;
            useTrigger = triggerSight != null;

            if (isSister)
            {
                SisterSensor().Forget();
            }
            else
            {
                NormalSensor().Forget() ;
            }
        }






        #region センサー群

        /// <summary>
        /// 戦闘中のセンサー
        /// 一秒に一回周囲の敵数を測る
        /// 戦闘モード突入時に必要なら呼ぶ
        /// </summary>
        async UniTaskVoid AggresiveSerch()
        {
            //一秒ごとに
            await UniTask.Delay(TimeSpan.FromSeconds(1),cancellationToken: sensorCancel.Token);

            UseSensor();

            //数を数えて教えてあげる
            enemyCount = sensor.GetDetections(SManager.instance.enemyTag).Count;

            //繰り返し
            AggresiveSerch().Forget();
        }



        /// <summary>
        /// 通常時に使うセンサー
        /// エネミーが使う敵を探すだけのセンサー
        /// </summary>
        /// <returns></returns>
        async UniTaskVoid NormalSensor()
        {
            //指定時間待機する
            await UniTask.Delay(TimeSpan.FromSeconds(pulseWait), cancellationToken: sensorCancel.Token);

            //パルスする
            UseSensor();

            //センサーでなにか見つけたなら報告
            if (sensor.GetDetections().Any())
            {
                charaController.FindEnemy();

            }
            //何もないなら繰り返し
            else
            {
                NormalSensor().Forget();
            }

        }


        async UniTaskVoid SisterSensor()
        {
            //指定時間待機する
            await UniTask.Delay(TimeSpan.FromSeconds(pulseWait), cancellationToken: sensorCancel.Token);

            //パルスする
            UseSensor();

            bool battleStart = false;

            //何か見つけたらしわけしろ
            if (sensor.GetDetections().Any())
            {
                battleStart = SortingDetectObjects();
            }

            //戦闘開始なら
            if (battleStart)
            {
                enemyCount = 0;
                //範囲変更
                range.Circle.Radius = aggresiveRange;
                AggresiveSerch().Forget();
            }
            else
            {
                //再呼び出し
                SisterSensor().Forget();
            }


        }




        #endregion


        #region キャラが使うメソッド

        /// <summary>
        /// 戦闘開始時に呼び出される
        /// 通常センサーを停止して必要なら戦闘センサーを起動する
        /// 
        /// 攻撃とか受けて戦闘開始した時に色々止めるのに必要
        /// </summary>
        public void BattleStart()
        {
            sensorCancel.Cancel();
            sensorCancel = new CancellationTokenSource();

            //戦闘センサーが必要なのは現状シスターさんだけ
            if (isSister)
            {
                enemyCount = 0;

                //範囲変更
                range.Circle.Radius = aggresiveRange;
                AggresiveSerch().Forget();
            }

        }


        /// <summary>
        /// 戦闘終了、通常センサーを呼び出す
        /// </summary>
        public void BattleEnd()
        {


            //戦闘センサーがあるのでここだけキャンセルがいる
            if (isSister)
            {
                sensorCancel.Cancel();
                sensorCancel = new CancellationTokenSource();
                range.Circle.Radius = fieldRange;

                SisterSensor().Forget();
            }
            else
            {
                NormalSensor().Forget();
            }
        }





        #endregion


        /// <summary>
        /// FieldSerch系統
        /// </summary>
        #region


        /// <summary>
        /// 見つけたものを仕分けする
        /// シスターさん専用
        /// </summary>
        /// <param name="collision"></param>
        bool SortingDetectObjects()
        {

            bool findEnemy = false; 

            //敵タグがあるなら
            if (sensor.GetDetections(SManager.instance.enemyTag).Any())
            {
                findEnemy = true;
                charaController.FindEnemy();
            }
            //危険タグがあるなら警戒
            else if (sensor.GetDetections(SManager.instance.dangerTag).Any())
            {
                charaController.ReportObject(true,sensor.GetNearestDetection(SManager.instance.dangerTag));
            }
            //環境物タグがあったら
            else if (sensor.GetDetections(SManager.instance.reactionTag).Any())
            {
                charaController.ReportObject(false, sensor.GetNearestDetection(SManager.instance.reactionTag));
            }
            //何もないなら何もないを返す
            charaController.ReportObject(false, null);
            return findEnemy;
        }

        #endregion








        /// <summary>
        /// センサー処理
        /// </summary>
        void UseSensor()
        {
            //トリガーセンサーを使うなら
            if (useTrigger)
            {
                triggerSight.Pulse();
            }

            //範囲センサーを使うなら
            if (useRange)
            {
                //入力センサーまで一括で
                rangeSight.PulseAll();
            }

        }




    }
}

