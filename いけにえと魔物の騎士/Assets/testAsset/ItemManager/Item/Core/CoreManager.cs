using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{

	public static CoreManager instance = null;

	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public CoreItem selectItem;
	public GameObject selectWindow;
	public GameObject equipWindow;//装備する、のボタン
	[HideInInspector] public bool isUseMenu;
	[HideInInspector] public bool isEquipMenu;

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
	private CoreDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<CoreItem, int> numOfItem = new Dictionary<CoreItem, int>();

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

	bool isUp;
	//[HideInInspector]public CoreItem use;

	// Use this for initialization
	void Start()
	{

		for (int i = 0; i < toolDataBase.GetItemLists().Count; i++)
		{
			//　アイテム数を適当に設定
			numOfItem.Add(toolDataBase.GetItemLists()[i], 0);

			//　確認の為データ出力
			////Debug.log(toolDataBase.GetItemLists()[i].GetItemName() + ": " + toolDataBase.GetItemLists()[i].GetInformation());
		}

		////Debug.log(GetItem("ナイフ").GetInformation());
		////Debug.log(numOfItem[GetItem("ハーブ")]);

	}


	private void Update()
	{
		////Debug.log(GetItem("test4").inventoryNum);
		////Debug.log(GetItem("テスト3").inventoryNum);



	}

	public void NumberAdd()
	{


	}




	//　名前でアイテムを取得
	public CoreItem GetItem(string searchName)
	{
		return toolDataBase.GetItemLists().Find(itemName => itemName.itemName == searchName);
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
	public Dictionary<CoreItem, int> GetItemDictionary()
	{
		return numOfItem;
	}
	public void DumpCore()
	{
		selectItem.inventoryNum -= changeNum;
		changeNum = 1;
	}


}