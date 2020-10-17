using UnityEngine;

public class GetMagic : MonoBehaviour
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
            MagicManager.instance.takeItem = addTool;
            MagicManager.instance.changeNum = addNum;
            MagicManager.instance.AddItem();
            isFirst = true;
            Destroy(this.gameObject);
        }

    }

}
