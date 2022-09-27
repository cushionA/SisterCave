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

    [HideInInspector]
    public bool isFirst;



    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
//Debug.Log($"s{c.enabled}s");
        if (isCallable && MainUICon.instance._reInput.SubmitButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown && GManager.instance.PlayerStateCheck())
        {
            WindowSet();
            isCallable = false;
            c.enabled = false;
            button.Select();
            
            if (!isFirst)
            {
                // Debug.Log($"aas{isFirst}");

                isFirst = true;
            }
           // Debug.Log("ssssssssseerf");
            GManager.instance.onGimmick = true;
            MainUICon.instance.UIOn = true;
            GManager.instance.PlayerEventLock(true);
        }
        else if (isFirst && !isCallable && MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown && window.activeSelf)
        {
            //ここ原因
            WindowCansel();
            c.enabled = true;
            //
            isFirst = false;
            GManager.instance.onGimmick = false;
            MainUICon.instance.tipNeed = false;
            MainUICon.instance.UIOn = false;
            MainUICon.instance.isSave = false;
            GManager.instance.PlayerEventLock(false);
        }

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
