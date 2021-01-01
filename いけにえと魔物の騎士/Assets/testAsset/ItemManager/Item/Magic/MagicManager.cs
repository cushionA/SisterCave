using System.Collections.Generic;
using UnityEngine;


public class MagicManager : MonoBehaviour
{

	public static MagicManager instance = null;
	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public Magic selectItem;
	public GameObject selectWindow;
	public GameObject equipWindow;
	[HideInInspector] public bool isUseMenu;//後で装備窓とかで多分使う
	[HideInInspector] public int setNumber;//セットする魔法の番号
	[HideInInspector] public bool isSisterM;
	[HideInInspector] public bool isKnightM;

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
	private MagicDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<Magic, int> numOfItem = new Dictionary<Magic, int>();

	string playerTag = "Player";
	[HideInInspector]
	public int changeNum;
	[HideInInspector]
	public string takeItem;

	bool isUp;
	//[HideInInspector]public Magic use;

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
	public Magic GetItem(string searchName)
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
	public Dictionary<Magic, int> GetItemDictionary()
	{
		return numOfItem;
	}


}
