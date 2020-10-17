using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{
	public static EnemyDataManager instance = null;
	public EnemyLibrary selectEnemy;
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
	private EnemyDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<EnemyLibrary, int> numOfItem = new Dictionary<EnemyLibrary, int>();

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

	bool isUp;
	//[HideInInspector]public EnemyLibrary use;

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
	public EnemyLibrary GetItem(string searchName)
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
	public Dictionary<EnemyLibrary, int> GetItemDictionary()
	{
		return numOfItem;
	}



}