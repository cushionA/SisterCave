using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationButton : MonoBehaviour
{

    [SerializeField]
    Selectable next;

    Selectable my;

    /// <summary>
    /// �{�^���̃^�C�v�B
    /// 1�����̑��ɍs���^�C�v2���������Ĉړ�����^�C�v�A3���ۑ�����^�C�v
    /// </summary>
    [SerializeField]
    int type;

    // Start is called before the first frame update
    void Start()
    {
        my = GetComponent<Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WakeWindow()
    {
        if(type == 0)
        {
            next.Select();
            transform.root.gameObject.GetComponent<OperationGeneral>().next = true;
        }
        else if(type == 1)
        {
            next.transform.root.gameObject.SetActive(true);
            next.Select();
        }
        else if (type == 2)
        {
            transform.root.gameObject.GetComponent<OperationGeneral>().next = true;
            MainUI.instance.saveWin.gameObject.SetActive(true);
            MainUI.instance.saveWin.WindowSet(1);
           // transform.root.gameObject.GetComponent<OperationGeneral>().SaveData();
            //�Z�[�u���܂����̑��o�������ł���
        }
    }

}
