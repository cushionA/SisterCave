using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
	public static EquipManager instance = null;
	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public Equip selectItem;
	public GameObject selectWindow;
	[HideInInspector] public bool isUseMenu;
	//　アイテムデータベース
	[SerializeField]
	private EquipDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<Equip, int> numOfItem = new Dictionary<Equip, int>();

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

	bool isUp;
	//[HideInInspector]public Equip use;

	// Use this for initialization

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
	public Equip GetItem(string searchName)
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
	public Dictionary<Equip, int> GetItemDictionary()
	{
		return numOfItem;
	}

	public void DumpEquip()
	{
		selectItem.inventoryNum -= changeNum;
		changeNum = 1;
	}

}