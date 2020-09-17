
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "PlayerStatus", menuName = "CreatePlayerStatus")]
public class PlayerStatus : CharacterStatus
{

    //　獲得経験値
    [SerializeField]
    public float earnedExperience = 0;
    //　装備している武器
    [SerializeField]
    public Wepon equipWeapon = null;
    //　装備している鎧
    public Item equipArmor = null;
    //　アイテムと個数のDictionary
    public ItemDictionary itemDictionary = null;

}
