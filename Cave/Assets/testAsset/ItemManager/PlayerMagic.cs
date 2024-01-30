using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "PlayerMgic", menuName = "PlayerMagic")]
public class PlayerMagic : Magic
{

    //攻撃モーションとかの儒法も入れるか
    //移動距離みたいなのも

    /// <summary>
    /// 使用するスタミナ
    /// </summary>
    public int useStamina;

}
