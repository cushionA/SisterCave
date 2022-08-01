using Rewired;
using Rewired.Integration.UnityUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI instance = null;

    [HideInInspector]public bool selectWindow;
    [HideInInspector] public bool openWindow;
    [HideInInspector] public bool menuButtonOff;

    [HideInInspector]
    public bool isMenu;
    public GameObject masterUI;

    [HideInInspector] public GameObject selectButton;//各UIの選択用
    public GameObject eqButton;
    public GameObject useButton;
    public GameObject weaponButton;
    public GameObject magicButton;
    public GameObject coreButton;
    public GameObject keyButton;
    public GameObject materialButton;
    public GameObject libraryButton;
    public GameObject systemButton;

    public Button eqB;
    public Button useB;
    public Button weaponB;
    public Button magicB;
    public Button coreB;
    public Button keyB;
    public Button materialB;
    public Button libraryB;
    public Button systemB;


    public GameObject eqWindow;
    public GameObject useWindow;
    public GameObject weaponWindow;
    public GameObject magicWindow;
    public GameObject coreWindow;
    public GameObject keyWindow;
    public GameObject materialWindow;
    public GameObject libraryWindow;
    public GameObject systemWindow;

    public UseController usec;
    public EquipController wec;
    public MagicController mac;
    public CoreController coc;
    public KeyController kec;
    public EnemyDataController lic;
    public MaterialController mtc;

    [HideInInspector]
    public bool isTips;

    [HideInInspector]
    public bool tipNeed;

    public bool isReBuild;
    public GameObject Scon;
    public SaveWinCon saveWin;

    public RewiredEventSystem eventSystem;
    public RewiredStandaloneInputModule stIn;

    /// <summary>
    /// 入力一覧
    /// </summary>
    #region
    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction0;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction1;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction2;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction3;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction4;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction5;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction6;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction7;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction8;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction9;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction10;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction11;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction12;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction13;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction14;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction15;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction16;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction17;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction18;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction19;

    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int rewiredAction20;

    #endregion
    //  Button eq;
    SisUI sis;



    bool isFirst;
    bool isInitial;

    /// <summary>
    /// こいつ注意
    /// </summary>
    [HideInInspector]
    public bool isConversation;

    /// <summary>
    /// 作戦UIで使うパラメータ
    /// </summary>
    #region

    ///<summary>
    /// 設定中のウィンドウはなにか
    /// 基本とか攻撃とか
    /// さらに攻撃0~2、回復3~5とかにして１なら条件2なら行動という風に分ける
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


    }

    // Update is called once per frame
    void Update()
    {
        //   if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction5))
        //    {
        //      ////Debug.log("unnchi");
        //    }


        if (tipNeed && isTips)
        {
            TipsWindow.gameObject.SetActive(true);
        }
        else
        {
            TipsWindow.gameObject.SetActive(false);
            isTips = false;

        }
        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction20) && eventSystem.currentSelectedGameObject != null)
        {
            tipNeed = !tipNeed;
        }

    /*    if (isConversation)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    */

      //  //Debug.Log($"時間{Time.timeScale}");
        if (MaterialManager.instance.isUseMenu || ToolManager.instance.isUseMenu || EquipManager.instance.isUseMenu
            || EnemyDataManager.instance.isUseMenu || CoreManager.instance.isUseMenu || ToolManager.instance.isUseMenu || KeyManager.instance.isUseMenu)
        {
            openWindow = true;

        }
        else
        {
            openWindow = false;
        }
       // ////Debug.log($"さぁて真になれ{openWindow}");



       // ////Debug.log($"装備窓確認trueになれ{isInitial}");

        if (!sis.sisMenu)
        {
            if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction14) && !isConversation)
            {
                //メニュー展開ボタンを押すとメニューの表示非表示を切り替え
                if (isMenu && !selectWindow && !openWindow)
                {
                    isFirst = false;
                    isMenu = false;
                    isReBuild = false;
                    Scon.SetActive(false);
                    usec.isEver = false;
                    wec.isEver = false;
                    mac.isEver = false;
                    kec.isEver = false;
                    lic.isEver = false;
                    mtc.isEver = false;
                    coc.isEver = false;
                    //////Debug.log($"qawsedfrgjui,lo{usec.isEver}");
                    isInitial = false;
                    ButtonOn();
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
                }

                else if (!isMenu)
                {
                    isFirst = false;
                    GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "Default");
                   GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "UI");
                    // isFirst = true;
                    isMenu = true;
                    isReBuild = true;
                    Scon.SetActive(true);
                }
            }

            //メニュー非展開
            if (!isMenu && !isConversation)
            {
               // //Debug.Log("野菜くえ");
                Time.timeScale = 1.0f;
                if (!isFirst)
                {
                    masterUI.SetActive(false);
                    isFirst = true;
                }
            }
            //メニュー展開中
            else if (isMenu)
            {
                bool change = false;
                if (selectButton != eventSystem.currentSelectedGameObject)
                {
                    selectButton = eventSystem.currentSelectedGameObject;
                    change = true;
                }

                Time.timeScale = 0;

              if (!isFirst)
                {
                    eqB.Select();
                    eqB.onClick.Invoke();
                    isFirst = true;
                    masterUI.SetActive(true);
                }

              //切り替わってるなら
                if (change)
                {
                    //イベントトリガーで
                    if (selectButton == eqButton && !isInitial)
                    {//セレクトしているボタンが左端のボタンの時

                        eqWindow.SetActive(true);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);

                        wec.isIniti = false;
                        coc.isIniti = false;
                        usec.isIniti = false;
                        //表示するUIの選択
                        isInitial = true;
                    }
                    else if (selectButton == useButton)
                    {
                        eqWindow.SetActive(false);
                        useWindow.SetActive(true);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);

                        EquipMenuReset();
                        usec.isIniti = false;
                        isInitial = false;
                    }
                    else if (selectButton == weaponButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(true);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        wec.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == coreButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(true);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        coc.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == keyButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(true);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        kec.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == magicButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(true);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        mac.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == materialButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(true);
                        isInitial = false;
                        mtc.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == libraryButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(true);
                        systemWindow.SetActive(false);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        lic.isIniti = false;
                        EquipMenuReset();
                    }
                    else if (selectButton == systemButton)
                    {

                        eqWindow.SetActive(false);
                        useWindow.SetActive(false);
                        keyWindow.SetActive(false);
                        magicWindow.SetActive(false);
                        libraryWindow.SetActive(false);
                        systemWindow.SetActive(true);
                        weaponWindow.SetActive(false);
                        coreWindow.SetActive(false);
                        materialWindow.SetActive(false);
                        isInitial = false;
                        EquipMenuReset();
                    }
                }
            }
        }
    }


    public void MenuCancel()
    {
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(true, "Default");
        GManager.instance.InputR.controllers.maps.SetMapsEnabled(false, "UI");
        selectWindow = false;
        isFirst = false;
        isMenu = false;
        isReBuild = false;
        Scon.SetActive(false);
        usec.isEver = false;
        wec.isEver = false;
        mac.isEver = false;
        kec.isEver = false;
        lic.isEver = false;
        mtc.isEver = false;
        coc.isEver = false;
        selectButton = null;
        ButtonOn();
      // MainUICon.instance.selectButton = null;
       // ToolManager.instance.selectItem = null;
        isInitial = false;
    }

    void EquipMenuReset()
    {
        ToolManager.instance.isEquipMenu = false;
    }

 //   public void UseReBuild()
//    {
  //      isReBuild = true;
  //      usec.isEver = false;
 //   }

    public void ButtonOff()
    {
        weaponB.enabled = false;
        useB.enabled = false;
        coreB.enabled = false;
        magicB.enabled = false;
        keyB.enabled = false;
        useB.enabled = false;
        systemB.enabled = false;
        libraryB.enabled = false;
        menuButtonOff = true;
        selectButton = null;
    }
    public void ButtonOn()
    {
        weaponB.enabled = true;
        useB.enabled = true;
        coreB.enabled = true;
        magicB.enabled = true;
        keyB.enabled = true;
        useB.enabled = true;
        systemB.enabled = true;
        libraryB.enabled = true;
        menuButtonOff = false;
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


}

