using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDrop : MonoBehaviour
{
    //二個目の窓
    public GameObject secondDrop;
    //三個目の窓
    public GameObject thirdDrop;
    //値設定
    public GameObject valueWindow;

    //こいつらから子オブジェクトを伝って設定する。

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 窓を変えた時並べなおす処理
    /// </summary>
    public void ChangeWindow()
    {

        //Settingナンバーで処理変える。
        //Settingナンバー1なら値設定の窓をスキップか違うものに変えるか。セカンドドロップ以降を動かすのと、値設定窓の切り替えと配置。
        //Selectableの選び直しを。（強敵、状態異常、支援、なしの時はない）
        //Settingナンバー3、5なら値設定の窓をスキップか違うものに変えるか。窓は一つだけなので窓を切り替えるのとSelectableの選びなおし
        //preにもnextにも最初のドロップダウンが入る
        //Settingナンバー2、4、6ならセカンドドロップ以降を消すだけ。値設定窓の切り替えと配置
        //Selectableの再設定はなし

        //プロセスとしてはどれでもまず一旦全部消す。
        //消した後必要な窓を出して並び変える。

        //ローカル変数でうまくやって

    }

    /// <summary>
    /// 最初に出現した時のメソッド
    /// ドロップダウンの値を決めて、ValueChangeを出したりする。
    /// あとすべてのドロップダウンや窓に最初に出すときに自分の値をちゃんと入れるようなのがあった方がよくね？
    /// if(hoge = 1)みたいなので順繰りにさ
    /// 窓を出そう
    /// </summary>
    public void Initialize()
    {
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;

        //まず設定番号に基づき回復とか攻撃とかを分ける

        //セットオブジェクトが真のオブジェクトのChildWindowでUI初期化させる？



    }
    /// <summary>
    /// 最初のドロップダウンを終了させる。
    /// バックスペースで行く。番号をもとに戻す。
    /// </summary>
    public void UIEnd()
    {
        //   MainUI.instance.selectList = new List<Selectable>();
        if (MainUI.instance.settingNumber % 2 == 1)
        {
            MainUI.instance.settingNumber--;
        }
        this.gameObject.SetActive(false);

    }
}
