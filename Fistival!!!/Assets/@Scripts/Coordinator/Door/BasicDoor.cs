using Assets._Scripts.Coordinator;
using UnityEngine;

namespace Coordinator.Door
{
    public class BasicDoor : MonoBehaviour, IDoor, IBasicInitializer
    {
        [SerializeField]
        private Sprite _openSprite;
        [SerializeField]
        private Sprite _closeSprite;
        [SerializeField]
        private GameObject _barrierObj;

        [SerializeField]
        private bool _isDefaultOpened;
        private bool _isOpen;
        private Animator _anim;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            _anim.Play("Init");
            if(_isDefaultOpened)
            {
                OpenAnimEnd();
            }
            else
            {
                CloseAnimEnd();
            }
        }
        public void Open()
        {
            _anim.Play("Open");
        }

        public void OpenAnimEnd()
        {
            _barrierObj.SetActive(false);
            _isOpen = true;
            _renderer.sprite = _openSprite;
        }

        public void Close()
        {
            _anim.Play("Close");
        }

        public void CloseAnimEnd()
        {
            _barrierObj.SetActive(true);
            _isOpen = false;
            _renderer.sprite = _closeSprite;
        }

        public bool IsOpen()
        {
            return _isOpen;
        }
    }
}
