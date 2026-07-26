using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Trigger
{
    public class MobBGMTrigger : BGMTriggerBase
    {
        private void Start()
        {
            GetComponentInChildren<HPCoordinator>().SubscribeOnDead(Play);
        }
    }
}
