using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Е��ɂ�������΂����Ȃ���
/// </summary>
public class SaveWinBase : MonoBehaviour
{
    [SerializeField]
    Selectable back;
    //�ۑ������̓{�^���C�x���g�ł��������˂�



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction17))
        {
            SaveEnd();
      //      Debug.Log("ssss");
        }

    }

    public void SaveEnd()
    {
        transform.root.gameObject.SetActive(false);
        back.Select();

    }
    /// <summary>
    /// �Z�[�u���ĂȂ��Ă��I����悤�ɂ���
    /// �I�yG���̏ꍇ�����������EditEnd�Ăׂ΂���
    /// </summary>
    public void RejectSave()
    {

        MainUI.instance.isSave = true;
        SaveEnd();
    }


}
