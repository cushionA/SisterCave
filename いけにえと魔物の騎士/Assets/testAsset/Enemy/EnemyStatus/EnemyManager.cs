using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance = null;

    public AnimationCurve jMove;
    public AnimationCurve jPower;
    ///<summary>
    ///マスクしてやれば地面しか検出しない。
    ///</summary>
    public ContactFilter2D filter;


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
