using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator
{
    public abstract class MobActBase : MonoBehaviour
    {
        protected Action _onActEnd;
        protected Coroutine _routine;
        public virtual void Init(Action onActionEnd)
        {
            _onActEnd = onActionEnd;
            _routine = null;
        }

        public abstract void StopAct();

        public abstract void Act();
    }
}