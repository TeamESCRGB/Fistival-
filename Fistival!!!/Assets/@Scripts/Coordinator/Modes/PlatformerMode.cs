using Data;
using Defines;

namespace Coordinator.Modes
{
    public class PlatformerMode : ModeBase
    {
        public override ModeTypes ModeType => ModeTypes.PLATFORMER;
        private void Awake()
        {
            
        }

        public override void Init(CommonModeData data)
        {
            base.Init(data);

        }

        public override void DeInit()
        {

            base.DeInit();
        }
    }
}
