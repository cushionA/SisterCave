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
	    ANGLE,//角度だけangleで指定できる
		HOMING,//完全追尾
		HOMING_Z,//指定した角度内に標的があるとき追尾。
		RAIN,//最初に発射地点から標的までの角度を計算してその角度で全弾撃ちおろす
		STOP,//動かない
	 
	 */
	//movableが否定なら呼ばない？

	public bool homing;
	public Vector3 posTarget;
	public bool targetLost;
	public bool movable;


	Vector3 memory;
   //homingAngleって追いかける角度だよな
	Quaternion homingAngle;

	//------------------------------
	//変わらないものは構造体にまとめる

	public Magic.BMoveStatus _status;


	//---------------------------変わるステータス

	float speed;
	float homingRange;
	public float time;



	//-------------------------------------	出力

	//０が速度、１が回転
	public NativeArray<Vector3> result;


	void IJobParallelForTransform.Execute(int index,TransformAccess _transform)
	{

		//direction とnewをjobでの影響
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
	//	Debug.Log($"どぇ{result[0]}");

	}

	/// <summary>
	/// 弾丸の時間経過を管理する
	/// </summary>
	void BulletStateControll(TransformAccess _transform)
    {

		speed += _status.speedA * time;

		// スケール計算
		//どんどん大きくなったり小さくなったり
		//小さくなりすぎたら破壊する
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
			// スプライト画像の回転処理
			_transform.rotation = Quaternion.Euler(_transform.rotation.eulerAngles +  memory);
		}
	}

	/// <summary>
	/// 動きの計算
	/// </summary>
	void MoveCalcu(TransformAccess _transform)
    {
		
		// ホーミング処理
		//ここでターゲットない場合は直進に
		if (targetLost)
		{

			// 角度から単位ベクトル
			//result[0] = new Vector2(Mathf.Cos((_transform.rotation.z +180) * Mathf.Deg2Rad), Mathf.Sin((_transform.rotation.z + 180) * Mathf.Deg2Rad));

			//進行角度にとぶ
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

				//進行角度に従って速度を変化させる

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
	

				//WaitTimeとHormingTimeを同じかそれ以下にすれば停止中だけ敵を狙うやつになる
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
					//方向は一致してる
					//方向自体が途中で変わってる？
				//	Debug.Log($"ｓｆ{homingAngle.eulerAngles.z}ededded{_transform.rotation.eulerAngles.z}");

                }
				
				
				// 対象物へ回転する
				result[0] = (homingAngle * Vector3.forward) * speed;

				result[0] = Quaternion.Euler(0.0f, 0.0f, _status.angle) * result[0];
				//ベクトルは途中で変わってない
				//でも進む角度と向きが違う
				//角度だけどっかで変わってる？
				//フレーム遅延のせいで前のフレームのresult受け取ってるはありそう
				//Debug.Log($"ｓｆ{homingAngle.eulerAngles.z == s}");
			}

			//zホーミングって結局追尾する角度に制限かけるってことだよね
			//完全追尾なら１８０度回転できるところを１２０度までとかって制限つけてる
			/*	else if (_status.moveType == 2)
				{
					if (homing)
					{
						//敵と弾丸の角度を求めて度に変換
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


					//うごけない場合は角度だけ変更

					result[0]	=  (homingAngle * Vector3.right) * speed;

				}*/

		}

		if (!movable)
		{
			return;
		}
		//弾丸を左右速度に応じて振り向かせる
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
