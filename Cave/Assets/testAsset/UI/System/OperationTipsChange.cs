using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationTipsChange : MonoBehaviour
{
    bool isFirst;

    [HideInInspector]
    public string description;

    [SerializeField]
    string[] txCandi;

    /// <summary>
    /// �^�C�v�Ƃ���TargetJudge�̂悤�ȓ��ނ����̂���
    /// ���⁪�͈��Z�b�g�ԍ��Ő؂�ւ��Ă邩�炢�����A�t�@�[�X�g��SR�R���f�B�V��������
    /// ���l�����̂Ƃ��������̂̉������΂������
    /// EditButton�̂悤�ɍU���񕜎x���Ŏg�����������
    /// �����񕜂̂�����Ȃ��ˁA�ŗL������
    /// ���ɂ����邩�v����
    /// �Ă���������Ȃ�
    /// </summary>
    [SerializeField]
    int type;

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        int s = MainUICon.instance.settingNumber;

        

        if (MainUICon.instance.eventSystem.currentSelectedGameObject == this.gameObject)
        {
            isFirst = true;
            MainUICon.instance.isTips = true;
        }
        else if (isFirst)
        {
            MainUICon.instance.isTips = false;
            isFirst = false;
        }

    }
}
