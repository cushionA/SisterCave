using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterStatus;
using static Equip;

public struct DefenseData
{

    #region 定義

    /// <summary>
    /// 防御状態
    /// すぐ変わるものを集める
    /// </summary>
    public enum DefState
    {
        攻撃中 = 1 << 0,
        アーマー付き = 1 << 1,
        スーパーアーマー = 1 << 2,
        被ダメージ増大 = 1 << 3,
        ガード中 = 1 << 4,
    }

    /// <summary>
    /// 防御倍率計算に使う数値
    /// </summary>
    public struct DefMultipler
    {
        /// <summary>
        /// 全防御に対する倍率
        /// </summary>
        public float allDefMultipler;

        /// <summary>
        /// 防御倍率
        /// </summary>
        public float phyDefMultipler;



        /// <summary>
        /// 聖防御倍率
        /// </summary>
        public float holyDefMultipler;
        /// <summary>
        /// 闇防御倍率
        /// </summary>
        public float darkDefMultipler;
        /// <summary>
        /// 炎防御倍率
        /// </summary>
        public float fireDefMultipler;

        /// <summary>
        /// 雷防御倍率
        /// </summary>
        public float thunderDefMultipler;
    }
    #endregion

    /// <summary>
    /// 防御力
    /// </summary>
    [HideInInspector]
    public DefStatus status;

    /// <summary>
    /// 防御倍率
    /// </summary>
    [HideInInspector]
    public DefMultipler multipler;

    /// <summary>
    /// ガードステータス
    /// </summary>
    [HideInInspector]
    public GuardStatus guardStatus;
    
    
    /// <summary>
    /// プレイヤーのみかな
    /// </summary>
    public bool nowParry;

    /// <summary>
    /// 防御関連の状態
    /// 頻繫に移り変わる
    /// </summary>
    public DefState state;

}
