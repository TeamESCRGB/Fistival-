using System;
using UnityEngine;

namespace Coordinator
{
    public abstract class MobActBase : MonoBehaviour
    {
        protected Action _onActEnd;
        protected Coroutine _routine;
        protected Animator _animator;
        public virtual void Init(Action onActionEnd, Animator animator)
        {
            _onActEnd = onActionEnd;
            _routine = null;
            _animator = animator;
        }

        public abstract void StopAct();

        public abstract void Act();
    }
}