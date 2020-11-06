using UnityEngine;

public class GetTool : MonoBehaviour
{

    string playerTag = "Player";
    //public ToolManager tm;
    [SerializeField] string addTool;
    [SerializeField] int addNum;
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
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction5) && !isFirst)
        {
            ToolManager.instance.takeItem = addTool;
            ToolManager.instance.changeNum = addNum;
            ToolManager.instance.AddItem();
            isFirst = true;
            Destroy(this.gameObject);
        }

    }

}
