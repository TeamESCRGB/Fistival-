using Defines;

namespace Coordinator.Rhythm
{
    public interface IParryResultReceiver
    {
        public void OnParry(bool isSuccess,IParrableObject parrableObject ,NoteTypes noteType);
    }
}
