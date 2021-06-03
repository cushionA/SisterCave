using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
	public static ToolManager instance = null;
//	[HideInInspector] public GameObject selectButton;
	[HideInInspector]public ToolItem selectItem;
	public GameObject selectWindow;
	public GameObject equipWindow;
	[HideInInspector]public bool isUseMenu;
	[HideInInspector]public bool isEquipMenu;
	[HideInInspector] public int setNumber;//何番目のボタンに入れるかどうか
	[HideInInspector] public int changeNum;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	//　アイテムデータベース
	[SerializeField]
	private ToolDataBase toolDataBase;
	//　アイテム数管理
	//private Dictionary<ToolItem, int> numOfItem = new Dictionary<ToolItem, int>();

	//string playerTag = "Player";
	//[HideInInspector]

	//[HideInInspector]
	

	bool isUp;
	//[HideInInspector]public ToolItem use;

	// Use this for initialization







    //　名前でアイテムを取得
    public ToolItem GetItem(string searchName)
	{
		return toolDataBase.GetItemLists().Find(itemName => itemName.itemName == searchName);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		

	}

	public void ChangeNum(ToolItem[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{
			int pas = takeItem[i].inventoryNum;
			takeItem[i].inventoryNum = pas + changeNum[i];
			isUp = true;
			/*int pas = numOfItem[GetItem($"{takeItem}")];
			numOfItem[GetItem($"{takeItem}")] = pas + changeNum;
			isUp = true;*/
			if (takeItem[i].inventoryNum < 0)
			{
				takeItem[i].inventoryNum = 0;

			}
		}
	}
	//public Dictionary<ToolItem,int> GetItemDictionary()
	//{
	//return numOfItem;
	//}
	public void Use()
	{
		if (selectItem.kindOfItem == ToolItem.KindOfItem.UseItem)
		{

		}
		int pas = selectItem.inventoryNum;
		selectItem.inventoryNum = pas - changeNum;

	}
	public void DumpTool()
    {
		int pas = selectItem.inventoryNum;
		selectItem.inventoryNum = pas - changeNum;
	}

}