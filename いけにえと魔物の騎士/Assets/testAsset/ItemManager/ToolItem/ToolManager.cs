using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolManager : MonoBehaviour
{

	//　アイテムデータベース
	[SerializeField]
	private ToolDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<ToolItem, int> numOfItem = new Dictionary<ToolItem, int>();

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

	bool isUp;
	//[HideInInspector]public ToolItem use;

	// Use this for initialization
	void Start()
	{

		for (int i = 0; i < toolDataBase.GetItemLists().Count; i++)
		{
			//　アイテム数を適当に設定
			numOfItem.Add(toolDataBase.GetItemLists()[i], 0);

			//　確認の為データ出力
			//Debug.Log(toolDataBase.GetItemLists()[i].GetItemName() + ": " + toolDataBase.GetItemLists()[i].GetInformation());
		}

		//Debug.Log(GetItem("ナイフ").GetInformation());
		//Debug.Log(numOfItem[GetItem("ハーブ")]);
	
		}


    private void Update()
    {
		//Debug.Log(GetItem("test4").inventoryNum);
		//Debug.Log(GetItem("テスト3").inventoryNum);



	}

	public void NumberAdd()
    {


    }




    //　名前でアイテムを取得
    public ToolItem GetItem(string searchName)
	{
		return toolDataBase.GetItemLists().Find(itemName => itemName.GetItemName() == searchName);
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
	public Dictionary<ToolItem,int> GetItemDictionary()
	{
		return numOfItem;
	}



}