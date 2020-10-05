using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "MagicDataBase", menuName = "CreateMagicDataBase")]
//所持データベースとか作れるな
public class MagicDataBase : ScriptableObject
{

	[SerializeField]
	private List<Magic> toolLists = new List<Magic>();

	//　アイテムリストを返す
	public List<Magic> GetItemLists()
	{
		return toolLists;
	}
}
