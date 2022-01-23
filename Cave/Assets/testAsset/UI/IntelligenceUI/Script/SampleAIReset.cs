using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAIReset : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    OperationGeneral op;

    [SerializeField]
    SisterParameter sis;



    public void ResetAI()
    {
        op.OparationCopy(MainUI.instance.editParameter, sis,true);
    }

}
