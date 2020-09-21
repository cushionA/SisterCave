using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SisterMagic", menuName = "SisterMagic")]
public class SisMagic : Magic
{
    public float recobverBase;//回復基礎量
    public float coolTime;
    public float useLimit;//使用可能回数

    public AnimationCurve faithCurve;//信仰補。シスターさんは魔力と信仰


}
