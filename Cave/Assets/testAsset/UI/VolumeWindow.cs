using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeWindow : MonoBehaviour
{

    //rewiredでキャンセル入ったらWindowを消す
    //そして設定窓のVolumeボタンをSelectする
    //あとスライダーいじった後？選択されている間？
    //それともスライダーのボタンイベントでスライダーの数値をボリュームにかける？
    //100~0でいこう
    [SerializeField] Button bgmButton;
    [SerializeField] Button seButton;

    //設定ウィンドウの方の戻るボタン
    [SerializeField] Button sButton;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;

    bool isChange;
    GameObject selectObject;
    
    bool isDown;
    bool wait;
    bool charge;//長押し完了してるかどうか。完了してたらぱっぱと変わる
    bool isFirst;//最初に音量設定開いたかどうか
    bool isBlock;
    bool isUp;

    float changeTime;

    // Start is called before the first frame update
    void Start()
    {
        bgmSlider.value = 50;
        seSlider.value = 50;
    }

    // Update is called once per frame
    void Update()
    {
 

        selectObject = MainUICon.instance.eventSystem.currentSelectedGameObject;


        if (!isFirst)
        {
            bgmSlider.value = DarkTonic.MasterAudio.MasterAudio.GrabBusByName("BGM").volume * 100;
            seSlider.value = DarkTonic.MasterAudio.MasterAudio.GrabBusByName("SE").volume * 100;

            isFirst = true;
        }

        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction14))
        {
            isChange = false;
            isBlock = false;
            wait = false;
            charge = false;
            isDown = false;
            isUp = false;
            isFirst = false;
            changeTime = 0.0f;
            this.gameObject.SetActive(false);
            //sButton.Select();
            MainUICon.instance.MenuCancel();
        }



            if (!isChange && MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
        {
            MainUICon.instance.masterUI.SetActive(true);
            this.gameObject.SetActive(false);
            isFirst = false;
            sButton.Select();

        }
        else if (isChange) 
        {
            ////Debug.log("開始");
            if (bgmSlider.gameObject == selectObject)
            {
                ////Debug.log("bgm調整中");

                //決定ボタン
                if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
                {
                    
                    bgmSlider.value = MasterAudio.GrabBusByName("BGM").volume * 100;
                    //DarkTonic.MasterAudio.MasterAudio.GetBusVolume();
                   // bgmButton.enabled = true;
                    bgmButton.Select();
                    isChange = false;
                    ////Debug.log("b");
                    //こっちでは逆に音量をスライダーの数値に入れる
                }
                else if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction18))
                {
                    if (!isBlock)
                    {
                        //音量変更。スライダーの数値を音量に入れる。
                        MasterAudio.SetBusVolumeByName("BGM", bgmSlider.value / 100);
                        // bgmButton.enabled = true;
                        bgmButton.Select();
                        isChange = false;
                        ////Debug.log("c");
                    }
                    else
                    {
                        isBlock = false;
                    }
                }
                else if(MainUICon.instance._reInput.UIMovement.y > 0 && !isUp)
                {
                    ////Debug.log("a");
                    bgmSlider.value += 10;
                    isUp = true;
                    isDown = false;
                }
                else if (MainUICon.instance._reInput.UIMovement.y < 0 && !isDown)
                {
                    bgmSlider.value -= 10;
                    isUp = false;
                    isDown = true;
                }
            }
            else if(seSlider.gameObject == selectObject)
            {
                if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
                {
                    seSlider.value = DarkTonic.MasterAudio.MasterAudio.GrabBusByName("SE").volume * 100;
                    seButton.Select();
                    isChange = false;
                }
                else if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction18))
                {
                    if (!isBlock)
                    {
                        //音量変更。スライダーの数値を音量に入れる。
                        MasterAudio.SetBusVolumeByName("SE", seSlider.value / 100);
                        // bgmButton.enabled = true;
                        seButton.Select();
                        isChange = false;
                        ////Debug.log("c");
                    }
                    else
                    {
                        isBlock = false;
                    }
                }
                else if (MainUICon.instance._reInput.UIMovement.y > 0 && !isUp)
                {
                    seSlider.value += 10;
                    isUp = true;
                    isDown = false;
                }
                else if (MainUICon.instance._reInput.UIMovement.y < 0 && !isDown)
                {
                    seSlider.value -= 10;
                    isUp = false;
                    isDown = true;
                }
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

            isBlock = false;
        }
    }

    public void VChange()
    {
        ////Debug.log("お肉");
    }

    /// <summary>
    /// BGMの設定をいじるボタンを押したとき
    /// </summary>
    public void BGMSelect()
    {
   //     bgmButton.enabled = false;
        bgmSlider.Select();
        isChange = true;
        isBlock = true;
    }

    /// <summary>
    /// SEの設定をいじるボタンを押したとき
    /// </summary>
    public void SESelect()
    {
        seSlider.Select();
        isChange = true;
        isBlock = true;
    }


}
