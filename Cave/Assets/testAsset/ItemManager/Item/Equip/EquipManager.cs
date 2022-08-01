using System.Collections.Generic;
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
	[HideInInspector] public bool isWeaponM;
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
		//////Debug.log($"武器窓選択{isWeaponM}");
		//////Debug.log(GetItem("テスト3")5);



	}

	public void NumberAdd()
	{


	}






	private void OnTriggerStay2D(Collider2D collision)
	{


	}

	public void ChangeNum(Equip[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{

		}
	}



	public void DumpEquip()
	{
		
		//5 = pas - changeNum;
	}

}