using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : ScriptableObject
{



	[Header("アイテムのアイコン")]
	public Sprite icon;
	[Header("アイテムの名前")]
	public string itemName;
	[Header("アイテムの平仮名名")]
	public string hiraganaName = "";
	[Header("アイテムの情報")]
	public string information;
	[Header("ソート用のナンバー")]
	public int number;
	[Header("所持限界")]
	public int holdLimmit;

	[Header("ドロップアイテムかどうか")]
	///<summary>
	///ドロップなら設定
	///ドロップアイテムはロードで消え去る
	///しかしドロップアイテムでも重要アイテムならフラグは立てない
	///</summary>
	public bool isDrop;

	//所持数
	[HideInInspector] public int inventoryNum = 0;



}