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


    //�������悤�B
    //�܂�������g���͍̂U�������̉��A�x���񕜏����̏㉺�A�^�C�v�ƃX���C�_�[�̏㉺�ɂȂ�B
    //pre�Ƃ��l�N�X�g�Ƃ�������ChildWindow��Use�i���o�[�����ĉ��Ԃ̃I�u�W�F�N�g���X�g�𗘗p���Ă邩�𓾂邩
    //�I�u�W�F�N�g�𓾂���@��S�Ƃ���
    //�Ă�Use�i���o�[�Ȃ��Ă�S�Ƃ��̎��Z�J���h�E�B���h�E�Ƃ��ŉ��g���Ă邩�킩����
    //
    //s��1�̃t�@�[�X�g�E�B���h�E�̉��ƃ^�C�v�ƃX���C�_�[�̏㉺�A�Z�J���h�͐ڑ��ΏۂƂ��Ă������݁B
    //�x���񕜏����̎��A�t�@�[�X�g�E�B���h�E�̉��ƃ^�C�v�ƃX���C�_�[�̏㉺�A�X���C�_�[��^�C�v�̉��̓t�@�[�X�g�ɂȂ�
    //
    //����𗘗p���ăR�[�h������
    //�l�����ׂ���S��1,3,5����
    //

    //  [SerializeField] bool isUpSet;


    /// <summary>
    /// �����ɐݒ肷��I�u�W�F�N�g�����B����ݒ�
    /// </summary>
    [SerializeField] List<Selectable> raidObject;



    /// <summary>
    /// �����ݒ�ɖ߂��̂Ɏg��
    /// </summary>
   // Navigation mine;
    Selectable me;

    /// <summary>
    /// value���p
    /// </summary>
     [SerializeField]
    bool isValue;




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
            int s = MainUI.instance.settingNumber;
            //int e = MainUI.instance.editNumber;
            Selectable sl;
            int num;

            #region
            if (s == 1)
            {
                num = 0;
            }
            else if(s == 2)
            {
                num = 1;
            }
            else if (s == 3 || s == 5)
            {
                num = 4;
            }
            else if (s == 4)
            {
                num = 2;
            }
            else
            {
                num = 3;
            }
            #endregion

            if (MainUI.instance.isChange == 1)
            {
                 sl = MainUI.instance.firstDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    UpSet(sl);
               MainUI.instance.isChange = 2;
            }
            else
            {
                    if (s == 1)
                    {
                        sl = MainUI.instance.secondDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    }
                    else
                    {
                        sl = MainUI.instance.firstDrop.GetComponent<ChildWindow>().objList[num].GetComponent<Selectable>();
                    }
                if (!isValue)
                {
                        UnderSet(sl);
                }
                else
                {
                    AnotherSet(sl);
                }

                MainUI.instance.isChange = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UnderSet(Selectable nextObject)
    {
        Navigation navi = me.navigation;
        navi.selectOnDown = nextObject;
        me.navigation = navi;
        navi = nextObject.navigation;
        navi.selectOnUp = me;
        nextObject.navigation = navi;

        if (raidObject.Count > 0 && !isValue)
        {
        //    Debug.Log("������");
            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i <= raidObject.Count - 1; i++)
            {
                 navi = raidObject[i].navigation;
                navi.selectOnDown = nextObject;
                raidObject[i].navigation = navi;
            }
        }
    }
    /// <summary>
    /// ��̃I�u�W�F�N�g�ݒ�B
    /// </summary>
    /// <param name="raid"></param>
    public void UpSet(Selectable preObject)
    {
        Navigation navi = me.navigation;
        navi.selectOnUp = preObject;
        me.navigation = navi;
        navi = preObject.navigation;
        navi.selectOnDown = me;
        preObject.navigation = navi;

        if (raidObject.Count > 0 && !isValue)
        {

            //   navi.selectOnDown = raid;
            //Navigation navi;
            for (int i = 0; i <= raidObject.Count - 1; i++)
            {
                navi = raidObject[i].navigation;
                navi.selectOnUp = preObject;
                raidObject[i].navigation = navi;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void AnotherSet(Selectable nextObject)
    {
        

        Navigation navi = raidObject[0].navigation;
        navi.selectOnDown = nextObject;
        raidObject[0].navigation = navi;
        navi = nextObject.navigation;
        navi.selectOnUp = raidObject[0];
        nextObject.navigation = navi;
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
