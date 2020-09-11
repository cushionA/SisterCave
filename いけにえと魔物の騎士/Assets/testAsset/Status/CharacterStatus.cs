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
    public bool isPoisonState = false;

    //　痺れ状態かどうか
    public bool isNumbnessState = false;

    //　キャラクターのレベル
    public int level = 1;

    //　最大HP
    public float maxHp = 100;

    //　HP
    public float hp = 100;

    //　最大MP
    public float maxMp = 30;

    //　MP
    public float mp = 30;

    //　最大スタミナ
    public float maxStamina = 60;

    //　スタミナ
    public float stamina = 60;

    //　力
    public float power = 10;

    //　防御力
    public float DEF = 10;

    //　魔法力
    public float magicPower = 10;

    //小怯み。普段は基本ゼロ。攻撃時だけ
    public float Armor;

    [SerializeField] float BlowBase;

    //大怯み、吹っ飛び。基本均一でアーマー値を足す

  /*
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
  */
}