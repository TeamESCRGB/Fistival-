using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class AttackParticleCoordinator : MonoBehaviour
    {
        private Dictionary<string, ParticleSystem> _particles = new Dictionary<string, ParticleSystem>();


        public void RegisterParticle(string particleName)
        {
            var particle = gameObject.GetChild<ParticleSystem>(particleName, true, true);
            _particles[particleName] = particle;
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