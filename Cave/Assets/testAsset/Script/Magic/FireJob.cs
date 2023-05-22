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
	public float time;



	//-------------------------------------	�o��

	//�O�����x�A�P����]
	public NativeArray<Vector3> result;


	void IJobParallelForTransform.Execute(int index,TransformAccess _transform)
	{

		//direction ��new��job�ł̉e��
		if (homing)
		{
			posTarget += Vector3.up;
		}

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
	//	Debug.Log($"�ǂ�{result[0]}");

	}

	/// <summary>
	/// �e�ۂ̎��Ԍo�߂��Ǘ�����
	/// </summary>
	void BulletStateControll(TransformAccess _transform)
    {

		speed += _status.speedA * time;

		// �X�P�[���v�Z
		//�ǂ�ǂ�傫���Ȃ����菬�����Ȃ�����
		//�������Ȃ肷������j�󂷂�
		if (_status.bulletScaleV != Vector2.zero)
		{
			
			memory.Set(Mathf.Abs(_transform.localScale.x), Mathf.Abs(_transform.localScale.y),_transform.localScale.z);
			memory.x = (_status.bulletScaleV.x + memory.x) > 0 ? (_status.bulletScaleV.x + memory.x) : memory.x;
			memory.y = (_status.bulletScaleV.y + memory.y) > 0 ? (_status.bulletScaleV.y + memory.y) : memory.y;


			memory.x = _transform.localScale.x >= 0 ? memory.x : -memory.x;
			_transform.localScale = memory;
		}
		if (_status.bulletScaleA != Vector2.zero)
		{
			_status.bulletScaleV += _status.bulletScaleA * time;
        }
		if (_status.isRotate)
		{

			memory.Set(0.0f, 0.0f, time * _status.rotateVt);
			// �X�v���C�g�摜�̉�]����
			_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles +  memory);
		}
	}

	/// <summary>
	/// �����̌v�Z
	/// </summary>
	void MoveCalcu(TransformAccess _transform)
    {
		
		// �z�[�~���O����
		//�����Ń^�[�Q�b�g�Ȃ��ꍇ�͒��i��
		if (targetLost)
		{

			// �p�x����P�ʃx�N�g��
			//result[0] = new Vector2(Mathf.Cos((_transform.rotation.z +180) * Mathf.Deg2Rad), Mathf.Sin((_transform.rotation.z + 180) * Mathf.Deg2Rad));

			//�i�s�p�x�ɂƂ�
			result[0] *= speed;
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

				result[0] = Quaternion.Euler(0.0f, 0.0f, _status.angle) * memory;
				if (Math.Sign(posTarget.x - _transform.position.x) > 0)
				{
					memory.Set(0, 0, 180 - _transform.rotation.z);
					result[0] += memory;
				}
			}
			else if (_status.fireType == Magic.FIREBULLET.HOMING)
			{
				float s;
	

				//WaitTime��HormingTime�𓯂�������ȉ��ɂ���Β�~�������G��_����ɂȂ�
				if (homing)
				{
					memory.Set(0, 0,Mathf.Atan2( posTarget.y - _transform.position.y, posTarget.x - _transform.position.x) * Mathf.Rad2Deg);
					homingAngle = Quaternion.Euler(memory) * Quaternion.Euler(0.0f, 0.0f, _status.angle);

				//	
				}
				s = homingAngle.eulerAngles.z;
				if (_transform.rotation != homingAngle)
				{
					_transform.rotation = homingAngle;
				}
				if (!movable)
				{
					result[0] = Vector3.zero;
					return;
				}
                if (!homing)
                {
					//�����͈�v���Ă�
					//�������̂��r���ŕς���Ă�H
				//	Debug.Log($"����{homingAngle.eulerAngles.z}ededded{_transform.rotation.eulerAngles.z}");

                }
				
				
				// �Ώە��։�]����
				result[0] = (homingAngle * Vector3.forward) * speed;

				result[0] = Quaternion.Euler(0.0f, 0.0f, _status.angle) * result[0];
				//�x�N�g���͓r���ŕς���ĂȂ�
				//�ł��i�ފp�x�ƌ������Ⴄ
				//�p�x�����ǂ����ŕς���Ă�H
				//�t���[���x���̂����őO�̃t���[����result�󂯎���Ă�͂��肻��
				//Debug.Log($"����{homingAngle.eulerAngles.z == s}");
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
						float deltaHomingAngle = homingRange * time;
						if (Mathf.Abs(deltaAngle) >= deltaHomingAngle)
						{
							homingAngle += (deltaAngle < 0.0f) ? +deltaHomingAngle : -deltaHomingAngle;
						}
						homingRange += (_status.homingAngleA * time);
						homingAngle = Quaternion.Euler(0.0f, 0.0f, homingAngle);
					}


					//�������Ȃ��ꍇ�͊p�x�����ύX

					result[0]	=  (homingAngle * Vector3.right) * speed;

				}*/

		}

		if (!movable)
		{
			return;
		}
		//�e�ۂ����E���x�ɉ����ĐU���������
		if (result[0].x > 0f && _transform.localScale.x < 0)
		{
			Vector3 theScale = _transform.localScale;
			theScale.x = Mathf.Abs(theScale.x);
			_transform.localScale = theScale;
		}
		else if (result[0].x < 0f && _transform.localScale.x > 0)
		{
			Vector3 theScale = _transform.localScale;
			theScale.x = -1 * Mathf.Abs(theScale.x);
			_transform.localScale = theScale;

		}


	}

}
