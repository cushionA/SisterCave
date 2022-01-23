using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveWinCon : MonoBehaviour
{
    //‘I‚Ô‚Ù‚¤‚ð“ü‚ê‚é
    [SerializeField]
    Selectable[] selectList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


    public void WindowSet(int call)
    {
//        Debug.Log($"‚ª‚ ‚ {call}");
        //‚Ü‚¸‘S’âŽ~
        for (int i = 0;i <= selectList.Length -1;i++)
        {
            selectList[i].transform.parent.gameObject.SetActive(false);
        }
        selectList[call].transform.parent.gameObject.SetActive(true);
        selectList[call].Select();


    }


    

}
