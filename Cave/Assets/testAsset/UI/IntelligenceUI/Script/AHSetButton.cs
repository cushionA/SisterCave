using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AHSetButton : MonoBehaviour
{

    [SerializeField]
    bool isAct;

    [SerializeField]
    int editNum;

    [SerializeField]
    Text myTex;

    [SerializeField]
    SettingWindowCon pare;



 

    public void ValueApply()
    {
        RecoverCondition e;
        if(editNum == 1)
        {
            e = MainUI.instance.editParameter.nFirstRecover;
        }
        else if(editNum == 2)
        {
            e = MainUI.instance.editParameter.nSecondRecover;
        }
        else
        {
            e = MainUI.instance.editParameter.nThirdRecover;
        }
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
            MainUI.instance.settingNumber = 6;
        }
        else
        {
            MainUI.instance.settingNumber = 5;
        }
        pare.nextWindow.SetActive(true);

    }
}
