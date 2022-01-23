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
Debug.Log($"s{c.enabled}s");
        if (isCallable && GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction5))
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
            if (!GManager.instance.equipWeapon.twinHand)
            {

                GManager.instance.pm.anim.Play("OStand");

            }
            else
            {
                GManager.instance.pm.anim.Play("TStand");

            }
            MainUI.instance.isConversation = true;
        }
        else if (isFirst && !isCallable && GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17) && window.activeSelf)
        {
            //��������
            WindowCansel();
            c.enabled = true;
            //
            isFirst = false;
            GManager.instance.onGimmick = false;
            MainUI.instance.tipNeed = false;
            MainUI.instance.isConversation = false;
            MainUI.instance.isSave = false;
        }

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
