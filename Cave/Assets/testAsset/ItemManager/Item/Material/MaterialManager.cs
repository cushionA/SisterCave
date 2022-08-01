using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{

	public static MaterialManager instance = null;
//	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public MaterialItem selectItem;
	public GameObject selectWindow;
	[HideInInspector] public bool isUseMenu;
	[HideInInspector] public int changeNum;
	//bool isUp;
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
	private MaterialDataBase toolDataBase;
	//　アイテム数管理
//	private Dictionary<MaterialItem, int> numOfItem = new Dictionary<MaterialItem, int>();

	//string playerTag = "Player";
	//[HideInInspector]

	//[HideInInspector]
	

	
	//[HideInInspector]public MaterialItem use;

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






	private void OnTriggerStay2D(Collider2D collision)
	{


	}

	public void ChangeNum(MaterialItem[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{

		}
	}


	public void DumpMaterial()
	{
		//int pas = 5;
	//	5 = pas - changeNum;
	}


}