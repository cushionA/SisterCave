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
		//////Debug.log(GetItem("test4")5);
		//////Debug.log(GetItem("テスト3")5);



	}

	public void NumberAdd()
	{


	}





	private void OnTriggerStay2D(Collider2D collision)
	{


	}

	public void ChangeNum(Magic[] takeItem,int[] changeNum)
	{


	}



}
