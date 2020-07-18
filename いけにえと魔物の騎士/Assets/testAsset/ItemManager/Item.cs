using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{

	public enum KindOfItem
	{
		Weapon,
		RecoverItem,
		CureItem,
		ToolItem,
		AttackMagic,
		RecoverMagic,
		CureMagic,
		KeyItem
	}

	//　アイテムの種類
	[SerializeField]
	private KindOfItem kindOfItem;
	//　アイテムのアイコン
	[SerializeField]
	private Sprite icon;
	//　アイテムの名前
	[SerializeField]
	private string itemName;
	//　アイテムの平仮名名
	[SerializeField]
	private string hiraganaName = "";
	//　アイテムの情報
	[SerializeField]
	private string information;
	//ソート用のナンバー
	[SerializeField]
	private int number;

	public KindOfItem GetKindOfItem()
	{
		return kindOfItem;
	}

	public Sprite GetIcon()
	{
		return icon;
	}

	public string GetItemName()
	{
		return itemName;
	}
	//　アイテムの平仮名の名前を返す
	public string GetHiraganaName()
	{
		return hiraganaName;
	}
	public string GetInformation()
	{
		return information;
	}
	public int GetNumber()
	{
		return number;
	}


}