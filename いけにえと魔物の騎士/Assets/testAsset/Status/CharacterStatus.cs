using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class CharacterStatus : ScriptableObject
{


    //　毒状態かどうか
    [SerializeField]
    private bool isPoisonState = false;
    //　痺れ状態かどうか
    [SerializeField]
    private bool isNumbnessState = false;
    //　キャラクターのレベル
    [SerializeField]
    private int level = 1;
    //　最大HP
    [SerializeField]
    private int maxHp = 100;
    //　HP
    [SerializeField]
    private int hp = 100;
    //　最大MP
    [SerializeField]
    private int maxMp = 30;
    //　MP
    [SerializeField]
    private int mp = 30;
    //　最大スタミナ
    [SerializeField]
    private int maxStamina = 60;
    //　スタミナ
    [SerializeField]
    private int stamina = 60;
    //　力
    [SerializeField]
    private int power = 10;
    //　打たれ強さ
    [SerializeField]
    private int strikingStrength = 10;
    //　魔法力
    [SerializeField]
    private int magicPower = 10;

  
    public void SetPoisonState(bool poisonFlag)
    {
        isPoisonState = poisonFlag;
    }

    public bool IsPoisonState()
    {
        return isPoisonState;
    }

    public void SetNumbness(bool numbnessFlag)
    {
        isNumbnessState = numbnessFlag;
    }

    public bool IsNumbnessState()
    {
        return isNumbnessState;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetMaxHp(int hp)
    {
        this.maxHp = hp;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public void SetHp(int hp)
    {
        //ゼロと最大HPと現在のHPで大きい方を比較して、大きい方を入れるようにしてる。すなわちゼロ以上かつ最大値以下
        this.hp = Mathf.Max(0, Mathf.Min(GetMaxHp(), hp));
    }

    public int GetHp()
    {
        return hp;
    }

    public void SetMaxMp(int mp)
    {
        this.maxMp = mp;
    }

    public int GetMaxMp()
    {
        return maxMp;
    }

    public void SetMp(int mp)
    {
        this.mp = Mathf.Max(0, Mathf.Min(GetMaxMp(), mp));
    }

    public int GetMp()
    {
        return mp;
    }

    public void SetMaxStamina(int maxStamina)
    {
        this.maxStamina = stamina;
    }

    public int GetMaxStamina()
    {
        return maxStamina;
    }

    public void SetStamina(int stamina)
    {
        this.hp = Mathf.Max(0, Mathf.Min(GetMaxStamina(), stamina));
    }

    public int GetStamina()
    {
        return stamina;
    }

    public void SetPower(int power)
    {
        this.power = power;
    }

    public int GetPower()
    {
        return power;
    }

    public void SetStrikingStrength(int strikingStrength)
    {
        this.strikingStrength = strikingStrength;
    }

    public int GetStrikingStrength()
    {
        return strikingStrength;
    }

    public void SetMagicPower(int magicPower)
    {
        this.magicPower = magicPower;
    }

    public int GetMagicPower()
    {
        return magicPower;
    }
}