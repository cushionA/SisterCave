using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item : ScriptableObject
{



	//　アイテムのアイコン
	public Sprite icon;
	//　アイテムの名前
    public string itemName;
	//　アイテムの平仮名名
	public string hiraganaName = "";
	//　アイテムの情報
	public string information;
	//ソート用のナンバー
	public int number;

	//所持数
	[HideInInspector] public int inventoryNum = 0;



}