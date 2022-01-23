using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWindow : ValueChangeBase
{
    //�ǂ̃^�C�v�����Ă��
    [SerializeField]
    int type;

    //fase�ݒ�Y�ꂩ

    // Start is called before the first frame update
   protected  void Start()
    {
        fase = 1;
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
        else if(type == 3)
        {
            numberSave |= 0b000001000;
        }
        else
        {
            numberSave |= 0b000010000;
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
        else if(type == 3)
        {
            numberSave &= 0b11110111;
        }
        else
        {
           // Debug.Log($"��{numberSave & 0b11101111}");
            numberSave &= 0b11101111;
        }
        ApplyValue();
    }

    public void ApplyType()
    {
        numberSave = GetType(MainUI.instance.settingNumber,MainUI.instance.editNumber);

        bool value = GetComponent<Toggle>().isOn;
        if (value)
        {
            TypeAdd();
        }
        else
        {
            TypeDelete();
        }
    }

    /// <summary>
    /// �Q�Ƃ��Ă�^�C�v���m�F
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    int GetType(int s, int e)
    {
        if (s == 1)
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstTarget.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondTarget.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdTarget.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthTarget.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fiveTarget.percentage;
            }
        }
        else if (s == 3)
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstPlan.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondPlan.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdPlan.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthPlan.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fivePlan.percentage;
            }
        }
        else
        {
            if (e == 1)
            {
                return MainUI.instance.editParameter.firstRecover.percentage;
            }
            else if (e == 2)
            {
                return MainUI.instance.editParameter.secondRecover.percentage;
            }
            else if (e == 3)
            {
                return MainUI.instance.editParameter.thirdRecover.percentage;
            }
            else if (e == 4)
            {
                return MainUI.instance.editParameter.forthRecover.percentage;
            }
            else
            {
                return MainUI.instance.editParameter.fiveRecover.percentage;
            }
        }
    }

}
