using Data;
using Manager;
using UnityEngine;

namespace Coordinator.NPC
{
    public abstract class EndingNPCBase : MonoBehaviour
    {
        [SerializeField]
        protected string _talkName;
        protected Animator _animator;
        private void Awake()
        {
            OnAwake();   
        }

        protected virtual void OnAwake()
        {
            _animator = GetComponent<Animator>();
        }

        public abstract void Init();
        public abstract void OnTalkEnd();
    }
}