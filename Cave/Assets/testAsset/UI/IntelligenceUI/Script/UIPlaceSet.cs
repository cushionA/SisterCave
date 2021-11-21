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
    /// </summary>
    public int windowType;

    //基本的に各スクリプトはsaveNumberを改変保存する。

    //セッッティングナンバーは六種類
    //攻撃条件設定は４フェーズ、一個目がドロップダウン（確定）、二個目は色々（敵タイプ窓、数値設定、何もなしでスキップ）、三個目は弱点ドロップダウン
    //、四個目はドロップダウン（UpDownあり）。肯定側は全部真
    //一個目のドロップダウンで残りの窓をすべて出す。指定なし以外
    //攻撃選択は3フェーズ、全部ドロップダウン。最後のドロップダウンはUpDownあり。一個目のドロップダウンで残りの窓をすべて出す。指定なしと移行以外
    //支援条件は2フェーズ、一個目ドロップダウン、二個目は数値設定か敵チェックか必要な支援を選んだりを決めるドロップダウンか
    //一個目のドロップダウンで残りの窓を出す。指定なしの時以外
    //支援選択は３フェーズ、一個目ドロップダウン、二個目ドロップダウン、三個目もドロップダウン（UpDownあり）。指定なし以外残りが出てくる。
    //回復条件は2フェーズ、一個目ドロップダウン、二個目は数値設定か敵チェックか必要な支援を選んだり強敵がいるかを決めるドロップダウンか
    //回復選択は３フェーズ、一個目ドロップダウン、二個目ドロップダウン、三個目もドロップダウン（UpDownあり）


    //ファーストドロップ、セカンドドロップなどのドロップダウンの切り替えはセットナンバーで行う。

    //攻撃条件
    //ファーストドロップ（基礎条件）→値設定（強敵、状態異常、支援、なしの時はない）→セカンドドロップ（弱点）→サードドロップ（標的条件）
    //攻撃選択パターン
    //ファーストドロップ（選択行動、属性）（なにもしないと移行で後の窓が消える）→セカンドドロップ（使用する攻撃の条件設定）→サードドロップ（さらに条件）
    //支援条件パターン
    //ファーストドロップ（基礎条件）→値設定（強敵、なしの時はない）
    //支援選択パターン
    //ファーストドロップ（行動選択）（なにもしないと移行で後の窓が消える）→セカンドドロップ（どんなサポートがついたものを選ぶか）→サードドロップ（どんな条件で魔法選ぶか）

    //選択制御スクリプトに必要な機能
    //セッッティングナンバー見てドロップダウンを有効無効を切り替える
    //魔法選択はすでに軒並み決定してる。
    //さらに魔法選択でのレイアウト変更はウィンドウの消去回復だけ
    //厄介なのは各ステートの条件設定
    //値設定の前後だけSelectableを入れなおす、さらにレイアウトの変更もある。
    //でもそれも条件判断で切り替えられそうなんだよなぁ
    //出てくる窓は最大四つ。配置換えの時点で以下全部配置しなおそ



    //いろいろ入り混じるのは三か所だけ（攻撃条件の２、支援回復条件の三個目）

    //全て消すときはEditNumで戻ればいい。
    //ドロップダウンは最大三つ必要

    // Start is called before the first frame update

    RectTransform myPosi;

    void Start()
    {
        myPosi = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PlaceSet();
    }
    public void PlaceSet()
    {
        if(windowType == 0)
        {
            //ドロップダウン
            if(MainUI.instance.beforeSet == null)
            {
                //既定の場所にセット
              //  RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
                Vector2 posi = new Vector2(425, 300);
                myPosi.anchoredPosition = posi;
            }
            else
            {
                UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
                RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();

                if (before.windowType == 0)
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 125);
                    myPosi.anchoredPosition = posi;
                    //規定値を前のオブジェクトの位置に足した場所に入れる
                    //125下がった位置に入れる
                }
                else if (before.windowType == 1)
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 130);
                    myPosi.anchoredPosition = posi;
                    //規定値を前のオブジェクトの位置に足した場所に入れる
                    //130下がった位置に入れる
                }
                else
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 162);
                    myPosi.anchoredPosition = posi;
                    //規定値を前のオブジェクトの位置に足した場所に入れる
                    //162下げた場所に置く
                }
            }
        }
        else if (windowType == 1)
        {
            RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
            Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 130);
            myPosi.anchoredPosition = posi;
#if false
            //敵タイプ
            UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
            if (before.windowType == 0)
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //130下がった位置に入れる
            }
            else if (before.windowType == 1)
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //ない可能性高い。一個目の条件だけでオーケー
            }
            else
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //ない可能性高い。一個目の条件だけでオーケー
            }
#endif
        }
        else
        {
            RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
            Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 162);
             myPosi.anchoredPosition = posi;
            #if false
            //窓タイプ２、数値設定
            UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
            if (before.windowType == 0)
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //162下がった位置に入れる。
            }
            else if (before.windowType == 1)
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //ない可能性高い。一個目の条件だけでオーケー
            }
            else
            {
                //規定値を前のオブジェクトの位置に足した場所に入れる
                //ない可能性高い。一個目の条件だけでオーケー
            }
#endif
        }
        //入れ替え
        MainUI.instance.beforeSet = this.gameObject;
    }



}
