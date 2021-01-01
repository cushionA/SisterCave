using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI instance = null;

    [HideInInspector]public bool selectWindow;
    [HideInInspector] public bool openWindow;
    [HideInInspector] public bool menuButtonOff;

    public bool isMenu;
    public GameObject masterUI;


    public GameObject eqButton;
    public GameObject useButton;
    public GameObject weponButton;
    public GameObject magicButton;
    public GameObject coreButton;
    public GameObject keyButton;
    public GameObject materialButton;
    public GameObject libraryButton;
    public GameObject systemButton;

    public Button eqB;
    public Button useB;
    public Button weponB;
    public Button magicB;
    public Button coreB;
    public Button keyB;
    public Button materialB;
    public Button libraryB;
    public Button systemB;


    public GameObject eqWindow;
    public GameObject useWindow;
    public GameObject weponWindow;
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

    float verticalKey;

    public bool isReBuild;
    public GameObject Scon;


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

    #endregion
    //  Button eq;
    SisUI sis;

    GameObject selectButtom;

    bool isFirst;
    bool isInitial;

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
     //   if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction5))
    //    {
      //      //Debug.log("unnchi");
    //    }


         if (MaterialManager.instance.isUseMenu || ToolManager.instance.isUseMenu || EquipManager.instance.isUseMenu
            || EnemyDataManager.instance.isUseMenu || CoreManager.instance.isUseMenu || ToolManager.instance.isUseMenu || KeyManager.instance.isUseMenu)
        {
            openWindow = true;

        }
        else
        {
            openWindow = false;
        }
       // //Debug.log($"さぁて真になれ{openWindow}");

         verticalKey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15);

       // //Debug.log($"装備窓確認trueになれ{isInitial}");

        if (!sis.sisMenu)
        {
            if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
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
                    ////Debug.log($"qawsedfrgjui,lo{usec.isEver}");
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
            if (!isMenu)
            {
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

                selectButtom = eventSystem.currentSelectedGameObject;

                Time.timeScale = 0;

              if (!isFirst)
                {
                    eqB.Select();
                    eqB.onClick.Invoke();
                    isFirst = true;
                    masterUI.SetActive(true);
                }

                if(selectButtom == eqButton && !isInitial)
                {//セレクトしているボタンが左端のボタンの時

                    eqWindow.SetActive(true);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);

                    wec.isIniti = false;
                    coc.isIniti = false;
                    usec.isIniti = false;
                    //表示するUIの選択
                    isInitial = true;
                }
                else if(selectButtom == useButton)
                {
                    eqWindow.SetActive(false);
                    useWindow.SetActive(true);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);

                    EquipMenuReset();
                    usec.isIniti = false;
                    isInitial = false;
                }
                else if (selectButtom == weponButton)
                {

                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(true);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    wec.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == coreButton)
                {

                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(true);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    coc.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == keyButton)
                {
                   
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(true);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    kec.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == magicButton)
                {
                  
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(true);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    mac.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == materialButton)
                {

                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(true);
                    isInitial = false;
                    mtc.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == libraryButton)
                {

                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(true);
                    systemWindow.SetActive(false);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    lic.isIniti = false;
                    EquipMenuReset();
                }
                else if (selectButtom == systemButton)
                {
                   
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(true);
                    weponWindow.SetActive(false);
                    coreWindow.SetActive(false);
                    materialWindow.SetActive(false);
                    isInitial = false;
                    EquipMenuReset();
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
        ButtonOn();
      // ToolManager.instance.selectButton = null;
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
        weponB.enabled = false;
        useB.enabled = false;
        coreB.enabled = false;
        magicB.enabled = false;
        keyB.enabled = false;
        useB.enabled = false;
        systemB.enabled = false;
        libraryB.enabled = false;
        menuButtonOff = true;
    }
    public void ButtonOn()
    {
        weponB.enabled = true;
        useB.enabled = true;
        coreB.enabled = true;
        magicB.enabled = true;
        keyB.enabled = true;
        useB.enabled = true;
        systemB.enabled = true;
        libraryB.enabled = true;
        menuButtonOff = false;
    }
}

