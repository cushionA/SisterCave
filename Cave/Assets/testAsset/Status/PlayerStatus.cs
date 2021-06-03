
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

    public float PlayerLevel = 1;

    public float initialHp = 100;
    //　HP初期値
    public float initialMp = 30;
    //　MP初期値
    //　スタミナ初期値
    public float initialStamina = 60;

    public int magicNumber = 1;
    public float initialWeight;

    //　獲得した魂
    [SerializeField]
    public float earnedSoul = 0;
    //　装備している武器
    [SerializeField]
    public Wepon equipWeapon = null;
    public Shield equipShield;
    public CoreItem equipCore;

    public AnimationCurve HpCurve;
    public AnimationCurve StaminaCurve;
    public AnimationCurve weightCurve;
    public AnimationCurve MpCurve;

    public float equipWeight = 1;//装備重量
    public float capacityWeight;

    //　装備している鎧
    public Item equipArmor = null;
    //　アイテムと個数のDictionary
    public ItemDictionary itemDictionary = null;

    //　装備している鎧
    public List<PlayerMagic> equipMagic = null;
    [HideInInspector]public PlayerMagic useMagic;

    public float guardSpeed;//ガード中の移動速度



    public float lightSpeed;
    public float middleSpeed;
    public float heavySpeed;
    public float overSpeed;

    public float lightDash;
    public float middleDash;
    public float heavyDash;
    public float overDash;

    public float lightAvoid;
    public float middleAvoid;
    public float heavyAvoid;
    public float overAvoid;

}
