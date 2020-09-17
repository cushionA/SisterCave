﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

	//　アイテムデータベース
	[SerializeField]
	private ItemDataBase itemDataBase;
	//　アイテム数管理。アイテム型とInt型のdictionary型。アイテム型がキーで、intが値。アイテムの名前と個数を管理。
	private Dictionary<Item, int> numOfItem = new Dictionary<Item, int>();

	// Use this for initialization
	void Start()
	{
		//List<T>.Countはリストの中の要素数を取得するやつ。
		for (int i = 0; i < itemDataBase.GetItemLists().Count; i++)
		{
			//　アイテム数を適当に設定
			numOfItem.Add(itemDataBase.GetItemLists()[i], i);
			//　確認の為データ出力
			Debug.Log(itemDataBase.GetItemLists()[i].itemName + ": " + itemDataBase.GetItemLists()[i].information);
		}

		Debug.Log(GetItem("Fire").information);
		Debug.Log(numOfItem[GetItem("Fire")]);
	}

	//　名前でアイテムを取得
	public Item GetItem(string searchName)
	{
		return itemDataBase.GetItemLists().Find(itemName => itemName.itemName == searchName);
	}
}
