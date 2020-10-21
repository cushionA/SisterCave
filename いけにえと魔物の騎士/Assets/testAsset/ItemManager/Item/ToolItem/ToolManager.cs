using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
	public static ToolManager instance = null;
	[HideInInspector] public GameObject selectButton;
	[HideInInspector]public ToolItem selectItem;
	public GameObject selectWindow;
	public GameObject equipWindow;
	[HideInInspector]public bool isUseMenu;
	[HideInInspector]public bool isEquipMenu;
	[HideInInspector] public int setNumber;//何番目のボタンに入れるかどうか

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

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

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

    public void AddItem()
    {


		int pas = GetItem($"{takeItem}").inventoryNum;
		GetItem($"{takeItem}").inventoryNum = pas + changeNum;
		isUp = true;
		/*int pas = numOfItem[GetItem($"{takeItem}")];
		numOfItem[GetItem($"{takeItem}")] = pas + changeNum;
		isUp = true;*/

	}
	public void ReduceItem()
	{

			/*int pas = numOfItem[GetItem($"{takeItem}")];
			numOfItem[GetItem($"{takeItem}")] = pas - changeNum;
			isUp = true;*/
		int pas = GetItem($"{takeItem}").inventoryNum;
		GetItem($"{takeItem}").inventoryNum = pas - changeNum;
		isUp = true;
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
		selectItem.inventoryNum -= changeNum;
		changeNum = 1;

	}
	public void DumpTool()
    {
		selectItem.inventoryNum -= changeNum;
		changeNum = 1;
	}

}