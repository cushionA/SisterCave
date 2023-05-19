using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using Rewired.Integration.CorgiEngine;
using DarkTonic.MasterAudio;


/// �U�����͏ꏊ�ς��Ȃ��悤�ɂ���B���f�ŃG�t�F�N�g�c��
/// ���Ɗp�x����ł������m�F
/// z���ʒu����
/// ���ɒǉ�����v�f
/// bool���X�g��MP�ߖ�Ƃ��搧�U���Ƃ��ʂɂ������邩
/// ���邢��MP�g�p�x�݂����Ȃ̂�int�ō��`�F�b�N���Ă锠�łP�A�Q�Ƃ����l�ς��邩
/// ���ꂩ������펝������H
/// �Ή����˂͓������z�[�~���O��ClosestEnemy���^�[�Q�b�g��
/// �G���Ńg���K�[���������Ȃ�
/// ���ꂼ��̏����ɖ��@������
/// �Ȃ������画�f���ĒT��
/// �~�܂�؂ɐG���AI�������ʂɍs���ƈ�U�������@�𔒎��ɂ���

namespace MoreMountains.CorgiEngine // you might want to use your own namespace here
{
	/// <summary>
	/// ���ʂɏ󋵔��f���ē������߂̃X�N���v�g
	/// ���[�v�A�U���A�R���r�l�[�V�����i�e����͕�����H�j�͕ʃX�N���v�g��
	/// ���邢�̓R���r�l�[�V�����͎�l���Ɏ�������H
	/// ����Ƃ��Ă͔��f�A�r���A�U����ԂɑJ�ڂ����[�V�����ω��A�A�j���[�V�����C�x���g�A���@�g�p�A�A�j���[�V�����C�x���g�ŏI��
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Abilities/FireAbility")]
	public class FireAbility : MyAbillityBase
	{

		public override string HelpBoxText() { return "�V�X�^�[����̍U��"; }


		// Animation parameters
		//0���Ȃ���Ȃ��A�P�����@�A�Q���R���r�l�[�V�����A�R���r��
		protected const string _actParameterName = "actType";
		protected int _actAnimationParameter;

		/// <summary>
		/// ���[�V������I��
		/// </summary>
		protected const string _motionParameterName = "motionNum";
		protected int _motionAnimationParameter;

		//�������f��U���̃p�����[�^�[
		public SisterParameter sister;

		/// <summary>
		/// �V�X�^�[����̊�bAI
		/// </summary>
		[HideInInspector]
		public BrainAbility sb;

		/// <summary>
		/// �N�[���^�C���ҋ@�̂��ߐ�p�̃t���O
		/// </summary>
		bool disEnable;


		List<SisMagic> useSupport;//���g�p�̎x��
		List<float> effectiveTime;//�x�����@�A���W�F�l�A�U���̎��Ԃ��͂���
		/// <summary>
		/// �r�����ԑ҂�
		/// </summary>
		 [HideInInspector]
		public float waitCast;

		float coolTime;
		/// <summary>
		/// �U���A�񕜁A�Ȃǂ̗D��s�������ւ���
		/// </summary>
		float stateJudge = 30;
		
		/// <summary>
		/// �G���m�A���f�̎��Ԍv��
		/// </summary>
		float targetJudge = 30;

		/// <summary>
		/// ���ڂ̏����Ń^�[�Q�b�g�����������m�F����
		/// </summary>
		 [HideInInspector]
		public int judgeSequence;

		/// <summary>
		/// �r����������
		/// </summary>
		string castSound = "NormalCastLoop1";

		/// <summary>
		/// �r���̉��炷�T�C��
		/// 1�͂܂��Đ����ĂȂ��B2�͍Đ����B3�͏I���
		/// </summary>
		bool soundStart;
		/// <summary>
		/// ���e�ۂ̐�
		/// </summary>
		int bCount;
		float recoverTime;

		List<GameObject> targetCanList;
		List<EnemyAIBase> targetCanStatus;
		List<SisMagic> magicCanList;



		/// <summary>
		/// ���Ԗڂ̉r���Ȃ̂�
		/// </summary>
		int actionNum;

		[SerializeField] CombinationAbility ca;






		bool fireStart;

		/// <summary>
		/// �퓬�J�n���̏������̂��߂̃t���O
		/// </summary>
		[HideInInspector]
		public bool isReset;
		//�񕜎���
		float healJudge;


		/// <summary>
		/// �e�ې����̍Œ����ǂ���
		/// </summary>
		bool delayNow;
		//�r�b�g���Z�ŃN�[���^�C����j������
		int _skipCondition;



		//protected RewiredCorgiEngineInputManager _inputManager;


		//-------------------------------------------�o�t�̐��l

		/// <summary>
		/// �U���{��
		/// </summary>
		[HideInInspector]
		public float attackFactor = 1;
		[HideInInspector]
		public float fireATFactor = 1;
		[HideInInspector]
		public float thunderATFactor = 1;
		[HideInInspector]
		public float darkATFactor = 1;
		[HideInInspector]
		public float holyATFactor = 1;

		/// <summary>
		/// ���@�g�p�ҋ@���̃G�t�F�N�g
		/// </summary>
		public GameObject castEffect;

		/// <summary>
		/// �e�ې������Ƀ^�[�Q�b�g�������Ă������悤�Ɉʒu���o���Ă���
		/// </summary>
		Vector2 _tgPosition;

		


		/// <summary>
		///�@�����ŁA�p�����[�^������������K�v������܂��B
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			sister.nowMove = sister.priority;

			//Brain����Ȃ���ł���
			sb = GetComponent<BrainAbility>();
		}


		/// <summary>
		/// 1�t���[�����ƂɁA���Ⴊ��ł��邩�ǂ����A�܂����Ⴊ��ł���ׂ������`�F�b�N���܂�
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();


			FireAct();
		}

		/// <summary>
		/// ����R���r�l�[�V�����ɈڐA����H
		/// </summary>
		protected override void HandleInput()
		{

		}

		/// <summary>
		/// �U���̎��s
		/// </summary>
		async void  FireAct()
		{
			//��~���ߏo�Ă邩�ǂ���

			//Debug.Log($"�`�F�b�N{MathF.Floor(waitCast)}");


			//�s�����ĂȂ��Ƃ����͉�
			if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
			{
				recoverTime += _controller.DeltaTime;
				if (recoverTime >= 2.5 && sb.mp < sb.maxMp)
				{
					//�N�[���^�C�����͖��͉�1.2�{
					float recoverAmo = disEnable ? SManager.instance.sisStatus.mpRecover * 1.2f : SManager.instance.sisStatus.mpRecover;

					sb.mp += (recoverAmo + SManager.instance.sisStatus.additionalRecover);
					recoverTime = 0;
				}
			}
			if (sb.mp > sb.maxMp)
			{
				sb.mp = sb.maxMp;

			}
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
			//	
				MagicUse(SManager.instance.useMagic.HRandom, SManager.instance.useMagic.VRandom);
				return;
			}

			//�퓬���A���ʒu�ɂ��Ă���Ȃ�
			else if (sb.nowState == BrainAbility.SisterState.�킢)
			{
				//�퓬�J�n���Ƀt���O�����ꂢ�ɂ��Ă����܂�

				//�퓬�J�n���̏������I������Ȃ�

				//�������@�Ȃ��Ȃ�߂�
				if (sb.status.equipMagic == null)
				{
					return;
				}



				//�^�[�Q�b�g����������r����߂�
				castCheck();


				
				stateJudge += _controller.DeltaTime;

				///�^�[�Q�b�g�Ǝg�p���@�ݒ�
				//�N�[���^�C�����ł��X�L�b�v����������Ȃ瓮��
				#region
				
				if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
				{
				//	Debug.Log($"ss{sister.nowMove}");
				
					//Disenable�Ȃ�N�[���^�C����҂�
					
					CoolTimeWait();
�@�@�@�@�@�@�@�@�@�@targetJudge += _controller.DeltaTime;

					//�ʒu�ɂ��Ă��Ȃ����A�N�[���^�C���������łȂ����X�L�b�v�R���f�B�V�������Ȃ��Ȃ�
                    if ((disEnable && _skipCondition == 0) || !sb.nowPosition)
                    {
						return;
                    }

                    bool reset = false;
					//��莞�Ԍo�߂Ő퓬�v�l���A�Ȃ��łȂ�����͗D�悷���Ԃɖ߂�
					if (stateJudge >= sister.stateResetRes && sister.priority != SisterParameter.MoveType.�Ȃ�)
					{
						//�X�e�[�g���Z�b�g
						sister.nowMove = sister.priority;
						stateJudge = 0.0f;
						reset = true;
					}



					//���Ԍo�߂��^�[�Q�b�g���ł��X�e�[�g�`�F���W�ōĔ��f
					if (targetJudge >= sister.targetResetRes + 2 || SManager.instance.target == null || reset)
                    {
						targetJudge = 0;
						//��莞�Ԃ��ƂɌ��m�������Ȃ�
						//�����Ō��m���Ă���U�����s���܂ł͎��Ԃ��v�����Ȃ����Ăьv�����s���Ȃ�
						//�������Č��m����܂łɃN�[���^�C�����I���΍ĂэU��������
						//���̎��W�I������ł��Ȃ�����̓^�[�Q�b�g��ς��Ȃ�
						sb._sensor.AggresiveSerch();

						await sb._sensor.SerchAsync;

						//���Ⴀ�����Ń^�[�Q�b�g�Đݒ���̂�
						//�ł������Ĕ��f�̎��Ԃ������ƃN�[���^�C���j���ɋC���t���ɂ����Ȃ�

						reset = true;
                    }
                       TargetReset(reset);
				}
				#endregion

				//�����ƃ^�[�Q�b�g�Ǝg�p���@���ݒ肳��Ă��邩�̃`�F�b�N
				//���ꂪ�I���Δ���

				if (SManager.instance.target != null && SManager.instance.useMagic != null)
				{
					//MP�Ȃ��Ȃ瓦����
					if (sb.mp < SManager.instance.useMagic.useMP)
					{
						sb.PositionJudge();
						return;
					}

					//	bool isWrong = false;
					//�g�p���@���U���ŁA���^�[�Q�b�g���G�ł���B
					if (SManager.instance.useMagic.mType == SisMagic.MagicType.Attack)
					{
						if (sister.nowMove != SisterParameter.MoveType.�U��)
						{
							return;
						}

						coolTime = sister.attackCT[judgeSequence];
						_skipCondition = sister.atSkipList[judgeSequence];
					}
					else if (SManager.instance.useMagic.mType == SisMagic.MagicType.Recover)
					{
						if (sister.nowMove != SisterParameter.MoveType.��)
						{
							return;
							//isWrong = true;
						}
						coolTime = sister.healCT[judgeSequence];
						_skipCondition = sister.hSkipList[judgeSequence];
					}
					else
					{
						if (sister.nowMove != SisterParameter.MoveType.�x��)
						{
							return;
						}
						coolTime = sister.supportCT[judgeSequence];
						_skipCondition = sister.sSkipList[judgeSequence];
					}

					if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					{
						//Debug.Log($"ssd{(int)SManager.instance.useMagic.castType}");
						actionNum = (int)SManager.instance.useMagic.castType;
						_movement.ChangeState(CharacterStates.MovementStates.Cast);
						_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
						_controller.SetHorizontalForce(0);
					}


					//�^�[�Q�b�g�����Ďg�p���閂�@�������Ďg�p���@�ƃ^�[�Q�b�g�����ݍ����Ă���Ȃ�

					ActionFire();

				}


			}

			//���ݐ킢������Ȃ��Ȃ�
			else if (sb.nowState != BrainAbility.SisterState.�킢 )
			{
				//�ꉞ���Z�b�g�Ɛ퓬�J�n���Ƀ��Z�b�g���Ă��炦��悤�d����


				if (sister.autoHeal && !disEnable)
				{
					//bool healAct = false;


					if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
					{
						healJudge += _controller.DeltaTime;
						if (healJudge >= 3f)
						{

						for (int i = 0; i < sister.recoverCondition.Length; i++)
						{
								if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
								{
									continue;
								}
								if (HealJudge(sister.nRecoverCondition[i]))
								{
									RecoverAct(sister.nRecoverCondition[i]);
									judgeSequence = i;
									
									break;
								}
						}


							healJudge = 0;
						}
						else if (healJudge < 3f)
						{

							SManager.instance.target = null;
						}
					}
					if (SManager.instance.target != null && SManager.instance.useMagic != null)
					{

						if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
						{
					//		Debug.Log("hhhhhhj");
							actionNum = (int)SManager.instance.useMagic.castType;
							_movement.ChangeState(CharacterStates.MovementStates.Cast);
							_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
						}
						coolTime = sister.autHealCT[judgeSequence];
						_skipCondition = sister.ahSkipList[judgeSequence];

						ActionFire();
					}
				}

			}



		}

		/// <summary>
		/// �r���Ǘ����\�b�h
		/// �r������������U����Ԃɓ����ă��[�V�����J��
		/// </summary>
		/// <param name="random"></param>
		public void ActionFire(float random = 0.0f)
		{
			if (disEnable)
			{
				waitCast = 0;
				disEnable = false;
			}

			//�����_���ɓ���Ă��������Ǖ��ʂɓ���Ă�����

			//�g�p���閂�@��MP�������āA���W�I������Ȃ�
			if (sb.mp >= SManager.instance.useMagic.useMP && SManager.instance.target != null)
			{
				//�����J�n
				//
				waitCast += _controller.DeltaTime;
				_controller.SetHorizontalForce(0);
				float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
				sb.SisFlip(dir);

				

				//�r���I�������
				if (waitCast >= SManager.instance.useMagic.castTime)
				{


					
					soundStart = false;
					disEnable = true;
					//GManager.instance.StopSound(castSound, 0.5f);
					_movement.ChangeState(CharacterStates.MovementStates.Attack);
					waitCast = 0;

					actionNum = (int)SManager.instance.useMagic.FireType;
					Addressables.InstantiateAsync(SManager.instance.useMagic.castBreak, sb.firePosition.position, sb.firePosition.rotation);
					if (castEffect != null)
					{
						Vector3 pos = new Vector3(transform.position.x, transform.position.y, -100);
						castEffect.transform.position = pos;
						Addressables.ReleaseInstance(castEffect);
						castEffect = null;
					}
					//��������̏����ł̓A�j���[�V�����C�x���g���g��
				}
				//�r�����Ȃ�
				else if(waitCast > 0.3)
				{

					//sb.//(SManager.instance.useMagic.castAnime);
					if (!soundStart)
					{
						 CastCircle().Forget();

						GManager.instance.PlaySound(castSound, transform.position);
						
						soundStart = true;
					}
				}
			}
			//Mp����Ȃ����^�[�Q�b�g�������Ȃ�
			else
			{
				//	�r�����~�B�G������ŏ�������Ƃ�
				GManager.instance.StopSound(castSound, 0.5f);


				//�Ƃ肠�����J�����������
				Debug.Log("�ꂃ");
				Vector3 pos = new Vector3(transform.position.x, transform.position.y, -100);
                 castEffect.transform.position = pos;
					Addressables.ReleaseInstance(castEffect);
					castEffect = null;
			//	}
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				actionNum = 0;
				waitCast = 0;
			//	Debug.Log("adedwdwadwd");
				return;
			}
		}

		/// <summary>
		/// X��Y�̊Ԃŗ������o��
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
		public int RandomValue(int X, int Y, bool setSeed = false, int seed = 0)
		{
			//Random rnd = new UnityEngine.Random();

			/*	if (setSeed)
				{
					Random.InitState(seed);
				}*/
			//MyRandom random = new MyRandom();
			return UnityEngine.Random.Range(X, Y + 1);

		}




		/// <summary>
		///���͉񕜂Ȃǂ̂��߂ɍs�����ƂɃN�[���^�C���ݒ�\ 
		/// </summary>
		void CoolTimeWait()
		{
			if (disEnable && _condition.CurrentState != CharacterStates.CharacterConditions.Moving)
			{
			//	Debug.Log($"ddddd{coolTime}����{waitCast}d{SManager.instance.useMagic}");
				waitCast += _controller.DeltaTime;
				//sb.//("Stand");
				//������
				sb.PositionJudge();
				if (waitCast >= coolTime + 0.5f)
				{
					disEnable = false;
					_skipCondition = 0;
					waitCast = 0;
					//SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
					
				}
			}

		}

		/// <summary>
		/// �U�����A�j���C�x���g
		/// �r���������A�ˌ����[�V�����ɂĈ�x�Ă�
		/// ���@���x���Ƒ����ŉ����f
		/// </summary>
		public void EventFire()
		{

			fireStart = true;
			//�҂Ă���L���X�g�T�E���h���K�v����H
			//���ʂɔ��f���ĕ��ʂɓ����
		//	FireSoundJudge();
		}

		/// <summary>
		/// �r�����[�V�����̃A�j���C�x���g
		/// �r�����̂��߂ɉ��ƃG�t�F�N�g���Z�b�g
		/// �g�p���閂�@�������ɂ��Ė��@���x���Ƒ����Ō���
		/// �r���̉���G�t�F�N�g�ς������Ȃ炱���ł�����
		public async UniTaskVoid CastCircle()
		{

			Vector3 posi = new Vector3(transform.position.x, transform.position.y - 11, 40);
			//�����ʒu��Player
			castEffect = await Addressables.InstantiateAsync(SManager.instance.useMagic.castEffect, posi, Quaternion.Euler(-98,0,0));
			//	}

			float dir = Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
			sb.SisFlip(dir);


		}


		/// <summary>
		/// �O�����璆�f������
		/// </summary>
		public void MagicEnd()
		{
			
			_skipCondition = 0;
			//disEnable = false;
			stateJudge = 0;
			waitCast = 0;
			//�Ƃ肠�����J�����������
							
			if (castEffect != null)
			{

				Vector3 pos = new Vector3(transform.position.x, transform.position.y, -100);
				castEffect.transform.position = pos;
				Addressables.ReleaseInstance(castEffect);
				castEffect = null;
			}

			//Debug.Log("ffggrfe");
			actionNum = 0;
			_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
		}

		/// <summary>
		/// MP�����@�g�p�ł��Ȃ��قǌ������āA�Ȃ�����MP�x�����O�ł��邩�ǂ������m�F���郁�\�b�h
		/// </summary>
		public bool MPCheck()
        {

			return (SManager.instance.useMagic.useMP > sb.mp && bCount < 2);

        }



		/// <summary>
		/// for���ł͂Ȃ���	bcount�𒴂���܂�useMagic���^�Ȃ̂Ŕ�����������
		/// �e�ۂ���郁�\�b�h
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		void MagicUse(int hRandom, int vRandom)
		{
	//		Debug.Log($"�Ȃ܂�{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
			if (!fireStart || delayNow)
			{
				return;
			}


            

            //���@�g�p��MagicUse�ł��e�ې������łȂ����
            if (_movement.CurrentState == CharacterStates.MovementStates.Combination)
            {
				return;
            }



			bCount += 1;
			//�e�̔��˂Ƃ����������ʒu
			Vector3 goFire = sb.firePosition.position;
			//�e���ꔭ�ڂȂ�
			if (bCount == 1)
			{

				//�^�[�Q�b�g�������Ă������悤�ɏꏊ���o���Ă���
				if (SManager.instance.target != null) 
				{
					_tgPosition.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y);
				}
                else
                {
					//Debug.Log($"ddd{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");

					//disEnable = true;
					bCount = 0;
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

					actionNum = 0;
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					SManager.instance.useMagic = null;
					fireStart = false;

					return;
                }

				//�Ȃ񂩂����Ă������悤�Ɉꉞ�^�[�Q�b�g��ۑ�
				SManager.instance.restoreTarget = SManager.instance.target;

				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic._moveSt.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//�R�Ȃ�̒e���őł������Ƃ��Ƃ��ˏo�p�x���߂ꂽ�炢������
					//�ʒu�������_���ɂ���Ίp�x�͂ǂ��ł�������������
					SManager.instance.useAngle = GetAim(sb.firePosition.position,_tgPosition);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//�G�̈ʒu�ɃT�[�`�U������Ƃ�
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(_tgPosition.x, _tgPosition.y, 40);

			}
			//�����_���Ȉʒu�ɔ�������Ƃ�
			else if (hRandom != 0 || vRandom != 0)
			{
				//Transform goFire = firePosition;


				float xRandom = 0;
				float yRandom = 0;
				if (hRandom != 0)
				{

					xRandom = RandomValue(-hRandom, hRandom);

				}
				if (vRandom != 0)
				{
					yRandom = RandomValue(-vRandom, vRandom);
				}
				//	xRandom = RandomValue(RandomValue(-random,0),RandomValue(0, random));
				//	yRandom = RandomValue(RandomValue(-random, 0), RandomValue(0, random));

				goFire = new Vector3(sb.firePosition.position.x + xRandom, sb.firePosition.position.y + yRandom, 0);//�e������
				
			}
			
			//Debug.Log($"���@�̖��O5{SManager.instance.useMagic.hiraganaName}");
			//    MyInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.identity).Forget();//.Result;//�����ʒu��Player
			//�����ɔ�������e�ۂ̈ꔭ�ڂȂ�
			if (SManager.instance.useMagic.delayTime == 0 || bCount == 1)
			{
				Addressables.InstantiateAsync(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation));
			}
			//2���ڈȍ~�̒e�Ő���������Ȃ��Ȃ�i�`�b��ɔ����Ƃ������j
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}


			//�e�ۂ𐶐����I�������
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"ddd{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");

				//disEnable = true;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				//Debug.Log("dddw3d");
				actionNum = 0;
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				SManager.instance.useMagic = null;
				fireStart = false;

				//�e�ۑS���o���I������烍�b�N�J�[�\�������F�ɖ߂�
				if (SManager.instance.restoreTarget != null && SManager.instance.target != GManager.instance.Player) 
				{
					SManager.instance.restoreTarget.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				}

				sb.PositionJudge();
			}
			//	bCount += 1;
		}





		///<summary>
		///���f�ɗ��p
		/// </summary>
		#region

		///<summary>
		///�W�I�ƍs�����Đݒ肷��
		///���Z�b�g���͍U���̏ꍇ�^�[�Q�b�g������JudgeSequence��ς���
		///�����ƃV�[�P���X��������ւ���
		/// </summary>
		void TargetReset(bool _reset)
        {
			//�U���X�e�[�g��
			if (sister.nowMove == SisterParameter.MoveType.�U��)
			{

			//	Debug.Log($"s1{sister.targetCondition.Length}");

                if (_reset)
                {
					SManager.instance.target = null;
					judgeSequence = 0;
				}

				
				//�^�[�Q�b�g�����Ȃ��Ȃ�^�[�Q�b�g��T���܂��B
				if (SManager.instance.target == null)
				{

					//�ܔԖڂ܂ł��
					//�ܔԖڂ܂ł����璷������1�����Ă�
					for (int i = 0; i < sister.targetCondition.Length; i++)
					{
						//	int skiCheck =  (int)Mathf.Pow(2, i)
						//0��͂P
						//�N�[���^�C�����ŁA�Ȃ����X�L�b�v�R���f�B�V�����ɓ��Ă͂܂�Ȃ��Ȃ珈�����΂��B
						//�V�t�g���Z�H
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
						{
							continue;
						}

						SManager.instance.target = TargetSelect(sister.targetCondition[i], sister.AttackCondition[i]);

						if (SManager.instance.target != null)
						{
						//	Debug.Log($"{SManager.instance.target.name}");
							judgeSequence = i;
							break;
						}


					}
				//	Debug.Log($"s2");

					//����ł��^�[�Q�b�g�����Ȃ�������⑫�s��
					if (SManager.instance.target == null && !disEnable)
					{
						if (sister.AttackCondition[5].condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || sister.AttackCondition[5].condition == FireCondition.ActJudge.�x���s���Ɉڍs)
						{
							AttackStateChange(sister.AttackCondition[5]);
							return;
						}
						else if (sister.AttackCondition[5].condition != FireCondition.ActJudge.�Ȃɂ����Ȃ�)
						{
							SManager.instance.target = SManager.instance.targetList[RandomValue(0, SManager.instance.targetList.Count - 1)];
							judgeSequence = 5;
						}
					}

					//EnemyRecord��targetCondition�͈�v���Ă�B
					//�G���X�V
				}

				if (SManager.instance.target != null)
				{
					
					//�N�[���^�C�������ł��ĂȂ��ă��Z�b�g����Ȃ��Ȃ�_��
					if(!_reset && disEnable)
                    {
						return;
                    }

					AttackAct(sister.AttackCondition[judgeSequence]);

				}


			}

			//�x���̎��͑Ώۂ͌��܂��Ă�̂ŏ����ɓ��Ă͂܂�󋵂�
			//�����ē��Ă͂܂�x�������邩�𒲂ׂ�
			else if (sister.nowMove == SisterParameter.MoveType.�x��)
			{
				SManager.instance.target = GManager.instance.Player;

				//���Z�b�g�Ȃ画�f���Ȃ���
				if (_reset)
				{

					for (int i = 0; i < sister.supportPlan.Length; i++)
					{
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
						{
							continue;
						}

						//	�N�[���^�C�����ł͂Ȃ��Ō�̏����Ȃ疳�����ł����͒ʂ�
						if (i == sister.supportPlan.Length - 1 && !disEnable)
						{
							SupportAct(sister.supportPlan[i]);
							Debug.Log($"df{i}");
							judgeSequence = i;
						}
						else if (SupportJudge(sister.supportPlan[i]))
						{

							SupportAct(sister.supportPlan[i]);
							judgeSequence = i;
							Debug.Log($"ff{i}");
							break;
						}
					}
				}
                
				//���Z�b�g����Ȃ��Ȃ瓯��JudgeSequence�Ŕ��f
				else
                {
					if (SupportJudge(sister.supportPlan[judgeSequence]))
					{
						SupportAct(sister.supportPlan[judgeSequence]);

						Debug.Log($"ff");
					}
				}		

			}
			//�x���Ɠ���
			else if (sister.nowMove == SisterParameter.MoveType.��)
			{
				SManager.instance.target = GManager.instance.Player;

                if (_reset)
                {
					for (int i = 0; i < sister.recoverCondition.Length; i++)
					{
						if (disEnable && (_skipCondition & (int)Mathf.Pow(2, i)) != (int)Mathf.Pow(2, i))
						{
							continue;
						}

						//	�N�[���^�C�����ł͂Ȃ��Ō�̏����Ȃ疳�����ł����͒ʂ�
						if (i == sister.recoverCondition.Length - 1 && !disEnable)
						{
							RecoverAct(sister.recoverCondition[i]);
							judgeSequence = i;
						}
						else if (HealJudge(sister.recoverCondition[i]))
						{
							RecoverAct(sister.recoverCondition[i]);
							judgeSequence = i;

							break;
						}
					}
				}
				//���Z�b�g����Ȃ��Ȃ瓯��JudgeSequence�Ŕ��f
                else
				{
					if (HealJudge(sister.recoverCondition[judgeSequence]))
					{
						RecoverAct(sister.recoverCondition[judgeSequence]);
					}
				}
			}
		}

		/// <summary>
		/// �U���X�e�[�g�Ń^�[�Q�b�g������
		/// </summary>
		/// <param name="condition"></param>
		public GameObject TargetSelect(AttackJudge condition, FireCondition act)
		{
			//Debug.Log($"���f�ԍ�{judgeSequence}");
			//		bool testes = false;
			//	targetJudge = 0;
			//float value = 0.0f;//HP�Ȃǂ̔�r�̂��߂̐�����
			switch (condition.condition)
			{
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�w��Ȃ�:
					//	�����_���o�����[�g���ă��R�[�h����w��

					if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
					{

						AttackStateChange(act);
						return null;
					}
					else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
					{
						return null;
					}
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);

				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������:
					//	�����_���o�����[�g���ă��R�[�h����w��
					if (condition.highOrLow)
					{
						if (sb.pc.HPRatio() >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{
								Debug.Log($"zz");
								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						//		Debug.Log($"������{sb.pc.HPRatio()}��{condition.percentage / 100}");
						//Debug.Log($"������{condition.percentage / 100f}");
						if (sb.pc.HPRatio() <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{
								//kuroko++;
								//Debug.Log($"����{sister.nowMove}");

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�v���C���[��MP���K��l�ɒB������:
					//	�����_���o�����[�g���ă��R�[�h����w��
					if (condition.highOrLow)
					{
						if (GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						if (GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������://������
															  //	�����_���o�����[�g���ă��R�[�h����w��
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G://������
														 //	�����_���o�����[�g���ă��R�[�h����w��
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.������MP���K��l�ɒB������:
					if (condition.highOrLow)
					{
						if (sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{

						if (sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return null;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return null;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.���G�̑���:
					//���G��D��
					targetCanList = new List<GameObject>();
					targetCanStatus = new List<EnemyAIBase>();
					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
								{

									AttackStateChange(act);
									return null;
								}
								else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								{
									return null;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								//break;
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{

							if (!SManager.instance.targetCondition[i].status.strong)
							{
								if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
								{

									AttackStateChange(act);
									return null;
								}
								else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								{
									return null;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								//break;
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
							}
						}
					}
					return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					//�����ɓ񎟏����O��������CanList�������ɊJ�n

				
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�G�^�C�v:
					//   Soldier,//���̎G��

					targetCanList = new List<GameObject>();
					targetCanStatus = new List<EnemyAIBase>();

					//�I�ԓG�^�C�v�����ׂĂƑI������Ă�Ȃ�
					if (condition.percentage == 0b00011111)
					{

						if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
						{

							AttackStateChange(act);
							return null;
						}
						else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
						{
							return null;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);


						return SecondTargetJudge(targetCanList, condition, targetCanStatus);
						//break;
					}
					else
					{
						//int test;
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							//test = ;


							if ((condition.percentage & 0b00000001) == 0b00000001)
							{

								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							//test = 0b01000000;
							if ((condition.percentage & 0b00000010) == 0b00000010)
							{
							//	Debug.Log($"���������Ă�{SManager.instance.targetCondition == null}{SManager.instance.targetCondition.Count}��{i}");
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00000100) == 0b00000100)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return null;
									}
									//	siroko++;
									//		Debug.Log($"���̐���{siroko}");
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00001000) == 0b00001000)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return null;
									}
									//	siroko++;
									//		Debug.Log($"���̐���{siroko}");
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							if ((condition.percentage & 0b00010000) == 0b00010000)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return null;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return null;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
						}
						return SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}

					
					//-----------------------------------------------------------------------------------------------------
			}
			return null;
		}

		/// <summary>
		/// �U���X�e�[�g�Ŏg�p���@������
		/// </summary>
		/// <param name="condition"></param>
		public void AttackAct(FireCondition condition)
		{


			if (condition.UseMagic == null)
			{

				switch (condition.condition)
				{
					case FireCondition.ActJudge.�a������:

						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Slash)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�h�ˑ���:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Stab)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�Ō�����:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].phyBase > 0 && SManager.instance.attackMagi[i].atType == Magic.AttackType.Strike)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.������:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].holyBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�ő���:
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].darkBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.������:

						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].fireBase > 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						//Debug.Log($"asgd{magicCanList[0].name}");
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.������:
						//Debug.Log($"ssssss");
						magicCanList = new List<SisMagic>();
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase > 0)
							{
								//		Debug.Log($"���i�K{SManager.instance.attackMagi[i].name}");
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�ő���:
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�����w��Ȃ�:

						magicCanList = new List<SisMagic>(SManager.instance.attackMagi);

						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�ړ����x�ቺ�U��://������
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�U���͒ቺ�U��://������
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
					//-----------------------------------------------------------------------------------------------------
					case FireCondition.ActJudge.�h��͒ቺ�U��://������
						for (int i = 0; i < SManager.instance.attackMagi.Count; i++)
						{
							if (SManager.instance.attackMagi[i].thunderBase >= 0)
							{
								magicCanList.Add(SManager.instance.attackMagi[i]);
								break;
							}
						}
						secondATMagicJudge(magicCanList, condition);
						break;
				}
			}
			else
			{

				if (condition.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || condition.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
				{

					AttackStateChange(condition);
					return;
				}
				else if (condition.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
				{
					return;
				}
				SManager.instance.useMagic = condition.UseMagic;
			}
		}

		public bool SupportJudge(SupportCondition condition)
		{

			switch (condition.sCondition)
			{
				/*	case SupportCondition.SupportStatus.�������Ă��Ȃ��x��������:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�v���C���[�̗̑͂��K��l�̎�:

					if (condition.highOrLow)
					{
						return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�v���C���[��MP���K��l�ɒB������:

					if (condition.highOrLow)
					{
						return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.������MP���K��l�ɒB������:

					if (condition.highOrLow)
					{
						return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�v���C���[����Ԉُ�ɂ���������:

					return GManager.instance.badCondition ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.���G�����邩�ǂ���:
					//���G��D��

					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return true;
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return false;
							}
						}
						return true;
					}
					return false;
				//	break;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�G�^�C�v:
					//   Soldier,//���̎G��
					if ((0b00011111 & condition.percentage) == 0b00011111)
					{

						return true;
					}
					//int test;
					for (int i = 0; i < SManager.instance.targetList.Count; i++)
					{
						//test = ;


						if ((0b00000001 & condition.percentage) == 0b00000001)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
							{
								return true;
							}
						}
						//test = 0b01000000;
						else if ((0b00000010 & condition.percentage) == 0b00000010)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
							{
								return true;
							}
						}
						else if ((0b00000100 & condition.percentage) == 0b00000100)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
							{
								return true;
							}
						}
						else if ((0b00001000 & condition.percentage) == 0b00001000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
							{
								return true;
							}
						}
						else if ((0b00010000 & condition.percentage) == 0b00010000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
							{
								return true;
							}
						}
					}
					return false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�C�ӂ̎x�����؂�Ă���Ƃ�:

					for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
					{
						if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
						{
							magicCanList.Add(SManager.instance.supportMagi[i]);
						}
					}
					return magicCanList != null ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case SupportCondition.SupportStatus.�w��Ȃ�:
					//	�����_���o�����[�g���ă��R�[�h����w��
					return true;
					//-----------------------------------------------------------------------------------------------------
			}
			return false;
		}

		public void SupportAct(SupportCondition condition)
		{
			if (sister.nowMove != SisterParameter.MoveType.�x��)
			{
				return;


			}
			List<SisMagic> candidate;
			List<int> removeNumber = new List<int>();
			if (magicCanList != null)
			{
				candidate = magicCanList;
			}
			else
			{
				candidate = new List<SisMagic>(SManager.instance.supportMagi);
			}


			if (condition.UseMagic != null)
			{

				if (condition.ActBase == SupportCondition.MagicJudge.�Ȃɂ����Ȃ�)
				{
					magicCanList = null;

				}
				//-----------------------------------------------------------------------------------------------------
				else if (condition.ActBase == SupportCondition.MagicJudge.�񕜃X�e�[�g��)
				{
					magicCanList = null;
					SupportStateChange(condition.ActBase);
				}
				else if (condition.ActBase == SupportCondition.MagicJudge.�U���X�e�[�g��)
				{
					magicCanList = null;
					SupportStateChange(condition.ActBase);

				}
				else if (condition.ActBase == SupportCondition.MagicJudge.�e��x�����@)
				{

					for (int i = 0; i < candidate.Count; i++)
					{
						if (candidate[i].sType != condition.useSupport)
						{
							removeNumber.Add(i);
						}
					}
				}

				if (removeNumber.Count != 0)
				{
					for (int i = 0; i < removeNumber.Count; i++)
					{
						candidate.Remove(candidate[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
				if (candidate.Count == 0)
				{
					return;
				}
				else
				{
					if (condition.nextCondition == SupportCondition.AdditionalJudge.�w��Ȃ�)
					{
						SManager.instance.useMagic = candidate[0];
						condition.UseMagic = candidate[0];
						magicCanList = null;
					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == SupportCondition.AdditionalJudge.MP�g�p��)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP > candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP < candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.�r������)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime > candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime < candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.�������ʎ���)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime > candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime < candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == SupportCondition.AdditionalJudge.�����{��)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].mValue > candidate[selectNumber].mValue)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].mValue < candidate[selectNumber].mValue)
									{
										selectNumber = i;
									}
								}
							}
						}
						SManager.instance.useMagic = candidate[selectNumber];

					}
				}
			}
			else
			{
			
				SManager.instance.useMagic = condition.UseMagic;
			}
		}

		public bool HealJudge(RecoverCondition condition)
		{

			//Debug.Log($"�񕜔��f{condition.condition}�^�Ȃ��{condition.highOrLow}");

			switch (condition.condition)
			{

				/*	case RecoverCondition.RecoverStatus.�������Ă��Ȃ��x��������:
						//useSupport = null;
						for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
						{
							if (!SManager.instance.supportMagi[i].effectNow)
							{
								magicCanList.Add(SManager.instance.supportMagi[i]);
							}
						}

						return useSupport != null ? true : false;
					//break;*/

				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�v���C���[��HP���K��l�̎�:

					if (condition.highOrLow)
					{
						return sb.pc.HPRatio() >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.pc.HPRatio() <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�v���C���[��MP���K��l�ɒB������:

					if (condition.highOrLow)
					{
						return GManager.instance.mp / GManager.instance.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.������MP���K��l�ɒB������:

					if (condition.highOrLow)
					{
						return sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f ? true : false;
					}
				//return GManager.instance.hp == GManager.instance.maxHp ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�v���C���[����Ԉُ�ɂ���������:

					return GManager.instance.badCondition ? true : false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.���G�����邩�ǂ���:
					//���G��D��

					if (condition.highOrLow)
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return true;
							}
						}
					}
					else
					{
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							if (SManager.instance.targetCondition[i].status.strong)
							{
								return false;
							}
						}
						return true;
					}
					return false;
				//	break;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�G�^�C�v:
					//   Soldier,//���̎G��
					if ((0b00011111 & condition.percentage) == 0b00011111)
					{
						return true;
					}
					//int test;
					for (int i = 0; i < SManager.instance.targetList.Count; i++)
					{
						//test = ;


						if ((0b00000001 & condition.percentage) == 0b00000001)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
							{
								return true;
							}
						}
						//test = 0b01000000;
						else if ((0b00000010 & condition.percentage) == 0b00000010)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
							{
								return true;
							}
						}
						else if ((0b00000100 & condition.percentage) == 0b00000100)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Shooter)
							{
								return true;
							}
						}
						else if ((0b00001000 & condition.percentage) == 0b00001000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Knight)
							{
								return true;
							}
						}
						else if ((0b00010000 & condition.percentage) == 0b00010000)
						{
							if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Trap)
							{
								return true;
							}
						}
					}
					return false;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�C�ӂ̎x�����؂�Ă���Ƃ�:

					for (int i = 0; i < SManager.instance.supportMagi.Count; i++)
					{
						if (!SManager.instance.supportMagi[i].effectNow && SManager.instance.supportMagi[i].sType == condition.needSupport)
						{
							return false;
						}
					}
					return true;
				//-----------------------------------------------------------------------------------------------------
				case RecoverCondition.RecoverStatus.�w��Ȃ�:
					//	�����_���o�����[�g���ă��R�[�h����w��
					return true;
					//-----------------------------------------------------------------------------------------------------
			}
			return false;
		}

		public void RecoverAct(RecoverCondition condition)
		{


			if (sister.nowMove != SisterParameter.MoveType.�� && sb.nowState == BrainAbility.SisterState.�킢)
			{
				//	judgeSequence = 0;
				return;
			}
			if (condition.ActBase == RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�)
			{
				magicCanList = null;
				return;
			}
			//-----------------------------------------------------------------------------------------------------
			else if (condition.ActBase == RecoverCondition.MagicJudge.�x���X�e�[�g��)
			{
				magicCanList = null;
				RecoverStateChange(condition.ActBase);
				return;
			}
			else if (condition.ActBase == RecoverCondition.MagicJudge.�U���X�e�[�g��)
			{
				magicCanList = null;
				RecoverStateChange(condition.ActBase);
				return;
			}

			List<SisMagic> candidate = new List<SisMagic>(SManager.instance.recoverMagi);
			List<int> removeNumber = new List<int>();


			if (condition.UseMagic == null)
			{

				if (condition.useSupport != SisMagic.SupportType.�Ȃ�)
				{
					for (int i = 0; i < candidate.Count; i++)
					{
						if (candidate[i].sType != condition.useSupport)
						{
							removeNumber.Add(i);
							//Debug.Log($"���ׂ܂�{candidate[i].name}");
						}
					}
				}
				if (removeNumber.Count != 0)
				{
					for (int i = 0; i < removeNumber.Count; i++)
					{
						candidate.Remove(candidate[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
				if (candidate.Count == 0)
				{
					
					SManager.instance.useMagic = null;
					return;
				}
				else
				{
					if (condition.nextCondition == RecoverCondition.AdditionalJudge.�w��Ȃ�)
					{
						SManager.instance.useMagic = candidate[0];

						condition.UseMagic = candidate[0];
						magicCanList = null;

					}
					else
					{
						int selectNumber = 150;
						if (condition.nextCondition == RecoverCondition.AdditionalJudge.MP�g�p��)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP > candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].useMP < candidate[selectNumber].useMP)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�r������)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime > candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].castTime < candidate[selectNumber].castTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l�񕜗�)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount > candidate[selectNumber].regeneAmount)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount < candidate[selectNumber].regeneAmount)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.���W�F�l���񕜗�)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount * candidate[i].effectTime > candidate[selectNumber].regeneAmount * candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].regeneAmount * candidate[i].effectTime < candidate[selectNumber].regeneAmount * candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�������ʎ���)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime > candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].effectTime < candidate[selectNumber].effectTime)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.�񕜗�)
						{
							if (condition.upDown)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].recoverBase > candidate[selectNumber].recoverBase)
									{
										selectNumber = i;
									}

								}
							}
							else
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (selectNumber == 150 || candidate[i].recoverBase < candidate[selectNumber].recoverBase)
									{
										selectNumber = i;
									}
								}
							}
						}
						else if (condition.nextCondition == RecoverCondition.AdditionalJudge.��Ԉُ��)
						{
							for (int i = 0; i < candidate.Count; i++)
							{
								if (selectNumber == 150 || candidate[i].cureCondition)
								{
									selectNumber = i;
									break;
								}

							}

						}
						SManager.instance.useMagic = candidate[selectNumber];
						condition.UseMagic = candidate[selectNumber];
					}
				}
			}
			else
			{
				SManager.instance.useMagic = condition.UseMagic;
			}

			SManager.instance.target = GManager.instance.Player;

		}

		/// <summary>
		/// ��_�Ԃ̊p�x�����߂�
		/// </summary>
		/// <param name="p1">�����̍��W</param>
		/// <param name="p2">����̍��W</param>
		/// <returns></returns>
		float GetAim(Vector2 p1, Vector2 p2)
		{
			float dx = p2.x - p1.x;
			float dy = p2.y - p1.y;
			float rad = Mathf.Atan2(dy, dx);
			return rad * Mathf.Rad2Deg;
		}


		/// <summary>
		///�@���X�g�����A���̒����獇�v��������̓G��I�яo���B
		/// </summary>
		/// <param name="targetList"></param>
		/// <param name="condition"></param>
		/// <param name="statusList"></param>
		GameObject SecondTargetJudge(List<GameObject> targetList, AttackJudge condition, List<EnemyAIBase> statusList)
		{

			//	Debug.Log($"sddf{targetCanList[0].name}");
			//	Debug.Log($"sdgs{targetList[0].name}");


			if (targetList.Count == 0 || targetList == null)
			{
				targetCanList = null;
				targetCanStatus = null;
				return null;
			}
			else if (targetList.Count >= 1)
			{
				List<int> removeNumber = new List<int>();

				if (condition.wp != AttackJudge.WeakPoint.�w��Ȃ�)
				{
					///<Summary>
					///�������f
					/// </Summary>
					#region
					if (condition.wp != AttackJudge.WeakPoint.�a������)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Slash))
							{
								removeNumber.Add(i);
							}
						}

					}
					else if (condition.wp != AttackJudge.WeakPoint.�h�ˑ���)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Stab))
							{
								removeNumber.Add(i);
							}
						}

					}
					else if (condition.wp != AttackJudge.WeakPoint.�Ō�����)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.�Ō�����)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Strike))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.������)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Fire))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.������)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Thunder))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.������)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Holy))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.�ő���)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Dark))
							{
								removeNumber.Add(i);
							}
						}
					}
					else if (condition.wp != AttackJudge.WeakPoint.�ő���)
					{
						for (int i = 0; i < targetList.Count; i++)
						{
							if (!statusList[i].status.wp.Contains(EnemyStatus.WeakPoint.Poison))
							{
								removeNumber.Add(i);
							}
						}
					}
					for (int i = 0; i < removeNumber.Count; i++)
					{
						//[removeNumber[i] - i]�͂���܂Ŕz��̒�����������������������Ďw�肵�Ă�
						//i=2�Ȃ班�Ȃ��Ƃ���̗v�f�������Ă�̂ł��̕��C���f�b�N�X���ς��
						targetList.Remove(targetList[removeNumber[i] - i]);
						statusList.Remove(statusList[removeNumber[i] - i]);
					}
					removeNumber = null;
					#endregion

				}

			}

			if (targetList.Count == 0 || targetList == null)
			{
				
				return null;

			}

			///<summary>
			///�ǉ��������f
			/// </summary>
			else
			{
				int selectNumber = 150;
				if (condition.nextCondition != AttackJudge.AdditionalJudge.�w��Ȃ�)
				{
					if (condition.nextCondition != AttackJudge.AdditionalJudge.�G��HP)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i]._health.CurrentHealth > statusList[selectNumber]._health.CurrentHealth)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i]._health.CurrentHealth < statusList[selectNumber]._health.CurrentHealth)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̍U����)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.atkDisplay > statusList[selectNumber].status.atkDisplay)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.atkDisplay < statusList[selectNumber].status.atkDisplay)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̖h���)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.defDisplay > statusList[selectNumber].status.defDisplay)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || statusList[i].status.defDisplay < statusList[selectNumber].status.defDisplay)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̍��x)//�^�Ȃ獂��
					{
						if (condition.upDown)
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || targetList[i].transform.position.y > targetList[selectNumber].transform.position.y)
								{
									selectNumber = i;
								}
							}
						}
						else
						{
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || targetList[i].transform.position.y < targetList[selectNumber].transform.position.y)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition != AttackJudge.AdditionalJudge.�G�̋���)
					{
						float distance = 0;
						if (condition.upDown)
						{
							//�߂�
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x) < distance)
								{
									selectNumber = i;
									distance = Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
						else
						{
							//����
							for (int i = 0; i < targetList.Count; i++)
							{
								if (selectNumber == 150 || Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x) > distance)
								{
									selectNumber = i;
									distance = Mathf.Abs(targetList[i].transform.position.x - this.gameObject.transform.position.x);
								}
							}
						}
					}
					targetCanList = null;
					targetCanStatus = null;

					if (SManager.instance.target != null)
					{
						targetList[selectNumber].MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
					}
					return targetList[selectNumber];
				}
				else
				{
					if (SManager.instance.target != null)
					{
						targetList[0].MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
					}
					targetCanList = null;
					targetCanStatus = null;
					return targetList[0];
				}

			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="magicList"></param>
		void secondATMagicJudge(List<SisMagic> magicList, FireCondition condition)
		{
			//	Debug.Log($"�������{magicList[0].name}");
			if (magicList.Count == 0)
			{
				return;
			}
			else
			{
				//	Debug.Log("�m�F");
				//������
				if (condition.firstCondition != FireCondition.FirstCondition.�w��Ȃ�)
				{
					List<int> removeNumber = new List<int>();
					if (condition.firstCondition == FireCondition.FirstCondition.�G�𐁂���΂�)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isBlow && !magicList[i].cBlow)
							{
								//Debug.Log("�폜");
								removeNumber.Add(i);
							}
						}
					}
					//-----------------------------------------------------------------------------------------------------
					else if (condition.firstCondition == FireCondition.FirstCondition.�͈͍U��)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isExprode)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�ђʂ���)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].penetration)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�ǔ�����)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i]._moveSt.fireType == Magic.FIREBULLET.ANGLE || magicList[i]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�ݒu�U��)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i]._moveSt.speedV != 0)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�ǔ�����)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i]._moveSt.fireType == Magic.FIREBULLET.ANGLE || magicList[i]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�͈͍U��)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (magicList[i]._moveSt.fireType == Magic.FIREBULLET.RAIN)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					else if (condition.firstCondition == FireCondition.FirstCondition.�T�[�`�U��)
					{
						for (int i = 0; i < magicList.Count; i++)
						{
							if (!magicList[i].isChaice)
							{
								removeNumber.Add(i);
							}
						}
						//break;
					}
					for (int i = 0; i < removeNumber.Count; i++)
					{
						magicList.Remove(magicList[removeNumber[i] - i]);
						//targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
				}
			}

			if (magicList.Count == 0)
			{
				Debug.Log("�A��");
				return;
			}
			else
			{
				//	Debug.Log("��O�i�K");
				if (condition.nextCondition == FireCondition.AdditionalCondition.�w��Ȃ�)
				{

					SManager.instance.useMagic = magicList[0];
					condition.UseMagic = magicList[0];
					magicCanList = null;
				}
				else
				{
					int selectNumber = 150;
					if (condition.nextCondition == FireCondition.AdditionalCondition.MP�g�p��)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].useMP > magicList[selectNumber].useMP)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].useMP < magicList[selectNumber].useMP)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.�U����)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].displayAtk > magicList[selectNumber].displayAtk)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].displayAtk < magicList[selectNumber].displayAtk)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.���ː�)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].bulletNumber > magicList[selectNumber].bulletNumber)
								{
									selectNumber = i;
								}

							}

						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].bulletNumber < magicList[selectNumber].bulletNumber)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.���l)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].shock > magicList[selectNumber].shock)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].shock < magicList[selectNumber].shock)
								{
									selectNumber = i;
								}
							}
						}
					}
					else if (condition.nextCondition == FireCondition.AdditionalCondition.�r������)
					{
						if (condition.upDown)
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].castTime > magicList[selectNumber].castTime)
								{
									selectNumber = i;
								}

							}
						}
						else
						{
							for (int i = 0; i < magicList.Count; i++)
							{
								if (selectNumber == 150 || magicList[i].castTime < magicList[selectNumber].castTime)
								{
									selectNumber = i;
								}
							}
						}
					}

					SManager.instance.useMagic = magicList[selectNumber];
					condition.UseMagic = magicList[selectNumber];
					magicCanList = null;
				}
			}
		}
		/// <summary>
		/// �U���X�e�[�g����̃X�e�[�g�ύX
		/// </summary>
		void AttackStateChange(FireCondition condition)
		{
			if (condition.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs)
			{
				sister.nowMove = SisterParameter.MoveType.��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.�x��;
			}
			stateJudge = 0.0f;
			targetJudge = 1000;
			judgeSequence = 0;
			SManager.instance.useMagic = null;
		}
		/// <summary>
		/// �x���X�e�[�g����̃X�e�[�g�ύX
		/// </summary>
		void SupportStateChange(SupportCondition.MagicJudge condition)
		{
			if (condition == SupportCondition.MagicJudge.�U���X�e�[�g��)
			{
				//	targetJudge = sister.targetResetRes;

			//	SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.�U��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.��;
			}
			stateJudge = 0.0f;
			targetJudge = 1000;
			judgeSequence = 0;
			SManager.instance.useMagic = null;
		}
		/// <summary>
		/// �񕜃X�e�[�g����̃X�e�[�g�ύX
		/// </summary>
		void RecoverStateChange(RecoverCondition.MagicJudge condition)
		{
			if (condition == RecoverCondition.MagicJudge.�U���X�e�[�g��)
			{
				//	targetJudge = sister.targetResetRes;
				if (SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>() != null)
				{
					SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				}
				//SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.�U��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.�x��;
			}
			SManager.instance.useMagic = null;
			targetJudge = 1000;
			judgeSequence = 0;
			stateJudge = 0.0f;
		}



		void FireSoundJudge()
		{
			GManager.instance.PlaySound("NormalCastEnd", transform.position);
		}

		#endregion


		void castCheck()
		{

			if (_movement.CurrentState == CharacterStates.MovementStates.Cast && SManager.instance.target == null)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				waitCast = 0;
				//�Ƃ肠�����J�����������

				if (castEffect != null)
				{

					Vector3 pos = new Vector3(transform.position.x, transform.position.y, -100);
					castEffect.transform.position = pos;
					Addressables.ReleaseInstance(castEffect);
					castEffect = null;
				}

			//	Debug.Log("dsdffg");
				actionNum = 0;

				waitCast = 0;
			}

		}




		async UniTaskVoid DelayInstantiate(object key, Vector3 position, Quaternion rotation, Transform parent = null, bool trackHandle = true)
		{
			delayNow = true;
			await UniTask.Delay(TimeSpan.FromSeconds(SManager.instance.useMagic.delayTime));
			await Addressables.InstantiateAsync(key, position, rotation, parent);
			delayNow = false;
		}

		/// <summary>
		/// �o�t�̐��l��^����
		/// �e�ۂ���Ă�
		/// </summary>
		public void BuffCalc(FireBullet _fire)
		{
			_fire.attackFactor = attackFactor;
			_fire.fireATFactor = fireATFactor;
			_fire.thunderATFactor = thunderATFactor;
			_fire.darkATFactor = darkATFactor;
			_fire.holyATFactor = holyATFactor;
		}

		///<summary>
		///  Brain�Ƃ̘A�g���s��
		/// </summary>
        #region
		
		///<sumary>
		/// ��Ԃ��Ƃ̏���������
		/// </sumary>
		public void StateInitialize(bool battle)
        {
            if (battle)
            {
				judgeSequence = 0;
				targetJudge = 1000;
				stateJudge = 1000;

            }
            else
            {
				//����͓����񕜊J�n�Ƃ��̃T�E���h���I��点�Ă�

				if (soundStart)
				{
					soundStart = false;
					GManager.instance.StopSound(castSound, 0.5f);
					if (castEffect != null)
					{
						Vector3 pos = new Vector3(transform.position.x, transform.position.y, -100);
						castEffect.transform.position = pos;
						Addressables.ReleaseInstance(castEffect);
						castEffect = null;
					}
					FireSoundJudge();
				}

				stateJudge = 0;
				coolTime = 0;
				disEnable = false;
				_skipCondition = 0;
				if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				}

				isReset = false;
			}
		}

        #endregion

        /// <summary>
        ///  �K�v�ȃA�j���[�^�[�p�����[�^�[������΁A�A�j���[�^�[�p�����[�^�[���X�g�ɒǉ����܂��B
        /// </summary>
        protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_actParameterName, AnimatorControllerParameterType.Int, out _actAnimationParameter);
			RegisterAnimatorParameter(_motionParameterName, AnimatorControllerParameterType.Int, out _motionAnimationParameter);
		}

		/// <summary>
		/// ������I�[�o�[���C�h����ƁA�L�����N�^�[�̃A�j���[�^�[�Ƀp�����[�^�𑗐M���邱�Ƃ��ł��܂��B
		/// ����́ACharacter�N���X�ɂ���āAEarly�Anormal�ALate process()�̌�ɁA1�T�C�N�����Ƃ�1��Ăяo�����B
		/// </summary>
		public override void UpdateAnimator()
		{
			//�N���E�`���O�ɋC�������
			//MasicUse��Castnow��g�ݍ��킹�悤��
			int state = 0;
			if (_movement.CurrentState == CharacterStates.MovementStates.Attack)
			{
				state = 2;
			}
			else if (_movement.CurrentState == CharacterStates.MovementStates.Cast)
			{
				state = 1;
			}

			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _actAnimationParameter, (state), _character._animatorParameters);
			MMAnimatorExtensions.UpdateAnimatorInteger(_animator, _motionAnimationParameter, (actionNum), _character._animatorParameters);
		}




	}
}