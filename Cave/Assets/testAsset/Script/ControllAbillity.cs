using MoreMountains.CorgiEngine;
using System;
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
        /// 体力の数値
        /// </summary>
        public float hpNum;

        /// <summary>
        /// MPの割合
        /// </summary>
        public float mpRatio;

        /// <summary>
        /// ついてるバフを記録
        /// </summary>
        public PositiveCondition buffImfo;

        /// <summary>
        /// ついているデバフを記録
        /// </summary>
        public NegativeCondition debuffImfo;


        public TargettigData target;
    }

    /// <summary>
    /// ターゲットしてる相手が誰なのかを示すデータ
    /// </summary>
    public struct TargettigData
    {
        /// <summary>
        /// 標的の所属
        /// </summary>
        public Side targetSide;

        /// <summary>
        /// 標的の番号
        /// </summary>
        public int targetNum;
    }

    /// <summary>
    /// どの陣営を表すか
    /// </summary>
    public enum Side
    {
        Enemy = 1,
        Player = 2,
        Other = 3,
        なし = 0//敵を認識してない
    }


    /// <summary>
    /// いい効果、いわゆるバフ
    /// </summary>
    [Flags]

    public enum PositiveCondition
    {
        HP継続回復 = 1,
        MP継続回復 = 1 << 1,
        防御力上昇 = 1 << 2,
        攻撃力上昇 = 1 << 3,
        被ダメージカット = 1 << 4,
        与ダメージ増加 = 1 <<5 ,
        移動速度上昇 = 1 << 6,

        スタミナ回復加速 = 1 << 7,
        ガード性能上昇 = 1 << 8,
        アーマー上昇 = 1 << 9,
        攻撃HP回復 = 1 << 10,
        撃破HP回復 = 1 << 11,
        攻撃MP回復 = 1 << 12,
        撃破MP回復 = 1 << 13,
        飛び道具バリア = 1 <<14,
        状態異常耐性上昇 = 1 << 15,//毒とか個別にするかも
    }


    /// <summary>
    /// 悪い効果、いわゆるデバフ
    /// </summary>
    [Flags]

    public enum NegativeCondition
    {
        毒 = 1 << 0,//凍結などの状態異常との区別どうする？　蓄積した時ここのデータを見るとか？
        浸食 = 1 << 1,//MP毒
        凍結 = 1 << 2,//移動停止
        沈黙 = 1<<3,//魔術禁止
        防御力減少 = 1 << 4,
        攻撃力減少 = 1 << 5,
        被ダメージ増加 = 1 << 6,
        与ダメージ減少 = 1 << 7,
        移動速度低下 = 1 << 8,
        スタミナ回復減速 = 1 << 9,
        ガード性能低下 = 1 << 10,
        アーマー低下 = 1 << 11,
        ヘイト上昇 = 1 << 12,
        状態異常耐性減少 = 1 << 13,//毒とか個別にするかも
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
