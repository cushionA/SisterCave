using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestPoint : MonoBehaviour
{
    /// <summary>
    /// トリガー反応中にアクションボタンで起動
    /// ギミックオンにして停止させる？　UI表示
    /// いろんな機能がある。ステータス表示したりスペル付け替えたり
    /// まぁ今回は戦術UI表示だけ
    /// </summary>

     public GameObject window;
    [SerializeField] Selectable button;

    bool isCallable;
    CircleCollider2D c;

    bool isFirst;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isCallable && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction5))
        {
            WindowSet();
            isCallable = false;
            c.enabled = false;
        }
        else if (isFirst && !isCallable && GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction17))
        {
            WindowCansel();
            c.enabled = true;
            isFirst = false;
        }
    }

    private void OnEnable()
    {
        if (!isFirst)
        {
            button.Select();
            isFirst = true;
        }
     //   WindowChange();
    }

    ///<summary>
    ///窓を初期化するやつ
    /// </summary>
    void WindowChange()
    {


     /*   for(int i = 0;i >= buttonList.Length; i++)
        {

            if(MainUI.instance.selectButton == buttonList[i])
            {
                windowList[i].SetActive(true);
            }
            else
            {
                windowList[i].SetActive(false);
            }
        }*/

    }

    public void WindowSet()
    {
        window.SetActive(true);
    }

    public void WindowCansel()
    {
        window.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isCallable = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isCallable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isCallable = false;
        }
    }


}
