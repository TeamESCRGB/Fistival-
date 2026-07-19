using Manager;
using UnityEngine;
using Utils;

namespace Coordinator
{
    public class StageChangeTrigger : MonoBehaviour
    {
        [SerializeField]
        protected LayerMask _playerLayer;
        [SerializeField]
        protected string _chunkKey;
        [SerializeField]
        protected string _chunkResourceKey;
        [SerializeField]
        protected Transform _nextChunkSpawnPoint;

        protected virtual void InternalTriggerEnterHandler(Collider2D collision)
        {
            if ((1 << collision.gameObject.layer) != _playerLayer)
            {
                return;
            }
            Managers.Instance.ResourceManager.LoadAsyncAllIn(_chunkResourceKey, (_, now, max) =>
            {
                var chunk = Managers.Instance.StageManager.TrySpawnChunk(_chunkKey, _chunkResourceKey, _nextChunkSpawnPoint.position);
                if (chunk == null)
                {
                    return;
                }
                var pos = chunk.gameObject.GetChildGameObject("@PlayerTeleportPoint").transform.position;
                collision.gameObject.transform.position = pos;
                var cam = Camera.main.transform;
                var camPos = cam.transform.position;
                camPos.x = pos.x;
                camPos.y = pos.y;
                cam.transform.position = camPos;
            });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            InternalTriggerEnterHandler(collision);
        }
    }
}