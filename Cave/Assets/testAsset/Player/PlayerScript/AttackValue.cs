using UnityEngine;
using UnityEngine.AddressableAssets;


[System.Serializable] //これを書くとinspectorに表示される。

public class AttackValue
{

    #region 固有部分の定義

    #endregion

    /// <summary>
    /// 攻撃モーションに関する共通データ
    /// </summary>
    [Header("攻撃モーションに関する共通データ")]
    public AttackValueBase baseData;


    [Header("スタミナ消費")]
    public int useStamina;


    /// <summary>
    /// 盾の攻撃力を参照する攻撃かどうか
    /// </summary>
    [HideInInspector]
    public bool isShield;






}
