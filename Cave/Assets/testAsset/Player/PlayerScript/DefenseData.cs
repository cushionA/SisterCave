using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseData
{


    public float phyCut;//カット率
	public float holyCut;//光。
	public float darkCut;//闇。
	public float fireCut;//魔力
	public float thunderCut;//魔力

	public float guardPower;//受け値

    //　無属性防御力。体力で上がる
    public float Def = 70;
    //刺突防御。筋力で上がる
    public float pierDef = 70;
    //打撃防御、技量で上がる
    public float strDef = 70;
    //神聖防御、筋と賢さで上がる。
    public float holyDef = 70;
    //闇防御。賢さで上がる
    public float darkDef = 70;
    //炎防御。賢さと生命で上がる
    public float fireDef = 70;
    //雷防御。賢さと持久で上がる。
    public float thunderDef = 70;

    /// <summary>
    /// プレイヤーのみかな
    /// </summary>
    public bool nowParry;

    public bool isGuard;

    /// <summary>
    /// trueの時攻撃を受けるとダメージが多くなる。
    /// 各主体のcalcで決定する。
    /// </summary>
    public bool isDangerous;

    public bool attackNow;

    public float nowArmor;
}
