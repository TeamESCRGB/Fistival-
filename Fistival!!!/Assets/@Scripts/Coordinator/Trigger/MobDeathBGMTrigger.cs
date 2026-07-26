using System;
using System.Collections.Generic;
using System.Text;

namespace Coordinator.Trigger
{
    /// <summary>
    /// 몹이 죽으면 그때부터 나올 bgm트리거
    /// 
    /// </summary>
    public class MobDeathBGMTrigger : BGMTriggerBase
    {
        private void Start()
        {
            GetComponentInChildren<HPCoordinator>().SubscribeOnDead(Play);
        }
    }
}
