using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlaceSet : MonoBehaviour
{

    //�ŏ��̃h���b�v�_�E���ɐݒ�ԍ���
    //UI���V���A���C�Y����@�\�����B
    //UI���ŏI�v�f����Ȃ���΃h���b�v�_�E���Ƌ��ɏo�Ă���
    //�悭�l������V���A���C�Y������̂̓h���b�v�_�E�������̎d������

    //�V���A���C�Y�̏����͐ڑ�����I�u�W�F�N�g���V���A���C�Y������ɓn������
    //����Ƀh���b�v�_�E���̏����ɉ����Ăǂ̑����o����
    //�ʒu�ݒ�͌Ăяo���ꂽ���������Ă鐔�l�Ŏ����ł��܂��B
    //�h���b�v�_�E���̈ʒu�ݒ�̓h���b�v�_�E�����O�̏��������ăE�B���h�E�������𔻒f���Ď��������܂��B
    //�Z�b�e�B���O�����Ă�h���b�v�_�E������������
    //���C���̃h���b�v�_�E�����ǂ�����ݒ肷��
    //���̃X�N���v�g��UI�v�f�̃��[�g�I�u�W�F�N�g�ɒu��
    //����Ⴆ�ΎO�Ԗڂœ�Ԗڂ̗v�f���ς���ă��C�A�E�g�ς�������̂��Ƃ��l���Ȃ��Ⴖ���

    //�o�b�N�X�y�[�X�Ŗ߂�
    //�ݒ�R�[�h�ƌp���R�[�h����ޏ���
    //�p���R�[�h�ł͕ύX�̍ۉ��߂ĕʂ�UI�o�����肷��B
    //�ݒ�ł͂����ݒ肷�邾��
    //���Ȃ݂Ɍp���R�[�h�ɂ��ݒ肷�邱�Ƃ����ȁH
    //���ꂷ�邽�߂�ref�Ō���ς��邩
    //���Ȃ݂Ɍp���R�[�h����SelectCon��Change���Ă�
    //�p���R�[�h�ŃZ�b�e�B���O�ԍ��ƌ��݂�UI���Ƃ����炳��Ɍp���h���b�v�_�E�����o�����Ƃ����߂�B
    //��̔��f�Ɋ�Â��ĕ\�����镶���Ƃ������߂�
    //�ݒ肷��I�u�W�F�N�g�ɓn��������Ƃ���
    //���߂�Ώۂ��đ�̌Œ肾����Ref�g���đ�{�������炢���񂶂��
    //�Z�b�e�B���O�ԍ��ŏ�肢���Ƃ���ł���
    //���ƕ\�����Ă�UI�̃��[�g�I�u�W�F�N�g�܂Ƃ߃��X�g�Ƃ��A�Z�b�e�B���O�ԍ��Ƃ��Ɋ�Â����f��ۑ��ł���ꏊ�͕K�v����

    /// <summary>
    /// �O�̗v�f�i�����Ă��h���b�v�_�E���Ƃ����܂��Ă�j���炿�傤�ǂ����ʒu�ɔz�u���邽�߂̐��l
    /// </summary>
    public float addjustPosition;

    /// <summary>
    /// ���̃^�C�v
    /// �n���p�h���b�v�_�E�����A����Ƃ��G�^�C�v�ݒ肷�鑋�Ȃ̂��Ƃ�
    /// �������p���p�h���b�v�_�E���ł���ꍇ�͑O�̑��́iSelectList����UIPlaceSet�Ŏ擾�j�^�C�v�ɂ�蒲��������ς���
    /// 0�͊J�n�h���b�v�_�E���A�P�͌p���p�h���b�v�_�E���A2�͐ݒ�p�h���b�v�_�E���A3�͐��l��Bool�i�n�C���[�j�A4�͒����`�F�b�N�{�b�N�X�A5�͒Z���`�F�b�N�{�b�N�X
    /// 
    /// </summary>
    public int windowType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlaceSet()
    {
        if(windowType == 0)
        {

        }
        else if (windowType == 1)
        {
            //SelectList��Add�����̂�Update����

            int number = MainUI.instance.selectList[MainUI.instance.selectList.Count - 1].gameObject.GetComponent<UIPlaceSet>().windowType;

            if(number == 2)
            {
                addjustPosition = 0;
            }
            else if (number == 3)
            {
                addjustPosition = 0;
            }
            else if (number == 4)
            {
                addjustPosition = 0;
            }
            else if (number == 5)
            {
                addjustPosition = 0;
            }

            //�����ɑO��UI�̈ʒu����addjust����悤���Ă��

        }
    }

    /// <summary>
    /// �ŏ��̃h���b�v�_�E�����I��������B
    /// �o�b�N�X�y�[�X�ōs����
    /// </summary>
    public void UIEnd()
    {
        MainUI.instance.selectList = new List<Selectable>();
        if(MainUI.instance.settingNumber % 2 == 1)
        {
            MainUI.instance.settingNumber--;
        }
        this.gameObject.SetActive(false);
    }

}
