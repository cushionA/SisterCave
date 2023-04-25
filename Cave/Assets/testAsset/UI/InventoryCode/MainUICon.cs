using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine.UI;
using Rewired.Integration.CorgiEngine;
using Rewired;
using Rewired.Integration.UnityUI;
using TMPro;
using MoreMountains.Feedbacks;

public class MainUICon : MonoBehaviour
{
    public static MainUICon instance = null;

    [HideInInspector] public bool selectWindow;

    [HideInInspector] public bool menuButtonOff;

    [HideInInspector]
    public bool UIOn;

    [HideInInspector]
    public bool isMenu;
    public GameObject masterUI;

    [HideInInspector] public GameObject selectButton;//各UIの選択用

    /// <summary>
    ///     表示するウィンドウを切り替える
    /// </summary>
    public TMP_Dropdown _windowSelecter;

    //装備画面、インベントリ、システム設定

    public GameObject equipWindow; 
    public GameObject inventoryWindow;
    public GameObject systemWindow;


    [HideInInspector]
    public bool isTips;

    [HideInInspector]
    public bool tipNeed;



    public SaveWinCon saveWin;

    public RewiredEventSystem eventSystem;
    public RewiredStandaloneInputModule stIn;


    public RewiredCorgiEngineInputManager _reInput;

    //  Button eq;
    SisUI sis;


    //最初の処理を行ったかのフラグ
    bool isInitial;

    /// <summary>
    /// こいつ注意
    /// </summary>
    [HideInInspector]
    public bool isConversation;



    ///<summary>
    ///  インベントリのスクロールに使う
    /// </summary>
    #region







    //UIでの入力が長押し判定になるまでの時間




    //長押し計測用
    float pressTime;

    //長押しリピート時に次に動く時間
    float nextTime;


    //現在スクロールするか
    bool _scrollEnable;

    #endregion


    /// <summary>
    /// 作戦UIで使うパラメータ
    /// </summary>
    #region

    ///<summary>
    /// 設定中のウィンドウはなにか
    /// 基本とか攻撃とか
    /// 1、ターゲット設定 2、攻撃設定
    /// 3、支援条件 4、支援決定
    /// 5、回復条件 6、回復決定
    ///</summary>
    [HideInInspector]
    public int settingNumber;

    ///<summary>
    /// 何番目の条件を編集中か。参照先獲得
    /// 第一条件編集中とか
    /// セッティングナンバーと併用で特定できる
    ///</summary>
    [HideInInspector]
    public int editNumber;

    ///<summary>
    /// 編集用のパラメータ。いろいろ参照する。
    ///</summary>

    public SisterParameter editParameter;

    public GameObject secondDrop;
    //1個目の窓
    public GameObject firstDrop;
    //値設定
    [HideInInspector]
    public GameObject valueWindow;

    /// <summary>
    /// ドロップダウン変更による入れ替え対象
    /// </summary>
    [HideInInspector]
    public Selectable changeTarget;

    /// <summary>
    /// 先ほどセットしたゲームオブジェクト。参照用
    /// </summary>
    [HideInInspector]
    public GameObject beforeSet;

    /// <summary>
    /// 接続変更するとき
    /// </summary>
    public int isChange;

    /// <summary>
    /// オートヒール設定中
    /// </summary>
    [HideInInspector]
    public bool isAH;

    /// <summary>
    /// セーブしたかどうか、色々使える
    /// ゲーム開始時か鳴らす偽に
    /// </summary>
    [HideInInspector]
    public bool isSave;
    [HideInInspector]
    public bool editNow;

    [SerializeField]
    Transform TipsWindow;

    [SerializeField]
    MainInventryCon _inventoryCon;

    //求められる機能
    //
    //ドロップダウンのついたウィンドウを一つ用意する
    //第二段階のボタンが押されるとその窓が出る。ドロップダウンの要素はボタンごとについた番号と設定番号から割り出す。参照先も
    //参照先は変数にして持つ。また、当然何もしないでは何も出てこない
    //ドロップダウンで選択されたタイプに応じて同じようなドロップダウンがついてたりチェックボックスがついてたりするウィンドウが出る。
    //ドロップダウンと含めて最大三つのウィンドウが出る
    //行動はクールタイムも持たせる？つまり四つ目も

    //支援と回復条件の場合
    //ウィンドウ一つ
    //状態異常の時、チェックボックスが出る（数は未定）
    //切れてる支援、これもチェックボックス（数は未定）
    //シスターさんとプレイヤーのMPやHP、入力フォームと以上以下指定
    //敵タイプはチェックボックス。
    //チェックボックスごとに備えられた数値で論理積かける桁を選んだりすれば流用できそう

    //ウィンドウゼロは強敵が出るかどうかと指定なし
    //ドロップダウンとかで何を選んでるかで表示する説明文を変えていいかもね

    //支援回復行の行動は原則三つ
    //一番目で何もしないや他へ移行、二番目で何もしないを選ぶとウィンドウの数が変わる
    //三番目では条件に加えて以上以下を選ぶ
    //支援効果は回復がリジェネとバリア、支援がそれ以外とすみ分けた方がよさそう

    //攻撃条件
    //まずゼロはなし
    //基本的に全ての条件で敵の弱点と敵パラメーターと位置での指定ができるので基本三つ以上
    //何もしないだけ三つ
    //他は四つ。シスターさんとプレイヤーのHP,MPは数値指定と上下について。強敵の存在はいないときも欲しいので作る
    //敵タイプや敵やプレイヤーの状態異常、切れてる支援はチェックボックス。これらは扱う桁と数値の解釈が違うだけで処理は同じ。共通のスクリプトで扱えるでしょう。
    //二番目の箱ではただ選ぶだけ。三番目では選んで、さらに例えばMP使用量ならそれが多いか少ないかも選べる。
    //最初からドロップダウンにMP消費量多いとか少ないとか入れるのか、別に設定するのかは任せる。

    //説明文はこの条件で攻撃に類する行動をとる場合の設定、みたいなのがいいかも

    //支援回復条件は見た目的にドロップダウン、設定
    //支援回復選択はドロップダウン、ドロップダウン（設定）、ドロップダウン、設定一つ

    //攻撃条件は見た目的には条件がドロップダウン、前項に基づく設定、弱点設定、第三条件ドロップダウン、チェックボックス一つ
    //魔法選択がドロップダウン、第二条けんドロップダウン、第三段階ドロップダウンとチェックボックス一つ。

    //設定ウィンドウの位置は前のドロップダウンから窓自体が持つ数値で上下に動く
    //必ずドロップダウン来るから
    //ドロップダウンはウィンドウタイプに応じて同じ処理を行う。

    //必要なチェックボックス
    //属性８、敵タイプ４，bool系、多いか少ないかの一つだけ。boolけいはドロップダウンにする。
    //必要なサポートはドロップダウンにする

    //となると設定ウィンドウのタイプはドロップダウン一つと、数値とbool、長いチェックボックスと短いチェックボックスの四種類になる。
    //こいつらを差別化するのに必要なのは参照先と何を設定するかのタイトルと、

    //自分に番号をつけて番号に合うリストに自分を入れる。Selectableでそれを参照する。
    //上と下、どちらも設定するかしないかを選べる。


    /// <summary>
    /// 先ほどまでの入力
    /// </summary>
    int lastDirection;

    /// <summary>
    /// 先ほどまで選択していた窓
    /// </summary>
    int lastNumber = 100;

    #endregion
    [SerializeField]
    MyScroll _scroll;
    [SerializeField]
    MyInventoryDisplay[] _useInventory;
    bool _slotNow = true;

    [SerializeField]
    Selectable[] startButton;

    [SerializeField]
    GameObject[] OptionWindow;

    /// <summary>
    /// みんなが使えるフィードバック
    /// </summary>
    #region

    public MMFeedbacks _TimeStop;

    public MMFeedbacks _TimeStart;

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        sis = GetComponent<SisUI>();

        // 自分を選択状態にする
        // eq = equip.GetComponent<Button>();



        nextTime = stIn.repeatDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //   if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction5))
        //    {
        //      ////
        //    }
        if (eventSystem.currentSelectedGameObject != null)
        {
            //  Debug.Log($"ね{eventSystem.currentSelectedGameObject}{selectButton}");
        }
        else
        {
            //Debug.Log($"unnchi");
        }
   //     Debug.Log($"ね{stIn.RewiredInputManager.get(stIn.verticalAxis)}");




        if (UIOn)
        {

        if (tipNeed && isTips)
        {

            TipsWindow.gameObject.SetActive(true);
        }
        else
        {
            
            TipsWindow.gameObject.SetActive(false);
            isTips = false;

        }
        if (_reInput.TipsButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && eventSystem.currentSelectedGameObject != null)
        {
          //  Debug.Log($"s");
            tipNeed = !tipNeed;
        }
            if (selectButton != eventSystem.currentSelectedGameObject)
            {
                selectButton = eventSystem.currentSelectedGameObject;
                if (selectButton == _windowSelecter.gameObject)
                {

                    _scroll.ResetPoss();
                }
            }
        }

        //UIついてるのにisMenuじゃないとき
        if (!(UIOn && !isMenu))
        {

            if (_reInput.MenuCallButton.State.CurrentState == MMInput.ButtonStates.ButtonDown && !isConversation)
            {
                //メニュー展開ボタンを押すとメニューの表示非表示を切り替え
                if (isMenu && !selectWindow)
                {

                    isMenu = false;
                    masterUI.SetActive(false);
                    UIOn = false;
                    _TimeStart?.PlayFeedbacks();

                    for (int i = 0;i <= OptionWindow.Length - 1;i++)
                    {
                        OptionWindow[i].SetActive(false);
                    }
                    isInitial = false;
                    ButtonOn();

                    _scrollEnable = false;
                }

                else if (!isMenu)
                {
                    

                    _TimeStop?.PlayFeedbacks();

                    isMenu = true;
                    UIOn = true;
                    masterUI.SetActive(true);

                }
            }

           if (isMenu)
            {



 

                if (!isInitial)
                {
                    

                    _windowSelecter.Select();
                    _windowSelecter.value = 0;

                }
                //ドロップダウンの数値を入力で変えるやつ
                else if(selectButton == _windowSelecter.gameObject)
                {
                   
                    //右入力
                    if (_reInput.UIMovement.x > 0)
                    {

                        //前のフレームのRaw入力（0か1か‐1）から今の入力を引いて0以下なら
                        //これをやると長押し時の高速カーソル移動に上から下にいきなり変えたりの時上から下に急反転しない
                        //これは前の入力が-1か0の時に上を押したときとなる。ニュートラルか下入力から上に切り替えた時
                        if (lastDirection <= 0)
                        {
                            pressTime = 0;
                            nextTime = stIn.repeatDelay;
                        }

                        if (pressTime == 0 || pressTime > nextTime)
                        {
                            if (pressTime > stIn.repeatDelay)
                            {
                                //次に動くのはactcool秒後
                                nextTime += stIn.repeatDelay;
                            }
                            if (_windowSelecter.value != _windowSelecter.options.Count - 1)
                            {
                                _windowSelecter.value++;
                            }
                            else
                            {
                                _windowSelecter.value = 0;
                            }

                        }
                        pressTime += Time.unscaledDeltaTime;

                    }
                    //左入力
                    else if (_reInput.UIMovement.x < 0)
                    {
                        //前のフレームのRaw入力（0か1か‐1）から今の入力を引いて0以下なら
                        //これをやると長押し時の高速カーソル移動に上から下にいきなり変えたりの時上から下に急反転しない
                        //これは前の入力が1か0の時に下を押したときとなる。ニュートラルか上入力から下に切り替えた時
                        if (lastDirection >= 0)
                        {
                            pressTime = 0;
                            nextTime = stIn.repeatDelay;
                        }

                        if (pressTime == 0 || pressTime > nextTime)
                        {
                            if (pressTime > stIn.repeatDelay)
                            {
                                //次に動くのはactcool秒後
                                nextTime += stIn.repeatDelay;
                            }
                            if (_windowSelecter.value != 0)
                            {
                                _windowSelecter.value--;
                            }
                            else
                            {
                                _windowSelecter.value = _windowSelecter.options.Count - 1;
                            }
                        }
                        pressTime += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        pressTime = 0;
                    }
                    lastDirection = (int)_reInput.UIMovement.x;
                }
                // Debug.Log($"value{_windowSelecter.value}to{lastNumber}");
                if (_slotNow && selectButton != _windowSelecter.gameObject)
                {
                    if (_scrollEnable == false)
                    {
                        pressTime = 0;
                    }
                    _scrollEnable = true;
                }
                else
                {
                    //カーソルが上のボタンに戻ったら一番上にスクロールする
                    //           useBar.value = 1;
                    _scrollEnable = false;
                }

                //切り替わってるなら
                if (_windowSelecter.value != lastNumber || !isInitial)
                {

                    //windowのSetActiveのかわりにインベントリの描画対象を変える
                    //システム設定だけは他の窓

                    //スロットにカーソルあるか
                    

                    //イベントトリガーで
                    if (_windowSelecter.value == 0 || !isInitial)
                    {//セレクトしているボタンが左端のボタンの時

                        equipWindow.SetActive(true);
                        
                        
                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                            inventoryWindow.SetActive(false);
                        }
                        else
                        {
                            systemWindow.SetActive(false);
                        }
                        lastNumber = 0;
                        //表示するUIの選択
                        isInitial = true;
                        _slotNow = false;
                        NavigationSetting(0);

                    }
                    //最初と最後以外なら
                    else if (_windowSelecter.value != 0 && !(_windowSelecter.value >= _windowSelecter.options.Count - 1))
                    {
 　　　　　　　　　　　Debug.Log("ふぇｓ");

                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                         //  
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                        }
                        else
                        {
                            inventoryWindow.SetActive(true);
                            systemWindow.SetActive(false);
                            equipWindow.SetActive(false);
                        }
                        _useInventory[_windowSelecter.value - 1].gameObject.SetActive(true);

                        //インベントリあるならやれ
                        if (_useInventory[_windowSelecter.value - 1] != null) 
                        {
                            _inventoryCon.MenuReset(_useInventory[_windowSelecter.value - 1]);
                        }
                      //  Debug.Log($"ddddddd{lastNumber -1}{_windowSelecter.value - 1}de{_useInventory[lastNumber - 1].gameObject.name}");
                        lastNumber = _windowSelecter.value;
                       
                        _slotNow = true;
                    }
                    else
                    {

                        if (lastNumber != 0 && !(lastNumber >= _windowSelecter.options.Count - 1))
                        {
                            _useInventory[lastNumber - 1].gameObject.SetActive(false);
                            equipWindow.SetActive(false);
                        }
                        else
                        {
                             inventoryWindow.SetActive(false);
                        }
                        systemWindow.SetActive(true);
                        
                        NavigationSetting(1);
                        lastNumber = _windowSelecter.value;
                        _slotNow = false;
                    }

                   

                }

                JButton();
            } 
        }
    }


    public void MenuCancel()
    {
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
        selectWindow = false;

        isMenu = false;


        selectButton = null;
        ButtonOn();
        // MainUICon.instance.selectButton = null;
        // ToolManager.instance.selectItem = null;
        isInitial = false;
    }



    public void ButtonOff()
    {

    }

    public void ButtonOn()
    {
    }

    public void ConversationStart()
    {

        isConversation = true;
        ////Debug.Log("肉食べたい");
    }

    public void ConversationEnd()
    {

        isConversation = false;
    }

    public void JButton()
    {
        //インベントリ内のアイテムの数がdata.count

        //これ重要ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
        //　 pos = 1f / ((float)data.Count - 4.0f);
        //MyItem.rowLengthが20にあたります、intで宣言しているので、floatに置換しています。
        //公式は　ScrollBarのValueの上限、セルの数、表示可能なセルの数。
        if (!_scrollEnable)
        {
            _scroll._inputRead = false;
            return;
        }
        //   Debug.Log($"ad{_reInput.UIMovement.y}");
        //なんなら四段目までは動かないようにしてもいいね
        _scroll._inputRead = true;

    }

    /// <summary>
    /// 窓切り替えと最初のボタンの間でナビゲーションを設定する
    /// </summary>
    /// <param name="i"></param>
    public void NavigationSetting(int i)
    {
        Navigation startPoint = _windowSelecter.navigation;
        startPoint.selectOnDown = startButton[i];
        _windowSelecter.navigation = startPoint;

        startPoint = startButton[i].navigation;
        startPoint.selectOnUp = _windowSelecter;
        startButton[i].navigation = startPoint;
    }

}

