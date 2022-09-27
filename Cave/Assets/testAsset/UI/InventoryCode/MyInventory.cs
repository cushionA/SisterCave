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
	/// �x�[�X�ƂȂ�C���x���g���[�N���X�ł��B 
	/// �A�C�e���̕ۑ��A���̓��e�̕ۑ��Ɠǂݍ��݁A�A�C�e���̒ǉ��A�A�C�e���̍폜�A�����Ȃǂ��������邱�ƂɂȂ�܂��B
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
