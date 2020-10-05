using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemData
{
    public ToolItem[] Contents;

    public UseItemData(params ToolItem[] contents)
    {//これコンストラクタ付きの変数っすね
        this.Contents = contents;
    }
}