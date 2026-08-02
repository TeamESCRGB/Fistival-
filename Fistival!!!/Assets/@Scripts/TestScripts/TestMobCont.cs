
using Coordinator.Movements;
using UnityEngine;

namespace Assets._Scripts.TestScripts
{
    public class TestMobCont : MonoBehaviour, IStunnable, IPushable
    {
        public Vector2 fo;
        public Rigidbody2D rb;
        public void PushTo(Vector2 force)
        {
            rb.AddForce(force,ForceMode2D.Impulse);
            Debug.Log(force);
        }

        [ContextMenu("asdf")]
        public void f()
        {
            rb.AddForce(fo, ForceMode2D.Impulse);
        }

        public void ReleaseStun()
        {
            Debug.Log("스턴해제");
        }

        public void StunFor(float time)
        {
            Debug.Log("스턴");
        }
    }
}
