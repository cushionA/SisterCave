using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{





    [SerializeField]
    List<BattleTest> bt;

    /// <summary>
    /// ��������G�̃r�b�g
    /// </summary>
    public int callNumber = 0b1111;



    [SerializeField]
    Toggle[] _numberSetter;

    [SerializeField]
    /// <summary>
    /// �߂��̑�
    /// </summary>
    Button _returnWindow;




    private void Awake()
    {
        for (int i = 0; i < _numberSetter.Length; i++)
        {

            int num = 0b0001;
            if (i != 0)
            {
                num = 0b0001 << i;
            }
            //�I���Ȃ�
            if ((callNumber & num) > 0)
            {
                _numberSetter[i].isOn = true;
            }
            else
            {
                _numberSetter[i].isOn = false;

            }
        }
    }

    private void Update()
    {
        if (MainUICon.instance._reInput.CancelButton.State.CurrentState == MoreMountains.Tools.MMInput.ButtonStates.ButtonDown)
        {
            NumberSet();
            WindowCancel();
        }
    }

    public void NumberSet()
    {
        for (int i = 0; i < _numberSetter.Length; i++)
        {
            int num = 0b1;
            if (i != 0)
            {
                num = 0b1 << i;
            }
            //�I���Ȃ�
            if (_numberSetter[i].isOn)
            {
                callNumber |= num;
            }
            else
            {
                //num�𔽓]
                num = ~num;
                //�����Ƃ�
                callNumber &= num;
// Debug.Log($"��������{num}");
            }
           
        }
    }

    void WindowCancel()
    {
        _returnWindow.transform.root.gameObject.SetActive(true);
        _returnWindow.Select();
        this.gameObject.SetActive(false);
    }


    public void ResetEnemy()
    {
        for (int i = 0; i < bt.Count; i++)
        {

            bt[i].Reset();

        }
    }

    public bool EnemyExist()
    {
        for (int i = 0; i < bt.Count; i++)
        {
            if (bt[i].Enemy != null)
            {
                return true;
            }

        }
        return false;
    }

    public void AnotherReset()
    {
        NumberSet();
        GManager.instance.HPReset();
        GManager.instance.mp = GManager.instance.maxMp; 
        if (SManager.instance.Sister.activeSelf)
        {
        SManager.instance.Sister.MMGetComponentNoAlloc<BrainAbility>().MPReset();
        }

        if (SManager.instance.Sister.activeSelf) 
        {
            SManager.instance.Sister.MMGetComponentNoAlloc<FireAbility>().MagicEnd();
        }
        for (int i = 0;i < bt.Count;i++)
        {
            int num = 0b0001;
            if (i != 0)
            {
                 num = 0b0001 << i;
            }
            if ((callNumber & num) > 0)
            {
//                Debug.Log($"������{i}");
                bt[i].ReSporn().Forget();
            }
        }
        WindowCancel();
    }
}
