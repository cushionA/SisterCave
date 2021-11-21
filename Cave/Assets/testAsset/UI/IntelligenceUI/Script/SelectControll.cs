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


  //  [SerializeField] bool isUpSet;

    /// <summary>
    /// Falseなら下
    /// </summary>
    [SerializeField] bool isUp;
    /// <summary>
    /// 同時に設定するオブジェクトを持つ。下を設定
    /// </summary>
    [SerializeField] List<Navigation> raidObject;



    /// <summary>
    /// 初期設定に戻すのに使う
    /// </summary>
   // Navigation mine;
    Selectable me;






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
            if (MainUI.instance.isChange == 1)
            {
                UpSet();
                MainUI.instance.isChange = 2;
            }
            else
            {
                UnderSet();
                MainUI.instance.isChange = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnderSet()
    {
        Navigation navi = me.navigation;
        navi.selectOnDown = MainUI.instance.nextObject;
        me.navigation = navi;
        navi = MainUI.instance.nextObject.navigation;
        navi.selectOnUp = me;
        MainUI.instance.nextObject.navigation = navi;

        if (raidObject.Count > 0)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i >= raidObject.Count; i++)
            {
                 navi = raidObject[i];
                navi.selectOnDown = MainUI.instance.nextObject;
                raidObject[i] = navi;
            }
        }
    }
    /// <summary>
    /// 上のオブジェクト設定。
    /// </summary>
    /// <param name="raid"></param>
    public void UpSet()
    {
        Navigation navi = me.navigation;
        navi.selectOnUp = MainUI.instance.preObject;
        me.navigation = navi;
        navi = MainUI.instance.preObject.navigation;
        navi.selectOnDown = me;
        MainUI.instance.preObject.navigation = navi;

        if (raidObject.Count > 0)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i >= raidObject.Count; i++)
            {
                navi = raidObject[i];
                navi.selectOnUp = MainUI.instance.preObject;
                raidObject[i] = navi;
            }
        }
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
