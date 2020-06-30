using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public bool isMenu;
    public GameObject masterUI;
    public GameObject equip;


    public GameObject eqButton;
    public GameObject useButton;
    public GameObject keyButton;
    public GameObject magicButton;
    public GameObject libraryButton;
    public GameObject systemButton;


    public GameObject eqWindow;
    public GameObject useWindow;
    public GameObject keyWindow;
    public GameObject magicWindow;
    public GameObject libraryWindow;
    public GameObject systemWindow;



    float verticalKey;



    [SerializeField] EventSystem eventSystem;
    [SerializeField] StandaloneInputModule stIn;

    Button eq;
    SisUI sis;

    GameObject selectButtom;

    bool isFirst;



    // Start is called before the first frame update
    void Start()
    {
        sis = GetComponent<SisUI>();

        // 自分を選択状態にする
        eq = equip.GetComponent<Button>();
        
        //ボタンが選択された状態になる
        //bt1.Select();


    }

    // Update is called once per frame
    void Update()
    {
         verticalKey = Input.GetAxisRaw("Vertical");

        if (!sis.sisMenu)
        {
            if (Input.GetButtonDown("Menu"))
            {
                //メニュー展開ボタンを押すとメニューの表示非表示を切り替え
                if (isMenu)
                {
                    isMenu = false;
                }

                else if (!isMenu)
                {
                    isFirst = true;
                    isMenu = true;
                }
            }

            //メニュー非展開
            if (!isMenu)
            {
                Time.timeScale = 1.0f;
                masterUI.SetActive(false);

            }
            //メニュー展開中
            else if (isMenu)
            {

                selectButtom = eventSystem.currentSelectedGameObject;

                Time.timeScale = 0;
                masterUI.SetActive(true);

                if (isFirst)
                {
                

                    eq.Select();
                    eq.onClick.Invoke();
                    isFirst = false;
                }

                if(selectButtom == eqButton)
                {//セレクトしているボタンが左端のボタンの時

                    eqWindow.SetActive(true);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);
                    //表示するUIの選択
                }
                else if(selectButtom == useButton)
                {

                  

                    eqWindow.SetActive(false);
                    useWindow.SetActive(true);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);

                }
                else if (selectButtom == keyButton)
                {
                   
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(true);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);

                }
                else if (selectButtom == magicButton)
                {
                  
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(true);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(false);

                }
                else if (selectButtom == libraryButton)
                {

                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(true);
                    systemWindow.SetActive(false);

                }
                else if (selectButtom == systemButton)
                {
                   
                    eqWindow.SetActive(false);
                    useWindow.SetActive(false);
                    keyWindow.SetActive(false);
                    magicWindow.SetActive(false);
                    libraryWindow.SetActive(false);
                    systemWindow.SetActive(true);

                }

            }
        }
    }
    public void MenuBreake()
    {
        isMenu = false;

    }

    public void MenuCheck()
    {
        //右ボタンに反応するイベント
        Debug.Log("Left");

    }
    public void Check()
    {

        //左ボタンに反応するイベント
        Debug.Log("Right");

    }

}

