using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaceSet : MonoBehaviour
{

    //最初のドロップダウンに設定番号で
    //UIをシリアライズする機能も持つ。
    //UIが最終要素じゃなければドロップダウンと共に出てくる
    //よく考えたらシリアライズさせるのはドロップダウンだけの仕事だな

    //シリアライズの処理は接続するオブジェクトをシリアライズしたやつに渡すこと
    //さらにドロップダウンの条件に応じてどの窓を出すか
    //位置設定は呼び出された側が持ってる数値で自分でやります。
    //ドロップダウンの位置設定はドロップダウンが前の条件を見てウィンドウが何かを判断して自分がやります。
    //セッティングがついてるドロップダウンをもう一つ作る
    //メインのドロップダウンかどうかを設定する
    //このスクリプトはUI要素のルートオブジェクトに置く
    //これ例えば三番目で二番目の要素が変わってレイアウト変わった時のことも考えなきゃじゃん

    //バックスペースで戻る
    //設定コードと継続コードを二種類書く
    //継続コードでは変更の際改めて別のUI出したりする。
    //設定ではただ設定するだけ
    //ちなみに継続コードにも設定することあるよな？
    //統一するためにrefで元を変えるか
    //ちなみに継続コードからSelectConのChangeを呼ぶ
    //継続コードでセッティング番号と現在のUI数とかからさらに継続ドロップダウンを出すかとか決める。
    //上の判断に基づいて表示する文字とかも決める
    //設定するオブジェクトに渡す文字列とかも
    //決める対象って大体固定だからRef使って大本かえたらいいんじゃね
    //セッティング番号で上手いことやれるでしょ
    //あと表示してるUIのルートオブジェクトまとめリストとか、セッティング番号とかに基づく判断を保存できる場所は必要だな

    /// <summary>
    /// 前の要素（たいていドロップダウンとか決まってる）からちょうどいい位置に配置するための数値
    /// </summary>
    public float addjustPosition;

    /// <summary>
    /// 窓のタイプ
    /// 始動用ドロップダウンか、それとも敵タイプ設定する窓なのかとか
    /// 自分が継続用ドロップダウンである場合は前の窓の（SelectListからUIPlaceSetで取得）タイプにより調整距離を変える
    /// 0は開始ドロップダウン、１は継続用ドロップダウン、2は設定用ドロップダウン、3は数値とBool（ハイロー）、4は長いチェックボックス、5は短いチェックボックス
    /// 
    /// </summary>
    public int windowType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlaceSet()
    {
        if(windowType == 0)
        {

        }
        else if (windowType == 1)
        {
            //SelectListにAddされるのはUpdateから

            int number = MainUI.instance.selectList[MainUI.instance.selectList.Count - 1].gameObject.GetComponent<UIPlaceSet>().windowType;

            if(number == 2)
            {
                addjustPosition = 0;
            }
            else if (number == 3)
            {
                addjustPosition = 0;
            }
            else if (number == 4)
            {
                addjustPosition = 0;
            }
            else if (number == 5)
            {
                addjustPosition = 0;
            }

            //ここに前のUIの位置からaddjustをりようしてやる

        }
    }

    /// <summary>
    /// 最初のドロップダウンを終了させる。
    /// バックスペースで行くか
    /// </summary>
    public void UIEnd()
    {
        MainUI.instance.selectList = new List<Selectable>();
        if(MainUI.instance.settingNumber % 2 == 1)
        {
            MainUI.instance.settingNumber--;
        }
        this.gameObject.SetActive(false);
    }

}
