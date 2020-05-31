
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "AllyStatus", menuName = "CreateAllyStatus")]
public class PlayerStatus : CharacterStatus
{

    //　獲得経験値
    [SerializeField]
    private int earnedExperience = 0;
    //　装備している武器
    [SerializeField]
    private Item equipWeapon = null;
    //　装備している鎧
    [SerializeField]
    private Item equipArmor = null;
    //　アイテムと個数のDictionary
    [SerializeField]
    private ItemDictionary itemDictionary = null;

    public void SetEarnedExperience(int earnedExperience)
    {
        this.earnedExperience = earnedExperience;
    }

    public int GetEarnedExperience()
    {
        return earnedExperience;
    }

    public void SetEquipWeapon(Item weaponItem)
    {
        this.equipWeapon = weaponItem;
    }

    public Item GetEquipWeapon()
    {
        return equipWeapon;
    }

    public void SetEquipArmor(Item armorItem)
    {
        this.equipArmor = armorItem;
    }

    public Item GetEquipArmor()
    {
        return equipArmor;
    }

    public void CreateItemDictionary(ItemDictionary itemDictionary)
    {
        this.itemDictionary = itemDictionary;
    }

    public void SetItemDictionary(Item item, int num = 0)
    {
        itemDictionary.Add(item, num);
    }

    //　アイテムが登録された順番のItemDictionaryを返す
    public ItemDictionary GetItemDictionary()
    {
        return itemDictionary;
    }
    //　平仮名の名前でソートしたItemDictionaryを返す
    public IOrderedEnumerable<KeyValuePair<Item, int>> GetSortItemDictionary()
    {
        return itemDictionary.OrderBy(item => item.Key.GetHiraganaName());
    }
    public int SetItemNum(Item tempItem, int num)
    {
        return itemDictionary[tempItem] = num;
    }
    //　アイテムの数を返す
    public int GetItemNum(Item item)
    {
        return itemDictionary[item];
    }
}
