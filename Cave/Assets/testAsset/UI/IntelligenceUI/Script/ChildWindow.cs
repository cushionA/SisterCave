using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildWindow : MonoBehaviour
{
    /// <summary>
    /// 切り替えるオブジェクト
    /// 値設定の場合要素の制御にも使おう
    /// ドロップダウン系は対象、攻撃選択、支援選択、回復選択、支援条件、回復条件の順
    /// </summary>
    public List<GameObject> objList;

    /// <summary>
    /// マスター窓であるか
    /// </summary>
    [SerializeField]
    bool Master;

    /// <summary>
    /// これがドロップダウンか何か
    /// </summary>
    [SerializeField]
    int type;

    /// <summary>
    /// 説明用の文字配列
    /// </summary>
    [SerializeField]
    string[] introduction;

    //説明用の文字
    [SerializeField]
    Text tx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (type >= 0 && type < 3)
        {
            ResetWindow();
            int i;
            //ドロップダウンが何を参照してるかでUIのドロップダウンのやつを決める
            //objListからドロップダウンを引きずりだす
            //あとオブジェクトリストとSとeでうまいことSetActiveを操る。
            int s = MainUI.instance.settingNumber;
           // Debug.Log($"数値か？{s}");
            if (s == 1)
            {

                objList[0].SetActive(true);
                i = 0;
            }
            //攻撃
            else if (s == 2)
            {
                objList[1].SetActive(true);
                i = 1;
            }
            else if (s == 3)
            {
                i = 4;
                objList[4].SetActive(true);
            }
            else if (s == 4)
            {
                objList[2].SetActive(true);
                i = 2;
            }
            else if (s == 5)
            {
                //これは最初のドロップダウンだけ
                objList[4].SetActive(true);
                i = 4;
            }
            else
            {
                objList[3].SetActive(true);
                i = 3;
            }

            tx.text = introduction[i];
            if (Master)
            {
                objList[i].GetComponent<Selectable>().Select();
            }
        }
        //アプライバリューの前にどのオブジェクトを表示するかを決める
        ApplyValue();
    }
    private void OnDisable()
    {
        if (type >= 0 && type < 3)
        {
            ResetWindow();
            tx.text = null;
        }
    }
    /// <summary>
    /// 設定したり初期化されたりした値をUIに反映
    /// </summary>
    public void ApplyValue()
    {
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;
        //ドロップダウンなら
        //  0以上3以下
        if (type >= 0 && type < 3)
        {
            //ドロップダウンが何を参照してるかでUIのドロップダウンのやつを決める
            //objListからドロップダウンを引きずりだす
            //あとオブジェクトリストとSとeでうまいことSetActiveを操る。
            if (s == 1)
            {
                AttackJudge editor = GetTarget(s, e);
                Dropdown dd = objList[0].GetComponent<Dropdown>();
                //標的
                //ちょっと複雑すぎる
                if (type == 0)
                {
                    //タイプゼロは五個ある


                    if (editor.condition == AttackJudge.TargetJudge.敵タイプ)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーのHPが規定値に達した際)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーのMPが規定値に達した際)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.自分のMPが規定値に達した際)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.強敵の存在)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.状態異常にかかってる敵)
                    {
                        dd.value = 7;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.かかってない支援がある)
                    {
                        dd.value = 8;
                    }
                    else if (editor.condition == AttackJudge.TargetJudge.指定なし)
                    {
                        dd.value = 9;
                    }

                }
                else if (type == 1)
                {
                    if (editor.wp == AttackJudge.WeakPoint.斬撃属性)
                    {
                        dd.value = 0;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.刺突属性)
                    {
                        dd.value = 1;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.打撃属性)
                    {
                        dd.value = 2;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.聖属性)
                    {
                        dd.value = 3;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.闇属性)
                    {
                        dd.value = 4;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.炎属性)
                    {
                        dd.value = 5;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.雷属性)
                    {
                        dd.value = 6;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.毒属性)
                    {
                        dd.value = 7;
                    }
                    else if (editor.wp == AttackJudge.WeakPoint.指定なし)
                    {
                        dd.value = 8;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == AttackJudge.AdditionalJudge.敵のHP)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の距離)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の高度)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の攻撃力)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.敵の防御力)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == AttackJudge.AdditionalJudge.指定なし)
                    {
                        dd.value = 10;
                    }

                }
            }
            //攻撃
            else if (s == 2)
            {
                FireCondition editor = GetAttack(s, e);
                Dropdown dd = objList[1].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (type == 0)
                {
                    if (editor.condition == FireCondition.ActJudge.斬撃属性)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == FireCondition.ActJudge.刺突属性)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == FireCondition.ActJudge.打撃属性)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == FireCondition.ActJudge.聖属性)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == FireCondition.ActJudge.闇属性)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == FireCondition.ActJudge.炎属性)
                    {
                        dd.value = 5;
                    }
                    else if (editor.condition == FireCondition.ActJudge.雷属性)
                    {
                        dd.value = 6;
                    }
                    else if (editor.condition == FireCondition.ActJudge.毒属性)
                    {
                        dd.value = 7;
                    }
                    else if (editor.condition == FireCondition.ActJudge.移動速度低下攻撃)
                    {
                        dd.value = 8;
                    }
                    else if (editor.condition == FireCondition.ActJudge.攻撃力低下攻撃)
                    {
                        dd.value = 9;
                    }
                    else if (editor.condition == FireCondition.ActJudge.防御力低下攻撃)
                    {
                        dd.value = 10;
                    }
                    else if (editor.condition == FireCondition.ActJudge.属性指定なし)
                    {
                        dd.value = 11;
                    }
                    else if (editor.condition == FireCondition.ActJudge.支援行動に移行)
                    {
                        dd.value = 12;
                    }
                    else if (editor.condition == FireCondition.ActJudge.回復行動に移行)
                    {
                        dd.value = 13;
                    }
                    else if (editor.condition == FireCondition.ActJudge.なにもしない)
                    {
                        dd.value = 14;
                    }

                }
                else if(type == 1)
                {
                    if (editor.firstCondition == FireCondition.FirstCondition.敵を吹き飛ばす)
                    {
                        dd.value = 0;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.貫通する)
                    {
                        dd.value = 1;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.設置攻撃)
                    {
                        dd.value = 2;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.範囲攻撃)
                    {
                        dd.value = 3;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.追尾する)
                    {
                        dd.value = 4;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.サーチ攻撃)
                    {
                        dd.value = 5;
                    }
                    else if (editor.firstCondition == FireCondition.FirstCondition.指定なし)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == FireCondition.AdditionalCondition.発射数)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.攻撃力)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.削り値)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == FireCondition.AdditionalCondition.指定なし)
                    {
                        dd.value = 10;
                    }


                }
            }
            else if (s == 3)
            {
                SupportCondition editor = GetSupport(s, e);
                //これは最初のドロップダウンだけ
                Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (type == 0)
                {
                    if (editor.sCondition == SupportCondition.SupportStatus.敵タイプ)
                    {
                        dd.value = 0;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーの体力が規定値の時)
                    {
                        dd.value = 1;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーのMPが規定値に達した際)
                    {
                        dd.value = 2;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.自分のMPが規定値に達した際)
                    {
                        dd.value = 3;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時)
                    {
                        dd.value = 4;
                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.強敵がいるかどうか)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }

                    }
                    else if (editor.sCondition == SupportCondition.SupportStatus.任意の支援が切れているとき)
                    {
                        dd.value = 7;
                    }

                    else if (editor.sCondition == SupportCondition.SupportStatus.指定なし)
                    {
                        dd.value = 8;
                    }

                }
                else if (type == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.攻撃強化)
                    {
                        dd.value = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.防御強化)
                    {
                        dd.value = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.エンチャント)
                    {
                        dd.value = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.アクション強化)
                    {
                        dd.value = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.バリア)
                    {
                        dd.value = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.リジェネ)
                    {
                        dd.value = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.なし)
                    {
                        dd.value = 6;
                    }
                }

            }
            else if (s == 4)
            {
                SupportCondition editor = GetSupport(s, e);
                Dropdown dd = objList[2].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (type == 0)
                {
                    if (editor.ActBase == SupportCondition.MagicJudge.各種支援魔法)
                    {
                        dd.value = 0;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.攻撃ステートに)
                    {
                        dd.value = 1;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.回復ステートに)
                    {
                        dd.value = 2;
                    }
                    else if (editor.ActBase == SupportCondition.MagicJudge.なにもしない)
                    {
                        dd.value = 3;
                    }
                }
                else if (type == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.攻撃強化)
                    {
                        dd.value = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.防御強化)
                    {
                        dd.value = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.エンチャント)
                    {
                        dd.value = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.アクション強化)
                    {
                        dd.value = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.バリア)
                    {
                        dd.value = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.リジェネ)
                    {
                        dd.value = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.なし)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == SupportCondition.AdditionalJudge.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.持続効果時間)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.強化倍率)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == SupportCondition.AdditionalJudge.指定なし)
                    {
                        dd.value = 8;
                    }
                    
                }
            }
            else if (s == 5)
            {
                RecoverCondition editor = GetRecover(s, e);
                //これは最初のドロップダウンだけ
                Dropdown dd = objList[4].GetComponent<Dropdown>();
                if (type == 0)
                {
                    if (editor.condition == RecoverCondition.RecoverStatus.敵タイプ)
                    {
                        dd.value = 0;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーのHPが規定値の時)
                    {
                        dd.value = 1;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーのMPが規定値に達した際)
                    {
                        dd.value = 2;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.自分のMPが規定値に達した際)
                    {
                        dd.value = 3;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時)
                    {
                        dd.value = 4;
                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.強敵がいるかどうか)
                    {
                        if (editor.highOrLow)
                        {
                            dd.value = 5;
                        }
                        else
                        {
                            dd.value = 6;
                        }

                    }
                    else if (editor.condition == RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                    {
                        dd.value = 7;
                    }

                    else if (editor.condition == RecoverCondition.RecoverStatus.指定なし)
                    {
                        dd.value = 8;
                    }

                }
                else if (type == 1)
                {
                    if (editor.needSupport == SisMagic.SupportType.攻撃強化)
                    {
                        dd.value = 0;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.防御強化)
                    {
                        dd.value = 1;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.エンチャント)
                    {
                        dd.value = 2;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.アクション強化)
                    {
                        dd.value = 3;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.バリア)
                    {
                        dd.value = 4;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.リジェネ)
                    {
                        dd.value = 5;
                    }
                    else if (editor.needSupport == SisMagic.SupportType.なし)
                    {
                        dd.value = 6;
                    }
                }
            }
            else
            {
                RecoverCondition editor = GetRecover(s, e);
                Dropdown dd = objList[3].GetComponent<Dropdown>();
                //ちょっと複雑すぎる
                if (type == 0)
                {
                    if (editor.ActBase == RecoverCondition.MagicJudge.治癒魔法)
                    {
                        dd.value = 0;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.攻撃ステートに)
                    {
                        dd.value = 1;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.支援ステートに)
                    {
                        dd.value = 2;
                    }
                    else if (editor.ActBase == RecoverCondition.MagicJudge.なにもしない)
                    {
                        dd.value = 3;
                    }
                }
                else if (type == 1)
                {
                    if (editor.useSupport == SisMagic.SupportType.攻撃強化)
                    {
                        dd.value = 0;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.防御強化)
                    {
                        dd.value = 1;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.エンチャント)
                    {
                        dd.value = 2;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.アクション強化)
                    {
                        dd.value = 3;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.バリア)
                    {
                        dd.value = 4;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.リジェネ)
                    {
                        dd.value = 5;
                    }
                    else if (editor.useSupport == SisMagic.SupportType.なし)
                    {
                        dd.value = 6;
                    }
                }
                else if (type == 2)
                {
                    if (editor.nextCondition == RecoverCondition.AdditionalJudge.詠唱時間)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 0;
                        }
                        else
                        {
                            dd.value = 1;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.持続効果時間)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 2;
                        }
                        else
                        {
                            dd.value = 3;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.MP使用量)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 4;
                        }
                        else
                        {
                            dd.value = 5;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.回復量)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 6;
                        }
                        else
                        {
                            dd.value = 7;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.リジェネ回復量)
                    {
                        if (editor.upDown)
                        {
                            dd.value = 8;
                        }
                        else
                        {
                            dd.value = 9;
                        }
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.状態異常回復)
                    {
                        dd.value = 10;
                    }
                    else if (editor.nextCondition == RecoverCondition.AdditionalJudge.指定なし)
                    {
                        dd.value = 11;
                    }
                }
            }
        }


        //敵タイプ
        else if (type == 3)
        {
            Toggle target;
            //兵士1,飛ぶやつ2,Shooter,Knight4,Trap8,問わず0

            int p = GetType(s, e);
            if (p != 0)
            {
                for (int i = 0; i >= objList.Count; i++)
                {
                    target = objList[i].GetComponent<Toggle>();
                    if (i == 0)
                    {
                        target.isOn = ((p & 0b00000001) == 0b00000001);
                    }
                    else if (i == 1)
                    {
                        target.isOn = ((p & 0b00000010) == 0b00000010);
                    }
                    else if (i == 2)
                    {
                        target.isOn = ((p & 0b00000100) == 0b00000100);
                    }
                    else if (i == 3)
                    {
                        target.isOn = ((p & 0b00001000) == 0b00001000);
                    }
                    else
                    {
                        target.isOn = ((p & 0b00001000) == 0b00010000);
                    }
                }
            }
            else
            {
                for (int i = 0; i >= objList.Count; i++)
                {
                    target = objList[i].GetComponent<Toggle>();
                    target.isOn = false;
                }
            }
        }
        //スライド値設定
        else
        {
            int p = GetType(s, e);
            bool h = GetBool(s, e);
            objList[0].GetComponent<Slider>().value = p;
            objList[1].GetComponent<Toggle>().isOn = h;
        }

    }


    void WakeWindow() 
    {

    }

    void ResetWindow()
    {
        for (int i = 0;i >= objList.Count; i++)
        {
            if (objList[i].activeSelf == true)
                objList[i].SetActive(false);
        }

    }




    /// <summary>
    /// 参照してるタイプを確認
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    int GetType(int s,int e)
    {
        if(s == 1)
        {
            if (e == 1)
            {
               return MainUI.instance.editParameter.firstTarget.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondTarget.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdTarget.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthTarget.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fiveTarget.percentage;
            }
        }
        else if (s == 3)
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstPlan.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondPlan.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdPlan.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthPlan.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fivePlan.percentage;
            }
        }
        else
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstRecover.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondRecover.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdRecover.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthRecover.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fiveRecover.percentage;
            }
        }
    }

    /// <summary>
    /// 参照してるタイプを確認
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    bool GetBool(int s, int e)
    {
        if (s == 1)
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstTarget.highOrLow;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondTarget.highOrLow;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdTarget.highOrLow;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthTarget.highOrLow;
            }
            else
            {
                return MainUI.instance.editParameter.fiveTarget.highOrLow;
            }
        }
        else if (s == 3)
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstPlan.highOrLow;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondPlan.highOrLow;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdPlan.highOrLow;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthPlan.highOrLow;
            }
            else
            {
                return MainUI.instance.editParameter.fivePlan.highOrLow;
            }
        }
        else
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstRecover.highOrLow;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondRecover.highOrLow;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdRecover.highOrLow;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthRecover.highOrLow;
            }
            else
            {
                return MainUI.instance.editParameter.fiveRecover.highOrLow;
            }
        }
    }

    AttackJudge GetTarget(int s,int e)
    {
        if (e == 1)
        {
            return MainUI.instance.editParameter.firstTarget;
        }
        else if (e == 2)
        {
            return MainUI.instance.editParameter.secondTarget;
        }
        else if (e == 3)
        {
            return MainUI.instance.editParameter.thirdTarget;
        }
        else if (e == 4)
        {
            return MainUI.instance.editParameter.forthTarget;
        }
        else
        {
            return MainUI.instance.editParameter.fiveTarget;
        }
    }
    FireCondition GetAttack(int s, int e)
    {
        if (e == 1)
        {
            return MainUI.instance.editParameter.firstAttack;
        }
        else if (e == 2)
        {
            return MainUI.instance.editParameter.secondAttack;
        }
        else if (e == 3)
        {
            return MainUI.instance.editParameter.thirdAttack;
        }
        else if (e == 4)
        {
            return MainUI.instance.editParameter.fourthAttack;
        }
        else if (e == 5)
        {
            return MainUI.instance.editParameter.fiveAttack;
        }
        else
        {
            return MainUI.instance.editParameter.nonAttack;
        }
    }

    SupportCondition GetSupport(int s, int e)
    {
        if (e == 1)
        {
            return MainUI.instance.editParameter.firstPlan;
        }
        else if (e == 2)
        {
            return MainUI.instance.editParameter.secondPlan;
        }
        else if (e == 3)
        {
            return MainUI.instance.editParameter.thirdPlan;
        }
        else if (e == 4)
        {
            return MainUI.instance.editParameter.forthPlan;
        }
        else if (e == 5)
        {
            return MainUI.instance.editParameter.fivePlan;
        }
        else
        {
            return MainUI.instance.editParameter.sixPlan;
        }
    }
    RecoverCondition GetRecover(int s, int e)
    {
        if (e == 1)
        {
            return MainUI.instance.editParameter.firstRecover;
        }
        else if (e == 2)
        {
            return MainUI.instance.editParameter.secondRecover;
        }
        else if (e == 3)
        {
            return MainUI.instance.editParameter.thirdRecover;
        }
        else if (e == 4)
        {
            return MainUI.instance.editParameter.forthRecover;
        }
        else if (e == 5)
        {
            return MainUI.instance.editParameter.fiveRecover;
        }
        else
        {
            return MainUI.instance.editParameter.nonRecover;
        }
    }
}
