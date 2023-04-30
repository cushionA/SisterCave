using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MyScroll: MonoBehaviour
{

    /// <summary>
    /// �������^�[�Q�b�g�̃I�u�W�F�N�g
    /// </summary>
    public RectTransform Target;

    /// <summary>
    /// �X�N���[���ŏ㉺�ɓ�������
    /// </summary>
    public float _moveDistance;

    /// <summary>
    /// ����܂ŃX�N���[���ł��邩
    /// �Ⴆ�Ό܉�Ȃ��񉺂ɍs����1������
    /// �����ď�ɍs���ƈ����
    /// ��{�I�ɂ͗�̐�
    /// </summary>
    public int _scrollLimit = 0;

    int _limitCounter = -1;

    GameObject lastObject;

    //1����A2����
    int moveState;

    /// <summary>
    /// �����ɓ��ꂽ�񐔂̐������X�N���[����҂�
    /// </summary>
    [SerializeField]
    int scrollWait;

    float startPosition;

    /// <summary>
    /// ������I���ɂ���ƃX���C�_�[���N������
    /// �ŏ�����N��������ꍇ�̓I���ɂ��Ƃ�
    /// �r������Ȃ�X�N���v�g�ő���
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
       //�h���b�v�_�E�������鎞
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

               //     Debug.Log("������");
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
          //  Debug.Log($"��{_limitCounter}");

    }
}
