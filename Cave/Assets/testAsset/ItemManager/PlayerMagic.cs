using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "PlayerMgic", menuName = "PlayerMagic")]
public class PlayerMagic : Magic
{
    /// <summary>
    /// 矢とかの残弾
    /// </summary>
    public float shootCount;//
    public float magicAromor;//攻撃するときリストの中からその都度攻撃アーマー設定
}
