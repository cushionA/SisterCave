using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeWindow : ValueChangeBase
{
    //どのタイプかってやつ
    [SerializeField]
    int type;

    // Start is called before the first frame update
   protected override void Start()
    {
        
    }

    // Update is called once per frame
     void Update()
    {
        
    }

    /// <summary>
    /// トグルが真のとき呼ばれる
    /// </summary>
    void TypeAdd()
    {
        //兵士1,飛ぶやつ2,Shooter,Knight4,Trap8,問わず0

        if (type == 0)
        {
            numberSave |= 0b00000001;
        }
        else if (type == 1)
        {
            numberSave |= 0b00000010;
        }
        else if (type == 2)
        {
            numberSave |= 0b00000100;
        }
        else
        {
            numberSave |= 0b000001000;
        }
        ApplyValue();
    }
    /// <summary>
    /// トグルが偽の時呼ばれる
    /// </summary>
    void TypeDelete()
    {
        //兵士1,飛ぶやつ2,Shooter,Knight4,Trap8,問わず0

        if (type == 0)
        {
            numberSave &= 0b11111110;
        }
        else if (type == 1)
        {
            numberSave &= 0b11111101;
        }
        else if (type == 2)
        {
            numberSave &= 0b11111011;
        }
        else
        {
            numberSave &= 0b11110111;
        }
        ApplyValue();
    }
}
