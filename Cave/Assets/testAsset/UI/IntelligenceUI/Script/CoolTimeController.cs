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


    /// 第一、1000000
    //  第二、0100000
    //　第三、0010000
    //　第四、00010000



    [SerializeField] Text tx;



    bool isDown;
    bool wait;
    bool charge;//長押し完了してるかどうか。完了してたらぱっぱと変わる
    bool isUp;

    float changeTime;

    bool isFirst;

    float firstValue;

    /// <summary>
    /// 攻撃、サポート、回復の順番
    /// 0~3まではトグル以降はドロップ
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


        ////Debug.log("開始");
        if (_coolSlider.gameObject == MainUICon.instance.selectButton)
        {
            ////Debug.log("bgm調整中");

            //決定ボタン
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

    /// <summary>
    /// 数値操作関連
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
    /// 最初にトグルの表示を決めるやつ
    /// </summary>
    /// <param name="i"></param>
    void SetToggle(int j)
    {
       // Debug.Log($"スキップ番号{GetSkipOption()}");
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
    /// 決定ボタンで行くやつ
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
    /// トグルが変化するたびにやる
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
    /// ナビゲーションを設定する
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

    //スライダー操作関連
    #region

    /// <summary>
    /// 値を適用する
    /// </summary>
    public void VChange()
    {
        //決定ボタン
        SetCoolTime();

        isFirst = false;
        _coolButton.Select();

    }

    /// <summary>
    /// 途中で編集やめた時
    /// 数値をもとに戻す
    /// </summary>
    public void VEnd()
    {
        _coolSlider.value = firstValue;
        _coolButton.Select();
        isFirst = false;

    }



    /// <summary>
    /// スライダーの設定をいじるボタンを押したとき
    /// 最初のメソッド
    /// </summary>
    public void SliderSelect()
    {
        _coolSlider.Select();


    }

    //数値変更の時に呼ぶ
    public void txChange()
    {
        tx.text = _coolSlider.value.ToString();
    }

    #endregion



}
