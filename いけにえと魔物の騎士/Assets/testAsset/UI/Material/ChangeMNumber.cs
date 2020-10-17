using UnityEngine;
using UnityEngine.UI;

public class ChangeMNumber : MonoBehaviour
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
        ChangeNum.text = $"{MaterialManager.instance.changeNum}";

    }

    // Update is called once per frame
    void Update()
    {
        //  Debug.Log($"変更数{MaterialManager.instance.changeNum}");
        //  Debug.Log($"所持数{MaterialManager.instance.selectItem.inventoryNum}");
        //  Debug.Log($"偽なら変更可能{isChange}");
        verticalKey = Input.GetAxisRaw("Vertical");
        horizontalKey = Input.GetAxisRaw("Horizontal");
        //    Debug.Log($"上キー{verticalKey}");
        //  Debug.Log($"横キー{horizontalKey}");
        if (!isChange)
        {
            Debug.Log("あかつきの");
            if (verticalKey > 0 && horizontalKey == 0)
            {
                isChange = true;
                if (MaterialManager.instance.changeNum + 1 <= MaterialManager.instance.selectItem.inventoryNum)
                {
                    MaterialManager.instance.changeNum += 1;
                }
            }
            else if (verticalKey < 0 && horizontalKey == 0)
            {
                isChange = true;

                if (MaterialManager.instance.changeNum - 1 >= 1)
                {
                    MaterialManager.instance.changeNum -= 1;
                }
            }
            else if (verticalKey == 0 && horizontalKey > 0)
            {
                isChange = true;
                if (MaterialManager.instance.changeNum + 10 <= MaterialManager.instance.selectItem.inventoryNum)
                {
                    MaterialManager.instance.changeNum += 10;
                }
                else
                {
                    MaterialManager.instance.changeNum = MaterialManager.instance.selectItem.inventoryNum;
                }
            }
            else if (verticalKey == 0 && horizontalKey < 0)
            {
                isChange = true;

                if (MaterialManager.instance.changeNum - 10 >= 1)
                {
                    MaterialManager.instance.changeNum -= 10;
                }
                else
                {
                    MaterialManager.instance.changeNum = 1;
                }
            }
        }
        else if (isChange)
        {
            if (horizontalKey != 0 || verticalKey != 0)
            {
                //    Debug.Log("しののめの");
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
                        //       Debug.Log("長押し清算");
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
                        //       Debug.Log("長押し清算");
                        isChange = false;
                        wait = false;
                        changeTime = 0.0f;
                    }
                }
            }
            else if (horizontalKey == 0 && verticalKey == 0)
            {
                //        Debug.Log("ボタン離し清算");
                isChange = false;
                changeTime = 0.0f;
            }
        }
        // Debug.Log($"第一条件{isChange || horizontalKey != 0 || verticalKey != 0}");
        // Debug.Log($"第二条件{isChange && horizontalKey == 0 && verticalKey == 0}");
        ChangeNum.text = $"{MaterialManager.instance.changeNum}";
    }
}
