using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCControllerAbillity : ControllAbillity
{


    /// <summary>
    /// 敵を見つけた時に呼び出すメソッド
    /// </summary>
    public abstract void FindEnemy();


    /// <summary>
    /// 発見したオブジェクトを報告する
    /// </summary>
    /// <param name="isDanger">危険物かどうか</param>
    /// <param name="obj">見つけたもの</param>
    public abstract void ReportObject(bool isDanger,GameObject obj);

}
