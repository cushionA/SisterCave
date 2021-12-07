using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 右側の窓全体の制御
/// </summary>
public class SettingWindowCon : MonoBehaviour
{
    [SerializeField]
    Selectable[] myButton;

    public OperationGeneral o;

    
    public GameObject nextWindow;

    //このまどのそうさがゆうこうである
    bool isEnable;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnable && o.next)
        {
            if (GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction17))
            {
                o.SelectReset();
            }
            else if (GManager.instance.InputR.GetButton(MainUI.instance.rewiredAction18))
            {
                isEnable = true;
            }
        }
    }
    /// <summary>
    /// これはFirstWindow消すときに呼び出す。
    /// </summary>
    public void CancelWindow()
    {
        nextWindow.SetActive(false);
        isEnable = false;
        if (MainUI.instance.settingNumber % 2 == 0)
        {
            myButton[MainUI.instance.editNumber - 1].Select();
            MainUI.instance.settingNumber--;
        }
        else
        {
            myButton[MainUI.instance.editNumber + 4].Select();
        }
    }

    public void ButtonSet()
    {
      //  Debug.Log($"あああ{myButton.Length}");

        for(int i = 0; i <= myButton.Length - 1; i++)
        {
            myButton[i].GetComponent<EditButton>().ChangeString();
        }
    }

}
