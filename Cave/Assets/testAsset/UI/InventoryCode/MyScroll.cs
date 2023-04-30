using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MyScroll: MonoBehaviour
{

    /// <summary>
    /// 動かすターゲットのオブジェクト
    /// </summary>
    public RectTransform Target;

    /// <summary>
    /// スクロールで上下に動く距離
    /// </summary>
    public float _moveDistance;

    /// <summary>
    /// 何回までスクロールできるか
    /// 例えば五回なら一回下に行くと1増える
    /// そして上に行くと一つ減る
    /// 基本的には列の数
    /// </summary>
    public int _scrollLimit = 0;

    int _limitCounter = -1;

    GameObject lastObject;

    //1が上、2が下
    int moveState;

    /// <summary>
    /// ここに入れた回数の数だけスクロールを待つ
    /// </summary>
    [SerializeField]
    int scrollWait;

    float startPosition;

    /// <summary>
    /// これをオンにするとスライダーが起動する
    /// 最初から起動させる場合はオンにしとく
    /// 途中からならスクリプトで操作
    /// </summary>
    public bool _inputRead;

    [SerializeField]
    TMP_Dropdown _useDropdown;


    private void Start()
    {
        if(Target == null)
        {
            Debug.Log($"{transform.root.gameObject.name}");
        }

        startPosition = Target.anchoredPosition.y;
       if(_scrollLimit == 0)
        {
            LimitChange();
        }
       //ドロップダウンがある時
        if (_useDropdown != null)
        {
            if (_useDropdown.value > 0)
            {
              //  _scrollLimit -= _useDropdown.value;
                Vector2 hoge = Target.anchoredPosition;
                hoge.Set(hoge.x, startPosition + _moveDistance * _useDropdown.value);
                Target.anchoredPosition = hoge;
                _limitCounter = _useDropdown.value;
                scrollWait = 0;
            }
        }

       if(scrollWait != 0)
        {
            _limitCounter = -scrollWait;
            _scrollLimit -= scrollWait;
        } 

    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {



    }

    private void Update()
    {

        if (_inputRead)
        {
            if(MainUICon.instance._reInput.UIMovement.y > 0)
            {
                UpScroll();
            }
            else if (MainUICon.instance._reInput.UIMovement.y < 0)
            {
                DownScroll();
            }
            else
            {
                moveState = 0;
            }
        }

        if (moveState != 0)
        {
            if (lastObject != MainUICon.instance.selectButton)
            {
                 lastObject = MainUICon.instance.selectButton;
                if(_limitCounter < 0)
                {
                    moveState = 0;
                    _limitCounter++;
                    return;
                }
                if(moveState == 1)
                {

                    Vector2 hoge = Target.anchoredPosition;
                    hoge.Set(hoge.x, hoge.y - _moveDistance);
                    Target.anchoredPosition = hoge;
                    _limitCounter--;
                }
                else if(moveState == 2)
                {

               //     Debug.Log("えええ");
                    Vector2 hoge = Target.anchoredPosition;
                    hoge.Set(hoge.x, hoge.y + _moveDistance);
                    Target.anchoredPosition = hoge;

                    //  Target.DOMoveY(_moveDistance, 0.3f);
                    _limitCounter++;
                }
                moveState = 0;
                
            }
        }

    }


    public void UpScroll()
    {

        if (_limitCounter == 0)
        {
            return;
        }

        moveState = 1;

    }

    public void DownScroll()
    {

        if(_limitCounter >= _scrollLimit)
        {
            return;
        }

        moveState = 2;

    }

    public void LimitChange()
    {
        float height = Target.sizeDelta.y;
        _scrollLimit = (int)(height / _moveDistance);
    }

    public void ResetPoss()
    {

            Vector2 hoge = Target.anchoredPosition;
            hoge.Set(hoge.x,startPosition);
            Target.anchoredPosition = hoge;
            _limitCounter = -scrollWait;
          //  Debug.Log($"毛{_limitCounter}");

    }
}
