using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ����͍�푋���o�鎞��Edit�p�����[�^�����݂̂ɏ㏑��������AEdit��V�����ۑ��p�p�����[�^�[�ɕۑ�������t�̂��Ƃ�����
/// ���ƍŏ��ɑI��������
/// ���͑I������
/// ��͉E�̑��̕���������������H
/// �e�{�^���������Ƃ��̏���������Ƃ���
/// </summary>
public class OperationGeneral : MonoBehaviour
{


    /// <summary>
    /// �߂��̂ɂ��g��
    /// 4���O�̂ŁA5���Z�[�u�m�F���
    /// </summary>
    public Selectable[] myButton;
    public Text winName;

    public SettingWindowCon setWin;

    /// <summary>
    /// ���̑��Ŏg�����߂̃t���O�A
    /// </summary>
    [HideInInspector]
    public bool next;

    int bNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        //�ŏ��ɃG�f�B�b�g�ɔ��f�B
        //MainUI.instance.editParameter = SManager.instance.Sister.GetComponent<SisterFire>().sister;
        myButton[0].Select();
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log($"�i���o�[{MainUI.instance.settingNumber}");
        /// Debug.Log($"�I���{�^��{MainUI.instance.eventSystem.currentSelectedGameObject}");
        if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[1].gameObject)
        {
           // winName.text = "�U�������ݒ�";
            MainUI.instance.settingNumber = 1;
            
        }
        else if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[2].gameObject)
        {
           // winName.text = "�x�������ݒ�";
            MainUI.instance.settingNumber = 3;
        }
        else if (MainUI.instance.eventSystem.currentSelectedGameObject == myButton[3].gameObject)
        {
           // winName.text = "�񕜏����ݒ�";
            MainUI.instance.settingNumber = 5;
        }
        if(bNumber != MainUI.instance.settingNumber)
        {
            if (MainUI.instance.settingNumber == 1)
            {
                 winName.text = "�U�������ݒ�";
            }
            else if (MainUI.instance.settingNumber == 3)
            {
                 winName.text = "�x�������ݒ�";
            }
            else if (MainUI.instance.settingNumber == 5)
            {
                 winName.text = "�񕜏����ݒ�";
                
            }
                 setWin.ButtonSet();
            bNumber = MainUI.instance.settingNumber;
        }

       
    }

    /// <summary>
    /// 
    /// </summary>
    public void SelectReset()
    {
        int s = MainUI.instance.settingNumber;

        if(s == 1)
        {
            myButton[1].Select();
        }
        else if (s == 3)
        {
            myButton[2].Select();
        }
        else if (s == 5)
        {
            myButton[3].Select();
        }
    }

    public void EditEnd()
    {
        //�v�ҏW�B�Z�[�u������߂���@���Ȃ�
        if (MainUI.instance.isSave)
        {
            MainUI.instance.isSave = false;
            myButton[4].transform.root.gameObject.SetActive(true);
            myButton[4].Select();
        }
        else
        {
            myButton[5].transform.root.gameObject.SetActive(true);
            myButton[5].Select();
        }
    }



}
