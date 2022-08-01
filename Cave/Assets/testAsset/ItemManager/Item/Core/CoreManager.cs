using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{

	public static CoreManager instance = null;

//	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public CoreItem selectItem;
	public GameObject selectWindow;
	public GameObject equipWindow;//装備する、のボタン
	[HideInInspector] public bool isUseMenu;
	[HideInInspector] public bool isEquipMenu;
	[HideInInspector]public int  changeNum;

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
//	private Dictionary<CoreItem, int> numOfItem = new Dictionary<CoreItem, int>();

	//string playerTag = "Player";
	//[HideInInspector]

	//[HideInInspector]
	

	
	//[HideInInspector]public CoreItem use;

	// Use this for initialization
	void Start()
	{



		//////Debug.log(GetItem("ナイフ").GetInformation());
		//////Debug.log(numOfItem[GetItem("ハーブ")]);

	}


	private void Update()
	{
		//////Debug.log(GetItem("test4")5);
		//////Debug.log(GetItem("テスト3")5);



	}

	public void NumberAdd()
	{


	}








	public void ChangeNum(CoreItem[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{

		}
	}

	public void DumpCore()
	{
		//int pas = 5;
		//5 = pas - changeNum;
	}


}