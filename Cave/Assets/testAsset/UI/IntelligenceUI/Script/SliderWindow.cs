using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderWindow : ValueChangeBase
{
    //rewired�ŃL�����Z����������Window������
    //�����Đݒ葋��Volume�{�^����Select����
    //���ƃX���C�_�[����������H�I������Ă���ԁH
    //����Ƃ��X���C�_�[�̃{�^���C�x���g�ŃX���C�_�[�̐��l���{�����[���ɂ�����H
    //100~0�ł�����
    [SerializeField] Button bgmButton;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Text tx;
    [SerializeField] Toggle mine;

    bool isChange;

    bool isDown;
    bool wait;
    bool charge;//�������������Ă邩�ǂ����B�������Ă���ς��ςƕς��
    bool isUp;
    float verticalKey;
    float changeTime;

    bool isFirst;

    float firstValue;
    // Update is called once per frame
    void Update()
    {

        verticalKey = GManager.instance.InputR.GetAxisRaw(MainUI.instance.rewiredAction15);


        if (!isFirst)
        {

            tx.text = bgmSlider.value.ToString();
            firstValue = bgmSlider.value;
            boolSave = mine.isOn;
            isFirst = true;
        }

       if (isChange)
        {
            ////Debug.log("�J�n");
            if (bgmSlider.gameObject == MainUI.instance.eventSystem.currentSelectedGameObject)
            {
                ////Debug.log("bgm������");

                //����{�^��
                if (verticalKey > 0 && !isUp)
                {
                    ////Debug.log("a");
                    bgmSlider.value += 10;
                    isUp = true;
                    isDown = false;
                    //tx.text = bgmSlider.value.ToString();
                }
                else if (verticalKey < 0 && !isDown)
                {
                    bgmSlider.value -= 10;
                   // tx.text = bgmSlider.value.ToString();
                    isUp = false;
                    isDown = true;
                }
            }
            if (verticalKey != 0)
            {
                //    ////Debug.log("���̂̂߂�");
                //changeTime += Time.realtimeSinceStartup;
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
                        //       ////Debug.log("���������Z");
                        //  isChange = false;
                        wait = false;
                        changeTime = 0.0f;
                        isUp = false;
                        isDown = false;
                    }
                }
            }
            else if (verticalKey == 0)
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

    public void VChange()
    {
        //����{�^��


        //DarkTonic.GetBusVolume();
        // bgmButton.enabled = true;
        bgmSlider.value = firstValue;
        //txChange();
        isFirst = false;
        bgmButton.Select();
            isChange = false;
        ////Debug.log("b");
        //�������ł͋t�ɉ��ʂ��X���C�_�[�̐��l�ɓ����
        //   EditEnd();
        Invoke("EditEnd", 0.05f);
    }

    public void VEnd()
    {
        numberSave = (int)bgmSlider.value;
        //base.ApplyValue();
        bgmButton.Select();
        isFirst = false;
        isChange = false;
       Invoke("EditEnd",0.05f);
    }

    public void ToggleChange()
    {
        boolSave = mine.isOn;
        base.ApplyValue();
    }

    /// <summary>
    /// BGM�̐ݒ��������{�^�����������Ƃ�
    /// </summary>
    public void BGMSelect()
    {
         
        //     bgmButton.enabled = false;
        bgmSlider.Select();
        isChange = true;

    }

    public void txChange()
    {
        tx.text = bgmSlider.value.ToString();
    }

}