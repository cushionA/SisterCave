using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "ToolItem", menuName = "CreateToolItem")]
public class ToolItem : ScriptableObject
{
	public enum KindOfItem
	{
		CureItem,
		UseItem
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

	//所持数
	[HideInInspector] public int inventoryNum = 0;


	public KindOfItem GetKindOfItems()
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

	public ToolItem GetTool()
    {

		return this;

    }


}