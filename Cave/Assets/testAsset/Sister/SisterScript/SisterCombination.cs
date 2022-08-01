using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SisterCombination", menuName = "SisterCombinationCreate")]

�@//�݌v
�@//�^�[�Q�b�g���邩�ǂ������f
  //�i�����ĘA���؂��܂Ŏ��s
  //�؂ꂽ��i�����Z�b�g���ăN�[���^�C���v���J�n
  //�i�������g���ƃN�[���^�C���L�т��肷��H����u���������邩���߂悤���ȁB�L�т�ɂ���ق�̂킸���ł���
  //�i���ɂ��Ă͂ނ�������z�񂩃��X�g��Count�ł͂���΂����񂶂��
  //�R���r�l�[�V�����^�C�v�ɂ��Ă͐ڒn�n�Ƃ����������̂ōĎv�l
  //�T�u�ƃ��C���̏������X�g�͒i���ŏ�����
  //�ǂ̓v���C���[���猩�ď�����ɗ���悤�ɒ����H
  //�N�[���^�C����҂����ɘA�g�����MP�g���悤�ɂ���H

public class SisterCombination : Magic
{

    /* [HideInInspector]
     public enum CombinationType
     {
         Attack,//�U��
         Recover,//��
         Support,//�x��
         help//�����ŏ���ɔ����H
     }
     public CombinationType cType;
    */

    [Header("�v���C���[�ȊO�̕W�I��ݒ肷�邩�ǂ���")]
    public bool isTargeting;
    [Header("����܂łȂ��邩")]
    public int chainNumber = 1;
    [Header("�N�[���^�C��")]
    public float coolTime;

    [Header("���C���̓G�I������")]
    public List<AttackJudge> mainTarget;//���f�����Z�b�g

    [Header("�T�u�̓G�I������")]
    public List<AttackJudge> subTarget;//���f�����Z�b�g

    [Header("�����Ȃ��ƂɎg���^�U")]
    bool isUtility;

    [Header("�n���^�C�v")]
    public ActType _combiType;

    public enum ActType
    {
        soon = 1,
        cast = 2,
        longPress = 3,
        castAndPress = 4
    }

    //�����Ƃ��Ă͋��G�A��s�^�C�v�̓G�c����N�[���^�C�����邵�U�����݂ɍׂ����Ă�������
    //�����ׂ̍����Őݒ�
    //���[�v�͕ǂɖ��܂�Ȃ��悤�ɋC��t������
    //�ǂ͐�̒n�ʂɌ����Ƃ��������㑫��ɂ��ł��邩�B�ւ��񂾒n�`�p�ӂ���
    //���Ⴀ��ԏ��Ground�^�O�t���邩
    //�{�^���������ŕǂ̈ʒu���ς��B�ǂ̈ʒu�ǂ��ɂȂ邩�̓K�C�h�\�������
    //���C�L���X�g�Œn�ʒT��X�N���v�g���p
    //�ŏ��̏ꏊ���N�_�Ɉړ�����
    //�Ȃ�W�I�Ƃ�����Ȃ��ˁH�B�킩���





}
