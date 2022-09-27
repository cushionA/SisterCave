using MoreMountains.Tools;
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
            if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
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
            if (MainUICon.instance.settingNumber == 6)
            {
                //  Debug.Log($"�O{MainUICon.instance.settingNumber}");
                myButton[MainUICon.instance.editNumber + 2].Select();
                MainUICon.instance.settingNumber = 5;
           //     Debug.Log($"ato{MainUICon.instance.settingNumber}");
            }
            else
            {
                myButton[MainUICon.instance.editNumber - 1].Select();
            }
           // MainUICon.instance.isAH = false;
        }
        else
        {
            if (MainUICon.instance.settingNumber % 2 == 0)
            {
                //  Debug.Log($"�O{MainUICon.instance.settingNumber}");
                myButton[MainUICon.instance.editNumber + 4].Select();
                MainUICon.instance.settingNumber--;
                Debug.Log($"ato{MainUICon.instance.settingNumber}");
            }
            else
            {
                myButton[MainUICon.instance.editNumber - 1].Select();
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
                myButton[i].gameObject.MMGetComponentNoAlloc<EditButton>().ChangeString();
            }
        }
        else
        {
            for (int i = 0; i <= myButton.Length - 1; i++)
            {
                myButton[i].gameObject.MMGetComponentNoAlloc<AHSetButton>().ValueApply();
            }
        }
    }

}
