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
    /// </summary>
    public int windowType;

    //��{�I�Ɋe�X�N���v�g��saveNumber�����ϕۑ�����B

    //�Z�b�b�e�B���O�i���o�[�͘Z���
    //�U�������ݒ�͂S�t�F�[�Y�A��ڂ��h���b�v�_�E���i�m��j�A��ڂ͐F�X�i�G�^�C�v���A���l�ݒ�A�����Ȃ��ŃX�L�b�v�j�A�O�ڂ͎�_�h���b�v�_�E��
    //�A�l�ڂ̓h���b�v�_�E���iUpDown����j�B�m�葤�͑S���^
    //��ڂ̃h���b�v�_�E���Ŏc��̑������ׂďo���B�w��Ȃ��ȊO
    //�U���I����3�t�F�[�Y�A�S���h���b�v�_�E���B�Ō�̃h���b�v�_�E����UpDown����B��ڂ̃h���b�v�_�E���Ŏc��̑������ׂďo���B�w��Ȃ��ƈڍs�ȊO
    //�x��������2�t�F�[�Y�A��ڃh���b�v�_�E���A��ڂ͐��l�ݒ肩�G�`�F�b�N���K�v�Ȏx����I�񂾂�����߂�h���b�v�_�E����
    //��ڂ̃h���b�v�_�E���Ŏc��̑����o���B�w��Ȃ��̎��ȊO
    //�x���I���͂R�t�F�[�Y�A��ڃh���b�v�_�E���A��ڃh���b�v�_�E���A�O�ڂ��h���b�v�_�E���iUpDown����j�B�w��Ȃ��ȊO�c�肪�o�Ă���B
    //�񕜏�����2�t�F�[�Y�A��ڃh���b�v�_�E���A��ڂ͐��l�ݒ肩�G�`�F�b�N���K�v�Ȏx����I�񂾂苭�G�����邩�����߂�h���b�v�_�E����
    //�񕜑I���͂R�t�F�[�Y�A��ڃh���b�v�_�E���A��ڃh���b�v�_�E���A�O�ڂ��h���b�v�_�E���iUpDown����j


    //�t�@�[�X�g�h���b�v�A�Z�J���h�h���b�v�Ȃǂ̃h���b�v�_�E���̐؂�ւ��̓Z�b�g�i���o�[�ōs���B

    //�U������
    //�t�@�[�X�g�h���b�v�i��b�����j���l�ݒ�i���G�A��Ԉُ�A�x���A�Ȃ��̎��͂Ȃ��j���Z�J���h�h���b�v�i��_�j���T�[�h�h���b�v�i�W�I�����j
    //�U���I���p�^�[��
    //�t�@�[�X�g�h���b�v�i�I���s���A�����j�i�Ȃɂ����Ȃ��ƈڍs�Ō�̑���������j���Z�J���h�h���b�v�i�g�p����U���̏����ݒ�j���T�[�h�h���b�v�i����ɏ����j
    //�x�������p�^�[��
    //�t�@�[�X�g�h���b�v�i��b�����j���l�ݒ�i���G�A�Ȃ��̎��͂Ȃ��j
    //�x���I���p�^�[��
    //�t�@�[�X�g�h���b�v�i�s���I���j�i�Ȃɂ����Ȃ��ƈڍs�Ō�̑���������j���Z�J���h�h���b�v�i�ǂ�ȃT�|�[�g���������̂�I�Ԃ��j���T�[�h�h���b�v�i�ǂ�ȏ����Ŗ��@�I�Ԃ��j

    //�I�𐧌�X�N���v�g�ɕK�v�ȋ@�\
    //�Z�b�b�e�B���O�i���o�[���ăh���b�v�_�E����L��������؂�ւ���
    //���@�I���͂��łɌ����݌��肵�Ă�B
    //����ɖ��@�I���ł̃��C�A�E�g�ύX�̓E�B���h�E�̏����񕜂���
    //���Ȃ̂͊e�X�e�[�g�̏����ݒ�
    //�l�ݒ�̑O�ゾ��Selectable�����Ȃ����A����Ƀ��C�A�E�g�̕ύX������B
    //�ł�������������f�Ő؂�ւ���ꂻ���Ȃ񂾂�Ȃ�
    //�o�Ă��鑋�͍ő�l�B�z�u�����̎��_�ňȉ��S���z�u���Ȃ���



    //���낢����荬����͎̂O���������i�U�������̂Q�A�x���񕜏����̎O�ځj

    //�S�ď����Ƃ���EditNum�Ŗ߂�΂����B
    //�h���b�v�_�E���͍ő�O�K�v

    // Start is called before the first frame update

    RectTransform myPosi;

    void Start()
    {
        myPosi = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        PlaceSet();
    }
    public void PlaceSet()
    {
        if(windowType == 0)
        {
            //�h���b�v�_�E��
            if(MainUI.instance.beforeSet == null)
            {
                //����̏ꏊ�ɃZ�b�g
              //  RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
                Vector2 posi = new Vector2(425, 300);
                myPosi.anchoredPosition = posi;
            }
            else
            {
                UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
                RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();

                if (before.windowType == 0)
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 125);
                    myPosi.anchoredPosition = posi;
                    //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                    //125���������ʒu�ɓ����
                }
                else if (before.windowType == 1)
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 130);
                    myPosi.anchoredPosition = posi;
                    //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                    //130���������ʒu�ɓ����
                }
                else
                {
                    Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 162);
                    myPosi.anchoredPosition = posi;
                    //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                    //162�������ꏊ�ɒu��
                }
            }
        }
        else if (windowType == 1)
        {
            RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
            Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 130);
            myPosi.anchoredPosition = posi;
#if false
            //�G�^�C�v
            UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
            if (before.windowType == 0)
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //130���������ʒu�ɓ����
            }
            else if (before.windowType == 1)
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //�Ȃ��\�������B��ڂ̏��������ŃI�[�P�[
            }
            else
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //�Ȃ��\�������B��ڂ̏��������ŃI�[�P�[
            }
#endif
        }
        else
        {
            RectTransform bt = MainUI.instance.beforeSet.GetComponent<RectTransform>();
            Vector2 posi = new Vector2(myPosi.anchoredPosition.x, bt.anchoredPosition.y - 162);
             myPosi.anchoredPosition = posi;
            #if false
            //���^�C�v�Q�A���l�ݒ�
            UIPlaceSet before = MainUI.instance.beforeSet.GetComponent<UIPlaceSet>();
            if (before.windowType == 0)
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //162���������ʒu�ɓ����B
            }
            else if (before.windowType == 1)
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //�Ȃ��\�������B��ڂ̏��������ŃI�[�P�[
            }
            else
            {
                //�K��l��O�̃I�u�W�F�N�g�̈ʒu�ɑ������ꏊ�ɓ����
                //�Ȃ��\�������B��ڂ̏��������ŃI�[�P�[
            }
#endif
        }
        //����ւ�
        MainUI.instance.beforeSet = this.gameObject;
    }



}
