using UnityEngine;
using PathologicalGames;
using UnityEngine.AddressableAssets;



/// <summary>
/// オブジェクトプールの生成と破棄などの処理をデリゲートで上書き
/// </summary>
public class MyInstanceHandlerDelegate : MonoBehaviour
{
	private void Awake()
	{
		// 早急にGlobal PoolManagerデリゲートを設定する。これらは常に利用可能です。
		InstanceHandler.InstantiateDelegates += this.InstantiateDelegate;
		InstanceHandler.DestroyDelegates += this.DestroyDelegate;
	}

	private void Start()
	{
		// プールはAwake時に作られることが多いので、SpawnPoolデリゲートにはStart以降を使う
		// override destroy delegateも設定できるが、この例ではこの1つしかない。
		//PoolManager.Pools["Shapes"].instantiateDelegates += this.InstantiateDelegateForShapesPool;
	}

	public GameObject InstantiateDelegate(GameObject location, Vector3 pos, Quaternion rot)
	{
		Debug.Log("Using my own instantiation delegate on prefab '" + location + "'!");

		return Addressables.InstantiateAsync( location, pos,rot).Result;
	}

	public void DestroyDelegate(GameObject instance)
	{
		Debug.Log("Using my own destroy delegate on '" + instance.name + "'!");

		Addressables.ReleaseInstance(instance);
	}

	public GameObject InstantiateDelegateForShapesPool(GameObject prefab, Vector3 pos, Quaternion rot)
	{
		Debug.Log("Using my own instantiation delegate for just the 'Shapes' pool on prefab '" + prefab.name + "'!");

		return Object.Instantiate(prefab, pos, rot) as GameObject;
	}

}
