using UnityEngine;

namespace Coordinator.Door
{
    public interface IDoor
    {
        public void Init();
        public bool IsOpen();
        public void Open();
        public void OpenAnimEnd();
        public void Close();
        public void CloseAnimEnd();
    }
}