using System.Collections.Generic;
using UnityEngine;


public class MagicManager : MonoBehaviour
{

	public static MagicManager instance = null;
	//[HideInInspector] public GameObject selectButton;
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
//	private Dictionary<Magic, int> numOfItem = new Dictionary<Magic, int>();

	////string playerTag = "Player";


	//[HideInInspector]
	

	
	//[HideInInspector]public Magic use;

	// Use this for initialization
	void Start()
	{



		//////Debug.log(GetItem("ナイフ").GetInformation());
		//////Debug.log(numOfItem[GetItem("ハーブ")]);

	}


	private void Update()
	{
		//////Debug.log(GetItem("test4").inventoryNum);
		//////Debug.log(GetItem("テスト3").inventoryNum);



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

	public void ChangeNum(Magic[] takeItem,int[] changeNum)
	{

		for(int i = 0;i < takeItem.Length; i++)
        {
			int pas = takeItem[i].inventoryNum;
			takeItem[i].inventoryNum = pas + changeNum[i];
			//isUp = true;
			/*int pas = numOfItem[GetItem($"{takeItem}")];
			numOfItem[GetItem($"{takeItem}")] = pas + changeNum;
			isUp = true;*/
			if(takeItem[i].inventoryNum < 0)
            {
				takeItem[i].inventoryNum = 0;

			}
		}
	}



}
