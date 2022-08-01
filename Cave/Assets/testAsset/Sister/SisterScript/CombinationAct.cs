using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CombinationAct : MonoBehaviour
{
    [SerializeField] SisterBrain sb;
    

    public void CombinationDo(SisterCombination combo,int chainNumber)
    {
        //ワープ
        if(combo._sortNumber == 1)
        {
            Vector2 warpPosi = SManager.instance.target.transform.position;
            

            float exY = sb.RayGroundCheck(warpPosi);

            //地表から十五以下の距離なら設置
            warpPosi.y = warpPosi.y - exY <= 15 ? exY : warpPosi.y;

            if(SManager.instance.target.transform.localScale.x > 0)
            {
                warpPosi.Set(warpPosi.x - 10,warpPosi.y);

                GManager.instance.Player.transform.position = warpPosi;
                if (GManager.instance.Player.transform.localScale.x < 0)
                {
                    GManager.instance.pm.Flip();
                }
            }
            else
            {
                warpPosi.Set(warpPosi.x + 10, warpPosi.y);

                GManager.instance.Player.transform.position = warpPosi;
                GManager.instance.pm.Flip();
                if (GManager.instance.Player.transform.localScale.x > 0)
                {
                    GManager.instance.pm.Flip();
                }
            }
            Transform gofire = GManager.instance.PlayerEffector.transform;
            //gofire.localScale *= 0.8f;
            gofire.localScale = GManager.instance.Player.transform.localScale;
            Addressables.InstantiateAsync("WarpCircle", gofire);//.Result;//発生位置をPlayer
            GManager.instance.PlaySound("Warp", gofire.position);
            //return 1;
        }
       // return 0;
    }
}
