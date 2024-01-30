using Cysharp.Threading.Tasks;
using MoreMountains.CorgiEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackData;

/// <summary>
/// �e�ۂ��ꔭ�ȏ�̎��ɋ������R���g���[�����Ă������
/// ���Ə���n���Ă����
/// �ʒu�𐮂����ڂ����i���̏ꍇ�ŏ��A�e�ۂ��A�N�e�B�u������O�ɂ��B��A�N�e�B�u��Ԃ��ƍ��W����ɕςȕ��ׂ�������Ȃ��j
/// �P���̏ꍇ����͂���Ȃ�
/// �����܂ŕ����̒e�ۂ��܂Ƃ߂鏈��
/// ���������ĒP�����ǂ����Œe�ۂ̏��łƌĂяo���̏������ς��H
/// 
/// ���߂���@�\
/// �E�e�ۂ��K�v�Ƃ�������󂯎���ĕێ�����@�\�i���͎Q�Ƃ��ǂ��Ēe�ۂ̕������ɗ���j�@�~�@controll�A�r���e�B�ƌq����
/// �E�e�ۂ����ԂɃA�N�e�B�u�����Ă����@�\�B�i�ʂɕb�����߂��j�@�Z
/// �E�A�N�e�B�u���Ɠ����ɐe�q�֌W��؂藣���@�Z
/// �E�Q�Ƃ��ǂ��Ēe�ۂ��I���Ăяo�����Ă������A�N�e�B�u��������B�S����A�N�e�B�u������������v�[���ɋA��@�Z
/// �E�^�[�Q�b�g�����݂ł��邩����������������@�\�B�����n�͈ꊇ�ň�x�����@�~
/// �E�^�[�Q�b�g�̈ʒu���X�V��������@�\�@�~
/// �E���@���q�b�g�������A�q�b�g�񍐂��󂯂ăq�b�g�񕜂Ȃǂ̌��ʂ��󂯂�
/// 
/// ���
/// �E�}�Y���t���b�V���n�͂ǂ����邩�i�e���@���A�N�e�B�u���Ɏ����ł�����atef��ʂ��ČĂяo���H�j
/// �E�P���͏��������邩�H�i�����Ȃ��B��A�N�e�B�u�̎��ɐe�q�֌W�̑���������Ȃ��΂����j
/// �E�ʁX�̋����̒e�ۂ�����ɂ́H�@���o���b�g�R���g���[���[�ɖ��@�����Ă����B�e�ۂ̋����f�[�^�Ɩ��@�f�[�^�𕪂���H
/// �Epool����̌Ăяo����particle����Ȃ��Ȃ�B���e�G�t�F�N�g�Ǝq���@�͕����邩�H
/// �E�v�[���ւ̃G�t�F�N�g�ǉ��ǂ����邩�i�e�ۏ��݂����ɂ��Ĕz��ł܂Ƃ߂邩�B���@�̒e�ۏ��̂ǂ���Q�Ƃ��邩�݂����Ȃ̂�ԍ��Ŏ������Ƃ��j
/// 
/// 
/// �E�A�C�e���Ƃ��Ẵf�[�^
/// �E�����f�[�^�i���x�A�ǔ��`���A�e�ۂ̐������ԁj
/// �E�U���̓f�[�^�i�q�b�g�񐔁A���[�V�����l�A�U���́j�@�e�ۏ����i�U���͂������@���玝���Ă���j
/// �E�G�t�F�N�g�f�[�^�i�t���b�V���G�t�F�N�g�A���e�G�t�F�N�g�A���@�G�t�F�N�g�j�@�e�ۏ����i�t���b�V���G�t�F�N�g�ƒ��e�G�t�F�N�g�́j
/// �E�r���f�[�^�i�r�����ԁA���[�V�����w��j�@���@����
/// �E�o���b�g�R���g���[���[�v���n�u�ւ̎Q�Ɓ@���@����
/// 
/// 
/// </summary>
public class BulletController : MonoBehaviour
{

    /// <summary>
    /// �e�ۂ��R���g���[������̂ɕK�v�ȏ��
    /// </summary>
    public struct BulletControllData
    {

        /// <summary>
        /// ��������܂ł̎���
        /// </summary>
        [Header("���������܂ł̎���")]
        public float emitSecond;

        [Header("�����ʒu")]
        public Vector2 firstPosition;

        [Header("�e�ۃI�u�W�F�N�g")]
        public FireBullet bullet;

    }


    /// <summary>
    /// �ŏ��ɏ���������
    /// �e�ې�
    /// </summary>
    int bulletCount;

    /// <summary>
    /// ��A�N�e�B�u�ɂȂ����I�u�W�F�N�g�𐔂���
    /// �S����A�N�e�B�u�ɂȂ�����v�[���ɋA��
    /// </summary>
    int disenableCount;


    /// <summary>
    /// �e�ۂ̔z��
    /// �������N������
    /// </summary>
    [SerializeField]
    BulletControllData[] bullets;


    /// <summary>
    /// �G�t�F�N�g�R���g���[���[
    /// </summary>
    AtEffectCon atEf;



    /// <summary>
    /// �U���{��
    /// </summary>
    public AttackMultipler multipler;

    #region�@�����E���Ōn�̃��\�b�h

    /// <summary>
    /// �ŏ��ɒe�ۂ𐶐����Ă�������
    /// </summary>
    void BulletGenerater()
    {
        //�e�ۂ���R����Ȃ�
        if (bulletCount > 1)
        {
            for (int i = 0; i < bulletCount; i++)
            {
                //�e�ې���������U�蕪���Ă���
                BulletGenerateExe(bullets[i]).Forget();
            }
        }
        //�e�ۂ���Ȃ�
        else
        {
            //�e�ې���������U�蕪���Ă���
            BulletGenerateExe(bullets[i]).Forget();
        }
    }


    /// <summary>
    /// �e�ې������s
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    async UniTaskVoid BulletGenerateExe(BulletControllData data)
    {
        //�ʒu���ŏ��ɖ߂�
        data.bullet.gameObject.transform.position = data.firstPosition;

        //�e�q�֌W��r��
        data.bullet.gameObject.transform.parent = null;

        //��]�������邩�H
        //0�ɂ��悤�A����e�ۂ��N�����ɏ���ɕς���΂����������Ō��߂�����

        if (data.emitSecond > 0)
        {
            //�҂����Ԃ���Ȃ�҂�
            await UniTask.Delay(TimeSpan.FromSeconds(data.emitSecond));
        }

        //�e�ۂ�L����
        data.bullet.gameObject.SetActive(true);
    }




    /// <summary>
    /// �e�ۂ��������郁�\�b�h
    /// ��A�N�e�B�u����e�q�֌W��߂�
    /// �S�Ė߂�����v�[�����������
    /// 
    /// ���Ȃ݂Ƀ}�Y���t���b�V���͎��Ԍo�߂ŏ���ɏ�����͂��ł�
    /// </summary>
    /// <param name="bullet"></param>
    public void BulletClear(FireBullet bullet)
    {
        //���łɔ�A�N�e�B�u�Ȃ�
        //�܂������������Ă�Ȃ瑽�d�Ăяo���͋���
        if(bullet.gameObject.activeSelf == false)
        {
            return;
        }


        //������
        bullet.gameObject.SetActive(false);

        //�e�I�u�W�F�N�g�ɖ߂�
        bullet.gameObject.transform.parent = this.transform;

        //�J�E���g���v���X��
        disenableCount++;

        //�e�ۂ̐����A���Ă����e�ۂ̐���������
        //�܂�S�Ă̒e�ۂ��Ԃ��Ă����Ǝv����Ƃ�����
        //�q�I�u�W�F�N�g�̐��m�F������
        //�T�d�ɂ��
        if(disenableCount >= bulletCount)
        {
            //�q�I�u�W�F�N�g�ɂ݂�Ȗ߂��Ă���Ȃ�
            if(transform.childCount >= bulletCount)
            {

                //�v�[���ɋA��
                atEf.BulletClear(this.transform);
            }

        }


    }

    #endregion
}
