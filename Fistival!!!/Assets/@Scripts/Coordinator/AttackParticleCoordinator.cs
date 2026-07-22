using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class AttackParticleCoordinator : MonoBehaviour
    {
        private Dictionary<string, ParticleSystem> _particles = new Dictionary<string, ParticleSystem>();


        private void Awake()
        {
            var particles = GetComponentsInChildren<ParticleSystem>();    
            for(int i = 0; i < particles.Length; i++)
            {
                _particles[particles[i].name] = particles[i];
            }
        }

        public void Play(string particleName, float rotation)
        {
            var p =_particles[particleName];
            var prop = p.main;
            prop.startRotation= new ParticleSystem.MinMaxCurve(rotation * Mathf.Deg2Rad);
            p.Stop();
            p.Play();
        }
    }
}