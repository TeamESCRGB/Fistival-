using Defines;

namespace Coordinator.Rhythm
{
    public interface ILateRhythmReceiver
    {
        public void OnLateBPM(int idx, NoteTypes noteType);
    }
}
