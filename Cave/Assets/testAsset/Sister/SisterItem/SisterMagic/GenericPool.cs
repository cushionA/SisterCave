using System.Collections.Generic;
using UnityEngine;

// 名前がGenericなのはジェネリクスを使おうとして失敗した名残。。
public class GenericPool : MonoBehaviour
{
    // ここにプールしたいPrefabをエディタで指定します
    [SerializeField]
    private PoolableObject PooledObject;

    // プールのサイズを指定しておきます（メモリ節約）
    // 必要に応じて調整してください。
    private const int PoolSize = 5;
    // プールの実態。Queueを使用します。
    private readonly Queue<PoolableObject> _pool =
                                       new Queue<PoolableObject>(PoolSize);
    // どうでもいい定数。あんまり回転いじらないのでゼロを作ってるだけ。
    private static readonly Quaternion NoRotation = Quaternion.Euler(0, 0, 0);

    /// <summary>
    /// プールにオブジェクトがあればそれを利用します。
    /// 無ければ新たにオブジェクトをInstantiateします。
    /// </summary>
    /// <param name="position"></param>
    public T Place<T>(Vector2 position) where T : PoolableObject
    {
        return (T)Place(position);
    }

    /// <summary>
    /// プールにオブジェクトがあればそれを利用します。
    /// 無ければ新たにオブジェクトをInstantiateします。
    /// </summary>
    /// <param name="position"></param>
    public PoolableObject Place(Vector2 position)
    {
        PoolableObject obj;
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
            //キューの先頭にあるオブジェクトを削除しつつ返す

            obj.gameObject.SetActive(true);
            obj.transform.position = position;
            obj.Init();
        }
        else
        {
            obj = Instantiate(PooledObject, position, PooledObject.transform.rotation);
            obj.Pool = this;
            obj.Init();
        }
        return obj;
    }

    /// <summary>
    /// オブジェクトをプールに戻します
    /// </summary>
    /// <param name="obj"></param>
    public void Return(PoolableObject obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

}