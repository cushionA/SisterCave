using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDrop : MonoBehaviour
{
    //��ڂ̑�
    public GameObject secondDrop;
    //�O�ڂ̑�
    public GameObject thirdDrop;
    //�l�ݒ�
    public GameObject valueWindow;

    //�����炩��q�I�u�W�F�N�g��`���Đݒ肷��B

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ����ς��������ׂȂ�������
    /// </summary>
    public void ChangeWindow()
    {

        //Setting�i���o�[�ŏ����ς���B
        //Setting�i���o�[1�Ȃ�l�ݒ�̑����X�L�b�v���Ⴄ���̂ɕς��邩�B�Z�J���h�h���b�v�ȍ~�𓮂����̂ƁA�l�ݒ葋�̐؂�ւ��Ɣz�u�B
        //Selectable�̑I�ђ������B�i���G�A��Ԉُ�A�x���A�Ȃ��̎��͂Ȃ��j
        //Setting�i���o�[3�A5�Ȃ�l�ݒ�̑����X�L�b�v���Ⴄ���̂ɕς��邩�B���͈�����Ȃ̂ő���؂�ւ���̂�Selectable�̑I�тȂ���
        //pre�ɂ�next�ɂ��ŏ��̃h���b�v�_�E��������
        //Setting�i���o�[2�A4�A6�Ȃ�Z�J���h�h���b�v�ȍ~�����������B�l�ݒ葋�̐؂�ւ��Ɣz�u
        //Selectable�̍Đݒ�͂Ȃ�

        //�v���Z�X�Ƃ��Ă͂ǂ�ł��܂���U�S�������B
        //��������K�v�ȑ����o���ĕ��ѕς���B

        //���[�J���ϐ��ł��܂������

    }

    /// <summary>
    /// �ŏ��ɏo���������̃��\�b�h
    /// �h���b�v�_�E���̒l�����߂āAValueChange���o�����肷��B
    /// ���Ƃ��ׂẴh���b�v�_�E���⑋�ɍŏ��ɏo���Ƃ��Ɏ����̒l�������Ɠ����悤�Ȃ̂������������悭�ˁH
    /// if(hoge = 1)�݂����Ȃ̂ŏ��J��ɂ�
    /// �����o����
    /// </summary>
    public void Initialize()
    {
        int s = MainUI.instance.settingNumber;
        int e = MainUI.instance.editNumber;

        //�܂��ݒ�ԍ��Ɋ�Â��񕜂Ƃ��U���Ƃ��𕪂���

        //�Z�b�g�I�u�W�F�N�g���^�̃I�u�W�F�N�g��ChildWindow��UI������������H



    }
    /// <summary>
    /// �ŏ��̃h���b�v�_�E�����I��������B
    /// �o�b�N�X�y�[�X�ōs���B�ԍ������Ƃɖ߂��B
    /// </summary>
    public void UIEnd()
    {
        //   MainUI.instance.selectList = new List<Selectable>();
        if (MainUI.instance.settingNumber % 2 == 1)
        {
            MainUI.instance.settingNumber--;
        }
        this.gameObject.SetActive(false);

    }
}
