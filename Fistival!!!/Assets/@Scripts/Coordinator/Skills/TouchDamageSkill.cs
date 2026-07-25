using Coordinator.Victims;
using Manager;
using UnityEngine;

namespace Coordinator.Skills
{
    public class TouchDamageSkill : SkillCoordinatorBase
    {
        private void OnDisable()
        {
            ResetOnAttack();
        }

        protected float _knockbackForce = 1;
        protected bool _usePhys2d;
        protected bool _useOverlapBox;

        public void Init(int attackableLayers, int baseDamage, float baseStunTime, float knockbackForce, bool usePhys2d=false, bool useOverlapBox=true)
        {
            _useOverlapBox= useOverlapBox;
            _usePhys2d= usePhys2d;
            _knockbackForce=knockbackForce;
            base.Init(attackableLayers, baseDamage, baseStunTime);
        }

        protected virtual void Attack(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            if (((1 << go.layer) & _attackableLayers) == 0 || go.TryGetComponent<IAttackable>(out var comp) == false)
            {
                return;
            }

            var knockBackForce = _baseDamage * _knockbackForce;
            var force = Mathf.Sign(transform.right.x) * knockBackForce;
            Managers.Instance.AttackManager.RequestAttack(comp, this, _baseDamage, new Vector2(force, knockBackForce), _baseStunTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(_usePhys2d)
            {
                Attack(collision.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_usePhys2d)
            {
                Attack(collision.gameObject);
            }
        }

        private void Update()
        {
            if(_useOverlapBox==false)
            {
                return;
            }
            var col = Physics2D.OverlapBox(transform.position, transform.lossyScale, transform.eulerAngles.z, _attackableLayers);//이거 각을 eularangle.z로 줘야함
            if(col == null)
            {
                return;
            }
            Attack(col.gameObject);
        }


        public override bool Act(IAttackable target, int calculatedDamage, Vector2 knockback, float stun)
        {
            if (target.CanAttack())
            {
                target.TakeDamage(calculatedDamage,transform.position, true);
                target.TakeKnockBack(knockback);
                target.StunFor(stun);
                target.StartInvincibleTime();
            }
            return true;
        }
    }
}