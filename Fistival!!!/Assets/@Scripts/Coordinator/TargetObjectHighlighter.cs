using UnityEngine;

namespace Coordinator
{
    public class TargetObjectHighlighter : HIghlighterBase
    {
        [SerializeField]
        private LayerMask _targeterLayer;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(((1 << collision.gameObject.layer) & _targeterLayer) != 0)
            {
                _renderer.sharedMaterial = _on;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & _targeterLayer) != 0)
            {
                _renderer.sharedMaterial = _off;
            }
        }
    }
}