using UnityEngine;
using PathologicalGames;
using UnityEngine.AddressableAssets;



/// <summary>
/// �I�u�W�F�N�g�v�[���̐����Ɣj���Ȃǂ̏������f���Q�[�g�ŏ㏑��
/// </summary>
public class MyInstanceHandlerDelegate : MonoBehaviour
{
	private void Awake()
	{
		// ���}��Global PoolManager�f���Q�[�g��ݒ肷��B�����͏�ɗ��p�\�ł��B
		InstanceHandler.InstantiateDelegates += this.InstantiateDelegate;
		InstanceHandler.DestroyDelegates += this.DestroyDelegate;
	}

	private void Start()
	{
		// �v�[����Awake���ɍ���邱�Ƃ������̂ŁASpawnPool�f���Q�[�g�ɂ�Start�ȍ~���g��
		// override destroy delegate���ݒ�ł��邪�A���̗�ł͂���1�����Ȃ��B
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
