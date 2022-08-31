using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.AddressableAssets;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
    /// <summary>
    /// TODO_DESCRIPTION
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/RenderWakeUp")]
    public class RenderWakeUp : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "RenderWakeUp"; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 

        [SerializeField]
        EnemyAIBase eb;
        [System.NonSerialized] public bool cameraEnabled = false;//���J�����ɉf����
        [System.NonSerialized] public bool inActiveZone = false;
        // protected Rigidbody2D _controller;
        string avtiveTag = "ActiveZone";

        bool activeEnter;
        bool activeStay;
        //bool activeOut;
        bool isSleep;//�x�����t���O
                     // Animator ani;




        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
           // eb = GetComponent<EnemyAIBase>();

        }

        void OnWillRenderObject()
        {
            //  if (Camera.current.tag == "MainCamera")�����_�����O���Ă�J���������C���J�����Ȃ���Ă�B�����������炭URP�̂����œ����Ȃ�
            // {

            eb.cameraRendered = true;
            cameraEnabled = true;

            //  }
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
            ////Debug.log($"�J�����Ȃ��Ł[��{eb.cameraRendered}");
           Debug.Log($"�J������{activeStay}");
            if (!cameraEnabled && !isSleep)
            {
                //�J�����ɂ����ĂȂ�
             //   eb.rb.Sleep();
                isSleep = true;
                //Debug.Log("����������");
                _animator.enabled = false;
                eb.cameraRendered = false;
                return;
            }
            if (eb.cameraRendered && isSleep)
            {

               // _controller.WakeUp();
                //isSleep = false;
                isSleep = false;
                _animator.enabled = true;
            }

            if (cameraEnabled)
            {
                if (activeEnter || activeStay)
                {
                    //�A�N�e�B�u�]�[���ɂ���Ȃ�
                    eb.cameraRendered = true;
                    //activeOut = false;
                }
                else
                {
                    //  eb.cameraRendered = false;
                    //  eb.rb.Sleep();
                    //  isSleep = true;
                    //  eb.sAni.enabled = false;
                    cameraEnabled = false;

                }
                activeStay = false;
                activeEnter = false;

            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == avtiveTag)
            {
                activeEnter = true;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.tag == avtiveTag)
            {
                activeStay = true;
            }
        }


    }
}
