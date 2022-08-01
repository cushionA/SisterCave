using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeJump : MonoBehaviour
{

	public float climbUpInterval = 0.05f;       //interval between climbing up
	public float climbDownInterval = 0.04f;     //interval between climbing down
	public float swingForce = 10.0f;            //force which will be added to connected chain when left/right buttons will be pressed
	public float delayBeforeSecondHang = 0.4f;  //delay after jump before player will be able to hang on another rope


	private static Transform collidedChain;     //saves transform on which player is connected
	private static List<Transform> chains;      //

	private Transform playerTransform;          //saves player's transform
	private int chainIndex = 0;                 //saves chains index on which player is connected
	private Collider2D[] colliders;             //saves all colliders of player
	private PlayerMove pControl;             //used for enabling/disabling PlayerControl script
	private Animator anim;                      //used for playing animations

	private bool onRope = false;
	private float timer = 0.0f;
	private float direction;

	Vector2 jumpForce;
	public float jumpX;
	public float jumpY;

	bool disenable;
	float delayTime = 1.0f;


	// Use this for initialization
	void Start()
	{
		//get player's components
		playerTransform = transform;
		colliders = GetComponentsInChildren<Collider2D>();//自身のみならず子のコンポネントまでコライダーを獲得
		pControl = GetComponent<PlayerMove>();
		anim = GetComponent<Animator>();
		jumpForce = new Vector2(jumpX, jumpY);
	}


	// Update is called once per frame
	void LateUpdate()
	{


		if (onRope)
		{//OnColisionで接触したらOnRopeになるよ
		 //make player's position and rotation same as connected chain's
			playerTransform.position = collidedChain.position;//接触したチェインのトランスフォーム
			playerTransform.localRotation = Quaternion.AngleAxis(direction, Vector3.forward);//接触したチェインの角度にあわせるよう
																								  //ローカルポジションを調整
							//forwardは奥へと進む一本の線。つまりAngleAxisでこれを中心に回転させるので平面的な回転
																								  //if up button is pressed and "chainIndex > 1" (there is another chain above player), climb up
			if (GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction1) > 0 && chainIndex > 1)
			{
				timer += Time.deltaTime;

				if (timer > climbUpInterval)
				{
					ClimbUp();
					timer = 0.0f;
				}
			}

			//if down button is pressed and "chainIndex < 1" (there is another chain below player), climb down
			if (GManager.instance.InputR.GetAxisRaw(MainUICon.instance.rewiredAction1) < 0)
			{
				if (chainIndex < chains.Count - 1)
				{
					timer += Time.deltaTime;

					if (timer > climbUpInterval)
					{
						ClimbDown();
						timer = 0.0f;
					}
				}
				else
					JumpOff(); //if there isn't chain below player, jump from rope
			}

			//if jump button is pressed, jump from rope
			if (GManager.instance.InputR.GetButtonDown(MainUICon.instance.rewiredAction5))
			{
				JumpOff();
				pControl.RopeJump();
				//this.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
			}

			// Cache the horizontal input.
			float H = Input.GetAxis("Horizontal");

			if (H > 0 && !pControl.isRight)
				// ... flip the player.
				pControl.Flip();
			//プレーヤーオブジェクトが向いてる向きの反対の入力で向き変わる
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (H < 0 && pControl.isRight)
				// ... flip the player.
				pControl.Flip();

			//add swing force to connected chain
			//チェインに力加える
			collidedChain.GetComponent<Rigidbody2D>().AddForce(Vector2.right * H * swingForce);
		}
	}


	void ClimbDown()
	{
		//get all HingeJoint2D components from chain below the player
		var joint = chains[chainIndex + 1].GetComponent<HingeJoint2D>();
		//今のチェインの一つ先に移動

		//if chain has HingeJoint2D but isn't enabled jump from rope
		if (joint == null || !joint.enabled)
		{
			//JumpOff();
			return;
		}

		//connect player to below chain
		//今のチェインの一つ先の子オブジェクトに
		//動くたびに更新
		collidedChain = chains[chainIndex + 1];
		playerTransform.parent = collidedChain;
		chainIndex++;
	}


	void ClimbUp()
	{
		//get all HingeJoint2D components from chain above the player
		var joint = chains[chainIndex - 1].GetComponent<HingeJoint2D>();
		//今のチェインの一つ上のチェインのHingeJointを獲得

		//if chain has HingeJoint2D but isn't enabled don't do anything
		//HingeJointが機能してないなら戻れ
		if (joint && !joint.enabled)
			return;

		//connect player to above chain
		//今のチェインの一つ上の子オブジェクトに
		//動くたびに更新
		collidedChain = chains[chainIndex - 1];
		playerTransform.parent = collidedChain;
		chainIndex--;
	}


	void JumpOff()//ジャンプやめるメソッド
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;    //reset velocity
		playerTransform.parent = null;          //改めて親を決めるためにいったん切る
		onRope = false;
		pControl.enabled = true;                //activate PlayerControl script

		foreach (var col in colliders)
		{
			//プレイヤーコライダーをすべて再起動
			col.enabled = true;
		}

		disenable = true;

	}

	void FixedUpdate()
    {
        if (disenable)
        {
			delayTime += Time.fixedDeltaTime;

        }

    }


	//チェインと接触した時に起動
	IEnumerator OnCollisionEnter2D(Collision2D coll)
	{//返り値はイテレータのコレクション
		//接地していなくてかつロープタグと衝突したら
		if (/*!pControl.isGround && coll.gameObject.tag == "Rope" &&*/ delayTime >= delayBeforeSecondHang)
		{
			delayTime = 0.0f;
			disenable = false;

			//collのゲームオブジェクトはロープ2Dタグが付いてるやつ
			var joint = coll.gameObject.GetComponent<HingeJoint2D>(); //get HingeJoint2D component from collided object

			if (joint && joint.enabled)
			{
				pControl.AllStop();
				pControl.enabled = false;   //プレーヤーコントローラーを停止
				anim.SetFloat("Speed", 0);  //stop animation

				//disable all player colliders
				foreach (var col in colliders)
					col.enabled = false;
				//プレイヤーコライダーをすべて停止

				var chainsParent = coll.transform.parent;   //get collided object's parent
															//衝突したオブジェクトの親を獲得
				chains = new List<Transform>();//チェインを格納するメモリ確保

				//チェインリストに格納
				foreach (Transform child in chainsParent)//さっき獲得した親のすべての子オブジェクトをリストに
														 //子供から親を見つけ親からすべての兄弟を見つけるような流れ
					chains.Add(child);

				//connect player to collided object
				collidedChain = coll.transform;//プレイヤーオブジェクトに接触したチェインのトランスフォームを獲得
				chainIndex = chains.IndexOf(collidedChain);//全ての鎖の数、兄弟の数の中で衝突した鎖が何番目なのかを記録
				playerTransform.parent = collidedChain;//プレイヤーオブジェクトの親に接触したチェインを設定
				onRope = true;
				//ロープにぶら下がっています

				direction = Mathf.Sign(Vector3.Dot(-collidedChain.right, Vector3.up));//チェインオブジェクトがどちらを向いているかどうか。
			}                                     //符号でわかる
		}
		return null;
	}


}