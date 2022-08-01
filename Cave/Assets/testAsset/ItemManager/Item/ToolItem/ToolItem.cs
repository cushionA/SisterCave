using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.InventoryEngine;


[Serializable]
[CreateAssetMenu(fileName = "ToolItem", menuName = "CreateToolItem")]
public class ToolItem : Item
{
	public enum KindOfItem
	{
		CureItem,
		UseItem
	}


	//　アイテムの種類
	public KindOfItem kindOfItem;
	//　アイテムのアイコン

	[Header("まとめて使えるかどうかのフラグ")]
	public bool summarizeUse;

}