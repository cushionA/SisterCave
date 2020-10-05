using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "SisterStatus", menuName = "CreateSisterStatus")]
 public class SisterStatus : ScriptableObject
{
    public int level = 1;
    //　最大MP
    public float maxMp = 30;
    //　MP
    public float mp = 30;


    //クールタイム短縮
    public float Endurance = 1;
    //MPと魔法もてる数
    public float capacity = 1;
    //技量。詠唱時間
    public float castSkill = 1;
    //　魔法力。賢さ
    public float _int = 1;
    //　信仰。これがあるからシスターさんの魔法は威力が高い
    public float faith = 1;
    //　獲得経験値
    public int earnedExperience = 0;

    public float attackBuff = 1.0f;
    //　装備している魔法
    public List<SisMagic> equipMagic = null;
    [HideInInspector]public SisMagic useMagic;

    public float castSpeed;//詠唱短縮
    public float magicAssist;
    //private Item useMagic;


	[Header("移動設定判断")]
	///<summary>
	///移動ステート切り替え
	///</summary>
	public float judgePace = 3.0f;

	[Header("おんぶ可能距離")]
	///<summary>
	///おんぶできる距離
	///</summary>
	public float pegionDis = 6.0f;


	[Header("加速速度")]
	///<summary>
	///yこの値が大きいほど加速が早くなる
	///</summary>
	public float addSpeed;

	[Header("警戒中のスピード")]
	///<summary>
	///警戒中この速度でくっつく
	///</summary>
	public float patrolSpeed;

	[Header("警戒状態の維持距離")]
	///<summary>
	///そばにくっついていく距離。-1くらい？
	///</summary>
	public float patrolDistance;

	[Header("警戒で歩く距離")]
	///<summary>
	///この距離まで来たら歩く
	///</summary>
	public float walkDistance;

	[Header("歩く速さ")]
	///<summary>
	///歩き速度
	///</summary>
	public float walkSpeed;

	[Header("のんびりの維持距離")]
	///<summary>
	///くっついていく距離。-1くらい？少しくらい離れても走らない
	///</summary>
	public float playDistance;

	[Header("のんびりのスピード")]
	///<summary>
	///のんびりでこの速度
	///</summary>
	public float playSpeed;

	[Header("のんびりでたまに停止する時間")]
	///<summary>
	///端まで来たら一定時間そのまま待ってる。
	///</summary>
	public float waitRes;

	[Header("走る速度")]
	public float dashSpeed;

	[Header("停止範囲")]
	///<summary>
	///目的距離にゆとり持たせる。
	///</summary>
	public float adjust;

	[Header("水平ジャンプ力")]
	///<summary>
	///AddForceでやる。
	///</summary>
	public float jumpSpeed;

	[Header("垂直ジャンプ力")]
	///<summary>
	///AddForceでやる。
	///</summary>
	public float jumpPower;

	[Header("ジャンプの継続時間")]
	public float jumpRes;

	[Header("ジャンプのクールタイム")]
	///<summary>
	///飛行キャラはほぼなし。
	///</summary>
	public float jumpCool;

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

	[Header("負の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);

	[Header("正の最高速度")]
	///<summary>
	///これを超えると修正される
	///</summary>
	public Vector2 velocityMax = new Vector2(+100.0f, +50.0f);



	[Header("接地判定用のフィルター")]
	///<summary>
	///マスクしてやれば地面しか検出しない。
	///</summary>
	public ContactFilter2D filter;

}