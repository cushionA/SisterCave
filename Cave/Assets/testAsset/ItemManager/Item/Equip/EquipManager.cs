﻿using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
	public static EquipManager instance = null;
//	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public Equip selectItem;
	[Header("出現させるウインドウ。装備窓とかあってもいいかも")]
	public GameObject selectWindow;
	public GameObject equipWindow;//装備画面で使う窓
	[HideInInspector] public int setNumber;
	[HideInInspector] public bool isUseMenu;
	[HideInInspector] public bool isWeponM;
	[HideInInspector] public bool isShieldM;
	[HideInInspector] public int changeNum;

	//　アイテムデータベース
	[SerializeField]
	private EquipDataBase toolDataBase;
	//　アイテム数管理
	private Dictionary<Equip, int> numOfItem = new Dictionary<Equip, int>();

	//string playerTag = "Player";
	//[HideInInspector]

//	[HideInInspector]
	

	
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
			//////Debug.log(toolDataBase.GetItemLists()[i].GetItemName() + ": " + toolDataBase.GetItemLists()[i].GetInformation());
		}

		//////Debug.log(GetItem("ナイフ").GetInformation());
		//////Debug.log(numOfItem[GetItem("ハーブ")]);

	}


	private void Update()
	{
		//////Debug.log($"盾窓選択{isShieldM}");
		//////Debug.log($"武器窓選択{isWeponM}");
		//////Debug.log(GetItem("テスト3").inventoryNum);



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

	public void ChangeNum(Equip[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{
			int pas = takeItem[i].inventoryNum;
			takeItem[i].inventoryNum = pas + changeNum[i];
			//isUp = true;
			/*int pas = numOfItem[GetItem($"{takeItem}")];
			numOfItem[GetItem($"{takeItem}")] = pas + changeNum;
			isUp = true;*/
			if (takeItem[i].inventoryNum < 0)
			{
				takeItem[i].inventoryNum = 0;

			}
		}
	}
	public Dictionary<Equip, int> GetItemDictionary()
	{
		return numOfItem;
	}

	public void DumpEquip()
	{
		int pas = selectItem.inventoryNum;
		selectItem.inventoryNum = pas - changeNum;
	}

}