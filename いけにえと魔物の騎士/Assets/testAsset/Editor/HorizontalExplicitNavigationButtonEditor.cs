using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
//Unityエディタのみで実行したい処理を記述。インスペクタ拡張。実行時には無効
using UnityEditor;
using UnityEditor.UI;
#endif

#if UNITY_EDITOR
//Unityエディタのみで実行したい処理を記述
[CustomEditor(typeof(HorizontalExplicitNavigationButton), true)]
//改造するエディタのスクリプトを選択
[CanEditMultipleObjects]

public class HorizontalExplicitNavigationButtonEditor : ButtonEditor
{
    private SerializedProperty selectOnLeftProperty;
    //Editor上で表示できるプロパティを設定？
    private SerializedProperty selectOnRightProperty;
    //同上

    //UIの書式というか形？外観？をButtonEditor.OnInspectorGuiから継承？
    /// <inheritdoc />
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //カスタムエディタを作るメソッド
        (this.target as Selectable).navigation = Navigation.defaultNavigation;
        //Editor型のtargetオブジェクトをSelectable型にキャストし、Navigation型のdefauitNavigationに代入。
        //つまりインスペクタにSelectable型を含むゲームオブジェクトを入れたらその情報をやり取りしてくれる？

        this.serializedObject.Update();
        //表示されるオブジェクトの形式を更新する
        EditorGUILayout.PropertyField(this.selectOnLeftProperty);
        EditorGUILayout.PropertyField(this.selectOnRightProperty);
        //SelectedOnleftとRightを表示する
        this.serializedObject.ApplyModifiedProperties();
        //プロパティの変更を適用
    }

    //UIの書式というか形？外観？をButtonEditor.OnEnableから継承？
    /// <inheritdoc />
    protected override void OnEnable()
    {
        base.OnEnable();
        //UIに表示されるプロパティにselectOnLeftとRightとして選択されたものを入れる？
        this.selectOnLeftProperty = this.serializedObject.FindProperty("selectOnLeft");
        this.selectOnRightProperty = this.serializedObject.FindProperty("selectOnRight");

    }
}
#endif

