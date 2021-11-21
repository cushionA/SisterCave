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
    virtual protected void Start()
    {
        
    }

    // Update is called once per frame

    /// <summary>
    /// 数値を適用するためのメソッド
    /// </summary>
    public void ApplyValue()
    {
        SisterParameter sis = MainUI.instance.editParameter;
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;
        if (s == 1)
        {
            AttackJudge editJudge = new AttackJudge();
            //設定
            #region
            if (e == 1)
            {
                editJudge = sis.firstTarget;
            }
            else if (e == 2)
            {
                editJudge = sis.secondTarget;
            }
            else if (e == 3)
            {
                editJudge = sis.thirdTarget;
            }
            else if (e == 4)
            {
                editJudge = sis.forthTarget;
            }
            else
            {
                editJudge = sis.fiveTarget;
            }
            #endregion

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
            #region
            if (e == 1)
            {
                sis.firstTarget = editJudge;
            }
            else if (e == 2)
            {
                sis.secondTarget = editJudge;
            }
            else if (e == 3)
            {
                sis.thirdTarget = editJudge;
            }
            else if (e == 4)
            {
                sis.forthTarget = editJudge;
            }
            else if (e == 5)
            {
                sis.fiveTarget = editJudge;
            }
            #endregion
        }
        //攻撃選択の時
        else if (s == 2)
        {
            FireCondition editAT;
            //内容割り当て
            #region
            if (e == 1)
            {
                editAT = sis.firstAttack;
            }
            else if (e == 2)
            {
                editAT = sis.secondAttack;
            }
            else if (e == 3)
            {
                editAT = sis.thirdAttack;
            }
            else if (e == 4)
            {
                editAT = sis.fourthAttack;
            }
            else if (e == 5)
            {
                editAT = sis.fiveAttack;
            }
            else
            {
                editAT = sis.nonAttack;
            }
            #endregion
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
                //  sis.firstTarget.wp = saveWeak;

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
            #region
            if (e == 1)
            {
                sis.firstAttack = editAT;
            }
            else if (e == 2)
            {
                sis.secondAttack = editAT;
            }
            else if (e == 3)
            {
                sis.thirdAttack = editAT;
            }
            else if (e == 4)
            {
                sis.fourthAttack = editAT;
            }
            else if (e == 5)
            {
                sis.fiveAttack = editAT;
            }
            else if (e == 6)
            {
                sis.nonAttack = editAT;
            }
            #endregion
        }
        //支援条件の時
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //設定
            #region
            if (e == 1)
            {
                editSP = sis.firstPlan;
            }
            else if (e == 2)
            {
                editSP = sis.secondPlan;
            }
            else if (e == 3)
            {
                editSP = sis.thirdPlan;
            }
            else if (e == 4)
            {
                editSP = sis.forthPlan;
            }
            else if (e == 5)
            {
                editSP = sis.fivePlan;
            }
            else
            {
                editSP = sis.sixPlan;
            }
            #endregion
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
                    }
                    else if (numberSave == 6)
                    {
                        editSP.sCondition = SupportCondition.SupportStatus.任意の支援が切れているとき;
                    }

                    else if (numberSave == 7)
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
            //設定
            #region
            if (e == 1)
            {
                sis.firstPlan = editSP;
            }
            else if (e == 2)
            {
                sis.secondPlan = editSP;
            }
            else if (e == 3)
            {
                sis.thirdPlan = editSP;
            }
            else if (e == 4)
            {
                sis.forthPlan = editSP;
            }
            else if (e == 5)
            {
                sis.fivePlan = editSP;
            }
            else
            {
                sis.sixPlan = editSP;
            }
            #endregion
        }
        //支援行動選択

        //回復選択
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //設定
            #region
            if (e == 1)
            {
                editRC = sis.firstRecover;
            }
            else if (e == 2)
            {
                editRC = sis.secondRecover;
            }
            else if (e == 3)
            {
                editRC = sis.thirdRecover;
            }
            else if (e == 4)
            {
                editRC = sis.forthRecover;
            }
            else if (e == 5)
            {
                editRC = sis.fiveRecover;
            }
            else
            {
                editRC = sis.nonRecover;
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
                    }
                    else if (numberSave == 6)
                    {
                        editRC.condition = RecoverCondition.RecoverStatus.任意の支援が切れているとき;
                    }

                    else if (numberSave == 7)
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
            #region
            if (e == 1)
            {
                sis.firstRecover = editRC;
            }
            else if (e == 2)
            {
                sis.secondRecover = editRC;
            }
            else if (e == 3)
            {
                sis.thirdRecover = editRC;
            }
            else if (e == 4)
            {
                sis.forthRecover = editRC;
            }
            else if (e == 5)
            {
                sis.fiveRecover = editRC;
            }
            else
            {
                sis.nonRecover = editRC;
            } 
            #endregion
        }



        MainUI.instance.editParameter = sis;
    }

   //abstract public void ValueSet(ref int valueInt,ref bool valueBool) ;




}
