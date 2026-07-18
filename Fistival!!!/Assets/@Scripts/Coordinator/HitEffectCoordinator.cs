using Defines;
using Manager;
using System.Collections;
using UnityEngine;

namespace Coordinator
{
    public class HitEffectCoordinator : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _lowParticle;
        [SerializeField]
        private ParticleSystem _middleParticle;
        [SerializeField]
        private ParticleSystem _highParticle;
        private WaitForSeconds _waiter;

        private void Start()
        {
            if(_waiter == null)
            {
                _waiter = new WaitForSeconds(_lowParticle.main.startLifetime.constant);
            }
        }

        public void Show(int damage)
        {
            StartCoroutine(ShowRoutine(damage));
        }

        private IEnumerator ShowRoutine(int damage)
        {
            if(damage < (int)HitEffectRange.LowHitEffectMaxExculsiveDmg)
            {
                _lowParticle.Stop();
                _lowParticle.Play();
            }
            else if(damage < (int)HitEffectRange.MiddleEffectMaxExculsiveDmg)
            {
                _middleParticle.Stop();
                _middleParticle.Play();
            }
            else
            {
                _highParticle.Stop();
                _highParticle.Play();
            }
            yield return _waiter;
            Managers.Instance.ResourceManager.Destroy(gameObject);
        }
    }
}
