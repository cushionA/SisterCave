using UnityEngine;
using System.Collections;

public class DogPile : MonoBehaviour
{

	public GameObject[] enemyList;//オブジェクトを守る敵
	public GameObject[] destroyObjectList;//敵撃破で壊れるオブジェクト

	void Start()
	{
		InvokeRepeating("CheckEnemy", 0.0f, 1.0f);
	}

	void CheckEnemy()
	{
		// 登録されている敵リストから敵の生存状態を確認
		// （1秒に1回でもよい）
		bool flag = true;
		foreach (GameObject enemy in enemyList)
		{
			if (enemy != null)
			{
				flag = false;
			}
		}

		// すべての敵が倒されているか？
		if (flag)
		{
			// 登録されている破壊物リストのオブジェクトを削除
			foreach (GameObject destroyObject in destroyObjectList)
			{
				//destroyObject.AddComponent<Effect_FadeObject>();消えるときのオブジェクト
				destroyObject.SendMessage("FadeStart");
				Destroy(destroyObject, 1.0f);
			}
			CancelInvoke("CheckEnemy");
		}
	}
}
