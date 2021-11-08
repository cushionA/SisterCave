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
    /// マスターというか、上下の起点になるかどうか
    /// </summary>
    [SerializeField] bool isMaster;
    /// <summary>
    /// 同時に設定するオブジェクトを持つ。下を設定
    /// </summary>
    [SerializeField] List<Navigation> raidObject;



    /// <summary>
    /// 初期設定に戻すのに使う
    /// </summary>
    Navigation mine;
    Selectable me;


    /// <summary>
    /// 呼び出される際に初期化される番号
    /// この数値にあうUIがあてがわれる。
    /// </summary>
    public int UINumber;

    /// <summary>
    /// セットリストに設定したかどうか
    /// </summary>
    bool done;

  //  public bool isChange;

    // Start is called before the first frame update
    void Start()
    {
        
       // mine = GetComponent<Navigation>();
        me = GetComponent<Selectable>();
        mine = me.navigation;
        ChangeItem();
        UpSet();
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
        {
            done = false;
            MainUI.instance.selectList.Add(me);
        }

    }

    public void UnderSet(Selectable raid)
    {
        Navigation navi = me.navigation;
        navi.selectOnDown = raid;
        me.navigation = navi;
        if(raidObject.Count > 0)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i >= raidObject.Count; i++)
            {
                 navi = raidObject[i];
                navi.selectOnDown = raid;
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
        if (mine.selectOnUp == null)
        {
            if (MainUI.instance.selectList.Count > 0)
            {
                Navigation navi = me.navigation;
                navi.selectOnUp = MainUI.instance.selectList[MainUI.instance.selectList.Count - 1];
                me.navigation = navi;
                if (isMaster)
                {
                    MainUI.instance.selectList[MainUI.instance.selectList.Count - 1].gameObject.GetComponent<SelectControll>().UnderSet(me);
                    
                }
            }
            else
            {
                if (isMaster)
                {
                    MainUI.instance.selectList.Add(me);
                }
            }
        }
        else if(isMaster)
        {
            done = true;
        }
    }

    public void SelectReset(Selectable change)
    {

        MainUI.instance.changeTarget = me;
         me.navigation = mine;
    }

    public void ChangeItem()
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
    }

}
