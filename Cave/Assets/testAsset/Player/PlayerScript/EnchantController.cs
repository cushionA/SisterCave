using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantController : MonoBehaviour
{
    // Start is called before the first frame update
    //�G���`�����g�摜�̖����L���؂�ւ�
    //�G���`�����g�摜�̉��Ƃ��̐؂�ւ�
    //�G���`�����g�摜�̃��C���[����

    [SerializeField] SpriteRenderer mySr;//�����̃����_���[
    [SerializeField] SpriteRenderer pareSr;//�e�̃����_���[

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mySr.sortingOrder != pareSr.sortingOrder)
        {
            mySr.sortingOrder = pareSr.sortingOrder;
        }
    }


}
