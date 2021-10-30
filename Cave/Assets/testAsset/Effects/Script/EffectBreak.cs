using UnityEngine.AddressableAssets;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EffectBreak : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem pm;
    float lifeTime;
    [SerializeField]
    [Tooltip("í‚é~ÇµÇΩÇ‹Ç‹Ç…Ç∑ÇÈÇ©")]
    bool isStop;
    Vector2 startPosi;

    private void Start()
    {
        pm = this.gameObject.GetComponent<ParticleSystem>();
        if (isStop)
        {
            startPosi.Set(transform.position.x,transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        if (isStop)
        {
            gameObject.transform.position = startPosi;
        }

        lifeTime += Time.fixedDeltaTime;
        if(lifeTime >= pm.main.duration)
        {
            lifeTime = 0;
            Addressables.ReleaseInstance(this.gameObject);
        }
    }

    /*  private void OnParticleSystemStopped()
      {
          Debug.Log("Ç≥ÇªÇË");
          Addressables.ReleaseInstance(this.gameObject);
      }*/

}
