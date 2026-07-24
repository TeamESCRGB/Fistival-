using Manager;
using UI.Popup;

namespace Coordinator.NPC
{
    public class PlatformerBossNPC : EndingNPCBase
    {
        public override void Init()
        {
            _animator.Play("EndAnimation");
        }

        public void OnAnimatorEnd()
        {
            Managers.Instance.UIManager.ShowPopupUI<TalkCutScenePopup>("TalkCutScenePopup").SetData(_talkName).SetOnEnd(OnTalkEnd);
        }

        public override void OnTalkEnd()
        {
            Managers.Instance.StageManager.OnClear();
        }
    }
}
