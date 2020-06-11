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

    Button eq;
    GameObject selectedObj;
    SisUI sis;

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
        //float choice = Input.GetAxisRaw("Horizontal");

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
                Time.timeScale = 0;
                masterUI.SetActive(true);

                if (isFirst)
                {
                    eq.Select();
                    eq.onClick.Invoke();
                    isFirst = false;
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

