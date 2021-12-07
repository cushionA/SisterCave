using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestPoint : MonoBehaviour
{
    /// <summary>
    /// �g���K�[�������ɃA�N�V�����{�^���ŋN��
    /// �M�~�b�N�I���ɂ��Ē�~������H�@UI�\��
    /// �����ȋ@�\������B�X�e�[�^�X�\��������X�y���t���ւ�����
    /// �܂�����͐�pUI�\������
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
    ///����������������
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
