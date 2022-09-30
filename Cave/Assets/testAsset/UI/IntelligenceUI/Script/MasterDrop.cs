using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
public class MasterDrop : MonoBehaviour
{


    public Transform[] value;


    //こいつらから子オブジェクトを伝って設定する。

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

            if (!MainUICon.instance.editNow && MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
            {
            int i = MainUICon.instance.isAH ? 3 : 2;

         //   Debug.Log($"いつも{MainUICon.instance.editNow}");
                UIEnd();
                value[i].gameObject.MMGetComponentNoAlloc<SettingWindowCon>().CancelWindow();
            }


    }

    private void OnEnable()
    {
        ChangeWindow();
    }

    /// <summary>
    /// 窓を変えた時並べなおす処理
    /// 最初も呼ぶ
    /// </summary>
    public void ChangeWindow()
    {
        //こいつボタン押すたびに呼ぼう
        //S＝とかで色々決めよう

        //こいつはvalue窓の切り替えとUIPlaceSetの呼び出しとSetActiveだけ

        if (MainUICon.instance.valueWindow != null)
        {
            MainUICon.instance.valueWindow.SetActive(false);
        }
        if(MainUICon.instance.secondDrop != null)
        MainUICon.instance.secondDrop.SetActive(false);

        //窓の切り替え
        WindowJudge();

        //Settingナンバーで処理変える。
        //Settingナンバー1なら値設定の窓をスキップか違うものに変えるか。セカンドドロップ以降を動かすのと、値設定窓の切り替えと配置。
        //Selectableの選び直しを。（強敵、状態異常、支援、なしの時はない）
        //Settingナンバー3、5なら値設定の窓をスキップか違うものに変えるか。窓は一つだけなので窓を切り替えるのとSelectableの選びなおし
        
        //preにもnextにも最初のドロップダウンが入る
        //Settingナンバー2、4、6ならセカンドドロップ以降を消すだけ。値設定窓の切り替えと配置
        //Selectableの再設定はなし






        //並び替えのために前のオブジェクトを改変
      //  MainUICon.instance.beforeSet = this.gameObject;
    }


    /// <summary>
    /// 最初のドロップダウンを終了させる。
    /// バックスペースで行く。番号をもとに戻す。
    /// </summary>
    public void UIEnd()
    {
        //   MainUICon.instance.selectList = new List<Selectable>();

        MainUICon.instance.beforeSet = null;
        this.gameObject.SetActive(false);
        if (MainUICon.instance.valueWindow != null)
        {
            MainUICon.instance.valueWindow.SetActive(false);
        }
            MainUICon.instance.secondDrop.SetActive(false);

    }


    void WindowJudge()
    {


        //攻撃条件
        //ファーストドロップ（基礎条件）→値設定（強敵、状態異常、支援、なしの時はない）→セカンドドロップ（弱点）→サードドロップ（標的条件）
        //攻撃選択パターン
        //ファーストドロップ（選択行動、属性）（なにもしないと移行で後の窓が消える）→セカンドドロップ（使用する攻撃の条件設定）→サードドロップ（さらに条件）
        //支援条件パターン
        //ファーストドロップ（基礎条件）→値設定（強敵、なしの時はない）
        //支援選択パターン
        //ファーストドロップ（行動選択）（なにもしないと移行で後の窓が消える）→セカンドドロップ（どんなサポートがついたものを選ぶか）→サードドロップ（どんな条件で魔法選ぶか）

        SisterParameter sis = MainUICon.instance.editParameter;
        int s = MainUICon.instance.settingNumber;
        int e = MainUICon.instance.editNumber;
        Selectable sl;
        Selectable sl2;
        int num;
        bool valueCheck = false;

        #region
        if (s == 1)
        {
            num = 0;
        }
        else if (s == 2)
        {
            num = 1;
        }
        else if (s == 3 || s == 5)
        {
            num = 4;
        }
        else if (s == 4)
        {
            num = 2;
        }
        else
        {
            num = 3;
        }
        #endregion
        sl = this.gameObject.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();
        sl2 = MainUICon.instance.secondDrop.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();

        //上下の選択初期化、失敗してないか注意
        UnderSet(sl, null);
        UpSet(sl2, null);
        //まずセレクト初期化
        MainUICon.instance.beforeSet = null;
        this.gameObject.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
        if (s == 1)
        {
            AttackJudge editJudge = new AttackJudge();
            //設定
            #region
            editJudge = sis.targetCondition[e];
            #endregion

            if (editJudge.condition == AttackJudge.TargetJudge.プレイヤーが状態異常にかかった時 ||
               editJudge.condition == AttackJudge.TargetJudge.かかってない支援がある ||
               editJudge.condition == AttackJudge.TargetJudge.強敵の存在 ||
               editJudge.condition == AttackJudge.TargetJudge.指定なし ||
               editJudge.condition == AttackJudge.TargetJudge.状態異常にかかってる敵)
            {
                if(MainUICon.instance.valueWindow != null)
                {
                    MainUICon.instance.valueWindow.SetActive(false);
                }
            }
            else
            {
                if(editJudge.condition == AttackJudge.TargetJudge.敵タイプ)
                {
                    MainUICon.instance.valueWindow = value[0].gameObject;
                }
                else
                {
                    MainUICon.instance.valueWindow = value[1].gameObject;
                  //  MainUICon.instance.valueWindow
                }
                MainUICon.instance.isChange = 1;
                MainUICon.instance.valueWindow.SetActive(true);
                valueCheck = true;
                MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
                MainUICon.instance.secondDrop.SetActive(true);
                MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
        }
        else if(s == 2)
        {
            //MainUICon.instance.valueWindow.SetActive(false);
            FireCondition editAT;
            //内容割り当て


            editAT = sis.AttackCondition[e];

            if (editAT.condition == FireCondition.ActJudge.なにもしない || editAT.condition == FireCondition.ActJudge.回復行動に移行 ||
                editAT.condition == FireCondition.ActJudge.支援行動に移行)
            {
                MainUICon.instance.secondDrop.SetActive(false);
               // thirdDrop.SetActive(false);
            }
            else
            {
                MainUICon.instance.secondDrop.SetActive(true);
                MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
        }
        else if (s == 3 || s == 4)
        {
            SupportCondition editSP;
            //設定

            editSP = sis.supportPlan[e];

            if (s == 3)
            {
                if(editSP.sCondition == SupportCondition.SupportStatus.強敵がいるかどうか || editSP.sCondition == SupportCondition.SupportStatus.指定なし || editSP.sCondition == SupportCondition.SupportStatus.プレイヤーが状態異常にかかった時
                    || editSP.sCondition == SupportCondition.SupportStatus.任意の支援が切れているとき)
                {
                    if (MainUICon.instance.valueWindow != null)
                    {
                        MainUICon.instance.valueWindow.SetActive(false);
                    }
                    if (editSP.sCondition == SupportCondition.SupportStatus.任意の支援が切れているとき)
                    {
                        MainUICon.instance.secondDrop.SetActive(true);
                        MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                    }
                }
                else
                {
                    if (editSP.sCondition == SupportCondition.SupportStatus.敵タイプ)
                    {
                        MainUICon.instance.valueWindow = value[0].gameObject;
                    }
                    else
                    {
                        MainUICon.instance.valueWindow = value[1].gameObject;

                    }

                    MainUICon.instance.valueWindow.SetActive(true);
                    MainUICon.instance.isChange = 1;
                    valueCheck = true;
                    MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                }
            }
            else
            {
                if (editSP.ActBase != SupportCondition.MagicJudge.各種支援魔法)
                {
                    MainUICon.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUICon.instance.secondDrop.SetActive(true);
                    MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
            }

        }
        else if (s == 5 || s == 6)
        {
            RecoverCondition editRC;
            //設定
            if (MainUICon.instance.isAH)
            {
                editRC = sis.nRecoverCondition[e];
            }
            else
            {
                editRC = sis.recoverCondition[e];
            }
            if (s == 5)
            {
                if(editRC.condition == RecoverCondition.RecoverStatus.強敵がいるかどうか || editRC.condition == RecoverCondition.RecoverStatus.指定なし ||
                    editRC.condition == RecoverCondition.RecoverStatus.プレイヤーが状態異常にかかった時 || editRC.condition == RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                {
                    if (MainUICon.instance.valueWindow != null)
                    {
                        MainUICon.instance.valueWindow.SetActive(false);
                    }
                    if (editRC.condition == RecoverCondition.RecoverStatus.任意の支援が切れているとき)
                    {
                        MainUICon.instance.secondDrop.SetActive(true);
                        MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();

                    }
                }
                else
                {
                    if(editRC.condition == RecoverCondition.RecoverStatus.敵タイプ)
                    {
                        MainUICon.instance.valueWindow = value[0].gameObject;

                    }
                    else
                    {
                        MainUICon.instance.valueWindow = value[1].gameObject;

                    }
                        MainUICon.instance.valueWindow.SetActive(true);
                        MainUICon.instance.isChange = 1;
                        valueCheck = true;
                        MainUICon.instance.valueWindow.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
            }
            }
            else
            {
                if(editRC.ActBase != RecoverCondition.MagicJudge.治癒魔法)
                {
                    MainUICon.instance.secondDrop.SetActive(false);
                }
                else
                {
                    MainUICon.instance.secondDrop.SetActive(true);
                    MainUICon.instance.secondDrop.MMGetComponentNoAlloc<UIPlaceSet>().PlaceSet();
                }
            }
        }

        if (!valueCheck && MainUICon.instance.secondDrop.activeSelf)
        {
            sl = this.gameObject.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();
            sl2 = MainUICon.instance.secondDrop.MMGetComponentNoAlloc<ChildWindow>().objList[num].MMGetComponentNoAlloc<Selectable>();

            //値窓なくて二番目ある時二番目と一番目の上下をつなげる。
            UnderSet(sl, sl2);
            UpSet(sl2, sl);
        }
    }


    public void UnderSet(Selectable me ,Selectable nextObject)
    {
        if (nextObject != null)
        {
            Navigation navi = me.navigation;
            navi.selectOnDown = nextObject;
            me.navigation = navi;
            navi = nextObject.navigation;
            navi.selectOnUp = me;
            nextObject.navigation = navi;
        }
        else
        {
            Navigation navi = me.navigation;
            navi.selectOnDown = null;
            me.navigation = navi;
        }
    }
    public void UpSet(Selectable me ,Selectable preObject)
    {
        if (preObject != null)
        {
            Navigation navi = me.navigation;
            navi.selectOnUp = preObject;
            me.navigation = navi;
            navi = preObject.navigation;
            navi.selectOnDown = me;
            preObject.navigation = navi;
        }
        else
        {
            Navigation navi = me.navigation;
            navi.selectOnUp = null;
            me.navigation = navi;
        }
    }





}
