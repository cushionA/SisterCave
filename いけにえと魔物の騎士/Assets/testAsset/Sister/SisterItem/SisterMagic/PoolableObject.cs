
using UnityEngine;

// abstractである必要はないけど、僕はこれを単体で使うことがないので
public abstract class PoolableObject : MonoBehaviour
{
    // 使い終わったら戻すためにプールへの参照を持ちます。
    public GenericPool Pool { private get; set; }
    // Start()が呼べないのでInitを別途実装します。
    public abstract void Init();

    /// <summary>
    /// まあ、忘れそうなので。
    /// プールに戻します。（実態はReturnToPool()）
    /// </summary>
    /// <param name="obj"></param>
    protected new void Destroy(Object obj)
    {
        ReturnToPool();
    }

    /// <summary>
    /// プールに戻します。
    /// </summary>
    protected void ReturnToPool()
    {
        Pool.Return(this);
    }
}