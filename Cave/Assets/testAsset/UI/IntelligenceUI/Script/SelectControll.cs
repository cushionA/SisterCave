using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectControll : MonoBehaviour
{

    //必要なチェックボックス
    //属性８、敵タイプ４，bool系、多いか少ないかの一つだけ。boolけいはドロップダウンにする。
    //必要なサポートはドロップダウンにする

    //となると設定ウィンドウのタイプはドロップダウン一つと、数値とbool、長いチェックボックスと短いチェックボックスの四種類になる。
    //こいつらを差別化するのに必要なのは参照先と何を設定するかのタイトルと、

    //自分に番号をつけて番号に合うリストに自分を入れる。Selectableでそれを参照する。
    //上と下、どちらも設定するかしないかを選べる。
    //各Selectableな要素に入れる。ボタンとかトグルとか



    //処理窓、変更窓につく機能
    //参照先の型を渡したらその要素をメソッドの引数にしてすうちをかえる？Hoge(UI3.percentage);みたいな
    //単にintとboolを引数にして、セッッティングナンバーで型三つ分のメソッドの差分を設ける
    //この型で何番目の条件を編集してるので、この数値を変えますとすればよくね？
    //ほんなら引数いらないやん！！


    //整理しよう。
    //まずこれを使うのは攻撃条件の下、支援回復条件の上下、タイプとスライダーの上下になる。
    //preとかネクストとか消してChildWindowにUseナンバーをつけて何番のオブジェクトリストを利用してるかを得るか
    //オブジェクトを得る方法とSとかで
    //てかUseナンバーなくてもSとかの時セカンドウィンドウとかで何使われてるかわかるよね
    //
    //sが1のファーストウィンドウの下とタイプとスライダーの上下、セカンドは接続対象としてだけ存在。
    //支援回復条件の時、ファーストウィンドウの下とタイプとスライダーの上下、スライダーやタイプの下はファーストになる
    //
    //これを利用してコード書こう
    //考慮すべきはS＝1,3,5だけ
    //

    //  [SerializeField] bool isUpSet;


    /// <summary>
    /// 同時に設定するオブジェクトを持つ。下を設定
    /// </summary>
    [SerializeField] List<Selectable> raidObject;



    /// <summary>
    /// 初期設定に戻すのに使う
    /// </summary>
   // Navigation mine;
    Selectable me;

    /// <summary>
    /// value窓用
    /// </summary>
     [SerializeField]
    bool isValue;




  //  public bool isChange;

    // Start is called before the first frame update
    void Start()
    {
        
       // mine = GetComponent<Navigation>();
        me = GetComponent<Selectable>();
   //     mine = me.navigation;
        //ChangeItem();

    }

    // Update is called once per frame
    void Update()
    {
        /*     if (done)
             {
                 done = false;
                 MainUI.instance.selectList.Add(me);
             }*/
        //基本的に値設定窓は一つなのでisChangeが他にいじられない

        if (MainUI.instance.isChange > 0) 
        {
            int s = MainUI.instance.settingNumber;
            //int e = MainUI.instance.editNumber;
            Selectable sl;
            int num;

            #region
            if (s == 1)
            {
                num = 0;
            }
            else if(s == 2)
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

            if (MainUI.instance.isChange == 1)
            {
                 sl = MainUI.instance.firstDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    UpSet(sl);
               MainUI.instance.isChange = 2;
            }
            else
            {
                    if (s == 1)
                    {
                        sl = MainUI.instance.secondDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    }
                    else
                    {
                        sl = MainUI.instance.firstDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    }
                if (!isValue)
                {
                        UnderSet(sl);
                }
                else
                {
                    AnotherSet(sl);
                }

                MainUI.instance.isChange = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnderSet(Selectable nextObject)
    {
        Navigation navi = me.navigation;
        navi.selectOnDown = nextObject;
        me.navigation = navi;
        navi = nextObject.navigation;
        navi.selectOnUp = me;
        nextObject.navigation = navi;

        if (raidObject.Count > 0 && !isValue)
        {
        //    Debug.Log("あああ");
            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i <= raidObject.Count - 1; i++)
            {
                 navi = raidObject[i].navigation;
                navi.selectOnDown = nextObject;
                raidObject[i].navigation = navi;
            }
        }
    }
    /// <summary>
    /// 上のオブジェクト設定。
    /// </summary>
    /// <param name="raid"></param>
    public void UpSet(Selectable preObject)
    {
        Navigation navi = me.navigation;
        navi.selectOnUp = preObject;
        me.navigation = navi;
        navi = preObject.navigation;
        navi.selectOnDown = me;
        preObject.navigation = navi;

        if (raidObject.Count > 0 && !isValue)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i <= raidObject.Count - 1; i++)
            {
                navi = raidObject[i].navigation;
                navi.selectOnUp = preObject;
                raidObject[i].navigation = navi;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void AnotherSet(Selectable nextObject)
    {
        

        Navigation navi = raidObject[0].navigation;
        navi.selectOnDown = nextObject;
        raidObject[0].navigation = navi;
        navi = nextObject.navigation;
        navi.selectOnUp = raidObject[0];
        nextObject.navigation = navi;
    }

    /*   public void SelectReset(Selectable change)
       {

           MainUI.instance.changeTarget = me;
            me.navigation = mine;
       }*/

    /*  public void ChangeItem()
      {
          if (MainUI.instance.isChange && isMaster && mine.selectOnUp == null)
          {
              for (int i = 0; i >= MainUI.instance.selectList.Count; i++)
              {
                  //新しいやつと入れ替え。
                  if (MainUI.instance.selectList[i] == MainUI.instance.changeTarget)
                  {
                      MainUI.instance.selectList[i] = me;
                      i = 100;
                  }

              }
              MainUI.instance.isChange = false;
          }
      }*/

}
