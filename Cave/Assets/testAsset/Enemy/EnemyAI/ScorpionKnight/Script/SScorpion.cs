using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SScorpion : EnemyBase
{
    //�W�����v���Ă�Ƃ�����������
    //�t���O�̏d���ɋC��t����
    //�����蔻��͂�����x�����Ȃ��Ƃ����Ȃ�

    //�U���ԍ�����
    //0�A�ʏ�U��,�P�A���c�A��,�R�A���З͂̂�����,�S�A�����l�߂�˂�

    float direX;//���[�V������X���������߂�
    float direY;//���[�V�����̂����������߂�
    float attackChanceTime;
    bool rareAttack;
    [SerializeField]bool test;
    // Start is called before the first frame update
    [SerializeField] int tesnum;
    protected override void Start()
    {
      //  rb = GetComponent<Rigidbody2D>();
      //  Debug.Log("mennti");
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

  
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);
    }
    void RandomDirection(int upperRes, int separate)
    {
        if (separate >= upperRes)
        {
            separate = upperRes;
        }
        if (RandomValue(0, upperRes) <= separate)
        {
            direX = 1;
        }
        else
        {
            direX = -1;
        }
    }
}
