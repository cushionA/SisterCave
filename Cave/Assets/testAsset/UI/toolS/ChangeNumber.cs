﻿using UnityEngine;
using UnityEngine.UI;
public class ChangeNumber : MonoBehaviour
{
    [SerializeField]
    Text ChangeNum;

    bool isChange;

    float verticalKey;
    float horizontalKey;
    float changeTime;
    bool wait;
    bool charge;//入力からこれだけ時間がたつまでは連続入力しない


    // Start is called before the first frame update
    void Start()
    {
        ChangeNum.text = $"{ToolManager.instance.changeNum}";

    }

    // Update is called once per frame
    void Update()
    {
        //  ////Debug.log($"変更数{ToolManager.instance.changeNum}");
        //  ////Debug.log($"所持数{5}");
        //  ////Debug.log($"偽なら変更可能{isChange}");
        verticalKey = GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction15);
        horizontalKey = GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction16);
     //   ////Debug.log($"上キー{verticalKey}");
      //  ////Debug.log($"横キー{horizontalKey}");
        if (!isChange)
        {
        //    ////Debug.log("あかつきの");
            if (verticalKey > 0 && horizontalKey == 0)
            {
                isChange = true;
                if (ToolManager.instance.changeNum + 1 <= 5)
                {
                    ToolManager.instance.changeNum += 1;
                }
            }
            else if (verticalKey < 0 && horizontalKey == 0)
            {
                isChange = true;

                if (ToolManager.instance.changeNum - 1 >= 1)
                {
                    ToolManager.instance.changeNum -= 1;
                }
            }
            else if (verticalKey == 0 && horizontalKey > 0)
            {
                isChange = true;
                if (ToolManager.instance.changeNum + 10 <= 5)
                {
                    ToolManager.instance.changeNum += 10;
                }
                else
                {
                    ToolManager.instance.changeNum = 5;
                }
            }
            else if (verticalKey == 0 && horizontalKey < 0)
            {
                isChange = true;

                if (ToolManager.instance.changeNum - 10 >= 1)
                {
                    ToolManager.instance.changeNum -= 10;
                }
                else
                {
                    ToolManager.instance.changeNum = 1;
                }
            }
        }
        else if (isChange)
        {
            if (horizontalKey != 0 || verticalKey != 0)
            {
                //    ////Debug.log("しののめの");
                //changeTime += Time.realtimeSinceStartup;
                if (!charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;//TimeScaleのせい
                        wait = true;
                    }
                    if (Time.realtimeSinceStartup - changeTime >= 0.5)
                    {
                        //       ////Debug.log("長押し清算");
                        charge = true;
                        wait = false;
                        changeTime = 0.0f;
                    }
                }
                else if (charge)
                {
                    if (!wait)
                    {
                        changeTime = Time.realtimeSinceStartup;
                        wait = true;
                    }

                    //Time.realtimeSinceStartup - changeTime
                    if (Time.realtimeSinceStartup - changeTime >= 0.1)
                    {
                        //       ////Debug.log("長押し清算");
                        isChange = false;
                        wait = false;
                        changeTime = 0.0f;
                    }
                }
            }
            else if (horizontalKey == 0 && verticalKey == 0)
            {
                charge = false;
              //  ////Debug.log("ボタン離し清算");
                isChange = false;
                changeTime = 0.0f;
            }
        }
     //   ////Debug.log($"第一条件{isChange || horizontalKey != 0 || verticalKey != 0}");
      //  ////Debug.log($"第二条件{isChange && horizontalKey == 0 && verticalKey == 0}");
        ChangeNum.text = $"{ToolManager.instance.changeNum}";
    }
}
