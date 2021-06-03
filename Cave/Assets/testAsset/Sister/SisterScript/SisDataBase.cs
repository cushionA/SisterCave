using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SisDataBase : MonoBehaviour
{

    //　アイテムの種類
    public enum ItemType
    {
       Attack,
       Support,
       Other
    }
    //　アイテムデータのリスト
    public List<SisData> SisDataList = new List<SisData>();

    void Awake()
    {
        //　アイテムの全情報を作成
        SisDataList.Add(new SisData(Resources.Load("Light", typeof(Sprite)) as Sprite, "Light", ItemType.Support, "あれば便利な辺りを照らすライト"));
        SisDataList.Add(new SisData(Resources.Load("FireBall", typeof(Sprite)) as Sprite, "FireBall", ItemType.Attack, "火の玉"));


    }
    //　全アイテムデータを返す
    public List<SisData> GetSisDataList()
    {
        return SisDataList;
    }
    //　個々のアイテムデータを返す
    public SisData GetSisData(string itemName)
    {
        foreach (var item in SisDataList)
        {
            if (item.GetItemName() == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
