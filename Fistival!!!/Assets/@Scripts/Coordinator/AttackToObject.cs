using Coordinator.Victims;
using Manager;
using System;
using UnityEngine;

namespace Coordinator
{
    public class AttackToObject : MonoBehaviour
    {
        private float _progress=10f;
        private float _elapsedTime;
        private float _druation;
        private float _value;
        private bool _isObjectized;
        private Rigidbody2D _rb2d;
        private Transform _checkBox;
        private LayerMask _layer;

        private SkillCoordinatorBase _skill;
        private int _damage;
        private float _stun;

        public event Action OnObjectized;
        public event Action OnReturnStart;

        private bool _isReturnStarted;

        private void Awake()
        {
            _checkBox = transform.Find("@AttackToObjectCheckbox");
            _rb2d = GetComponent<Rigidbody2D>();
            _skill = GetComponentInChildren<SkillCoordinatorBase>();
        }

        public void Init(float duration, float value, LayerMask layer, int damage, float stun)
        {
            OnReturnStart = null;
            OnObjectized = null;
            _layer=layer;
            _isReturnStarted = false;
            _isObjectized = false;
            _druation = duration;
            _value = value;
            _damage = damage;
            _stun = stun;
            _rb2d.gravityScale = 0;
        }
        
        private void OnDisable()
        {
            DisableComponentData();
            OnObjectized = null;
            OnReturnStart = null;
        }

        private void DisableComponentData()
        {
            _layer = 0;
            _isObjectized = false;
            _rb2d.gravityScale = 1;
        }

        private void Objectized()
        {
            DisableComponentData();
            OnObjectized?.Invoke();
            OnObjectized = null;
            OnReturnStart = null;
            _isObjectized = true;
        }

        public void Launch()
        {
            _progress = 0;
            _elapsedTime = 0;
        }

        private void OnCollisionToWall(GameObject go)
        {
            if (_isObjectized)
            {
                return;
            }

            _isObjectized = true;

            if (((1 << go.layer) & _layer) != 0)
            {

                if (go.TryGetComponent<IAttackable>(out var comp))
                {
                    Managers.Instance.AttackManager.RequestAttack(comp, _skill, _damage, _rb2d.linearVelocity, _stun);
                }
            }
            _rb2d.gravityScale = 1;
            Objectized();
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionToWall(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnCollisionToWall(collision.gameObject);
        }

        private void FixedUpdate()
        {
            if(_isObjectized)
            {
                return;
            }

            if(_rb2d.bodyType == RigidbodyType2D.Kinematic)
            {
                _isObjectized = true;
                _rb2d.gravityScale = 1;
                Objectized();
                return;
            }

            var result = Physics2D.OverlapBox(_checkBox.position, _checkBox.lossyScale, 0, _layer);

            if (result != null)
            {
                _isObjectized = true;
                if (result.gameObject.TryGetComponent<IAttackable>(out var comp))
                {
                    Managers.Instance.AttackManager.RequestAttack(comp, _skill, _damage, _rb2d.linearVelocity, _stun);
                }
                _rb2d.gravityScale = 1;
                Objectized();
                return;
            }

            if (_progress > 1.0f)
            {
                return;
            }

            _elapsedTime += Time.fixedDeltaTime;
            _progress = _elapsedTime / _druation; // 0 ~ 1 진행도

            float mappedX = Mathf.Lerp(-_value, _value, _progress);
            float targetVelocityX = mappedX * mappedX;

            if(_isReturnStarted == false && _progress >= 0.5f)
            {
                _isReturnStarted = true;
                OnReturnStart?.Invoke();
            }

            if (mappedX < 0)
            {
                targetVelocityX = -targetVelocityX;
            }

            _rb2d.linearVelocity = new Vector2(targetVelocityX, _rb2d.linearVelocity.y);
        }
    }
}