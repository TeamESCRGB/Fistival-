using Unity.Mathematics;
using UnityEngine;

namespace Actor
{
    public class ParallaxInfinityBGActor : MonoBehaviour
    {
        [SerializeField]
        private bool _activateXInfinity = true;
        [SerializeField]
        private bool _activateYInfinity = true;
        private Transform _mainCam;
        private Vector3 _camLastPos;
        [SerializeField]
        private float _followRate = 0;
        private Vector2 _textureUnitSize;
        private Vector2 _camSiz;

        private void Start()
        {
            _mainCam = Camera.main.transform;
            _camLastPos = _mainCam.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            _textureUnitSize.x = sprite.rect.width / sprite.pixelsPerUnit * transform.lossyScale.x;
            _textureUnitSize.y = sprite.rect.height / sprite.pixelsPerUnit * transform.lossyScale.y;
            _camSiz.y = Camera.main.orthographicSize * 2;
            _camSiz.x = _camSiz.y * Camera.main.aspect;
        }


        private void LateUpdate()
        {
            Vector3 camPos = _mainCam.position;
            Vector3 delta = camPos - _camLastPos;
            transform.position += new Vector3(delta.x * _followRate, delta.y * _followRate, 0);
            _camLastPos = camPos;
            Vector3 myPos = transform.position;

            Vector2 camSiz = _camSiz / 2;
            Vector2 mySiz = _textureUnitSize / 2;

            float myRight = myPos.x + mySiz.x;
            float myLeft = myPos.x - mySiz.x;
            float myUp = myPos.y + mySiz.y;
            float myDown = myPos.y - mySiz.y;

            float camRight = _camLastPos.x + camSiz.x;
            float camLeft = _camLastPos.x - camSiz.x;
            float camUp = _camLastPos.y + camSiz.y;
            float camDown = _camLastPos.y - camSiz.y;

            if(_activateXInfinity)
            {
                if (myRight < camLeft)//내 오른쪽과 캠의 왼쪽을 비교
                {
                    myPos.x = _camLastPos.x + (mySiz.x + camSiz.x) - (camLeft - myRight);
                }
                else if (myLeft > camRight)//내 왼쪽과 캠의 오른쪽을 비교
                {
                    myPos.x = _camLastPos.x - (mySiz.x + camSiz.x) + (myLeft - camRight);
                }
            }

            if(_activateYInfinity)
            {
                if (myUp < camDown)//내 위와 캠의 아래를 비교
                {
                    myPos.y = _camLastPos.y + (mySiz.y + camSiz.y) - (camDown - myUp);
                }
                else if (myDown > camUp)//내 아래와 캑의 위를 비교
                {
                    myPos.y = _camLastPos.y - (mySiz.y + camSiz.y) + (myDown - camUp);
                }
            }

            transform.position = myPos;
        }
    }
}