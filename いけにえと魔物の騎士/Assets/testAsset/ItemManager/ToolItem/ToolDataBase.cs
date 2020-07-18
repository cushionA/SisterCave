using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "toolDataBase", menuName = "CreateToolDataBase")]
public class ToolDataBase : ScriptableObject
{

	[SerializeField]
	private List<ToolItem> toolLists = new List<ToolItem>();

	//　アイテムリストを返す
	public List<ToolItem> GetItemLists()
	{
		return toolLists;
	}
}
