using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SisData : MonoBehaviour
{

    //　アイテムのImage画像
    private Sprite itemSprite;
    //　アイテムの名前
    private string itemName;
    //　アイテムのタイプ
    private SisDataBase.ItemType itemType;
    //　アイテムの情報
    private string itemInformation;

    public SisData(Sprite image, string itemName, SisDataBase.ItemType itemType, string information)
    {
        this.itemSprite = image;
        this.itemName = itemName;
        this.itemType = itemType;
        this.itemInformation = information;
    }

    public Sprite GetItemSprite()
    {
        return itemSprite;
    }

    public string GetItemName()
    {
        return itemName;
    }

    public SisDataBase.ItemType GetItemType()
    {
        return itemType;
    }

    public string GetItemInformation()
    {
        return itemInformation;
    }
}
