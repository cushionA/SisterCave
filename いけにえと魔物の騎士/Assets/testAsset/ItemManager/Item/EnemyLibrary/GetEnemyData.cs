using UnityEngine;

public class GetEnemyData : MonoBehaviour
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
        if (GManager.instance.InputR.GetButtonDown(MainUI.instance.rewiredAction18) && !isFirst)
        {
            EnemyDataManager.instance.takeItem = addTool;
            EnemyDataManager.instance.changeNum = addNum;
            EnemyDataManager.instance.AddItem();
            isFirst = true;
            Destroy(this.gameObject);
        }

    }

}
