using UnityEngine;

namespace Actor
{
    public class ParallaxBGActor : MonoBehaviour
    {
        private Transform _mainCam;
        private Vector3 _camLastPos;
        [SerializeField]
        private float _followRate = 0;

        private void Start()
        {
            _mainCam = Camera.main.transform;
            _camLastPos = _mainCam.position;
        }

        private void LateUpdate()
        {
            Vector3 camPos = _mainCam.position;
            Vector3 delta = camPos - _camLastPos;
            transform.position += new Vector3(delta.x * _followRate, delta.y * _followRate, transform.position.z);
            _camLastPos = camPos;
        }
    }
}