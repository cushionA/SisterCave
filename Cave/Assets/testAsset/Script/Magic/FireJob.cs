using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct FireJob : IJobParallelForTransform
{

	/*
	    ANGLE,//�p�x����angle�Ŏw��ł���
		HOMING,//���S�ǔ�
		HOMING_Z,//�w�肵���p�x���ɕW�I������Ƃ��ǔ��B
		RAIN,//�ŏ��ɔ��˒n�_����W�I�܂ł̊p�x���v�Z���Ă��̊p�x�őS�e�������낷
		STOP,//�����Ȃ�
	 
	 */
	//movable���ے�Ȃ�Ă΂Ȃ��H

	public bool homing;
	public Vector3 posTarget;
	public bool targetLost;
	public bool movable;


	Vector3 memory;
   //homingAngle���Ēǂ�������p�x�����
	Quaternion homingAngle;

	//------------------------------
	//�ς��Ȃ����͍̂\���̂ɂ܂Ƃ߂�

	public Magic.BMoveStatus _status;


	//---------------------------�ς��X�e�[�^�X

	float speed;
	float homingRange;


	//-------------------------------------	�o��

	//�O�����x�A�P����]
	//public NativeArray<Vector3> result;

	public Vector3 _velocity;


	void IJobParallelForTransform.Execute(int index,TransformAccess _transform)
	{

		Debug.Log($"�ǂ�{_velocity}");
		if (homing)
		{
			posTarget += Vector3.up;
		}
		Debug.Log($"hhhhhhhhhhhh");
		MoveCalcu(_transform);
		BulletStateControll(_transform);
		
	}

	public void Initialize(Vector3 enemy,Vector3 myPosi)
    {
		switch (_status.fireType)
		{


			case Magic.FIREBULLET.ANGLE:
				speed = _status.speedV;
				_status.angle = SManager.instance.useAngle;
				break;
			case Magic.FIREBULLET.HOMING:
				speed = _status.speedV;
				homingAngle = Quaternion.LookRotation(enemy - myPosi);
				//	transform.localScale = 
				break;
			case Magic.FIREBULLET.HOMING_Z:
				speed = _status.speedV;
				homingAngle = Quaternion.LookRotation(enemy - myPosi);
				homingRange = _status.homingAngleV;

				break;
			case Magic.FIREBULLET.RAIN:

				speed = _status.speedV;
				
				break;
		}
	//	Debug.Log($"�ǂ�{_velocity}");

	}

	/// <summary>
	/// �e�ۂ̎��Ԍo�߂��Ǘ�����
	/// </summary>
	void BulletStateControll(TransformAccess _transform)
    {

		speed += _status.speedA * Time.fixedDeltaTime;

		// �X�P�[���v�Z
		//�ǂ�ǂ�傫���Ȃ����菬�����Ȃ�����
		//�������Ȃ肷������j�󂷂�
		if (_status.bulletScaleV != Vector2.zero)
		{
			Vector2 changeScale = new Vector2();
			changeScale.Set(Mathf.Abs(_transform.localScale.x), Mathf.Abs(_transform.localScale.y));
			changeScale.x = (_status.bulletScaleV.x + changeScale.x) > 0 ? (_status.bulletScaleV.x + changeScale.x) : changeScale.x;
			changeScale.y = (_status.bulletScaleV.y + changeScale.y) > 0 ? (_status.bulletScaleV.y + changeScale.y) : changeScale.y;


			changeScale.x = _transform.localScale.x >= 0 ? changeScale.x : -changeScale.x;
			_transform.localScale = changeScale;
		}
		if (_status.bulletScaleA != Vector2.zero)
		{
			_status.bulletScaleV += _status.bulletScaleA * Time.fixedDeltaTime;
        }
		if (_status.isRotate)
		{

			memory.Set(0.0f, 0.0f, Time.fixedDeltaTime * _status.rotateVt);
			// �X�v���C�g�摜�̉�]����
			_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles +  memory);
		}
	}

	/// <summary>
	/// �����̌v�Z
	/// </summary>
	void MoveCalcu(TransformAccess _transform)
    {
		Debug.Log($"sdfssff");
		// �z�[�~���O����
		//�����Ń^�[�Q�b�g�Ȃ��ꍇ�͒��i��
		if (targetLost)
		{

			// �p�x����P�ʃx�N�g��
			_velocity = new Vector2(Mathf.Cos((_transform.rotation.z +180) * Mathf.Deg2Rad), Mathf.Sin((_transform.rotation.z + 180) * Mathf.Deg2Rad));

			//�i�s�p�x�ɂƂ�
			_velocity *= speed;
		}
		else
		{
			//Magic.FIREBULLET.ANGLE:
			if (_status.fireType == Magic.FIREBULLET.ANGLE || _status.fireType == Magic.FIREBULLET.RAIN)
			{

                if (!movable)
                {
					return;
                }

				//�i�s�p�x�ɏ]���đ��x��ω�������

				memory.Set(speed, 0.0f, 0.0f);

				_velocity = Quaternion.Euler(0.0f, 0.0f, _status.angle) * memory;

				if (Math.Sign(posTarget.x - _transform.position.x) > 0)
				{
					memory.Set(0, 0, 180 - _transform.rotation.z);
					_velocity += memory;
				}
			}
			else if (_status.fireType == Magic.FIREBULLET.HOMING)
			{


				//WaitTime��HormingTime�𓯂�������ȉ��ɂ���Β�~�������G��_����ɂȂ�
				if (homing)
				{
					homingAngle = Quaternion.LookRotation(posTarget - _transform.position);
					_transform.rotation = homingAngle;

				}

				if (!movable)
				{
					return;
				}

				// �Ώە��։�]����

				_velocity = (homingAngle * Vector3.forward) * speed;

				_velocity = Quaternion.Euler(0.0f, 0.0f, _status.angle) * _velocity;

			}

			//z�z�[�~���O���Č��ǒǔ�����p�x�ɐ�����������Ă��Ƃ����
			//���S�ǔ��Ȃ�P�W�O�x��]�ł���Ƃ�����P�Q�O�x�܂łƂ����Đ������Ă�
			/*	else if (_status.moveType == 2)
				{
					if (homing)
					{
						//�G�ƒe�ۂ̊p�x�����߂ēx�ɕϊ�
						float targetAngle = Mathf.Atan2(posTarget.y - _transform.position.y,
															posTarget.x - _transform.position.x) * Mathf.Rad2Deg;

						float deltaAngle = Mathf.DeltaAngle(targetAngle, homingAngle);
						float deltaHomingAngle = homingRange * Time.fixedDeltaTime;
						if (Mathf.Abs(deltaAngle) >= deltaHomingAngle)
						{
							homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
						}
						homingRange += (_status.homingAngleA * Time.fixedDeltaTime);
						homingAngle = Quaternion.Euler(0.0f, 0.0f, homingAngle);
					}


					//�������Ȃ��ꍇ�͊p�x�����ύX

					_velocity	=  (homingAngle * Vector3.right) * speed;

				}*/
		}

		if (!movable)
		{
			return;
		}
		//�e�ۂ����E���x�ɉ����ĐU���������
		if (_velocity.x > 0f && _transform.localScale.x < 0)
		{
			Vector3 theScale = _transform.localScale;
			theScale.x = Mathf.Abs(theScale.x);
			_transform.localScale = theScale;
		}
		else if (_velocity.x < 0f && _transform.localScale.x > 0)
		{
			Vector3 theScale = _transform.localScale;
			theScale.x = -1 * Mathf.Abs(theScale.x);
			_transform.localScale = theScale;

		}


	}

}
