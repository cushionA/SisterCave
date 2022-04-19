
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


    
    public float initialWeight;

    //　獲得した魂
    [SerializeField]
    public float earnedSoul = 0;
    //　装備している武器

    //　最大スタミナ
    public float maxStamina = 60;
    //　スタミナ
    //[HideInInspector]
    public float stamina = 60;

    //生命力
    public float Vitality = 1;
    //持久力
    public float Endurance = 1;
    //MPと魔法もてる数
    public float capacity = 1;
    //　力
    public float power = 1;
    //技量
    public float skill = 1;
    //　魔法力。賢さ
    public float _int = 1;

    /// <summary>
    /// データごとに保持しておくためにも必要？
    /// </summary>
    [SerializeField]

    public CoreItem equipCore;

    public AnimationCurve HpCurve;
    public AnimationCurve StaminaCurve;
    public AnimationCurve weightCurve;
    public AnimationCurve MpCurve;

   
    public float capacityWeight;

    //　装備している鎧
    public Item equipArmor = null;
    //　アイテムと個数のDictionary
    public ItemDictionary itemDictionary = null;



    public float guardSpeed;//ガード中の移動速度



    public float moveSpeed;


    public float dashSpeed;


    public float avoidRes;

    public float avoidSpeed;
	#region

	[Header("加速速度")]
	///<summary>
	///yこの値が大きいほど加速が早くなる
	///</summary>
	public Vector2 addSpeed;




	[Header("横のジャンプ力")]
	///<summary>
	///AddForceでやる。空飛ぶキャラはきもちジャンプ力低め？
	///</summary>
	public float jumpMove;

	[Header("縦のジャンプ力")]
	///<summary>
	///AddForceでやる。空飛ぶキャラはきもちジャンプ力低め？
	///</summary>
	public float jumpPower;

	[Header("ジャンプの継続時間")]
	public float jumpRes = 0.4f;

	[Header("ジャンプのクールタイム")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public float jumpCool = 2;

	[Header("ジャンプの回数")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public int jumpLimit;

	[Header("回避のクールタイム")]
	public float avoidCool;

	[Header("デフォルトの重力")]
	///<summary>
	///最初の重力。飛行キャラは小さく
	///</summary>
	public float firstGravity;//重力。gravityScaleに入れる？　空中キャラはダウンした時だけ重力入る

	[Header("ダウンする時間")]
	///<summary>
	///ダウンする時間
	///</summary>
	public float downRes;




	#endregion



    public enum PlayerWeightState
    {
        軽装備,
        通常装備,
        重装備,
        重量オーバー

    }

    public PlayerWeightState _weightState;




}
