using MoreMountains.Feedbacks;
using System.Collections;
using MoreMountains.Tools;
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

    [SerializeField]
    Selectable _resetWindow;



    // Start is called before the first frame update

    EnemyController eCon;

    void Start()
    {
        c = GetComponent<CircleCollider2D>();
        eCon = _resetWindow.transform.parent.gameObject.MMGetComponentNoAlloc<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
//Debug.Log($"s{c.enabled}s");
        if (isCallable && MainUICon.instance._reInput.SubmitButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown && GManager.instance.PlayerStateCheck())
        {
            GManager.instance.PlayerEventLock(true);
            WindowSet();
            isCallable = false;
            c.enabled = false;
            button.Select();
            eCon.ResetEnemy();
            if (!isFirst)
            {
                // Debug.Log($"aas{isFirst}");

                isFirst = true;
            }

            GManager.instance.onGimmick = true;
            MainUICon.instance.UIOn = true;
            
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


   public void ResetWindowCall()
    {
        if (eCon.EnemyExist())
        {
            eCon.ResetEnemy();
        }
        _resetWindow.transform.parent.gameObject.SetActive(true);
        _resetWindow.Select();
        WindowCansel();
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
