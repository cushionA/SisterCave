using UnityEngine;

public class GetEquip : MonoBehaviour
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
            EquipManager.instance.takeItem = addTool;
            EquipManager.instance.changeNum = addNum;
            EquipManager.instance.AddItem();
            isFirst = true;
            Destroy(this.gameObject);
        }

    }

}

