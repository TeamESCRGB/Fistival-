using ComponentModule;
using Coordinator.Movements;
using Defines;
using Manager;
using System;
using UnityEngine;
using Utils;

namespace Coordinator.Victims
{
    public class PlayerVictimCoordinator : MonoBehaviour, IAttackable
    {
        private IPushable _pushable;
        private IStunnable _internalTarget;
        private HPCoordinator _hpCoord;
        private CooldownComponentModule _invincibilityTimeCounter = null;
        private int _maskedLayer = 0;
        private bool _isAttackableOn;
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponentInParent<Animator>();
            _hpCoord = gameObject.GetOrAddComponent<HPCoordinator>();
            _internalTarget = transform.parent.GetComponentInParent<IStunnable>();
            transform.parent.Find("@ModeManageObject").GetComponent<ModeManageCoordinator>().OnModeChanged += OnModeChanged;
        }

        public void Init(int hp, int maxHP, float invincibilityTime)
        {
            _isAttackableOn = true;
            _maskedLayer = 1 << gameObject.layer;
            _hpCoord.Init(hp, maxHP);
            _invincibilityTimeCounter = Managers.Instance.CooldownManager.GetCooldownModule(invincibilityTime);
            _animator.SetBool("IsDead", false);
        }
        public VictimType GetVictimType()
        {
            return VictimType.Player;
        }
        public void SetAttackableState(bool canAttack)
        {
            _isAttackableOn= canAttack;
        }

        public bool IsAttackableStateOn()
        {
            return _isAttackableOn;
        }

        private void OnModeChanged(ModeBase target)
        {
            _internalTarget = target;
            _pushable = target.GetComponentInChildren<IPushable>();
        }

        private void OnDisable()
        {
            if(Managers.Instance != null && _invincibilityTimeCounter is not null)
            {
                Managers.Instance.CooldownManager.ReturnModule(_invincibilityTimeCounter);
            }
            _invincibilityTimeCounter = null;
        }

        public void SetMaxHP(int maxHP)
        {
            _hpCoord.SetMaxHP(maxHP);
        }

        public void Respawn()
        {
            SetAttackableState(true);
            _hpCoord.Respawn();
            _animator.Rebind();
        }

        public bool CanAttack()
        {
            if (_isAttackableOn == false || _invincibilityTimeCounter is null)
            {
                return false;
            }
            return (_hpCoord.IsDead() == false) && _invincibilityTimeCounter.IsCooldownEnded();
        }
        [Obsolete("아직 구현 완성 안됨")]
        public T RequestComponent<T>() where T : class
        {
            //나중에 기획 더 나오면 자주 쓰이는 컴포넌트들은 미리 내부에 저장해두고, 그게 아닌것들은 다른 경로에서 가져오도록 코드 짜둘 것
            //솔직히 이거 자체가 solid위반이긴 한데, 그렇게 하기에는 성능이 너무 떨어질 가능성이 높음
            //여전히
            return GetComponent<T>();
        }

        public void TakeDamage(int damage, Vector3 attackerPos, bool showHitEffect)
        {
            if (damage < 0)
            {
                return;
            }
            _hpCoord.SubtractHP(damage);
            if(_hpCoord.IsDead())
            {
                _animator.SetBool("IsDead", true);
                _animator.SetTrigger("Dead");
            }
            else
            {
                _animator.SetTrigger("Hit");
            }
            Managers.Instance.StageManager.TakeDamage(damage);//이벤트로 하려고 했는데, 체력 까인거 이펙트 띄우는건 더 밑에 HPCoord에서 할거기도 하고, 이건 딱 거기서밖에 안쓸거같아서 일단 이렇게 함
        }

        public void StartInvincibleTime()
        {
            if (_invincibilityTimeCounter is null)
            {
                return;
            }
            _invincibilityTimeCounter.StartCooldown();
        }

        public int GetMaskedLayer()
        {
            return _maskedLayer;
        }

        public void StunFor(float time)
        {
            _internalTarget?.StunFor(time);
        }

        public void ReleaseStun()
        {
            _internalTarget?.ReleaseStun();
        }

        public void TakeKnockBack(Vector2 force)
        {
            _pushable?.PushTo(force);
        }
    }
}
