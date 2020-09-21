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
    public int earnedExperience = 0;
    //　装備している魔法
    public List<SisMagic> equipMagic = null;
    [HideInInspector]public SisMagic useMagic;

    public float castSkill;//詠唱短縮
    public float magicAssist;
    //private Item useMagic;


}