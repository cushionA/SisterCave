using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EquipDataBase", menuName = "CreateEquipDataBase")]
//所持データベースとか作れるな
public class EquipDataBase : ScriptableObject
{

	[SerializeField]
	private List<Equip> toolLists = new List<Equip>();

	//　アイテムリストを返す
	public List<Equip> GetItemLists()
	{
		return toolLists;
	}
}