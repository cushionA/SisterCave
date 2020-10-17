using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "CreateEnemyDataBase")]
//所持データベースとか作れるな
public class EnemyDataBase : ScriptableObject
{

	[SerializeField]
	private List<EnemyLibrary> toolLists = new List<EnemyLibrary>();

	//　アイテムリストを返す
	public List<EnemyLibrary> GetItemLists()
	{
		return toolLists;
	}
}