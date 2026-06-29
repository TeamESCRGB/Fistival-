using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator
{
    public abstract class MobActBase
    {
        public Action _onActEnd;
        public virtual void Init(Action onActionEnd)
        {
            _onActEnd = onActionEnd;
        }
        public abstract void Act();
    }
}