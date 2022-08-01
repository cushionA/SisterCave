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
    /// �V�X�^�[����̃��[�v
    /// isWarp�Ń��[�v�J�n���ăA�j���[�V�����C�x���g�őJ�ځA�I��
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Abilities/WarpAbility")]
    public class WarpAbillity : MyAbillityBase
    {
        /// ���̃��\�b�h�́A�w���v�{�b�N�X�̃e�L�X�g��\�����邽�߂ɂ̂ݎg�p����܂��B
        /// �\�͂̃C���X�y�N�^�̖`���ɂ���
        public override string HelpBoxText() { return "warp_HELPBOX_TEXT."; }

        //   [Header("����f�[�^")]
        /// declare your parameters here
        ///WeaponHandle�Q�l�ɂ��� 


        // Animation parameters
        protected const string _warpParameterName = "_nowWarp";
        protected int _warpAnimationParameter;

        //----------------------------------------------------------------------------

        [SerializeField]
        private LayerMask layerMask;

        bool isWarp;//���[�v��

        BrainAbility _sister;

        Vector2 warpPoint;

        //----------------------------------------------------------------------------

        /// <summary>
        ///�@�����ŁA�p�����[�^������������K�v������܂��B
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            // randomBool = false;
           
        }

        /// <summary>
        /// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();
        }





        //���[�v��p�̒n�ʒT�m
        public float RayGroundCheck(Vector2 position)
        {

            //�^���Ƀ��C���΂��܂��傤
            RaycastHit2D onHitRay = Physics2D.Raycast(position, Vector2.down, Mathf.Infinity, layerMask.value);

            //  ////Debug.log($"{onHitRay.transform.gameObject}");
            ////Debug.DrawRay(i_fromPosition,i_toTargetDir * SerchRadius);

            //Debug.Log($"������������{onHitRay.transform.gameObject.name}");
            return onHitRay.point.y;
        }

        public void WarpEffect()
        {
            Transform gofire = _sister.firePosition;

            //Transform rotate = SManager.instance.useMagic.castEffect.LoadAssetAsync<Transform>().Result as Transform;

            Addressables.InstantiateAsync("WarpCircle", gofire.position, gofire.rotation);//.Result;//�����ʒu��Player
            GManager.instance.PlaySound("Warp", transform.position);
        }

        /// <summary>
        /// �퓬���ɓ����郏�[�v
        /// </summary>
        public void WarpStart(Vector2 point)
        {
            if (_sister.mp >= 5)
            {
                _sister.mp -= 5;
            }
            _movement.ChangeState(CharacterStates.MovementStates.Warp);
            warpPoint.Set(point.x,point.y);
        }
        //�A�j���[�V�����C�x���g
        public void WarpAct()
        {
            transform.position = warpPoint;
            WarpEffect();
        }

        //�A�j���[�V�����C�x���g
        public void WarpEnd()
        {
            _movement.ChangeState(CharacterStates.MovementStates.Idle);
        }

        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
        {
            RegisterAnimatorParameter(_warpParameterName, AnimatorControllerParameterType.Bool, out _warpAnimationParameter);
        }

        /// <summary>
        /// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
        /// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
        /// </summary>
        public override void UpdateAnimator()
        {
            //�N���E�`���O�ɋC�������
            MMAnimatorExtensions.UpdateAnimatorBool(_animator, _warpAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.Warp), _character._animatorParameters);
        }
    }
}
