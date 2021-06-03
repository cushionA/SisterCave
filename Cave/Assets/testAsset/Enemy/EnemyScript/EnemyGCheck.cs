using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGCheck : MonoBehaviour
{
    [Header("接地判定用のフィルター")]
    ///<summary>
    ///マスクしてやれば地面しか検出しない。
    ///</summary>
    [SerializeField] ContactFilter2D filter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
