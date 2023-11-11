using DarkTonic.MasterAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class CharacterStatus : ScriptableObject
{

    #region 定義


    /// <summary>
    /// 送信するデータ、不変の物
    /// </summary>
    public struct CharacterData
    {

        /// <summary>
        /// 敵の種類
        /// 騎士とか射手とかそういうの
        /// </summary>
        public KindofEnemy _kind;

        /// <summary>
        /// キャラの属性というか
        /// タイプを示す
        /// </summary>
        public CharaType _type;


        /// <summary>
        /// 表示攻撃力
        /// </summary>
        public float displayAtk;

        /// <summary>
        /// 表示防御力
        /// </summary>
        public float displayDef;
    }


    public enum KindofEnemy
    {
        Soldier,//陸の雑兵
        Fly,//飛ぶやつ
        Shooter,//遠距離
        Knight,//盾持ち
        Trap,//待ち構えてるやつ
        none//指定なし
    }

    /// <summary>
    /// 所属する勢力的なもの
    /// </summary>
    public enum CharaType
    {
        Player,
        Sister,
        NPC,
        Enemy,
        Boss,
        none//指定なし
    }


    public enum AttackType
    {
        Slash,//斬撃
        Stab,//刺突
        Strike//打撃
    }


    [HideInInspector]
    public enum WeakPoint
    {
        Slash,
        Stab,
        Strike,
        Holy,
        Dark,
        Fire,
        Thunder,
        Poison
    }

    /// <summary>
    /// ヘイトのレベル
    /// このレベルが高いと他の敵に邪魔されない
    /// </summary>
    public enum hateLevel
    {
        weake,//雑魚
        normal,//基本はこれ
        strong,//強モブ
        absolute//ボスだけ
    }

    /// <summary>
    /// 行動を使用可能なモード
    /// 五つまで
    /// </summary>
    public enum Mode
    {
        Mode1,
        Mode2,
        Mode3,
        Mode4,
        Mode5,
        AllMode
    }
    /// <summary>
    /// モードを変える条件
    /// </summary>
    [Serializable]
    public struct ModeBehavior
    {

        /// <summary>
        /// アタックストップフラグが真の時だけ
        /// レベルとか関係なく発動
        /// </summary>
        [Header("攻撃抑制モードか")]
        public bool isAttackStop;

        /// <summary>
        /// 現在のモード
        /// </summary>
        [Header("遷移元のモード")]
        public Mode _nowMode;

        /// <summary>
        /// モードチェンジする体力割合
        /// 0なら無視
        /// </summary>
        [Header("モード変更する体力比。0で無視")]
        public int healthRatio;

        /// <summary>
        /// 前回のモードチェンジから何秒で変化するか
        /// 0なら無視
        /// </summary>
        [Header("モード変更時間")]
        public int changeTime;

        /// <summary>
        /// xからyの距離でこのモードに
        /// つまり直線距離Xメートルからyメートルの範囲ってことね
		/// 直線距離
        /// 00なら無視
        /// </summary>
        [Header("モード変更距離（00で無効）")]
        public Vector2 changeDistance;

        /// <summary>
        /// 変える先のモード
        /// Allならランダムに変わる
        /// 間合いとかの配列数からモードの数を割り出す
        /// </summary>
        [Header("変更先のモード")]
        public Mode changeMode;

        /// <summary>
        /// この条件のチェンジの優先度
        /// 0は基本モードにのみ使う
        /// いや基本モードはマイナス1でもいいな
        /// どこからでも戻れるように条件は軽く
        /// </summary>
        [Header("チェンジの優先度。5の時固定")]
        public int modeLevel;

    }





    #endregion


    public CharaType characterType;


    //　キャラクターのレベル
    public int level = 1;

    //　最大HP
    public float maxHp = 100;



    //　最大MP
    public float maxMp = 30;








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



    //isGuardの時使う

    //小怯み。普段は基本ゼロ。攻撃時だけ
    public float Armor = 1;


    //大怯み、吹っ飛び。基本均一でアーマー値を足す

    public CharacterData _charaData;




}
