using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SisterParameter", menuName = "CreateSisterParameter")]
public class SisterParameter : ScriptableObject
{
    [Header("作戦名")]
    public string oparationName;

    [Header("作戦説明")]
    public string oparationDescription;

    [Header("何番目の作戦か示す")]
    public int FairstNumber;

    [Header("先制攻撃するかどうか")]
    public bool isPreemtive;
    [Header("最初に支援するかどうか")]
    public bool startSupport;//最初に支援させて優先ステートに戻る。戦闘ステート解除の時に再セットだな
    [Header("道中で勝手に回復させるかどうか")]
    public bool autoHeal;
    [Header("初期ステートに戻って判断しなおす時間")]
    ///<summary>
    ///　一秒から三十秒までで設定できるように選べる
    /// </summary>
    public float stateResetRes;
    [Header("ターゲットの再設定間隔")]
    ///<summary>
    ///　三秒から三十秒までで設定できるように選べる
    /// </summary>
    public float targetResetRes;

    [Header("攻撃のクールタイム")]
    ///<Summary>
    /// 攻撃のクールタイム
    ///</Summary>
    public List<float> attackCT = new List<float>(6);

    [Header("支援のクールタイム")]
    ///<Summary>
    /// 支援のクールタイム
    ///</Summary>
    public List<float> supportCT = new List<float>(6);

    [Header("回復のクールタイム")]
    ///<Summary>
    /// 回復のクールタイム
    ///</Summary>
    public List<float> healCT = new List<float>(6);

    [Header("道中回復のクールタイム")]
    ///<Summary>
    ///  道中回復のクールタイム
    ///</Summary>
    public List<float> autHealCT = new List<float>(3);
    public enum MoveType
    {
        攻撃,
        回復,
        支援,
        なし
    }
    [Header("どのタイプの魔法を優先するか")]
    public MoveType priority;
    [Header("現在どのタイプの魔法を使おうとしてるのか")]
    [HideInInspector] public MoveType nowMove;

    #region//nowStateが攻撃の時


/// <summary>
/// ターゲット選択
/// </summary>
    public AttackJudge[] targetCondition = new AttackJudge[5];//判断条件セット

    /// <summary>
    /// 攻撃魔法選択
    /// </summary>
    public FireCondition[] AttackCondition = new FireCondition[6];//一個目の条件に当てはまるやつ


    //UIチェックボックス入れて数字変える
    [Header("クールタイムをスキップする条件")]
    [Tooltip("第一1,第二2,第三4,第四8,第五16,問わず0")]
    /// <summary>
    /// 第一、1000000
    //  第二、0100000
    //　第三、0010000
    //　第四、00010000
    /// </summary>
    public int[] atSkipList = new int[6];

    #endregion

    #region//nowStateが支援の時

    public SupportCondition[] supportPlan = new SupportCondition[6];

    public int[] sSkipList = new int[6];



    #endregion

    #region//nowStateが回復の時
    public RecoverCondition[] recoverCondition = new RecoverCondition[6];//一個目の条件に当てはまるやつ


    public int[] hSkipList = new int[6];

    #endregion

    public RecoverCondition[] nRecoverCondition = new RecoverCondition[3];//一個目の条件に当てはまるやつ


    public int[] ahSkipList = new int[3];


}
