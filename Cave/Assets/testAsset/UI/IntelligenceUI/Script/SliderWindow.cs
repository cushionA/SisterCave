using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderWindow : ValueChangeBase
{
    //rewiredでキャンセル入ったらWindowを消す
    //そして設定窓のVolumeボタンをSelectする
    //あとスライダーいじった後？選択されている間？
    //それともスライダーのボタンイベントでスライダーの数値をボリュームにかける？
    //100~0でいこう
    [SerializeField] Button bgmButton;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Text tx;
    [SerializeField] Toggle mine;



    bool isDown;
    bool wait;
    bool charge;//長押し完了してるかどうか。完了してたらぱっぱと変わる
    bool isUp;

    float changeTime;

    bool isFirst;

    float firstValue;
    // Update is called once per frame
    void Update()
    {

        


        if (!isFirst)
        {

            tx.text = bgmSlider.value.ToString();
            firstValue = bgmSlider.value;
            boolSave = mine.isOn;
            isFirst = true;
        }


            ////Debug.log("開始");
            if (bgmSlider.gameObject == MainUICon.instance.selectButton)
            {
                ////Debug.log("bgm調整中");

                //決定ボタン
                if (MainUICon.instance._reInput.UIMovement.y > 0 && !isUp)
                {
                    ////Debug.log("a");
                    bgmSlider.value += 10;
                    isUp = true;
                    isDown = false;
                    //tx.text = bgmSlider.value.ToString();
                }
                else if (MainUICon.instance._reInput.UIMovement.y < 0 && !isDown)
                {
                    bgmSlider.value -= 10;
                   // tx.text = bgmSlider.value.ToString();
                    isUp = false;
                    isDown = true;
                }
            if (MainUICon.instance._reInput.UIMovement.y != 0)
            {
                //    ////Debug.log("しののめの");
                //changeTime += Time.realtimeSinceStartup;
                if (!charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;//TimeScaleのせい
                                                               //ChangeTimeに現在時間を代入してそこから時間数えて待つ
                        wait = true;
                        //待ち始めます

                    }
                    if (Time.realtimeSinceStartup - changeTime >= 0.5)
                    {
                        //       ////Debug.log("長押し清算");
                        isUp = false;
                        isDown = false;
                        wait = false;
                        changeTime = 0.0f;
                        charge = true;
                        //Debug.log("起動");
                    }
                }
                else if (charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;
                        wait = true;
                        //待ち始めるので時間数えません
                    }

                    //Time.realtimeSinceStartup - changeTime
                    if (Time.realtimeSinceStartup - changeTime >= 0.1)
                    {
                        //       ////Debug.log("長押し清算");
                        //  isChange = false;
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
                //  ////Debug.log("ボタン離し清算");
                // isChange = false;
                changeTime = 0.0f;
                wait = false;
                charge = false;
            }
        }


        
    }

    public void VChange()
    {
        //決定ボタン

        bgmSlider.value = firstValue;

        isFirst = false;
        bgmButton.Select();



    }

    public void VEnd()
    {
        numberSave = (int)bgmSlider.value;
        base.ApplyValue();
        bgmButton.Select();
        isFirst = false;

        EditEnd();
    }

    public void ToggleChange()
    {
        boolSave = mine.isOn;
        base.ApplyValue();
    }

    /// <summary>
    /// BGMの設定をいじるボタンを押したとき
    /// </summary>
    public void BGMSelect()
    {
         
        //     bgmButton.enabled = false;
        bgmSlider.Select();


    }

    public void txChange()
    {
        tx.text = bgmSlider.value.ToString();
    }

}
