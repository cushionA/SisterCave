using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantController : MonoBehaviour
{
    // Start is called before the first frame update
    //エンチャント画像の無効有効切り替え
    //エンチャント画像の炎とかの切り替え
    //エンチャント画像のレイヤー制御

    [SerializeField] SpriteRenderer mySr;//自分のレンダラー
    [SerializeField] SpriteRenderer pareSr;//親のレンダラー

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mySr.sortingOrder != pareSr.sortingOrder)
        {
            mySr.sortingOrder = pareSr.sortingOrder;
        }
    }


}
