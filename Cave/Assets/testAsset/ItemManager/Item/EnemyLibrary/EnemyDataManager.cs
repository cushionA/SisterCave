using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager : MonoBehaviour
{
	public static EnemyDataManager instance = null;
//	[HideInInspector] public GameObject selectButton;
	[HideInInspector] public EnemyLibrary selectItem;
	[Header("出現させるウインドウ。装備窓とかあってもいいかも")]
	public GameObject selectWindow;
	[HideInInspector] public bool isUseMenu;

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
	//private Dictionary<EnemyLibrary, int> numOfItem = new Dictionary<EnemyLibrary, int>();

	//string playerTag = "Player";
	//[HideInInspector]

//	[HideInInspector]
	

	
	//[HideInInspector]public EnemyLibrary use;

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

	public void ChangeNum(EnemyData[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{

		}
	}




}