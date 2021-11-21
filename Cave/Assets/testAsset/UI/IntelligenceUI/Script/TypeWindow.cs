using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeWindow : ValueChangeBase
{
    //�ǂ̃^�C�v�����Ă��
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
    /// �g�O�����^�̂Ƃ��Ă΂��
    /// </summary>
    void TypeAdd()
    {
        //���m1,��Ԃ��2,Shooter,Knight4,Trap8,��킸0

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
    /// �g�O�����U�̎��Ă΂��
    /// </summary>
    void TypeDelete()
    {
        //���m1,��Ԃ��2,Shooter,Knight4,Trap8,��킸0

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
