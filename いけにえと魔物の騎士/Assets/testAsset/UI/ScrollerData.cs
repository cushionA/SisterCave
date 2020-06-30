using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerData
{
    public string[] Contents;

    public ScrollerData(params string[] contents)
    {
        this.Contents = contents;
    }
}