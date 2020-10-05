using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "CoreDataBase", menuName = "CreateCoreDataBase")]
//所持データベースとか作れるな
public class CoreDataBase : ScriptableObject
{

	[SerializeField]
	private List<CoreItem> toolLists = new List<CoreItem>();

	//　アイテムリストを返す
	public List<CoreItem> GetItemLists()
	{
		return toolLists;
	}
}