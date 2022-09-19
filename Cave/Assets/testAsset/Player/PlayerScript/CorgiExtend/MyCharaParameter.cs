using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	/// <summary>
	/// The various states you can use to check if your character is doing something at the current frame
	/// by Renaud Forestié
	/// </summary>

	public class MyCharaParameter : CharacterStates
	{
		/// The possible character conditions
		public new enum CharacterConditions
		{
			Normal,
			ControlledMovement,
			Frozen,
			Paused,
			Dead,
			Stunned,
			Moving
		}

		/// The possible Movement States the character can be in. These usually correspond to their own class, 
		/// but it's not mandatory
		public new enum MovementStates
		{
			Null,
			Idle,
			Walking,
			Falling,
			Running,
			Crouching,
			Crawling,
			Dashing,
			LookingUp,
			WallClinging,
			Jetpacking,
			Diving,
			Gripping,
			Dangling,
			Jumping,
			Pushing,
			DoubleJumping,
			WallJumping,
			LadderClimbing,
			SwimmingIdle,
			Gliding,
			Flying,
			FollowingPath,
			LedgeHanging,
			LedgeClimbing,
			Rolling,
			Attack,
			Guard,
			GuardMove,
			Warp,
			moving,
			FastFlying,
			Parry,
			Nostate//つなぎに使う
		}
	}
}
