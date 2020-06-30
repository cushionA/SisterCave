using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
//Unityエディタのみで実行したい処理を記述。インスペクタ拡張。実行時には無効
using UnityEditor;
using UnityEditor.UI;
#endif

public class HorizontalExplicitNavigationButton : Button
{
    [SerializeField] private Selectable selectOnLeft;
    //左で選択するobject
    [SerializeField] private Selectable selectOnRight;
    //右で選択するobject

    /// <inheritdoc />
    public override Selectable FindSelectableOnUp()
    {
        return null;
        //上は選択できないよう

    }

    /// <inheritdoc />
    public override Selectable FindSelectableOnLeft()
    {
        return this.selectOnLeft;
        //選択するオブジェクトを渡す
    }

    /// <inheritdoc />
    public override Selectable FindSelectableOnRight()
    {
        return this.selectOnRight;
        //選択するオブジェクトを渡す
    }
}
