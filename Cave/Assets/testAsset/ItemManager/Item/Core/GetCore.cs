using UnityEngine;

public class GetCore : MonoBehaviour
{

    string playerTag = "Player";
    //public ToolManager tm;
    [SerializeField] CoreItem[] addTool;
    [SerializeField] int[] addNum;
    bool isFirst;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction18) && !isFirst)
        {
            // MagicManager.instance.takeItem = addTool;
            //  MagicManager.instance.changeNum = addNum;
            CoreManager.instance.ChangeNum(addTool, addNum);
            isFirst = true;
            addTool = null;
            Destroy(this.gameObject);
        }

    }

}
