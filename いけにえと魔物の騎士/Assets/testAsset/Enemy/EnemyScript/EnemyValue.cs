using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public struct EnemyValue
{
    [Header("モーション値")]
    public float mvalue;
    [Header("攻撃時のアーマー")]
    public float addArmor;
    [Header("強靭削り")]
    public float shock;
    [Header("ヒット数")]
    public int hitLimmit;
    [Header("吹き飛ばせるかどうか")]
    public bool isBlow;
    [Header("弾かれるかどうか")]
    public bool isLight;
    [Header("攻撃タイプ")]
    public EnemyStatus.AttackType type;
    [Header("クールタイム")]
    public float coolList;
    [Header("吹き飛ばす力")]
    public Vector2 blowPower;
    [Header("コンボするかどうか")]
    public bool isCombo;
    [Header("遠距離攻撃かどうか")]
    public bool isShoot;
    [Header("パリィできるかどうか")]
    public bool parriable;
}
