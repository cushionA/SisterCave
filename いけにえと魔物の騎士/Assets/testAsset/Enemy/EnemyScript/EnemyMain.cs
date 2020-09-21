using UnityEngine;
using System.Collections;


public class EnemyMain : MonoBehaviour
{

	// === 外部パラメータ（インスペクタ表示） =====================
	public bool cameraSwitch = true;
	public bool inActiveZoneSwitch = false;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public bool cameraEnabled = false;
	[System.NonSerialized] public bool inActiveZone = false;


	// === キャッシュ ==========================================
	protected EnemyBase eb;
	protected GameObject player;
	protected PlayerMove pm;
	protected Rigidbody2D rb;

	// === 内部パラメータ ======================================


	// === コード（Monobehaviour基本機能の実装） ================
	public virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		eb = GetComponent<EnemyBase>();
		player = eb.player;
		pm = player.GetComponent<PlayerMove>();
	}



	public virtual void Update()
	{
		cameraEnabled = false;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		inActiveZone = true;
    }



    // === コード（基本AI動作処理） =============================

}
