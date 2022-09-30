using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 片方にだけあればいいなこれ
/// </summary>
public class SaveWinBase : MonoBehaviour
{
    [SerializeField]
    Selectable back;
    //保存処理はボタンイベントでいいじゃんねぇ



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
        {
            SaveEnd();
      //      Debug.Log("ssss");
        }

    }

    public void SaveEnd()
    {
        transform.root.gameObject.SetActive(false);
        back.Select();

    }
    /// <summary>
    /// セーブしてなくても終われるようにする
    /// オペG窓の場合これやった後にEditEnd呼べばいい
    /// </summary>
    public void RejectSave()
    {

        MainUICon.instance.isSave = true;
        SaveEnd();
    }


}
