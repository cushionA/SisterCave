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


    //　毒状態かどうか
    public bool isPoison = false;

    //　痺れ状態かどうか
    public bool isParalyze = false;

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

    //攻撃バフ値
    //[HideInInspector] public float defBuff;防御力は直接加算


    [Header("使用する音声")]
    [SoundGroup]
    ///<Summary>
    ///　使用する音声のリスト。
    ///　Status.useSound[i]という形でアニメイベントで指定
    ///</Summary>
    public List<string> useSound;
    [Header("足音の音声")]
    [SoundGroup]
    ///<Summary>
    ///　使用する音声
    ///</Summary>
    public string footStepSound;

    [Header("歩く音声")]
    [SoundGroup]
    ///<Summary>
    ///　使用する音声
    ///</Summary>
    public string walkSound;

}
