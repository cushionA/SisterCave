using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MoreMountains.InventoryEngine
{
	[Serializable]
	/// <summary>
	/// ベースとなるインベントリークラスです。 
	/// アイテムの保存、その内容の保存と読み込み、アイテムの追加、アイテムの削除、装備などを処理することになります。
	/// </summary>
	public class MyInventory : Inventory
	{

		public new enum InventoryTypes { Tool, REquipment, LEquipment, Core, Magic,Pray,Combination, KeyItem,Material, Library }
		public bool allowEmpty;

		[Header("Inventory Type")]
		/// whether this inventory is a main inventory or equipment one
		[Tooltip("Here you can define your inventory's type. Main are 'regular' inventories. Equipment inventories will be bound to a certain item class and have dedicated options.")]
		public new InventoryTypes InventoryType = InventoryTypes.Tool;
	}
}
