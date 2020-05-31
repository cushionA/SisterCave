using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "SisterStatus", menuName = "CreateSisterStatus")]
public class SisterStatus : CharacterStatus
{

    //　獲得経験値
    [SerializeField]
    private int earnedExperience = 0;
    //　装備している魔法
    [SerializeField]
    private Item equipMagic = null;

    public void SetEarnedExperience(int earnedExperience)
    {
        this.earnedExperience = earnedExperience;
    }

    public int GetEarnedExperience()
    {
        return earnedExperience;
    }

    public void SetEquipMagic(Item magicItem)
    {
        this.equipMagic = magicItem;
    }

    public Item GetEquipMagic()
    {
        return equipMagic;
    }


}