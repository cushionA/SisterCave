using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本的にベーススクリプト運用。
/// これを継承したスクリプトのそれぞれ敵タイプ窓とかなんとかで（論理積とかの）値設定はやって、最後にApplyValueを使う
/// faseは各ボタンで決めて、setとeditはぼちぼち決めてや。あと継承先のスクリプトで表示する文字変えたり。
/// あと決定にに次の窓を呼び出す機能も
/// </summary>
public  class ValueChangeBase : MonoBehaviour
{
    ///<summary>
    /// 設定中のウィンドウはなにか
    /// 基本とか攻撃とか
    /// さらに攻撃0~2、回復3~5とかにして１なら条件2なら行動という風に分ける
    ///</summary>
    //   [HideInInspector]
    //  public int settingNumber;

    ///<summary>
    /// 何番目の条件を編集中か。参照先獲得
    /// 第一条件編集中とか
    /// セッティングナンバーと併用で特定できる
    ///</summary>
    //[HideInInspector]
    // public int editNumber;

    ///<summary>
    ///今何段階目の設定してるか。インスペクタから行く。
    ///例えばターゲットならWPかNextConditionか。スクリプト側で決める。変える。
    ///</summary>
    public int fase;

    /// <summary>
    /// 各スクリプトで値を保存するもの
    /// </summary>
    protected int numberSave;

    /// <summary>
    /// 各スクリプトで値を保存するもの
    /// </summary>
    protected bool boolSave;



    // Start is called before the first frame update
    public void NumberSet()
    {
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        //ドロップダウンなら
        //  0以上3以下
        if (fase >= 0 && fase < 3)
        {
            //ドロップダウンが何を参照してるかでUIのドロップダウンのやつを決める
            //objListからドロップダウンを引きずりだす
            //あとオブジェクトリストとSとeでうまいことSetActiveを操る。
            if (s == 1)
            {
                AttackJudge editor = GetTarget(e);
             //   Dropdown dd = objList[0].GetComponent<Dropdown>();
                //標的
                //ちょっと複雑すぎる
                if (fase == 0)
                {
                    //タイプゼロは五個ある


                    if (editor.condition == AttackJudge.TargetJudge.敵タイプ)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーのMPが規定値に達した際)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.自分のMPが規定値に達した際)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.強敵の存在)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.状態異常にかかってる敵)
                    {
                        numberSave = 7;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.かかってない支援がある)
                    {
                        
                        numberSave = 8;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.指定なし)
                    {
                        numberSave = 9;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.wp == AttackJudge.WeakPoint.斬撃属性)
                    {
                        numberSave = 0;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.刺突属性)
                    {
                        numberSave = 1;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.打撃属性)
                    {
                        numberSave = 2;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.聖属性)
                    {
                        numberSave = 3;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.闇属性)
                    {
                        numberSave = 4;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.炎属性)
                    {
                        numberSave = 5;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.雷属性)
                    {
                        numberSave = 6;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.毒属性)
                    {
                        numberSave = 7;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.指定なし)
                    {
                        numberSave = 8;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == AttackJudge.AdditionalJudge.敵のHP)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の距離)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の高度)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の攻撃力)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の防御力)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.指定なし)
                    {
                        numberSave = 10;
                    }

                }
            }
            //攻撃
            else if (s == 2)
            {
                FireCondition editor = GetAttack(e);
           //     Dropdown dd = objList[1].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (fase == 0)
                {
                    if (editor.condition == FireCondition.ActJudge.斬撃属性)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == FireCondition.ActJudge.刺突属性)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == FireCondition.ActJudge.打撃属性)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == FireCondition.ActJudge.聖属性)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == FireCondition.ActJudge.闇属性)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == FireCondition.ActJudge.炎属性)
                    {
                        numberSave = 5;
                    }
                    else if (editor.condition == FireCondition.ActJudge.雷属性)
                    {
                        numberSave = 6;
                    }
                    else if (editor.condition == FireCondition.ActJudge.毒属性)
                    {
                        numberSave = 7;
                    }
                    else if (editor.condition == FireCondition.ActJudge.移動速度低下攻撃)
                    {
                        numberSave = 8;
                    }
                    else if (editor.condition == FireCondition.ActJudge.攻撃力低下攻撃)
                    {
                        numberSave = 9;
                    }
                    else if (editor.condition == FireCondition.ActJudge.防御力低下攻撃)
                    {
                        numberSave = 10;
                    }
                    else if (editor.condition == FireCondition.ActJudge.属性指定なし)
                    {
                        numberSave = 11;
                    }
                    else if (editor.condition == FireCondition.ActJudge.支援行動に移行)
                    {
                        numberSave = 12;
                    }
                    else if (editor.condition == FireCondition.ActJudge.回復行動に移行)
                    {
                        numberSave = 13;
                    }
                    else if (editor.condition == FireCondition.ActJudge.なにもしない)
                    {
                        numberSave = 14;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.firstCondition == FireCondition.FirstCondition.敵を吹き飛ばす)
                    {
                        numberSave = 0;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.貫通する)
                    {
                        numberSave = 1;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.設置攻撃)
                    {
                        numberSave = 2;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.範囲攻撃)
                    {
                        numberSave = 3;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.追尾する)
                    {
                        numberSave = 4;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.サーチ攻撃)
                    {
                        numberSave = 5;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.指定なし)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == FireCondition.AdditionalCondition.発射数)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.攻撃力)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.削り値)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.指定なし)
                    {
                        numberSave = 10;
                    }


                }
            }
            else if (s == 3)
            {
                SupportCondition editor = GetSupport(e);
                //これは最初のドロップダウンだけ
               // Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (fase == 0)
                {
                    if (editor.sCondition == SupportCondition.SupportStatus.敵タイプ)
                    {
                        numberSave = 0;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーの体力が規定値の時)
                    {
                        numberSave = 1;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーのMPが規定値に達した際)
                    {
                        numberSave = 2;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.自分のMPが規定値に達した際)
                    {
                        numberSave = 3;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時)
                    {
                        numberSave = 4;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.強敵がいるかどうか)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }

                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.任意の支援が切れているとき)
                    {
                        numberSave = 7;
                    }

                    else if (editor.sCondition == SupportCondition.SupportStatus.指定なし)
                    {
                        numberSave = 8;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.攻撃強化)
                    {
                        numberSave = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.防御強化)
                    {
                        numberSave = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.エンチャント)
                    {
                        numberSave = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.アクション強化)
                    {
                        numberSave = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.バリア)
                    {
                        numberSave = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.リジェネ)
                    {
                        numberSave = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.なし)
                    {
                        numberSave = 6;
                    }
                }

            }
            else if (s == 4)
            {
                SupportCondition editor = GetSupport(e);
              //  Dropdown dd = objList[2].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (fase == 0)
                {
                    if (editor.ActBase == SupportCondition.MagicJudge.各種支援魔法)
                    {
                        numberSave = 0;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.攻撃ステートに)
                    {
                        numberSave = 1;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.回復ステートに)
                    {
                        numberSave = 2;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.なにもしない)
                    {
                        numberSave = 3;
                    }
                }
                else if (fase == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.攻撃強化)
                    {
                        numberSave = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.防御強化)
                    {
                        numberSave = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.エンチャント)
                    {
                        numberSave = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.アクション強化)
                    {
                        numberSave = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.バリア)
                    {
                        numberSave = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.リジェネ)
                    {
                        numberSave = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.なし)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == SupportCondition.AdditionalJudge.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.持続効果時間)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.強化倍率)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.指定なし)
                    {
                        numberSave = 8;
                    }

                }
            }
            else if (s == 5)
            {
                RecoverCondition editor = GetRecover(e);
                //これは最初のドロップダウンだけ
            //    Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (fase == 0)
                {
                    if (editor.condition == RecoverCondition.RecoverStatus.敵タイプ)
                    {
                        numberSave = 0;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーのHPが規定値の時)
                    {
                        numberSave = 1;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーのMPが規定値に達した際)
                    {
                        numberSave = 2;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.自分のMPが規定値に達した際)
                    {
                        numberSave = 3;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時)
                    {
                        numberSave = 4;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.強敵がいるかどうか)
                    {
                        if (editor.highOrLow)
                        {
                            numberSave = 5;
                        }
                        else
                        {
                            numberSave = 6;
                        }

                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                    {
                        numberSave = 7;
                    }

                    else if (editor.condition == RecoverCondition.RecoverStatus.指定なし)
                    {
                        numberSave = 8;
                    }

                }
                else if (fase == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.攻撃強化)
                    {
                        numberSave = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.防御強化)
                    {
                        numberSave = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.エンチャント)
                    {
                        numberSave = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.アクション強化)
                    {
                        numberSave = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.バリア)
                    {
                        numberSave = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.リジェネ)
                    {
                        numberSave = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.なし)
                    {
                        numberSave = 6;
                    }
                }
            }
            else
            {
                RecoverCondition editor = GetRecover(e);
               // Dropdown dd = objList[3].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (fase == 0)
                {
                    if (editor.ActBase == RecoverCondition.MagicJudge.治癒魔法)
                    {
                        numberSave = 0;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
                    {
                        numberSave = 1;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.支援ステートに)
                    {
                        numberSave = 2;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.なにもしない)
                    {
                        numberSave = 3;
                    }
                }
                else if (fase == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.攻撃強化)
                    {
                        numberSave = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.防御強化)
                    {
                        numberSave = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.エンチャント)
                    {
                        numberSave = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.アクション強化)
                    {
                        numberSave = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.バリア)
                    {
                        numberSave = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.リジェネ)
                    {
                        numberSave = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.なし)
                    {
                        numberSave = 6;
                    }
                }
                else if (fase == 2)
                {
                    if (editor.nextCondition == RecoverCondition.AdditionalJudge.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 0;
                        }
                        else
                        {
                            numberSave = 1;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.持続効果時間)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 2;
                        }
                        else
                        {
                            numberSave = 3;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 4;
                        }
                        else
                        {
                            numberSave = 5;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.回復量)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 6;
                        }
                        else
                        {
                            numberSave = 7;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.リジェネ回復量)
                    {
                        if (editor.upDown)
                        {
                            numberSave = 8;
                        }
                        else
                        {
                            numberSave = 9;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.状態異常回復)
                    {
                        numberSave = 10;
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.指定なし)
                    {
                        numberSave = 11;
                    }
                }
            }

        }
    }



    // Update is called once per frame

    /// <summary>
    /// 数値を適用するためのメソッド
    /// </summary>
    public void ApplyValue()
    {

        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        if (s == 1)
        {
            AttackJudge editJudge = MainUICon.instance.editParameter.targetCondition[e];

            ///<summary>
            ///ターゲット設定
            /// </summary>
            #region
            if (fase == 0)
            {
                if (numberSave == 0)
                {
                    editJudge.condition = AttackJudge.TargetJudge.敵タイプ;
                }
                else if (numberSave == 1)
                {
                    editJudge.condition = AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際;
                }
                else if (numberSave == 2)
                {
                    editJudge.condition = AttackJudge.TargetJudge.プレイヤーのMPが規定値に達した際;
                }
                else if (numberSave == 3)
                {
                    editJudge.condition = AttackJudge.TargetJudge.自分のMPが規定値に達した際;
                }
                else if (numberSave == 4)
                {
                    editJudge.condition = AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時;
                }
                else if (numberSave == 5)
                {
                    editJudge.condition = AttackJudge.TargetJudge.強敵の存在;
                    editJudge.highOrLow = true;
                }
                else if (numberSave == 6)
                {
                    editJudge.condition = AttackJudge.TargetJudge.強敵の存在;
                    editJudge.highOrLow = false;
                }
                else if (numberSave == 7)
                {
                    editJudge.condition = AttackJudge.TargetJudge.状態異常にかかってる敵;
                }
                else if (numberSave == 8)
                {
                //    Debug.Log($"ebihurai{gameObject.name}");
                    editJudge.condition = AttackJudge.TargetJudge.かかってない支援がある;
                }
                else if (numberSave == 9)
                {
                    editJudge.condition = AttackJudge.TargetJudge.指定なし;
                }

            }
            else if (fase == 1)
            {
                editJudge.percentage = numberSave;
                editJudge.highOrLow = boolSave;
            }
            else if (fase == 2)
            {
                //  editJudge.wp = saveWeak;

                if (numberSave == 0)
                {
                    editJudge.wp = AttackJudge.WeakPoint.斬撃属性;
                }
                else if (numberSave == 1)
                {
                    editJudge.wp = AttackJudge.WeakPoint.刺突属性;
                }
                else if (numberSave == 2)
                {
                    editJudge.wp = AttackJudge.WeakPoint.打撃属性;
                }
                else if (numberSave == 3)
                {
                    editJudge.wp = AttackJudge.WeakPoint.聖属性;
                }
                else if (numberSave == 4)
                {
                    editJudge.wp = AttackJudge.WeakPoint.闇属性;
                }
                else if (numberSave == 5)
                {
                    editJudge.wp = AttackJudge.WeakPoint.炎属性;
                }
                else if (numberSave == 6)
                {
                    editJudge.wp = AttackJudge.WeakPoint.雷属性;
                }
                else if (numberSave == 7)
                {
                    editJudge.wp = AttackJudge.WeakPoint.毒属性;
                }
                else if (numberSave == 8)
                {
                    editJudge.wp = AttackJudge.WeakPoint.指定なし;
                }
            }
            else if (fase == 3)
            {
                editJudge.upDown = false;
                if (numberSave == 0)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵のHP;
                }
                else if (numberSave == 1)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵のHP;
                }
                else if (numberSave == 2)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の距離;
                }
                else if (numberSave == 3)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の距離;
                }
                else if (numberSave == 4)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の高度;
                }
                else if (numberSave == 5)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の高度;
                }
                else if (numberSave == 6)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の攻撃力;
                }
                else if (numberSave == 7)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の攻撃力;
                }
                else if (numberSave == 8)
                {
                    editJudge.upDown = true;
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の防御力;
                }
                else if (numberSave == 9)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.敵の防御力;
                }
                else if (numberSave == 10)
                {
                    editJudge.nextCondition = AttackJudge.AdditionalJudge.指定なし;
                }
                
            }
            #endregion


            //設定
            MainUICon.instance.editParameter.targetCondition[e] = editJudge;

        }
        //攻撃選択の時
        else if (s == 2)
        {
            FireCondition editAT;
            //内容割り当て

            editAT = MainUICon.instance.editParameter.AttackCondition[e];

            //行動
            //条件
            //条件2＋真偽値
            ///<summary>
            ///攻撃選定
            /// </summary>
            #region
            if (fase == 0)
            {
                if (numberSave == 0)
                {
                    editAT.condition = FireCondition.ActJudge.斬撃属性;
                }
                else if (numberSave == 1)
                {
                    editAT.condition = FireCondition.ActJudge.刺突属性;
                }
                else if (numberSave == 2)
                {
                    editAT.condition = FireCondition.ActJudge.打撃属性;
                }
                else if (numberSave == 3)
                {
                    editAT.condition = FireCondition.ActJudge.聖属性;
                }
                else if (numberSave == 4)
                {
                    editAT.condition = FireCondition.ActJudge.闇属性;
                }
                else if (numberSave == 5)
                {
                    editAT.condition = FireCondition.ActJudge.炎属性;
                }
                else if (numberSave == 6)
                {
                    editAT.condition = FireCondition.ActJudge.雷属性;
                }
                else if (numberSave == 7)
                {
                    editAT.condition = FireCondition.ActJudge.毒属性;
                }
                else if (numberSave == 8)
                {
                    editAT.condition = FireCondition.ActJudge.移動速度低下攻撃;
                }
                else if (numberSave == 9)
                {
                    editAT.condition = FireCondition.ActJudge.攻撃力低下攻撃;
                }
                else if (numberSave == 10)
                {
                    editAT.condition = FireCondition.ActJudge.防御力低下攻撃;
                }
                else if (numberSave == 11)
                {
                    editAT.condition = FireCondition.ActJudge.属性指定なし;
                }
                else if (numberSave == 12)
                {
                    editAT.condition = FireCondition.ActJudge.支援行動に移行;
                }
                else if (numberSave == 13)
                {
                    editAT.condition = FireCondition.ActJudge.回復行動に移行;
                }
                else if (numberSave == 14)
                {
                    editAT.condition = FireCondition.ActJudge.なにもしない;
                }
            }
            else if (fase == 1)
            {
                //  MainUICon.instance.editParameter.firstTarget.wp = saveWeak;

                if (numberSave == 0)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.敵を吹き飛ばす;
                }
                else if (numberSave == 1)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.貫通する;
                }
                else if (numberSave == 2)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.設置攻撃;
                }
                else if (numberSave == 3)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.範囲攻撃;
                }
                else if (numberSave == 4)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.追尾する;
                }
                else if (numberSave == 5)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.サーチ攻撃;
                }
                else if (numberSave == 6)
                {
                    editAT.firstCondition = FireCondition.FirstCondition.指定なし;
                }

            }
            else if (fase == 2)
            {
                editAT.upDown = false;
                if (numberSave == 0)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.発射数;
                }
                else if (numberSave == 1)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.発射数;
                }
                else if (numberSave == 2)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.詠唱時間;
                }
                else if (numberSave == 3)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.詠唱時間;
                }
                else if (numberSave == 4)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.攻撃力;
                }
                else if (numberSave == 5)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.攻撃力;
                }
                else if (numberSave == 6)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.削り値;
                }
                else if (numberSave == 7)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.削り値;
                }
                else if (numberSave == 8)
                {
                    editAT.upDown = true;
                    editAT.nextCondition = FireCondition.AdditionalCondition.MP使用量;
                }
                else if (numberSave == 9)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.MP使用量;
                }
                else if (numberSave == 10)
                {
                    editAT.nextCondition = FireCondition.AdditionalCondition.指定なし;
                }  
            }
            #endregion
            //UseMagicをなしに
            editAT.UseMagic = null;

            MainUICon.instance.editParameter.AttackCondition[e] = editAT;

        }
        //支援条件の時
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //設定

            editSP = MainUICon.instance.editParameter.supportPlan[e];
            if (s == 3)
            {


                ///<summary>
                ///　支援条件
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.敵タイプ;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.プレイヤーの体力が規定値の時;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.プレイヤーのMPが規定値に達した際;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.自分のMPが規定値に達した際;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.強敵がいるかどうか;
                        editSP.highOrLow = true;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.強敵がいるかどうか;
                        editSP.highOrLow = false;
                    }
                    else if (numberSave == 7)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.任意の支援が切れているとき;
                    }
                    else if (numberSave == 8)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.指定なし;
                    }
                }
                else if (fase == 1)
                {
                    if (editSP.sCondition != SupportCondition.SupportStatus.任意の支援が切れているとき)
                    {
                        editSP.percentage = numberSave;
                        editSP.highOrLow = boolSave;
                    }
                    else
                    {
                        if (numberSave == 0)
                        {
                            editSP.needSupport = SisMagic.SupportType.攻撃強化;
                        }
                        else if (numberSave == 1)
                        {
                            editSP.needSupport = SisMagic.SupportType.防御強化;
                        }
                        else if (numberSave == 2)
                        {
                            editSP.needSupport = SisMagic.SupportType.エンチャント;
                        }
                        else if (numberSave == 3)
                        {
                            editSP.needSupport = SisMagic.SupportType.アクション強化;
                        }
                        else if (numberSave == 4)
                        {
                            editSP.needSupport = SisMagic.SupportType.バリア;
                        }
                        else if (numberSave == 5)
                        {
                            editSP.needSupport = SisMagic.SupportType.リジェネ;
                        }
                        else if (numberSave == 6)
                        {
                            editSP.needSupport = SisMagic.SupportType.なし;
                        }
                    }
                }
                #endregion

            }
            else if (s == 4)
            {
                ///<summary>
                /// 支援行動設定
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.各種支援魔法;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.攻撃ステートに;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.回復ステートに;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.ActBase = SupportCondition.MagicJudge.なにもしない;
                    }
                }
                else if (fase == 1)
                {
                    if (numberSave == 0)
                    {
                        editSP.useSupport = SisMagic.SupportType.攻撃強化;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.useSupport = SisMagic.SupportType.防御強化;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.useSupport = SisMagic.SupportType.エンチャント;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.useSupport = SisMagic.SupportType.アクション強化;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.useSupport = SisMagic.SupportType.バリア;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.useSupport = SisMagic.SupportType.リジェネ;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.useSupport = SisMagic.SupportType.なし;
                    }
                }
                else if (fase == 2)
                {
                    editSP.upDown = false;
                    if (numberSave == 0)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.詠唱時間;
                    }
                    else if (numberSave == 1)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.詠唱時間;
                    }
                    else if (numberSave == 2)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.持続効果時間;
                    }
                    else if (numberSave == 3)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.持続効果時間;
                    }
                    else if (numberSave == 4)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.MP使用量;
                    }
                    else if (numberSave == 5)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.MP使用量;
                    }
                    else if (numberSave == 6)
                    {
                        editSP.upDown = true;
                        editSP.nextCondition = SupportCondition.AdditionalJudge.強化倍率;
                    }
                    else if (numberSave == 7)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.強化倍率;
                    }
                    else if (numberSave == 8)
                    {
                        editSP.nextCondition = SupportCondition.AdditionalJudge.指定なし;
                    }
                }
                #endregion

            }
            //UseMagicをなしに
            editSP.UseMagic = null;
            //設定
            MainUICon.instance.editParameter.supportPlan[e] = editSP;
        }
        //支援行動選択

        //回復選択
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //設定
            #region
            if (!MainUICon.instance.isAH)
            {
                editRC = MainUICon.instance.editParameter.recoverCondition[e];
            }
            else
            {
                editRC = MainUICon.instance.editParameter.nRecoverCondition[e];
            }
            #endregion

            if (s == 5)
            {


                ///<summary>
                ///　支援条件
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.敵タイプ;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.プレイヤーのHPが規定値の時;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.プレイヤーのMPが規定値に達した際;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.自分のMPが規定値に達した際;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.強敵がいるかどうか;
                        editRC.highOrLow = true;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.強敵がいるかどうか;
                        editRC.highOrLow = false;
                    }
                    else if (numberSave == 7)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.任意の支援が切れているとき;
                    }
                    else if (numberSave == 8)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.指定なし;
                    }
                }
                else if (fase == 1)
                {
                    if (editRC.condition != RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                    {
                        editRC.percentage = numberSave;
                        editRC.highOrLow = boolSave;
                    }
                    else
                    {
                        if (numberSave == 0)
                        {
                            editRC.needSupport = SisMagic.SupportType.攻撃強化;
                        }
                        else if (numberSave == 1)
                        {
                            editRC.needSupport = SisMagic.SupportType.防御強化;
                        }
                        else if (numberSave == 2)
                        {
                            editRC.needSupport = SisMagic.SupportType.エンチャント;
                        }
                        else if (numberSave == 3)
                        {
                            editRC.needSupport = SisMagic.SupportType.アクション強化;
                        }
                        else if (numberSave == 4)
                        {
                            editRC.needSupport = SisMagic.SupportType.バリア;
                        }
                        else if (numberSave == 5)
                        {
                            editRC.needSupport = SisMagic.SupportType.リジェネ;
                        }
                        else if (numberSave == 6)
                        {
                            editRC.needSupport = SisMagic.SupportType.なし;
                        }
                    }
                }
                #endregion

            }
            else if (s == 6)
            {
                ///<summary>
                /// 支援行動設定
                /// </summary>
                #region
                if (fase == 0)
                {
                    if (numberSave == 0)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.治癒魔法;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.攻撃ステートに;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.支援ステートに;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.ActBase = RecoverCondition.MagicJudge.なにもしない;
                    }
                }
                else if (fase == 1)
                {
                    if (numberSave == 0)
                    {
                        editRC.useSupport = SisMagic.SupportType.攻撃強化;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.useSupport = SisMagic.SupportType.防御強化;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.useSupport = SisMagic.SupportType.エンチャント;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.useSupport = SisMagic.SupportType.アクション強化;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.useSupport = SisMagic.SupportType.バリア;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.useSupport = SisMagic.SupportType.リジェネ;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.useSupport = SisMagic.SupportType.なし;
                    }
                }
                else if (fase == 2)
                {
                    editRC.upDown = false;
                    if (numberSave == 0)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.詠唱時間;
                    }
                    else if (numberSave == 1)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.詠唱時間;
                    }
                    else if (numberSave == 2)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.持続効果時間;
                    }
                    else if (numberSave == 3)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.持続効果時間;
                    }
                    else if (numberSave == 4)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.MP使用量;
                    }
                    else if (numberSave == 5)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.MP使用量;
                    }
                    else if (numberSave == 6)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.回復量;
                    }
                    else if (numberSave == 7)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.回復量;
                    }
                    else if (numberSave == 8)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.リジェネ回復量;
                    }
                    else if (numberSave == 9)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.リジェネ回復量;
                    }
                    else if (numberSave == 10)
                    {
                        editRC.upDown = true;
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.リジェネ総回復量;
                    }
                    else if (numberSave == 11)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.リジェネ総回復量;
                    }
                    else if (numberSave == 12)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.状態異常回復;
                    }
                    else if (numberSave == 13)
                    {
                        editRC.nextCondition = RecoverCondition.AdditionalJudge.指定なし;
                    }
                   
                }
                #endregion

            }
            //設定

            //UseMagicをなしに
            editRC.UseMagic = null;

            #region
            if (!MainUICon.instance.isAH)
            {
                MainUICon.instance.editParameter.recoverCondition[e] = editRC;
            }
            else
            {
                MainUICon.instance.editParameter.nRecoverCondition[e] = editRC;
            }
            #endregion
        }


        MainUICon.instance.isSave = false;
        //MainUICon.instance.editParameter = sis;
    }

    //abstract public void ValueSet(ref int valueInt,ref bool valueBool) ;


    public void EditStart()
    {
        MainUICon.instance.editNow = true;
    }

    public void EditEnd()
    {
        MainUICon.instance.editNow = false;
    }


    AttackJudge GetTarget(int e)
    {
        return MainUICon.instance.editParameter.targetCondition[e];
    }
    FireCondition GetAttack(int e)
    {
        return MainUICon.instance.editParameter.AttackCondition[e];
    }

    SupportCondition GetSupport(int e)
    {
        return MainUICon.instance.editParameter.supportPlan[e];
    }
    RecoverCondition GetRecover(int e)
    {
        if (!MainUICon.instance.isAH)
        {
            return MainUICon.instance.editParameter.recoverCondition[e];
        }
        else
        {
            return MainUICon.instance.editParameter.nRecoverCondition[e];
        }
    }
}
