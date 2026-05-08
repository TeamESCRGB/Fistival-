using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Coordinator.Movements
{
    public interface IPushable
    {
        public void PushTo(Vector2 force);
    }
}
