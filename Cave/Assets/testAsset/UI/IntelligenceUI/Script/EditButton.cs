using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 設定窓。文字入れ替えとセッティングナンバーでもじ変更
/// エディットナンバー入れる
/// </summary>
public class EditButton : MonoBehaviour
{
    /// <summary>
    /// 何番目の箱かどうか
    /// </summary>
    public int editNum;

    /// <summary>
    /// これ真なら１足す、引く
    /// </summary>
    [SerializeField]
    bool isAct;

    [SerializeReference]
    Text myTex;

    Selectable my;


    [SerializeReference]
    SettingWindowCon pare;

    // Start is called before the first frame update
    void Start()
    {
        my = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log($"あじゃばー{MainUI.instance.edi}");
    }




    /// <summary>
    /// ボタンの文字変更
    /// どこで使うか、だな
    /// </summary>
    public void ChangeString()
    {
      
        int s = MainUI.instance.settingNumber;



        if (s == 1)
        {
            if (!isAct)
            {
                AttackJudge j = GetTarget();
                if (j.condition == AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際)
                {
                    myTex.text = "プレイヤーのHPが指定した値の時";
                }
                else if (j.condition == AttackJudge.TargetJudge.プレイヤーのMPが規定値に達した際)
                {
                    myTex.text = "プレイヤーのMPが指定した値の時";
                }
                else if (j.condition == AttackJudge.TargetJudge.自分のMPが規定値に達した際)
                {
                    myTex.text = "シスターさんのMPが指定した値の時";
                }
                else if (j.condition == AttackJudge.TargetJudge.状態異常にかかってる敵)
                {
                    myTex.text = "状態異常の敵に反応";
                }
                else if (j.condition == AttackJudge.TargetJudge.敵タイプ)
                {
                    myTex.text = "指定したタイプの敵に反応";
                }
                else if (j.condition == AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時)
                {
                    myTex.text = "プレイヤーが状態異常を受けた時";
                }
                else if (j.condition == AttackJudge.TargetJudge.かかってない支援がある)
                {
                    myTex.text = "効果切れの支援魔法がある時";
                }
                else if (j.condition == AttackJudge.TargetJudge.強敵の存在)
                {
                    if (!j.highOrLow)
                    {
                        //いないほう
                        myTex.text = "強敵以外に反応";
                    }
                    else
                    {
                        
                        myTex.text = "強敵に反応";
                    }
                }
                else
                {
                    myTex.text = "指定なし";
                }
            }
            else
            {
                FireCondition a = GetAttack();
                if(a.condition == FireCondition.ActJudge.斬撃属性)
                {
                    myTex.text = "斬撃属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.打撃属性)
                {
                    myTex.text = "打撃属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.刺突属性)
                {
                    myTex.text = "刺突属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.聖属性)
                {
                    myTex.text = "聖属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.闇属性)
                {
                    myTex.text = "闇属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.炎属性)
                {
                    myTex.text = "炎属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.雷属性)
                {
                    myTex.text = "雷属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.毒属性)
                {
                    myTex.text = "毒属性攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.攻撃力低下攻撃)
                {
                    myTex.text = "攻撃力低下攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.移動速度低下攻撃)
                {
                    myTex.text = "移動速度低下攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.防御力低下攻撃)
                {
                    myTex.text = "防御力低下攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.属性指定なし)
                {
                    myTex.text = "条件指定なしで攻撃";
                }
                else if (a.condition == FireCondition.ActJudge.支援行動に移行)
                {
                    myTex.text = "支援行動に移行";
                }
                else if (a.condition == FireCondition.ActJudge.回復行動に移行)
                {
                    myTex.text = "回復行動に移行";
                }
                else
                {
                    myTex.text = "なにもしない";
                }
            }
        }
        else if (s == 3)
        {
            SupportCondition e = GetSupport();
            if (!isAct)
            {
                if(e.sCondition == SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時)
                {
                    myTex.text = "プレイヤーが状態異常を受けた時";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.プレイヤーのMPが規定値に達した際)
                {
                    myTex.text = "プレイヤーのMPが指定した値の時";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.プレイヤーの体力が規定値の時)
                {
                    myTex.text = "プレイヤーのHPが指定した値の時";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.任意の支援が切れているとき)
                {
                    myTex.text = "指定した支援の効果が切れた時";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.強敵がいるかどうか)
                {
                    if (!e.highOrLow)
                    {
                        myTex.text = "強敵がいない時";
                    }
                    else
                    {
                        myTex.text = "強敵がいる時";
                    }
                    
                }
                else if (e.sCondition == SupportCondition.SupportStatus.指定なし)
                {
                    myTex.text = "指定なし";
                }
                else if (e.sCondition == SupportCondition.SupportStatus.敵タイプ)
                {
                    myTex.text = "指定したタイプの敵がいる時";
                }
                else
                {
                    myTex.text = "シスターさんのMPが指定した値の時";
                }
            }
            else
            {
                if(e.ActBase == SupportCondition.MagicJudge.各種支援魔法)
                {
                    myTex.text = "支援魔法を使用"; 
                }
                else if (e.ActBase == SupportCondition.MagicJudge.回復ステートに)
                {
                    myTex.text = "回復行動に移行";
                }
                else if (e.ActBase == SupportCondition.MagicJudge.攻撃ステートに)
                {
                    myTex.text = "攻撃行動に移行";
                }
                else
                {
                    myTex.text = "なにもしない";
                }
            }
        }
        else if (s == 5)
        {
            RecoverCondition e = GetRecover();
            if (!isAct)
            {
                if (e.condition == RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時)
                {
                    myTex.text = "プレイヤーが状態異常を受けた時";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.プレイヤーのMPが規定値に達した際)
                {
                    myTex.text = "プレイヤーのMPが指定した値の時";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.プレイヤーのHPが規定値の時)
                {
                    myTex.text = "プレイヤーのHPが指定した値の時";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                {
                    myTex.text = "指定した支援の効果が切れた時";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.強敵がいるかどうか)
                {
                    if (!e.highOrLow)
                    {
                        myTex.text = "強敵がいない時";
                    }
                    else
                    {
                        myTex.text = "強敵がいる時";
                    }

                }
                else if (e.condition == RecoverCondition.RecoverStatus.指定なし)
                {
                    myTex.text = "指定なし";
                }
                else if (e.condition == RecoverCondition.RecoverStatus.敵タイプ)
                {
                    myTex.text = "指定したタイプの敵がいる時";
                }
                else
                {
                    myTex.text = "シスターさんのMPが指定した値の時";
                }
            }
            else
            {
                if (e.ActBase == RecoverCondition.MagicJudge.治癒魔法)
                {
                    myTex.text = "回復魔法を使用";
                }
                else if (e.ActBase == RecoverCondition.MagicJudge.支援ステートに)
                {
                    myTex.text = "支援行動に移行";
                }
                else if (e.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
                {
                    myTex.text = "攻撃行動に移行";
                }
                else
                {
                    myTex.text = "なにもしない";
                }
            }
        }
    }


    /// <summary>
    /// ボタン押したとき
    /// </summary>
    public void SelectButton()
    {
        MainUI.instance.editNumber = editNum;
        //前の窓を操作不能に
        pare.enable = true;
        if (isAct)
        {
            MainUI.instance.settingNumber++;
        }
        pare.nextWindow.SetActive(true);

    }

    AttackJudge GetTarget()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstTarget;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondTarget;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdTarget;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthTarget;
        }
        else
        {
            return MainUI.instance.editParameter.fiveTarget;
        }
    }

    FireCondition GetAttack()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstAttack;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondAttack;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdAttack;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.fourthAttack;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fiveAttack;
        }
        else
        {
            return MainUI.instance.editParameter.nonAttack;
        }
    }

    SupportCondition GetSupport()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstPlan;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondPlan;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdPlan;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthPlan;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fivePlan;
        }
        else
        {
            return MainUI.instance.editParameter.sixPlan;
        }
    }
    RecoverCondition GetRecover()
    {
        if (editNum== 1)
        {
            return MainUI.instance.editParameter.firstRecover;
        }
        else if (editNum== 2)
        {
            return MainUI.instance.editParameter.secondRecover;
        }
        else if (editNum== 3)
        {
            return MainUI.instance.editParameter.thirdRecover;
        }
        else if (editNum== 4)
        {
            return MainUI.instance.editParameter.forthRecover;
        }
        else if (editNum== 5)
        {
            return MainUI.instance.editParameter.fiveRecover;
        }
        else
        {
            return MainUI.instance.editParameter.nonRecover;
        }
    }


}
