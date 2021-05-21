using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class CharacterStatus : ScriptableObject
{


    //　毒状態かどうか
    public bool isPoison = false;

    //　痺れ状態かどうか
    public bool isParalyze = false;

    //　キャラクターのレベル
    public int level = 1;

    //　最大HP
    public float maxHp = 100;

    //　HP
    public float hp = 100;

    //　最大MP
    public float maxMp = 30;

    //　MP
    public float mp = 30;

    //　最大スタミナ
    public float maxStamina = 60;
    //　スタミナ
    //[HideInInspector]
    public float stamina = 60;

    //生命力
    public float Vitality = 1;
    //持久力
    public float Endurance = 1;
    //MPと魔法もてる数
    public float capacity = 1;
    //　力
    public float power = 1;
    //技量
    public float skill = 1;
    //　魔法力。賢さ
    public float _int = 1;


    public float Atk;
    //　無属性
    public float phyAtk;
    //神聖
    public float holyAtk;
    //闇
    public float darkAtk;
    //炎
    public float fireAtk;
    //雷
    public float thunderAtk;

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

    public float phyCut;//カット率
    public float holyCut;//光。
    public float darkCut;//闇。
    public float fireCut;//魔力
    public float thunderCut;//魔力

    public float guardPower;//受け値

    //isGuardの時使う

    //小怯み。普段は基本ゼロ。攻撃時だけ
    public float Armor = 1;
 //   [HideInInspector]public float nowArmor;

    public float nockBackPower;

    //大怯み、吹っ飛び。基本均一でアーマー値を足す

    public float attackBuff = 1.0f;
    //攻撃バフ値
    //[HideInInspector] public float defBuff;防御力は直接加算
}