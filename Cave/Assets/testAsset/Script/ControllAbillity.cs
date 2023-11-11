using MoreMountains.CorgiEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static EnemyStatus;

/// <summary>
/// ヘルスなどで使うキャラクターコントロールに必要なメソッドを積み込む
/// </summary>
public abstract class ControllAbillity : MyAbillityBase
{





    #region 定義



    /// <summary>
    /// 体力の割合とか変化するキャラデータ
    /// </summary>
    public struct ConditionData
    {
        /// <summary>
        /// 敵の位置
        /// </summary>
        public Vector2 targetPosition;

        /// <summary>
        /// 体力の割合
        /// </summary>
        public float hpRatio;

        /// <summary>
        /// MPの割合
        /// </summary>
        public float mpRatio;

        /// <summary>
        /// バフついているか
        /// </summary>
        public bool isBuffOn;

        /// <summary>
        /// デバフついているか
        /// </summary>
        public bool isDebuffOn;
    }

    #endregion

    #region キャラの制御関連





    /// <summary>
    /// キャラクターの状態をマネージャーにセット
    /// ずっと
    /// </summary>
    public abstract void TargetDataSet(int num);

    /// <summary>
    /// キャラクターの状態をマネージャーに追加
    /// 最初だけ
    /// </summary>
    public abstract void TargetDataAdd();

    /// <summary>
    /// ターゲットリストから削除されたエネミーを消し去る
    /// そしてヘイトリストやらを調整
    /// プレイヤーはなんか別の処理入れてもいいかもな
    /// あと敵の死を通知するメソッドとしても使える
    /// </summary>
    /// <param name="deletEnemy"></param>
    public abstract void TargetListChange(int deletEnemy);


    /// <summary>
    /// IDを確認
    /// これを使って味方と連携するので重複確認大事
    /// </summary>
    /// <returns></returns>
    public abstract int ReturnID();


    /// <summary>
    /// 味方がターゲットを決定した際
    /// イベントを持ってたら飛ばす
    /// </summary>
    public abstract void CommandEvent(TargetingEvent _event, int level, int targetNum, int commanderID);

    #endregion

    #region ダメージ計算関連

    public abstract void GuardReport();

    /// <summary>
    /// バフの数値を与える
    /// 弾丸から呼ぶ
    /// </summary>
    public abstract void BuffCalc(FireBullet _fire);

    public abstract void ParryStart(bool isBreake);

    public abstract void GuardSound();

    public abstract void ArmorReset();

    public abstract void StartStun(MyWakeUp.StunnType stunState);
    public abstract MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack);

    public abstract int GetStunState();

    /// <summary>
    /// パリィできたかチェック
    /// </summary>
    /// <returns></returns>
    public abstract bool ParryArmorJudge();

    /// <summary>
    /// 空中ダウンに関する判定
    /// </summary>
    /// <param name="stunnState"></param>
    /// <returns></returns>
    public abstract bool AirDownJudge(MyWakeUp.StunnType stunnState);

    public abstract void DamageCalc();
    public abstract void DefCalc();

#endregion







    /// <summary>
    /// 死亡モーションの開始
    /// </summary>
    public abstract void DeadMotionStart(MyWakeUp.StunnType stunnState);




    //こんだけデリゲートがHealthにあるなら使うべき
    //引数いらないなら使おう
    //あと+=でいくらでもデリゲート追加できる
    //やっぱダメデリゲートはInvokeで呼び出すたびにクラスのインスタンスを作ってる
    //負荷重すぎなんだよね

    //public OnDeathDelegate OnDeath;
    //public OnHitDelegate OnHit;
    //public OnHitZeroDelegate OnHitZero;
    //public OnReviveDelegate OnRevive;

    /// <summary>
    /// ダメージ受けた後に呼び出される処理
    /// </summary>
    public abstract void DamageEvent(bool isStunn,GameObject enemy);



    //こんだけデリゲートがDamageOnTouchにあるなら使うべき
    //引数いらないなら使おう
    //あと+=でいくらでもデリゲート追加できる
    //public OnHitDelegate OnHit;
    //public OnHitDelegate OnHitDamageable;
    //public OnHitDelegate OnHitNonDamageable;
    //public OnHitDelegate OnKill;

    /// <summary>
    /// 攻撃がヒットした時にどのようにヒットしたかを含めて教える
    /// </summary>
    /// <param name="isBack">当てた相手が自分の後ろにいる時は真</param>
    public abstract void HitReport(bool isBack);


    /// <summary>
    /// 死亡処理
    /// </summary>
    public abstract void Die();

}
