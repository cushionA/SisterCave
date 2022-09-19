using UnityEngine.AddressableAssets;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EffectBreak : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem pm;
    float lifeTime;

    

    [SerializeField]

    private void Start()
    {
        pm = this.gameObject.GetComponent<ParticleSystem>();

    }

    private void FixedUpdate()
    {


        lifeTime += Time.fixedDeltaTime;
        if(lifeTime >= pm.main.duration)
        {
            lifeTime = 0;
            Addressables.ReleaseInstance(this.gameObject);
        }
    }

    /*  private void OnParticleSystemStopped()
      {
          Debug.Log("‚³‚»‚è");
          Addressables.ReleaseInstance(this.gameObject);
      }*/

}
