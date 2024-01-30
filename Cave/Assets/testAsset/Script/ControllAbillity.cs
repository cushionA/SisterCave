using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static EnemyStatus;
using static Equip;

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


    /// <summary>
    /// 倍率やステータスを操作するときに使う列挙型
    /// </summary>
    public enum StatusControlType
    {
        ステータス加算,
        ステータス乗算,
        倍率加算,
        倍率乗算
    }






    #endregion

    //変数
    //これいらなくね？　もう直接書き変えよう
    
    /// <summary>
    /// 防御関連の値
    /// 防御計算に使う
    /// すぐふらふら変わるところとめったに変わらないところがある
    /// 変わらないのはステート
    /// </summary>
    DefenseData myDefData;

    /// <summary>
    /// 攻撃力関連の値
    /// めったに変わらない
    /// </summary>
    AttackStatus myAtData;


    #region キャラの制御関連





    /// <summary>
    /// キャラクターの状態をマネージャーにセット
    /// ずっと呼び続ける
    /// </summary>
    public abstract void TargetDataUpdate(int num);

    /// <summary>
    /// キャラクターの状態をマネージャーに追加
    /// 最初だけ
    /// </summary>
    public abstract void TargetDataAdd(int newID);


    /// <summary>
    /// ターゲットリストから削除されたエネミーを消し去る
    /// そしてヘイトリストやらを調整
    /// プレイヤーはなんか別の処理入れてもいいかもな
    /// あと敵の死を通知するメソッドとしても使える
    /// </summary>
    /// <param name="deletEnemy"></param>
    public abstract void TargetListChange(int deletEnemy,int deadID);


    /// <summary>
    /// IDを確認
    /// これを使って味方と連携するので重複確認大事
    /// </summary>
    /// <returns></returns>
    public abstract int ReturnID();

    /// <summary>
    /// 味方が狙うターゲットを決定した際
    /// 指揮するイベントを持ってたら飛ばす
    /// コンバットマネージャーを通じて仲間に通達が行く
    /// でも敵側でしか実装しないかも
    /// </summary>
    public abstract void CommandEvent(TargetingEvent _event, int level, int targetNum, int commanderID);


    #endregion

    #region ダメージ計算関連


    /// 
    /// 目指す処理
    /// 
    /// ・なるべくキャラとヘルスのやり取りを少なく
    /// ・ヘルス側に共通数値の大半を持たせる
    /// ・アーマー関連はキャラ側でやる。シスターさんはMPがアーマーだったりするから
    /// ・ステータス変更時にキャラクターがヘルスやダメージ機能を更新する感じ
    /// ・防御状態のアーマー開始、攻撃中などは攻撃機能に持たせよう。敵側は攻撃アビリティ、プレイヤーは武器アビリティ
    /// ・防御状態のスタン状態はスタンアビリティが管理。スタン開始と終了で見る〇
    /// ・ガード中はガードアビリティと攻撃アビリティ△
    /// ・バフ関連はバフ適用時に呼ぶ（バフで直接ヘルスとかのデータ書き換えたらよくね？）
    /// 
    /// ・これをすると被弾時に何か呼び出したりしなくてよくなる
    /// ///





    /// <summary>
    /// 弾丸にバフの数値を与える
    /// 弾丸側から呼ぶ
    /// </summary>
    public abstract AttackData.AttackMultipler BulletBuffCalc();



    /// <summary>
    /// ダメージ機能の攻撃バフ（デバフ）の数値を更新
    /// 変更時のみ呼ぶ
    /// 倍率で属性無しだと全体倍率の変更
    /// リバースの時、もし現在バフやデバフが一つもなくなってるなら全部1、あるいは初期値に戻すように
    /// </summary>
    /// <param name="type">操作タイプ</param>
    /// <param name="changeVlue">変更数値</param>
    /// <param name="isReverse">元に戻す処理かどうか</param>
    public abstract void AttackStatusUpdate(StatusControlType type,AtEffectCon.Element changeElement,float changeVlue,bool isReverse);

    /// <summary>
    /// ダメージ機能の防御バフ（デバフ）、ステータスの数値を更新
    /// 変更時のみ呼ぶ
    /// 倍率で属性無しだと全体倍率の変更
    /// リバースの時、もし現在バフやデバフが一つもなくなってるなら全部1、あるいは初期値に戻すように
    /// </summary>
    /// <param name="type">操作タイプ</param>
    /// <param name="changeVlue">変更数値</param>
    /// <param name="isReverse">元に戻す処理かどうか</param>
    public abstract void DefStatusUpdate(StatusControlType type, AtEffectCon.Element changeElement, float changeVlue);


    /// <summary>
    /// パリィが成功した時の処理
    /// モーション開始
    /// </summary>
    /// <param name="isBreake"></param>
    public abstract void ParryStart(bool isBreake);


    /// <summary>
    /// ガードの音の処理
    /// 参照元が違うので処理をわける
    /// </summary>
    public abstract void GuardSound();

    /// <summary>
    /// スタン後ヘルスからアーマーを回復させるのに使う
    /// </summary>
    public abstract void ArmorReset();

    /// <summary>
    /// スタン時にコントロールアビリティを通じてスタン開始
    /// </summary>
    /// <param name="stunState"></param>
    public abstract void StartStun(MyWakeUp.StunnType stunState);

    /// <summary>
    /// 攻撃食らったとき
    /// ヘルスから送られたアーマー削りに応じてイベントとばす
    /// </summary>
    public abstract MyWakeUp.StunnType ArmorControll(float shock, bool isBlow, bool isBack);

    /// <summary>
    /// スタン時の音とかを出すために
    /// 現在のスタンが何かを通知する機能
    /// </summary>
    /// <returns></returns>
    public abstract int GetStunState();

    /// <summary>
    /// 自分がジャスガされた時アーマーブレイクするかどうかを伝える
    /// プレイヤーはジャスガ一発でパリィにする？
    /// </summary>
    /// <returns></returns>
    public abstract bool ParryArmorJudge();

    /// <summary>
    /// 空中ダウンに関する判定
    /// </summary>
    /// <param name="stunnState"></param>
    /// <returns></returns>
    public abstract bool AirDownJudge(MyWakeUp.StunnType stunnState);


    /// <summary>
    /// 自分の攻撃ステータスをダメージ機能に伝える
    /// </summary>
    public abstract void DamageCalc();



    /// <summary>
    /// 自分の防御ステータスをダメージ機能に伝える
    /// </summary>
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
    /// こちらがダメージ受けた後に呼び出される処理
    /// スタンしてるか、ダメージをどれくらい受けたか
    /// 誰に攻撃されたか、背後から攻撃を受けたかなどがわかる
    /// </summary>
    public abstract void DamageEvent(bool isStunn,GameObject enemy,int damage,bool back);



    //こんだけデリゲートがDamageOnTouchにあるなら使うべき
    //引数いらないなら使おう
    //あと+=でいくらでもデリゲート追加できる
    //public OnHitDelegate OnHit;
    //public OnHitDelegate OnHitDamageable;
    //public OnHitDelegate OnHitNonDamageable;
    //public OnHitDelegate OnKill;

    /// <summary>
    /// 自分の攻撃がヒットした時にどのようにヒットしたかを含めて教える
    /// こちらが攻撃を当てた時
    /// </summary>
    /// <param name="isBack">当てた相手が背中向けてたら真/param>
    public abstract void HitReport(bool isBack);


    /// <summary>
    /// 死亡処理
    /// </summary>
    public abstract void Die();

}
