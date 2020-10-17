using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Equip : Item
{
    public List<float> phyBase;//物理攻撃。これが1以上ならモーションにアニメイベントとかで斬撃打撃の属性つける
    public List<float> holyBase;//光。筋力と賢さが関係。生命力だから
    public List<float> darkBase;//闇。魔力と技量が関係
    public List<float> fireBase;//魔力
    public List<float> thunderBase;//魔力
    public float needPower;//必要筋力
    public float needSkill;//必要技量
    public float needInt;//必要な賢さ


    // Start is called before the first frame update
    public List<float> phyCut;//カット率
    public List<float> holyCut;//光。
    public List<float> darkCut;//闇。
    public List<float> fireCut;//魔力
    public List<float> thunderCut;//魔力

    public List<float> guardPower;//受け値

    [HideInInspector]
    public float shock;
}
