using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "MaterialDataBase", menuName = "CreateMaterialDataBase")]
//所持データベースとか作れるな
public class MaterialDataBase : ScriptableObject
{

	[SerializeField]
	private List<MaterialItem> toolLists = new List<MaterialItem>();

	//　アイテムリストを返す
	public List<MaterialItem> GetItemLists()
	{
		return toolLists;
	}
}