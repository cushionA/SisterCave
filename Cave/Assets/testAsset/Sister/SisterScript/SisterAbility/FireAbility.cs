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

		//   [Header("����f�[�^")]
		/// declare your parameters here
		///WeaponHandle�Q�l�ɂ��� 


		// Animation parameters
		//0���Ȃ���Ȃ��A�P�����@�A�Q���R���r�l�[�V�����A�R���r��
		protected const string _actParameterName = "actType";
		protected int _actAnimationParameter;

		/// <summary>
		/// ���[�V������I��
		/// </summary>
		protected const string _motionParameterName = "motionNum";
		protected int _motionAnimationParameter;

		public SisterParameter sister;

		//public GameObject firePosition;
		//	SisterFireBullet sisF;
		[HideInInspector]
		public BrainAbility sb;
		/// <summary>
		/// �N�[���^�C���ҋ@�̂��ߐ�p�̃t���O
		/// </summary>
		bool disEnable;
		//List<GameObject> targetPlan;
		//GameObject target;
		List<SisMagic> useSupport;//���g�p�̎x��
		List<float> effectiveTime;//�x�����@�A���W�F�l�A�U���̎��Ԃ��͂���
		/// <summary>
		/// �r�����ԑ҂�
		/// </summary>
		float waitCast;//
		float coolTime;
		/// <summary>
		/// �U���A�񕜁A�Ȃǂ̗D��s�������ւ���
		/// </summary>
		float stateJudge = 30;
		//float targetJudge = 30;

		/// <summary>
		/// ���ڂ̏����Ń^�[�Q�b�g�����������m�F����
		/// </summary>
		int judgeSequence;

		/// <summary>
		/// �r����������
		/// </summary>
		string castSound;

		/// <summary>
		/// �r���̉��炷�T�C��
		/// 1�͂܂��Đ����ĂȂ��B2�͍Đ����B3�͏I���
		/// </summary>
		byte soundStart = 0;
		/// <summary>
		/// ���e�ۂ̐�
		/// </summary>
		int bCount;
		float recoverTime;

		List<GameObject> targetCanList;
		List<EnemyAIBase> targetCanStatus;
		List<SisMagic> magicCanList;
		/// <summary>
		///�@�^�[�Q�b�g�̎�ނƎg�p���@�����ݍ����Ă��邩���邽�߂̃p�����[�^�[
		/// </summary>
		byte targetType = 0;

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

		[HideInInspector]
		public bool[] skipCondition = new bool[5];


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
		//	Debug.Log($"������{_movement.CurrentState}");
			if (isReset)
			{
				//����͓����񕜊J�n�Ƃ��̃T�E���h���I��点�Ă�

				if (soundStart == 2)
				{
					soundStart = 0;
					GManager.instance.StopSound(castSound, 0.5f);
					FireSoundJudge();
				}
				stateJudge = 0;
				coolTime = 0;
				disEnable = false;
				if (_condition.CurrentState == CharacterStates.CharacterConditions.Moving)
				{
					_movement.ChangeState(CharacterStates.MovementStates.Idle);
					_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				}

				isReset = false;
			}

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
		void FireAct()
		{
			//��~���ߏo�Ă邩�ǂ���

			//Debug.Log($"����������{MathF.Floor(waitCast)}");
			//�s�����ĂȂ��Ƃ����͉�



			if (_movement.CurrentState == CharacterStates.MovementStates.Combination )
            {
				return;
            }
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
			else if (sb.nowState == BrainAbility.SisterState.�킢 && sb.nowPosition)
			{
				//�퓬�J�n���Ƀt���O�����ꂢ�ɂ��Ă����܂�

				//�퓬�J�n���̏������I������Ȃ�

				//�������@�Ȃ��Ȃ�߂�
				if (sb.status.equipMagic == null)
				{
					return;
				}
				//Disenable�Ȃ�N�[���^�C����҂�

				CoolTimeWait();

				//�^�[�Q�b�g����������r����߂�
				CastStop().Forget();


				//	targetJudge += _controller.DeltaTime;
				stateJudge += _controller.DeltaTime;

				///�^�[�Q�b�g�Ǝg�p���@�ݒ�
				#region
				if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving && !disEnable)
				{

					//��莞�Ԍo�߂Ő퓬�v�l���A�Ȃ��łȂ�����͗D�悷���Ԃɖ߂�
					if (stateJudge >= sister.stateResetRes && sister.priority != SisterParameter.MoveType.�Ȃ�)
					{
						//�X�e�[�g���Z�b�g
						sister.nowMove = sister.priority;
						stateJudge = 0.0f;
					}



					//�U���X�e�[�g��
					if (sister.nowMove == SisterParameter.MoveType.�U��)
					{

						//�^�[�Q�b�g�����Ȃ��Ȃ�^�[�Q�b�g��T���܂��B
						if (SManager.instance.target == null)
						{


							judgeSequence = 1;
							TargetSelect(sister.firstTarget, sister.firstAttack);

							//�ȉ��^�[�Q�b�g��������܂ŒT��
							if (SManager.instance.target == null)
							{

								TargetSelect(sister.secondTarget, sister.secondAttack);
								judgeSequence = 2;
							}
							if (SManager.instance.target == null)
							{
								TargetSelect(sister.thirdTarget, sister.thirdAttack);
								judgeSequence = 3;
							}
							if (SManager.instance.target == null)
							{

								TargetSelect(sister.forthTarget, sister.fourthAttack);
								judgeSequence = 4;
							}
							if (SManager.instance.target == null)
							{
								//		if (sister.fiveAttack.condition != FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								//	{
								TargetSelect(sister.fiveTarget, sister.fiveAttack);
								judgeSequence = 5;
								//	}
							}
							if (SManager.instance.target == null)
							{
								if (sister.nonAttack.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || sister.nonAttack.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
								{
									AttackStateChange(sister.nonAttack);
									return;
								}
								else if (sister.nonAttack.condition != FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								{
									SManager.instance.target = SManager.instance.targetList[RandomValue(0, SManager.instance.targetList.Count - 1)];
									judgeSequence = 6;
								}
							}

							//EnemyRecord��targetCondition�͈�v���Ă�B
							//�G���X�V
						}
						if (SManager.instance.target != null)
						{
							//Debug.Log("�g�p���@�ݒ肪�ł��ĂȂ��̂����");

							if (sister.nowMove != SisterParameter.MoveType.�U��)
							{
								judgeSequence = 0;
								return;


							}
							if (judgeSequence == 1)
							{

								AttackAct(sister.firstAttack);
								//Debug.Log("���i�K");
							}
							else if (judgeSequence == 2)
							{
								AttackAct(sister.secondAttack);
								//	Debug.Log($"������΁[");
							}
							else if (judgeSequence == 3)
							{
								AttackAct(sister.thirdAttack);

							}
							else if (judgeSequence == 4)
							{
								AttackAct(sister.fourthAttack);
							}
							else if (judgeSequence == 5)
							{
								AttackAct(sister.nonAttack);
							}
							else if (judgeSequence == 6)
							{
								AttackAct(sister.nonAttack);
							}

						}


					}

					//�x���̎��͑Ώۂ͌��܂��Ă�̂ŏ����ɓ��Ă͂܂�󋵂�
					//�����ē��Ă͂܂�x�������邩�𒲂ׂ�
					else if (sister.nowMove == SisterParameter.MoveType.�x��)
					{
						SManager.instance.target = GManager.instance.Player;
						//���傤����݂����Ă�
						if (SupportJudge(sister.firstPlan))
						{
							SupportAct(sister.firstPlan);
						}
						else if (SupportJudge(sister.secondPlan))
						{
							SupportAct(sister.secondPlan);
						}
						else if (SupportJudge(sister.thirdPlan))
						{
							SupportAct(sister.thirdPlan);
						}
						else if (SupportJudge(sister.forthPlan))
						{
							SupportAct(sister.forthPlan);
						}
						else if (SupportJudge(sister.fivePlan))
						{
							SupportAct(sister.fivePlan);
						}
						else
						{
							SupportAct(sister.sixPlan);
						}
					}
					//�x���Ɠ���
					else if (sister.nowMove == SisterParameter.MoveType.��)
					{
						//SManager.instance.target = GManager.instance.Player;
						if (HealJudge(sister.firstRecover))
						{

							RecoverAct(sister.firstRecover);
						}
						else if (HealJudge(sister.secondRecover))
						{
							RecoverAct(sister.secondRecover);
						}
						else if (HealJudge(sister.thirdRecover))
						{
							RecoverAct(sister.thirdRecover);
						}
						else if (HealJudge(sister.forthRecover))
						{
							RecoverAct(sister.forthRecover);
						}
						else if (HealJudge(sister.fiveRecover))
						{
							RecoverAct(sister.fiveRecover);
						}
						else
						{
							RecoverAct(sister.nonRecover);
						}
					}

				}
				#endregion

				//�����ƃ^�[�Q�b�g�Ǝg�p���@���ݒ肳��Ă��邩�̃`�F�b�N
				//���ꂪ�I���Δ���

				if (SManager.instance.target != null && SManager.instance.useMagic != null)
				{
					//	bool isWrong = false;
					//�g�p���@���U���ŁA���^�[�Q�b�g���G�ł���B
					if (SManager.instance.useMagic.mType == SisMagic.MagicType.Attack)
					{
						if (sister.nowMove != SisterParameter.MoveType.�U�� && targetType != 1)
						{

							return;
						}

					}
					else if (SManager.instance.useMagic.mType == SisMagic.MagicType.Recover)
					{
						if (sister.nowMove != SisterParameter.MoveType.�� && targetType != 2)
						{
							return;
							//isWrong = true;
						}

					}
					else
					{
						if (sister.nowMove != SisterParameter.MoveType.�x�� && targetType != 2)
						{
							return;
						}
					}

					if (_condition.CurrentState == CharacterStates.CharacterConditions.Normal)
					{
					//	Debug.Log("ssd");
						actionNum = (int)SManager.instance.useMagic.castType;
						_movement.ChangeState(CharacterStates.MovementStates.Cast);
						_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
					}

					
					//�^�[�Q�b�g�����Ďg�p���閂�@�������Ďg�p���@�ƃ^�[�Q�b�g�����ݍ����Ă���Ȃ�

					ActionFire();

				}


			}

			//���ݐ킢������Ȃ��Ȃ�
			else if (sb.nowState != BrainAbility.SisterState.�킢)
			{
				//�ꉞ���Z�b�g�Ɛ퓬�J�n���Ƀ��Z�b�g���Ă��炦��悤�d����


				if (sister.autoHeal)
				{
					//bool healAct = false;


					if (_condition.CurrentState != CharacterStates.CharacterConditions.Moving)
					{
						healJudge += _controller.DeltaTime;
						if (healJudge >= 3f)
						{
							//	healAct = true;
							//SManager.instance.target = GManager.instance.Player;
							//SManager.instance.useMagic = null;
							//SManager.instance.target = GManager.instance.Player;
							if (HealJudge(sister.nFirstRecover))
							{

								RecoverAct(sister.nFirstRecover);
								//Debug.Log($"1{SManager.instance.useMagic}");
							}
							else if (HealJudge(sister.nSecondRecover))
							{
								RecoverAct(sister.nSecondRecover);
								//Debug.Log("2");
							}
							else if (HealJudge(sister.nThirdRecover))
							{
								RecoverAct(sister.nThirdRecover);
								//Debug.Log("3");
							}
							else
							{
								SManager.instance.useMagic = null;
								//Debug.Log("4");
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
							actionNum = (int)SManager.instance.useMagic.castType;
							_movement.ChangeState(CharacterStates.MovementStates.Cast);
							_condition.ChangeState(CharacterStates.CharacterConditions.Moving);
						}

						//Debug.Log("������");
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
			//�����_���ɓ���Ă��������Ǖ��ʂɓ���Ă�����

			//�g�p���閂�@��MP�������āA���W�I������Ȃ�
			if (sb.mp >= SManager.instance.useMagic.useMP && SManager.instance.target != null)
			{
				//�����J�n
			//	Debug.Log($"�A�C�C�C{SManager.instance.useMagic}d{SManager.instance.useMagic.castTime}");
				waitCast += _controller.DeltaTime;
				//�r���I�������
				if (waitCast >= SManager.instance.useMagic.castTime)
				{
					
					//	waitCast = 0;
					disEnable = true;
					//GManager.instance.StopSound(castSound, 0.5f);
					_movement.ChangeState(CharacterStates.MovementStates.Attack);

					actionNum = (int)SManager.instance.useMagic.fireType;

					//��������̏����ł̓A�j���[�V�����C�x���g���g��
				}
				//�r�����Ȃ�
				else
				{

					//sb.//(SManager.instance.useMagic.castAnime);
					if (soundStart == 1)
					{
						GManager.instance.PlaySound(castSound, transform.position);
						soundStart = 2;
					}
				}
			}
			//Mp����Ȃ����^�[�Q�b�g�������Ȃ�
			else
			{
				//	�r�����~�B�G������ŏ�������Ƃ�
				GManager.instance.StopSound(castSound, 0.5f);
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				actionNum = 0;
				waitCast = 0;
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
				waitCast += _controller.DeltaTime;
				//sb.//("Stand");
				if (waitCast >= coolTime + 0.5f)
				{
					disEnable = false;
					waitCast = 0;
					//SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
					SManager.instance.target = null;
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
		//	Debug.Log("��tyaaaa");
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
		public void CastEffect()
		{

			Transform gofire = sb.firePosition;
			//�����ʒu��Player
			Addressables.InstantiateAsync(SManager.instance.useMagic.castEffect, gofire.position, gofire.rotation);
			//	}

			castSound = "normalCast";
			soundStart = 1;
			int dir = (int)Mathf.Sign(SManager.instance.target.transform.position.x - transform.position.x);
			sb.Flip(dir);
		}



		public void MagicEnd()
		{
			disEnable = false;
			_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
			_movement.ChangeState(CharacterStates.MovementStates.Idle);
			stateJudge = 0;
			waitCast = 0;
			coolTime = 0;
			//	SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
			SManager.instance.target = null;
			actionNum = 0;
			waitCast = 0;

		}


		/// <summary>
		/// for���ł͂Ȃ���	bcount�𒴂���܂�useMagic���^�Ȃ̂Ŕ�����������
		/// �e�ۂ���郁�\�b�h
		/// </summary>
		/// <param name="hRandom"></param>
		/// <param name="vRandom"></param>
		void MagicUse(int hRandom, int vRandom)
		{

			if (!fireStart || delayNow)
			{
				return;
			}
		//	Debug.Log("���Ă�");
			//	Debug.Log($"�n�U�[�h{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");
			//���@�g�p��MagicUse�ł��e�ې������łȂ����

			bCount += 1;
			//�e�̔��˂Ƃ����������ʒu
			Vector3 goFire = sb.firePosition.position;
			//�e���ꔭ�ڂȂ�
			if (bCount == 1)
			{
				//   MyInstantiate(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity).Forget();
				//Addressables.InstantiateAsync(SManager.instance.useMagic.fireEffect, goFire, Quaternion.identity);
				if (SManager.instance.useMagic.fireType == SisMagic.FIREBULLET.RAIN)
				{
					//�R�Ȃ�̒e���őł������Ƃ��Ƃ��ˏo�p�x���߂ꂽ�炢������
					//�ʒu�������_���ɂ���Ίp�x�͂ǂ��ł�������������
					SManager.instance.useMagic.angle = GetAim(sb.firePosition.position, SManager.instance.target.transform.position);

				}
				sb.mp -= SManager.instance.useMagic.useMP;
			}

			//�G�̈ʒu�ɃT�[�`�U������Ƃ�
			if (SManager.instance.useMagic.isChaice)
			{
				goFire.Set(SManager.instance.target.transform.position.x, SManager.instance.target.transform.position.y, SManager.instance.target.transform.position.y);

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
			//2���ڈȍ~�̒e�Ő���������Ȃ��Ȃ�
			else if (bCount > 1 && !delayNow)
			{
				DelayInstantiate(SManager.instance.useMagic.effects, goFire, Quaternion.Euler(SManager.instance.useMagic.startRotation)).Forget();
			}
			//�e�ۂ𐶐����I�������
			if (bCount >= SManager.instance.useMagic.bulletNumber)
			{
				//Debug.Log($"�e���y�X�g{SManager.instance.useMagic.name}�W�I{SManager.instance.target}����{sister.nowMove}");

				//disEnable = true;
				coolTime = SManager.instance.useMagic.coolTime;
				bCount = 0;
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);

				actionNum = 0;
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				SManager.instance.useMagic = null;
				fireStart = false;
				SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
			}
			//	bCount += 1;
		}




		///���f�ɗ��p
		#region
		/// <summary>
		/// �U���X�e�[�g�Ń^�[�Q�b�g������
		/// </summary>
		/// <param name="condition"></param>
		public void TargetSelect(AttackJudge condition, FireCondition act)
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
						return;
					}
					else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
					{
						return;
					}
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					SecondTargetJudge(targetCanList, condition, targetCanStatus);

					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�v���C���[��HP���K��l�ɒB������:
					//	�����_���o�����[�g���ă��R�[�h����w��
					if (condition.highOrLow)
					{
						if (GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						//		Debug.Log($"������{GManager.instance.hp / GManager.instance.maxHp}��{condition.percentage / 100}");
						//Debug.Log($"������{condition.percentage / 100f}");
						if (GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{
								//kuroko++;
								//Debug.Log($"����{sister.nowMove}");

								AttackStateChange(act);
								return;
								//		testes = true;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
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
								return;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{
						if (GManager.instance.mp / GManager.instance.maxMp <= condition.percentage / 100f)
						{

							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}

							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.�v���C���[����Ԉُ�ɂ���������://������
															  //	�����_���o�����[�g���ă��R�[�h����w��
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					SecondTargetJudge(targetCanList, condition, targetCanStatus);
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.��Ԉُ�ɂ������Ă�G://������
														 //	�����_���o�����[�g���ă��R�[�h����w��
					targetCanList = new List<GameObject>(SManager.instance.targetList);
					targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
					SecondTargetJudge(targetCanList, condition, targetCanStatus);
					break;
				//-----------------------------------------------------------------------------------------------------
				case AttackJudge.TargetJudge.������MP���K��l�ɒB������:
					if (condition.highOrLow)
					{
						if (sb.mp / SManager.instance.sisStatus.maxMp >= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
						}
					}
					else
					{

						if (sb.mp / SManager.instance.sisStatus.maxMp <= condition.percentage / 100f)
						{
							if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
							{

								AttackStateChange(act);
								return;
							}
							else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
							{
								return;
							}
							targetCanList = new List<GameObject>(SManager.instance.targetList);
							targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);
							SecondTargetJudge(targetCanList, condition, targetCanStatus);
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								{
									return;
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
									return;
								}
								else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
								{
									return;
								}
								targetCanList.Add(SManager.instance.targetList[i]);
								//break;
								targetCanStatus.Add(SManager.instance.targetCondition[i]);
							}
						}
					}
					SecondTargetJudge(targetCanList, condition, targetCanStatus);
					//�����ɓ񎟏����O��������CanList�������ɊJ�n

					break;
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
							return;
						}
						else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
						{
							return;
						}
						targetCanList = new List<GameObject>(SManager.instance.targetList);
						targetCanStatus = new List<EnemyAIBase>(SManager.instance.targetCondition);


						SecondTargetJudge(targetCanList, condition, targetCanStatus);
						//break;
					}
					else
					{
						//int test;
						for (int i = 0; i < SManager.instance.targetList.Count; i++)
						{
							//test = ;
							//Debug.Log("�卪");

							if ((condition.percentage & 0b00000001) == 0b00000001)
							{

								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Soldier)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
							//test = 0b01000000;
							if ((condition.percentage & 0b00000010) == 0b00000010)
							{
								if (SManager.instance.targetCondition[i].status.kind == EnemyStatus.KindofEnemy.Fly)
								{
									if (act.condition == FireCondition.ActJudge.�񕜍s���Ɉڍs || act.condition == FireCondition.ActJudge.�x���s���Ɉڍs)
									{

										AttackStateChange(act);
										return;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return;
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
										return;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return;
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
										return;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return;
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
										return;
									}
									else if (act.condition == FireCondition.ActJudge.�Ȃɂ����Ȃ�)
									{
										return;
									}
									targetCanList.Add(SManager.instance.targetList[i]);
									targetCanStatus.Add(SManager.instance.targetCondition[i]);
									continue;
								}
							}
						}
						SecondTargetJudge(targetCanList, condition, targetCanStatus);
					}

					break;
					//-----------------------------------------------------------------------------------------------------
			}

		}

		/// <summary>
		/// �U���X�e�[�g�Ŏg�p���@������
		/// </summary>
		/// <param name="condition"></param>
		public void AttackAct(FireCondition condition)
		{
			//	Debug.Log($"�d�v��ԊJ��{sister.nowMove }");
			//Debug.Log($"asgd{condition.condition}");
			if (condition.UseMagic == null)
			{
				//Debug.Log("ghdks");
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
						Debug.Log($"rrrr");
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
				//	Debug.Log($"�������ȂƂ�{condition.UseMagic.name}");
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
						return GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f ? true : false;
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
						return GManager.instance.hp / GManager.instance.maxHp >= condition.percentage / 100f ? true : false;
					}
					else
					{
						return GManager.instance.hp / GManager.instance.maxHp <= condition.percentage / 100f ? true : false;
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

				/*		if (condition.ActBase == RecoverCondition.MagicJudge.�Ȃɂ����Ȃ�)
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
						else if (condition.ActBase == RecoverCondition.MagicJudge.�������@)
						{
							if (condition.useSupport != SisMagic.SupportType.�Ȃ�)
							{
								for (int i = 0; i < candidate.Count; i++)
								{
									if (candidate[i].sType != condition.useSupport)
									{
										removeNumber.Add(i);
										Debug.Log($"�ē�{candidate[i].name}");
									}
								}
							}
						}*/
				if (condition.useSupport != SisMagic.SupportType.�Ȃ�)
				{
					for (int i = 0; i < candidate.Count; i++)
					{
						if (candidate[i].sType != condition.useSupport)
						{
							removeNumber.Add(i);
							Debug.Log($"�ē�{candidate[i].name}");
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
					//Debug.Log($"�ē�{SManager.instance.useMagic.name}");
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
						Debug.Log($"�N�̖���{SManager.instance.useMagic.name}");
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
			targetType = 2;
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
		void SecondTargetJudge(List<GameObject> targetList, AttackJudge condition, List<EnemyAIBase> statusList)
		{

			//	Debug.Log($"sddf{targetCanList[0].name}");
			//	Debug.Log($"sdgs{targetList[0].name}");


			if (targetList.Count == 0 || targetList == null)
			{
				return;
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
						targetCanList.Remove(targetList[removeNumber[i] - i]);
						targetCanStatus.Remove(statusList[removeNumber[i] - i]);
					}
					removeNumber = null;
					#endregion

				}

			}

			if (targetList.Count == 0)
			{
				targetCanList = null;
				targetCanStatus = null;
				return;

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

					SManager.instance.target = targetList[selectNumber];
					SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
				}
				else
				{

					SManager.instance.target = targetList[0];
					SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(2);
				}
				targetCanList = null;
				targetCanStatus = null;
			}
			targetType = 1;
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
							if (magicList[i].fireType == Magic.FIREBULLET.ANGLE || magicList[i].fireType == Magic.FIREBULLET.RAIN)
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
							if (magicList[i].speedV != 0)
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
							if (magicList[i].fireType == Magic.FIREBULLET.ANGLE || magicList[i].fireType == Magic.FIREBULLET.RAIN)
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
							if (magicList[i].fireType == Magic.FIREBULLET.RAIN)
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
				//Debug.Log("����");
				sister.nowMove = SisterParameter.MoveType.��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.�x��;
			}
			stateJudge = 0.0f;
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

				SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.�U��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.��;
			}
			stateJudge = 0.0f;
			//	return;
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
				SManager.instance.target.MMGetComponentNoAlloc<EnemyAIBase>().TargetEffectCon(3);
				SManager.instance.target = null;
				sister.nowMove = SisterParameter.MoveType.�U��;
			}
			else
			{
				sister.nowMove = SisterParameter.MoveType.�x��;
			}
			SManager.instance.useMagic = null;
			stateJudge = 0.0f;
		}



		void FireSoundJudge()
		{
			GManager.instance.PlaySound("NormalCastEnd", transform.position);
		}

		#endregion

		async UniTaskVoid CastStop()
		{
			await UniTask.RunOnThreadPool(() => castCheck());
		}
		void castCheck()
		{

			if (_movement.CurrentState == CharacterStates.MovementStates.Cast && SManager.instance.target == null)
			{
				_movement.ChangeState(CharacterStates.MovementStates.Idle);
				_condition.ChangeState(CharacterStates.CharacterConditions.Normal);
				waitCast = 0;
				actionNum = (int)SManager.instance.useMagic.castType;
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