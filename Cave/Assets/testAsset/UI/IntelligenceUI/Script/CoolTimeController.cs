using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeController : MonoBehaviour
{

    [SerializeField]
    Slider _coolSlider;

    [SerializeField]
    Button _coolButton;

    [SerializeField]
    Toggle[] _operationToggle = new Toggle[5];

    int s;
    int e;


    /// ���A1000000
    //  ���A0100000
    //�@��O�A0010000
    //�@��l�A00010000



    [SerializeField] Text tx;



    bool isDown;
    bool wait;
    bool charge;//�������������Ă邩�ǂ����B�������Ă���ς��ςƕς��
    bool isUp;

    float changeTime;

    bool isFirst;

    float firstValue;

    /// <summary>
    /// �U���A�T�|�[�g�A�񕜂̏���
    /// 0~3�܂ł̓g�O���ȍ~�̓h���b�v
    /// </summary>
    [SerializeField]
    Selectable[] _naviObject;

    private void OnEnable()
    {
        s = MainUICon.instance.settingNumber;
        e = MainUICon.instance.editNumber;

        _coolSlider.value = GetCoolTime();
        txChange();
        SetToggle(GetSkipOption());
        SetNavigation();
    }

    void Update()
    {
        if (!isFirst)
        {
            tx.text = _coolSlider.value.ToString();
            firstValue = _coolSlider.value;
            isFirst = true;
        }


        ////Debug.log("�J�n");
        if (_coolSlider.gameObject == MainUICon.instance.selectButton)
        {
            ////Debug.log("bgm������");

            //����{�^��
            if (MainUICon.instance._reInput.UIMovement.y > 0 && !isUp)
            {
                ////Debug.log("a");
                _coolSlider.value += 10;
                isUp = true;
                isDown = false;
            }
            else if (MainUICon.instance._reInput.UIMovement.y < 0 && !isDown)
            {
                _coolSlider.value -= 10;
                isUp = false;
                isDown = true;
            }
            if (MainUICon.instance._reInput.UIMovement.y != 0)
            {

                if (!charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;//TimeScale�̂���
                                                               //ChangeTime�Ɍ��ݎ��Ԃ������Ă������玞�Ԑ����đ҂�
                        wait = true;
                        //�҂��n�߂܂�

                    }
                    if (Time.realtimeSinceStartup - changeTime >= 0.5)
                    {
                        //       ////Debug.log("���������Z");
                        isUp = false;
                        isDown = false;
                        wait = false;
                        changeTime = 0.0f;
                        charge = true;
                        //Debug.log("�N��");
                    }
                }
                else if (charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;
                        wait = true;
                        //�҂��n�߂�̂Ŏ��Ԑ����܂���
                    }

                    //Time.realtimeSinceStartup - changeTime
                    if (Time.realtimeSinceStartup - changeTime >= 0.1)
                    {

                        wait = false;
                        changeTime = 0.0f;
                        isUp = false;
                        isDown = false;
                    }
                }


            }
            else if (MainUICon.instance._reInput.UIMovement.y == 0)
            {
                isUp = false;
                isDown = false;
                //  ////Debug.log("�{�^���������Z");
                // isChange = false;
                changeTime = 0.0f;
                wait = false;
                charge = false;
            }

        }

    }

    /// <summary>
    /// ���l����֘A
    /// </summary>
    /// <returns></returns>
    #region
    float GetCoolTime()
    {
        if (s == 2)
        {
            return MainUICon.instance.editParameter.attackCT[e];
        }
        else if (s == 4)
        {
            return MainUICon.instance.editParameter.supportCT[e];
        }
        else
        {
            if (!MainUICon.instance.isAH)
            {
                return MainUICon.instance.editParameter.healCT[e];
            }
            else
            {
                return MainUICon.instance.editParameter.autHealCT[e];
            }
        }

    }

    int GetSkipOption()
    {
        if (s == 2)
        {
            return MainUICon.instance.editParameter.atSkipList[e];
        }
        else if (s == 4)
        {
            return MainUICon.instance.editParameter.sSkipList[e];
        }
        else
        {
            if (!MainUICon.instance.isAH)
            {
                return MainUICon.instance.editParameter.hSkipList[e];
            }
            else
            {
                return MainUICon.instance.editParameter.ahSkipList[e];
            }
        }
    }

    /// <summary>
    /// �ŏ��Ƀg�O���̕\�������߂���
    /// </summary>
    /// <param name="i"></param>
    void SetToggle(int j)
    {
       // Debug.Log($"�X�L�b�v�ԍ�{GetSkipOption()}");
        for (int i = 0; i < _operationToggle.Length; i++)
        {
            if (j != 0)
            {
                int d = (int)Mathf.Pow(2, i);
                Debug.Log($"{j}{d}{j & d}");
                if ((j & d) == d)
                {
                    _operationToggle[i].isOn = true;
                }
                else
                {
                    _operationToggle[i].isOn = false;
                }
            }
            else
            {
                _operationToggle[i].isOn = false;
            }
        }
    }

    /// <summary>
    /// ����{�^���ōs�����
    /// </summary>
     void SetCoolTime()
    {

        if (s == 2)
        {
           MainUICon.instance.editParameter.attackCT[e] = _coolSlider.value;
        }
        else if (s == 4)
        {
            MainUICon.instance.editParameter.supportCT[e] = _coolSlider.value;
        }
        else
        {
            if (!MainUICon.instance.isAH)
            {
                MainUICon.instance.editParameter.healCT[e] = _coolSlider.value;
            }
            else
            {
                MainUICon.instance.editParameter.autHealCT[e] = _coolSlider.value;
            }
        }
    }

    /// <summary>
    /// �g�O�����ω����邽�тɂ��
    /// </summary>
    public void SetSkipList()
    {
        int skip = 0;

        for (int i = 0;i < _operationToggle.Length;i++)
        {
            if(_operationToggle[i].isOn == true)
            {
                skip += (int)Mathf.Pow(2,i);
            }
        }
        if (s == 2)
        {
            MainUICon.instance.editParameter.atSkipList[e] = skip;
        }
        else if (s == 4)
        {
            MainUICon.instance.editParameter.sSkipList[e] = skip;
        }
        else
        {
            if (!MainUICon.instance.isAH)
            {
                MainUICon.instance.editParameter.hSkipList[e] = skip;
            }
            else
            {
                MainUICon.instance.editParameter.ahSkipList[e] = skip;
            }
        }

    }

    /// <summary>
    /// �i�r�Q�[�V������ݒ肷��
    /// </summary>
    void SetNavigation()
    {
        Selectable dropDisti;
        Selectable toggleDisti;
        if (s == 2)
        {
            toggleDisti = _naviObject[0];
            dropDisti = _naviObject[3];
        }
        else if(s == 4)
        {
            toggleDisti = _naviObject[1];
            dropDisti = _naviObject[4];
        }
        else
        {
            toggleDisti = _naviObject[2];
            dropDisti = _naviObject[5];
        }

        Navigation navi = _coolButton.navigation;
        navi.selectOnUp = dropDisti;
        _coolButton.navigation = navi;
        for (int i = 0;i < _operationToggle.Length; i++)
        {
            navi = _operationToggle[i].navigation;
            navi.selectOnDown = toggleDisti;
            _operationToggle[i].navigation = navi;
        }
    }

    #endregion

    //�X���C�_�[����֘A
    #region

    /// <summary>
    /// �l��K�p����
    /// </summary>
    public void VChange()
    {
        //����{�^��
        SetCoolTime();

        isFirst = false;
        _coolButton.Select();

    }

    /// <summary>
    /// �r���ŕҏW��߂���
    /// ���l�����Ƃɖ߂�
    /// </summary>
    public void VEnd()
    {
        _coolSlider.value = firstValue;
        _coolButton.Select();
        isFirst = false;

    }



    /// <summary>
    /// �X���C�_�[�̐ݒ��������{�^�����������Ƃ�
    /// �ŏ��̃��\�b�h
    /// </summary>
    public void SliderSelect()
    {
        _coolSlider.Select();


    }

    //���l�ύX�̎��ɌĂ�
    public void txChange()
    {
        tx.text = _coolSlider.value.ToString();
    }

    #endregion



}
