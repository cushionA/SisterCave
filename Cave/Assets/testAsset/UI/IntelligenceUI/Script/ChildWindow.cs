using MoreMountains.Tools;
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
        //aまだバグある
        

        if (type >= 0 && type < 3)
        {
            ResetWindow();
            int i;
            //ドロップダウンが何を参照してるかでUIのドロップダウンのやつを決める
            //objListからドロップダウンを引きずりだす
            //あとオブジェクトリストとSとeでうまいことSetActiveを操る。
            int s = MainUICon.instance.settingNumber;
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
                if(type == 1)
                {
                    objList[5].SetActive(true);
                }
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
                if (type == 1)
                {
                    objList[5].SetActive(true);
                }
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
                if (type == 1)
                {
                    objList[5].SetActive(true);
                }
                i = 3;
            }
             objList[i].MMGetComponentNoAlloc<ValueChangeBase>().NumberSet();
            tx.text = introduction[i];
            if (Master)
            {
                objList[i].MMGetComponentNoAlloc<Selectable>().Select();
                
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
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        //ドロップダウンなら
        //  0以上3以下
        if (type >= 0 && type < 3)
        {
            //ドロップダウンが何を参照してるかでUIのドロップダウンのやつを決める
            //objListからドロップダウンを引きずりだす
            //あとオブジェクトリストとSとeでうまいことSetActiveを操る。
            if (s == 1)
            {
                AttackJudge editor = GetTarget(e);
                Dropdown dd = objList[0].MMGetComponentNoAlloc<Dropdown>();
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
                FireCondition editor = GetAttack(e);
                Dropdown dd = objList[1].MMGetComponentNoAlloc<Dropdown>();
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
                SupportCondition editor = GetSupport(e);
                //これは最初のドロップダウンだけ
                Dropdown dd = objList[4].MMGetComponentNoAlloc<Dropdown>();
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
                SupportCondition editor = GetSupport(e);
                Dropdown dd = objList[2].MMGetComponentNoAlloc<Dropdown>();
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
                RecoverCondition editor = GetRecover(e);
                //これは最初のドロップダウンだけ
                Dropdown dd = objList[4].MMGetComponentNoAlloc<Dropdown>();
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
                RecoverCondition editor = GetRecover(e);
                Dropdown dd = objList[3].MMGetComponentNoAlloc<Dropdown>();
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
             //   Debug.Log("wee");
                for (int i = 0; i <= objList.Count - 1; i++)
                {
                    target = objList[i].MMGetComponentNoAlloc<Toggle>();
                    if (i == 0)
                    {
                     //   Debug.Log("ssssあｓ");
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
                        target.isOn = ((p & 0b00010000) == 0b00010000);
                    }
                }
            }
            else
            {
                Debug.Log("aee");
                for (int i = 0; i <= objList.Count- 1; i++)
                {
                    target = objList[i].MMGetComponentNoAlloc<Toggle>();
                    target.isOn = false;
                }
            }
        }
        //スライド値設定
        else
        {
            int p = GetType(s, e);
            bool h = GetBool(s, e);
            objList[0].MMGetComponentNoAlloc<Slider>().value = p;
            objList[1].MMGetComponentNoAlloc<Toggle>().isOn = h;
        }

    }



    void ResetWindow()
    {
        for (int i = 0;i <= objList.Count - 1; i++)
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
            return MainUICon.instance.editParameter.targetCondition[e].percentage;
        }
        else if (s == 3)
        {
            return MainUICon.instance.editParameter.supportPlan[e].percentage;
        }
        else
        {
            return MainUICon.instance.editParameter.recoverCondition[e].percentage;
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
            return MainUICon.instance.editParameter.targetCondition[e].highOrLow;
        }
        else if (s == 3)
        {
            return MainUICon.instance.editParameter.supportPlan[e].highOrLow;
        }
        else
        {
            return MainUICon.instance.editParameter.recoverCondition[e].highOrLow;
        }
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
    RecoverCondition GetRecover( int e)
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
