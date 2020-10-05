using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyDataBase", menuName = "CreateKeyDataBase")]
//所持データベースとか作れるな
public class KeyDataBase : ScriptableObject
{

	[SerializeField]
	private List<KeyItem> toolLists = new List<KeyItem>();

	//　アイテムリストを返す
	public List<KeyItem> GetItemLists()
	{
		return toolLists;
	}
}