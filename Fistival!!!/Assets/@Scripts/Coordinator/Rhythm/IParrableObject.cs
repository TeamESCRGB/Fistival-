using Defines;

namespace Coordinator.Rhythm
{
    public interface IParrableObject
    {
        public bool IsParrySuccess(int idx, NoteTypes noteType);
    }
}