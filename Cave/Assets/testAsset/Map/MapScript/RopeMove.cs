using UnityEngine;
using System.Collections;

public class RopeMove : MonoBehaviour
{

	//　進んでいる方向
	private int direction = 1;
	//　Z軸の角度
	private float angle = -180f;

	

	HingeJoint2D hinge;
	JointMotor2D motorS;
	//　補間間隔
	[SerializeField]
	private float motorPower = 100f;
	//　Z軸で振り子をする角度。右がマイナス。
	[SerializeField]
	private float limitAngle = 45f;


	void Start()
	{
		hinge = this.GetComponent<HingeJoint2D>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{

		//　スムーズに角度を計算
		angle = this.transform.localEulerAngles.z;
		//　角度を変更。最初、下向いてるときは180。左回りに増えてく。

		//プロパティが返す構造体はいったん入れ物に入れてから代入しないとダメ
		motorS = hinge.motor;
		motorS.motorSpeed = motorPower * direction;
		hinge.motor = motorS;

		//　角度が指定した角度と1度の差になったら反転
		//BendLimitsを変えれば揺れ方ぐにゃぐにゃから変えられる。5くらいがベスト

		if (direction > 0)
		{
			if (angle <= 180 - limitAngle)
			{
				direction = -1;
				
			}
		}

		if(direction < 0)
        {
			if (angle >= 180 + limitAngle)
			{
				direction = 1;
				

			}

		}
		//Debug.log($"{angle}");
	}
	//　進んでいる向きを返す（実際にはint値）
	public int GetDirection()
	{
		return direction;
	}
}