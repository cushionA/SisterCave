using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationTipsChange : MonoBehaviour
{
    bool isFirst;

    [HideInInspector]
    public string description;

    [SerializeField]
    string[] txCandi;

    /// <summary>
    /// タイプとしてTargetJudgeのような二種類だけのもの
    /// いや↑は一個一個セット番号で切り替えてるからいらんやろ、ファーストのSRコンディションだけ
    /// 数値入れるのとかも窓自体の解説すればいいよな
    /// EditButtonのように攻撃回復支援で使い分けるもの
    /// 道中回復のもいらないね、固有だから
    /// 他にもあるか要検証
    /// てかこいついらない
    /// </summary>
    [SerializeField]
    int type;

    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        int s = MainUICon.instance.settingNumber;

        

        if (MainUICon.instance.eventSystem.currentSelectedGameObject == this.gameObject)
        {
            isFirst = true;
            MainUICon.instance.isTips = true;
        }
        else if (isFirst)
        {
            MainUICon.instance.isTips = false;
            isFirst = false;
        }

    }
}
