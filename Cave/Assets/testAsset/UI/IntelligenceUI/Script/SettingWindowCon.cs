using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �E���̑��S�̂̐���
/// </summary>
public class SettingWindowCon : MonoBehaviour
{
    [SerializeField]
    Selectable[] myButton;

    [SerializeField]
    bool isAH;

    public OperationGeneral o;
    [HideInInspector]
    public bool enable;
    
    public GameObject nextWindow;

    //���̂܂ǂ̂��������䂤�����ł���
    bool isEnable;

    // Start is called before the first frame update

    private void OnEnable()
    {
        if (isAH)
        {
            ButtonSet();
            Debug.Log("asdfg");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enable && o.next)
        {
            if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
            {
                if (isAH)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                o.next = false;
                }
              //  Debug.Log("ssss3");
                o.SelectReset();

            }

        }
    }
    /// <summary>
    /// �����FirstWindow�����Ƃ��ɌĂяo���B
    /// </summary>
    public void CancelWindow()
    {
      //  nextWindow.SetActive(false);


        if (isAH)
        {
            if (MainUI.instance.settingNumber == 6)
            {
                //  Debug.Log($"�O{MainUI.instance.settingNumber}");
                myButton[MainUI.instance.editNumber + 2].Select();
                MainUI.instance.settingNumber = 5;
           //     Debug.Log($"ato{MainUI.instance.settingNumber}");
            }
            else
            {
                myButton[MainUI.instance.editNumber - 1].Select();
            }
           // MainUI.instance.isAH = false;
        }
        else
        {
            if (MainUI.instance.settingNumber % 2 == 0)
            {
                //  Debug.Log($"�O{MainUI.instance.settingNumber}");
                myButton[MainUI.instance.editNumber + 4].Select();
                MainUI.instance.settingNumber--;
                Debug.Log($"ato{MainUI.instance.settingNumber}");
            }
            else
            {
                myButton[MainUI.instance.editNumber - 1].Select();
            }
        }
        enable = false;
        ButtonSet();
    }

    public void ButtonSet()
    {
        //  Debug.Log($"������{myButton.Length}");

        if (!isAH)
        {
            for (int i = 0; i <= myButton.Length - 1; i++)
            {
                myButton[i].GetComponent<EditButton>().ChangeString();
            }
        }
        else
        {
            for (int i = 0; i <= myButton.Length - 1; i++)
            {
                myButton[i].GetComponent<AHSetButton>().ValueApply();
            }
        }
    }

}
