using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired.Integration.CorgiEngine;
namespace MoreMountains.CorgiEngine
{
	[SelectionBase]
	/// <summary>
	/// ���̃N���X�́A�L�����N�^�[��CorgiController�R���|�[�l���g�𑀏c���܂��B
	/// ���͂����ǂ��邽�߂̃��m�ł����AFlip�Ƃ����ς��Ă���������
	/// �W�����v�A�_�b�V���A�V���[�g�ȂǁA�L�����N�^�[�̂��ׂẴQ�[�����[������������ꏊ�ł��B
	/// �A�j���[�^�[�̃p�����[�^�[�� Grounded (bool), xSpeed (float), ySpeed (float), 
	/// CollidingLeft (bool), CollidingRight (bool), CollidingBelow (bool), CollidingAbove (bool), Idle (bool).
	/// �Ⴆ�΁A��ԑJ�ڂɕ��U��ǉ�����̂ɕ֗��ł��B
	/// RandomConstant : 0����1000�̊Ԃ̃����_����int�^�B�X�^�[�g���ɐ�������A�A�j���[�^�[�̎������s����܂ň��ɕۂ���܂��B
	/// �����^�C�v�̃L�����N�^�[���قȂ鋓���ɂ���̂ɕ֗��ł��B
	/// </summary>
	[AddComponentMenu("Corgi Engine/Character/Core/MyCharacter")]
	public class MyCharacter : Character
	{

		/// associated input manager
		public new RewiredCorgiEngineInputManager  LinkedInputManager { get; protected set; }

		protected  MyAbillityBase[] _myAbilities;


				/// <summary>
		/// �A�r���e�B���擾���A����Ɏg�p���邽�߂ɃL���b�V������B
		/// ���s���ɔ\�͂�ǉ�����ꍇ�́A�K��������Ăяo���܂��B
		/// ���z�I�ɂ́A���s���ɃR���|�[�l���g��ǉ�����͔̂��������Ƃ���ł��B
		/// ���̑���ɃR���|�[�l���g��L����/����������̂��őP�ł���B
		/// �������A�����K�v�ł���΁A���̃��\�b�h���Ăяo���B
		/// </summary>
		public override void CacheAbilities()
        {
			//�@
			// �������́A�������̃��x���ł��ׂĂ̔\�͂�����
			//CharacterAbility���p��������̂��ׂĂ�Get
		//	_characterAbilities = this.gameObject.GetComponents<CharacterAbility>();
			_myAbilities = this.gameObject.GetComponents<MyAbillityBase>();
			// ���[�U������Ƀm�[�h���w�肵���ꍇ
			if ((AdditionalAbilityNodes != null) && (AdditionalAbilityNodes.Count > 0))
            {
				// ���X�g���쐬���܂��B
				//List<CharacterAbility> tempAbilityList = new List<CharacterAbility>();
				List<MyAbillityBase> abillityList = new List<MyAbillityBase>();
				// �@�ł��łɔ������Ă���\�͂����ׂă��X�g�ɂ��܂����B
				for (int i = 0; i < _myAbilities.Length; i++)
                {
					//tempAbilityList.Add(_characterAbilities[i]);
				//	tempAbilityList.Add((CharacterAbility)_myAbilities[i]);
					abillityList.Add(_myAbilities[i]);
					//_myAbilities[i] = (MyAbillityBase)_characterAbilities[i];
				}

				// �m�[�h�̂��̂�ǉ�����B
				for (int j = 0; j < AdditionalAbilityNodes.Count; j++)
                {
					// CharacterAbility[] tempArray = AdditionalAbilityNodes[j].GetComponentsInChildren<CharacterAbility>();
					MyAbillityBase[] tempArray = AdditionalAbilityNodes[j].GetComponentsInChildren<MyAbillityBase>();
					_myAbilities = abillityList.ToArray();
					foreach (MyAbillityBase ability in tempArray)
                    {
						//	Debug.Log($"�ԍ�{j}�A���O{ability.GetComponent<CharacterAbility>()}");
						//Debug.Log($"sssss{ability.GetType()}");

                       // tempAbilityList.Add(ability);
						//MyAbillityBase _alter = (MyAbillityBase)ability;
						abillityList.Add(ability);
					}
                }

               _myAbilities = abillityList.ToArray();
				_characterAbilities = Array.ConvertAll<MyAbillityBase, CharacterAbility>(_myAbilities, ReturnMyAbility);
				
				
			}
            _abilitiesCachedOnce = true;
        }

		private CharacterAbility ReturnMyAbility(MyAbillityBase ability)
        {
			return (CharacterAbility) ability;
        }

		/// <summary>
		/// �L�����N�^�[�̃v���C���[ID�ɑΉ�����InputManager���擾���܂��i���݂���ꍇ�j�B
		/// </summary>
		public override void SetInputManager()
		{
			if (CharacterType == CharacterTypes.AI)
			{
				LinkedInputManager = null;
				UpdateInputManagersInAbilities();
				return;
			}

			// we get the corresponding input manager
			if (!string.IsNullOrEmpty(PlayerID))
			{
				LinkedInputManager = null;
				InputManager[] foundInputManagers = FindObjectsOfType(typeof(RewiredCorgiEngineInputManager)) as RewiredCorgiEngineInputManager[];
				foreach (RewiredCorgiEngineInputManager foundInputManager in foundInputManagers)
				{
					if (foundInputManager.PlayerID == PlayerID)
					{
						LinkedInputManager = foundInputManager;
					}
				}
			}
			UpdateInputManagersInAbilities();
		}

		/// <summary>
		/// Sets a new input manager for this Character and all its abilities
		/// </summary>
		/// <param name="inputManager"></param>
		public override void SetInputManager(InputManager inputManager)
		{
			LinkedInputManager = (RewiredCorgiEngineInputManager)inputManager;
			UpdateInputManagersInAbilities();
		}

		/// <summary>
		/// Updates the linked input manager for all abilities
		/// </summary>
		protected override void UpdateInputManagersInAbilities()
		{
			if (_characterAbilities == null)
			{
				return;
			}
			for (int i = 0; i < _myAbilities.Length; i++)
			{
				_myAbilities[i].SetInputManager(LinkedInputManager);
			}
		}

		/// <summary>
		/// Resets the input for all abilities
		/// </summary>
		public override void ResetInput()
		{
			if (_characterAbilities == null)
			{
				return;
			}
			foreach (CharacterAbility ability in _myAbilities)
			{
				ability.ResetInput();
			}
		}

		/// <summary>
		/// Sets the player ID
		/// </summary>
		/// <param name="newPlayerID">New player ID.</param>
		public override void SetPlayerID(string newPlayerID)
		{
			PlayerID = newPlayerID;
			SetInputManager();
		}


		/// <summary>
		/// Calls all registered abilities' Early Process methods
		/// �C���v�b�g������
		/// </summary>
		protected override void EarlyProcessAbilities()
		{
			//Debug.Log($"ddd�A�r���e�B�̂Ȃ���{_myAbilities.Length}�R�[�M�[�A�r���e�B�̒���{_characterAbilities.Length}");

			foreach (MyAbillityBase ability in _myAbilities)
			{


		//		Debug.Log("asdf");
				if (ability.enabled && ability.AbilityInitialized)
				{
				//	Debug.Log("af");
					ability.EarlyProcessAbility();
				}
			}
		}

		/// <summary>
		/// Calls all registered abilities' Process methods
		/// </summary>
		protected override void ProcessAbilities()
		{
			//	Debug.Log($"sssss{ability.GetType()}");

			foreach (MyAbillityBase ability in _myAbilities)
			{
				if (ability.enabled && ability.AbilityInitialized)
				{
					ability.ProcessAbility();
				}
			}
		}

		/// <summary>
		/// Calls all registered abilities' Late Process methods
		/// </summary>
		protected override void LateProcessAbilities()
		{
			foreach (MyAbillityBase ability in _myAbilities)
			{
				if (ability.enabled && ability.AbilityInitialized)
				{
					ability.LateProcessAbility();
				}
			}
		}



	}
}