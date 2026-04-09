using Defines;

namespace Coordinator.Rhythm
{
    public interface IExactRhythmReceiver
    {
        public void OnExactBPM(int idx, NoteTypes noteType);
    }
}