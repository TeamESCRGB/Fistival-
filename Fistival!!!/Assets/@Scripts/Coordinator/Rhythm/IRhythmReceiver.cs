using Defines;

namespace Coordinator.Rhythm
{
    public interface IRhythmReceiver
    {
        public void OnExactBPM(int idx, NoteTypes noteType);
        public void OnLateBPM(int idx, NoteTypes noteType);
    }
}