using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectControll : MonoBehaviour
{

    //�K�v�ȃ`�F�b�N�{�b�N�X
    //�����W�A�G�^�C�v�S�Cbool�n�A���������Ȃ����̈�����Bbool�����̓h���b�v�_�E���ɂ���B
    //�K�v�ȃT�|�[�g�̓h���b�v�_�E���ɂ���

    //�ƂȂ�Ɛݒ�E�B���h�E�̃^�C�v�̓h���b�v�_�E����ƁA���l��bool�A�����`�F�b�N�{�b�N�X�ƒZ���`�F�b�N�{�b�N�X�̎l��ނɂȂ�B
    //����������ʉ�����̂ɕK�v�Ȃ͎̂Q�Ɛ�Ɖ���ݒ肷�邩�̃^�C�g���ƁA

    //�����ɔԍ������Ĕԍ��ɍ������X�g�Ɏ���������BSelectable�ł�����Q�Ƃ���B
    //��Ɖ��A�ǂ�����ݒ肷�邩���Ȃ�����I�ׂ�B
    //�eSelectable�ȗv�f�ɓ����B�{�^���Ƃ��g�O���Ƃ�



    //�������A�ύX���ɂ��@�\
    //�Q�Ɛ�̌^��n�����炻�̗v�f�����\�b�h�̈����ɂ��Ă�������������HHoge(UI3.percentage);�݂�����
    //�P��int��bool�������ɂ��āA�Z�b�b�e�B���O�i���o�[�Ō^�O���̃��\�b�h�̍�����݂���
    //���̌^�ŉ��Ԗڂ̏�����ҏW���Ă�̂ŁA���̐��l��ς��܂��Ƃ���΂悭�ˁH
    //�ق�Ȃ��������Ȃ����I�I


  //  [SerializeField] bool isUpSet;

    /// <summary>
    /// False�Ȃ牺
    /// </summary>
    [SerializeField] bool isUp;
    /// <summary>
    /// �����ɐݒ肷��I�u�W�F�N�g�����B����ݒ�
    /// </summary>
    [SerializeField] List<Navigation> raidObject;



    /// <summary>
    /// �����ݒ�ɖ߂��̂Ɏg��
    /// </summary>
   // Navigation mine;
    Selectable me;






  //  public bool isChange;

    // Start is called before the first frame update
    void Start()
    {
        
       // mine = GetComponent<Navigation>();
        me = GetComponent<Selectable>();
   //     mine = me.navigation;
        //ChangeItem();

    }

    // Update is called once per frame
    void Update()
    {
        /*     if (done)
             {
                 done = false;
                 MainUI.instance.selectList.Add(me);
             }*/
        //��{�I�ɒl�ݒ葋�͈�Ȃ̂�isChange�����ɂ������Ȃ�

        if (MainUI.instance.isChange > 0) 
        {
            if (MainUI.instance.isChange == 1)
            {
                UpSet();
                MainUI.instance.isChange = 2;
            }
            else
            {
                UnderSet();
                MainUI.instance.isChange = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnderSet()
    {
        Navigation navi = me.navigation;
        navi.selectOnDown = MainUI.instance.nextObject;
        me.navigation = navi;
        navi = MainUI.instance.nextObject.navigation;
        navi.selectOnUp = me;
        MainUI.instance.nextObject.navigation = navi;

        if (raidObject.Count > 0)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i >= raidObject.Count; i++)
            {
                 navi = raidObject[i];
                navi.selectOnDown = MainUI.instance.nextObject;
                raidObject[i] = navi;
            }
        }
    }
    /// <summary>
    /// ��̃I�u�W�F�N�g�ݒ�B
    /// </summary>
    /// <param name="raid"></param>
    public void UpSet()
    {
        Navigation navi = me.navigation;
        navi.selectOnUp = MainUI.instance.preObject;
        me.navigation = navi;
        navi = MainUI.instance.preObject.navigation;
        navi.selectOnDown = me;
        MainUI.instance.preObject.navigation = navi;

        if (raidObject.Count > 0)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i >= raidObject.Count; i++)
            {
                navi = raidObject[i];
                navi.selectOnUp = MainUI.instance.preObject;
                raidObject[i] = navi;
            }
        }
    }

 /*   public void SelectReset(Selectable change)
    {

        MainUI.instance.changeTarget = me;
         me.navigation = mine;
    }*/

  /*  public void ChangeItem()
    {
        if (MainUI.instance.isChange && isMaster && mine.selectOnUp == null)
        {
            for (int i = 0; i >= MainUI.instance.selectList.Count; i++)
            {
                //�V������Ɠ���ւ��B
                if (MainUI.instance.selectList[i] == MainUI.instance.changeTarget)
                {
                    MainUI.instance.selectList[i] = me;
                    i = 100;
                }

            }
            MainUI.instance.isChange = false;
        }
    }*/

}
