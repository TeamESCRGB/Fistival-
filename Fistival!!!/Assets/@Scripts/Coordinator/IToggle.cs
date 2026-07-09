using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator
{
    public interface IToggle
    {
        public void Init(bool initialState);
        public bool GetState();
        public void SetState(bool state);
        public void RegisterOnStateChanged(Action<bool> callback);
        public void UnregisterOnStateChanged(Action<bool> callback);
    }
}
