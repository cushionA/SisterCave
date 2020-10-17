using UnityEngine;

public class GetCore : MonoBehaviour
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
        if (Input.GetButtonDown("Submit") && !isFirst)
        {
            CoreManager.instance.takeItem = addTool;
            CoreManager.instance.changeNum = addNum;
            CoreManager.instance.AddItem();
            isFirst = true;
            Destroy(this.gameObject);
        }

    }

}
