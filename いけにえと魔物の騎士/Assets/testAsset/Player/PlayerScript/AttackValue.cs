using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //これを書くとinspectorに表示される。
public struct AttackValue
{
    [Header("モーション値")]
    public float x;
    [Header("攻撃時のアーマー")]
    public float y;
    [Header("強靭削り")]
    public float z;
    [Header("ヒット数")]
    public int hitLimmit;
    [Header("吹き飛ばせるかどうか")]
    public bool isBlow;
    [Header("弾かれるかどうか")]
    public bool isLight;
    [Header("攻撃タイプ")]
    public Wepon.AttackType type;

}
