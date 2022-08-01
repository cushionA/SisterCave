using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
	public static KeyManager instance = null;
	//[HideInInspector] public GameObject selectButton;
	[HideInInspector] public KeyItem selectItem;
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
	private KeyDataBase toolDataBase;
	//　アイテム数管理
	//private Dictionary<KeyItem, int> numOfItem = new Dictionary<KeyItem, int>();

	//string playerTag = "Player";
//	[HideInInspector]

	//[HideInInspector]
	

	
	//[HideInInspector]public KeyItem use;

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

	public void ChangeNum(KeyItem[] takeItem, int[] changeNum)
	{

		for (int i = 0; i < takeItem.Length; i++)
		{

		}
	}




}