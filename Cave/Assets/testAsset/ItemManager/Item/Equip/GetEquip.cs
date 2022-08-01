using UnityEngine;

public class GetEquip : MonoBehaviour
{

    ////string playerTag = "Player";
    //public ToolManager tm;
    [SerializeField] Equip[] addTool;
    [SerializeField] int[] addNum;
    bool isFirst;

    // Start is called before the first frame update



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction18) && !isFirst)
        {
            // MagicManager.instance.takeItem = addTool;
            //  MagicManager.instance.changeNum = addNum;
            EquipManager.instance.ChangeNum(addTool, addNum);
            isFirst = true;
            addTool = null;
            Destroy(this.gameObject);
        }

    }

}

