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
    bool cancel;
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
    bool isUp;
    float firstValue;

    float changeTime;
    [SerializeField] Text bgmTx;
    [SerializeField] Text seTx;

    // Start is called before the first frame update
    void Start()
    {
      //  bgmSlider.value = 50;
     //   seSlider.value = 50;
    }

    private void OnDisable()
    {
        ValueCancel();
    }

    // Update is called once per frame
    void Update()
    {
 

        selectObject = MainUICon.instance.selectButton;


        if (!isFirst)
        {
            bgmSlider.value = DarkTonic.MasterAudio.MasterAudio.GrabBusByName("BGM").volume * 100;
            seSlider.value = DarkTonic.MasterAudio.MasterAudio.GrabBusByName("SE").volume * 100;
            bgmTx.text = bgmSlider.value.ToString();
            seTx.text = seSlider.value.ToString();
            isFirst = true;
        }


        if (isChange)
        {

            if (bgmSlider.gameObject == selectObject)
            {
                ValueControll(bgmSlider);
                MasterAudio.SetBusVolumeByName("BGM", bgmSlider.value / 100);
            }
            else if (seSlider.gameObject == selectObject)
            {
                ValueControll(seSlider);
                MasterAudio.SetBusVolumeByName("SE", seSlider.value / 100);
            }
        }
        else
        {
            
            if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
            {
                if (!cancel)
                {
                    cancel = true;
                    return;
                }
                this.gameObject.SetActive(false);
                MainUICon.instance.masterUI.SetActive(true);
                this.gameObject.SetActive(false);
                sButton.Select();
                
            }
        }
    }


    /// <summary>
    /// BGMの設定をいじるボタンを押したとき
    /// </summary>
    public void BGMSelect()
    {
        cancel = false;
        //     bgmButton.enabled = false;
        bgmSlider.Select();
        isChange = true;
        firstValue = bgmSlider.value;
    }

    /// <summary>
    /// SEの設定をいじるボタンを押したとき
    /// </summary>
    public void SESelect()
    {
        cancel = false;
        seSlider.Select();
        isChange = true;
        firstValue = seSlider.value;
    }

    /// <summary>
    /// 戻るボタンで最初の値に戻る
    /// </summary>
    public void ValueCancel()
    {
        //決定ボタン

        isFirst = false;

        if (selectObject == bgmSlider.gameObject)
        {
            bgmSlider.value = firstValue;
            MasterAudio.SetBusVolumeByName("BGM", firstValue / 100);
            bgmButton.Select();
        }
        else
        {
            seSlider.value = firstValue;
            MasterAudio.SetBusVolumeByName("SE", firstValue / 100);
            seButton.Select();
        }
        isChange = false;
    }

    /// <summary>
    /// 確定して編集終了
    /// </summary>
    public void ValueEnd()
    {
        isChange = false;
        isFirst = false;
        if (selectObject == bgmSlider.gameObject)
        {
            bgmButton.Select();
        }
        else
        {
            seButton.Select();
        }
        Debug.Log($"ｓｓｓ{selectObject.name}");
    }





    public void txChange()
    {Debug.Log($"dd{selectObject.name}{bgmSlider.name}");
        if (selectObject == bgmSlider.gameObject)
        {
            bgmTx.text = bgmSlider.value.ToString();
            
        }
        else
        {
           seTx.text = seSlider.value.ToString();
        }
        
    }

    public void ValueControll(Slider _slider)
    {
        //決定ボタン
        if (MainUICon.instance._reInput.UIMovement.y > 0 && !isUp)
        {
            ////Debug.log("a");
            _slider.value += 10;
            isUp = true;
            isDown = false;
            //tx.text = _slider.value.ToString();
        }
        else if (MainUICon.instance._reInput.UIMovement.y < 0 && !isDown)
        {
            _slider.value -= 10;
            // tx.text = _slider.value.ToString();
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
