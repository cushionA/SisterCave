using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SisUIsub : MonoBehaviour
{
    public bool sisMenu;
    public GameObject sisPanel;

    public GameObject sisr;
    public GameObject sisl;
    [SerializeField] EventSystem eventSystem;

    Button bt1;
    Button bt2;
    GameObject selectedObj;

    // Start is called before the first frame update
    void Start()
    {
        // 自分を選択状態にする
        bt1 = sisr.GetComponent<Button>();
        bt2 = sisl.GetComponent<Button>();
        //ボタンが選択された状態になる
        bt1.Select();
    

}

    // Update is called once per frame
    void Update()
    {
        float choice = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("SMenu"))
        {
            //メニュー展開ボタンを押すとメニューの表示非表示を切り替え
            if (sisMenu)
            {

                sisMenu = false;
            }

            else if (!sisMenu)
            {
                sisMenu = true;
            }
        }

        //メニュー非展開
        if (!sisMenu)
        {
            Time.timeScale = 1.0f;
            sisPanel.SetActive(false);

        }
        //メニュー展開中
        else if (sisMenu)
        {
            Time.timeScale = 0;
            sisPanel.SetActive(true);

            selectedObj = eventSystem.currentSelectedGameObject;

            if (sisr == selectedObj)
            {
                //右のボタンが選択中に右キーを押したら

                if (Input.GetButtonDown("ChoiceR"))
                {
                    bt1.onClick.Invoke();

                }
            }
            else if (sisl == selectedObj)
            {
                //左のボタンが選択中に左キーを押したら

                if (Input.GetButtonDown("ChoiceL"))
                {
                    bt2.onClick.Invoke();

                }


            }


            }



        }
    
    public void MenuBreake()
    {
        sisMenu = false;

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
