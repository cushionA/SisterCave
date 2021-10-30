using Guirao.UltimateTextDamage;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance = null;
    public AnimationCurve jMove;
    public AnimationCurve jPower;
    [HideInInspector] public string PMagicTag = "PlayerMagic";
    [HideInInspector] public string SMagicTag = "SisAttack";
    [HideInInspector] public string AttackTag = "Attack";
    [HideInInspector] public string JumpTag = "JumpTrigger";
    public UltimateTextDamageManager um;
    public LayerMask rayFilter;
    ///<summary>
    ///マスクしてやれば地面しか検出しない。
    ///</summary>
    public ContactFilter2D filter;

    public AssetReference Dispari;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


}
